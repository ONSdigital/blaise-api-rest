using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Admin;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Mappers;
using NUnit.Framework;
using Blaise.Nuget.Api.Contracts.Models;

namespace Blaise.Api.Tests.Unit.Mappers
{
    public class OpenConnectionModelMapperTests
    {
        private IOpenConnectionModelMapper _sut;

        [SetUp]
        public void SetUpTests()
        {
            _sut = new OpenConnectionModelMapper();
        }

        [Test]
        public void Given_I_Call_MapTOpenConnectionDtos_With_A_Null_Value_Then_An_Empty_List_Of_OpenConnectionDtos_Is_Returned()
        {
            //act
            var result = _sut.MapTOpenConnectionDtos(null); ;

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<OpenConnectionDto>>(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void Given_I_Have_A_Empty_List_Of_OpenConnectionModels_When_I_Call_MapTOpenConnectionDtos_Then_An_Empty_List_Of_OpenConnectionDtos_Is_Returned()
        {
            //arrange
            var openConnectionModels = new List<OpenConnectionModel>();

            //act
            var result = _sut.MapTOpenConnectionDtos(openConnectionModels);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<OpenConnectionDto>>(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public void Given_I_Have_A_Populated_List_Of_OpenConnectionModels_When_I_Call_MapTOpenConnectionDtos_Then_A_Correct_List_Of_OpenConnectionDtos_Is_Returned()
        {
            //arrange
            var openConnectionModels = new List<OpenConnectionModel>
            {
                new OpenConnectionModel {
                    ConnectionType = "TestConnection1",
                    NumberOfConnections = 2,
                    Connections = new Dictionary<string, DateTime>
                        {
                            { "Key1", DateTime.Now.AddMinutes(-30) },
                            { "Key2", DateTime.Now.AddMinutes(-60) },
                        }
                },
                new OpenConnectionModel {
                    ConnectionType = "TestConnection2",
                    NumberOfConnections = 2,
                    Connections = new Dictionary<string, DateTime>
                    {
                        { "Key1", DateTime.Now.AddMinutes(-20) },
                        { "Key2", DateTime.Now.AddMinutes(-40) },
                    }
                }
            };

            //act
            var result = _sut.MapTOpenConnectionDtos(openConnectionModels).ToList();

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<IEnumerable<OpenConnectionDto>>(result);
            Assert.AreEqual(2, result.Count);
            Assert.True(result.Any(c =>
                c.ConnectionType == openConnectionModels[0].ConnectionType && c.Connections == openConnectionModels[0].Connections && Equals(c.NumberOfConnections, openConnectionModels[0].NumberOfConnections)));
            Assert.True(result.Any(c =>
                c.ConnectionType == openConnectionModels[1].ConnectionType && c.Connections == openConnectionModels[1].Connections && Equals(c.NumberOfConnections, openConnectionModels[1].NumberOfConnections)));
        }
    }
}
