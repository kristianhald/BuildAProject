using System;
using BuildAProject.BuildManagement.CsProjects.Compilers.Logging;
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
  public sealed class BuildEngineCompilerLoggerTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void BuildEngineCompilerLogger_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture().Customize(new AutoMoqCustomization());
      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(BuildEngineCompilerLogger).GetConstructors());
    }

    [Test]
    public void BuildStarted_WhenCalled_CallsTheInformationMethodOnTheLog()
    {
      // Arrange
      var mockLog = mockRepository.Create<ILog>();

      var logger = new BuildEngineCompilerLogger(mockLog.Object);

      // Act
      logger.BuildStarted(new DotNetCompilerBuildStarted(new CsProjectBuilder().Build()));

      // Assert
      mockLog
        .Verify(log => log.Information(It.IsAny<string>(), It.IsAny<int>()));
    }

    [Test]
    public void BuildStarted_WhenCalled_TheCleansTheErrorLogForThatProject()
    {
      // Arrange
      var mockLog = mockRepository.Create<ILog>();

      var project = new CsProjectBuilder().Build();

      var logger = new BuildEngineCompilerLogger(mockLog.Object);
      logger.ErrorRaised(new DotNetCompilerBuildError(project, "This error is removed when calling 'BuildStarted'."));

      // Act
      logger.BuildStarted(new DotNetCompilerBuildStarted(project));

      // Assert
      logger.BuildFinished(new DotNetCompilerBuildFinished(project, BuildStatus.Success)); // This should not throw an exception, but will if there are any build errors
    }

    [Test]
    public void ErrorRaised_WhenCalled_ThenDoesNotCallTheLogger()
    {
      // Arrange
      var mockLog = mockRepository.Create<ILog>();

      var logger = new BuildEngineCompilerLogger(mockLog.Object);

      // Act
      logger.ErrorRaised(new DotNetCompilerBuildError(new CsProjectBuilder().Build(), "One Message"));

      // Assert
      mockLog
        .Verify(log => log.Error(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
      mockLog
        .Verify(log => log.Information(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Test]
    public void ErrorRaised_WhenCalledMultipleTimesFromMultipleProjects_ThenStoresTheErrorsSeparatelyAndPrintsForEachProjectWhenTheBuildingIsFinished()
    {
      // Arrange
      var mockLog = mockRepository.Create<ILog>();

      var logger = new BuildEngineCompilerLogger(mockLog.Object);

      var firstProject = new CsProjectBuilder
                         {
                           Name = "First Project"
                         }.Build();
      var secondProject = new CsProjectBuilder
                          {
                            Name = "Second Project"
                          }.Build();

      // Act
      logger.ErrorRaised(new DotNetCompilerBuildError(firstProject, "One Message on " + firstProject.Name));
      logger.ErrorRaised(new DotNetCompilerBuildError(secondProject, "One Message on " + secondProject.Name));
      logger.ErrorRaised(new DotNetCompilerBuildError(firstProject, "Second Message on " + firstProject.Name));

      // Assert
      logger.BuildFinished(new DotNetCompilerBuildFinished(firstProject, BuildStatus.Failed));

      mockLog
        .Verify(log => log.Error(It.Is<string>(message => !message.Contains(firstProject.Name)), It.IsAny<int>()), Times.Never());

      mockLog.ResetCalls();

      logger.BuildFinished(new DotNetCompilerBuildFinished(secondProject, BuildStatus.Failed));

      mockLog
        .Verify(log => log.Error(It.Is<string>(message => !message.Contains(secondProject.Name)), It.IsAny<int>()), Times.Never());
    }

    [Test]
    public void BuildFinished_WhenCalledWithSuccess_CallsTheInformationMethodOnTheLog()
    {
      // Arrange
      var mockLog = mockRepository.Create<ILog>();

      var logger = new BuildEngineCompilerLogger(mockLog.Object);

      // Act
      logger.BuildFinished(new DotNetCompilerBuildFinished(new CsProjectBuilder().Build(), BuildStatus.Success));

      // Assert
      mockLog
        .Verify(log => log.Information(It.IsAny<string>(), It.IsAny<int>()));
    }

    [Test]
    public void BuildFinished_WhenCalledWithSuccessButWithBuildErrors_ThrowsError()
    {
      // Arrange
      var mockLog = mockRepository.Create<ILog>();

      var logger = new BuildEngineCompilerLogger(mockLog.Object);
      logger.ErrorRaised(new DotNetCompilerBuildError(new CsProjectBuilder().Build(), "Some error"));

      // Act + Assert
      Assert.Throws<Exception>(() => logger.BuildFinished(new DotNetCompilerBuildFinished(new CsProjectBuilder().Build(), BuildStatus.Success)));
    }

    [Test]
    public void BuildFinished_WhenCalledWithFailedButNoBuildErrors_ThrowsError()
    {
      // Arrange
      var mockLog = mockRepository.Create<ILog>();

      var logger = new BuildEngineCompilerLogger(mockLog.Object);

      // Act + Assert
      Assert.Throws<Exception>(() => logger.BuildFinished(new DotNetCompilerBuildFinished(new CsProjectBuilder().Build(), BuildStatus.Failed)));
    }
  }
}
