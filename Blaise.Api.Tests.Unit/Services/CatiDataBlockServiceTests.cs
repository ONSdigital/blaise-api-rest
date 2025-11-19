namespace Blaise.Api.Tests.Unit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Blaise.Api.Core.Services;
    using NUnit.Framework;
    using StatNeth.Blaise.Shared.Util;

    public class CatiDataBlockServiceTests
    {
        private CatiDataBlockService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _sut = new CatiDataBlockService();
        }

        [Test]
        public void Given_FieldData_Containing_A_CatiMana_Block_When_I_Call_RemoveCatiManaBlock_Then_The_CatMana_FieldData_Is_Removed()
        {
            // arrange
            var fieldData = new Dictionary<string, string>
            {
                { "CatiMana.CatiAppoint.AppointType", "blah1" },
                { "CatiMana.CatiSlices.DialData[1].WeekDay", "blah2" },
                { "CatiMana.CatiSlices.DialData[1].DialTime", "blah3" },
                { "InterviewerID", "Jambo" },
            };

            // act
            _sut.RemoveCatiManaBlock(fieldData);

            // assert
            Assert.That(fieldData, Is.Not.Empty);
            Assert.That(fieldData.Count, Is.EqualTo(1));
            Assert.That(fieldData, Contains.Key("InterviewerID"));
            Assert.That(fieldData["InterviewerID"], Is.EqualTo("Jambo"));
        }

        [Test]
        public void Given_FieldData_Containing_A_CallHistory_Block_When_I_Call_RemoveCallHistoryBlock_Then_The_CallHistory_FieldData_Is_Removed()
        {
            // arrange
            var fieldData = new Dictionary<string, string>
            {
                { "CallHistory.ATime", "blah1" },
                { "CallHistory.CaseHist[1].THour", "blah2" },
                { "CallHistory.CaseHist[1].TMinute", "blah3" },
                { "InterviewerID", "Jambo" },
            };

            // act
            _sut.RemoveCallHistoryBlock(fieldData);

            // assert
            Assert.That(fieldData, Is.Not.Empty);
            Assert.That(fieldData.Count, Is.EqualTo(1));
            Assert.That(fieldData, Contains.Key("InterviewerID"));
            Assert.That(fieldData["InterviewerID"], Is.EqualTo("Jambo"));
        }

        [Test]
        public void Given_I_Call_RemoveWebNudgedField_Then_The_WebNudged_FieldData_Is_Removed()
        {
            // arrange
            var fieldData = new Dictionary<string, string>
            {
                { "WebNudged", "1" },
                { "InterviewerID", "Jambo" },
            };

            // act
            _sut.RemoveWebNudgedField(fieldData);

            // assert
            Assert.That(fieldData, Is.Not.Empty);
            Assert.That(fieldData.Count, Is.EqualTo(1));
            Assert.That(fieldData, Contains.Key("InterviewerID"));
            Assert.That(fieldData["InterviewerID"], Is.EqualTo("Jambo"));
        }

        [TestCase("", "1")]
        [TestCase("0", "1")]
        [TestCase("1", "2")]
        [TestCase("3", "4")]
        [TestCase("8", "9")]
        [TestCase("10", "11")]
        public void Given_I_Call_AddCatiManaNrOfCallItem_Then_The_Field_Is_Added_And_Existing_Value_Is_Incremented(
            string initialValue,
            string newValue)
        {
            // arrange
            var existingFieldData = new Dictionary<string, string>
            {
                { "CatiMana.CatiCall.NrOfCall", initialValue },
            };

            var newFieldData = new Dictionary<string, string>();

            // act
            _sut.AddCatiManaNrOfCallItem(newFieldData, existingFieldData);

            // assert
            Assert.That(newFieldData, Is.Not.Empty);
            Assert.That(newFieldData.Count, Is.EqualTo(1));
            Assert.That(newFieldData, Contains.Key("CatiMana.CatiCall.NrOfCall"));
            Assert.That(newFieldData["CatiMana.CatiCall.NrOfCall"], Is.EqualTo(newValue));
        }

        [Test]
        public void Given_FirstDay_Is_Not_Set_When_I_Call_SetFirstDayIfNotSet_Then_Todays_Date_Is_Added_In_The_Correct_Format()
        {
            // arrange
            var existingFieldData = new Dictionary<string, string>
            {
                { "CatiMana.CatiCall.FirstDay", string.Empty },
            };

            var newFieldData = new Dictionary<string, string>();

            // act
            _sut.SetFirstDayIfNotSet(newFieldData, existingFieldData);

            // assert
            Assert.That(newFieldData, Is.Not.Empty);
            Assert.That(newFieldData.Count, Is.EqualTo(1));
            Assert.That(newFieldData, Contains.Key("CatiMana.CatiCall.FirstDay"));

            // throws error if exact format is not used
            var dateTime = DateTime.ParseExact(newFieldData["CatiMana.CatiCall.FirstDay"], "ddMMyyyy", CultureInfo.InvariantCulture);

            Assert.That(dateTime, Is.Not.Null);
            Assert.That(newFieldData["CatiMana.CatiCall.FirstDay"], Is.EqualTo(DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture)));
        }

        [Test]
        public void Given_FirstDay_Is_Set_When_I_Call_SetFirstDayIfNotSet_Then_The_Existing_Date_Is_Carried_Over()
        {
            // arrange
            var existingFieldData = new Dictionary<string, string>
            {
                { "CatiMana.CatiCall.FirstDay", "18022021" },
            };

            var newFieldData = new Dictionary<string, string>();

            // act
            _sut.SetFirstDayIfNotSet(newFieldData, existingFieldData);

            // assert
            Assert.That(newFieldData, Is.Not.Empty);
            Assert.That(newFieldData.Count, Is.EqualTo(1));
            Assert.That(newFieldData, Contains.Key("CatiMana.CatiCall.FirstDay"));
            Assert.That(newFieldData["CatiMana.CatiCall.FirstDay"], Is.EqualTo("18022021"));
        }

        [TestCase(110, "1", "1")]
        [TestCase(210, "6", "2")]
        public void Given_A_CatiCall_Item_When_I_Call_BuildCatiManaRegCallItems_Then_The_Field_Is_Inserted_At_Position_One(
            int outcomeCode,
            string telDialResult,
            string webDialResult)
        {
            // arrange
            var existingFieldData = new Dictionary<string, string>
            {
                { "CatiMana.CatiCall.RegsCalls[1].WhoMade", "Tel1" },
                { "CatiMana.CatiCall.RegsCalls[1].DayNumber", "1" },
                { "CatiMana.CatiCall.RegsCalls[1].DialTime", "12:17:37" },
                { "CatiMana.CatiCall.RegsCalls[1].NrOfDials", "2" },
                { "CatiMana.CatiCall.RegsCalls[1].DialResult", telDialResult },
                { "CatiMana.CatiCall.RegsCalls[2].WhoMade", "Tel2" },
                { "CatiMana.CatiCall.RegsCalls[2].DayNumber", "2" },
                { "CatiMana.CatiCall.RegsCalls[2].DialTime", "15:17:37" },
                { "CatiMana.CatiCall.RegsCalls[2].NrOfDials", "3" },
                { "CatiMana.CatiCall.RegsCalls[2].DialResult", telDialResult },
                { "CatiMana.CatiCall.RegsCalls[3].WhoMade", "Tel3" },
                { "CatiMana.CatiCall.RegsCalls[3].DayNumber", "3" },
                { "CatiMana.CatiCall.RegsCalls[3].DialTime", "17:17:37" },
                { "CatiMana.CatiCall.RegsCalls[3].NrOfDials", "4" },
                { "CatiMana.CatiCall.RegsCalls[3].DialResult", telDialResult },
                { "CatiMana.CatiCall.RegsCalls[4].WhoMade", "Tel4" },
                { "CatiMana.CatiCall.RegsCalls[4].DayNumber", "4" },
                { "CatiMana.CatiCall.RegsCalls[4].DialTime", "19:17:37" },
                { "CatiMana.CatiCall.RegsCalls[4].NrOfDials", "5" },
                { "CatiMana.CatiCall.RegsCalls[4].DialResult", telDialResult },
                { "CatiMana.CatiCall.RegsCalls[5].WhoMade", "Tel1" },
                { "CatiMana.CatiCall.RegsCalls[5].DayNumber", "1" },
                { "CatiMana.CatiCall.RegsCalls[5].DialTime", "12:17:37" },
                { "CatiMana.CatiCall.RegsCalls[5].NrOfDials", "2" },
                { "CatiMana.CatiCall.RegsCalls[5].DialResult", telDialResult },
            };

            var expectedFieldData = new Dictionary<string, string>
            {
                { "CatiMana.CatiCall.RegsCalls[1].WhoMade", "Web" },
                { "CatiMana.CatiCall.RegsCalls[1].DayNumber", "1" },
                { "CatiMana.CatiCall.RegsCalls[1].DialTime", $"{DateTime.Now:HH:mm:ss}" },
                { "CatiMana.CatiCall.RegsCalls[1].NrOfDials", "1" },
                { "CatiMana.CatiCall.RegsCalls[1].DialResult", webDialResult },
                { "CatiMana.CatiCall.RegsCalls[2].WhoMade", "Tel1" },
                { "CatiMana.CatiCall.RegsCalls[2].DayNumber", "1" },
                { "CatiMana.CatiCall.RegsCalls[2].DialTime", "12:17:37" },
                { "CatiMana.CatiCall.RegsCalls[2].NrOfDials", "2" },
                { "CatiMana.CatiCall.RegsCalls[2].DialResult", telDialResult },
                { "CatiMana.CatiCall.RegsCalls[3].WhoMade", "Tel2" },
                { "CatiMana.CatiCall.RegsCalls[3].DayNumber", "2" },
                { "CatiMana.CatiCall.RegsCalls[3].DialTime", "15:17:37" },
                { "CatiMana.CatiCall.RegsCalls[3].NrOfDials", "3" },
                { "CatiMana.CatiCall.RegsCalls[3].DialResult", telDialResult },
                { "CatiMana.CatiCall.RegsCalls[4].WhoMade", "Tel3" },
                { "CatiMana.CatiCall.RegsCalls[4].DayNumber", "3" },
                { "CatiMana.CatiCall.RegsCalls[4].DialTime", "17:17:37" },
                { "CatiMana.CatiCall.RegsCalls[4].NrOfDials", "4" },
                { "CatiMana.CatiCall.RegsCalls[4].DialResult", telDialResult },

                // always remain the same which is the first call
                { "CatiMana.CatiCall.RegsCalls[5].WhoMade", "Tel1" },
                { "CatiMana.CatiCall.RegsCalls[5].DayNumber", "1" },
                { "CatiMana.CatiCall.RegsCalls[5].DialTime", "12:17:37" },
                { "CatiMana.CatiCall.RegsCalls[5].NrOfDials", "2" },
                { "CatiMana.CatiCall.RegsCalls[5].DialResult", telDialResult },
            };

            // act
            var result = _sut.BuildCatiManaRegCallItems(existingFieldData, outcomeCode);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(expectedFieldData.Count));

            foreach (var fieldData in expectedFieldData)
            {
                if (fieldData.Key == "CatiMana.CatiCall.RegsCalls[1].DialTime")
                {
                    // throws error if exact format is not used
                    var dateTime = DateTime.ParseExact(fieldData.Value, "HH:mm:ss", CultureInfo.InvariantCulture);
                    Assert.That(dateTime, Is.Not.Null);
                }

                Assert.That(result[fieldData.Key], Is.EqualTo(fieldData.Value));
            }
        }

        [TestCase(110, "1")]
        [TestCase(210, "2")]
        public void Given_No_Existing_CatiMana_Data_When_I_Call_AddCatiManaCallItems_Then_The_Correct_Items_Are_Added(
            int outcomeCode,
            string webDialResult)
        {
            // arrange
            var existingFieldData = new Dictionary<string, string>
            {
                { "CatiMana.CatiCall.NrOfCall", string.Empty },
                { "CatiMana.CatiCall.FirstDay", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[1].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[1].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[1].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[1].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[1].DialResult", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].DialResult", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].DialResult", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].DialResult", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[5].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[5].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[5].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[5].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[5].DialResult", string.Empty },
            };

            var expectedFieldData = new Dictionary<string, string>
            {
                { "CatiMana.CatiCall.NrOfCall", "1" },
                { "CatiMana.CatiCall.FirstDay", $"{DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture)}" },
                { "CatiMana.CatiCall.RegsCalls[1].WhoMade", "Web" },
                { "CatiMana.CatiCall.RegsCalls[1].DayNumber", "1" },
                { "CatiMana.CatiCall.RegsCalls[1].DialTime", $"{DateTime.Now:HH:mm:ss}" },
                { "CatiMana.CatiCall.RegsCalls[1].NrOfDials", "1" },
                { "CatiMana.CatiCall.RegsCalls[1].DialResult", webDialResult },
                { "CatiMana.CatiCall.RegsCalls[2].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].DialResult", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].DialResult", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].DialResult", string.Empty },

                // always the same which is the first call
                { "CatiMana.CatiCall.RegsCalls[5].WhoMade", "Web" },
                { "CatiMana.CatiCall.RegsCalls[5].DayNumber", "1" },
                { "CatiMana.CatiCall.RegsCalls[5].DialTime", $"{DateTime.Now:HH:mm:ss}" },
                { "CatiMana.CatiCall.RegsCalls[5].NrOfDials", "1" },
                { "CatiMana.CatiCall.RegsCalls[5].DialResult", webDialResult },
            };

            var newFieldData = new Dictionary<string, string>();

            // act
            _sut.AddCatiManaCallItems(newFieldData, existingFieldData, outcomeCode);

            // assert
            Assert.That(newFieldData, Is.Not.Null);
            Assert.That(newFieldData, Is.Not.Empty);
            Assert.That(newFieldData.Count, Is.EqualTo(expectedFieldData.Count));

            foreach (var fieldData in expectedFieldData)
            {
                if (fieldData.Key == "CatiMana.CatiCall.RegsCalls[1].DialTime")
                {
                    // throws error if exact format is not used
                    var dateTime = DateTime.ParseExact(fieldData.Value, "HH:mm:ss", CultureInfo.InvariantCulture);
                    Assert.That(dateTime, Is.Not.Null);
                }

                Assert.That(newFieldData[fieldData.Key], Is.EqualTo(fieldData.Value));
            }
        }

        [TestCase(110, "1")]
        [TestCase(210, "2")]
        public void Given_A_New_Online_Case_When_I_Call_AddCatiManaCallItems_Then_The_Correct_Items_Are_Added(
            int outcomeCode,
            string webDialResult)
        {
            // arrange
            var existingFieldData = new Dictionary<string, string>
            {
                // non cati mana data fields should be preserved
                { "InterviewerID", "Jambo" },

                // cati mana
                { "CatiMana.CatiCall.NrOfCall", string.Empty },
                { "CatiMana.CatiCall.FirstDay", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[1].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[1].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[1].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[1].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[1].DialResult", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].DialResult", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].DialResult", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].DialResult", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[5].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[5].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[5].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[5].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[5].DialResult", string.Empty },
            };

            var expectedFieldData = new Dictionary<string, string>
            {
                { "InterviewerID", "Jambo" },
                { "CatiMana.CatiCall.NrOfCall", "1" },
                { "CatiMana.CatiCall.FirstDay", $"{DateTime.Now.ToString("ddMMyyyy", CultureInfo.InvariantCulture)}" },
                { "CatiMana.CatiCall.RegsCalls[1].WhoMade", "Web" },
                { "CatiMana.CatiCall.RegsCalls[1].DayNumber", "1" },
                { "CatiMana.CatiCall.RegsCalls[1].DialTime", $"{DateTime.Now:HH:mm:ss}" },
                { "CatiMana.CatiCall.RegsCalls[1].NrOfDials", "1" },
                { "CatiMana.CatiCall.RegsCalls[1].DialResult", webDialResult },
                { "CatiMana.CatiCall.RegsCalls[2].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[2].DialResult", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[3].DialResult", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].WhoMade", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].DayNumber", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].DialTime", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].NrOfDials", string.Empty },
                { "CatiMana.CatiCall.RegsCalls[4].DialResult", string.Empty },

                // always the same which is the first call
                { "CatiMana.CatiCall.RegsCalls[5].WhoMade", "Web" },
                { "CatiMana.CatiCall.RegsCalls[5].DayNumber", "1" },
                { "CatiMana.CatiCall.RegsCalls[5].DialTime", $"{DateTime.Now:HH:mm:ss}" },
                { "CatiMana.CatiCall.RegsCalls[5].NrOfDials", "1" },
                { "CatiMana.CatiCall.RegsCalls[5].DialResult", webDialResult },
            };

            // data with cati mana block removed
            var newFieldData = new Dictionary<string, string>
            {
                { "InterviewerID", "Jambo" },
            };

            // act
            _sut.AddCatiManaCallItems(newFieldData, existingFieldData, outcomeCode);

            // assert
            Assert.That(newFieldData, Is.Not.Null);
            Assert.That(newFieldData, Is.Not.Empty);
            Assert.That(newFieldData.Count, Is.EqualTo(expectedFieldData.Count));

            foreach (var fieldData in expectedFieldData)
            {
                if (fieldData.Key == "CatiMana.CatiCall.RegsCalls[1].DialTime")
                {
                    // throws error if exact format is not used
                    var dateTime = DateTime.ParseExact(fieldData.Value, "HH:mm:ss", CultureInfo.InvariantCulture);
                    Assert.That(dateTime, Is.Not.Null);
                }

                Assert.That(newFieldData[fieldData.Key], Is.EqualTo(fieldData.Value));
            }
        }
    }
}
