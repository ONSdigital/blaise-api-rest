using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Questionnaire;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Interfaces.Mappers
{
    public interface IQuestionnaireNodeDtoMapper
    {
        IEnumerable<QuestionnaireNodeDto> MapToQuestionnaireNodeDtos(IMachineConfigurationCollection questionnaireConfigurations);
    }
}
