using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Instrument;
using Blaise.Nuget.Api.Contracts.Models;

namespace Blaise.Api.Core.Interfaces.Mappers
{
    public interface IDataEntrySettingsDtoMapper
    {
        IEnumerable<DataEntrySettingsDto> MapDataEntrySettingsDtos(
            IEnumerable<DataEntrySettingsModel> dataEntrySettingsModels);
    }
}