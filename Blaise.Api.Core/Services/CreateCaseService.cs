using System.Collections.Generic;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Services
{
    public class CreateCaseService : ICreateCaseService
    {
        private readonly IBlaiseCaseApi _blaiseApi;
        private readonly ICatiDataService _catiManaService;
        private readonly ILoggingService _loggingService;

        public CreateCaseService(
            IBlaiseCaseApi blaiseApi,
            ICatiDataService catiManaService,
            ILoggingService loggingService)
        {
            _blaiseApi = blaiseApi;
            _catiManaService = catiManaService;
            _loggingService = loggingService;
        }

        public void CreateOnlineCase(IDataRecord dataRecord, string instrumentName, string serverParkName, 
             string primaryKey)
        {
            var outcomeCode = _blaiseApi.GetOutcomeCode(dataRecord);
            var existingFieldData = _blaiseApi.GetRecordDataFields(dataRecord);
            
            var newFieldData = _blaiseApi.GetRecordDataFields(dataRecord);
            _catiManaService.RemoveCatiManaBlock(newFieldData);
            
            _catiManaService.AddCatiManaCallItems(newFieldData, existingFieldData, outcomeCode);

            _blaiseApi.CreateCase(primaryKey, newFieldData, instrumentName, serverParkName);
            _loggingService.LogInfo($"Created new case with SerialNumber '{primaryKey}'");
        }
    }
}
