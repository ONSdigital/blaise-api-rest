namespace Blaise.Api.Contracts.Models.Questionnaire
{
    using System;
    using System.Collections.Generic;

    public class QuestionnaireDto
    {
        public string Name { get; set; }

        public Guid Id { get; set; }

        public string ServerParkName { get; set; }

        public DateTime InstallDate { get; set; }

        public string Status { get; set; }

        public int DataRecordCount { get; set; }

        public bool HasData => DataRecordCount > 0;

        public string BlaiseVersion { get; set; }

        public DateTime? FieldPeriod { get; set; }

        public string SurveyTla { get; set; }

        public IEnumerable<QuestionnaireNodeDto> Nodes { get; set; } = new List<QuestionnaireNodeDto>();
    }
}
