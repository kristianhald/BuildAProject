using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers.Dependencies;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using BuildAProject.Console.Logging;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.Console.Test.Logging
{
  [TestFixture]
  public sealed class UnhandledBuildTasksLoggerTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void UnhandledBuildTasksLogger_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture().Customize(new AutoMoqCustomization());
      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(UnhandledBuildTasksLogger).GetConstructors());
    }

    [Test]
    public void GetTasks_BuildPhaseCollectionWithNoUnhandledBuildTasks_ReturnsProvidedPhasedBuildTasks()
    {
      // Arrange
      var expectedPhasedBuildTasks = new BuildTaskPhaseCollectionBuilder
                                     {
                                       UnhandledTasks = new IBuildTask[0]
                                     }.Build();

      var tasks = new IBuildTask[0];

      var stubDependencyAlgorithm = mockRepository.Create<IDependencyAlgorithm>();
      stubDependencyAlgorithm
        .Setup(algorithm => algorithm.OrderTasksByPhase(tasks))
        .Returns(expectedPhasedBuildTasks);

      var logger = new UnhandledBuildTasksLogger(stubDependencyAlgorithm.Object, mockRepository.Create<ILog>().Object);

      // Act 
      var actualPhasedBuildTasks = logger.OrderTasksByPhase(tasks);

      // Assert
      Assert.AreEqual(expectedPhasedBuildTasks, actualPhasedBuildTasks);
    }
  }
}
