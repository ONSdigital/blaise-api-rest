using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Unit.Services
{
    public class CaseServiceTests
    {
        private ICaseService _sut;
        private IBlaiseCaseApi _blaiseCaseApi;


        [SetUp]
        public void SetUpTests()
        {
            _sut = new CaseService(_blaiseCaseApi);
        }

        [Test]
        public void Given_I_Call_GetCaseIds_Then_I_Get_A_List_Of_CaseIds_Back()
        {
            //arrange


            //act
            var result = _sut.GetCaseIds(serverParkName, instrumentName);

            //assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<CatiInstrumentDto>>(result);
        }
    }
}
