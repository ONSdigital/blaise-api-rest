namespace Blaise.Api.Core.Interfaces.Services
{
    public interface INisraFileImportService
    {
        void ImportNisraDatabaseFile(string databaseFilePath, string questionnaireName, string serverParkName);
    }
}