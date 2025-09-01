
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class InstrumentChecker
    {
        private readonly IBlaiseSurveyApi _surveyApi;

        public InstrumentChecker(IBlaiseSurveyApi surveyApi)
        {
            _surveyApi = surveyApi;
        }
    }
}
