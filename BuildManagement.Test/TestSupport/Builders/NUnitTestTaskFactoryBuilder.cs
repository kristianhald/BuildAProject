using BuildAProject.BuildManagement.CsProjects.Compilers;
using BuildAProject.BuildManagement.NUnit.Runners;
using BuildAProject.BuildManagement.NUnit.TaskProviders;
using Moq;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class NUnitTestTaskFactoryBuilder
  {
    private static readonly MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    public ITestRunner TestRunner = MockRepository.Create<ITestRunner>().Object;

    public BuildEngineParameters Parameters = new BuildEngineParameters();

    public NUnitTestTaskFactory Build()
    {
      return new NUnitTestTaskFactory(TestRunner, Parameters);
    }
  }
}
