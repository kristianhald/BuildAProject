using System;
using System.IO;
using BuildAProject.BuildManagement.NuGet.Configurations;
using NuGet;

namespace BuildAProject.BuildManagement.NuGet.Downloaders
{
  // The design principles used have been found from the following sites:
  // http://blog.nuget.org/20130520/Play-with-packages.html
  public class RepositoryNuGetDownloader : INuGetDownloader
  {
    private readonly INuGetRepositoriesFactory repositoriesFactory;
    private readonly INuGetConfigFileReader configFileReader;

    public RepositoryNuGetDownloader(INuGetRepositoriesFactory repositoriesFactory, INuGetConfigFileReader configFileReader)
    {
      if (repositoriesFactory == null)
        throw new ArgumentNullException("repositoriesFactory");

      if (configFileReader == null)
        throw new ArgumentNullException("configFileReader");

      this.repositoriesFactory = repositoriesFactory;
      this.configFileReader = configFileReader;
    }

    public void Download(NuGetPackageFile packageFile)
    {
      if (packageFile == null)
      {
        throw new ArgumentNullException("packageFile");
      }

      var nugetConfig = configFileReader.Read(packageFile.FilePath);
      var absoluteRepositoryPath = GetRepositoryPath(nugetConfig.RepositoryPath, packageFile.FilePath);

      IPackageRepository repositoryWithPackage = null;
      foreach (var repository in repositoriesFactory.Create(nugetConfig))
      {
        var packageFound = repository.FindPackage(packageFile.Name, new SemanticVersion(packageFile.Version), true, false);
        if (packageFound == null)
          continue;

        repositoryWithPackage = repository;
        break;
      }

      if (repositoryWithPackage == null)
        throw new InvalidOperationException(
          String.Format("Package '{0}' could not be found with version '{1}'", packageFile.Name, packageFile.Version));

      var packageManager = new PackageManager(repositoryWithPackage, absoluteRepositoryPath);

      packageManager.InstallPackage(packageFile.Name, new SemanticVersion(packageFile.Version), true, true);
    }

    private string GetRepositoryPath(string repositoryPath, string filePath)
    {
      return Path.IsPathRooted(repositoryPath) ? repositoryPath : Path.Combine(Directory.GetParent(filePath).FullName, repositoryPath);
    }
  }
}
