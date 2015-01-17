using System;

namespace BuildAProject.BuildManagement.CsProjects.Compilers.Logging
{
  public class DotNetCompilerBuildError : IEquatable<DotNetCompilerBuildError>
  {
    private readonly CsProject project;

    public DotNetCompilerBuildError(CsProject project, string message)
    {
      if (project == null)
      {
        throw new ArgumentNullException("project");
      }

      if (String.IsNullOrWhiteSpace(message))
      {
        throw new ArgumentNullException("message");
      }

      this.project = project;
      Message = message;
    }

    public string ProjectName { get { return project.Name; } }

    public string Message { get; private set; }

    public bool Equals(DotNetCompilerBuildError other)
    {
      return
        other != null &&
        project.Equals(project) &&
        String.Equals(Message, other.Message);
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as DotNetCompilerBuildError;
      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      return project.GetHashCode();
    }

    public override string ToString()
    {
      return String.Format("{0}: {1} - '{2}'", GetType(), project.Name, Message);
    }
  }
}
