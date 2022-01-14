using System.Collections.Generic;
using Blaise.Api.Tests.Behaviour.Models.Case;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Blaise.Api.Tests.Behaviour.StepArgumentTransformations
{
    [Binding]
    public class ImportOnlineCasesTransformations
    {
        [StepArgumentTransformation]
        public IEnumerable<CaseModel> TransformCasesTableIntoListOfCaseModels(Table table)
        {
            return table.CreateSet<CaseModel>();
        }
    }
}
