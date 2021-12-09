using Blaise.Nuget.Api.Contracts.Enums;

namespace Blaise.Api.Tests.Models.Questionnaire
{
    public class Questionnaire
    {
        public string Name { get; set; }

        public SurveyStatusType Status { get; set; }
    }
}
