using System;
using System.Collections.Generic;

namespace Blaise.Api.Contracts.Models.Questionnaire
{
    public class QuestionnaireDto
    {
        public QuestionnaireDto()
        {
            Nodes = new List<QuestionnaireNodeDto>();
        }

        public string Name { get; set; }
        public Guid Id { get; set; }
        public string ServerParkName { get; set; }
        public DateTime InstallDate { get; set; }
        public string Status { get; set; }
        public int DataRecordCount { get; set; }
        public bool HasData => DataRecordCount > 0;
        public string BlaiseVersion { get; set; }
        public DateTime? FieldPeriod { get; set; }

        public IEnumerable<QuestionnaireNodeDto> Nodes { get; set; }
    }
}
