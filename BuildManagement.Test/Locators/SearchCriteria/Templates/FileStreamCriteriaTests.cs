using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.Locators.SearchCriteria.Templates;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.Locators.SearchCriteria.Templates
{
  [TestFixture]
  public sealed class FileStreamCriteriaTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose) { CallBase = true };

    [Test]
    public void FileStreamCriteria_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(MockFileStreamCriteria).GetConstructors());
    }

    [Test]
    public void CreateProjectsFrom_FilePathCouldNotBeFound_ThrowsError()
    {
      // Arrange
      var stubLocatorFileSystem = mockRepository.Create<ILocatorFileSystem>();

      var criteria = new MockFileStreamCriteria(new Regex("[a]"), stubLocatorFileSystem.Object);

      // Act + Assert
      Assert.Throws<Exception>(() => criteria.CreateProjectsFrom("Filename"));
    }

    [Test]
    public void CreateProjectsFrom_FilePath_InheritorCreatesProject()
    {
      // Test Constants
      const string filePath = "Filename";

      // Arrange
      var expectedProject = mockRepository.Create<IProject>().Object;

      var fakeMemoryStream = new MemoryStream();

      var fakeFileSystem = mockRepository.Create<ILocatorFileSystem>();
      fakeFileSystem
        .Setup(fileSystem => fileSystem.CreateFileStream(filePath))
        .Returns(fakeMemoryStream);

      var mockCriteria = mockRepository.Create<FileStreamCriteria>(new Regex("[a]"), fakeFileSystem.Object);
      mockCriteria
        .Protected()
        .Setup<IEnumerable<IProject>>("CreateProjectFromStream", fakeMemoryStream, filePath)
        .Returns(new[] { expectedProject });

      // Act
      var actualProjects = mockCriteria
        .Object
        .CreateProjectsFrom(filePath);

      // Assert
      CollectionAssert.AreEquivalent(new[] { expectedProject }, actualProjects);
    }

    private class MockFileStreamCriteria : FileStreamCriteria
    {
      public MockFileStreamCriteria(Regex filenameMatchRegex, ILocatorFileSystem locatorFileSystem)
        : base(filenameMatchRegex, locatorFileSystem)
      {
      }

      protected override IEnumerable<IProject> CreateProjectFromStream(Stream projectStream, string filePath)
      {
        throw new System.NotImplementedException();
      }
    }
  }
}
