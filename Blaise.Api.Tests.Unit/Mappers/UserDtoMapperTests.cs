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
        public void Given_A_List_Of_UsersWhen_I_Call_MapToUserDtos_Then_A_Correct_List_Of_UserDtos_Are_Returned()
        {
            // arrange
            const string user1Name = "Name1";
            const string role1 = "Role1";
            var serverParks = new List<string> { "ServerPark1", "ServerPark2" };
            var user1Mock = new Mock<IUser>();
            user1Mock.As<IUser2>();
            user1Mock.Setup(r => r.Name).Returns(user1Name);
            user1Mock.As<IUser2>().Setup(m => m.Role).Returns(role1);
            var userServerParkCollection = new Mock<IUserServerParkCollection>();
            userServerParkCollection.Setup(s => s.GetEnumerator()).Returns(serverParks.GetEnumerator());
            user1Mock.As<IUser2>().Setup(m => m.ServerParks).Returns(userServerParkCollection.Object);

            const string user2Name = "Name2";
            const string role2 = "Role2";
            var serverParks2 = new List<string> { "ServerPark3", "ServerPark4" };
            var user2Mock = new Mock<IUser>();
            user2Mock.As<IUser2>();
            user2Mock.Setup(r => r.Name).Returns(user2Name);
            user2Mock.As<IUser2>().Setup(m => m.Role).Returns(role2);
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
                    r => r.Name == user1Name &&
                    r.Role == role1 &&
                    r.ServerParks.Contains(serverParks[0]) &&
                    r.ServerParks.Contains(serverParks[1])),
                Is.True);

            Assert.That(
                result.Any(
                    r => r.Name == user2Name &&
                    r.Role == role2 &&
                    r.ServerParks.Contains(serverParks2[0]) &&
                    r.ServerParks.Contains(serverParks2[1])),
                Is.True);
        }

        [Test]
        public void Given_A_Role_When_I_Call_MapToUserDto_Then_A_Correct_UserDto_Is_Returned()
        {
            // arrange
            const string user1Name = "Name1";
            const string role1 = "Role1";
            var serverParks = new List<string> { "ServerPark1", "ServerPark2" };

            var user1Mock = new Mock<IUser>();
            user1Mock.As<IUser2>();
            user1Mock.Setup(r => r.Name).Returns(user1Name);
            user1Mock.As<IUser2>().Setup(m => m.Role).Returns(role1);

            var userServerParkCollection = new Mock<IUserServerParkCollection>();
            userServerParkCollection.Setup(s => s.GetEnumerator()).Returns(serverParks.GetEnumerator());
            user1Mock.As<IUser2>().Setup(m => m.ServerParks).Returns(userServerParkCollection.Object);

            // act
            var result = _sut.MapToUserDto(user1Mock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<UserDto>());
            Assert.That(result.Name, Is.EqualTo(user1Name));
            Assert.That(result.Role, Is.EqualTo(role1));
            Assert.That(result.ServerParks, Is.EqualTo(serverParks));
        }
    }
}
