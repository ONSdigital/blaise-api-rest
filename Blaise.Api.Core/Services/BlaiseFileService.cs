using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using System.IO.Abstractions;
using System.Runtime.CompilerServices;
using Blaise.Api.Contracts.Interfaces;

[assembly: InternalsVisibleTo("Blaise.Api.Tests.Unit")]
namespace Blaise.Api.Core.Services
{
    public class BlaiseFileService : IBlaiseFileService
    {
        private readonly IBlaiseFileApi _blaiseFileApi;
        private readonly IFileSystem _fileSystem;
        private readonly IConfigurationProvider _configurationProvider;

        public BlaiseFileService(
            IBlaiseFileApi blaiseFileApi, 
            IFileSystem fileSystem, 
            IConfigurationProvider configurationProvider)
        {
            _blaiseFileApi = blaiseFileApi;
            _fileSystem = fileSystem;
            _configurationProvider = configurationProvider;
        }

        public void UpdateInstrumentFileWithSqlConnection(string instrumentFile)
        {
            instrumentFile.ThrowExceptionIfNullOrEmpty("instrumentFile");
            var instrumentName = GetInstrumentNameFromFile(instrumentFile);

            _blaiseFileApi.UpdateInstrumentFileWithSqlConnection(
                instrumentName,
                instrumentFile);
        }

        public void UpdateInstrumentFileWithData(string serverParkName, string instrumentFile)
        {
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");
            instrumentFile.ThrowExceptionIfNullOrEmpty("instrumentFile");

            var instrumentName = GetInstrumentNameFromFile(instrumentFile);

            _blaiseFileApi.UpdateInstrumentFileWithData(serverParkName, instrumentName,
                instrumentFile);
        }
        public void DeleteFile(string instrumentFile)
        {
            _fileSystem.File.Delete(instrumentFile);
        }

        public string GetInstrumentNameFromFile(string instrumentFile)
        {
            return _fileSystem.Path.GetFileNameWithoutExtension(instrumentFile);
        }

        public string GetInstrumentPackageName(string instrumentName)
        {
            return $"{instrumentName}.{_configurationProvider.PackageExtension}";
        }
    }
}
