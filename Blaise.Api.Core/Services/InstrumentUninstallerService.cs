﻿using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class InstrumentUninstallerService : IInstrumentUninstallerService
    {
        private readonly IBlaiseSurveyApi _blaiseSurveyApi;
        private readonly IBlaiseCaseApi _blaiseCaseApi;

        public InstrumentUninstallerService(
            IBlaiseSurveyApi blaiseApi, 
            IBlaiseCaseApi blaiseCaseApi)
        {
            _blaiseSurveyApi = blaiseApi;
            _blaiseCaseApi = blaiseCaseApi;
        }
        public void UninstallInstrument(string instrumentName, string serverParkName)
        {
            instrumentName.ThrowExceptionIfNullOrEmpty("instrumentName");
            serverParkName.ThrowExceptionIfNullOrEmpty("serverParkName");

            _blaiseCaseApi.RemoveCases(instrumentName, serverParkName);
            _blaiseSurveyApi.UninstallSurvey(instrumentName, serverParkName);
        }
    }
}
