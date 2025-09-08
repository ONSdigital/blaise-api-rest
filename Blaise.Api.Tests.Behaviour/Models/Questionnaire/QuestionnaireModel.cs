namespace Blaise.Api.Tests.Behaviour.Models.Questionnaire
{
    using Blaise.Nuget.Api.Contracts.Enums;

    public class QuestionnaireModel
    {
        public string Name { get; set; }

        public QuestionnaireStatusType Status { get; set; }
    }
}
