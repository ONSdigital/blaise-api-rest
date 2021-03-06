﻿using System.Threading.Tasks;
using Blaise.Api.Contracts.Models.Instrument;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IInstrumentDataService
    {
        Task<string> GetInstrumentPackageWithDataAsync(string serverParkName, string instrumentName, string tempFilePath);

        Task ImportOnlineDataAsync(InstrumentDataDto instrumentDataDto, string serverParkName, string instrumentName, string tempFilePath);
    }
}