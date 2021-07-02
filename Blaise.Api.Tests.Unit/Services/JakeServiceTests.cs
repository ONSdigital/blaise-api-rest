using System;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Services;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.Runtime.DataContract.Cati.Actions;

namespace Blaise.Api.Tests.Unit.Services
{
    public class JakeServiceTests
    {

        [TestCase("ello", "Jake")]
        [TestCase("hi", "Aidan")]
        [TestCase("yo", "Sarah")]
        [TestCase("heya", "Callum")]
        public void Given_Customer_Greets_Us_With_A_Greeting_Other_Than_Hello_And_They_Supply_Name_Response_Is_Given(string greeting, string name)
        {
            //Arrange
            var sut = new JakeService();

            //Act
            var response = sut.GreetCustomer(greeting, name);

            //Assert
            Assert.AreEqual($"Hey {name}", response);
        }

        [TestCase("hEllo", "Jake")]
        [TestCase("hellO", "Aidan")]
        [TestCase("Hello", "Sarah")]
        [TestCase("heLlo", "Callum")]
        public void Given_Customer_Greets_Us_With_Hello_And_They_Supply_Name_Response_Is_Given(string greeting, string name)
        {
            //Arrange
            var sut = new JakeService();

            //Act
            var response = sut.GreetCustomer(greeting, name);

            //Assert
            Assert.AreEqual($"Hi {name}", response);
        }

        [TestCase("ello", "")]
        [TestCase("hi", " ")]
        [TestCase("yo", "  ")]
        [TestCase("heya", null)]
        public void Given_Customer_Greets_Us_With_A_Greeting_Other_Than_Hello_And_They_Do_Not_Supply_Name_Response_Is_Given(string greeting, string name)
        {
            //Arrange
            var sut = new JakeService();

            //Act
            var response = sut.GreetCustomer(greeting, name);

            //Assert
            Assert.AreEqual("Good morning", response);
        }

        [TestCase("hello", "Jamie")]
        [TestCase("hey", "Nik")]
        [TestCase("hi", "Elinor")]
        [TestCase("wassup", "Matthew")]
        [TestCase("", "Ali")]
        public void Given_I_Am_On_The_Best_Friends_List_With_Any_Greeting_Response_Is_Given(string greeting, string name)
        {
            //Arrange
            var sut = new JakeService();

            //Act
            var response = sut.GreetCustomer(greeting, name);

            //Assert
            Assert.AreEqual($"Hiya {name}, great to see you!", response);
        }

        [TestCase("HeLlo", "Richmond")]
        [TestCase("yo", "Richmond")]
        [TestCase("heya", "Richmond")]
        [TestCase("hi", "Richmond")]
        public void Given_I_Am_On_The_Naughty_List_With_Any_Greeting_Response_Is_Given(string greeting, string name)
        {
            //Arrange
            var sut = new JakeService();

            //Act
            var response = sut.GreetCustomer(greeting, name);

            //Assert
            Assert.AreEqual("And what do you think you are doing here?!", response);
        }

        [TestCase(" ", null)]
        [TestCase(null, "")]
        [TestCase("   ", " ")]
        [TestCase("", "")]
        public void Given_Customer_Does_Not_Greet_Us_And_Does_Not_Give_A_Name_Response_Is_Given(string greeting, string name)
        {
            //Arrange
            var sut = new JakeService();

            //Act
            var response = sut.GreetCustomer(greeting, name);

            //Assert
            Assert.AreEqual("How may I help you?", response);
        }

        [TestCase(" ", "jami")]
        [TestCase(null, "jake")]
        public void Given_Customer_Does_Not_Greet_Us_And_Does_Give_A_Name_Response_Is_Given(string greeting, string name)
        {
            //Arrange
            var sut = new JakeService();

            //Act
            var response = sut.GreetCustomer(greeting, name);

            //Assert
            Assert.AreEqual($"How may I help you {name}?", response);
        }
    }
}
