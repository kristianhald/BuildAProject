using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskProviders;
using BuildAProject.BuildManagement.NuGet.Downloaders;

namespace BuildAProject.BuildManagement.NuGet.TaskProviders
{
  public class NuGetPackageDownloadTaskFactory : IBuildTaskFactory<NuGetPackageFile>
  {
    private readonly INuGetDownloader nuGetDownloader;

    public NuGetPackageDownloadTaskFactory(INuGetDownloader nuGetDownloader)
    {
      if (nuGetDownloader == null)
      {
        throw new ArgumentNullException("nuGetDownloader");
      }

      this.nuGetDownloader = nuGetDownloader;
    }

    public IBuildTask Create(NuGetPackageFile packageFile)
    {
      return new NuGetPackageDownloadTask(packageFile, nuGetDownloader);
    }
  }
}
