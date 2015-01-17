using System;
using System.Collections.Generic;
using System.Linq;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.BuildManagers.TaskManagers.Dependencies
{
  public class SimpleDependencyAlgorithm : IDependencyAlgorithm
  {
    public BuildTaskPhaseCollection OrderTasksByPhase(IEnumerable<IBuildTask> tasks)
    {
      if (tasks == null)
      {
        throw new ArgumentNullException("tasks");
      }

      var buildPhases = new List<BuildTaskPhase>();

      var tasksLeft = tasks;
      var tasksHandled = new List<IBuildTask>();
      var order = 0;
      do
      {
        var tasksInPhase = tasksLeft
          .Where(task => !task.Dependencies.Any() || task.Dependencies.All(dependency => tasksHandled.Any(dependency.Equals)))
          .ToList();

        if (!tasksInPhase.Any())
        {
          return new BuildTaskPhaseCollection(buildPhases, tasksLeft.ToList());
        }

        var buildPhase = new BuildTaskPhase(order, tasksInPhase);
        buildPhases.Add(buildPhase);

        tasksLeft = tasksLeft.Where(task => !tasksInPhase.Contains(task));

        tasksHandled.AddRange(tasksInPhase);
        order++;
      } while (tasksLeft.Any());

      return new BuildTaskPhaseCollection(buildPhases, new IBuildTask[0]);
    }
  }
}
