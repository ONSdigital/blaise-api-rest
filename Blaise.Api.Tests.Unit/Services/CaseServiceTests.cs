using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Cati;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Services
{
    public class CaseServiceTests
    {
        private ICaseService _sut;
        private IBlaiseCaseApi _blaiseCaseApi;

        public CaseServiceTests(IBlaiseCaseApi blaiseCaseApi)
        {
            _blaiseCaseApi = blaiseCaseApi;
        }

        [SetUp]
        public void SetUpTests()
        {
            _sut = new CaseService(_blaiseCaseApi);
        }

        [TestCase("localdevelopment", "OPN2101A")]
        public void Given_An_Instrument_Has_Two_Cases_When_I_Call_GetCaseIds_Then_I_Get_A_List_Containing_Two_CaseIds_Back(string serverParkName, string instrumentName)
        {
            //arrange

            //act
            var result = _sut.GetCaseIds(serverParkName, instrumentName);

            //assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Given_An_Instrument_Has_No_Cases_When_I_Call_CaseIds_Then_I_Get_An_Empty_List_Back()
        {
            //arrange

            //act

            //assert

        }
    }
}
