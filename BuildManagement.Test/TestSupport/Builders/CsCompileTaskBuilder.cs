using BuildAProject.BuildManagement.CsProjects;
using BuildAProject.BuildManagement.CsProjects.Compilers;
using Moq;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  public sealed class CsCompileTaskBuilder
  {
    private static readonly MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    public CsProject CsProject = new CsProjectBuilder().Build();
    public IDotNetCompiler DotNetCompiler = MockRepository.Create<IDotNetCompiler>().Object;

    public CsCompileTask Build()
    {
      return new CsCompileTask(CsProject, DotNetCompiler);
    }
  }
}
