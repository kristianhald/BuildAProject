using System;

namespace BuildAProject.BuildManagement.CsProjects.Compilers.Logging
{
  public enum BuildStatus
  {
    Success,
    Failed
  }

  public class DotNetCompilerBuildFinished : IEquatable<DotNetCompilerBuildFinished>
  {
    private readonly CsProject project;

    public DotNetCompilerBuildFinished(CsProject project, BuildStatus status)
    {
      if (project == null)
      {
        throw new ArgumentNullException("project");
      }

      this.project = project;
      Status = status;
    }

    public string ProjectName { get { return project.Name; } }

    public BuildStatus Status { get; private set; }

    public bool Equals(DotNetCompilerBuildFinished other)
    {
      return
        other != null &&
        project.Equals(other.project) &&
        Status == other.Status;
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as DotNetCompilerBuildFinished;
      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      return project.GetHashCode();
    }

    public override string ToString()
    {
      return String.Format("{0}: {1} - {2}", GetType(), project.Name, Status);
    }
  }
}
