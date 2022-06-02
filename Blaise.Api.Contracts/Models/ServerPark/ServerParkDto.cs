using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Questionnaire;

namespace Blaise.Api.Contracts.Models.ServerPark
{
    public class ServerParkDto
    {
        public ServerParkDto()
        {
            Questionnaires = new List<QuestionnaireDto>();
        }

        public string Name { get; set; }

        public string LoadBalancer { get; set; }
        
        public IEnumerable<QuestionnaireDto> Questionnaires { get; set; }

        public IEnumerable<ServerDto> Servers { get; set; }
    }
}
