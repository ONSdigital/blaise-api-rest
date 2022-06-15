using System;
using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataInterface;

namespace Blaise.Api.Tests.Behaviour.Stubs.Blaise
{
    public class BlaiseFileApiStub : IBlaiseFileApi
    {
        public void UpdateQuestionnaireFileWithData(string serverParkName, string questionnaireName, string questionnaireFile)
        {
            throw new NotImplementedException();
        }

        public void UpdateQuestionnaireFileWithSqlConnection(string questionnaireName, string questionnaireFile)
        {
            throw new NotImplementedException();
        }

        public void CreateSettingsDataInterfaceFile(ApplicationType applicationType, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
