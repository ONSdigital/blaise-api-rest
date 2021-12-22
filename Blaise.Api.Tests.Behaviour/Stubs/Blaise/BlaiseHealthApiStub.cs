using System;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Tests.Behaviour.Stubs.Blaise
{
    public class BlaiseHealthApiStub : IBlaiseHealthApi
    {
        public bool ConnectionModelIsHealthy()
        {
            throw new NotImplementedException();
        }

        public bool ConnectionToBlaiseIsHealthy()
        {
            throw new NotImplementedException();
        }

        public bool RemoteConnectionToBlaiseIsHealthy()
        {
            throw new NotImplementedException();
        }

        public bool RemoteConnectionToCatiIsHealthy()
        {
            throw new NotImplementedException();
        }
    }
}
