
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using masz.Enums;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using masz.Events;
using masz.Exceptions;
using masz.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace masz.Repositories
{

    public class FileRepository : BaseRepository<FileRepository>
    {
        private readonly Identity _identity;
        private FileRepository(IServiceProvider serviceProvider, Identity identity) : base(serviceProvider)
        {
            _identity = identity;
        }

        public static FileRepository CreateDefault(IServiceProvider serviceProvider, Identity identity) => new FileRepository(serviceProvider, identity);

        public Models.FileInfo GetCaseFile(ulong guildId, int caseId, string fileName)
        {
            var filePath = Path.Join(_config.GetFileUploadPath(), guildId.ToString(), caseId.ToString(), _filesHandler.RemoveSpecialCharacters(fileName));

            var fullFilePath = Path.GetFullPath(filePath);

            // https://stackoverflow.com/a/1321535/9850709
            if (fullFilePath != filePath)
            {
                throw new InvalidPathException();
            }

            byte[] fileData;
            try
            {
                fileData = _filesHandler.ReadFile(filePath);
            } catch (Exception e)
            {
                _logger.LogError(e, "Failed to read file");
                throw new ResourceNotFoundException();
            }

            if (fileData == null)
            {
                throw new ResourceNotFoundException();
            }

            string contentType = _filesHandler.GetContentType(filePath);
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileName,
                Inline = true,
            };

            return new Models.FileInfo
            {
                Name = fileName,
                ContentType = contentType,
                ContentDisposition = cd,
                FileContent = fileData,
            };
        }

        public List<string> GetCaseFiles(ulong guildId, int caseId)
        {
            var uploadDir = Path.Join(_config.GetFileUploadPath(), guildId.ToString(), caseId.ToString());

            var fullPath = Path.GetFullPath(uploadDir);

            // https://stackoverflow.com/a/1321535/9850709
            if (fullPath != uploadDir)
            {
                throw new InvalidPathException();
            }

            System.IO.FileInfo[] files = _filesHandler.GetFilesByDirectory(fullPath);
            if (files == null)
            {
                throw new ResourceNotFoundException();
            }

            return files.Select(f => f.Name).ToList();
        }

        public async Task<string> UploadFile(IFormFile file, ulong guildId, int caseId)
        {
            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, _identity).GetModCase(guildId, caseId);
            DiscordUser currentUser = _identity.GetCurrentUser();

            var uploadDir = Path.Join(_config.GetFileUploadPath(), guildId.ToString(), caseId.ToString());

            var fullPath = Path.GetFullPath(uploadDir);

            // https://stackoverflow.com/a/1321535/9850709
            if (fullPath != uploadDir)
            {
                throw new InvalidPathException();
            }

            string fileName = await _filesHandler.SaveFile(file, fullPath);

            try
            {
                await _discordAnnouncer.AnnounceFile(fileName, modCase, currentUser, RestAction.Created);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to announce file.");
            }

            await _eventHandler.InvokeFileUploaded(new FileUploadedEventArgs(GetCaseFile(guildId, caseId, fileName)));

            return fileName;
        }

        public async Task DeleteFile(ulong guildId, int caseId, string fileName)
        {
            ModCase modCase = await ModCaseRepository.CreateDefault(_serviceProvider, _identity).GetModCase(guildId, caseId);
            DiscordUser currentUser = _identity.GetCurrentUser();

            var filePath = Path.Join(_config.GetFileUploadPath(), guildId.ToString(), caseId.ToString(), _filesHandler.RemoveSpecialCharacters(fileName));

            var fullFilePath = Path.GetFullPath(filePath);

            // https://stackoverflow.com/a/1321535/9850709
            if (fullFilePath != filePath)
            {
                throw new InvalidPathException();
            }

            if (! _filesHandler.FileExists(fullFilePath)) {
                throw new ResourceNotFoundException();
            }

            _filesHandler.DeleteFile(fullFilePath);

            try
            {
                await _discordAnnouncer.AnnounceFile(fileName, modCase, currentUser, RestAction.Deleted);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Failed to announce file.");
            }
        }
    }
}