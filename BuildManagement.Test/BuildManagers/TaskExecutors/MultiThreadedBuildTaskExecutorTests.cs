using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskExecutors;
using Moq;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.BuildManagers.TaskExecutors
{
  [TestFixture]
  public sealed class MultiThreadedBuildTaskExecutorTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void Execute_PhasedBuildTasksParameterIsNull_ThrowsError()
    {
      // Arrange
      var executor = new MultiThreadedBuildTaskExecutor();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => executor.Execute(null));
    }

    [Test]
    public void Execute_BuildTasksProvided_AreExecutedInPhase()
    {
      // Arrange
      var mockBuildTaskOne = mockRepository.Create<IBuildTask>();
      var mockBuildTaskTwo = mockRepository.Create<IBuildTask>();
      var mockBuildTaskThree = mockRepository.Create<IBuildTask>();

      var phasedBuildTasks = new BuildTaskPhaseCollection(
        new[]
        {
          new BuildTaskPhase(0, new[] {mockBuildTaskOne.Object, mockBuildTaskTwo.Object}),
          new BuildTaskPhase(1, new[] {mockBuildTaskThree.Object}),
        },
        new IBuildTask[0]);

      var executor = new MultiThreadedBuildTaskExecutor();

      // Act
      executor.Execute(phasedBuildTasks);

      // Assert
      mockBuildTaskOne.Verify(task => task.Execute(), Times.Once);
      mockBuildTaskTwo.Verify(task => task.Execute(), Times.Once);
      mockBuildTaskThree.Verify(task => task.Execute(), Times.Once);
    }
  }
}
