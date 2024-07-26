using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Contracts.Models.Edit;
using Blaise.Nuget.Api.Contracts.Models;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Interfaces.Mappers
{
    public interface IEditingDtoMapper
    {
        EditingDetailsDto MapToEditingDetailsDto(StatNeth.Blaise.API.DataRecord.IDataRecord caseRecord);
    }
}