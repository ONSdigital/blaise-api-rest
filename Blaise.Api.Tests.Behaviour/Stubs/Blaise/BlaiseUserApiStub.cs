using System;
using System.Collections.Generic;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Behaviour.Stubs.Blaise
{
    public class BlaiseUserApiStub : IBlaiseUserApi
    {
        public IEnumerable<IUser> GetUsers()
        {
            throw new NotImplementedException();
        }

        public IUser GetUser(string userName)
        {
            throw new NotImplementedException();
        }

        public bool UserExists(string userName)
        {
            throw new NotImplementedException();
        }

        public void AddUser(string userName, string password, string role, IList<string> serverParkNames, string defaultServerPark)
        {
            throw new NotImplementedException();
        }

        public void UpdatePassword(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public void UpdateRole(string userName, string role)
        {
            throw new NotImplementedException();
        }

        public void UpdateServerParks(string userName, IEnumerable<string> serverParkNames, string defaultServerPark)
        {
            throw new NotImplementedException();
        }

        public void RemoveUser(string userName)
        {
            throw new NotImplementedException();
        }

        public bool ValidateUser(string userName, string password)
        {
            throw new NotImplementedException();
        }
    }
}
