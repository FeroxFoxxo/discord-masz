using masz.Extensions;
using masz.Enums;

namespace masz.Translations
{
    public class Translation
    {
        public Language preferredLanguage { get; set; }
        private Translation(Language preferredLanguage = Language.en)
        {
            this.preferredLanguage = preferredLanguage;
        }
        public static Translation Ctx(Language preferredLanguage = Language.en) {
            return new Translation(preferredLanguage);
        }
        public string Features() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Features";
                case Language.de:
                    return "Features";
            }
            return "Features";
        }
        public string LanguageWord() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Language";
                case Language.de:
                    return "Sprache";
            }
            return "Language";
        }
        public string Timestamps() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Timestamps";
                case Language.de:
                    return "Zeitstempel";
            }
            return "Timestamps";
        }
        public string Support() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Support";
                case Language.de:
                    return "Support";
            }
            return "Support";
        }
        public string Punishment() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Punishment";
                case Language.de:
                    return "Bestrafung";
            }
            return "Punishment";
        }
        public string PunishmentUntil() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Punished until";
                case Language.de:
                    return "Bestrafung bis";
            }
            return "Punished until";
        }
        public string Description() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Description";
                case Language.de:
                    return "Beschreibung";
            }
            return "Description";
        }
        public string Labels() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Labels";
                case Language.de:
                    return "Labels";
            }
            return "Labels";
        }
        public string Filename() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Filename";
                case Language.de:
                    return "Dateiname";
            }
            return "Filename";
        }
        public string Message() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Message";
                case Language.de:
                    return "Nachricht";
            }
            return "Message";
        }
        public string UserNote() {
            switch (preferredLanguage) {
                case Language.en:
                    return "UserNote";
                case Language.de:
                    return "Benutzernotiz";
            }
            return "UserNote";
        }
        public string UserNotes() {
            switch (preferredLanguage) {
                case Language.en:
                    return "UserNotes";
                case Language.de:
                    return "Benutzernotizen";
            }
            return "UserNotes";
        }
        public string UserMap() {
            switch (preferredLanguage) {
                case Language.en:
                    return "UserMap";
                case Language.de:
                    return "Benutzerbeziehung";
            }
            return "UserMap";
        }
        public string UserMaps() {
            switch (preferredLanguage) {
                case Language.en:
                    return "UserMaps";
                case Language.de:
                    return "Benutzerbeziehungen";
            }
            return "UserMaps";
        }
        public string UserMapBetween(masz.Models.UserMapping userMap) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"UserMap between {userMap.UserA} and {userMap.UserB}.";
                case Language.de:
                    return $"Benutzerbeziehung zwischen {userMap.UserA} und {userMap.UserB}.";
            }
            return $"UserMap between {userMap.UserA} and {userMap.UserB}.";
        }
        public string NotificationModcaseCreatePublic(masz.Models.ModCase modCase) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created.";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde erstellt.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created.";
        }
        public string NotificationModcaseCreateInternal(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordUser moderator) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde von <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) erstellt.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been created by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
        }
        public string NotificationModcaseUpdatePublic(masz.Models.ModCase modCase) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated.";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde aktualisiert.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated.";
        }
        public string NotificationModcaseUpdateInternal(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordUser moderator) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde von <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) aktualisiert.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been updated by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
        }
        public string NotificationModcaseDeletePublic(masz.Models.ModCase modCase) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted.";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde gelöscht.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted.";
        }
        public string NotificationModcaseDeleteInternal(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordUser moderator) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
                case Language.de:
                    return $"Ein **Vorfall** für <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) wurde von <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}) gelöscht.";
            }
            return $"A **Modcase** for <@{modCase.UserId}> ({modCase.Username}#{modCase.Discriminator}) has been deleted by <@{moderator.Id}> ({moderator.Username}#{moderator.Discriminator}).";
        }
        public string NotificationModcaseCommentsShortCreate() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Comment created";
                case Language.de:
                    return "Kommentar erstellt";
            }
            return "Comment created";
        }
        public string NotificationModcaseCommentsShortUpdate() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Comment updated";
                case Language.de:
                    return "Kommentar aktualisiert";
            }
            return "Comment updated";
        }
        public string NotificationModcaseCommentsShortDelete() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Comment deleted";
                case Language.de:
                    return "Kommentar gelöscht";
            }
            return "Comment deleted";
        }
        public string NotificationModcaseCommentsCreate(DSharpPlus.Entities.DiscordUser actor) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **comment** has been created by <@{actor.Id}>.";
                case Language.de:
                    return $"Ein **Kommentar** wurde von <@{actor.Id}> erstellt.";
            }
            return $"A **comment** has been created by <@{actor.Id}>.";
        }
        public string NotificationModcaseCommentsUpdate(DSharpPlus.Entities.DiscordUser actor) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **comment** has been updated by <@{actor.Id}>.";
                case Language.de:
                    return $"Ein **Kommentar** wurde von <@{actor.Id}> aktualisiert.";
            }
            return $"A **comment** has been updated by <@{actor.Id}>.";
        }
        public string NotificationModcaseCommentsDelete(DSharpPlus.Entities.DiscordUser actor) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **comment** has been deleted by <@{actor.Id}>.";
                case Language.de:
                    return $"Ein **Kommentar** wurde von <@{actor.Id}> gelöscht.";
            }
            return $"A **comment** has been deleted by <@{actor.Id}>.";
        }
        public string NotificationModcaseFileCreate(DSharpPlus.Entities.DiscordUser actor) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **file** has been uploaded by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.de:
                    return $"Eine **Datei** wurde von <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) hochgeladen.";
            }
            return $"A **file** has been uploaded by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
        }
        public string NotificationModcaseFileDelete(DSharpPlus.Entities.DiscordUser actor) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **file** has been deleted by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.de:
                    return $"Eine **Datei** wurde von <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) gelöscht.";
            }
            return $"A **file** has been deleted by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
        }
        public string NotificationModcaseFileUpdate(DSharpPlus.Entities.DiscordUser actor) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"A **file** has been updated by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
                case Language.de:
                    return $"Eine **Datei** wurde von <@{actor.Id}> ({actor.Username}#{actor.Discriminator}) aktualisiert.";
            }
            return $"A **file** has been updated by <@{actor.Id}> ({actor.Username}#{actor.Discriminator}).";
        }
        public string NotificationModcaseDMWarn(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordGuild guild, string serviceBaseUrl) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have warned you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich verwarnt.\nFür weitere Informationen besuche: {serviceBaseUrl}";
            }
            return $"The moderators of guild `{guild.Name}` have warned you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMMuteTemp(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordGuild guild, string serviceBaseUrl) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have temporarily muted you until {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich temporär stummgeschalten bis {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFür weitere Informationen besuche: {serviceBaseUrl}";
            }
            return $"The moderators of guild `{guild.Name}` have temporarily muted you until {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMMutePerm(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordGuild guild, string serviceBaseUrl) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have muted you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich stummgeschalten.\nFür weitere Informationen besuche: {serviceBaseUrl}";
            }
            return $"The moderators of guild `{guild.Name}` have muted you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMBanTemp(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordGuild guild, string serviceBaseUrl) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have temporarily banned you until {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich temporär gebannt bis {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFür weitere Informationen besuche: {serviceBaseUrl}";
            }
            return $"The moderators of guild `{guild.Name}` have temporarily banned you until {modCase.PunishedUntil.Value.ToDiscordTS()}.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMBanPerm(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordGuild guild, string serviceBaseUrl) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have banned you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich gebannt.\nFür weitere Informationen besuche: {serviceBaseUrl}";
            }
            return $"The moderators of guild `{guild.Name}` have banned you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationModcaseDMKick(masz.Models.ModCase modCase, DSharpPlus.Entities.DiscordGuild guild, string serviceBaseUrl) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"The moderators of guild `{guild.Name}` have kicked you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
                case Language.de:
                    return $"Die Moderatoren von `{guild.Name}` haben dich kickt.\nFür weitere Informationen besuche: {serviceBaseUrl}";
            }
            return $"The moderators of guild `{guild.Name}` have kicked you.\nFor more information or rehabilitation visit: {serviceBaseUrl}";
        }
        public string NotificationFilesCreate() {
            switch (preferredLanguage) {
                case Language.en:
                    return "File uploaded";
                case Language.de:
                    return "Datei hochgeladen";
            }
            return "File uploaded";
        }
        public string NotificationFilesDelete() {
            switch (preferredLanguage) {
                case Language.en:
                    return "File deleted";
                case Language.de:
                    return "Datei gelöscht";
            }
            return "File deleted";
        }
        public string NotificationFilesUpdate() {
            switch (preferredLanguage) {
                case Language.en:
                    return "File updated";
                case Language.de:
                    return "Datei aktualisiert";
            }
            return "File updated";
        }
        public string NotificationRegisterWelcomeToMASZ() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Welcome to MASZ!";
                case Language.de:
                    return "Willkommen bei MASZ!";
            }
            return "Welcome to MASZ!";
        }
        public string NotificationRegisterDescriptionThanks() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Thanks for registering your guild.\nIn the following you will learn some useful tips for setting up and using **MASZ**.";
                case Language.de:
                    return "Vielen Dank für deine Registrierung.\nIm Folgenden wirst du einige nützliche Tipps zum Einrichten und Verwenden von **MASZ** erhalten.";
            }
            return "Thanks for registering your guild.\nIn the following you will learn some useful tips for setting up and using **MASZ**.";
        }
        public string NotificationRegisterUseFeaturesCommand() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Use the `/features` command to test if your current guild setup supports all features of **MASZ**.";
                case Language.de:
                    return "Benutze den `/features` Befehl um zu sehen welche Features von **MASZ** dein aktuelles Setup unterstützt.";
            }
            return "Use the `/features` command to test if your current guild setup supports all features of **MASZ**.";
        }
        public string NotificationRegisterDefaultLanguageUsed(string language) {
            switch (preferredLanguage) {
                case Language.en:
                    return $"MASZ will use `{language}` as default language for this guild whenever possible.";
                case Language.de:
                    return $"MASZ wird `{language}` als Standard-Sprache für diese Gilde verwenden, wenn möglich.";
            }
            return $"MASZ will use `{language}` as default language for this guild whenever possible.";
        }
        public string NotificationRegisterConfusingTimestamps() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Timezones can be confusing.\nMASZ uses a Discord feature to display timestamps in the local timezone of your computer/phone.";
                case Language.de:
                    return "Zeitzonen können kompliziert sein.\nMASZ benutzt ein Discord-Feature um Zeitstempel in der lokalen Zeitzone deines Computers/Handys anzuzeigen.";
            }
            return "Timezones can be confusing.\nMASZ uses a Discord feature to display timestamps in the local timezone of your computer/phone.";
        }
        public string NotificationRegisterSupport() {
            switch (preferredLanguage) {
                case Language.en:
                    return "Please refer to the [MASZ Support Server](https://discord.gg/5zjpzw6h3S) for further questions.";
                case Language.de:
                    return "Bitte wende dich an den [MASZ Support Server](https://discord.gg/5zjpzw6h3S) für weitere Fragen.";
            }
            return "Please refer to the [MASZ Support Server](https://discord.gg/5zjpzw6h3S) for further questions.";
        }
        public string Enum(masz.Enums.PunishmentType enumValue) {
            switch (enumValue) {
                case masz.Enums.PunishmentType.Mute:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Mute";
                        case Language.de:
                            return "Stummschaltung";
                        default:
                            return "Mute";
                    }
                case masz.Enums.PunishmentType.Ban:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Ban";
                        case Language.de:
                            return "Sperrung";
                        default:
                            return "Ban";
                    }
                case masz.Enums.PunishmentType.Kick:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Kick";
                        case Language.de:
                            return "Kick";
                        default:
                            return "Kick";
                    }
                case masz.Enums.PunishmentType.None:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Warn";
                        case Language.de:
                            return "Verwarnung";
                        default:
                            return "Warn";
                    }
            }
            return "Unknown";
        }
        public string Enum(masz.Enums.ViewPermission enumValue) {
            switch (enumValue) {
                case masz.Enums.ViewPermission.Self:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Self";
                        case Language.de:
                            return "Privat";
                        default:
                            return "Self";
                    }
                case masz.Enums.ViewPermission.Guild:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Guild";
                        case Language.de:
                            return "Gilde";
                        default:
                            return "Guild";
                    }
                case masz.Enums.ViewPermission.Global:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Global";
                        case Language.de:
                            return "Global";
                        default:
                            return "Global";
                    }
            }
            return "Unknown";
        }
        public string Enum(masz.Enums.AutoModerationAction enumValue) {
            switch (enumValue) {
                case masz.Enums.AutoModerationAction.None:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "None";
                        case Language.de:
                            return "Keine Aktion";
                        default:
                            return "None";
                    }
                case masz.Enums.AutoModerationAction.ContentDeleted:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Content deleted";
                        case Language.de:
                            return "Nachricht gelöscht";
                        default:
                            return "Content deleted";
                    }
                case masz.Enums.AutoModerationAction.CaseCreated:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Case created";
                        case Language.de:
                            return "Vorfall erstellt";
                        default:
                            return "Case created";
                    }
                case masz.Enums.AutoModerationAction.ContentDeletedAndCaseCreated:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Content deleted and case created";
                        case Language.de:
                            return "Nachricht gelöscht und Vorfall erstellt";
                        default:
                            return "Content deleted and case created";
                    }
            }
            return "Unknown";
        }
        public string Enum(masz.Enums.AutoModerationType enumValue) {
            switch (enumValue) {
                case masz.Enums.AutoModerationType.InvitePosted:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Invite posted";
                        case Language.de:
                            return "Einladung gesendet";
                        default:
                            return "Invite posted";
                    }
                case masz.Enums.AutoModerationType.TooManyEmotes:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many emotes used";
                        case Language.de:
                            return "Zu viele Emojis verwendet";
                        default:
                            return "Too many emotes used";
                    }
                case masz.Enums.AutoModerationType.TooManyMentions:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many users mentioned";
                        case Language.de:
                            return "Zu viele Benutzer erwähnt";
                        default:
                            return "Too many users mentioned";
                    }
                case masz.Enums.AutoModerationType.TooManyAttachments:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many attachments used";
                        case Language.de:
                            return "Zu viele Anhänge verwendet";
                        default:
                            return "Too many attachments used";
                    }
                case masz.Enums.AutoModerationType.TooManyEmbeds:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many embeds used";
                        case Language.de:
                            return "Zu viele Einbettungen verwendet";
                        default:
                            return "Too many embeds used";
                    }
                case masz.Enums.AutoModerationType.TooManyAutoModerations:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many auto-moderations";
                        case Language.de:
                            return "Zu viele automatische Moderationen";
                        default:
                            return "Too many auto-moderations";
                    }
                case masz.Enums.AutoModerationType.CustomWordFilter:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Custom wordfilter triggered";
                        case Language.de:
                            return "Benutzerdefinierter Wortfilter ausgelöst";
                        default:
                            return "Custom wordfilter triggered";
                    }
                case masz.Enums.AutoModerationType.TooManyMessages:
                    switch (preferredLanguage) {
                        case Language.en:
                            return "Too many messages";
                        case Language.de:
                            return "Zu viele Nachrichten";
                        default:
                            return "Too many messages";
                    }
            }
            return "Unknown";
        }

    }
}
