﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Blaise.Api.Tests.Behaviour.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Ingest Questionnaire Data")]
    [NUnit.Framework.CategoryAttribute("ingest")]
    public partial class IngestQuestionnaireDataFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = new string[] {
                "ingest"};
        
#line 1 "IngestData.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Ingest Questionnaire Data", "\tIn order to ingest data from other organisations\r\n\tAs a service\r\n\tI want to be g" +
                    "iven cases to ingest representing the data captured by other organisations", ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("There is an ingest file available in the bucket that we wish to merge with an exi" +
            "sting blaise dataset")]
        [NUnit.Framework.CategoryAttribute("smoke")]
        public void ThereIsAnIngestFileAvailableInTheBucketThatWeWishToMergeWithAnExistingBlaiseDataset()
        {
            string[] tagsOfScenario = new string[] {
                    "smoke"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("There is an ingest file available in the bucket that we wish to merge with an exi" +
                    "sting blaise dataset", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 8
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
                TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                            "primarykey",
                            "outcome",
                            "mode"});
                table4.AddRow(new string[] {
                            "500001",
                            "110",
                            "Web"});
                table4.AddRow(new string[] {
                            "500002",
                            "210",
                            "Web"});
                table4.AddRow(new string[] {
                            "500003",
                            "110",
                            "Web"});
#line 9
 testRunner.Given("there is an ingest file that contains the following cases", ((string)(null)), table4, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                            "primarykey",
                            "outcome",
                            "mode"});
                table5.AddRow(new string[] {
                            "900001",
                            "110",
                            "Web"});
                table5.AddRow(new string[] {
                            "900002",
                            "110",
                            "Web"});
                table5.AddRow(new string[] {
                            "900003",
                            "210",
                            "Web"});
#line 15
 testRunner.And("blaise contains the existing cases", ((string)(null)), table5, "And ");
#line hidden
#line 21
 testRunner.When("the ingest file is processed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                            "primarykey",
                            "outcome",
                            "mode"});
                table6.AddRow(new string[] {
                            "500001",
                            "110",
                            "Web"});
                table6.AddRow(new string[] {
                            "500002",
                            "210",
                            "Web"});
                table6.AddRow(new string[] {
                            "500003",
                            "110",
                            "Web"});
                table6.AddRow(new string[] {
                            "900001",
                            "110",
                            "Web"});
                table6.AddRow(new string[] {
                            "900002",
                            "110",
                            "Web"});
                table6.AddRow(new string[] {
                            "900003",
                            "210",
                            "Web"});
#line 22
 testRunner.Then("blaise will contain the cases", ((string)(null)), table6, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion