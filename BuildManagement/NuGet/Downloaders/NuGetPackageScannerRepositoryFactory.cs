using System;
using System.Collections.Generic;
using System.Linq;
using BuildAProject.BuildManagement.NuGet.Configurations;
using NuGet;

namespace BuildAProject.BuildManagement.NuGet.Downloaders
{
  public class NuGetPackageScannerRepositoryFactory : INuGetRepositoriesFactory
  {
    private readonly IPackageRepositoryFactory packageRepositoryFactory;

    public NuGetPackageScannerRepositoryFactory(IPackageRepositoryFactory packageRepositoryFactory)
    {
      if (packageRepositoryFactory == null)
        throw new ArgumentNullException("packageRepositoryFactory");

      this.packageRepositoryFactory = packageRepositoryFactory;
    }

    public IEnumerable<IPackageRepository> Create(NuGetConfig configFile)
    {
      if (configFile == null)
        throw new ArgumentNullException("configFile");

      return configFile
        .PackageSources
        .Select(packageSource => packageRepositoryFactory.CreateRepository(packageSource));
    }
  }
}
