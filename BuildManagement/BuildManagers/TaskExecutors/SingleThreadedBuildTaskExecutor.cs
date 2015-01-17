using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.BuildManagers.TaskExecutors
{
  public class SingleThreadedBuildTaskExecutor : IBuildTaskExecutor
  {
    public void Execute(BuildTaskPhaseCollection phasedBuildTasks)
    {
      if (phasedBuildTasks == null)
      {
        throw new ArgumentNullException("phasedBuildTasks");
      }

      foreach (var buildTaskPhase in phasedBuildTasks)
      {
        foreach (var buildTask in buildTaskPhase.Tasks)
        {
          buildTask.Execute();
        }
      }
    }
  }
}
