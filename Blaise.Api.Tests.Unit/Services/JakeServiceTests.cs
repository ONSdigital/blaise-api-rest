using System;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Services;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class JakeServiceTests
    {
        [TestCase("Jamie")]
        [TestCase("Nik")]
        [TestCase("Rich")]
        public void Given_I_Call_HelloJake_With_A_Name_I_Get_Response_Back(string name)
        {
            //Arrange
            var loggerMock = new Mock<ILoggingService>();

            var sut = new JakeService(loggerMock.Object);

            //Act
            var result = sut.HelloJake(name);

            //Assert
            Assert.AreEqual($"hey {name}", result);
        }

        [Test]
        public void Given_I_Call_HelloJake_A_Response_is_Logged()
        {
            //Arrange
            var loggerMock = new Mock<ILoggingService>();
            loggerMock.Setup(l => l.LogInfo(It.IsAny<string>()));

            var sut = new JakeService(loggerMock.Object);

            //Act
            sut.HelloJake("jamie");

            //Assert
            loggerMock.Verify(l => l.LogInfo(It.IsAny<string>()), Times.Once);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Given_I_Call_HelloJake_With_A_Invalid_Value_Then_An_Error_Is_Logged(string name)
        {
            //Arrange
            var loggerMock = new Mock<ILoggingService>();
            loggerMock.Setup(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()));

            var sut = new JakeService(loggerMock.Object);

            //Act
            sut.HelloJake(name);

            //Assert
            loggerMock.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
            loggerMock.Verify(l => l.LogInfo(It.IsAny<string>()), Times.Never);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Given_I_Call_HelloJake_With_A_Invalid_Value_Then_Null_Is_Returned(string name)
        {
            //Arrange
            var loggerMock = new Mock<ILoggingService>();
            loggerMock.Setup(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()));

            var sut = new JakeService(loggerMock.Object);

            //Act
            var result = sut.HelloJake(name);

            //Assert
            Assert.IsNull(result);
        }
    }
}
