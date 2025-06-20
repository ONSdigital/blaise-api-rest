using System;
using System.Collections.Generic;
using Blaise.Api.Contracts.Models.UserRole;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Api.Core.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Moq;
using NUnit.Framework;
using StatNeth.Blaise.API.Security;

namespace Blaise.Api.Tests.Unit.Services
{
    public class UserRoleServiceTests
    {
        private IUserRoleService _sut;

        private Mock<IBlaiseRoleApi> _blaiseApiMock;
        private Mock<IUserRoleDtoMapper> _mapperMock;

        private string _name;
        private string _description;
        private List<string> _permissions;

        private UserRoleDto _roleDto;

        [SetUp]
        public void SetUpTests()
        {
            _name = "Admin";
            _description = "Test";
            _permissions = new List<string> { "Permission1" };

            _roleDto = new UserRoleDto
            {
                Name = _name,
                Description = _description,
                Permissions = _permissions
            };

            _blaiseApiMock = new Mock<IBlaiseRoleApi>();
            _mapperMock = new Mock<IUserRoleDtoMapper>();

            _sut = new UserRoleService(
                _blaiseApiMock.Object,
                _mapperMock.Object);
        }

        [Test]
        public void Given_I_Call_GetUserRoles_Then_I_Get_A_Correct_List_Of_UserRoleDtos_Back()
        {
            //arrange
            _blaiseApiMock.Setup(b => b.GetRoles())
                .Returns(new List<IRole>());

            var roleDtos = new List<UserRoleDto> { _roleDto };

            _mapperMock.Setup(m => m.MapToUserRoleDtos(new List<IRole>()))
                .Returns(roleDtos);
            //act
            var result = _sut.GetUserRoles();

            //assert
            Assert.IsNotNull(result);
            Assert.AreSame(roleDtos, result);
        }

        [Test]
        public void Given_I_Call_GetUserRole_Then_I_Get_A_UserRoleDto_Back()
        {
            //arrange
            var roleMock = new Mock<IRole>();

            _blaiseApiMock.Setup(b => b.GetRole(_name))
                .Returns(roleMock.Object);

            _mapperMock.Setup(m => m.MapToUserRoleDto(roleMock.Object))
                .Returns(_roleDto);
            //act
            var result = _sut.GetUserRole(_name);

            //assert
            Assert.IsNotNull(result);
            Assert.AreSame(_roleDto, result);
        }

        [Test]
        public void Given_An_Empty_Name_When_I_Call_GetUserRole_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.GetUserRole(string.Empty));
            Assert.AreEqual("A value for the argument 'name' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_Name_When_I_Call_GetRole_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.GetUserRole(null));
            Assert.AreEqual("name", exception.ParamName);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Given_Valid_Arguments_When_I_Call_UserRoleExists_Then_The_Correct_Value_Is_Returned(bool exists)
        {
            //arrange

            _blaiseApiMock.Setup(r => r.RoleExists(_name)).Returns(exists);

            //act
            var result = _sut.UserRoleExists(_name);

            //assert
            Assert.AreEqual(exists, result);
        }

        [Test]
        public void Given_An_Empty_Name_When_I_Call_UserRoleExists_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UserRoleExists(string.Empty));
            Assert.AreEqual("A value for the argument 'name' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_Name_When_I_Call_UserRoleExists_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UserRoleExists(null));
            Assert.AreEqual("name", exception.ParamName);
        }


        [Test]
        public void Given_Valid_Arguments_When_I_Call_AddRole_Then_The_Correct_Service_Method_Is_Called()
        {
            //act
            _sut.AddUserRole(_roleDto);

            //assert
            _blaiseApiMock.Verify(v => v.AddRole(_name, _description, _permissions), Times.Once);
        }

        [Test]
        public void Given_An_Empty_Name_When_I_Call_AddUserRole_Then_An_ArgumentException_Is_Thrown()
        {
            //arrange
            _roleDto.Name = string.Empty;

            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.AddUserRole(_roleDto));
            Assert.AreEqual("A value for the argument 'UserRoleDto.Name' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_Name_When_I_Call_AddRole_Then_An_ArgumentNullException_Is_Thrown()
        {
            //arrange
            _roleDto.Name = null;

            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.AddUserRole(_roleDto));
            Assert.AreEqual("UserRoleDto.Name", exception.ParamName);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_RemoveUserRole_Then_The_Correct_Service_Method_Is_Called()
        {
            //act
            _sut.RemoveUserRole(_name);

            //assert
            _blaiseApiMock.Verify(v => v.RemoveRole(_name), Times.Once);
        }

        [Test]
        public void Given_An_Empty_Name_When_I_Call_RemoveRole_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.RemoveUserRole(string.Empty));
            Assert.AreEqual("A value for the argument 'name' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_Name_When_I_Call_RemoveRole_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.RemoveUserRole(null));
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Given_Valid_Arguments_When_I_Call_UpdateUserRolePermissions_Then_The_Correct_Service_Method_Is_Called()
        {
            //act
            _sut.UpdateUserRolePermissions(_name, _permissions);

            //assert
            _blaiseApiMock.Verify(v => v.UpdateRolePermissions(_name, _permissions), Times.Once);
        }

        [Test]
        public void Given_An_Empty_Name_When_I_Call_UpdateRolePermissions_Then_An_ArgumentException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentException>(() => _sut.UpdateUserRolePermissions(string.Empty, _permissions));
            Assert.AreEqual("A value for the argument 'name' must be supplied", exception.Message);
        }

        [Test]
        public void Given_A_Null_Name_When_I_Call_UpdateUserRolePermissions_Then_An_ArgumentNullException_Is_Thrown()
        {
            //act && assert
            var exception = Assert.Throws<ArgumentNullException>(() => _sut.UpdateUserRolePermissions(null, _permissions));
            Assert.AreEqual("name", exception.ParamName);
        }
    }
}
