using Blaise.Api.Contracts.Enums;

namespace Blaise.Api.Contracts.Models.Case
{
    public class CaseEditInformationDto
    {
        public string PrimaryKey { get; set; }

        public int Outcome { get; set; }

        public string AssignedTo { get; set; }

        public EditedStatusType EditedStatus { get; set; }

        public string Interviewer { get; set; }
    }
}
