using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.Locators.SearchCriteria;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using Moq;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.Locators
{
  [TestFixture]
  public sealed class CriteriaProjectsLocatorTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void CriteriaProjectsLocator_FileSystemParameterIsNull_ThrowsError()
    {
      // Arrange
      var projectsLocatorBuilder = new ProjectsLocatorBuilder { LocatorFileSystem = null };

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => projectsLocatorBuilder.Build());
    }

    [Test]
    public void CriteriaProjectsLocator_FileCriteriasParameterIsNull_ThrowsError()
    {
      // Arrange
      var projectsLocatorBuilder = new ProjectsLocatorBuilder { FileCriterias = null };

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => projectsLocatorBuilder.Build());
    }

    [Test]
    public void CriteriaProjectsLocator_FileCriteriasParameterDoesNotContainAnyCriterias_ThrowsError()
    {
      // Arrange
      var projectsLocatorBuilder = new ProjectsLocatorBuilder
                                   {
                                     FileCriterias = new IFileCriteria[0]
                                   };

      // Act + Assert
      Assert.Throws<ArgumentException>(() => projectsLocatorBuilder.Build());
    }

    [Test]
    public void FindProjects_RootDirectoryPathParameterIsNull_ThrowsError()
    {
      // Arrange
      var projectsLocator = new ProjectsLocatorBuilder().Build();

      // Act + Arrange
      Assert.Throws<ArgumentNullException>(() => projectsLocator.FindProjects(null));
    }

    [Test]
    public void FindProjects_FilesMatchingCriteria_AreReturnedAsProjectObjects()
    {
      // Test Constants
      const string rootDirectory = "root";
      const string filename1 = "Filename1";

      // Arrange
      var expectedProject1 = mockRepository.Create<IProject>();
      var expectedProject2 = mockRepository.Create<IProject>();

      var expectedProjects = new[]
                             {
                               expectedProject1.Object,
                               expectedProject2.Object
                             };

      var fakeFileSystem = mockRepository.Create<ILocatorFileSystem>();
      fakeFileSystem
        .Setup(fileSystem => fileSystem.GetAllFilenames(rootDirectory))
        .Returns(new[] { filename1 });

      var fakeFileCriteria = mockRepository.Create<IFileCriteria>();
      fakeFileCriteria
        .Setup(criteria => criteria.DoesSupport(filename1))
        .Returns(true);
      fakeFileCriteria
        .Setup(criteria => criteria.CreateProjectsFrom(filename1))
        .Returns(new[] { expectedProject1.Object, expectedProject2.Object });

      var projectsLocator = new ProjectsLocatorBuilder
                                   {
                                     LocatorFileSystem = fakeFileSystem.Object,
                                     FileCriterias = new[] { fakeFileCriteria.Object }
                                   }.Build();

      // Act
      var actualProjects = projectsLocator.FindProjects(rootDirectory);

      // Assert
      CollectionAssert.AreEquivalent(expectedProjects, actualProjects);
    }

    [Test]
    public void FindProjects_NoFilesMatchesCriteria_NothingIsReturned()
    {
      // Test Constants
      const string rootDirectory = "root";
      const string filename1 = "Filename1";
      const string filename2 = "Filename2";
      const string filename3 = "Filename3";

      // Arrange
      var fakeFileSystem = mockRepository.Create<ILocatorFileSystem>();
      fakeFileSystem
        .Setup(fileSystem => fileSystem.GetAllFilenames(rootDirectory))
        .Returns(new[] { filename1, filename2, filename3 });

      var mockFileCriteria = mockRepository.Create<IFileCriteria>();

      var projectsLocator = new ProjectsLocatorBuilder
                            {
                              LocatorFileSystem = fakeFileSystem.Object,
                              FileCriterias = new[] { mockFileCriteria.Object }
                            }.Build();

      // Act
      var actualProjects = projectsLocator.FindProjects(rootDirectory);

      // Assert
      CollectionAssert.IsEmpty(actualProjects);
      mockFileCriteria.Verify(criteria => criteria.DoesSupport(filename1), Times.Once);
      mockFileCriteria.Verify(criteria => criteria.CreateProjectsFrom(filename1), Times.Never);
      mockFileCriteria.Verify(criteria => criteria.DoesSupport(filename2), Times.Once);
      mockFileCriteria.Verify(criteria => criteria.CreateProjectsFrom(filename2), Times.Never);
      mockFileCriteria.Verify(criteria => criteria.DoesSupport(filename3), Times.Once);
      mockFileCriteria.Verify(criteria => criteria.CreateProjectsFrom(filename3), Times.Never);
    }
  }
}
