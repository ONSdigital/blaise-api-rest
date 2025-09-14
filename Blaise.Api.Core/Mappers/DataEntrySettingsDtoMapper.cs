namespace Blaise.Api.Core.Mappers
{
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.Questionnaire;
    using Blaise.Api.Core.Interfaces.Mappers;
    using Blaise.Nuget.Api.Contracts.Models;

    public class DataEntrySettingsDtoMapper : IDataEntrySettingsDtoMapper
    {
        public IEnumerable<DataEntrySettingsDto> MapDataEntrySettingsDtos(IEnumerable<DataEntrySettingsModel> dataEntrySettingsModels)
        {
            var dataEntrySettingsDto = new List<DataEntrySettingsDto>();

            foreach (var dataEntrySettingsModel in dataEntrySettingsModels)
            {
                dataEntrySettingsDto.Add(new DataEntrySettingsDto
                {
                    Type = dataEntrySettingsModel.Type,
                    SaveSessionOnTimeout = dataEntrySettingsModel.SaveSessionOnTimeout,
                    SaveSessionOnQuit = dataEntrySettingsModel.SaveSessionOnQuit,
                    DeleteSessionOnTimeout = dataEntrySettingsModel.DeleteSessionOnTimeout,
                    DeleteSessionOnQuit = dataEntrySettingsModel.DeleteSessionOnQuit,
                    SessionTimeout = dataEntrySettingsModel.SessionTimeout,
                    ApplyRecordLocking = dataEntrySettingsModel.ApplyRecordLocking,
                });
            }

            return dataEntrySettingsDto;
        }
    }
}
