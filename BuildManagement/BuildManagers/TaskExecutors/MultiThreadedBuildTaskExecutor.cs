using System;
using System.Threading.Tasks;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.BuildManagers.TaskExecutors
{
  public class MultiThreadedBuildTaskExecutor : IBuildTaskExecutor
  {
    public void Execute(BuildTaskPhaseCollection phasedBuildTasks)
    {
      if (phasedBuildTasks == null)
      {
        throw new ArgumentNullException("phasedBuildTasks");
      }

      foreach (var buildTaskPhase in phasedBuildTasks)
      {
        Parallel.ForEach(buildTaskPhase.Tasks, buildTask => buildTask.Execute());
      }
    }
  }
}
