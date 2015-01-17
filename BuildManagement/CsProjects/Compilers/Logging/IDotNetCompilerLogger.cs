namespace BuildAProject.BuildManagement.CsProjects.Compilers.Logging
{
  public interface IDotNetCompilerLogger
  {
    void ErrorRaised(DotNetCompilerBuildError buildError);

    void BuildStarted(DotNetCompilerBuildStarted buildStarted);

    void BuildFinished(DotNetCompilerBuildFinished buildFinished);
  }
}
