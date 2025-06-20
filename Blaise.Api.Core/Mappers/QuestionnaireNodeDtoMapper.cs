using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Questionnaire;
using Blaise.Api.Core.Interfaces.Mappers;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Mappers
{
    public class QuestionnaireNodeDtoMapper : IQuestionnaireNodeDtoMapper
    {
        public IEnumerable<QuestionnaireNodeDto> MapToQuestionnaireNodeDtos(IMachineConfigurationCollection questionnaireConfigurations)
        {
            var questionnaireNodes = new List<QuestionnaireNodeDto>();

            if (questionnaireConfigurations == null)
            {
                return questionnaireNodes;
            }

            foreach (var configuration in questionnaireConfigurations)
            {
                questionnaireNodes.Add(new QuestionnaireNodeDto
                {
                    NodeName = configuration.Key,
                    NodeStatus = configuration.Value.Status
                });
            }

            return questionnaireNodes;
        }
    }
}
