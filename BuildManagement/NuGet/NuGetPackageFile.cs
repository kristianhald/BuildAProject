using System;
using System.Collections.Generic;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.NuGet
{
  public class NuGetPackageFile : IProject, IEquatable<NuGetPackageFile>
  {
    public NuGetPackageFile(string packageFilePath, string packageName, string version, string framework)
    {
      if (String.IsNullOrWhiteSpace(packageFilePath))
      {
        throw new ArgumentNullException("packageFilePath");
      }

      if (String.IsNullOrWhiteSpace(packageName))
      {
        throw new ArgumentNullException("packageName");
      }

      if (String.IsNullOrWhiteSpace(version))
      {
        throw new ArgumentNullException("version");
      }

      if (framework != null && String.IsNullOrWhiteSpace(framework))
      {
        throw new ArgumentNullException("framework");
      }

      FilePath = packageFilePath;
      Name = packageName;
      Version = version;
      Framework = framework;
    }

    public string FilePath { get; private set; }

    public string Name { get; private set; }

    public string Version { get; private set; }

    public string Framework { get; private set; }

    public IEnumerable<Dependency> Dependencies { get { return new Dependency[0]; } }

    public bool Equals(NuGetPackageFile other)
    {
      return
        other != null &&
        FilePath.Equals(other.FilePath) &&
        Name.Equals(other.Name) &&
        Version.Equals(other.Version) &&
        String.Equals(Framework, other.Framework);
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as NuGetPackageFile;
      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return
          FilePath.GetHashCode() +
          Name.GetHashCode() +
          Version.GetHashCode() +
          (Framework != null ? Framework.GetHashCode() : 0);
      }
    }

    public override string ToString()
    {
      return String.Format("NuGet Name: {0}; Version: {1}; Framework: {2};", Name, Version, Framework);
    }
  }
}
