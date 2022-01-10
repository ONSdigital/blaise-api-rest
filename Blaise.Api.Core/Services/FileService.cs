using System;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using System.Threading;

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

        public void UpdateInstrumentFileWithSqlConnection(string instrumentFile)
        {
            instrumentFile.ThrowExceptionIfNullOrEmpty("instrumentFile");
            var instrumentName = GetInstrumentNameFromFile(instrumentFile);

            _blaiseFileApi.UpdateInstrumentFileWithSqlConnection(
                instrumentName,
                instrumentFile);
        }

        public string GetInstrumentNameFromFile(string instrumentFile)
        {
            return _fileSystem.Path.GetFileNameWithoutExtension(instrumentFile);
        }

        public string GetDatabaseFile(string filePath, string instrumentName)
        {
            return _fileSystem.Path.Combine(filePath, $"{instrumentName}.bdix");
        }

        public void RemovePathAndFiles(string path)
        {
            if (!_fileSystem.Directory.Exists(path)) return;
            
            var directoryInfo = _fileSystem.Directory.CreateDirectory(path);

            if (directoryInfo.Parent != null &&
                Guid.TryParse(_fileSystem.Path.GetDirectoryName(directoryInfo.Parent.Name), out _))
            {
                CleanUpFiles(directoryInfo.Parent.FullName);
                return;
            }

            CleanUpFiles(path);
        }

        private void CleanUpFiles(string path)
        {
            try
            {
                Thread.Sleep(2000);
                _fileSystem.Directory.Delete(path, true);
            }
            catch //ewwwwwww fml
            {
            }
        }
    }
}
