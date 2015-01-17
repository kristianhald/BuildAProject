using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildAProject.BuildManagement.BuildManagers;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskExecutors;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers.Dependencies;
using BuildAProject.BuildManagement.CsProjects.Compilers;
using BuildAProject.BuildManagement.CsProjects.Compilers.Logging;
using BuildAProject.BuildManagement.CsProjects.SearchCriteria;
using BuildAProject.BuildManagement.CsProjects.TaskProviders;
using BuildAProject.BuildManagement.DLLs.SearchCriteria;
using BuildAProject.BuildManagement.DLLs.TaskProviders;
using BuildAProject.BuildManagement.Locators;
using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.Locators.SearchCriteria.Templates;
using BuildAProject.BuildManagement.NuGet.Configurations;
using BuildAProject.BuildManagement.NuGet.Downloaders;
using BuildAProject.BuildManagement.NuGet.SearchCriteria;
using BuildAProject.BuildManagement.NuGet.TaskProviders;
using BuildAProject.BuildManagement.NUnit.Runners;
using BuildAProject.BuildManagement.NUnit.TaskProviders;
using BuildAProject.BuildManagement.Test.TestSupport;
using BuildAProject.BuildManagement.Test.TestSupport.Settings;
using BuildAProject.BuildManagement.Test.TestSupport.Tools;
using NuGet;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test
{
  [TestFixture]
  [Category("IntegrationTest")]
  public sealed class BuildManagementIntegrationTests
  {
    private static readonly string TestPath = TestSettings.GetTempPath(typeof(BuildManagementIntegrationTests)).Value;

    [TestFixtureTearDown]
    public void TearDown()
    {
      Directory.Delete(TestPath, true);
    }

    [Test]
    [TestCaseSource("BuildManagementTestCaseSource")]
    public void PrimaryFlowTest(BuildManagementTestCaseContainer testContainer)
    {
      // Arrange
      SetupBuildEnvironment(testContainer);

      var fileSystem = new IgnoreFoldersFileSystem(new LocalComputerFileSystem());

      // This is the presumed flow for the build management module

      // 1. Find projects via the project locator module
      var fileCriterias = new FileNameCriteria[]
                          {
                            new CsProjectFileCriteria(fileSystem), 
                            new NuGetPackageFileCriteria(fileSystem),
                            new DllFileCriteria(), 
                          };
      var projectLocator = new CriteriaProjectsLocator(fileSystem, fileCriterias);

      // 2. Build the dependency graph of the projects
      // 3. Fail if not all dependencies are in the graph (This must be overrideable somehow and it must be the manager that decides if this should happen)
      // 4. Provide the dependency graph to build task providers that return tasks in the phases they should be executed
      var dotNetCompilerLogger = new StubBuildEngineCompilerLogger();
      var dotNetCompiler = new BuildEngineCompiler(testContainer.BuildParameters, dotNetCompilerLogger);

      var nuGetDownloader = new RepositoryNuGetDownloader(
        new NuGetPackageScannerRepositoryFactory(new PackageRepositoryFactory()),
        new HierachicalNuGetConfigFileReader());

      var nUnitLogger = new StubNUnitLogger();
      var testRunner = new NUnitFileTestRunner(nUnitLogger);

      var taskProviders = new IBuildTaskProvider[]
                          {
                            new CsCompileTaskProvider(new CsCompileTaskFactory(dotNetCompiler)),
                            new NuGetPackageDownloadTaskProvider(new NuGetPackageDownloadTaskFactory(nuGetDownloader)),
                            new DllFileTaskProvider(new DllFileTaskFactory()), 
                            new NUnitTestTaskProvider(new NUnitTestTaskFactory(testRunner, testContainer.BuildParameters)), 
                          };

      var buildTaskManager = new BuildTaskProviderManager(taskProviders);

      var dependencyAlgorithm = new UnhandledTaskManager(new SimpleDependencyAlgorithm());

      // 5. Iterate the phases and provide the tasks to the task executor
      // 6. Execute each task
      var buildTaskExecutor = new SingleThreadedBuildTaskExecutor();

      // Combine the dependency objects
      var buildManager = new BuildAllTasksManager(projectLocator, buildTaskManager, buildTaskExecutor, dependencyAlgorithm);

      // Act
      buildManager.Build(testContainer.TestDirectory);

      // Assert
      AssertDotNetCompilerLog(dotNetCompilerLogger);

      Assert.AreEqual(
        0,
        dependencyAlgorithm.LastUnhandledTasksEncountered.Count(),
        "There are tasks that weren't executed, because of lacking dependencies. These tasks are:{0}{1}",
        Environment.NewLine,
        String.Join(Environment.NewLine, dependencyAlgorithm.LastUnhandledTasksEncountered.Select(task => task.Name)));

      AssertNUnitLog(nUnitLogger);

      AssertProjectWasCompiledAsLibrary("DependencyLibrary", testContainer);
      AssertProjectWasCompiledAsExecutable("ApplicationConsole", testContainer);
      AssertProjectWasCompiledAsLibrary("Application.Test", testContainer);
      AssertProjectWasNotCompiledAsLibrary("IgnoredLibrary", testContainer);
    }

    private static void AssertDotNetCompilerLog(StubBuildEngineCompilerLogger dotNetCompilerLogger)
    {
      Assert.AreEqual(
        4,
        dotNetCompilerLogger.BuildStarts.Count(),
        Environment.NewLine,
        String.Join(
          Environment.NewLine,
          dotNetCompilerLogger.BuildStarts.Select(start => start.ProjectName)));

      Assert.AreEqual(
        0,
        dotNetCompilerLogger.BuildErrors.Count(),
        "Build errors occurred. The error messages encountered are: {0}{1}",
        Environment.NewLine,
        String.Join(
          Environment.NewLine,
          dotNetCompilerLogger.BuildErrors.Select(error => error.ProjectName + ": " + error.Message)));

      Assert.AreEqual(
        4,
        dotNetCompilerLogger.BuildFinishes.Count(),
        Environment.NewLine,
        String.Join(
          Environment.NewLine,
          dotNetCompilerLogger.BuildFinishes.Select(finish => finish.ProjectName + ": " + finish.Status)));
    }

    private static void AssertNUnitLog(StubNUnitLogger nUnitLogger)
    {
      Assert.AreEqual(
        4,
        nUnitLogger.Results.Count(),
        "The number of expected test results did not match the actual number of test results logged. There should be one test result per project compiled even though there might not be any tests in the project. The results are:{0}{1}",
        Environment.NewLine,
        String.Join(Environment.NewLine,
          nUnitLogger.Results.Select(
            result =>
              String.Format("{0}:{1}{2}", result.TestFilePath, Environment.NewLine,
                String.Join(Environment.NewLine, result.MethodResults.Select(methodResult => methodResult.ToString()))))));

      var failedTests = nUnitLogger
        .Results
        .Where(result => result.MethodResults.Any(method => method.Status != NUnitStatus.Success))
        .ToArray();
      CollectionAssert.AreEquivalent(new NUnitExecutionResult[0], failedTests, "The following NUnit tests failed, which should not have failed.");
    }

    private IEnumerable<BuildManagementTestCaseContainer> BuildManagementTestCaseSource()
    {
      var testBuildParametersList = new List<BuildManagementTestCaseContainer>();

      foreach (var buildTaskExecutor in new IBuildTaskExecutor[] { new SingleThreadedBuildTaskExecutor(), new MultiThreadedBuildTaskExecutor() })
      {
        // Parameters putting compiled files in the general directory no matter the project type
        var inGeneralPath = Path.Combine(TestPath, buildTaskExecutor.GetType().Name, "InGeneralPath");
        testBuildParametersList.Add(
          new BuildManagementTestCaseContainer(
            inGeneralPath,
            buildTaskExecutor,
            new BuildEngineParameters
            {
              GeneralOutputDirectory = inGeneralPath
            },
            "Tests that when compiling all files into a single directory that all links are correct."));

        // Parameters putting compiled files in directories in accordance with the project types
        var byProjectTypePath = Path.Combine(TestPath, buildTaskExecutor.GetType().Name, "ByProjectType");
        testBuildParametersList.Add(
          new BuildManagementTestCaseContainer(
            byProjectTypePath,
            buildTaskExecutor,
            new BuildEngineParameters
            {
              GeneralOutputDirectory = byProjectTypePath,
              LibraryOutputDirectory = Path.Combine(byProjectTypePath, "Libraries"),
              ExecutableOutputDirectory = Path.Combine(byProjectTypePath, "Executables")
            },
            "Tests that when projects are compiled into project type directories that all links are correct."));

        // Parameters putting compiled executable files into separate directories
        var separateExecutablesPath = Path.Combine(TestPath, buildTaskExecutor.GetType().Name, "SeparateExecutables");
        testBuildParametersList.Add(
          new BuildManagementTestCaseContainer(
            separateExecutablesPath,
            buildTaskExecutor,
            new BuildEngineParameters
            {
              GeneralOutputDirectory = separateExecutablesPath,
              LibraryOutputDirectory = Path.Combine(separateExecutablesPath, "Libraries"),
              ExecutableOutputDirectory = Path.Combine(separateExecutablesPath, "Executables"),
              ExecutablesInSeparateDirectories = true
            },
            "Tests that when projects are compiled into project type directories and executables into their own directories that all links are correct."));

        // Parameters putting compiled library files into separate directories
        var separateLibrariesPath = Path.Combine(TestPath, buildTaskExecutor.GetType().Name, "SeparateLibraries");
        testBuildParametersList.Add(
          new BuildManagementTestCaseContainer(
            separateLibrariesPath,
            buildTaskExecutor,
            new BuildEngineParameters
            {
              GeneralOutputDirectory = separateLibrariesPath,
              LibraryOutputDirectory = Path.Combine(separateLibrariesPath, "Libraries"),
              LibrariesInSeparateDirectories = true,
              ExecutableOutputDirectory = Path.Combine(separateLibrariesPath, "Executables")
            },
            "Tests that when projects are compiled into project type directories and libraries into their own directories that all links are correct."));
      }

      return testBuildParametersList;
    }

    /// <summary>
    /// Sets up the build environment for the integration test
    /// </summary>
    private void SetupBuildEnvironment(BuildManagementTestCaseContainer testContainer)
    {
      Directory.CreateDirectory(testContainer.TestDirectory);

      // Unzips an embedded resource that contains the projects to be compiled
      // The Nu-Get packages will be copied as embedded resources to the correct
      // library folder. The Test should not try to download the files
      using (
        var projectsZipFileStream =
          TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.IntegrationTestBuildProjects))
      {
        UnzipTool.UnzipStream(projectsZipFileStream, testContainer.TestDirectory);
      }

      const string nugetConfig = "<?xml version=\"1.0\" encoding=\"utf-8\"?><configuration><config><add key=\"repositorypath\" value=\"{1}\" /></config><packageRestore><add key=\"enabled\" value=\"True\" /><add key=\"automatic\" value=\"True\" /></packageRestore><packageSources><clear /><add key=\"local\" value=\"{0}\" /></packageSources><disabledPackageSources><clear /></disabledPackageSources><activePackageSource><clear /><add key=\"local\" value=\"{0}\" /></activePackageSource></configuration>";
      File.WriteAllText(
        Path.Combine(testContainer.TestDirectory, HierachicalNuGetConfigFileReader.NuGetConfigFileName),
        String.Format(nugetConfig, testContainer.NuGetPackageSourceDirectory, testContainer.NuGetPackageRepositoryDirectory));
    }

    /// <summary>
    /// Asserts that the project was compiled as a library and has been placed in the correct directory
    /// </summary>
    private void AssertProjectWasCompiledAsLibrary(string project, BuildManagementTestCaseContainer testContainer)
    {
      var libraryDirectory = testContainer.BuildParameters.LibraryOutputDirectory;
      if (testContainer.BuildParameters.LibrariesInSeparateDirectories)
        libraryDirectory = Path.Combine(libraryDirectory, project);

      AssertProjectWasCompiled(project + ".dll", libraryDirectory);
    }

    /// <summary>
    /// Asserts that the project was not compiled as a library
    /// </summary>
    private void AssertProjectWasNotCompiledAsLibrary(string project, BuildManagementTestCaseContainer testContainer)
    {
      AssertProjectWasNotCompiled(project + ".dll", testContainer.BuildParameters.LibraryOutputDirectory);
    }

    /// <summary>
    /// Asserts that the project was compiled as an executable and has been placed in the correct directory
    /// </summary>
    private void AssertProjectWasCompiledAsExecutable(string project, BuildManagementTestCaseContainer testContainer)
    {
      var executableDirectory = testContainer.BuildParameters.ExecutableOutputDirectory;
      if (testContainer.BuildParameters.ExecutablesInSeparateDirectories)
        executableDirectory = Path.Combine(executableDirectory, project);

      AssertProjectWasCompiled(project + ".exe", executableDirectory);
    }

    /// <summary>
    /// Asserts that the project was compiled and the dll belonging to it exists
    /// </summary>
    private void AssertProjectWasCompiled(string project, string projectFilePath)
    {
      var fileCompilation = Path.Combine(projectFilePath, project);
      Assert.IsTrue(File.Exists(fileCompilation), "The compiled file could not be found at the expected location {0}. The compilation did not complete as expected.", fileCompilation);
    }

    /// <summary>
    /// Asserts that the project was not compiled and that the file does not exist
    /// </summary>
    private void AssertProjectWasNotCompiled(string project, string projectFilePath)
    {
      var fileCompilation = Path.Combine(projectFilePath, project);
      Assert.IsFalse(File.Exists(fileCompilation), "The compiled file was found at the location {0}. The compilation was done even though it should not have been executed.", fileCompilation);
    }

    public class BuildManagementTestCaseContainer
    {
      private readonly string description;

      public BuildManagementTestCaseContainer(
        string testDirectory,
        IBuildTaskExecutor buildTaskExecutor,
        BuildEngineParameters buildParameters,
        string description)
      {
        if (String.IsNullOrWhiteSpace(testDirectory))
        {
          throw new ArgumentNullException("testDirectory");
        }

        if (buildTaskExecutor == null)
        {
          throw new ArgumentNullException("buildTaskExecutor");
        }

        if (buildParameters == null)
        {
          throw new ArgumentNullException("buildParameters");
        }

        if (String.IsNullOrWhiteSpace(description))
        {
          throw new ArgumentNullException("description");
        }

        TestDirectory = testDirectory;
        BuildParameters = buildParameters;
        BuildTaskExecutor = buildTaskExecutor;

        this.description = description;
      }

      /// <summary>
      /// The directory used to run the test
      /// </summary>
      public string TestDirectory { get; private set; }

      /// <summary>
      /// The directory where the packages are stored
      /// </summary>
      public string NuGetPackageSourceDirectory { get { return Path.Combine(TestDirectory, "NuGetLocalRepository"); } }

      /// <summary>
      /// The directory were the packages are downloaded to from the repository
      /// </summary>
      public string NuGetPackageRepositoryDirectory { get { return Path.Combine(TestDirectory, "packages"); } }

      /// <summary>
      /// The build parameters used for testing
      /// </summary>
      public BuildEngineParameters BuildParameters { get; private set; }

      /// <summary>
      /// The build executor to use with the tests
      /// </summary>
      public IBuildTaskExecutor BuildTaskExecutor { get; private set; }

      /// <summary>
      /// The description of the contents of the container
      /// </summary>
      public override string ToString()
      {
        return description;
      }
    }

    private sealed class UnhandledTaskManager : IDependencyAlgorithm
    {
      private readonly IDependencyAlgorithm composite;

      public UnhandledTaskManager(IDependencyAlgorithm composite)
      {
        this.composite = composite;
      }

      public IEnumerable<IBuildTask> LastUnhandledTasksEncountered { get; private set; }

      public BuildTaskPhaseCollection OrderTasksByPhase(IEnumerable<IBuildTask> tasks)
      {
        var phasedTasks = composite.OrderTasksByPhase(tasks);

        LastUnhandledTasksEncountered = phasedTasks.UnhandledTasks.Where(task => !task.Name.Contains("CSharp.01.HelloWorld"));

        return phasedTasks;
      }
    }

    private sealed class StubBuildEngineCompilerLogger : IDotNetCompilerLogger
    {
      private readonly List<DotNetCompilerBuildStarted> buildStarts = new List<DotNetCompilerBuildStarted>();
      private readonly List<DotNetCompilerBuildError> buildErrors = new List<DotNetCompilerBuildError>();
      private readonly List<DotNetCompilerBuildFinished> buildFinishes = new List<DotNetCompilerBuildFinished>();

      public IEnumerable<DotNetCompilerBuildStarted> BuildStarts { get { return buildStarts; } }

      public IEnumerable<DotNetCompilerBuildError> BuildErrors { get { return buildErrors; } }

      public IEnumerable<DotNetCompilerBuildFinished> BuildFinishes { get { return buildFinishes; } }

      public void ErrorRaised(DotNetCompilerBuildError buildError)
      {
        buildErrors.Add(buildError);
      }

      public void BuildStarted(DotNetCompilerBuildStarted buildStarted)
      {
        buildStarts.Add(buildStarted);
      }

      public void BuildFinished(DotNetCompilerBuildFinished buildFinished)
      {
        buildFinishes.Add(buildFinished);
      }
    }

    private sealed class StubNUnitLogger : INUnitLogger
    {
      private readonly List<NUnitExecutionResult> unitTestResults = new List<NUnitExecutionResult>();

      public IEnumerable<NUnitExecutionResult> Results { get { return unitTestResults; } }

      public void TestResult(NUnitExecutionResult result)
      {
        unitTestResults.Add(result);
      }
    }
  }
}
