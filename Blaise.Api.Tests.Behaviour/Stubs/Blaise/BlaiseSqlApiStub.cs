using System;
using System.Collections.Generic;
using Blaise.Nuget.Api.Contracts.Interfaces;
using Blaise.Nuget.Api.Contracts.Models;

namespace Blaise.Api.Tests.Behaviour.Stubs.Blaise
{
    public class BlaiseSqlApiStub : IBlaiseSqlApi
    {
        public bool DropQuestionnaireTables(string questionnaireName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CaseIdentifierModel> GetCaseIdentifiers(string questionnaireName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetCaseIds(string questionnaireName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetEditingCaseIds(string questionnaireName)
        {
            throw new NotImplementedException();
        }

        public string GetPostCode(string questionnaireName, string primaryKey)
        {
            throw new NotImplementedException();
        }
    }
}
