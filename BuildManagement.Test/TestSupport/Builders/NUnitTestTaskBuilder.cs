using BuildAProject.BuildManagement.CsProjects;
using BuildAProject.BuildManagement.CsProjects.Compilers;
using BuildAProject.BuildManagement.NUnit;
using BuildAProject.BuildManagement.NUnit.Runners;
using Moq;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class NUnitTestTaskBuilder
  {
    private static readonly MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    public ITestRunner TestRunner = MockRepository.Create<ITestRunner>().Object;

    public CsProject CsProject = new CsProjectBuilder().Build();

    public BuildEngineParameters BuildEngineParameters = new BuildEngineParameters();

    public NUnitTestTask Build()
    {
      return new NUnitTestTask(TestRunner, CsProject, BuildEngineParameters);
    }
  }
}
