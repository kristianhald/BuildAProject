using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers.Dependencies;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using Moq;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.BuildManagers.TaskManagers.Dependencies
{
  [TestFixture]
  public sealed class SimpleDependencyAlgorithmTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void OrderTasksByPhase_ProjectTasksParameterIsNull_ThrowsError()
    {
      // Arrange
      var algorithm = new SimpleDependencyAlgorithm();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => algorithm.OrderTasksByPhase(null));
    }

    [Test]
    public void OrderTasksByPhase_TasksWithNoDependencies_ArePutInPhaseZero()
    {
      // Arrange
      var mockTaskOne = mockRepository.Create<IBuildTask>();
      mockTaskOne
        .Setup(task => task.Dependencies)
        .Returns(new Dependency[0]);
      var mockTaskTwo = mockRepository.Create<IBuildTask>();
      mockTaskTwo
        .Setup(task => task.Dependencies)
        .Returns(new Dependency[0]);

      var expectedResult = new BuildTaskPhaseCollectionBuilder
                           {
                             Phases =
                               new[]
                               {
                                 new BuildTaskPhase(
                                   0,
                                   new[]
                                   {
                                     mockTaskOne.Object,
                                     mockTaskTwo.Object
                                   })
                               }
                           }.Build();

      var projectTasks = new[]
                         {
                           mockTaskOne.Object,
                           mockTaskTwo.Object
                         };

      var algorithm = new SimpleDependencyAlgorithm();

      // Act
      var actualResult = algorithm.OrderTasksByPhase(projectTasks);

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void OrderTasksByPhase_TasksWithDependenciesToTasksWithout_ArePutInPhaseOne()
    {
      // Arrange
      var mockTaskNoDependencies = mockRepository.Create<IBuildTask>();
      mockTaskNoDependencies
        .Setup(task => task.Dependencies)
        .Returns(new Dependency[0]);
      mockTaskNoDependencies
        .Setup(task => task.Name)
        .Returns("TaskNoDependencies");

      var mockTaskWithDependency = mockRepository.Create<IBuildTask>();
      mockTaskWithDependency
        .Setup(task => task.Dependencies)
        .Returns(new[] { new DependencyBuilder { DependencyName = "TaskNoDependencies" }.Build() });

      var expectedResult = new BuildTaskPhaseCollectionBuilder
                           {
                             Phases =
                               new[]
                               {
                                 new BuildTaskPhase(
                                   0,
                                   new[]
                                   {
                                     mockTaskNoDependencies.Object
                                   }),
                                 new BuildTaskPhase(
                                   1,
                                   new[]
                                   {
                                     mockTaskWithDependency.Object
                                   }),
                               }
                           }.Build();

      var projectTasks = new[]
                         {
                           mockTaskNoDependencies.Object,
                           mockTaskWithDependency.Object
                         };

      var algorithm = new SimpleDependencyAlgorithm();

      // Act
      var actualResult = algorithm.OrderTasksByPhase(projectTasks);

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void OrderTasksByPhase_TasksWithDependenciesToTasksAlsoWithDependencies_ArePutInPhaseTwo()
    {
      // Arrange
      var mockTaskNoDependencies = mockRepository.Create<IBuildTask>();
      mockTaskNoDependencies
        .Setup(task => task.Dependencies)
        .Returns(new Dependency[0]);
      mockTaskNoDependencies
        .Setup(task => task.Name)
        .Returns("TaskNoDependencies");

      var mockTaskWithDependencyOne = mockRepository.Create<IBuildTask>();
      mockTaskWithDependencyOne
        .Setup(task => task.Dependencies)
        .Returns(new[] { new DependencyBuilder { DependencyName = "TaskNoDependencies" }.Build() });
      mockTaskWithDependencyOne
        .Setup(task => task.Name)
        .Returns("TaskWithSingleDependencyLayer");

      var mockTaskWithDependencyTwo = mockRepository.Create<IBuildTask>();
      mockTaskWithDependencyTwo
        .Setup(task => task.Dependencies)
        .Returns(new[] { new DependencyBuilder { DependencyName = "TaskWithSingleDependencyLayer" }.Build() });

      var expectedResult = new BuildTaskPhaseCollectionBuilder
                           {
                             Phases =
                               new[]
                               {
                                 new BuildTaskPhase(
                                   0,
                                   new[]
                                   {
                                     mockTaskNoDependencies.Object
                                   }),
                                 new BuildTaskPhase(
                                   1,
                                   new[]
                                   {
                                     mockTaskWithDependencyOne.Object
                                   }),
                                 new BuildTaskPhase(
                                   2,
                                   new[]
                                   {
                                     mockTaskWithDependencyTwo.Object
                                   }),
                               }
                           }.Build();

      var projectTasks = new[]
                         {
                           mockTaskNoDependencies.Object,
                           mockTaskWithDependencyOne.Object,
                           mockTaskWithDependencyTwo.Object
                         };

      var algorithm = new SimpleDependencyAlgorithm();

      // Act
      var actualResult = algorithm.OrderTasksByPhase(projectTasks);

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void OrderTasksByPhase_TasksWithUnhandledDependencies_ArePutIntoTheUnhandledTasksList()
    {
      // Arrange
      var mockTask = mockRepository.Create<IBuildTask>();
      mockTask
        .Setup(task => task.Dependencies)
        .Returns(new[] { new DependencyBuilder().Build() });

      var expectedResult = new BuildTaskPhaseCollectionBuilder
                           {
                             Phases = new BuildTaskPhase[0],
                             UnhandledTasks = new[]
                                              {
                                                mockTask.Object
                                              }
                           }.Build();

      var projectTasks = new[]
                         {
                           mockTask.Object
                         };

      var algorithm = new SimpleDependencyAlgorithm();

      // Act
      var actualResult = algorithm.OrderTasksByPhase(projectTasks);

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }
  }
}
