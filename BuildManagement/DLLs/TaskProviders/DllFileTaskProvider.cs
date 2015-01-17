using System;
using System.Collections.Generic;
using System.Linq;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers;
using BuildAProject.BuildManagement.BuildManagers.TaskProviders;

namespace BuildAProject.BuildManagement.DLLs.TaskProviders
{
  public class DllFileTaskProvider : IBuildTaskProvider
  {
    private readonly IBuildTaskFactory<DllFileProject> buildTaskFactory;

    public DllFileTaskProvider(IBuildTaskFactory<DllFileProject> buildTaskFactory)
    {
      if (buildTaskFactory == null)
      {
        throw new ArgumentNullException("buildTaskFactory");
      }

      this.buildTaskFactory = buildTaskFactory;
    }

    public IEnumerable<IBuildTask> GetTasks(IEnumerable<IProject> projects)
    {
      var nonDllFileProjects = projects.Where(x => !(x is DllFileProject));

      var buildTasks = new List<IBuildTask>();
      foreach (var project in projects.Where(x => x is DllFileProject))
      {
        if (nonDllFileProjects.Any(nonProject => String.Equals(nonProject.Name, project.Name)))
          continue;

        buildTasks.Add(buildTaskFactory.Create(project as DllFileProject));
      }

      return buildTasks;
    }
  }
}
