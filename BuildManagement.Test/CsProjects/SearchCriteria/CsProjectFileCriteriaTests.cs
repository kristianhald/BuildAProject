using BuildAProject.BuildManagement.CsProjects;
using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.Test.TestSupport;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using Moq;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.CsProjects.SearchCriteria
{
  [TestFixture]
  public sealed class CsProjectFileCriteriaTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void DoesSupport_FilePathDoesNotMatchRegex_ReturnsFalse()
    {
      // Arrange
      var criteria = new CsProjectFileCriteriaBuilder().Build();

      // Act
      var actualResult = criteria.DoesSupport("DoesNotContainTheCsProjExtension");

      // Assert
      Assert.IsFalse(actualResult);
    }

    [Test]
    public void DoesSupport_FilePathMatchesRegex_ReturnTrue()
    {
      // Arrange
      var criteria = new CsProjectFileCriteriaBuilder().Build();

      // Act
      var actualResult = criteria.DoesSupport(@".\FilePath\pr1oj4-ect.csproj");

      // Assert
      Assert.IsTrue(actualResult);
    }

    [Test]
    public void CreateProjectFromStream_ProjectCreated_FromStream()
    {
      // Test Constants
      const string filePath = @".\filepath\project.csproj";

      // Arrange
      using (
        var projectStream =
          TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.AnotherWorldProjectResource))
      {
        var fakeFileSystem = mockRepository.Create<ILocatorFileSystem>();
        fakeFileSystem
          .Setup(fileSystem => fileSystem.CreateFileStream(filePath))
          .Returns(projectStream);

        var expectedProject = new CsProject(projectStream, filePath, fakeFileSystem.Object);
        projectStream.Position = 0; // Reset the stream after it has been read.

        var criteria = new CsProjectFileCriteriaBuilder { LocatorFileSystem = fakeFileSystem.Object }.Build();

        // Act
        var actualProjects = criteria.CreateProjectsFrom(filePath);

        // Assert
        CollectionAssert.AreEquivalent(new[] { expectedProject }, actualProjects);
      }
    }

    [Test]
    public void CreateProjectFromStream_ProjectIsUnsupported_ThrowsError()
    {
      // Test Constants
      const string filePath = @".\filepath\project.csproj";

      // Arrange
      using (
        var projectStream =
          TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.UnsupportedProject))
      {
        var fakeFileSystem = mockRepository.Create<ILocatorFileSystem>();
        fakeFileSystem
          .Setup(fileSystem => fileSystem.CreateFileStream(filePath))
          .Returns(projectStream);

        var criteria = new CsProjectFileCriteriaBuilder { LocatorFileSystem = fakeFileSystem.Object }.Build();

        // Act
        var actualProjects = criteria.CreateProjectsFrom(filePath);

        //Assert
        CollectionAssert.AreEquivalent(new CsProject[0], actualProjects);
      }
    }
  }
}
