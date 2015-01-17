using System;
using System.Collections.Generic;
using System.IO;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.DLLs
{
  public class DllFileProject : IProject, IEquatable<DllFileProject>
  {
    public DllFileProject(string filePath)
    {
      if (String.IsNullOrWhiteSpace(filePath))
      {
        throw new ArgumentNullException("filePath");
      }

      Name = Path.GetFileNameWithoutExtension(filePath);
    }

    public string Name { get; private set; }

    public IEnumerable<Dependency> Dependencies { get { return new Dependency[0]; } }

    public bool Equals(DllFileProject other)
    {
      return
        other != null &&
        Name.Equals(other.Name);
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as DllFileProject;

      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      return Name.GetHashCode();
    }

    public override string ToString()
    {
      return String.Format("Dll Name: {0}", Name);
    }
  }
}
