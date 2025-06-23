namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IFileService
    {
        void UpdateQuestionnaireFileWithSqlConnection(string questionnaireFile);

        string GetQuestionnaireNameFromFile(string questionnaireFile);

        string GetDatabaseFile(string filePath, string questionnaireName);

        void RemovePathAndFiles(string path);

        void UnzipFile(string filePath, string destinationPath);
    }
}
