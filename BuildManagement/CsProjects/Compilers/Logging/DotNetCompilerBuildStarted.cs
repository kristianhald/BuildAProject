using System;

namespace BuildAProject.BuildManagement.CsProjects.Compilers.Logging
{
  public class DotNetCompilerBuildStarted : IEquatable<DotNetCompilerBuildStarted>
  {
    private readonly CsProject project;

    public DotNetCompilerBuildStarted(CsProject project)
    {
      if (project == null)
      {
        throw new ArgumentNullException("project");
      }

      this.project = project;
    }

    public string ProjectName { get { return project.Name; } }

    public bool Equals(DotNetCompilerBuildStarted other)
    {
      return
        other != null &&
        project.Equals(other.project);
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as DotNetCompilerBuildStarted;
      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      return project.GetHashCode();
    }

    public override string ToString()
    {
      return String.Format("{0}: {1}", GetType(), project.Name);
    }
  }
}
