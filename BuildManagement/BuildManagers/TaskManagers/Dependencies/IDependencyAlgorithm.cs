using System.Collections.Generic;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.BuildManagers.TaskManagers.Dependencies
{
  public interface IDependencyAlgorithm
  {
    BuildTaskPhaseCollection OrderTasksByPhase(IEnumerable<IBuildTask> tasks);
  }
}
