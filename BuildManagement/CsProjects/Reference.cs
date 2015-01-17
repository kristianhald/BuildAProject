using System;
using System.Linq;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.CsProjects
{
  public class Reference : Dependency, IEquatable<Reference>
  {
    public Reference(string assemblyInformation, string hintPath)
      : base(ParseName(assemblyInformation))
    {
      if (hintPath == null)
      {
        throw new ArgumentNullException("hintPath");
      }

      Name = ParseName(assemblyInformation);
      HintPath = hintPath;
    }


    private static string ParseName(string dependency)
    {
      if (dependency == null)
        return null;

      return dependency
        .Split(',')
        .First();
    }

    public string Name { get; private set; }

    public string HintPath { get; private set; }

    public bool Equals(Reference other)
    {
      return
        other != null &&
        String.Equals(Name, other.Name) &&
        String.Equals(HintPath, other.HintPath);
    }

    public override bool Equals(object obj)
    {
      if (obj is Reference)
        return Equals(obj as Reference);

      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
