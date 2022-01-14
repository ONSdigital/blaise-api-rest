using System;
using Blaise.Api.Core.Interfaces.Services;

namespace Blaise.Api.Tests.Behaviour.Stubs.IO
{
    public class FileServiceStub : IFileService
    {
        public void UpdateInstrumentFileWithSqlConnection(string instrumentFile)
        {
           
        }

        public string GetInstrumentNameFromFile(string instrumentFile)
        {
            return "OpnTest";
        }

        public string GetDatabaseFile(string filePath, string instrumentName)
        {
            throw new NotImplementedException();
        }

        public void RemovePathAndFiles(string path)
        {
        }
    }
}
