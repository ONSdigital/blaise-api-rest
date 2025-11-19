namespace Blaise.Api.Tests.Unit.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using Blaise.Api.Contracts.Models.Questionnaire;
    using Blaise.Api.Core.Mappers;
    using Blaise.Nuget.Api.Contracts.Models;
    using NUnit.Framework;

    public class DataEntrySettingsDtoMapperTests
    {
        private DataEntrySettingsDtoMapper _sut;

        [SetUp]
        public void SetupTests()
        {
            _sut = new DataEntrySettingsDtoMapper();
        }

        [Test]
        public void Given_A_DataEntrySettingsModel_When_I_Call_MapToDataEntrySettingsDto_Then_I_Get_A_List_Of_DataEntrySettingsDtos_Back()
        {
            // act
            var result = _sut.MapDataEntrySettingsDtos(new List<DataEntrySettingsModel>());

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<DataEntrySettingsDto>>());
        }

        [Test]
        public void Given_A_DataEntrySettingsModel_When_I_Call_MapToDataEntrySettingsDto_Then_Properties_Are_Mapped_Correctly()
        {
            // arrange
            var dataEntrySettingsModelList = new List<DataEntrySettingsModel>
            {
                new DataEntrySettingsModel
                {
                    Type = "StrictInterviewing",
                    SaveSessionOnTimeout = true,
                    SaveSessionOnQuit = false,
                    DeleteSessionOnTimeout = true,
                    DeleteSessionOnQuit = false,
                    SessionTimeout = 30,
                    ApplyRecordLocking = true,
                },
                new DataEntrySettingsModel
                {
                    Type = "StrictCati",
                    SaveSessionOnTimeout = false,
                    SaveSessionOnQuit = true,
                    DeleteSessionOnTimeout = false,
                    DeleteSessionOnQuit = true,
                    SessionTimeout = 15,
                    ApplyRecordLocking = false,
                },
            };

            // act
            var result = _sut.MapDataEntrySettingsDtos(dataEntrySettingsModelList).ToList();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(2));

            var dataEntrySettings1 = result.FirstOrDefault(de => de.Type == "StrictInterviewing");
            Assert.That(dataEntrySettings1, Is.Not.Null);
            Assert.That(dataEntrySettings1.SaveSessionOnTimeout, Is.True);
            Assert.That(dataEntrySettings1.SaveSessionOnQuit, Is.False);
            Assert.That(dataEntrySettings1.DeleteSessionOnTimeout, Is.True);
            Assert.That(dataEntrySettings1.DeleteSessionOnQuit, Is.False);
            Assert.That(dataEntrySettings1.SessionTimeout, Is.EqualTo(30));
            Assert.That(dataEntrySettings1.ApplyRecordLocking, Is.True);

            var dataEntrySettings2 = result.FirstOrDefault(de => de.Type == "StrictCati");
            Assert.That(dataEntrySettings2, Is.Not.Null);
            Assert.That(dataEntrySettings2.SaveSessionOnTimeout, Is.False);
            Assert.That(dataEntrySettings2.SaveSessionOnQuit, Is.True);
            Assert.That(dataEntrySettings2.DeleteSessionOnTimeout, Is.False);
            Assert.That(dataEntrySettings2.DeleteSessionOnQuit, Is.True);
            Assert.That(dataEntrySettings2.SessionTimeout, Is.EqualTo(15));
            Assert.That(dataEntrySettings2.ApplyRecordLocking, Is.False);
        }
    }
}
