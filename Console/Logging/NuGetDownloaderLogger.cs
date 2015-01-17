using System;
using BuildAProject.BuildManagement.NuGet;
using BuildAProject.BuildManagement.NuGet.Downloaders;

namespace BuildAProject.Console.Logging
{
  sealed class NuGetDownloaderLogger : INuGetDownloader
  {
    private readonly INuGetDownloader composite;
    private readonly ILog logger;

    public NuGetDownloaderLogger(INuGetDownloader composite, ILog logger)
    {
      if (composite == null)
      {
        throw new ArgumentNullException("composite");
      }

      if (logger == null)
      {
        throw new ArgumentNullException("logger");
      }

      this.composite = composite;
      this.logger = logger;
    }

    public void Download(NuGetPackageFile packageFile)
    {
      logger.Information(String.Format("Starting download of {0}", GetPackageFileFriendlyName(packageFile)), 9);

      composite.Download(packageFile);

      logger.Information(String.Format("Finished download of {0}{1}", GetPackageFileFriendlyName(packageFile), Environment.NewLine), 9);
    }

    private static string GetPackageFileFriendlyName(NuGetPackageFile packageFile)
    {
      return String.Format(
        "{0}, {1}, {2}", 
        packageFile.Name, 
        packageFile.Version, 
        packageFile.Framework);
    }
  }
}
