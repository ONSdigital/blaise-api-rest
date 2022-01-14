using System;
using System.Collections.Generic;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Tests.Behaviour.Stubs.Blaise
{
    public class BlaiseServerParkApiStub: IBlaiseServerParkApi
    {
        public IServerPark GetServerPark(string serverParkName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IServerPark> GetServerParks()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetNamesOfServerParks()
        {
            throw new NotImplementedException();
        }

        public bool ServerParkExists(string serverParkName)
        {
            throw new NotImplementedException();
        }
    }
}
