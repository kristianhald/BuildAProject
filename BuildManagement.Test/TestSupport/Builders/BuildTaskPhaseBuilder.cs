using System.Collections.Generic;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using Moq;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class BuildTaskPhaseBuilder
  {
    private static readonly MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    private static readonly IBuildTask BuildTask1 = MockRepository.Create<IBuildTask>().Object;
    private static readonly IBuildTask BuildTask2 = MockRepository.Create<IBuildTask>().Object;

    public int Order = 1;

    public IEnumerable<IBuildTask> BuildTasks = new[]
                                                       {
                                                         BuildTask1,
                                                         BuildTask2
                                                       };

    public BuildTaskPhase Build()
    {
      return new BuildTaskPhase(Order, BuildTasks);
    }
  }
}
