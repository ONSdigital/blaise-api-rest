namespace Blaise.Api.Contracts.Models.Reports
{
    using System;
    using System.Collections.Generic;

    public class ReportDto
    {
        public string QuestionnaireName { get; set; }

        public Guid QuestionnaireId { get; set; }

        public List<Dictionary<string, string>> ReportingData { get; set; } = new List<Dictionary<string, string>>();
    }
}
