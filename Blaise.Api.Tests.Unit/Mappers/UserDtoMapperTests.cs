namespace Blaise.Api.Tests.Unit.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using Blaise.Api.Contracts.Models.User;
    using Blaise.Api.Core.Mappers;
    using Moq;
    using NUnit.Framework;
    using StatNeth.Blaise.API.ServerManager;

    public class UserDtoMapperTests
    {
        private UserDtoMapper _sut;

        [SetUp]
        public void SetupTests()
        {
            _sut = new UserDtoMapper();
        }

        [Test]
        public void Given_A_List_Of_Users_When_I_Call_MapToUserDtos_Then_A_Correct_List_Of_UserDtos_Are_Returned()
        {
            // arrange
            const string User1Name = "Name1";
            const string Role1 = "Role1";
            var serverParks = new List<string> { "ServerPark1", "ServerPark2" };
            var user1Mock = new Mock<IUser>();
            user1Mock.As<IUser2>();
            user1Mock.Setup(r => r.Name).Returns(User1Name);
            user1Mock.As<IUser2>().Setup(m => m.Role).Returns(Role1);
            var userServerParkCollection = new Mock<IUserServerParkCollection>();
            userServerParkCollection.Setup(s => s.GetEnumerator()).Returns(serverParks.GetEnumerator());
            user1Mock.As<IUser2>().Setup(m => m.ServerParks).Returns(userServerParkCollection.Object);

            const string User2Name = "Name2";
            const string Role2 = "Role2";
            var serverParks2 = new List<string> { "ServerPark3", "ServerPark4" };
            var user2Mock = new Mock<IUser>();
            user2Mock.As<IUser2>();
            user2Mock.Setup(r => r.Name).Returns(User2Name);
            user2Mock.As<IUser2>().Setup(m => m.Role).Returns(Role2);
            var userServerParkCollection2 = new Mock<IUserServerParkCollection>();
            userServerParkCollection2.Setup(s => s.GetEnumerator()).Returns(serverParks2.GetEnumerator());
            user2Mock.As<IUser2>().Setup(m => m.ServerParks).Returns(userServerParkCollection2.Object);

            var users = new List<IUser> { user1Mock.Object, user2Mock.Object };

            // act
            var result = _sut.MapToUserDtos(users).ToList();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<UserDto>>());
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(
                result.Any(
                    r => r.Name == User1Name &&
                    r.Role == Role1 &&
                    r.ServerParks.Contains(serverParks[0]) &&
                    r.ServerParks.Contains(serverParks[1])),
                Is.True);

            Assert.That(
                result.Any(
                    r => r.Name == User2Name &&
                    r.Role == Role2 &&
                    r.ServerParks.Contains(serverParks2[0]) &&
                    r.ServerParks.Contains(serverParks2[1])),
                Is.True);
        }

        [Test]
        public void Given_A_Role_When_I_Call_MapToUserDto_Then_A_Correct_UserDto_Is_Returned()
        {
            // arrange
            const string User1Name = "Name1";
            const string Role1 = "Role1";
            var serverParks = new List<string> { "ServerPark1", "ServerPark2" };

            var user1Mock = new Mock<IUser>();
            user1Mock.As<IUser2>();
            user1Mock.Setup(r => r.Name).Returns(User1Name);
            user1Mock.As<IUser2>().Setup(m => m.Role).Returns(Role1);

            var userServerParkCollection = new Mock<IUserServerParkCollection>();
            userServerParkCollection.Setup(s => s.GetEnumerator()).Returns(serverParks.GetEnumerator());
            user1Mock.As<IUser2>().Setup(m => m.ServerParks).Returns(userServerParkCollection.Object);

            // act
            var result = _sut.MapToUserDto(user1Mock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<UserDto>());
            Assert.That(result.Name, Is.EqualTo(User1Name));
            Assert.That(result.Role, Is.EqualTo(Role1));
            Assert.That(result.ServerParks, Is.EqualTo(serverParks));
        }
    }
}
