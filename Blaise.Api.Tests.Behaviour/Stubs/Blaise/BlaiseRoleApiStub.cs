using System;
using System.Collections.Generic;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.Security;

namespace Blaise.Api.Tests.Behaviour.Stubs.Blaise
{
    public class BlaiseRoleApiStub : IBlaiseRoleApi
    {
        public IEnumerable<IRole> GetRoles()
        {
            throw new NotImplementedException();
        }

        public IRole GetRole(string name)
        {
            throw new NotImplementedException();
        }

        public bool RoleExists(string name)
        {
            throw new NotImplementedException();
        }

        public void AddRole(string name, string description, IEnumerable<string> permissions)
        {
            throw new NotImplementedException();
        }

        public void RemoveRole(string name)
        {
            throw new NotImplementedException();
        }

        public void UpdateRolePermissions(string name, IEnumerable<string> permissions)
        {
            throw new NotImplementedException();
        }
    }
}
