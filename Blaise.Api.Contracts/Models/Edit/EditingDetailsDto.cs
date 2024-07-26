namespace Blaise.Api.Contracts.Models.Edit
{
    public class EditingDetailsDto
    {
        public string PrimaryKey { get; set; }

        public int Outcome { get; set; }

        public string AssignedTo { get; set; }

        public string EditedStatus { get; set; }

        public string Interviewer { get; set; }
    }
}
