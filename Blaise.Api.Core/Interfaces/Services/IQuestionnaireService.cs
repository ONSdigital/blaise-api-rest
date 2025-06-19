using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Questionnaire;
using Blaise.Nuget.Api.Contracts.Enums;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IQuestionnaireService
    {
        IEnumerable<QuestionnaireDto> GetAllQuestionnaires();

        IEnumerable<QuestionnaireDto> GetQuestionnaires(string serverParkName);

        QuestionnaireDto GetQuestionnaire(string questionnaireName, string serverParkName);

        bool QuestionnaireExists(string questionnaireName, string serverParkName);

        Guid GetQuestionnaireId(string questionnaireName, string serverParkName);

        QuestionnaireStatusType GetQuestionnaireStatus(string questionnaireName, string serverParkName);

        void ActivateQuestionnaire(string questionnaireName, string serverParkName);

        void DeactivateQuestionnaire(string questionnaireName, string serverParkName);

        IEnumerable<string> GetModes(string questionnaireName, string serverParkName);

        bool ModeExists(string questionnaireName, string serverParkName, string mode);

        IEnumerable<DataEntrySettingsDto> GetDataEntrySettings(string questionnaireName, string serverParkName);
    }
}
