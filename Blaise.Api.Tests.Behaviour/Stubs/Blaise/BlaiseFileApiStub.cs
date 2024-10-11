using Blaise.Nuget.Api.Contracts.Interfaces;
using StatNeth.Blaise.API.DataInterface;
using System;

namespace Blaise.Api.Tests.Behaviour.Stubs.Blaise
{
    public class BlaiseFileApiStub : IBlaiseFileApi
    {
        public void UpdateQuestionnaireFileWithBatchedData(string questionnaireFile, string questionnaireName, string serverParkName,
            int batchSize, bool addAudit = false)
        {
            throw new NotImplementedException();
        }

        public void UpdateQuestionnaireFileWithSqlConnection(string questionnaireName, string questionnaireFile, bool overwriteExistingData = true)
        {
            throw new NotImplementedException();
        }

        public void CreateSettingsDataInterfaceFile(ApplicationType applicationType, string fileName)
        {
            throw new NotImplementedException();
        }

        public void CreateCasesInBlaise(
            int numberOfCases,
            string questionnaireName,
            string serverParkName,
            int primaryKey)
        {
            throw new NotImplementedException();
        }

        public void UpdateQuestionnaireFileWithData(string serverParkName, string questionnaireName, string questionnaireFile, bool auditOption)
        {
            throw new NotImplementedException();
        }

        public void UpdateQuestionnaireFileWithData(string serverParkName, string questionnaireName, string questionnaireFile)
        {
            throw new NotImplementedException();
        }
    }
}
