using Blaise.Nuget.Api.Contracts.Enums;

namespace Blaise.Api.Tests.Behaviour.Models.Questionnaire
{
    public class QuestionnaireModel
    {
        public string Name { get; set; }

        public QuestionnaireStatusType Status { get; set; }
    }
}
