using System;
using System.Collections.Generic;
using System.Linq;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers;

namespace BuildAProject.Console.Logging
{
  sealed class BuildTaskManagerLogger : IBuildTaskManager
  {
    private readonly IBuildTaskManager composite;
    private readonly ILog logger;

    public BuildTaskManagerLogger(IBuildTaskManager composite, ILog logger)
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

    public IEnumerable<IBuildTask> GetTasks(IEnumerable<IProject> projects)
    {
      logger.Information(String.Format("Getting project tasks"), 7);

      var buildTasksRetrieved = composite.GetTasks(projects);

      logger.Information(String.Format("{0} tasks retrieved", buildTasksRetrieved.Count()), 8);
      logger.Information("", 7);

      return buildTasksRetrieved;
    }
  }
}
