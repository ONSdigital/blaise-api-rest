using System.Threading;
using Blaise.Api.Tests.Behaviour.Helpers.Configuration;
using Blaise.Api.Tests.Behaviour.Stubs;
using Blaise.Api.Tests.Behaviour.Stubs.Blaise;
using Blaise.Nuget.Api.Api;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Tests.Behaviour.Helpers.Instrument
{
    public class InstrumentHelper
    {
        private readonly IBlaiseSurveyApi _blaiseSurveyApi;

        private static InstrumentHelper _currentInstance;

        public InstrumentHelper()
        {
            if (StubConfigurationHelper.UseStubs)
            {
                _blaiseSurveyApi = UnityConfigStub.Resolve<IBlaiseSurveyApi>();

                return;
            }

            _blaiseSurveyApi = new BlaiseSurveyApi();
        }

        public static InstrumentHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new InstrumentHelper());
        }

        public void InstallSurvey()
        {
            _blaiseSurveyApi.InstallSurvey(
                BlaiseConfigurationHelper.InstrumentName,
                BlaiseConfigurationHelper.ServerParkName,
                BlaiseConfigurationHelper.InstrumentPackagePath,
                SurveyInterviewType.Cati);
        }

        public bool SurveyHasInstalled(int timeoutInSeconds)
        {
            return SurveyExists(BlaiseConfigurationHelper.InstrumentName, timeoutInSeconds) &&
                   SurveyIsActive(BlaiseConfigurationHelper.InstrumentName, timeoutInSeconds);
        }

        public void UninstallSurvey()
        {
            _blaiseSurveyApi.UninstallSurvey(
                BlaiseConfigurationHelper.InstrumentName,
                BlaiseConfigurationHelper.ServerParkName);
        }

        public bool SetSurveyAsActive(int timeoutInSeconds)
        {
            _blaiseSurveyApi.ActivateSurvey(
                BlaiseConfigurationHelper.InstrumentName,
                BlaiseConfigurationHelper.ServerParkName);

            return SurveyIsActive(BlaiseConfigurationHelper.InstrumentName, timeoutInSeconds);
        }

        private bool SurveyIsActive(string instrumentName, int timeoutInSeconds)
        {
            var counter = 0;
            const int maxCount = 10;

            while (GetSurveyStatus(instrumentName) == SurveyStatusType.Installing)
            {
                Thread.Sleep(timeoutInSeconds % maxCount);

                counter++;
                if (counter == maxCount)
                {
                    return false;
                }
            }
            return GetSurveyStatus(instrumentName) == SurveyStatusType.Active;
        }

        private bool SurveyExists(string instrumentName, int timeoutInSeconds)
        {
            var counter = 0;
            const int maxCount = 10;

            while (!_blaiseSurveyApi.SurveyExists(instrumentName, BlaiseConfigurationHelper.ServerParkName))
            {
                Thread.Sleep(timeoutInSeconds % maxCount);

                counter++;
                if (counter == maxCount)
                {
                    return false;
                }
            }
            return true;
        }

        private SurveyStatusType GetSurveyStatus(string instrumentName)
        {
            return _blaiseSurveyApi.GetSurveyStatus(instrumentName, BlaiseConfigurationHelper.ServerParkName);
        }
    }
}
