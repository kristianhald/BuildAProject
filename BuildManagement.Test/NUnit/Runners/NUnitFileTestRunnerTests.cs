using System;
using System.IO;
using BuildAProject.BuildManagement.NUnit.Runners;
using BuildAProject.BuildManagement.Test.TestSupport;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using BuildAProject.BuildManagement.Test.TestSupport.Settings;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.NUnit.Runners
{
  [TestFixture]
  public sealed class NUnitFileTestRunnerTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);
    private TestDirectory testDirectory;

    [SetUp]
    public void SetUp()
    {
      testDirectory = TestSettings.GetTempPath(this);
    }

    [TearDown]
    public void TearDown()
    {
      testDirectory.Dispose();
    }

    [Test]
    public void NUnitCsProjectTestRunner_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(NUnitFileTestRunner).GetConstructors());
    }

    [Test]
    public void Test_TestDllFilePathParameterIsNull_ThrowsError()
    {
      // Arrange
      var runner = new NUnitCsProjectTestRunnerBuilder().Build();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => runner.Test(null));
    }

    [Test]
    public void Test_TestDllFilePathParameterDoesNotExist_DoesNotFail()
    {
      // Arrange
      var runner = new NUnitCsProjectTestRunnerBuilder().Build();

      // Act + Assert
      Assert.DoesNotThrow(() => runner.Test(@"x:\this\file\does\not\exist.dll"));
    }

    [Test]
    public void Test_ProvidedDllContainsTests_WhichAreExecutedWithoutError()
    {
      // Test Constants
      const string testDllFileName = @"NUnitNoErrorsProject\NUnitNoErrorsProject.dll";

      // Arrange
      var testDirectory = TestSettings.GetTempPath(this);
      TestSettings.SetupTestEnvironmentFromZipFile(this.testDirectory.Value, TestFileResourceProvider.NUnitTestProject);

      var fullTestDllFilePath = Path.Combine(this.testDirectory.Value, testDllFileName);

      var expectedResult = new NUnitExecutionResult(
        fullTestDllFilePath,
        new[]
        {
          new NUnitTestMethodResult("NUnitNoErrorsProject.NoErrorTests.MethodReturnsOne_IsCalled_AndDoesNotReturnTwo", NUnitStatus.Success, ""),
          new NUnitTestMethodResult("NUnitNoErrorsProject.NoErrorTests.MethodReturnsOne_IsCalled_AndOneIsReturned", NUnitStatus.Success, "")
        });

      NUnitExecutionResult actualResult = null;
      var mockLogger = mockRepository.Create<INUnitLogger>();
      mockLogger
        .Setup(logger => logger.TestResult(It.IsAny<NUnitExecutionResult>()))
        .Callback<NUnitExecutionResult>(result => actualResult = result);

      var runner = new NUnitCsProjectTestRunnerBuilder
      {
        Logger = mockLogger.Object
      }.Build();

      // Act
      runner.Test(fullTestDllFilePath);

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void Test_ProvidedDllContainsTests_WhichAreExecutedWithError()
    {
      // Test Constants
      const string testDllFileName = @"NUnitContainsErrorsProject\NUnitContainsErrorsProject.dll";

      // Arrange
      var testDirectory = TestSettings.GetTempPath(this);
      TestSettings.SetupTestEnvironmentFromZipFile(this.testDirectory.Value, TestFileResourceProvider.NUnitTestProject);

      var fullTestDllFilePath = Path.Combine(this.testDirectory.Value, testDllFileName);

      var expectedResult = new NUnitExecutionResult(
        fullTestDllFilePath,
        new[]
        {
          new NUnitTestMethodResult("NUnitContainsErrorsProject.ContainsErrorTests.MethodReturnsOne_IsExpectedToReturnTwo_ButOneIsReturned", NUnitStatus.Failed, "  Expected: 2\r\n  But was:  1\r\n")
        });

      NUnitExecutionResult actualResult = null;
      var mockLogger = mockRepository.Create<INUnitLogger>();
      mockLogger
        .Setup(logger => logger.TestResult(It.IsAny<NUnitExecutionResult>()))
        .Callback<NUnitExecutionResult>(result => actualResult = result);

      var runner = new NUnitCsProjectTestRunnerBuilder
      {
        Logger = mockLogger.Object
      }.Build();

      // Act
      runner.Test(fullTestDllFilePath);

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void Test_ProvidedDllContainsNoTests_WhichLogsATestExecutionWithEmptyResultsList()
    {
      // Test Constants
      const string testDllFileName = @"NUnitProjectUnderTest\NUnitProjectUnderTest.dll";

      // Arrange
      var testDirectory = TestSettings.GetTempPath(this);
      TestSettings.SetupTestEnvironmentFromZipFile(this.testDirectory.Value, TestFileResourceProvider.NUnitTestProject);

      var fullTestDllFilePath = Path.Combine(this.testDirectory.Value, testDllFileName);

      var expectedResult = new NUnitExecutionResult(
        fullTestDllFilePath,
        new NUnitTestMethodResult[0]);

      NUnitExecutionResult actualResult = null;
      var mockLogger = mockRepository.Create<INUnitLogger>();
      mockLogger
        .Setup(logger => logger.TestResult(It.IsAny<NUnitExecutionResult>()))
        .Callback<NUnitExecutionResult>(result => actualResult = result);

      var runner = new NUnitCsProjectTestRunnerBuilder
      {
        Logger = mockLogger.Object
      }.Build();

      // Act
      runner.Test(fullTestDllFilePath);

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void Test_WhenExceptionIsThrownByRunner_ThenExceptionIsWrapperAndProvidedToLogger()
    {
      // Test Constants
      const string testDllFileName = @"NUnitProjectUnderTest\NUnitProjectUnderTest.dll";

      // Arrange
      var testDirectory = TestSettings.GetTempPath(this);
      TestSettings.SetupTestEnvironmentFromZipFile(this.testDirectory.Value, TestFileResourceProvider.NUnitTestProject);

      var fullTestDllFilePath = Path.Combine(this.testDirectory.Value, testDllFileName);

      NUnitExecutionResult actualResult = null;
      var mockLogger = mockRepository.Create<INUnitLogger>();
      mockLogger
        .Setup(logger => logger.TestResult(It.IsAny<NUnitExecutionResult>()))
        .Callback<NUnitExecutionResult>(result => actualResult = result);

      var exception = new Exception("An error occurred");
      mockLogger
        .Setup(logger => logger.TestResult(new NUnitExecutionResult(fullTestDllFilePath, new NUnitTestMethodResult[0])))
        .Throws(exception);

      var runner = new NUnitCsProjectTestRunnerBuilder
      {
        Logger = mockLogger.Object
      }.Build();

      // Act
      runner.Test(fullTestDllFilePath);

      // Assert
     var expectedResult = new NUnitExecutionResult(
        fullTestDllFilePath,
        new[]
        {
          new NUnitTestMethodResult("", NUnitStatus.Failed, exception.ToString()), 
        });

      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void Test_WhenCalledReadsTheAppConfigFileWithBindingRedirects_ThenDoesNotFailTheExecutionBecauseOfReferenceToOtherVersion()
    {
      // Test Constants
      const string testDllFileName = @"BindingRedirectProject.dll";

      // Arrange
      var testDirectory = TestSettings.GetTempPath(this);
      TestSettings.SetupTestEnvironmentFromZipFile(this.testDirectory.Value, TestFileResourceProvider.BindingRedirectTest);

      var fullTestDllFilePath = Path.Combine(this.testDirectory.Value, testDllFileName);

      var expectedResult = new NUnitExecutionResult(
        fullTestDllFilePath,
        new[]
        {
          new NUnitTestMethodResult("BindingRedirectProject.UsingDependency.Run", NUnitStatus.Success, "")
        });

      NUnitExecutionResult actualResult = null;
      var mockLogger = mockRepository.Create<INUnitLogger>();
      mockLogger
        .Setup(logger => logger.TestResult(It.IsAny<NUnitExecutionResult>()))
        .Callback<NUnitExecutionResult>(result => actualResult = result);

      var runner = new NUnitCsProjectTestRunnerBuilder
      {
        Logger = mockLogger.Object
      }.Build();

      // Act
      runner.Test(fullTestDllFilePath);

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void Test_WhenExceptionsThrownBySetUp_ThenSetUpErrorIsLogged()
    {
      // Test Constants
      const string testDllFileName = @"NUnitSetUpContainsErrorsProject\NUnitSetUpContainsErrorsProject.dll";

      // Arrange
      var testDirectory = TestSettings.GetTempPath(this);
      TestSettings.SetupTestEnvironmentFromZipFile(this.testDirectory.Value, TestFileResourceProvider.NUnitTestProject);

      var fullTestDllFilePath = Path.Combine(this.testDirectory.Value, testDllFileName);

      var expectedResult = new NUnitExecutionResult(
        fullTestDllFilePath,
        new[]
        {
          new NUnitTestMethodResult("NUnitSetUpContainsErrorsProject.ContainsErrorTests.MethodNotExecuted", NUnitStatus.Failed, "SetUp : System.Exception : Throws error making it possible to test if the NUnit runners handle this.\r\n  ----> System.Exception : With this inner exception.")
        });

      NUnitExecutionResult actualResult = null;
      var mockLogger = mockRepository.Create<INUnitLogger>();
      mockLogger
        .Setup(logger => logger.TestResult(It.IsAny<NUnitExecutionResult>()))
        .Callback<NUnitExecutionResult>(result => actualResult = result);

      var runner = new NUnitCsProjectTestRunnerBuilder
      {
        Logger = mockLogger.Object
      }.Build();

      // Act
      runner.Test(fullTestDllFilePath);

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }
  }
}
