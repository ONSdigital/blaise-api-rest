using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Enums;
using Blaise.Api.Contracts.Models.Health;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class HealthCheckServiceTests
    {
        private HealthCheckService _sut;

        private Mock<IBlaiseHealthApi> _blaiseApiMock;

        [SetUp]
        public void SetupTests()
        {
            _blaiseApiMock = new Mock<IBlaiseHealthApi>();
            _blaiseApiMock.Setup(b => b.ConnectionModelIsHealthy()).Returns(true);
            _blaiseApiMock.Setup(b => b.ConnectionToBlaiseIsHealthy()).Returns(true);
            _blaiseApiMock.Setup(b => b.RemoteConnectionToBlaiseIsHealthy()).Returns(true);
            _blaiseApiMock.Setup(b => b.RemoteConnectionToCatiIsHealthy()).Returns(true);

            _sut = new HealthCheckService(_blaiseApiMock.Object);
        }

        [Test]
        public void Given_Blaise_Is_Up_When_I_Call_PerformCheck_Then_A_List_Of_HealthResultDtos_Are_Returned()
        {
            //act
            var result = _sut.PerformCheck().ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<HealthCheckResultDto>>(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(4, result.Count);
        }

        [Test]
        public void Given_Blaise_Is_Up_When_I_Call_PerformCheck_Then_A_Correct_List_Of_HealthResultDtos_Are_Returned()
        {
            //act
            var result = _sut.PerformCheck().ToList();

            //assert
            Assert.IsNotNull(result);

            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.ConnectionModel && r.StatusType == HealthStatusType.Ok));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.Connection && r.StatusType == HealthStatusType.Ok));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.RemoteDataServer && r.StatusType == HealthStatusType.Ok));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.RemoteCatiManagement && r.StatusType == HealthStatusType.Ok));
        }

        [Test]
        public void Given_An_Invalid_ConnectionModel_When_I_Call_PerformCheck_Then_A_Correct_List_Of_HealthResultDtos_Are_Returned()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.ConnectionModelIsHealthy()).Returns(false);

            //act
            var result = _sut.PerformCheck().ToList();

            //assert
            Assert.IsNotNull(result);

            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.ConnectionModel && r.StatusType == HealthStatusType.Error));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.Connection && r.StatusType == HealthStatusType.Ok));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.RemoteDataServer && r.StatusType == HealthStatusType.Ok));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.RemoteCatiManagement && r.StatusType == HealthStatusType.Ok));
        }

        [Test]
        public void Given_Blaise_Connection_Is_Down_When_I_Call_PerformCheck_Then_A_Correct_List_Of_HealthResultDtos_Are_Returned()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.ConnectionToBlaiseIsHealthy()).Returns(false);

            //act
            var result = _sut.PerformCheck().ToList();

            //assert
            Assert.IsNotNull(result);

            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.ConnectionModel && r.StatusType == HealthStatusType.Ok));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.Connection && r.StatusType == HealthStatusType.Error));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.RemoteDataServer && r.StatusType == HealthStatusType.Ok));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.RemoteCatiManagement && r.StatusType == HealthStatusType.Ok));
        }

        [Test]
        public void Given_Blaise_Remote_Connection_Is_Down_When_I_Call_PerformCheck_Then_A_Correct_List_Of_HealthResultDtos_Are_Returned()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.RemoteConnectionToBlaiseIsHealthy()).Returns(false);

            //act
            var result = _sut.PerformCheck().ToList();

            //assert
            Assert.IsNotNull(result);

            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.ConnectionModel && r.StatusType == HealthStatusType.Ok));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.Connection && r.StatusType == HealthStatusType.Ok));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.RemoteDataServer && r.StatusType == HealthStatusType.Error));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.RemoteCatiManagement && r.StatusType == HealthStatusType.Ok));
        }

        [Test]
        public void Given_Blaise_Remote_Cati_Connection_Is_Down_When_I_Call_PerformCheck_Then_A_Correct_List_Of_HealthResultDtos_Are_Returned()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.RemoteConnectionToCatiIsHealthy()).Returns(false);

            //act
            var result = _sut.PerformCheck().ToList();

            //assert
            Assert.IsNotNull(result);

            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.ConnectionModel && r.StatusType == HealthStatusType.Ok));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.Connection && r.StatusType == HealthStatusType.Ok));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.RemoteDataServer && r.StatusType == HealthStatusType.Ok));
            Assert.IsTrue(result.Any(r => r.CheckType == HealthCheckType.RemoteCatiManagement && r.StatusType == HealthStatusType.Error));
        }
    }
}
