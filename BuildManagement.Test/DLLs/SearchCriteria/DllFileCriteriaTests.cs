using System;
using BuildAProject.BuildManagement.DLLs.SearchCriteria;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.DLLs.SearchCriteria
{
  [TestFixture]
  public sealed class DllFileCriteriaTests
  {
    [Test]
    public void DllFileCriteria_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(DllFileCriteria).GetConstructors());
    }

    [Test]
    public void DoesSupport_FilePathMatchesRegex_ReturnTrue()
    {
      // Arrange
      var criteria = new DllFileCriteria();

      // Act
      var actualResult = criteria.DoesSupport(@".\FilePath\pr1oj4-ect.dll");

      // Assert
      Assert.IsTrue(actualResult);
    }

    [Test]
    public void DoesSupport_FilePathDoesNotMatchRegex_ReturnsFalse()
    {
      // Arrange
      var criteria = new DllFileCriteria();

      // Act
      var actualResult = criteria.DoesSupport("DoesNotContainTheDllExtension");

      // Assert
      Assert.IsFalse(actualResult);
    }

    [Test]
    public void CreateProjectsFrom_FilePathParameterIsNull_ThrowsError()
    {
      // Arrange
      var criteria = new DllFileCriteria();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => criteria.CreateProjectsFrom(null));
    }

    [Test]
    public void CreateProjectsFrom_FilePathProvidedToCriteria_AndAppropriateProjectReturned()
    {
      // Test Constants
      const string filePath = @".\filepath\project.dll";

      // Arrange
      var expectedProject = new DllFileProjectBuilder { FilePath = filePath }.Build();

      var criteria = new DllFileCriteria();

      // Act
      var actualProjects = criteria.CreateProjectsFrom(filePath);

      // Assert
      CollectionAssert.AreEquivalent(new[] { expectedProject }, actualProjects);
    }
  }
}
