using System;
using System.Collections.Generic;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.CsProjects.Compilers;

namespace BuildAProject.BuildManagement.CsProjects
{
  public class CsCompileTask : IBuildTask, IEquatable<CsCompileTask>
  {
    private readonly CsProject csProject;
    private readonly IDotNetCompiler dotNetCompiler;

    public CsCompileTask(CsProject csProject, IDotNetCompiler dotNetCompiler)
    {
      if (csProject == null)
      {
        throw new ArgumentNullException("csProject");
      }

      if (dotNetCompiler == null)
      {
        throw new ArgumentNullException("dotNetCompiler");
      }

      this.csProject = csProject;
      this.dotNetCompiler = dotNetCompiler;
    }

    public IEnumerable<Dependency> Dependencies { get { return csProject.Dependencies; } }

    public string Name { get { return csProject.Name; } }

    public void Execute()
    {
      dotNetCompiler.Build(csProject);
    }

    public bool Equals(CsCompileTask other)
    {
      return
        other != null &&
        csProject.Equals(other.csProject);
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as CsCompileTask;
      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      return csProject.GetHashCode();
    }

    public override string ToString()
    {
      return csProject.ToString();
    }
  }
}
