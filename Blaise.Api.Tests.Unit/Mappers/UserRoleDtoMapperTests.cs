namespace Blaise.Api.Tests.Unit.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using Blaise.Api.Contracts.Models.UserRole;
    using Blaise.Api.Core.Mappers;
    using Moq;
    using NUnit.Framework;
    using StatNeth.Blaise.API.Security;

    public class UserRoleDtoMapperTests
    {
        private UserRoleDtoMapper _sut;

        [SetUp]
        public void SetupTests()
        {
            _sut = new UserRoleDtoMapper();
        }

        [Test]
        public void Given_A_List_Of_Roles_When_I_Call_MapToUserRoleDtos_Then_A_Correct_List_Of_UserRoleDtos_Are_Returned()
        {
            // arrange
            const string role1Name = "Name1";
            const string role1Description = "Description1";
            var role1Mock = new Mock<IRole>();
            role1Mock.Setup(r => r.Name).Returns(role1Name);
            role1Mock.Setup(r => r.Description).Returns(role1Description);

            const string permission1 = "Permission1";
            var actionPermission1Mock = new Mock<IActionPermission>();
            actionPermission1Mock.Setup(a => a.Action).Returns(permission1);
            actionPermission1Mock.Setup(a => a.Permission).Returns(PermissionStatus.Allowed);
            var actionPermissions = new List<IActionPermission> { actionPermission1Mock.Object };
            role1Mock.Setup(r => r.Permissions).Returns(actionPermissions);

            const string role2Name = "Name2";
            const string role2Description = "Description2";
            var role2Mock = new Mock<IRole>();
            role2Mock.Setup(r => r.Name).Returns(role2Name);
            role2Mock.Setup(r => r.Description).Returns(role2Description);

            const string permission2 = "Permission2";
            var actionPermission2Mock = new Mock<IActionPermission>();
            actionPermission2Mock.Setup(a => a.Action).Returns(permission2);
            actionPermission2Mock.Setup(a => a.Permission).Returns(PermissionStatus.Allowed);
            var actionPermissions2 = new List<IActionPermission> { actionPermission2Mock.Object };
            role2Mock.Setup(r => r.Permissions).Returns(actionPermissions2);

            var roles = new List<IRole> { role1Mock.Object, role2Mock.Object };

            // act
            var result = _sut.MapToUserRoleDtos(roles).ToList();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<UserRoleDto>>());
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(
                result.Any(
                    r =>
                    r.Name == role1Name &&
                    r.Description == role1Description &&
                    r.Permissions.Count() == 1 &&
                    r.Permissions.Contains(permission1)),
                Is.True);

            Assert.That(
                result.Any(
                    r =>
                    r.Name == role2Name &&
                    r.Description == role2Description &&
                    r.Permissions.Count() == 1 &&
                    r.Permissions.Contains(permission2)),
                Is.True);
        }

        [Test]
        public void Given_A_List_Of_Roles_When_I_Call_MapToUserRoleDtos_Then_Only_Allowed_Permissions_Are_Returned()
        {
            // arrange
            const string role1Name = "Name1";
            const string role1Description = "Description1";
            var role1Mock = new Mock<IRole>();
            role1Mock.Setup(r => r.Name).Returns(role1Name);
            role1Mock.Setup(r => r.Description).Returns(role1Description);

            const string permission1 = "Permission1";
            var actionPermission1Mock = new Mock<IActionPermission>();
            actionPermission1Mock.Setup(a => a.Action).Returns(permission1);
            actionPermission1Mock.Setup(a => a.Permission).Returns(PermissionStatus.Disallowed);
            var actionPermissions = new List<IActionPermission> { actionPermission1Mock.Object };
            role1Mock.Setup(r => r.Permissions).Returns(actionPermissions);

            const string role2Name = "Name2";
            const string role2Description = "Description2";
            var role2Mock = new Mock<IRole>();
            role2Mock.Setup(r => r.Name).Returns(role2Name);
            role2Mock.Setup(r => r.Description).Returns(role2Description);

            const string permission2 = "Permission2";
            var actionPermission2Mock = new Mock<IActionPermission>();
            actionPermission2Mock.Setup(a => a.Action).Returns(permission2);
            actionPermission2Mock.Setup(a => a.Permission).Returns(PermissionStatus.Allowed);
            var actionPermissions2 = new List<IActionPermission> { actionPermission2Mock.Object };
            role2Mock.Setup(r => r.Permissions).Returns(actionPermissions2);

            var roles = new List<IRole> { role1Mock.Object, role2Mock.Object };

            // act
            var result = _sut.MapToUserRoleDtos(roles).ToList();

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<List<UserRoleDto>>());
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(
                result.Any(
                    r =>
                    r.Name == role1Name &&
                    r.Description == role1Description &&
                    !r.Permissions.Any()),
                Is.True);

            Assert.That(
                result.Any(
                    r =>
                    r.Name == role2Name &&
                    r.Description == role2Description &&
                    r.Permissions.Count() == 1 &&
                    r.Permissions.Contains(permission2)),
                Is.True);
        }

        [Test]
        public void Given_A_Role_When_I_Call_MapToUserRoleDto_Then_A_Correct_UserRoleDto_Is_Returned()
        {
            // arrange
            const string role1Name = "Name1";
            const string role1Description = "Description1";
            var role1Mock = new Mock<IRole>();
            role1Mock.Setup(r => r.Name).Returns(role1Name);
            role1Mock.Setup(r => r.Description).Returns(role1Description);

            const string permission1 = "Permission1";
            var actionPermission1Mock = new Mock<IActionPermission>();
            actionPermission1Mock.Setup(a => a.Action).Returns(permission1);
            actionPermission1Mock.Setup(a => a.Permission).Returns(PermissionStatus.Allowed);

            const string permission2 = "Permission2";
            var actionPermission2Mock = new Mock<IActionPermission>();
            actionPermission2Mock.Setup(a => a.Action).Returns(permission2);
            actionPermission2Mock.Setup(a => a.Permission).Returns(PermissionStatus.Allowed);

            var actionPermissions = new List<IActionPermission> { actionPermission1Mock.Object, actionPermission2Mock.Object };
            role1Mock.Setup(r => r.Permissions).Returns(actionPermissions);

            // act
            var result = _sut.MapToUserRoleDto(role1Mock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<UserRoleDto>());
            Assert.That(result.Name, Is.EqualTo(role1Name));
            Assert.That(result.Description, Is.EqualTo(role1Description));
            Assert.That(result.Permissions.Count(), Is.EqualTo(2));
            Assert.That(result.Permissions.Any(p => p == permission1), Is.True);
            Assert.That(result.Permissions.Any(p => p == permission2), Is.True);
        }

        [Test]
        public void Given_A_Role_When_I_Call_MapToUserRoleDto_Then_Only_Allowed_Permissions_Are_Returned()
        {
            // arrange
            const string role1Name = "Name1";
            const string role1Description = "Description1";
            var role1Mock = new Mock<IRole>();
            role1Mock.Setup(r => r.Name).Returns(role1Name);
            role1Mock.Setup(r => r.Description).Returns(role1Description);

            const string permission1 = "Permission1";
            var actionPermission1Mock = new Mock<IActionPermission>();
            actionPermission1Mock.Setup(a => a.Action).Returns(permission1);
            actionPermission1Mock.Setup(a => a.Permission).Returns(PermissionStatus.Disallowed);

            const string permission2 = "Permission2";
            var actionPermission2Mock = new Mock<IActionPermission>();
            actionPermission2Mock.Setup(a => a.Action).Returns(permission2);
            actionPermission2Mock.Setup(a => a.Permission).Returns(PermissionStatus.Allowed);

            var actionPermissions = new List<IActionPermission> { actionPermission1Mock.Object, actionPermission2Mock.Object };
            role1Mock.Setup(r => r.Permissions).Returns(actionPermissions);

            // act
            var result = _sut.MapToUserRoleDto(role1Mock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<UserRoleDto>());
            Assert.That(result.Name, Is.EqualTo(role1Name));
            Assert.That(result.Description, Is.EqualTo(role1Description));
            Assert.That(result.Permissions.Count(), Is.EqualTo(1));
            Assert.That(result.Permissions.Any(p => p == permission2), Is.True);
        }
    }
}
