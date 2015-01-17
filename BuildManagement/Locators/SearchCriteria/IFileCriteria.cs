
using System.Collections.Generic;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.Locators.SearchCriteria
{
  public interface IFileCriteria
  {
    /// <summary>
    /// Returns true if the filePath is supported else false
    /// </summary>
    bool DoesSupport(string filename);

    /// <summary>
    /// Creates projects that the file with the provided filePath contains
    /// </summary>
    IEnumerable<IProject> CreateProjectsFrom(string filePath);
  }
}
