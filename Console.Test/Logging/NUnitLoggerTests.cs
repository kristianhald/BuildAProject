using System;
using BuildAProject.BuildManagement.NUnit.Runners;
using BuildAProject.Console.Logging;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.Console.Test.Logging
{
  [TestFixture]
  public sealed class NUnitLoggerTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void NUnitLogger_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture().Customize(new AutoMoqCustomization());
      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(NUnitLogger).GetConstructors());
    }

    [Test]
    public void TestResult_ResultParameterIsNull_ThrowsError()
    {
      // Arrange
      var fakeLog = mockRepository.Create<ILog>();
      var logger = new NUnitLogger(fakeLog.Object);

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => logger.TestResult(null));
    }

    [Test]
    public void TestResult_WithSomeFailures_AreEachSentToErrorLog()
    {
      // Arrange
      var mockLog = mockRepository.Create<ILog>();

      var logger = new NUnitLogger(mockLog.Object);

      var result = new NUnitExecutionResult(
        "SomeFilePath",
        new[]
        {
          new NUnitTestMethodResult("MethodOne", NUnitStatus.Failed, "MethodOneFailed"), 
          new NUnitTestMethodResult("MethodTwo", NUnitStatus.Failed, "MethodTwoFailed"),
          new NUnitTestMethodResult("MethodThree", NUnitStatus.Success, "")
        });

      // Act
      logger.TestResult(result);

      // Assert
      mockLog
        .Verify(log => log.Error(It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(5));
    }

    [Test]
    public void TestResult_WhenCalledWithoutTestError_ThenCallsInformationMethodOnLogToTellTestHasExecuted()
    {
      // Arrange
      var mockLog = mockRepository.Create<ILog>();

      var logger = new NUnitLogger(mockLog.Object);

      var result = new NUnitExecutionResult(
        "SomeFilePath",
        new[]
        {
          new NUnitTestMethodResult("MethodThree", NUnitStatus.Success, "")
        });

      // Act
      logger.TestResult(result);

      // Assert
      mockLog
        .Verify(log => log.Information(It.IsAny<string>(), It.IsAny<int>()));
    }
  }
}
