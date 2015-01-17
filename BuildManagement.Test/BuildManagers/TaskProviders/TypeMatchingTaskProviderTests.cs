using System;
using System.Collections.Generic;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskProviders;
using Moq;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.BuildManagers.TaskProviders
{
  [TestFixture]
  public sealed class TypeMatchingTaskProviderTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void TypeMatchingTaskProvider_BuildTaskFactoryParameterIsNull_ThrowsError()
    {
      // Arrange + Act + Assert
      Assert.Throws<ArgumentNullException>(() => new TypeMatchingTaskProvider<FakeProject>(null));
    }

    [Test]
    public void GetTasks_ForSingleMatchingProject_ABuildTaskIsReturned()
    {
      // Arrange
      var mockBuildTask = mockRepository.Create<IBuildTask>();
      var expectedResults = new[]
                            {
                              mockBuildTask.Object
                            };

      var fakeProject = new FakeProject();
      var projects = new[]
                     {
                       fakeProject
                     };

      var fakeTaskFactory = mockRepository.Create<IBuildTaskFactory<FakeProject>>();
      fakeTaskFactory
        .Setup(factory => factory.Create(fakeProject))
        .Returns(mockBuildTask.Object);

      var provider = new TypeMatchingTaskProvider<FakeProject>(fakeTaskFactory.Object);

      // Act
      var actualResults = provider.GetTasks(projects);

      // Assert
      CollectionAssert.AreEquivalent(expectedResults, actualResults);
    }

    [Test]
    public void GetTasks_ForMultipleMatchingProjects_MultipleBuildTasksAreReturned()
    {
      // Arrange
      var mockBuildTaskOne = mockRepository.Create<IBuildTask>();
      var mockBuildTaskTwo = mockRepository.Create<IBuildTask>();
      var expectedResults = new[]
                            {
                              mockBuildTaskOne.Object,
                              mockBuildTaskTwo.Object
                            };

      var fakeProjectOne = new FakeProject();
      var fakeProjectTwo = new FakeProject();
      var projects = new[]
                     {
                       fakeProjectOne,
                       fakeProjectTwo
                     };

      var fakeTaskFactory = mockRepository.Create<IBuildTaskFactory<FakeProject>>();
      fakeTaskFactory
        .Setup(factory => factory.Create(fakeProjectOne))
        .Returns(mockBuildTaskOne.Object);
      fakeTaskFactory
        .Setup(factory => factory.Create(fakeProjectTwo))
        .Returns(mockBuildTaskTwo.Object);

      var provider = new TypeMatchingTaskProvider<FakeProject>(fakeTaskFactory.Object);

      // Act
      var actualResults = provider.GetTasks(projects);

      // Assert
      CollectionAssert.AreEquivalent(expectedResults, actualResults);
    }

    [Test]
    public void GetTasks_NonMatchingProjects_AreIgnored()
    {
      // Arrange
      var mockBuildTask = mockRepository.Create<IBuildTask>();
      var expectedResults = new[]
                            {
                              mockBuildTask.Object
                            };

      var fakeProject = new FakeProject();
      var fakeNonCsProject = mockRepository.Create<IProject>().Object;
      var projects = new[]
                     {
                       fakeProject,
                       fakeNonCsProject
                     };

      var fakeTaskFactory = mockRepository.Create<IBuildTaskFactory<FakeProject>>();
      fakeTaskFactory
        .Setup(factory => factory.Create(fakeProject))
        .Returns(mockBuildTask.Object);

      var provider = new TypeMatchingTaskProvider<FakeProject>(fakeTaskFactory.Object);

      // Act
      var actualResults = provider.GetTasks(projects);

      // Assert
      CollectionAssert.AreEquivalent(expectedResults, actualResults);
    }

    [Test]
    public void GetTasks_ProjectsParameterIsNull_ThrowsError()
    {
      // Arrange
      var fakeCompileTaskFactory = mockRepository.Create<IBuildTaskFactory<FakeProject>>();
      var provider = new TypeMatchingTaskProvider<FakeProject>(fakeCompileTaskFactory.Object);

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => provider.GetTasks(null));
    }

    public sealed class FakeProject : IProject
    {
      public string Name { get; private set; }

      public IEnumerable<Dependency> Dependencies { get; private set; }
    }
  }
}
