using BuildAProject.BuildManagement.CsProjects.Compilers;
using BuildAProject.BuildManagement.CsProjects.Compilers.Logging;
using Moq;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class BuildEngineCompilerBuilder
  {
    private static readonly MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    public BuildEngineParameters Parameters = new BuildEngineParameters();

    public IDotNetCompilerLogger Logger = MockRepository.Create<IDotNetCompilerLogger>().Object;

    public BuildEngineCompiler Build()
    {
      return new BuildEngineCompiler(Parameters, Logger);
    }
  }
}
