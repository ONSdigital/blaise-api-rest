using System;
using System.Linq;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Nuget.Api.Contracts.Enums;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Mappers
{
    public class QuestionnaireStatusMapper : IQuestionnaireStatusMapper
    {
        private const int InstallExpiryTimeOutInMinutes = 10;

        public QuestionnaireStatusType GetQuestionnaireStatus(ISurvey questionnaire)
        {
            if (AllQuestionnaireNodesAreInAStatus(questionnaire, QuestionnaireStatusType.Active))
            {
                return QuestionnaireStatusType.Active;
            }

            if (AllQuestionnaireNodesAreInAStatus(questionnaire, QuestionnaireStatusType.Inactive))
            {
                return QuestionnaireStatusType.Inactive;
            }

            return AnyQuestionnaireNodeHasFailedOrTakenTooLongToInstall(questionnaire)
                ? QuestionnaireStatusType.Failed
                : QuestionnaireStatusType.Installing;
        }

        private static bool AllQuestionnaireNodesAreInAStatus(ISurvey questionnaire, QuestionnaireStatusType statusType)
        {
            return questionnaire.Configuration.Configurations
                .All(c => c.Status == statusType.ToString());
        }

        private static bool AnyQuestionnaireNodeHasFailedOrTakenTooLongToInstall(ISurvey questionnaire)
        {
            return AnyQuestionnaireNodeHasFailed(questionnaire) || QuestionnaireHasTakenTooLongToInstall(questionnaire);
        }

        private static bool AnyQuestionnaireNodeHasFailed(ISurvey questionnaire)
        {
            return questionnaire.Configuration.Configurations.Any(c =>
                c.Status != QuestionnaireStatusType.Active.ToString() &&
                c.Status != QuestionnaireStatusType.Installing.ToString());
        }

        private static bool QuestionnaireHasTakenTooLongToInstall(ISurvey questionnaire)
        {
            var expiredInstallDateTime = DateTime.Now.AddMinutes(-InstallExpiryTimeOutInMinutes);

            return questionnaire.InstallDate < expiredInstallDateTime;
        }
    }
}
