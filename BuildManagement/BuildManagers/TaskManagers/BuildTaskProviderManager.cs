using System;
using System.Collections.Generic;
using System.Linq;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.BuildManagers.TaskManagers
{
  public class BuildTaskProviderManager : IBuildTaskManager
  {
    private readonly IEnumerable<IBuildTaskProvider> taskProviders;

    public BuildTaskProviderManager(IEnumerable<IBuildTaskProvider> taskProviders)
    {
      if (taskProviders == null)
      {
        throw new ArgumentNullException("taskProviders");
      }

      if (!taskProviders.Any())
      {
        throw new ArgumentException("The 'taskProviders' parameter must contain at least one provider.");
      }

      this.taskProviders = taskProviders;
    }

    public IEnumerable<IBuildTask> GetTasks(IEnumerable<IProject> projects)
    {
      var projectTasks = new List<IBuildTask>();
      foreach (var taskProvider in taskProviders)
      {
        projectTasks.AddRange(taskProvider.GetTasks(projects));
      }

      return projectTasks;
    }
  }
}
