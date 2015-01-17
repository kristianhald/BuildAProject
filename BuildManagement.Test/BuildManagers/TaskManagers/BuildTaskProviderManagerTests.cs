using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.BuildManagers.TaskManagers
{
  [TestFixture]
  public sealed class BuildTaskProviderManagerTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void BuildTaskProviderManager_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(BuildTaskProviderManager).GetConstructors());
    }

    [Test]
    public void BuildTaskProviderManager_TaskProviderParameterIsEmpty_ThrowsError()
    {
      // Arrange + Act + Assert
      Assert.Throws<ArgumentException>(() => new BuildTaskProviderManagerBuilder { TaskProviders = new IBuildTaskProvider[0] }.Build());
    }

    [Test]
    public void GetTasks_FromProvidedProjects_AndReturnedWithCorrectPhases()
    {
      // Arrange
      var fakeTask1 = mockRepository.Create<IBuildTask>();
      var fakeTask2 = mockRepository.Create<IBuildTask>();
      var fakeTask3 = mockRepository.Create<IBuildTask>();

      var expectedResult = new[]
                           {
                             fakeTask1.Object,
                             fakeTask2.Object,
                             fakeTask3.Object
                           };

      var projects = new[]
                     {
                       mockRepository.Create<IProject>().Object,
                       mockRepository.Create<IProject>().Object
                     };

      var fakeTaskProvider1 = mockRepository.Create<IBuildTaskProvider>();
      fakeTaskProvider1
        .Setup(provider => provider.GetTasks(projects))
        .Returns(new[] { fakeTask1.Object, fakeTask2.Object });

      var fakeTaskProvider2 = mockRepository.Create<IBuildTaskProvider>();
      fakeTaskProvider2
        .Setup(provider => provider.GetTasks(projects))
        .Returns(new[] { fakeTask3.Object });

      var manager = new BuildTaskProviderManager(
        new[]
        {
          fakeTaskProvider1.Object,
          fakeTaskProvider2.Object
        });

      // Act
      var actualResult = manager.GetTasks(projects);

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }
  }
}
