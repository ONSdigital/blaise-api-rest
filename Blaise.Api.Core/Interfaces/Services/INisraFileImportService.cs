namespace Blaise.Api.Core.Interfaces.Services
{
    public interface INisraFileImportService
    {
        void ImportOnlineDatabaseFile(string databaseFilePath, string instrumentName, string serverParkName);
    }
}