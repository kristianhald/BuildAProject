using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.BuildManagers.TaskExecutors
{
  public interface IBuildTaskExecutor
  {
    void Execute(BuildTaskPhaseCollection phasedBuildTasks);
  }
}
