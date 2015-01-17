using System;
using System.Linq;
using BuildAProject.BuildManagement.CsProjects.Compilers;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.CsProjects.Compilers
{
  [TestFixture]
  public sealed class BuildEngineParametersTests
  {
    #region AddTargetToBuild Tests

    [Test]
    public void AddTargetToBuild_NewTargetAdded_IsInTargetList()
    {
      // Arrange
      var expectedTargets = new[]
                            {
                              "SomeTarget"
                            };

      var parameters = new BuildEngineParameters();

      // Act
      parameters.AddTargetToBuild(expectedTargets.First());
      var actualTargets = parameters.TargetsToBuild;

      // Assert
      CollectionAssert.AreEquivalent(expectedTargets, actualTargets);
    }

    [Test]
    public void AddTargetToBuild_TargetToBuildParameterIsNull_ThrowsError()
    {
      // Arrange
      var parameters = new BuildEngineParameters();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => parameters.AddTargetToBuild(null));
    }

    #endregion

    #region RemoveTargetToBuild Tests

    [Test]
    public void RemoveTargetToBuild_TargetRemoved_IsNotInTargetList()
    {
      // Arrange
      var expectedTargets = new[]
                            {
                              "SomeTarget"
                            };

      var parameters = new BuildEngineParameters();
      parameters.AddTargetToBuild("SomeTarget");
      parameters.AddTargetToBuild("AnotherTarget");

      // Act
      parameters.RemoveTargetToBuild("AnotherTarget");
      var actualTargets = parameters.TargetsToBuild;

      // Assert
      CollectionAssert.AreEquivalent(expectedTargets, actualTargets);
    }

    [Test]
    public void RemoveTargetToBuild_TargetToBuildParameterIsNull_ThrowsError()
    {
      // Arrange
      var parameters = new BuildEngineParameters();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => parameters.RemoveTargetToBuild(null));
    }

    [Test]
    public void RemoveTargetToBuild_TargetNotInList_NothingHappens()
    {
      // Arrange
      var expectedTargets = new[]
                            {
                              "SomeTarget"
                            };
      var parameters = new BuildEngineParameters();
      parameters.AddTargetToBuild(expectedTargets.First());

      // Act
      parameters.RemoveTargetToBuild("Target Does Not Exist In List");
      var actualTargets = parameters.TargetsToBuild;

      // Assert
      CollectionAssert.AreEquivalent(expectedTargets, actualTargets);
    }

    #endregion

    #region ExecutableOutputDirectory Tests

    [Test]
    public void ExecutableOutputDirectory_PropertyIsSet_ValueReturned()
    {
      // Arrange
      const string expectedResult = "This value is expected";

      var parameters = new BuildEngineParameters
                      {
                        ExecutableOutputDirectory = expectedResult
                      };

      // Act
      var actualResult = parameters.ExecutableOutputDirectory;

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void ExecutableOutputDirectory_PropertyIsNotSet_ValueFromGeneralOutputDirectoryIsReturned()
    {
      // Arrange
      const string expectedResult = "This value is expected";

      var parameters = new BuildEngineParameters
                       {
                         GeneralOutputDirectory = expectedResult
                       };

      // Act
      var actualResult = parameters.ExecutableOutputDirectory;

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    #endregion

    #region LibraryOutputDirectory Tests

    [Test]
    public void LibraryOutputDirectory_PropertyIsSet_ValueReturned()
    {
      // Arrange
      const string expectedResult = "This value is expected";

      var parameters = new BuildEngineParameters
      {
        LibraryOutputDirectory = expectedResult
      };

      // Act
      var actualResult = parameters.LibraryOutputDirectory;

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void LibraryOutputDirectory_PropertyIsNotSet_ValueFromGeneralOutputDirectoryIsReturned()
    {
      // Arrange
      const string expectedResult = "This value is expected";

      var parameters = new BuildEngineParameters
      {
        GeneralOutputDirectory = expectedResult
      };

      // Act
      var actualResult = parameters.LibraryOutputDirectory;

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    #endregion

    #region GetProjectCompilationDirectory

    [Test]
    public void GetProjectCompilationDirectory_CsProjectParameterIsNull_ThrowsError()
    {
      // Arrange
      var parameters = new BuildEngineParameters();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => parameters.GetProjectCompilationDirectory(null));
    }

    [Test]
    public void GetProjectCompilationDirectory_CsProjectIsLibraryAndAllArePlacedInSameDirectory_ReturnsLibraryOutputPath()
    {
      // Arrange
      const string expectedPath = "LibraryOutputPath";

      var parameters = new BuildEngineParameters
      {
        LibraryOutputDirectory = expectedPath,
        LibrariesInSeparateDirectories = false
      };

      var csProject = new CsProjectBuilder
      {
        OutputType = "Library"
      }.Build();

      // Act
      var actualPath = parameters.GetProjectCompilationDirectory(csProject);

      // Assert
      Assert.AreEqual(expectedPath, actualPath);
    }

    [Test]
    public void GetProjectCompilationDirectory_CsProjectIsLibraryAndAllArePlacedInDifferentDirectories_ReturnsLibraryPathWithProjectName()
    {
      // Arrange
      const string path = "LibraryOutputPath";

      var parameters = new BuildEngineParameters
      {
        LibraryOutputDirectory = path,
        LibrariesInSeparateDirectories = true
      };

      var csProject = new CsProjectBuilder
      {
        OutputType = "Library"
      }.Build();

      // Act
      var actualPath = parameters.GetProjectCompilationDirectory(csProject);

      // Assert
      Assert.AreEqual(path + "\\" + csProject.Name, actualPath);
    }

    [Test]
    public void GetProjectCompilationDirectory_CsProjectIsExecutableAndAllArePlacedInSameDirectory_ReturnsSingleExecutablePath()
    {
      // Arrange
      const string expectedPath = "ExecutablePath";

      var parameters = new BuildEngineParameters
      {
        ExecutableOutputDirectory = expectedPath,
        ExecutablesInSeparateDirectories = false
      };

      var csProject = new CsProjectBuilder
      {
        OutputType = "Exe"
      }.Build();

      // Act
      var actualPath = parameters.GetProjectCompilationDirectory(csProject);

      // Assert
      Assert.AreEqual(expectedPath, actualPath);
    }

    [Test]
    public void GetProjectCompilationDirectory_CsProjectIsExecutableAndAllArePlacedInDifferentDirectories_ReturnsExecutablePathWithProjectName()
    {
      // Arrange
      const string csProjectName = "ProjectName";
      const string executablePath = "ExecutablePath";

      var parameters = new BuildEngineParameters
      {
        ExecutableOutputDirectory = executablePath,
        ExecutablesInSeparateDirectories = true
      };

      var csProject = new CsProjectBuilder
      {
        OutputType = "Exe",
        Name = csProjectName
      }.Build();

      // Act
      var actualPath = parameters.GetProjectCompilationDirectory(csProject);

      // Assert
      Assert.AreEqual(executablePath + "\\" + csProjectName, actualPath);
    }

    [Test]
    public void GetProjectCompilationDirectory_CsProjectIsUnknown_ReturnsGeneralOutputPath()
    {
      // Arrange
      const string expectedPath = "GeneralOutputPath";

      var parameters = new BuildEngineParameters
      {
        GeneralOutputDirectory = expectedPath
      };

      var csProject = new CsProjectBuilder
      {
        OutputType = "Unknown"
      }.Build();

      // Act
      var actualPath = parameters.GetProjectCompilationDirectory(csProject);

      // Assert
      Assert.AreEqual(expectedPath, actualPath);
    }

    #endregion

    #region GetProjectCompilationFile

    [Test]
    public void GetProjectCompilationFile_CsProjectParameterIsNull_ThrowsError()
    {
      // Arrange
      var parameters = new BuildEngineParameters();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => parameters.GetProjectCompilationFile(null));
    }

    [Test]
    public void GetProjectCompilationFile_CsProjectIsLibrary_ReturnsDllExtension()
    {
      // Arrange
      const string expectedValue = "Project.dll";

      var parameters = new BuildEngineParameters();

      var csProject = new CsProjectBuilder
                      {
                        Name = "Project",
                        OutputType = "Library"
                      }.Build();

      // Act
      var actualValue = parameters.GetProjectCompilationFile(csProject);

      // Assert
      Assert.AreEqual(expectedValue, actualValue);
    }

    [Test]
    public void GetProjectCompilationFile_CsProjectIsExecutable_ReturnsExeExtension()
    {
      // Arrange
      const string expectedValue = "Project.exe";

      var parameters = new BuildEngineParameters();

      var csProject = new CsProjectBuilder
                      {
                        Name = "Project",
                        OutputType = "Exe"
                      }.Build();

      // Act
      var actualValue = parameters.GetProjectCompilationFile(csProject);

      // Assert
      Assert.AreEqual(expectedValue, actualValue);
    }

    [Test]
    public void GetProjectCompilationFile_CsProjectIsWindowsService_ReturnsExeExtension()
    {
      // Arrange
      const string expectedValue = "Project.exe";

      var parameters = new BuildEngineParameters();

      var csProject = new CsProjectBuilder
      {
        Name = "Project",
        OutputType = "WinExe"
      }.Build();

      // Act
      var actualValue = parameters.GetProjectCompilationFile(csProject);

      // Assert
      Assert.AreEqual(expectedValue, actualValue);
    }

    [Test]
    public void GetProjectCompilationFile_CsProjectIsUnknown_ThrowsError()
    {
      // Arrange
      var parameters = new BuildEngineParameters();

      var csProject = new CsProjectBuilder
                      {
                        OutputType = "Unknown"
                      }.Build();

      // Act + Assert
      Assert.Throws<Exception>(() => parameters.GetProjectCompilationFile(csProject));
    }

    #endregion
  }
}