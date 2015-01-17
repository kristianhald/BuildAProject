using System;
using System.Collections.Generic;
using System.Linq;
using BuildAProject.BuildManagement.BuildManagers;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskExecutors;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers.Dependencies;
using BuildAProject.BuildManagement.Locators;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.BuildManagers
{
  [TestFixture]
  public sealed class BuildAllTasksManagerTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void BuildAllTasksManager_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(BuildAllTasksManager).GetConstructors());
    }

    [Test]
    public void Build_TasksFromProjectsFound_AreExecutedInCorrectOrder()
    {
      // Test constants
      const string rootDirectoryPath = "rootDirectoryPath";

      // Arrange

      // 1. Find projects via projects locator module
      var fakeCoolStuffThirdPartyProject = mockRepository.Create<IProject>();
      var fakeLoggingThirdPartyProject = mockRepository.Create<IProject>();
      var fakeACompileProject = mockRepository.Create<IProject>();
      var fakeAnotherCompileProject = mockRepository.Create<IProject>();
      var projects = new[]
                     {
                       fakeCoolStuffThirdPartyProject.Object,
                       fakeACompileProject.Object,
                       fakeAnotherCompileProject.Object,
                       fakeLoggingThirdPartyProject.Object
                     };

      var fakeProjectsLocator = mockRepository.Create<IProjectsLocator>();
      fakeProjectsLocator
        .Setup(projectLocator => projectLocator.FindProjects(rootDirectoryPath))
        .Returns(projects);

      // 2. Provide all projects to each BuildTaskProvider
      var fakeThirdPartyTask = mockRepository.Create<IBuildTask>();
      var fakeCompileNoDependencies = mockRepository.Create<IBuildTask>();
      var fakeCompileDependsOnThirdPartyTask = mockRepository.Create<IBuildTask>();

      var phasedBuildTasks = new BuildTaskPhaseCollectionBuilder
                             {
                               Phases =
                                 new[]
                                 {
                                   new BuildTaskPhase(1, new[]
                                                         {
                                                           fakeThirdPartyTask.Object,
                                                           fakeCompileNoDependencies.Object
                                                         }),
                                   new BuildTaskPhase(2, new[]
                                                         {
                                                           fakeCompileDependsOnThirdPartyTask.Object
                                                         })
                                 }
                             }.Build();
      var buildTasks = phasedBuildTasks.SelectMany(phase => phase.Tasks);

      var fakeBuildTaskManager = mockRepository.Create<IBuildTaskManager>();
      fakeBuildTaskManager
        .Setup(taskManager => taskManager.GetTasks(projects))
        .Returns(buildTasks);

      var fakeDependencyAlgorithm = mockRepository.Create<IDependencyAlgorithm>();
      fakeDependencyAlgorithm
        .Setup(algorithm => algorithm.OrderTasksByPhase(buildTasks))
        .Returns(phasedBuildTasks);

      // 3. Build the dependency graph of the tasks (The tasks contain the projects as to provide the dependencies) (The algorithm must only ask for its own dependency interface)

      // 4. Fail if not all dependencies are in the graph (This must be overridden somehow)

      // 5. Iterate the phases and provide all tasks for a phase to the task executor
      var mockTaskExecutor = mockRepository.Create<IBuildTaskExecutor>();
      var manager = new BuildAllTasksManager(fakeProjectsLocator.Object, fakeBuildTaskManager.Object, mockTaskExecutor.Object, fakeDependencyAlgorithm.Object);

      // Act
      manager.Build(rootDirectoryPath);

      // Assert
      mockTaskExecutor.Verify(executor => executor.Execute(phasedBuildTasks));
    }

    [Test]
    public void Build_PhasedTasksContainsUnhandledTasks_CompilesPhasedTasks()
    {
      // Constant values
      const string rootDirectoryPath = ".";

      // Arrange
      var fakeThirdPartyTask = mockRepository.Create<IBuildTask>();
      var fakeCompileNoDependencies = mockRepository.Create<IBuildTask>();
      var phasedBuildTasks = new BuildTaskPhaseCollectionBuilder
                             {
                               Phases = new[]
                                        {
                                          new BuildTaskPhase(
                                            0,
                                            new[]
                                            {
                                              fakeCompileNoDependencies.Object
                                            }),
                                        },
                               UnhandledTasks = new[]
                                                {
                                                  fakeThirdPartyTask.Object
                                                }
                             }.Build();
      var buildTasks = phasedBuildTasks.SelectMany(phase => phase.Tasks);

      var fakeProjectsLocator = mockRepository.Create<IProjectsLocator>();
      fakeProjectsLocator
        .Setup(locator => locator.FindProjects(rootDirectoryPath))
        .Returns(new[] { mockRepository.Create<IProject>().Object });

      var fakeBuildTaskManager = mockRepository.Create<IBuildTaskManager>();
      fakeBuildTaskManager
        .Setup(taskManager => taskManager.GetTasks(It.IsAny<IEnumerable<IProject>>()))
        .Returns(buildTasks);

      var fakeDependencyAlgorithm = mockRepository.Create<IDependencyAlgorithm>();
      fakeDependencyAlgorithm
        .Setup(algorithm => algorithm.OrderTasksByPhase(buildTasks))
        .Returns(phasedBuildTasks);

      var mockTaskExecutor = mockRepository.Create<IBuildTaskExecutor>();
      var manager = new BuildAllTasksManager(fakeProjectsLocator.Object, fakeBuildTaskManager.Object, mockTaskExecutor.Object, fakeDependencyAlgorithm.Object);

      // Act
      manager.Build(rootDirectoryPath);

      // Assert
      mockTaskExecutor.Verify(executor => executor.Execute(phasedBuildTasks));
    }

    [Test]
    public void Build_RootDirectoryPathParameterIsNull_ThrowsError()
    {
      // Arrange
      var fakeProjectsLocator = mockRepository.Create<IProjectsLocator>();
      var fakeBuildTaskManager = mockRepository.Create<IBuildTaskManager>();
      var fakeTaskExecutor = mockRepository.Create<IBuildTaskExecutor>();
      var fakeDependencyAlgorithm = mockRepository.Create<IDependencyAlgorithm>();
      var manager = new BuildAllTasksManager(
        fakeProjectsLocator.Object,
        fakeBuildTaskManager.Object,
        fakeTaskExecutor.Object,
        fakeDependencyAlgorithm.Object);

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => manager.Build(null));
    }

    [Test]
    public void Build_WhenNewListOfProjectsIsReturned_BuildIsIterated()
    {
      // String constants
      const string rootDirectory = ".";

      // Arrange
      var expectedProjects = new[]
                             {
                               mockRepository.Create<IProject>().Object,
                               mockRepository.Create<IProject>().Object,
                               mockRepository.Create<IProject>().Object,
                               mockRepository.Create<IProject>().Object
                             };

      var twoProjects = expectedProjects.Skip(2);
      var threeProjects = expectedProjects.Skip(1);

      var buildTasks = new[]
                       {
                         mockRepository.Create<IBuildTask>().Object,
                         mockRepository.Create<IBuildTask>().Object,
                         mockRepository.Create<IBuildTask>().Object,
                         mockRepository.Create<IBuildTask>().Object
                       };

      var stubProjectsLocator = mockRepository.Create<IProjectsLocator>();
      stubProjectsLocator
        .SetupSequence(locator => locator.FindProjects(rootDirectory))
        .Returns(twoProjects)
        .Returns(threeProjects)
        .Returns(expectedProjects)
        .Returns(expectedProjects)
        .Throws<Exception>();

      var mockBuildTaskManager = mockRepository.Create<IBuildTaskManager>();
      mockBuildTaskManager
        .Setup(buildTaskManager => buildTaskManager.GetTasks(twoProjects))
        .Returns(buildTasks.Skip(2));
      mockBuildTaskManager
        .Setup(buildTaskManager => buildTaskManager.GetTasks(threeProjects))
        .Returns(buildTasks.Skip(1));
      mockBuildTaskManager
        .Setup(buildTaskManager => buildTaskManager.GetTasks(expectedProjects))
        .Returns(buildTasks);

      var fakeDependencyAlgorithm = mockRepository.Create<IDependencyAlgorithm>();
      fakeDependencyAlgorithm
        .Setup(algorithm => algorithm.OrderTasksByPhase(It.IsAny<IEnumerable<IBuildTask>>()))
        .Returns<IEnumerable<IBuildTask>>(CreateBuildTaskPhaseCollectionFromTaskCollection);

      var mockTaskExecutor = mockRepository.Create<IBuildTaskExecutor>();

      var manager = new BuildAllTasksManager(
        stubProjectsLocator.Object,
        mockBuildTaskManager.Object,
        mockTaskExecutor.Object,
        fakeDependencyAlgorithm.Object);

      // Act
      manager.Build(rootDirectory);

      // Assert
      mockBuildTaskManager
        .Verify(taskManager => taskManager.GetTasks(expectedProjects));

      mockTaskExecutor
        .Verify(executor => executor.Execute(CreateBuildTaskPhaseCollectionFromTaskCollection(buildTasks.Skip(2))), Times.Once);
      mockTaskExecutor
        .Verify(executor => executor.Execute(CreateBuildTaskPhaseCollectionFromTaskCollection(new[] { buildTasks.Skip(1).First() })), Times.Once);
      mockTaskExecutor
        .Verify(executor => executor.Execute(CreateBuildTaskPhaseCollectionFromTaskCollection(new[] { buildTasks.First() })), Times.Once);
    }

    private static BuildTaskPhaseCollection CreateBuildTaskPhaseCollectionFromTaskCollection(IEnumerable<IBuildTask> tasks)
    {
      return new BuildTaskPhaseCollection(
        new[]
        {
          new BuildTaskPhase(0, tasks)
        },
        new IBuildTask[0]);
    }
  }
}
