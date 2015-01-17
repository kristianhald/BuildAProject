using System;
using System.Collections.Generic;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.DLLs
{
  public class DllFileTask : IBuildTask, IEquatable<DllFileTask>
  {
    private readonly DllFileProject dllFileProject;

    public DllFileTask(DllFileProject dllFileProject)
    {
      if (dllFileProject == null)
      {
        throw new ArgumentNullException("dllFileProject");
      }

      this.dllFileProject = dllFileProject;
    }

    public string Name { get { return dllFileProject.Name; } }

    public IEnumerable<Dependency> Dependencies { get { return dllFileProject.Dependencies; } }

    public void Execute()
    {

    }

    public bool Equals(DllFileTask other)
    {
      return
        other != null &&
        Equals(dllFileProject, other.dllFileProject);
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as DllFileTask;
      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      return dllFileProject.GetHashCode();
    }

    public override string ToString()
    {
      return dllFileProject.ToString();
    }
  }
}
