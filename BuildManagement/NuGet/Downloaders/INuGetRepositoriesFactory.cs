using System.Collections.Generic;
using BuildAProject.BuildManagement.NuGet.Configurations;
using NuGet;

namespace BuildAProject.BuildManagement.NuGet.Downloaders
{
  public interface INuGetRepositoriesFactory
  {
    IEnumerable<IPackageRepository> Create(NuGetConfig configFile);
  }
}
