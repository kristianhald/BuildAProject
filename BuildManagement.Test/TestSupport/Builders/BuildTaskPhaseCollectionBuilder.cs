using System.Collections.Generic;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  public sealed class BuildTaskPhaseCollectionBuilder
  {
    public IEnumerable<BuildTaskPhase> Phases = new[]
                                                {
                                                  new BuildTaskPhaseBuilder().Build()
                                                };

    public IEnumerable<IBuildTask> UnhandledTasks = new IBuildTask[0];

    public BuildTaskPhaseCollection Build()
    {
      return new BuildTaskPhaseCollection(Phases, UnhandledTasks);
    }
  }
}
