using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildAProject.BuildManagement.NuGet.Configurations
{
  public class NuGetConfig
  {
    public NuGetConfig(string repositoryPath, IEnumerable<string> packageSources)
    {
      if (String.IsNullOrWhiteSpace(repositoryPath))
        throw new ArgumentNullException("repositoryPath");

      if (packageSources == null)
        throw new ArgumentNullException("packageSources");

      if (!packageSources.Any())
        throw new ArgumentOutOfRangeException("packageSources", "No package sources were found.");

      RepositoryPath = repositoryPath;
      PackageSources = packageSources;
    }

    public string RepositoryPath { get; private set; }

    public IEnumerable<string> PackageSources { get; private set; }
  }
}
