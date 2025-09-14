namespace Blaise.Api.Tests.Behaviour.StepArgumentTransformations
{
    using System.Collections.Generic;
    using Blaise.Api.Tests.Behaviour.Models.Case;
    using Reqnroll;
    using Reqnroll.Assist;

    [Binding]
    public class TableTransformations
    {
        [StepArgumentTransformation]
        public IEnumerable<CaseModel> TransformCasesTableIntoListOfCaseModels(Table table)
        {
            return table.CreateSet<CaseModel>();
        }
    }
}
