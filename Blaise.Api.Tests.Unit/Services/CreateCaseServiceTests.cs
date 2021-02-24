using System.Collections.Generic;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Enums;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.DataRecord;

namespace Blaise.Api.Tests.Unit.Services
{
    public class CreateCaseServiceTests
    {
        private Mock<IBlaiseCaseApi> _blaiseApiMock;
        private Mock<ICatiDataService> _catiManaMock;
        private Mock<ILoggingService> _loggingMock;
        private MockSequence _mockSequence;

        private Mock<IDataRecord> _nisraDataRecordMock;

        private readonly string _primaryKey;
        private readonly string _serverParkName;
        private readonly string _instrumentName;
        private Dictionary<string, string> _newFieldData;
        private Dictionary<string, string> _existingFieldData;

        private CreateCaseService _sut;

        public CreateCaseServiceTests()
        {
            _primaryKey = "SN123";
            _serverParkName = "Park1";
            _instrumentName = "OPN123";
        }

        [SetUp]
        public void SetUpTests()
        {
            _blaiseApiMock = new Mock<IBlaiseCaseApi>();
            _loggingMock = new Mock<ILoggingService>();
     
            //set up new record
            _nisraDataRecordMock = new Mock<IDataRecord>();
            _existingFieldData = new Dictionary<string, string>();
            _newFieldData = new Dictionary<string, string>();
            _blaiseApiMock.SetupSequence(b => b.GetRecordDataFields(_nisraDataRecordMock.Object))
                .Returns(_existingFieldData)
                .Returns(_newFieldData);

            //important that the service calls the methods in the right order, otherwise you could end up removing what you have added
            _catiManaMock = new Mock<ICatiDataService>(MockBehavior.Strict);
            _mockSequence = new MockSequence();

            _catiManaMock.InSequence(_mockSequence).Setup(c => c.RemoveCatiManaBlock(_newFieldData));
            _catiManaMock.InSequence(_mockSequence).Setup(c => c.AddCatiManaCallItems(_newFieldData, _existingFieldData,
                It.IsAny<int>()));
            
            _sut = new CreateCaseService(
                _blaiseApiMock.Object,
                _catiManaMock.Object,
                _loggingMock.Object);
        }

        [Test]
        public void Given_A_New_dataRecord_When_I_Call_CreateOnlineCase_Then_The_Case_Is_Created()
        {
            //arrange
            const int outcomeCode = 110;
            _blaiseApiMock.Setup(b => b.GetOutcomeCode(_nisraDataRecordMock.Object)).Returns(outcomeCode);

            //act
            _sut.CreateOnlineCase(_nisraDataRecordMock.Object, _serverParkName, _instrumentName, _primaryKey);

            //assert
            _catiManaMock.Verify(v => v.RemoveCatiManaBlock(_newFieldData), Times.Once);
            _catiManaMock.Verify(v => v.AddCatiManaCallItems(_newFieldData, _existingFieldData, outcomeCode),
                Times.Once);

            _blaiseApiMock.Verify(v => v.CreateCase(_primaryKey, _newFieldData, 
                _instrumentName, _serverParkName), Times.Once);
        }
    }
}
