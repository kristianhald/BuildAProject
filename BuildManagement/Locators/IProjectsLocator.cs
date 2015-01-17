using System.Collections.Generic;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.Locators
{
  public interface IProjectsLocator
  {
    IEnumerable<IProject> FindProjects(string rootDirectoryPath);
  }
}