namespace Blaise.Api.Core.Interfaces.Services
{
    public interface INisraService
    {
        void ImportOnlineDatabaseFile(string databaseFilePath, string instrumentName, string serverParkName);
    }
}