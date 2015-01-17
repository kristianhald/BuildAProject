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
  public sealed class DependencyAlgorithmLoggerTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void DependencyAlgorithmLogger_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture().Customize(new AutoMoqCustomization());
      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(DependencyAlgorithmLogger).GetConstructors());
    }

    [Test]
    public void OrderTasksByPhase_WhenCalled_ThenTheCompositeIsCalled()
    {
      // Arrange
      var expectedPhases = new BuildTaskPhaseCollectionBuilder().Build();

      var buildTasks = new[]
                       {
                         mockRepository.Create<IBuildTask>().Object
                       };

      var mockComposite = mockRepository.Create<IDependencyAlgorithm>();
      mockComposite
        .Setup(algorithm => algorithm.OrderTasksByPhase(buildTasks))
        .Returns(expectedPhases);
      var fakeLog = mockRepository.Create<ILog>();

      var logger = new DependencyAlgorithmLogger(mockComposite.Object, fakeLog.Object);

      // Act
      var actualPhases = logger.OrderTasksByPhase(buildTasks);

      // Assert
      Assert.AreEqual(expectedPhases, actualPhases);
    }

    [Test]
    public void OrderTasksByPhase_WhenCalled_ThenTheInformationMethodOnTheLogIsCalled()
    {
      // Arrange
      var fakeComposite = mockRepository.Create<IDependencyAlgorithm>();
      var mockLog = mockRepository.Create<ILog>();

      var logger = new DependencyAlgorithmLogger(fakeComposite.Object, mockLog.Object);

      // Act
      logger.OrderTasksByPhase(new IBuildTask[0]);

      // Assert
      mockLog
        .Verify(log => log.Information(It.IsAny<string>(), It.IsAny<int>()));
    }
  }
}
