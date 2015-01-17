using System;
using System.Collections.Generic;
using System.Linq;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.Locators.SearchCriteria;

namespace BuildAProject.BuildManagement.Locators
{
  public class CriteriaProjectsLocator : IProjectsLocator
  {
    private readonly ILocatorFileSystem locatorFileSystem;
    private readonly IEnumerable<IFileCriteria> fileCriterias;

    public CriteriaProjectsLocator(ILocatorFileSystem locatorFileSystem, IEnumerable<IFileCriteria> fileCriterias)
    {
      if (locatorFileSystem == null)
      {
        throw new ArgumentNullException("locatorFileSystem");
      }

      if (fileCriterias == null)
      {
        throw new ArgumentNullException("fileCriterias");
      }

      if (!fileCriterias.Any())
      {
        throw new ArgumentException("There must be some file criterias provided or else no projects will be found.");
      }

      this.locatorFileSystem = locatorFileSystem;
      this.fileCriterias = fileCriterias;
    }

    public IEnumerable<IProject> FindProjects(string rootDirectoryPath)
    {
      if (String.IsNullOrWhiteSpace(rootDirectoryPath))
      {
        throw new ArgumentNullException("rootDirectoryPath");
      }

      var projectsFound = new List<IProject>();

      var allFilenamesInDirectories = locatorFileSystem.GetAllFilenames(rootDirectoryPath);
      foreach (var filename in allFilenamesInDirectories)
      {
        foreach (var criteria in fileCriterias.Where(criteria => criteria.DoesSupport(filename)))
        {
          projectsFound.AddRange(criteria.CreateProjectsFrom(filename));
        }
      }

      return projectsFound;
    }
  }
}
