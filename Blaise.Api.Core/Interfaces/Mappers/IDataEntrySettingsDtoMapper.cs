namespace Blaise.Api.Core.Interfaces.Mappers
{
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.Questionnaire;
    using Blaise.Nuget.Api.Contracts.Models;

    public interface IDataEntrySettingsDtoMapper
    {
        IEnumerable<DataEntrySettingsDto> MapDataEntrySettingsDtos(
            IEnumerable<DataEntrySettingsModel> dataEntrySettingsModels);
    }
}
