using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Admin;
using Blaise.Nuget.Api.Contracts.Models;

namespace Blaise.Api.Core.Interfaces.Mappers
{
    public interface IOpenConnectionModelMapper
    {
        IEnumerable<OpenConnectionDto> MapTOpenConnectionDtos(IEnumerable<OpenConnectionModel> openConnectionModels);
    }
}