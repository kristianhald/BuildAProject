using System;
using System.Collections.Generic;
using System.Linq;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.Locators;

namespace BuildAProject.Console.Logging
{
  sealed class ProjectsLocatorLogger : IProjectsLocator
  {
    private readonly IProjectsLocator composite;
    private readonly ILog logger;

    public ProjectsLocatorLogger(IProjectsLocator composite, ILog logger)
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

    public IEnumerable<IProject> FindProjects(string rootDirectoryPath)
    {
      logger.Information(String.Format("Looking for new projects in '{0}'", rootDirectoryPath), 1);
      var projectsFound = composite.FindProjects(rootDirectoryPath);
      logger.Information(String.Format("{0} projects found{1}", projectsFound.Count(), Environment.NewLine), 4);

      return projectsFound;
    }
  }
}
