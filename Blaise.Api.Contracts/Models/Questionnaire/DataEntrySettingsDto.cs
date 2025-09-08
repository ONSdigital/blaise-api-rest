namespace Blaise.Api.Contracts.Models.Questionnaire
{
    public class DataEntrySettingsDto
    {
        public string Type { get; set; }

        public bool SaveSessionOnTimeout { get; set; }

        public bool SaveSessionOnQuit { get; set; }

        public bool DeleteSessionOnTimeout { get; set; }

        public bool DeleteSessionOnQuit { get; set; }

        public int SessionTimeout { get; set; }

        public bool ApplyRecordLocking { get; set; }
    }
}
