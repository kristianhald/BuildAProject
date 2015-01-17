using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
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
  public sealed class FileNameCriteriaTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void FileNameCriteria_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(MockFileNameCriteria).GetConstructors());
    }

    [Test]
    public void DoesSupport_FilenameIsNotSupportedByCriteria_ReturnsFalse()
    {
      // Arrange
      var criteria = new MockFileNameCriteria(new Regex("^[a]"));

      // Act
      var actualResult = criteria.DoesSupport("DoesNotMatchCriteria");

      // Assert
      Assert.IsFalse(actualResult);
    }

    [Test]
    public void DoesSupport_FilenameIsSupportedByCriteria_ReturnsTrue()
    {
      // Arrange
      var criteria = new MockFileNameCriteria(new Regex("[a][.][a-z]*"));

      // Act
      var actualResult = criteria.DoesSupport("a.txt");

      // Assert
      Assert.IsTrue(actualResult);
    }

    [Test]
    public void CreateProjectsFrom_FilenameParameterIsNull_ThrowsError()
    {
      // Arrange
      var criteria = new MockFileNameCriteria(new Regex("[a]"));

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => criteria.CreateProjectsFrom(null));
    }

    [Test]
    public void CreateProjectsFrom_FilenameIsNotSupported_ThrowsError()
    {
      // Arrange
      var criteria = new MockFileNameCriteria(new Regex("^[a]"));

      // Act + Assert
      Assert.Throws<ArgumentException>(() => criteria.CreateProjectsFrom("DoesNotMatchCriteria"));
    }

    [Test]
    public void CreateProjectsFrom_FilePath_InheritorCreatesProject()
    {
      // Test Constants
      const string filePath = "Filename";

      // Arrange
      var expectedProject = mockRepository.Create<IProject>().Object;

      var mockCriteria = mockRepository.Create<FileNameCriteria>(new Regex("[a]"));
      mockCriteria
        .Protected()
        .Setup<IEnumerable<IProject>>("CreateProjectFromFilePath", filePath)
        .Returns(new[] { expectedProject });

      // Act
      var actualProjects = mockCriteria
        .Object
        .CreateProjectsFrom(filePath);

      // Assert
      CollectionAssert.AreEquivalent(new[] { expectedProject }, actualProjects);
    }

    private class MockFileNameCriteria : FileNameCriteria
    {
      public MockFileNameCriteria(Regex filenameMatchRegex)
        : base(filenameMatchRegex)
      { }

      protected override IEnumerable<IProject> CreateProjectFromFilePath(string filePath)
      {
        throw new NotImplementedException();
      }
    }
  }
}
