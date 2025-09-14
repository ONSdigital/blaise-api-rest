namespace Blaise.Api.Contracts.Models.ServerPark
{
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.Questionnaire;

    public class ServerParkDto
    {
        public string Name { get; set; }

        public string LoadBalancer { get; set; }

        public IEnumerable<QuestionnaireDto> Questionnaires { get; set; } = new List<QuestionnaireDto>();

        public IEnumerable<ServerDto> Servers { get; set; }
    }
}
