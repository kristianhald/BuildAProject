using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildAProject.BuildManagement.CsProjects;
using BuildAProject.BuildManagement.CsProjects.Compilers;
using BuildAProject.BuildManagement.CsProjects.Compilers.Logging;
using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.Test.TestSupport;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using BuildAProject.BuildManagement.Test.TestSupport.FileSystems;
using BuildAProject.BuildManagement.Test.TestSupport.Settings;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.CsProjects.Compilers
{
  [TestFixture]
  [Category("IntegrationTest")]
  public sealed class BuildEngineCompilerTests
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
    public void BuildEngineCompiler_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(BuildEngineCompiler).GetConstructors());
    }

    [Test]
    public void Build_CSharpProject_CompiledIntoGeneralPath()
    {
      // Arrange
      var outputDirectory = TestSettings.GetTempPath(this);
      TestSettings.SetupTestEnvironmentFromZipFile(testDirectory.Value, TestFileResourceProvider.ProjectLibraryWithNoDependencies);

      var parameters = new BuildEngineParameters
                       {
                         GeneralOutputDirectory = testDirectory.Value
                       };

      var compiler = new BuildEngineCompilerBuilder
                     {
                       Parameters = parameters
                     }.Build();
      var projectPath = Path.Combine(testDirectory.Value, "ProjectLibraryWithNoDependencies.csproj");

      // Act
      string projectName;
      using (var projectStream = File.OpenRead(projectPath))
      {
        var project = new CsProject(projectStream, projectPath, new EmbeddedResourceFileSystem(new TestFileResourceProvider()));
        projectName = project.Name;

        compiler.Build(project);
      }

      // Assert
      AssertBuild(parameters.GeneralOutputDirectory, projectName + ".dll");
    }

    [Test]
    public void Build_LibraryCSharpProject_ToLibraryOutputPath()
    {
      // Arrange
      var outputDirectory = TestSettings.GetTempPath(this);
      TestSettings.SetupTestEnvironmentFromZipFile(testDirectory.Value, TestFileResourceProvider.ProjectLibraryWithNoDependencies);

      var parameters = new BuildEngineParameters
      {
        GeneralOutputDirectory = testDirectory.Value,
        LibraryOutputDirectory = Path.Combine(testDirectory.Value, "TestLibrary")
      };

      var compiler = new BuildEngineCompilerBuilder { Parameters = parameters }.Build();
      var projectPath = Path.Combine(testDirectory.Value, "ProjectLibraryWithNoDependencies.csproj");

      // Act
      string projectName;
      using (var projectStream = File.OpenRead(projectPath))
      {
        var project = new CsProject(projectStream, projectPath, new EmbeddedResourceFileSystem(new TestFileResourceProvider()));
        projectName = project.Name;

        compiler.Build(project);
      }

      // Assert
      AssertBuild(parameters.LibraryOutputDirectory, projectName + ".dll");
    }

    [Test]
    public void Build_ExecutableCSharpProject_ToSharedExecutableOutputPath()
    {
      // Arrange
      var outputDirectory = TestSettings.GetTempPath(this);
      TestSettings.SetupTestEnvironmentFromZipFile(testDirectory.Value, TestFileResourceProvider.ProjectExecutableWithNoDependencies);

      var parameters = new BuildEngineParameters
      {
        GeneralOutputDirectory = testDirectory.Value,
        ExecutableOutputDirectory = Path.Combine(testDirectory.Value, "TestSharedExecutable")
      };

      var compiler = new BuildEngineCompilerBuilder { Parameters = parameters }.Build();
      var projectPath = Path.Combine(testDirectory.Value, "ProjectExecutableWithNoDependencies.csproj");

      // Act
      string projectName;
      using (var projectStream = File.OpenRead(projectPath))
      {
        var project = new CsProject(projectStream, projectPath, new LocalComputerFileSystem());
        projectName = project.Name;

        compiler.Build(project);
      }

      // Assert
      AssertBuild(parameters.ExecutableOutputDirectory, projectName + ".exe");
    }

    [Test]
    public void Build_ExecutableCSharpProject_ToProjectSpecificExecutableOutputPath()
    {
      // Arrange
      var outputDirectory = TestSettings.GetTempPath(this);
      TestSettings.SetupTestEnvironmentFromZipFile(testDirectory.Value, TestFileResourceProvider.ProjectExecutableWithNoDependencies);

      var parameters = new BuildEngineParameters
      {
        GeneralOutputDirectory = testDirectory.Value,
        ExecutableOutputDirectory = Path.Combine(testDirectory.Value, "Executable"),
        ExecutablesInSeparateDirectories = true,
        LibraryOutputDirectory = Path.Combine(testDirectory.Value, "Library"),
        LibrariesInSeparateDirectories = true
      };

      var stubLogger = new StubLogger();
      var compiler = new BuildEngineCompilerBuilder
                     {
                       Parameters = parameters,
                       Logger = stubLogger
                     }.Build();
      var projectPath = Path.Combine(testDirectory.Value, @"ProjectExecutableWithNoDependencies.csproj");

      // Act
      string projectName;
      using (var projectStream = File.OpenRead(projectPath))
      {
        var project = new CsProject(projectStream, projectPath, new LocalComputerFileSystem());
        projectName = project.Name;

        compiler.Build(project);
      }

      // Assert
      AssertBuild(Path.Combine(parameters.ExecutableOutputDirectory, projectName), projectName + ".exe", stubLogger);
    }

    [Test]
    public void Build_CSharpProjectCannotCompile_ErrorIsProvidedToTheLogger()
    {
      // Arrange
      using (var outputDirectory = TestSettings.GetTempPath(this))
      {
        TestSettings.SetupTestEnvironmentFromZipFile(outputDirectory.Value, TestFileResourceProvider.CompileErrorProject);

        var parameters = new BuildEngineParameters
                         {
                           GeneralOutputDirectory = outputDirectory.Value
                         };

        var mockLogger = mockRepository.Create<IDotNetCompilerLogger>();
        var compiler = new BuildEngineCompilerBuilder
                       {
                         Parameters = parameters,
                         Logger = mockLogger.Object
                       }.Build();

        var projectPath = Path.Combine(outputDirectory.Value, "CompileErrorProject", "CompileErrorProject.csproj");

        DotNetCompilerBuildError expectedBuildError;
        using (var projectStream = File.OpenRead(projectPath))
        {
          var project = new CsProject(projectStream, projectPath, new LocalComputerFileSystem());

          expectedBuildError = new DotNetCompilerBuildError(project, "; expected");

          // Act
          compiler.Build(project);
        }

        // Assert
        mockLogger
          .Verify(logger => logger.ErrorRaised(expectedBuildError));
      }
    }

    [Test]
    public void Build_WhenCompilerBuilds_ThenLoggerIsProvidedWithLoggingInformation()
    {
      // Arrange
      using (var outputDirectory = TestSettings.GetTempPath(this))
      {
        TestSettings.SetupTestEnvironmentFromZipFile(
          outputDirectory.Value,
          TestFileResourceProvider.ProjectLibraryWithNoDependencies);

        var mockLogger = mockRepository.Create<IDotNetCompilerLogger>();
        var compiler = new BuildEngineCompilerBuilder
                       {
                         Logger = mockLogger.Object,
                         Parameters = new BuildEngineParameters
                                      {
                                        GeneralOutputDirectory = outputDirectory.Value
                                      }
                       }.Build();
        var projectPath = Path.Combine(outputDirectory.Value, @"ProjectLibraryWithNoDependencies.csproj");

        DotNetCompilerBuildStarted expectedBuildStart;
        DotNetCompilerBuildFinished expectedBuildFinished;
        using (var projectStream = File.OpenRead(projectPath))
        {
          var project = new CsProject(projectStream, projectPath,
            new EmbeddedResourceFileSystem(new TestFileResourceProvider()));
          expectedBuildStart = new DotNetCompilerBuildStarted(project);
          expectedBuildFinished = new DotNetCompilerBuildFinished(project, BuildStatus.Success);

          // Act
          compiler.Build(project);
        }

        // Assert
        mockLogger
          .Verify(logger => logger.BuildStarted(expectedBuildStart), Times.Once);
        mockLogger
          .Verify(logger => logger.BuildFinished(expectedBuildFinished), Times.Once);
      }
    }

    private void AssertBuild(string outputDirectory, string projectName, StubLogger stubLogger = null)
    {
      var additionErrorInformation = "";
      if (stubLogger != null && stubLogger.BuildErrors.Any())
      {
        additionErrorInformation = String.Format(
          "{0}Build errors encountered:{0}{1}",
          Environment.NewLine,
          String.Join(Environment.NewLine, stubLogger.BuildErrors.Select(error => error.Message)));
      }

      Assert.IsTrue(Directory.Exists(outputDirectory), "The output directory {0} for the test compilation does not exist. The compilation did not complete as expected. {1}", outputDirectory, additionErrorInformation);

      var fileCompilation = Path.Combine(outputDirectory, projectName);
      Assert.IsTrue(File.Exists(fileCompilation), "The compiled file could not be found at the expected location {0}. The compilation did not complete as expected. {1}", fileCompilation, additionErrorInformation);
    }

    private sealed class StubLogger : IDotNetCompilerLogger
    {
      private readonly List<DotNetCompilerBuildError> buildErrors = new List<DotNetCompilerBuildError>();

      public IEnumerable<DotNetCompilerBuildError> BuildErrors { get { return buildErrors; } }

      public void ErrorRaised(DotNetCompilerBuildError buildError)
      {
        buildErrors.Add(buildError);
      }

      public void BuildStarted(DotNetCompilerBuildStarted buildStarted)
      {
      }

      public void BuildFinished(DotNetCompilerBuildFinished buildFinished)
      {
      }
    }
  }
}
