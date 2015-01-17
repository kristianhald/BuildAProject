using BuildAProject.BuildManagement.NUnit.Runners;
using Moq;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  public sealed class NUnitCsProjectTestRunnerBuilder
  {
    private static readonly MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    public INUnitLogger Logger = MockRepository.Create<INUnitLogger>().Object;

    public NUnitFileTestRunner Build()
    {
      return new NUnitFileTestRunner(Logger);
    }
  }
}
