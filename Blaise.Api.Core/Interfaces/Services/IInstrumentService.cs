using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Nuget.Api.Contracts.Enums;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IInstrumentService
    {
        IEnumerable<InstrumentDto> GetAllInstruments();

        IEnumerable<InstrumentDto> GetInstruments(string serverParkName);

        InstrumentDto GetInstrument(string instrumentName, string serverParkName);

        bool InstrumentExists(string instrumentName, string serverParkName);

        Guid GetInstrumentId(string instrumentName, string serverParkName);

        SurveyStatusType GetInstrumentStatus(string instrumentName, string serverParkName);

        void ActivateInstrument(string instrumentName, string serverParkName);

        void DeactivateInstrument(string instrumentName, string serverParkName);
    }
}
