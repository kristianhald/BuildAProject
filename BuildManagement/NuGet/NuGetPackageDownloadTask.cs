using System;
using System.Collections.Generic;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.NuGet.Downloaders;

namespace BuildAProject.BuildManagement.NuGet
{
  public class NuGetPackageDownloadTask : IBuildTask, IEquatable<NuGetPackageDownloadTask>
  {
    private readonly NuGetPackageFile packageFile;
    private readonly INuGetDownloader downloader;

    public NuGetPackageDownloadTask(NuGetPackageFile packageFile, INuGetDownloader downloader)
    {
      if (packageFile == null)
      {
        throw new ArgumentNullException("packageFile");
      }

      if (downloader == null)
      {
        throw new ArgumentNullException("downloader");
      }

      this.packageFile = packageFile;
      this.downloader = downloader;
    }

    public IEnumerable<Dependency> Dependencies { get { return packageFile.Dependencies; } }

    public string Name { get { return packageFile.Name; } }

    public void Execute()
    {
      downloader.Download(packageFile);
    }

    public bool Equals(NuGetPackageDownloadTask other)
    {
      return
        other != null &&
        packageFile.Equals(other.packageFile);
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as NuGetPackageDownloadTask;
      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      return packageFile.GetHashCode();
    }

    public override string ToString()
    {
      return packageFile.ToString();
    }
  }
}
