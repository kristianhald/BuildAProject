using System;
using System.Linq;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskExecutors;

namespace BuildAProject.Console.Logging
{
  sealed class BuildTaskExecutorLogger : IBuildTaskExecutor
  {
    private readonly IBuildTaskExecutor composite;
    private readonly ILog logger;

    public BuildTaskExecutorLogger(IBuildTaskExecutor composite, ILog logger)
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

    public void Execute(BuildTaskPhaseCollection phasedBuildTasks)
    {
      var numberOfBuildTasks = phasedBuildTasks
        .Sum(buildTasks => buildTasks.Tasks.Count());
      logger.Information(String.Format("Executing {0} tasks{1}", numberOfBuildTasks, Environment.NewLine), 7);

      composite.Execute(phasedBuildTasks);
    }
  }
}
