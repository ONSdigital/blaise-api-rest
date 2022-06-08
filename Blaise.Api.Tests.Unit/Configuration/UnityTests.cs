using System;
using Blaise.Api.Configuration;
using Blaise.Api.Controllers;
using NUnit.Framework;

namespace Blaise.Api.Tests.Unit.Configuration
{
    public class UnityTests
    {
        [TestCase(typeof(CatiController))]
        [TestCase(typeof(CatiV2Controller))]
        [TestCase(typeof(HealthController))]
        [TestCase(typeof(InstrumentController))]
        [TestCase(typeof(QuestionnaireController))]
        [TestCase(typeof(InstrumentDataController))]
        [TestCase(typeof(QuestionnaireDataController))]
        [TestCase(typeof(ServerParkController))]
        [TestCase(typeof(ReportingController))]
        [TestCase(typeof(ReportingV2Controller))]
        [TestCase(typeof(UserRoleController))]
        [TestCase(typeof(UserController))]
        public void Given_A_Controller_Type_I_Resolve_The_Controller_Type_Then_All_Dependencies_Are_Resolved(Type controllerType)
        {
            //arrange
            var container = UnityConfig.UnityContainer;

            //act && assert
            Assert.DoesNotThrow(() => container.Resolve(controllerType, controllerType.ToString()));
        }
    }
}
