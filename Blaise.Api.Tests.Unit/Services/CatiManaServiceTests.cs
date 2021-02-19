using System;
using System.Collections.Generic;
using System.Globalization;
using Blaise.Api.Core.Services;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class CatiManaServiceTests
    {
        private CatiManaService _sut;

        [SetUp]
        public void SetUpTests()
        {
            _sut = new CatiManaService();
        }

        [Test]
        public void Given_I_Call_RemoveCatiManaBlock_Then_The_CatMana_FieldData_Is_Removed()
        {
            //arrange
            var fieldData = new Dictionary<string, string>
            {
                {"CatiMana.CatiAppoint.AppointType", "blah1"},
                {"CatiMana.CatiSlices.DialData[1].WeekDay", "blah2"},
                {"CatiMana.CatiSlices.DialData[1].DialTime", "blah3"},
                {"InterviewerID", "Jambo"}
            };

            //act
            _sut.RemoveCatiManaBlock(fieldData);

            //assert
            Assert.IsNotEmpty(fieldData);
            Assert.AreEqual(1, fieldData.Count);
            Assert.True(fieldData.ContainsKey("InterviewerID"));
            Assert.AreEqual("Jambo", fieldData["InterviewerID"]);
        }

        [Test]
        public void Given_I_Call_RemoveWebNudgedField_Then_The_WebNudged_FieldData_Is_Removed()
        {
            //arrange
            var fieldData = new Dictionary<string, string>
            {
                {"WebNudged", "1"},
                {"InterviewerID", "Jambo"}
            };

            //act
            _sut.RemoveWebNudgedField(fieldData);

            //assert
            Assert.IsNotEmpty(fieldData);
            Assert.AreEqual(1, fieldData.Count);
            Assert.True(fieldData.ContainsKey("InterviewerID"));
            Assert.AreEqual("Jambo", fieldData["InterviewerID"]);
        }

        [Test]
        public void Given_I_Call_AddCatiManaCallItems_Then_The_Existing_Call_Items_Are_Copied_Along_With_New_Values()
        {
            //arrange
            var newFieldData = new Dictionary<string, string>();
            var existingFieldData = new Dictionary<string, string>();

            //act
            _sut.AddCatiManaCallItems(newFieldData, existingFieldData);

            //assert

        }

        [Test]
        public void Given_A_CatiCall_Item_When_I_Call_BuildCatiManaCallItems_Then_The_Field_Is_Inserted_At_Position_One()
        {
            //arrange
            var initialFieldData = new Dictionary<string, string>
            {
                {"CatiMana.CatiCall.RegsCalls[1].WhoMade", "Tel1"},
                {"CatiMana.CatiCall.RegsCalls[1].DayNumber", "1"},
                {"CatiMana.CatiCall.RegsCalls[1].DialTime", "21:00"},
                {"CatiMana.CatiCall.RegsCalls[1].NrOfDials", "2"},
                {"CatiMana.CatiCall.RegsCalls[1].DialResult", "partial"},

                {"CatiMana.CatiCall.RegsCalls[2].WhoMade", "Tel2"},
                {"CatiMana.CatiCall.RegsCalls[2].DayNumber", "2"},
                {"CatiMana.CatiCall.RegsCalls[2].DialTime", "22:00"},
                {"CatiMana.CatiCall.RegsCalls[2].NrOfDials", "3"},
                {"CatiMana.CatiCall.RegsCalls[2].DialResult", "partial"},

                {"CatiMana.CatiCall.RegsCalls[3].WhoMade", "Tel3"},
                {"CatiMana.CatiCall.RegsCalls[3].DayNumber", "3"},
                {"CatiMana.CatiCall.RegsCalls[3].DialTime", "22:30"},
                {"CatiMana.CatiCall.RegsCalls[3].NrOfDials", "4"},
                {"CatiMana.CatiCall.RegsCalls[3].DialResult", "partial"},

                {"CatiMana.CatiCall.RegsCalls[4].WhoMade", "Tel4"},
                {"CatiMana.CatiCall.RegsCalls[4].DayNumber", "4"},
                {"CatiMana.CatiCall.RegsCalls[4].DialTime", "23:00"},
                {"CatiMana.CatiCall.RegsCalls[4].NrOfDials", "5"},
                {"CatiMana.CatiCall.RegsCalls[4].DialResult", "partial"},

                {"CatiMana.CatiCall.RegsCalls[5].WhoMade", "Tel1"},
                {"CatiMana.CatiCall.RegsCalls[5].DayNumber", "1"},
                {"CatiMana.CatiCall.RegsCalls[5].DialTime", "21:00"},
                {"CatiMana.CatiCall.RegsCalls[5].NrOfDials", "2"},
                {"CatiMana.CatiCall.RegsCalls[5].DialResult", "partial"}
            };

            const string whoMade = "Me";
            const string dayNumber = "2";
            var dialTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            const string numberOfDials = "1";
            const string dialResult = "Complete";
            
            var expectedFieldData = new Dictionary<string, string>
            {
                {"CatiMana.CatiCall.RegsCalls[1].WhoMade", whoMade},
                {"CatiMana.CatiCall.RegsCalls[1].DayNumber", dayNumber},
                {"CatiMana.CatiCall.RegsCalls[1].DialTime", dialTime},
                {"CatiMana.CatiCall.RegsCalls[1].NrOfDials", numberOfDials},
                {"CatiMana.CatiCall.RegsCalls[1].DialResult", dialResult},

                {"CatiMana.CatiCall.RegsCalls[2].WhoMade", "Tel1"},
                {"CatiMana.CatiCall.RegsCalls[2].DayNumber", "1"},
                {"CatiMana.CatiCall.RegsCalls[2].DialTime", "21:00"},
                {"CatiMana.CatiCall.RegsCalls[2].NrOfDials", "2"},
                {"CatiMana.CatiCall.RegsCalls[2].DialResult", "partial"},

                {"CatiMana.CatiCall.RegsCalls[3].WhoMade", "Tel2"},
                {"CatiMana.CatiCall.RegsCalls[3].DayNumber", "2"},
                {"CatiMana.CatiCall.RegsCalls[3].DialTime", "22:00"},
                {"CatiMana.CatiCall.RegsCalls[3].NrOfDials", "3"},
                {"CatiMana.CatiCall.RegsCalls[3].DialResult", "partial"},

                {"CatiMana.CatiCall.RegsCalls[4].WhoMade", "Tel3"},
                {"CatiMana.CatiCall.RegsCalls[4].DayNumber", "3"},
                {"CatiMana.CatiCall.RegsCalls[4].DialTime", "22:30"},
                {"CatiMana.CatiCall.RegsCalls[4].NrOfDials", "4"},
                {"CatiMana.CatiCall.RegsCalls[4].DialResult", "partial"},

                //always remain the same which is the first call
                {"CatiMana.CatiCall.RegsCalls[5].WhoMade", "Tel1"},
                {"CatiMana.CatiCall.RegsCalls[5].DayNumber", "1"},
                {"CatiMana.CatiCall.RegsCalls[5].DialTime", "21:00"},
                {"CatiMana.CatiCall.RegsCalls[5].NrOfDials", "2"},
                {"CatiMana.CatiCall.RegsCalls[5].DialResult", "partial"}
            };

            //act
            var result = _sut.BuildCatiManaCallItems(initialFieldData, whoMade, dayNumber, 
                dialTime, numberOfDials, dialResult);

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(expectedFieldData.Count, result.Count);

            foreach (var fieldData in expectedFieldData)
            {
                Assert.AreEqual(fieldData.Value, result[fieldData.Key]);
            }
        }
    }
}
