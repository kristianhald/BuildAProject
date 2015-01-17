using BuildAProject.BuildManagement.CsProjects.Compilers;
using BuildAProject.BuildManagement.CsProjects.TaskProviders;
using Moq;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class CsCompileTaskFactoryBuilder
  {
    private static readonly MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    public IDotNetCompiler DotNetCompiler = MockRepository.Create<IDotNetCompiler>().Object;

    public CsCompileTaskFactory Build()
    {
      return new CsCompileTaskFactory(DotNetCompiler);
    }
  }
}
