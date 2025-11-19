namespace Blaise.Api.Tests.Unit.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Blaise.Api.Contracts.Models.User;
    using Blaise.Api.Core.Interfaces.Mappers;
    using Blaise.Api.Core.Interfaces.Services;
    using Blaise.Api.Core.Services;
    using Blaise.Nuget.Api.Contracts.Interfaces;
    using Moq;
    using NUnit.Framework;
    using StatNeth.Blaise.API.ServerManager;

    public class UserServiceTests
    {
        private IUserService _sut;

        private Mock<IBlaiseUserApi> _blaiseApiMock;
        private Mock<IUserDtoMapper> _mapperMock;

        private string _userName;
        private string _password;
        private string _role;
        private List<string> _serverParks;
        private string _defaultServerPark;

        [SetUp]
        public void SetUpTests()
        {
            _userName = "Admin";
            _password = "Test";
            _role = "Role1";
            _serverParks = new List<string> { "ServerPark1", "ServerPark2" };
            _defaultServerPark = _serverParks.First();

            _blaiseApiMock = new Mock<IBlaiseUserApi>();
            _mapperMock = new Mock<IUserDtoMapper>();

            _sut = new UserService(
                _blaiseApiMock.Object,
                _mapperMock.Object);
        }

        [Test]
        public void Given_I_Call_GetUsers_Then_I_Get_A_Correct_List_Of_UserDtos_Back()
        {
            // arrange
            _blaiseApiMock.Setup(b => b.GetUsers())
                .Returns(new List<IUser>());

            var userDtos = new List<UserDto> { new UserDto() };

            _mapperMock.Setup(m => m.MapToUserDtos(new List<IUser>()))
                .Returns(userDtos);

            // act
            var result = _sut.GetUsers();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(userDtos));
        }

        [Test]
        public void Given_I_Call_GetUser_Then_I_Get_A_RoleDto_Back()
        {
            // arrange
            var userDto = new UserDto();
            var userMock = new Mock<IUser>();

            _blaiseApiMock.Setup(b => b.GetUser(_userName))
                .Returns(userMock.Object);

            _mapperMock.Setup(m => m.MapToUserDto(userMock.Object))
                .Returns(userDto);

            // act
            var result = _sut.GetUser(_userName);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(userDto));
        }

        [Test]
        public void Given_An_Empty_UserName_When_I_Call_GetUser_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetUser(string.Empty));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'userName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_UserName_When_I_Call_GetUser_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetUser(null));
            Assert.That(exception.ParamName, Is.EqualTo("userName"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_Valid_Arguments_When_I_Call_UserExists_Then_The_Correct_Value_Is_Returned(bool exists)
        {
            // arrange
            _blaiseApiMock.Setup(r => r.UserExists(_userName)).Returns(exists);

            // act
            var result = _sut.UserExists(_userName);

            // assert
            Assert.That(result, Is.EqualTo(exists));
        }

        [Test]
        public void Given_An_Empty_UserName_When_I_Call_UserExists_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UserExists(string.Empty));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'userName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_UserName_When_I_Call_UserExists_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UserExists(null));
            Assert.That(exception.ParamName, Is.EqualTo("userName"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_AddUser_Then_The_Correct_Service_Method_Is_Called()
        {
            // arrange
            var addUserDto = new AddUserDto
            {
                Name = _userName,
                Password = _password,
                Role = _role,
                ServerParks = _serverParks,
            };

            // act
            _sut.AddUser(addUserDto);

            // assert
            _blaiseApiMock.Verify(v => v.AddUser(addUserDto.Name, addUserDto.Password, addUserDto.Role, addUserDto.ServerParks, _defaultServerPark), Times.Once);
        }

        [Test]
        public void Given_An_Empty_Name_When_I_Call_AddUser_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var addUserDto = new AddUserDto
            {
                Name = string.Empty,
                Password = _password,
                Role = _role,
                ServerParks = _serverParks,
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddUser(addUserDto));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'addUserDto.Name' must be supplied"));
        }

        [Test]
        public void Given_A_Null_Name_When_I_Call_AddUser_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var addUserDto = new AddUserDto
            {
                Name = null,
                Password = _password,
                Role = _role,
                ServerParks = _serverParks,
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddUser(addUserDto));
            Assert.That(exception.ParamName, Is.EqualTo("addUserDto.Name"));
        }

        [Test]
        public void Given_An_Empty_Password_When_I_Call_AddUser_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var addUserDto = new AddUserDto
            {
                Name = _userName,
                Password = string.Empty,
                Role = _role,
                ServerParks = _serverParks,
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddUser(addUserDto));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'addUserDto.Password' must be supplied"));
        }

        [Test]
        public void Given_A_Null_Password_When_I_Call_AddUser_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var addUserDto = new AddUserDto
            {
                Name = _userName,
                Password = null,
                Role = _role,
                ServerParks = _serverParks,
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddUser(addUserDto));
            Assert.That(exception.ParamName, Is.EqualTo("addUserDto.Password"));
        }

        [Test]
        public void Given_An_Empty_Role_When_I_Call_AddUser_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var addUserDto = new AddUserDto
            {
                Name = _userName,
                Password = _password,
                Role = string.Empty,
                ServerParks = _serverParks,
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddUser(addUserDto));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'addUserDto.Role' must be supplied"));
        }

        [Test]
        public void Given_A_Null_Role_When_I_Call_AddUser_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var addUserDto = new AddUserDto
            {
                Name = _userName,
                Password = _password,
                Role = null,
                ServerParks = _serverParks,
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddUser(addUserDto));
            Assert.That(exception.ParamName, Is.EqualTo("addUserDto.Role"));
        }

        [Test]
        public void Given_An_Empty_ServerPark_List_When_I_Call_AddUser_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var addUserDto = new AddUserDto
            {
                Name = _userName,
                Password = _password,
                Role = _role,
                ServerParks = new List<string>(),
            };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddUser(addUserDto));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'addUserDto.ServerParks' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerPark_List_When_I_Call_AddUser_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var addUserDto = new AddUserDto
            {
                Name = _userName,
                Password = _password,
                Role = _role,
                ServerParks = null,
            };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddUser(addUserDto));
            Assert.That(exception.ParamName, Is.EqualTo("addUserDto.ServerParks"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_UpdatePassword_Then_The_Correct_Service_Method_Is_Called()
        {
            // act
            _sut.UpdatePassword(_userName, _password);

            // assert
            _blaiseApiMock.Verify(v => v.UpdatePassword(_userName, _password), Times.Once);
        }

        [Test]
        public void Given_An_Empty_UserName_When_I_Call_UpdatePassword_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdatePassword(string.Empty, _password));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'userName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_Name_When_I_Call_UpdatePassword_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdatePassword(null, _password));
            Assert.That(exception.ParamName, Is.EqualTo("userName"));
        }

        [Test]
        public void Given_An_Empty_Password_When_I_Call_UpdatePassword_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdatePassword(_userName, string.Empty));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'password' must be supplied"));
        }

        [Test]
        public void Given_A_Null_Password_When_I_Call_UpdatePassword_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdatePassword(_userName, null));
            Assert.That(exception.ParamName, Is.EqualTo("password"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_UpdateUser_Then_The_Correct_Service_Method_Is_Called()
        {
            // arrange
            var updateRoleDto = new UpdateUserRoleDto { Role = _role };

            // act
            _sut.UpdateRole(_userName, updateRoleDto);

            // assert
            _blaiseApiMock.Verify(v => v.UpdateRole(_userName, updateRoleDto.Role), Times.Once);
        }

        [Test]
        public void Given_An_Empty_UserName_When_I_Call_UpdateRole_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var updateRoleDto = new UpdateUserRoleDto { Role = _role };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateRole(string.Empty, updateRoleDto));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'userName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_UserName_When_I_Call_UpdateRole_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var updateUserRoleDto = new UpdateUserRoleDto { Role = _role };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateRole(null, updateUserRoleDto));
            Assert.That(exception.ParamName, Is.EqualTo("userName"));
        }

        [Test]
        public void Given_An_Empty_Role_When_I_Call_UpdateRole_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var updateUserRoleDto = new UpdateUserRoleDto { Role = string.Empty };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateRole(_userName, updateUserRoleDto));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'updateUserRoleDto.Role' must be supplied"));
        }

        [Test]
        public void Given_A_Null_Role_When_I_Call_UpdateRole_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var updateUserRoleDto = new UpdateUserRoleDto { Role = null };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateRole(_userName, updateUserRoleDto));
            Assert.That(exception.ParamName, Is.EqualTo("updateUserRoleDto.Role"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_UpdateServerParks_Then_The_Correct_Service_Method_Is_Called()
        {
            // arrange
            var updateServerParksDto = new UpdateUserServerParksDto { ServerParks = _serverParks };

            // act
            _sut.UpdateServerParks(_userName, updateServerParksDto);

            // assert
            _blaiseApiMock.Verify(v => v.UpdateServerParks(_userName, updateServerParksDto.ServerParks, updateServerParksDto.DefaultServerPark), Times.Once);
        }

        [Test]
        public void Given_An_Empty_UserName_When_I_Call_UpdateServerParks_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var updateServerParksDto = new UpdateUserServerParksDto { ServerParks = _serverParks };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateServerParks(string.Empty, updateServerParksDto));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'userName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_UserName_When_I_Call_UpdateServerParks_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var updateUserServerParksDto = new UpdateUserServerParksDto { ServerParks = _serverParks };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateServerParks(null, updateUserServerParksDto));
            Assert.That(exception.ParamName, Is.EqualTo("userName"));
        }

        [Test]
        public void Given_An_Empty_ServerPark_List_When_I_Call_UpdateServerParks_Then_An_ArgumentException_Is_Thrown()
        {
            // arrange
            var updateUserServerParksDto = new UpdateUserServerParksDto { ServerParks = new List<string>() };

            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateServerParks(_userName, updateUserServerParksDto));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'updateUserServerParksDto.ServerParks' must be supplied"));
        }

        [Test]
        public void Given_A_Null_ServerPark_List_When_I_Call_UpdateServerParks_Then_An_ArgumentNullException_Is_Thrown()
        {
            // arrange
            var updateUserServerParksDto = new UpdateUserServerParksDto { ServerParks = null };

            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateServerParks(_userName, updateUserServerParksDto));
            Assert.That(exception.ParamName, Is.EqualTo("updateUserServerParksDto.ServerParks"));
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_RemoveUser_Then_The_Correct_Service_Method_Is_Called()
        {
            // act
            _sut.RemoveUser(_userName);

            // assert
            _blaiseApiMock.Verify(v => v.RemoveUser(_userName), Times.Once);
        }

        [Test]
        public void Given_An_Empty_UserName_When_I_Call_RemoveUser_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.RemoveUser(string.Empty));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'userName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_UserName_When_I_Call_RemoveUser_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.RemoveUser(null));
            Assert.That(exception.ParamName, Is.EqualTo("userName"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_Valid_Arguments_When_I_Call_ValidateUser_Then_The_Correct_Value_Is_Returned(bool exists)
        {
            // arrange
            _blaiseApiMock.Setup(r => r.ValidateUser(_userName, _password)).Returns(exists);

            // act
            var result = _sut.ValidateUser(_userName, _password);

            // assert
            Assert.That(result, Is.EqualTo(exists));
        }

        [Test]
        public void Given_An_Empty_UserName_When_I_Call_ValidateUser_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.ValidateUser(string.Empty, _password));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'userName' must be supplied"));
        }

        [Test]
        public void Given_A_Null_UserName_When_I_Call_ValidateUser_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.ValidateUser(null, _password));
            Assert.That(exception.ParamName, Is.EqualTo("userName"));
        }

        [Test]
        public void Given_An_Empty_Password_When_I_Call_ValidateUser_Then_An_ArgumentException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.ValidateUser(_userName, string.Empty));
            Assert.That(exception.Message, Is.EqualTo("A value for the argument 'password' must be supplied"));
        }

        [Test]
        public void Given_A_Null_Password_When_I_Call_ValidateUser_Then_An_ArgumentNullException_Is_Thrown()
        {
            // act and assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.ValidateUser(_userName, null));
            Assert.That(exception.ParamName, Is.EqualTo("password"));
        }
    }
}
