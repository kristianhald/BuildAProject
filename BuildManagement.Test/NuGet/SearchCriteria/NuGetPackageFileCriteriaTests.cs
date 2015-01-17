using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.Test.TestSupport;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using Moq;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.NuGet.SearchCriteria
{
  [TestFixture]
  public sealed class NuGetPackageFileCriteriaTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void DoesSupport_FilePathNotMatchingRegex_IsNotSupported()
    {
      // Arrange
      var criteria = new NuGetPackageFileCriteriaBuilder().Build();

      // Act
      var actualResult = criteria.DoesSupport("DoesNotContainThePackagesConfig");

      // Assert
      Assert.IsFalse(actualResult);
    }

    [Test]
    public void DoesSupport_FilePathContainsPackagesConfigFile_DoesSupport()
    {
      // Arrange
      var criteria = new NuGetPackageFileCriteriaBuilder().Build();

      // Act
      var actualResult = criteria.DoesSupport(@".\FilePath\packages.config");

      // Assert
      Assert.IsTrue(actualResult);
    }

    [Test]
    public void CreateProjectFromStream_ProjectCreated_FromStream()
    {
      // Test Constants
      const string filePath = @".\filepath\packages.config";

      // Arrange
      var expectedProjects = new[]
                             {
                               new NuGetPackageFileBuilder
                               {
                                 FilePath = filePath,
                                 PackageName = "Moq",
                                 Version = "4.2.1312.1622",
                                 Framework = "net45"
                               }.Build(),
                               new NuGetPackageFileBuilder
                               {
                                 FilePath = filePath,
                                 PackageName = "NUnit",
                                 Version = "2.6.3",
                                 Framework = null
                               }.Build(),
                             };

      using (
        var projectStream =
          TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.NuGetPackageResource))
      {
        var fakeFileSystem = mockRepository.Create<ILocatorFileSystem>();
        fakeFileSystem
          .Setup(fileSystem => fileSystem.CreateFileStream(filePath))
          .Returns(projectStream);

        projectStream.Position = 0; // Reset the stream after it has been read.

        var criteria = new NuGetPackageFileCriteriaBuilder { LocatorFileSystem = fakeFileSystem.Object }.Build();

        // Act
        var actualProjects = criteria.CreateProjectsFrom(filePath);

        // Assert
        Assert.AreEqual(expectedProjects, actualProjects);
      }
    }
  }
}
