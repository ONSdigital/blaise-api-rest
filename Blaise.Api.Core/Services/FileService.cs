using System;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Threading;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;

[assembly: InternalsVisibleTo("Blaise.Api.Tests.Unit")]

namespace Blaise.Api.Core.Services
{
    public class FileService : IFileService
    {
        private readonly IBlaiseFileApi _blaiseFileApi;
        private readonly IFileSystem _fileSystem;

        public FileService(
            IBlaiseFileApi blaiseFileApi,
            IFileSystem fileSystem)
        {
            _blaiseFileApi = blaiseFileApi;
            _fileSystem = fileSystem;
        }

        public void UpdateQuestionnaireFileWithSqlConnection(string questionnaireFile)
        {
            questionnaireFile.ThrowExceptionIfNullOrEmpty("questionnaireFile");
            var questionnaireName = GetQuestionnaireNameFromFile(questionnaireFile);

            _blaiseFileApi.UpdateQuestionnaireFileWithSqlConnection(
                questionnaireName,
                questionnaireFile);
        }

        public string GetQuestionnaireNameFromFile(string questionnaireFile)
        {
            return _fileSystem.Path.GetFileNameWithoutExtension(questionnaireFile);
        }

        public string GetDatabaseFile(string filePath, string questionnaireName)
        {
            return _fileSystem.Path.Combine(filePath, $"{questionnaireName}.bdix");
        }

        public void RemovePathAndFiles(string path)
        {
            if (!_fileSystem.Directory.Exists(path))
            {
                return;
            }

            var directoryInfo = _fileSystem.Directory.CreateDirectory(path);

            if (directoryInfo.Parent != null &&
                Guid.TryParse(_fileSystem.Path.GetDirectoryName(directoryInfo.Parent.Name), out _))
            {
                CleanUpFiles(directoryInfo.Parent.FullName);
                return;
            }

            CleanUpFiles(path);
        }

        public void UnzipFile(string filePath, string destinationPath)
        {
            ZipFile.ExtractToDirectory(filePath, destinationPath);
        }

        private void CleanUpFiles(string path)
        {
            try
            {
                Thread.Sleep(2000);
                _fileSystem.Directory.Delete(path, true);
            }
            catch
            {
                // file locked? don't error, still continue...
            }
        }
    }
}
