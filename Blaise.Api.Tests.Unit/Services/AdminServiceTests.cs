using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Admin;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;
using Moq;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class AdminServiceTests
    {
        private IAdminService _sut;

        private Mock<IBlaiseAdminApi> _blaiseAdminApiMock;
        private Mock<IOpenConnectionModelMapper> _mapperMock;

        [SetUp]
        public void SetUpTests()
        {
            _blaiseAdminApiMock = new Mock<IBlaiseAdminApi>();
            _mapperMock = new Mock<IOpenConnectionModelMapper>();

            _sut = new AdminService(_blaiseAdminApiMock.Object, _mapperMock.Object);
        }

        [Test]
        public void Given_I_Call_GetOpenConnections_Then_A_Correct_List_Of_OpenConnectionDtos_Is_Returned()
        {
            //arrange
            var openConnectionModels = new List<OpenConnectionModel>
            {
                new OpenConnectionModel { ConnectionType = "TestConnection1", Connections = 10}
            };

            _blaiseAdminApiMock.Setup(b => b.OpenConnections()).Returns(openConnectionModels);

            var openConnectionDtos = new List<OpenConnectionDto>
            {
                new OpenConnectionDto { ConnectionType = "TestConnection1", Connections = 10}
            };

            _mapperMock.Setup(m => m.MapTOpenConnectionDtos(openConnectionModels))
                .Returns(openConnectionDtos);
            
            //act
            var result = _sut.GetOpenConnections();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<OpenConnectionDto>>(result);
            Assert.AreSame(openConnectionDtos, result);
        }

        [Test]
        public void Given_I_Call_ResetConnections_Then_The_Correct_Method_Is_Called_On_The_AdminApi()
        {
            //act
            _sut.ResetConnections();

            //assert
            _blaiseAdminApiMock.Verify(v => v.ResetConnections(), Times.Once);
        }
    }
}
