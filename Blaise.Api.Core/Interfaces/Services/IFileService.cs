﻿namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IFileService
    {
        void UpdateInstrumentFileWithSqlConnection(string instrumentFile);
        void UpdateInstrumentFileWithData(string serverParkName, string instrumentFile);
        void DeleteFile(string instrumentFile);
        string GetInstrumentNameFromFile(string instrumentFile);
        string GetInstrumentPackageName(string instrumentName);
        string GetDatabaseFile(string filePath, string instrumentName);
    }
}