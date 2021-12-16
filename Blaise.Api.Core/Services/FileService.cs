using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;

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
    }
}
