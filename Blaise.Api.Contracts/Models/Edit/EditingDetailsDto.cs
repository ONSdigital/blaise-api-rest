using Blaise.Api.Contracts.Enums;

namespace Blaise.Api.Contracts.Models.Edit
{
    public class EditingDetailsDto
    {
        public string PrimaryKey { get; set; }

        public int Outcome { get; set; }

        public string AssignedTo { get; set; }

        public EditedStatusType EditedStatus { get; set; }

        public string Interviewer { get; set; }
    }
}
