namespace Blaise.Api.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Blaise.Api.Contracts.Interfaces;
    using Blaise.Api.Contracts.Models.Reports;
    using Blaise.Api.Core.Interfaces.Services;
    using Blaise.Nuget.Api.Contracts.Interfaces;
    using StatNeth.Blaise.API.DataRecord;

    public class ReportingService : IReportingService
    {
        private readonly IBlaiseCaseApi _blaiseCaseApi;
        private readonly IBlaiseQuestionnaireApi _blaiseQuestionnaireApi;
        private readonly ILoggingService _loggingService;

        public ReportingService(
            IBlaiseCaseApi blaiseCaseApi,
            IBlaiseQuestionnaireApi blaiseQuestionnaireApi,
            ILoggingService loggingService)
        {
            _blaiseCaseApi = blaiseCaseApi;
            _blaiseQuestionnaireApi = blaiseQuestionnaireApi;
            _loggingService = loggingService;
        }

        public ReportDto GetReportingData(
            string serverParkName,
            string questionnaireName,
            List<string> fieldIds,
            string filter)
        {
            var questionnaireId = _blaiseQuestionnaireApi.GetIdOfQuestionnaire(questionnaireName, serverParkName);

            return BuildReportDto(serverParkName, questionnaireName, questionnaireId, fieldIds, filter);
        }

        public ReportDto GetReportingData(string serverParkName, Guid questionnaireId, List<string> fieldIds, string filter)
        {
            var surveys = _blaiseQuestionnaireApi.GetQuestionnaires(serverParkName);
            var questionnaireName = surveys.First(s => s.InstrumentID == questionnaireId).Name;

            return BuildReportDto(serverParkName, questionnaireName, questionnaireId, fieldIds, filter);
        }

        private ReportDto BuildReportDto(
            string serverParkName,
            string questionnaireName,
            Guid questionnaireId,
            List<string> fieldIds,
            string filter)
        {
            var reportDto = new ReportDto
            {
                QuestionnaireName = questionnaireName,
                QuestionnaireId = questionnaireId,
            };

            var cases = filter == null
                ? _blaiseCaseApi.GetCases(questionnaireName, serverParkName)
                : _blaiseCaseApi.GetFilteredCases(questionnaireName, serverParkName, filter);

            while (!cases.EndOfSet)
            {
                var caseData = GetReportFieldData(questionnaireName, fieldIds, cases.ActiveRecord);

                reportDto.ReportingData.Add(caseData);

                cases.MoveNext();
            }

            return reportDto;
        }

        private Dictionary<string, string> GetReportFieldData(string questionnaireName, IEnumerable<string> fieldIds, IDataRecord caseRecord)
        {
            var reportingData = new Dictionary<string, string>();

            foreach (var fieldId in fieldIds)
            {
                if (!_blaiseCaseApi.FieldExists(caseRecord, fieldId))
                {
                    _loggingService.LogWarn($"The field '{fieldId}' was not found in the questionnaire '{questionnaireName}'");
                    continue;
                }

                reportingData.Add(fieldId, _blaiseCaseApi.GetFieldValue(caseRecord, fieldId).ValueAsText);
            }

            return reportingData;
        }
    }
}
