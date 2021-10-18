using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Enums;
using masz.Events;
using masz.Exceptions;
using masz.Models;
using Microsoft.Extensions.Logging;

namespace masz.Repositories
{

    public class ModCaseRepository : BaseRepository<ModCaseRepository>
    {
        private readonly DiscordUser _currentUser;
        private ModCaseRepository(IServiceProvider serviceProvider, DiscordUser currentUser) : base(serviceProvider)
        {
            _currentUser = currentUser;
        }
        private ModCaseRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _currentUser = _discordAPI.GetCurrentBotInfo(CacheBehavior.Default);
        }
        public static ModCaseRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new ModCaseRepository(serviceProvider, identity.GetCurrentUser());
        public static ModCaseRepository CreateWithBotIdentity(IServiceProvider serviceProvider) => new ModCaseRepository(serviceProvider);
        public async Task<ModCase> CreateModCase(ModCase modCase, bool handlePunishment, bool sendPublicNotification, bool sendDmNotification)
        {
            DiscordUser currentReportedUser = await _discordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.IgnoreButCacheOnError);

            GuildConfig guildConfig;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(modCase.GuildId);
            } catch (ResourceNotFoundException) {
                throw new UnregisteredGuildException(modCase.GuildId);
            }

            if (currentReportedUser == null)
            {
                _logger.LogError("Failed to fetch modcase suspect.");
                throw new InvalidDiscordUserException(modCase.ModId);
            }
            if (currentReportedUser.IsBot)
            {
                _logger.LogError("Cannot create cases for bots.");
                throw new ProtectedModCaseSuspectException("Cannot create cases for bots.", modCase).WithError(APIError.ProtectedModCaseSuspectIsBot);
            }
            if (_config.GetSiteAdmins().Contains(currentReportedUser.Id))
            {
                _logger.LogInformation("Cannot create cases for site admins.");
                throw new ProtectedModCaseSuspectException("Cannot create cases for site admins.", modCase).WithError(APIError.ProtectedModCaseSuspectIsSiteAdmin);
            }

            modCase.Username = currentReportedUser.Username;
            modCase.Discriminator = currentReportedUser.Discriminator;

            DiscordMember currentReportedMember = await _discordAPI.FetchMemberInfo(modCase.GuildId, modCase.UserId, CacheBehavior.IgnoreButCacheOnError);
            if (currentReportedMember != null)
            {
                if (currentReportedMember.Roles.Where(x => guildConfig.ModRoles.Contains(x.Id)).Any() ||
                    currentReportedMember.Roles.Where(x => guildConfig.AdminRoles.Contains(x.Id)).Any())
                {
                    _logger.LogInformation("Cannot create cases for team members.");
                    throw new ProtectedModCaseSuspectException("Cannot create cases for team members.", modCase).WithError(APIError.ProtectedModCaseSuspectIsTeam);
                }
                modCase.Nickname = currentReportedMember.Nickname;
            }

            modCase.CaseId = await _database.GetHighestCaseIdForGuild(modCase.GuildId) + 1;
            modCase.CreatedAt = DateTime.UtcNow;
            if (modCase.OccuredAt == null)
            {
                modCase.OccuredAt = modCase.CreatedAt;
            } else
            {
                modCase.OccuredAt = modCase.CreatedAt;
            }
            modCase.ModId = _currentUser.Id;
            modCase.LastEditedAt = modCase.CreatedAt;
            modCase.LastEditedByModId = _currentUser.Id;
            if (modCase.Labels != null)
            {
                modCase.Labels = modCase.Labels.Distinct().ToArray();
            } else
            {
                modCase.Labels = new string[0];
            }
            modCase.Valid = true;
            if (modCase.PunishmentType == PunishmentType.None || modCase.PunishmentType == PunishmentType.Kick)
            {
                modCase.PunishedUntil = null;
                modCase.PunishmentActive = false;
            } else
            {
                modCase.PunishmentActive = modCase.PunishedUntil == null || modCase.PunishedUntil > DateTime.UtcNow;
            }

            await _database.SaveModCase(modCase);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeModCaseCreated(new ModCaseCreatedEventArgs(modCase));

            await _discordAnnouncer.AnnounceModCase(modCase, RestAction.Created, _currentUser, sendPublicNotification, sendDmNotification);

            if (handlePunishment && (modCase.PunishmentActive || modCase.PunishmentType == PunishmentType.Kick))
            {
                if (modCase.PunishedUntil == null || modCase.PunishedUntil > DateTime.UtcNow)
                {
                    await _punishmentHandler.ExecutePunishment(modCase);
                }
            }

            return modCase;
        }
        public async Task<ModCase> GetModCase(ulong guildId, int caseId)
        {
            ModCase modCase = await _database.SelectSpecificModCase(guildId, caseId);
            if (modCase == null)
            {
                throw new ResourceNotFoundException($"ModCase with id {caseId} does not exist.");
            }
            return modCase;
        }
        public async Task<ModCase> DeleteModCase(ulong guildId, int caseId, bool forceDelete = false, bool handlePunishment = true, bool announcePublic = true)
        {
            ModCase modCase = await this.GetModCase(guildId, caseId);

            if (forceDelete)
            {
                try
                {
                    _filesHandler.DeleteDirectory(Path.Combine(_config.GetFileUploadPath(), guildId.ToString(), caseId.ToString()));
                } catch (Exception e)
                {
                    _logger.LogError(e, $"Failed to delete files directory for modcase {guildId}/{caseId}.");
                }

                _logger.LogInformation($"Force deleting modCase {guildId}/{caseId}.");
                _database.DeleteSpecificModCase(modCase);
                await _database.SaveChangesAsync();

                await _eventHandler.InvokeModCaseDeleted(new ModCaseDeletedEventArgs(modCase));
            } else {
                modCase.MarkedToDeleteAt = DateTime.UtcNow.AddDays(7);
                modCase.DeletedByUserId = _currentUser.Id;
                modCase.PunishmentActive = false;

                _logger.LogInformation($"Marking modcase {guildId}/{caseId} as deleted.");
                _database.UpdateModCase(modCase);
                await _database.SaveChangesAsync();

                await _eventHandler.InvokeModCaseMarkedToBeDeleted(new ModCaseMarkedToBeDeletedEventArgs(modCase));
            }

            if (handlePunishment)
            {
                try
                {
                    _logger.LogInformation($"Handling punishment for case {guildId}/{caseId}.");
                    await _punishmentHandler.UndoPunishment(modCase);
                }
                catch(Exception e)
                {
                    _logger.LogError(e, $"Failed to handle punishment for modcase {guildId}/{caseId}.");
                }
            }

            try
            {
                await _discordAnnouncer.AnnounceModCase(modCase, RestAction.Deleted, _currentUser, announcePublic, false);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to announce modcase.");
            }
            return modCase;
        }
        public async Task<ModCase> UpdateModCase(ModCase modCase, bool handlePunishment, bool sendPublicNotification)
        {
            DiscordUser currentReportedUser = await _discordAPI.FetchUserInfo(modCase.UserId, CacheBehavior.IgnoreButCacheOnError);
            GuildConfig guildConfig;
            try
            {
                guildConfig = await GuildConfigRepository.CreateDefault(_serviceProvider).GetGuildConfig(modCase.GuildId);
            } catch (ResourceNotFoundException)
            {
                throw new UnregisteredGuildException(modCase.GuildId);
            }

            if (currentReportedUser == null)
            {
                _logger.LogError("Failed to fetch modcase suspect.");
                throw new InvalidDiscordUserException(modCase.ModId);
            }
            if (currentReportedUser.IsBot)
            {
                _logger.LogError("Cannot edit cases for bots.");
                throw new ProtectedModCaseSuspectException("Cannot edit cases for bots.", modCase).WithError(APIError.ProtectedModCaseSuspectIsBot);
            }
            if (_config.GetSiteAdmins().Contains(currentReportedUser.Id))
            {
                _logger.LogInformation("Cannot edit cases for site admins.");
                throw new ProtectedModCaseSuspectException("Cannot edit cases for site admins.", modCase).WithError(APIError.ProtectedModCaseSuspectIsSiteAdmin);
            }

            modCase.Username = currentReportedUser.Username;
            modCase.Discriminator = currentReportedUser.Discriminator;

            DiscordMember currentReportedMember = await _discordAPI.FetchMemberInfo(modCase.GuildId, modCase.UserId, CacheBehavior.IgnoreButCacheOnError);
            if (currentReportedMember != null)
            {
                if (currentReportedMember.Roles.Where(x => guildConfig.ModRoles.Contains(x.Id)).Any() ||
                    currentReportedMember.Roles.Where(x => guildConfig.AdminRoles.Contains(x.Id)).Any())
                {
                    _logger.LogInformation("Cannot create cases for team members.");
                    throw new ProtectedModCaseSuspectException("Cannot create cases for team members.", modCase).WithError(APIError.ProtectedModCaseSuspectIsTeam);
                }
                modCase.Nickname = currentReportedMember.Nickname;
            }

            modCase.LastEditedAt = DateTime.UtcNow;
            modCase.LastEditedByModId = _currentUser.Id;
            modCase.Valid = true;
            if (modCase.PunishmentType == PunishmentType.None || modCase.PunishmentType == PunishmentType.Kick)
            {
                modCase.PunishedUntil = null;
                modCase.PunishmentActive = false;
            } else
            {
                modCase.PunishmentActive = modCase.PunishedUntil == null || modCase.PunishedUntil > DateTime.UtcNow;
            }

            _database.UpdateModCase(modCase);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeModCaseUpdated(new ModCaseUpdatedEventArgs(modCase));

            await _discordAnnouncer.AnnounceModCase(modCase, RestAction.Edited, _currentUser, sendPublicNotification, false);

            if (handlePunishment && (modCase.PunishmentActive || modCase.PunishmentType == PunishmentType.Kick))
            {
                if (modCase.PunishedUntil == null || modCase.PunishedUntil > DateTime.UtcNow)
                {
                    await _punishmentHandler.ExecutePunishment(modCase);
                }
            }
            return modCase;
        }
        public async Task<List<ModCase>> GetCasePagination(ulong guildId, int startPage = 1, int pageSize = 20)
        {
            return await _database.SelectAllModCasesForGuild(guildId, startPage, pageSize);
        }
        public async Task<List<ModCase>> GetCasePaginationFilteredForUser(ulong guildId, ulong userId, int startPage = 1, int pageSize = 20)
        {
            return await _database.SelectAllModcasesForSpecificUserOnGuild(guildId, userId, startPage, pageSize);
        }
        public async Task<List<ModCase>> GetCasesForUser(ulong userId)
        {
            return await _database.SelectAllModCasesForSpecificUser(userId);
        }
        public async Task<List<ModCase>> GetCasesForGuild(ulong guildId)
        {
            return await _database.SelectAllModCasesForGuild(guildId);
        }
        public async Task<List<ModCase>> GetCasesForGuildAndUser(ulong guildId, ulong userId)
        {
            return await _database.SelectAllModcasesForSpecificUserOnGuild(guildId, userId);
        }
        public async Task<int> CountAllCases()
        {
            return await _database.CountAllModCases();
        }
        public async Task<int> CountAllCasesForGuild(ulong guildId)
        {
            return await _database.CountAllModCasesForGuild(guildId);
        }
        public async Task<int> CountAllPunishmentsForGuild(ulong guildId)
        {
            return await _database.CountAllActivePunishmentsForGuild(guildId);
        }
        public async Task<int> CountAllActiveMutesForGuild(ulong guildId)
        {
            return await _database.CountAllActivePunishmentsForGuild(guildId, PunishmentType.Mute);
        }
        public async Task<int> CountAllActiveBansForGuild(ulong guildId)
        {
            return await _database.CountAllActivePunishmentsForGuild(guildId, PunishmentType.Ban);
        }
        public async Task<List<ModCase>> SearchCases(ulong guildId, string searchString)
        {
            List<ModCase> modCases = await _database.SelectAllModCasesForGuild(guildId);
            List<ModCase> filteredModCases = new List<ModCase>();
            foreach (var c in modCases)
            {
                var entry = new ModCaseTableEntry(
                    c,
                    await _discordAPI.FetchUserInfo(c.ModId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(c.UserId, CacheBehavior.OnlyCache)
                );
                if (contains(entry, searchString)) {
                    filteredModCases.Add(c);
                }
            }
            return filteredModCases;
        }
        public async Task<List<ModCase>> SearchCasesFilteredForUser(ulong guildId, ulong userId, string searchString)
        {
            List<ModCase> modCases = await _database.SelectAllModcasesForSpecificUserOnGuild(guildId, userId);
            List<ModCase> filteredModCases = new List<ModCase>();
            foreach (var c in modCases)
            {
                var entry = new ModCaseTableEntry(
                    c,
                    await _discordAPI.FetchUserInfo(c.ModId, CacheBehavior.OnlyCache),
                    await _discordAPI.FetchUserInfo(c.UserId, CacheBehavior.OnlyCache)
                );
                if (contains(entry, searchString)) {
                    filteredModCases.Add(c);
                }
            }
            return filteredModCases;
        }
        public async Task<ModCase> LockCaseComments(ulong guildId, int caseId, DiscordUser moderator)
        {
            ModCase modCase = await GetModCase(guildId, caseId);
            modCase.AllowComments = false;
            modCase.LockedAt = DateTime.UtcNow;
            modCase.LockedByUserId = moderator.Id;

            _database.UpdateModCase(modCase);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeModCaseUpdated(new ModCaseUpdatedEventArgs(modCase));

            return modCase;
        }
        public async Task<ModCase> UnlockCaseComments(ulong guildId, int caseId)
        {
            ModCase modCase = await GetModCase(guildId, caseId);
            modCase.AllowComments = true;
            modCase.LockedAt = null;
            modCase.LockedByUserId = 0;

            _database.UpdateModCase(modCase);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeModCaseUpdated(new ModCaseUpdatedEventArgs(modCase));

            return modCase;
        }
        public async Task<ModCase> RestoreCase(ulong guildId, int caseId)
        {
            ModCase modCase = await GetModCase(guildId, caseId);
            modCase.MarkedToDeleteAt = null;
            modCase.DeletedByUserId = 0;

            _database.UpdateModCase(modCase);
            await _database.SaveChangesAsync();

            await _eventHandler.InvokeModCaseRestored(new ModCaseRestoredEventArgs(modCase));

            try
            {
                _logger.LogInformation($"Handling punishment for case {guildId}/{caseId}.");
                await _punishmentHandler.ExecutePunishment(modCase);
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"Failed to handle punishment for modcase {guildId}/{caseId}.");
            }

            return modCase;
        }
        public async Task<List<DbCount>> GetCounts(ulong guildId, DateTime since)
        {
            return await _database.GetCaseCountGraph(guildId, since);
        }
        public async Task<List<DbCount>> GetPunishmentCounts(ulong guildId, DateTime since)
        {
            return await _database.GetPunishmentCountGraph(guildId, since);
        }
    }
}