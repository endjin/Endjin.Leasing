﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.34209
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Endjin.Storage.Leasing.Azure.Specs.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Leasable Perf")]
    [NUnit.Framework.CategoryAttribute("ReleaseLeases")]
    [NUnit.Framework.CategoryAttribute("container")]
    public partial class LeasablePerfFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "LeasablePerf.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Leasable Perf", "", ProgrammingLanguage.CSharp, new string[] {
                        "ReleaseLeases",
                        "container"});
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Create 100 leasables")]
        [NUnit.Framework.CategoryAttribute("container")]
        [NUnit.Framework.CategoryAttribute("storage_emulator")]
        public virtual void Create100Leasables()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Create 100 leasables", new string[] {
                        "container",
                        "storage_emulator"});
#line 5
this.ScenarioSetup(scenarioInfo);
#line 6
 testRunner.When("I run 100 actions using leasable", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 7
 testRunner.Then("the result should take less than 20 second(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Run 10 mutex actions simultaneously with 5ms action")]
        [NUnit.Framework.CategoryAttribute("container")]
        [NUnit.Framework.CategoryAttribute("storage_emulator")]
        public virtual void Run10MutexActionsSimultaneouslyWith5MsAction()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Run 10 mutex actions simultaneously with 5ms action", new string[] {
                        "container",
                        "storage_emulator"});
#line 10
this.ScenarioSetup(scenarioInfo);
#line 11
 testRunner.Given("an action takes 5 ms", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 12
 testRunner.When("I run 10 mutex actions simultaneously", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 13
 testRunner.Then("the result should take less than 3 second(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Run 10 actions consecutively with 5ms action")]
        [NUnit.Framework.CategoryAttribute("container")]
        [NUnit.Framework.CategoryAttribute("storage_emulator")]
        public virtual void Run10ActionsConsecutivelyWith5MsAction()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Run 10 actions consecutively with 5ms action", new string[] {
                        "container",
                        "storage_emulator"});
#line 16
this.ScenarioSetup(scenarioInfo);
#line 17
 testRunner.Given("an action takes 5 ms", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 18
 testRunner.When("I run 10 actions consecutively", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 19
 testRunner.Then("the result should take less than 3 second(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Run 10 actions simultaneously with 5ms action")]
        [NUnit.Framework.CategoryAttribute("container")]
        [NUnit.Framework.CategoryAttribute("storage_emulator")]
        public virtual void Run10ActionsSimultaneouslyWith5MsAction()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Run 10 actions simultaneously with 5ms action", new string[] {
                        "container",
                        "storage_emulator"});
#line 22
this.ScenarioSetup(scenarioInfo);
#line 23
 testRunner.Given("an action takes 5 ms", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 24
 testRunner.When("I run 10 actions simultaneously", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 25
 testRunner.Then("the result should take less than 3 second(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Run 10 mutex actions simultaneously with 50ms action")]
        [NUnit.Framework.CategoryAttribute("container")]
        [NUnit.Framework.CategoryAttribute("storage_emulator")]
        public virtual void Run10MutexActionsSimultaneouslyWith50MsAction()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Run 10 mutex actions simultaneously with 50ms action", new string[] {
                        "container",
                        "storage_emulator"});
#line 28
this.ScenarioSetup(scenarioInfo);
#line 29
 testRunner.Given("an action takes 50 ms", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 30
 testRunner.When("I run 10 mutex actions simultaneously", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 31
 testRunner.Then("the result should take less than 3 second(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Run 10 actions consecutively with 50ms action")]
        [NUnit.Framework.CategoryAttribute("container")]
        [NUnit.Framework.CategoryAttribute("storage_emulator")]
        public virtual void Run10ActionsConsecutivelyWith50MsAction()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Run 10 actions consecutively with 50ms action", new string[] {
                        "container",
                        "storage_emulator"});
#line 34
this.ScenarioSetup(scenarioInfo);
#line 35
 testRunner.Given("an action takes 50 ms", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 36
 testRunner.When("I run 10 actions consecutively", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 37
 testRunner.Then("the result should take less than 3 second(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Run 10 actions simultaneously with 50ms action")]
        [NUnit.Framework.CategoryAttribute("container")]
        [NUnit.Framework.CategoryAttribute("storage_emulator")]
        public virtual void Run10ActionsSimultaneouslyWith50MsAction()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Run 10 actions simultaneously with 50ms action", new string[] {
                        "container",
                        "storage_emulator"});
#line 40
this.ScenarioSetup(scenarioInfo);
#line 41
 testRunner.Given("an action takes 50 ms", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 42
 testRunner.When("I run 10 actions simultaneously", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 43
 testRunner.Then("the result should take less than 3 second(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Run 10 mutex actions simultaneously with 500ms action")]
        [NUnit.Framework.CategoryAttribute("container")]
        [NUnit.Framework.CategoryAttribute("storage_emulator")]
        public virtual void Run10MutexActionsSimultaneouslyWith500MsAction()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Run 10 mutex actions simultaneously with 500ms action", new string[] {
                        "container",
                        "storage_emulator"});
#line 46
this.ScenarioSetup(scenarioInfo);
#line 47
 testRunner.Given("an action takes 500 ms", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 48
 testRunner.When("I run 10 mutex actions simultaneously", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 49
 testRunner.Then("the result should take less than 10 second(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Run 10 actions consecutively with 500ms action")]
        [NUnit.Framework.CategoryAttribute("container")]
        [NUnit.Framework.CategoryAttribute("storage_emulator")]
        public virtual void Run10ActionsConsecutivelyWith500MsAction()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Run 10 actions consecutively with 500ms action", new string[] {
                        "container",
                        "storage_emulator"});
#line 52
this.ScenarioSetup(scenarioInfo);
#line 53
 testRunner.Given("an action takes 500 ms", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 54
 testRunner.When("I run 10 actions consecutively", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 55
 testRunner.Then("the result should take less than 10 second(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Run 10 actions simultaneously with 500ms action")]
        [NUnit.Framework.CategoryAttribute("container")]
        [NUnit.Framework.CategoryAttribute("storage_emulator")]
        public virtual void Run10ActionsSimultaneouslyWith500MsAction()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Run 10 actions simultaneously with 500ms action", new string[] {
                        "container",
                        "storage_emulator"});
#line 58
this.ScenarioSetup(scenarioInfo);
#line 59
 testRunner.Given("an action takes 500 ms", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 60
 testRunner.When("I run 10 actions simultaneously", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 61
 testRunner.Then("the result should take less than 10 second(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Run 10 mutex actions simultaneously with 1000ms action")]
        [NUnit.Framework.CategoryAttribute("container")]
        [NUnit.Framework.CategoryAttribute("storage_emulator")]
        public virtual void Run10MutexActionsSimultaneouslyWith1000MsAction()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Run 10 mutex actions simultaneously with 1000ms action", new string[] {
                        "container",
                        "storage_emulator"});
#line 64
this.ScenarioSetup(scenarioInfo);
#line 65
 testRunner.Given("an action takes 1000 ms", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 66
 testRunner.When("I run 10 mutex actions simultaneously", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 67
 testRunner.Then("the result should take less than 20 second(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Run 10 actions consecutively with 1000ms action")]
        [NUnit.Framework.CategoryAttribute("container")]
        [NUnit.Framework.CategoryAttribute("storage_emulator")]
        public virtual void Run10ActionsConsecutivelyWith1000MsAction()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Run 10 actions consecutively with 1000ms action", new string[] {
                        "container",
                        "storage_emulator"});
#line 70
this.ScenarioSetup(scenarioInfo);
#line 71
 testRunner.Given("an action takes 1000 ms", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 72
 testRunner.When("I run 10 actions consecutively", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 73
 testRunner.Then("the result should take less than 20 second(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Run 10 actions simultaneously with 1000ms action")]
        [NUnit.Framework.CategoryAttribute("container")]
        [NUnit.Framework.CategoryAttribute("storage_emulator")]
        public virtual void Run10ActionsSimultaneouslyWith1000MsAction()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Run 10 actions simultaneously with 1000ms action", new string[] {
                        "container",
                        "storage_emulator"});
#line 76
this.ScenarioSetup(scenarioInfo);
#line 77
 testRunner.Given("an action takes 1000 ms", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 78
 testRunner.When("I run 10 actions simultaneously", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 79
 testRunner.Then("the result should take less than 20 second(s)", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion