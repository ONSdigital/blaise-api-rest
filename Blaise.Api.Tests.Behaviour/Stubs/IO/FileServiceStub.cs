using System;
using Blaise.Api.Core.Interfaces.Services;

namespace Blaise.Api.Tests.Behaviour.Stubs.IO
{
    public class FileServiceStub : IFileService
    {
        public void UpdateQuestionnaireFileWithSqlConnection(string questionnaireFile)
        {
           
        }

        public string GetQuestionnaireNameFromFile(string questionnaireFile)
        {
            return "OpnTest";
        }

        public string GetDatabaseFile(string filePath, string questionnaireName)
        {
            throw new NotImplementedException();
        }

        public void RemovePathAndFiles(string path)
        {
        }
    }
}
