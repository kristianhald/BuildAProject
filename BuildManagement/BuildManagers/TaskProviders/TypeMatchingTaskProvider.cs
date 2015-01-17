using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers;

[assembly: InternalsVisibleTo("BuildAProject.BuildManagement.Test")]

namespace BuildAProject.BuildManagement.BuildManagers.TaskProviders
{
  public class TypeMatchingTaskProvider<TProject> : IBuildTaskProvider where TProject : class, IProject
  {
    private readonly IBuildTaskFactory<TProject> buildTaskFactory;

    public TypeMatchingTaskProvider(IBuildTaskFactory<TProject> buildTaskFactory)
    {
      if (buildTaskFactory == null)
      {
        throw new ArgumentNullException();
      }

      this.buildTaskFactory = buildTaskFactory;
    }

    public IEnumerable<IBuildTask> GetTasks(IEnumerable<IProject> projects)
    {
      if (projects == null)
      {
        throw new ArgumentNullException("projects");
      }

      var buildTasks = new List<IBuildTask>();
      foreach (var project in projects)
      {
        if (project is TProject)
          buildTasks.Add(buildTaskFactory.Create(project as TProject));
      }

      return buildTasks;
    }
  }
}
