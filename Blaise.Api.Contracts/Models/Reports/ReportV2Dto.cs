using System;
using System.Collections.Generic;

namespace Blaise.Api.Contracts.Models.Reports
{
    public class ReportV2Dto
    {
        public ReportV2Dto()
        {
            ReportingData = new List<Dictionary<string, string>>();
        }

        public string QuestionnaireName { get; set; }

        public Guid QuestionnaireId { get; set; }

        public List<Dictionary<string, string>> ReportingData { get; set; }
    }
}
