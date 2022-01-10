using System;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataInterface;

namespace Blaise.Api.Tests.Behaviour.Stubs.Blaise
{
    public class BlaiseFileApiStub : IBlaiseFileApi
    {
        public void UpdateInstrumentFileWithData(string serverParkName, string instrumentName, string instrumentFile)
        {
            throw new NotImplementedException();
        }

        public void UpdateInstrumentFileWithSqlConnection(string instrumentName, string instrumentFile)
        {
            throw new NotImplementedException();
        }

        public void CreateSettingsDataInterfaceFile(ApplicationType applicationType, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
