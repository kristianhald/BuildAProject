using System;
using System.IO;
using BuildAProject.Console.Configuration;
using BuildAProject.Console.Test.TestSupport;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.Console.Test.Configuration
{
  [TestFixture]
  public sealed class ConsoleConfigurationTests
  {
    [Test]
    public void ConsoleConfiguration_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture().Customize(new AutoMoqCustomization());
      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(ConsoleConfiguration).GetConstructors());
    }

    #region Parse

    [Test]
    public void Parse_WhenProvidedWithUnknownParameters_ReturnsFalse()
    {
      // Arrange
      const string arguments = "--unknown";

      var configuration = new ConsoleConfiguration();

      // Act
      var actualValue = configuration.Parse(arguments.Split(' '));

      // Assert
      Assert.IsFalse(actualValue);
    }

    [Test]
    public void Parse_WhenArgumentContainsHelp_PrintsToConsoleAndReturnsFalse()
    {
      // Arrange
      const string arguments = "-h";

      var configuration = new ConsoleConfiguration();

      // Act
      var actualValue = configuration.Parse(arguments.Split(' '));

      // Assert
      Assert.IsFalse(actualValue);
    }

    #endregion


    #region LogLevel

    [Test]
    public void LogLevel_WhenCalledBeforeParse_ThrowsError()
    {
      // Arrange
      var configuration = new ConsoleConfiguration();

      // Act + Assert
      Assert.Throws<Exception>(() => { var notUsed = configuration.LogLevel; });
    }

    [Test]
    public void LogLevel_WithShortFormArgument_ThenLogLevelInConfigurationIsSet()
    {
      // Arrange
      const int expectedLogLevel = 1;

      var arguments = new ConsoleConfigurationArgumentsBuilder
                      {
                        LogLevelShortFormValue = expectedLogLevel.ToString()
                      }.Build();

      var configuration = new ConsoleConfiguration();

      // ACt
      configuration.Parse(arguments.Split(' '));
      var actualLogLevel = configuration.LogLevel;

      // Assert
      Assert.AreEqual(expectedLogLevel, actualLogLevel);
    }

    [Test]
    public void LogLevel_WithLongFormArgument_ThenLogLevelInConfigurationIsSet()
    {
      // Arrange
      const int expectedLogLevel = 1;

      var arguments = new ConsoleConfigurationArgumentsBuilder
                      {
                        LogLevelLongFormValue = expectedLogLevel.ToString()
                      }.Build();

      var configuration = new ConsoleConfiguration();

      // Act
      configuration.Parse(arguments.Split(' '));
      var actualLogLevel = configuration.LogLevel;

      // Assert
      Assert.AreEqual(expectedLogLevel, actualLogLevel);
    }

    #endregion


    #region BuildPath

    [Test]
    public void BuildPath_WhenCalledBeforeParse_ThrowsError()
    {
      // Arrange
      var configuration = new ConsoleConfiguration();

      // Act + Assert
      Assert.Throws<Exception>(() => { var notUsed = configuration.BuildPath; });
    }

    [Test]
    public void BuildPath_WithShortFormArgument_ThenBuildPathInConfigurationIsSet()
    {
      // Arrange
      const string expectedBuildPath = "SomeBuildPath";
      var arguments = new ConsoleConfigurationArgumentsBuilder
                      {
                        BuildPathShortFormValue = expectedBuildPath
                      }.Build();

      var configuration = new ConsoleConfiguration();

      // Act
      configuration.Parse(arguments.Split(' '));
      var actualBuildPath = configuration.BuildPath;

      // Assert
      Assert.AreEqual(expectedBuildPath, actualBuildPath);
    }

    [Test]
    public void BuildPath_WithLongFormArgument_ThenBuildPathInConfigurationIsSet()
    {
      // Arrange
      const string expectedBuildPath = "SomeBuildPath";
      var arguments = new ConsoleConfigurationArgumentsBuilder
                      {
                        BuildPathLongFormValue = expectedBuildPath
                      }.Build();

      var configuration = new ConsoleConfiguration();

      // Act
      configuration.Parse(arguments.Split(' '));
      var actualBuildPath = configuration.BuildPath;

      // Assert
      Assert.AreEqual(expectedBuildPath, actualBuildPath);
    }

    #endregion


    #region CompilationBasePath

    [Test]
    public void CompilationBasePath_WhenCalledBeforeParse_ThrowsError()
    {
      // Arrange
      var configuration = new ConsoleConfiguration();

      // Act + Assert
      Assert.Throws<Exception>(() => { var notUsed = configuration.CompilationBasePath; });
    }

    [Test]
    public void CompilationBasePath_WithShortFormArgument_ThenCompilationBasePathInConfigurationIsSet()
    {
      // Arrange
      const string compilationBasePath = "SomeCompilationBasePath";
      var expectedCompilationBasePath = Path.GetFullPath(compilationBasePath);

      var arguments = new ConsoleConfigurationArgumentsBuilder
                      {
                        CompilationBasePathShortFormValue = compilationBasePath
                      }.Build();

      var configuration = new ConsoleConfiguration();

      // Act
      configuration.Parse(arguments.Split(' '));
      var actualCompilationBasePath = configuration.CompilationBasePath;

      // Assert
      Assert.AreEqual(expectedCompilationBasePath, actualCompilationBasePath);
    }

    [Test]
    public void CompilationBasePath_WithLongFormArgument_ThenCompilationBasePathInConfigurationIsSet()
    {
      // Arrange
      const string compilationBasePath = "SomeCompilationBasePath";
      var expectedCompilationBasePath = Path.GetFullPath(compilationBasePath);

      var arguments = new ConsoleConfigurationArgumentsBuilder
                      {
                        CompilationBasePathLongFormValue = compilationBasePath
                      }.Build();

      var configuration = new ConsoleConfiguration();

      // Act
      configuration.Parse(arguments.Split(' '));
      var actualCompilationBasePath = configuration.CompilationBasePath;

      // Assert
      Assert.AreEqual(expectedCompilationBasePath, actualCompilationBasePath);
    }

    [Test]
    public void CompilationBasePath_WhenArgumentIsMissing_ReturnsFalse()
    {
      // Arrange
      const string arguments = "-l 2";

      var configuration = new ConsoleConfiguration();

      // Act
      var actualValue = configuration.Parse(arguments.Split(' '));

      // Assert
      Assert.IsFalse(actualValue);
    }

    #endregion
  }
}
