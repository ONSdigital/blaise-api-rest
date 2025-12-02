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
            const string Role1Name = "Name1";
            const string Role1Description = "Description1";
            var role1Mock = new Mock<IRole>();
            role1Mock.Setup(r => r.Name).Returns(Role1Name);
            role1Mock.Setup(r => r.Description).Returns(Role1Description);

            const string Permission1 = "Permission1";
            var actionPermission1Mock = new Mock<IActionPermission>();
            actionPermission1Mock.Setup(a => a.Action).Returns(Permission1);
            actionPermission1Mock.Setup(a => a.Permission).Returns(PermissionStatus.Allowed);
            var actionPermissions = new List<IActionPermission> { actionPermission1Mock.Object };
            role1Mock.Setup(r => r.Permissions).Returns(actionPermissions);

            const string Role2Name = "Name2";
            const string Role2Description = "Description2";
            var role2Mock = new Mock<IRole>();
            role2Mock.Setup(r => r.Name).Returns(Role2Name);
            role2Mock.Setup(r => r.Description).Returns(Role2Description);

            const string Permission2 = "Permission2";
            var actionPermission2Mock = new Mock<IActionPermission>();
            actionPermission2Mock.Setup(a => a.Action).Returns(Permission2);
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
                    r.Name == Role1Name &&
                    r.Description == Role1Description &&
                    r.Permissions.Count() == 1 &&
                    r.Permissions.Contains(Permission1)),
                Is.True);

            Assert.That(
                result.Any(
                    r =>
                    r.Name == Role2Name &&
                    r.Description == Role2Description &&
                    r.Permissions.Count() == 1 &&
                    r.Permissions.Contains(Permission2)),
                Is.True);
        }

        [Test]
        public void Given_A_List_Of_Roles_When_I_Call_MapToUserRoleDtos_Then_Only_Allowed_Permissions_Are_Returned()
        {
            // arrange
            const string Role1Name = "Name1";
            const string Role1Description = "Description1";
            var role1Mock = new Mock<IRole>();
            role1Mock.Setup(r => r.Name).Returns(Role1Name);
            role1Mock.Setup(r => r.Description).Returns(Role1Description);

            const string Permission1 = "Permission1";
            var actionPermission1Mock = new Mock<IActionPermission>();
            actionPermission1Mock.Setup(a => a.Action).Returns(Permission1);
            actionPermission1Mock.Setup(a => a.Permission).Returns(PermissionStatus.Disallowed);
            var actionPermissions = new List<IActionPermission> { actionPermission1Mock.Object };
            role1Mock.Setup(r => r.Permissions).Returns(actionPermissions);

            const string Role2Name = "Name2";
            const string Role2Description = "Description2";
            var role2Mock = new Mock<IRole>();
            role2Mock.Setup(r => r.Name).Returns(Role2Name);
            role2Mock.Setup(r => r.Description).Returns(Role2Description);

            const string Permission2 = "Permission2";
            var actionPermission2Mock = new Mock<IActionPermission>();
            actionPermission2Mock.Setup(a => a.Action).Returns(Permission2);
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
                    r.Name == Role1Name &&
                    r.Description == Role1Description &&
                    !r.Permissions.Any()),
                Is.True);

            Assert.That(
                result.Any(
                    r =>
                    r.Name == Role2Name &&
                    r.Description == Role2Description &&
                    r.Permissions.Count() == 1 &&
                    r.Permissions.Contains(Permission2)),
                Is.True);
        }

        [Test]
        public void Given_A_Role_When_I_Call_MapToUserRoleDto_Then_A_Correct_UserRoleDto_Is_Returned()
        {
            // arrange
            const string Role1Name = "Name1";
            const string Role1Description = "Description1";
            var role1Mock = new Mock<IRole>();
            role1Mock.Setup(r => r.Name).Returns(Role1Name);
            role1Mock.Setup(r => r.Description).Returns(Role1Description);

            const string Permission1 = "Permission1";
            var actionPermission1Mock = new Mock<IActionPermission>();
            actionPermission1Mock.Setup(a => a.Action).Returns(Permission1);
            actionPermission1Mock.Setup(a => a.Permission).Returns(PermissionStatus.Allowed);

            const string Permission2 = "Permission2";
            var actionPermission2Mock = new Mock<IActionPermission>();
            actionPermission2Mock.Setup(a => a.Action).Returns(Permission2);
            actionPermission2Mock.Setup(a => a.Permission).Returns(PermissionStatus.Allowed);

            var actionPermissions = new List<IActionPermission> { actionPermission1Mock.Object, actionPermission2Mock.Object };
            role1Mock.Setup(r => r.Permissions).Returns(actionPermissions);

            // act
            var result = _sut.MapToUserRoleDto(role1Mock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<UserRoleDto>());
            Assert.That(result.Name, Is.EqualTo(Role1Name));
            Assert.That(result.Description, Is.EqualTo(Role1Description));
            Assert.That(result.Permissions.Count(), Is.EqualTo(2));
            Assert.That(result.Permissions.Any(p => p == Permission1), Is.True);
            Assert.That(result.Permissions.Any(p => p == Permission2), Is.True);
        }

        [Test]
        public void Given_A_Role_When_I_Call_MapToUserRoleDto_Then_Only_Allowed_Permissions_Are_Returned()
        {
            // arrange
            const string Role1Name = "Name1";
            const string Role1Description = "Description1";
            var role1Mock = new Mock<IRole>();
            role1Mock.Setup(r => r.Name).Returns(Role1Name);
            role1Mock.Setup(r => r.Description).Returns(Role1Description);

            const string Permission1 = "Permission1";
            var actionPermission1Mock = new Mock<IActionPermission>();
            actionPermission1Mock.Setup(a => a.Action).Returns(Permission1);
            actionPermission1Mock.Setup(a => a.Permission).Returns(PermissionStatus.Disallowed);

            const string Permission2 = "Permission2";
            var actionPermission2Mock = new Mock<IActionPermission>();
            actionPermission2Mock.Setup(a => a.Action).Returns(Permission2);
            actionPermission2Mock.Setup(a => a.Permission).Returns(PermissionStatus.Allowed);

            var actionPermissions = new List<IActionPermission> { actionPermission1Mock.Object, actionPermission2Mock.Object };
            role1Mock.Setup(r => r.Permissions).Returns(actionPermissions);

            // act
            var result = _sut.MapToUserRoleDto(role1Mock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<UserRoleDto>());
            Assert.That(result.Name, Is.EqualTo(Role1Name));
            Assert.That(result.Description, Is.EqualTo(Role1Description));
            Assert.That(result.Permissions.Count(), Is.EqualTo(1));
            Assert.That(result.Permissions.Any(p => p == Permission2), Is.True);
        }
    }
}
