using System.Collections.Generic;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers;
using Moq;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class BuildTaskProviderManagerBuilder
  {
    private static readonly MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    private static readonly Mock<IBuildTaskProvider> FakeTaskProvider = MockRepository.Create<IBuildTaskProvider>();

    public IEnumerable<IBuildTaskProvider> TaskProviders = new[]
                                                           {
                                                             FakeTaskProvider.Object
                                                           };

    public BuildTaskProviderManager Build()
    {
      return new BuildTaskProviderManager(TaskProviders);
    }
  }
}
