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
            Assert.NotNull(result);
            Assert.IsInstanceOf<List<DataEntrySettingsDto>>(result);
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
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(2, result.Count);

            var dataEntrySettings1 = result.FirstOrDefault(de => de.Type == "StrictInterviewing");
            Assert.IsNotNull(dataEntrySettings1);
            Assert.IsTrue(dataEntrySettings1.SaveSessionOnTimeout);
            Assert.IsFalse(dataEntrySettings1.SaveSessionOnQuit);
            Assert.IsTrue(dataEntrySettings1.DeleteSessionOnTimeout);
            Assert.IsFalse(dataEntrySettings1.DeleteSessionOnQuit);
            Assert.AreEqual(30, dataEntrySettings1.SessionTimeout);
            Assert.IsTrue(dataEntrySettings1.ApplyRecordLocking);

            var dataEntrySettings2 = result.FirstOrDefault(de => de.Type == "StrictCati");
            Assert.IsNotNull(dataEntrySettings2);
            Assert.IsFalse(dataEntrySettings2.SaveSessionOnTimeout);
            Assert.IsTrue(dataEntrySettings2.SaveSessionOnQuit);
            Assert.IsFalse(dataEntrySettings2.DeleteSessionOnTimeout);
            Assert.IsTrue(dataEntrySettings2.DeleteSessionOnQuit);
            Assert.AreEqual(15, dataEntrySettings2.SessionTimeout);
            Assert.IsFalse(dataEntrySettings2.ApplyRecordLocking);
        }
    }
}
