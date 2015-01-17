using BuildAProject.BuildManagement.BuildManagers.TaskExecutors;
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
  public sealed class BuildTaskExecutorLoggerTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void BuildTaskExecutorLogger_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture().Customize(new AutoMoqCustomization());
      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(BuildTaskExecutorLogger).GetConstructors());
    }

    [Test]
    public void Execute_WhenCalled_ThenTheCompositeIsCalled()
    {
      // Arrange
      var expectedPhases = new BuildTaskPhaseCollectionBuilder().Build();

      var mockComposite = mockRepository.Create<IBuildTaskExecutor>();
      var fakeLog = mockRepository.Create<ILog>();

      var logger = new BuildTaskExecutorLogger(mockComposite.Object, fakeLog.Object);

      // Act
      logger.Execute(expectedPhases);

      // Assert
      mockComposite
        .Verify(executor => executor.Execute(expectedPhases));
    }

    [Test]
    public void Execute_WhenCalled_ThenTheInformationMethodOnTheLogIsCalled()
    {
      // Arrange
      var expectedPhases = new BuildTaskPhaseCollectionBuilder().Build();

      var fakeComposite = mockRepository.Create<IBuildTaskExecutor>();
      var mockLog = mockRepository.Create<ILog>();

      var logger = new BuildTaskExecutorLogger(fakeComposite.Object, mockLog.Object);

      // Act
      logger.Execute(expectedPhases);

      // Assert
      mockLog
        .Verify(log => log.Information(It.IsAny<string>(), It.IsAny<int>()), Times.Once());
    }
  }
}
