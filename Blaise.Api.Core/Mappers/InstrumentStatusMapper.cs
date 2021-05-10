using System;
using System.Linq;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Nuget.Api.Contracts.Enums;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Mappers
{
    public class InstrumentStatusMapper : IInstrumentStatusMapper
    {
        private const int InstallExpiryTimeOutInMinutes = 10;

        public SurveyStatusType GetInstrumentStatus(ISurvey instrument)
        {
            if (AllInstrumentNodesAreActive(instrument))
            {
                return SurveyStatusType.Active;
            }

            return AnyInstrumentNodeHasFailedOrTakenTooLongToInstall(instrument) 
                ? SurveyStatusType.Failed 
                : SurveyStatusType.Installing;
        }

        private static bool AllInstrumentNodesAreActive(ISurvey instrument)
        {
            return instrument.Configuration.Configurations
                .All(c => c.Status == SurveyStatusType.Active.ToString());
        }

        private static bool AnyInstrumentNodeHasFailedOrTakenTooLongToInstall(ISurvey instrument)
        {
            return AnyInstrumentNodeHasFailed(instrument) || InstrumentHasTakenTooLongToInstall(instrument);
        }
        
        private static bool AnyInstrumentNodeHasFailed(ISurvey instrument)
        {
            return instrument.Configuration.Configurations.Any(c =>
                c.Status != SurveyStatusType.Active.ToString() &&
                c.Status != SurveyStatusType.Installing.ToString());
        }
        
        private static bool InstrumentHasTakenTooLongToInstall(ISurvey instrument)
        {
            var expiredInstallDateTime = DateTime.Now.AddMinutes(-InstallExpiryTimeOutInMinutes);

            return instrument.InstallDate < expiredInstallDateTime;
        }
    }
}
