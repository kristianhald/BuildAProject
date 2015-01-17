using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers.Dependencies;

[assembly: InternalsVisibleTo("BuildAProject.Console.Test")]

namespace BuildAProject.Console.Logging
{
  internal class UnhandledBuildTasksLogger : IDependencyAlgorithm
  {
    private readonly IDependencyAlgorithm dependencyAlgorithm;
    private readonly ILog log;

    private static string errorMessage; // TODO: This is a bad design, but I do not know how to get the message to write only when running the last time

    public UnhandledBuildTasksLogger(IDependencyAlgorithm dependencyAlgorithm, ILog log)
    {
      if (dependencyAlgorithm == null)
      {
        throw new ArgumentNullException("dependencyAlgorithm");
      }

      if (log == null)
      {
        throw new ArgumentNullException("log");
      }

      this.dependencyAlgorithm = dependencyAlgorithm;
      this.log = log;
    }

    public BuildTaskPhaseCollection OrderTasksByPhase(IEnumerable<IBuildTask> tasks)
    {
      errorMessage = null;

      var buildTaskPhaseCollection = dependencyAlgorithm.OrderTasksByPhase(tasks);
      if (buildTaskPhaseCollection.UnhandledTasks.Any())
      {
        var allTasks = buildTaskPhaseCollection
          .SelectMany(phase => phase.Tasks);

        var taskErrorMessages = new List<string>();
        foreach (var unhandledTask in buildTaskPhaseCollection.UnhandledTasks)
        {
          var unhandledDependencies = unhandledTask
            .Dependencies
            .Where(dependency => !allTasks.Any(dependency.Equals));

          var taskErrorMessage = String.Format(
            "{0} were missing:{1} -{2}",
            unhandledTask.Name,
            Environment.NewLine,
            String.Join(Environment.NewLine + " -", unhandledDependencies));

          taskErrorMessages.Add(taskErrorMessage);
        }

        errorMessage = String.Format(
          "There were build tasks that are missing dependencies. They are{0}{1}",
          Environment.NewLine,
          String.Join(Environment.NewLine, taskErrorMessages));
      }

      return buildTaskPhaseCollection;
    }

    public static string GetErrorMessage()
    {
      return errorMessage;
    }
  }
}
