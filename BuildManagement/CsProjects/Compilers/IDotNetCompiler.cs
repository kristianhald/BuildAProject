namespace BuildAProject.BuildManagement.CsProjects.Compilers
{
  public interface IDotNetCompiler
  {
    void Build(CsProject csProject);
  }
}
