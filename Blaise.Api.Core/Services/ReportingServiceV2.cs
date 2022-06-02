using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Reports;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Core.Services
{
    public class ReportingServiceV2 : IReportingServiceV2
    {
        private readonly IBlaiseCaseApi _blaiseCaseApi;
        private readonly IBlaiseQuestionnaireApi _blaiseQuestionnaireApi;

        public ReportingServiceV2(
            IBlaiseCaseApi blaiseCaseApi,
            IBlaiseQuestionnaireApi blaiseQuestionnaireApi)
        {
            _blaiseCaseApi = blaiseCaseApi;
            _blaiseQuestionnaireApi = blaiseQuestionnaireApi;
        }

        public ReportV2Dto GetReportingData(string serverParkName, string questionnaireName,
            List<string> fieldIds)
        {
            var questionnaireId = _blaiseQuestionnaireApi.GetIdOfQuestionnaire(questionnaireName, serverParkName);

            return BuildReportDto(serverParkName, questionnaireName, questionnaireId, fieldIds);
        }

        public ReportV2Dto GetReportingData(string serverParkName, Guid questionnaireId, List<string> fieldIds)
        {
            var surveys = _blaiseQuestionnaireApi.GetQuestionnaires(serverParkName);
            var questionnaireName = surveys.First(s => s.InstrumentID == questionnaireId).Name;

            return BuildReportDto(serverParkName, questionnaireName, questionnaireId, fieldIds);
        }

        private ReportV2Dto BuildReportDto(string serverParkName, string questionnaireName, Guid questionnaireId, List<string> fieldIds)
        {
            var reportDto = new ReportV2Dto
            {
                QuestionnaireName = questionnaireName,
                QuestionnaireId = questionnaireId
            };

            var cases = _blaiseCaseApi.GetCases(questionnaireName, serverParkName);

            while (!cases.EndOfSet)
            {
                var caseData = GetReportFieldData(fieldIds, cases.ActiveRecord);

                reportDto.ReportingData.Add(caseData);

                cases.MoveNext();
            }

            return reportDto;
        }

        private Dictionary<string, string> GetReportFieldData(IEnumerable<string> fieldIds, IDataRecord caseRecord)
        {
            var reportingData = new Dictionary<string, string>();

            foreach (var fieldId in fieldIds)
            {
                reportingData.Add(fieldId, _blaiseCaseApi.GetFieldValue(caseRecord, fieldId).ValueAsText);
            }

            return reportingData;
        }
    }
}