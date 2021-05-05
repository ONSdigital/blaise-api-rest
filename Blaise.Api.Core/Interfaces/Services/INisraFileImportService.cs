namespace Blaise.Api.Core.Interfaces.Services
{
    public interface INisraFileImportService
    {
        void ImportNisraDatabaseFile(string databaseFilePath, string instrumentName, string serverParkName);
    }
}