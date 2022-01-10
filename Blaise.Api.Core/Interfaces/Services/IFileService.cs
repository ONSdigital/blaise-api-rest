
namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IFileService
    {
        void UpdateInstrumentFileWithSqlConnection(string instrumentFile);

        string GetInstrumentNameFromFile(string instrumentFile);

        string GetDatabaseFile(string filePath, string instrumentName);

        void RemovePathAndFiles(string path);
    }
}