using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using Moq;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.BuildManagers.Definitions
{
  [TestFixture]
  public sealed class DependencyTests
  {
    private static readonly MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void Equals_TwoDependenciesWithSameName_AreEqual()
    {
      // Arrange
      var dependencyBuilder1 = new DependencyBuilder { DependencyName = "SameName" };
      var dependencyBuilder2 = new DependencyBuilder { DependencyName = dependencyBuilder1.DependencyName };

      // Act + Assert
      Assert.AreEqual(dependencyBuilder1.Build(), dependencyBuilder2.Build());
    }

    [Test]
    public void Equals_TwoDependenciesWithDifferentNames_AreNotEqual()
    {
      // Arrange
      var dependencyBuilder1 = new DependencyBuilder { DependencyName = "Some Name" };
      var dependencyBuilder2 = new DependencyBuilder { DependencyName = "Another Name" };

      // Act + Assert
      Assert.AreNotEqual(dependencyBuilder1.Build(), dependencyBuilder2.Build());
    }

    [Test]
    public void Equals_BuildTaskWithSameNameAsDependency_AreEqual()
    {
      // Arrange
      var dependencyBuilder = new DependencyBuilder { DependencyName = "Some Name" };
      var fakeBuildTask = MockRepository.Create<IBuildTask>();
      fakeBuildTask
        .Setup(task => task.Name)
        .Returns(dependencyBuilder.DependencyName);

      // Act + Assert
      Assert.AreEqual(dependencyBuilder.Build(), fakeBuildTask.Object);
    }

    [Test]
    public void Equals_ProjectWithDifferentNameAsDependency_AreNotEqual()
    {
      // Arrange
      var dependencyBuilder = new DependencyBuilder { DependencyName = "Some Name" };
      var fakeProject = MockRepository.Create<IProject>();
      fakeProject
        .Setup(project => project.Name)
        .Returns("Another Name");

      // Act + Assert
      Assert.AreNotEqual(dependencyBuilder.Build(), fakeProject.Object);
    }

    [Test]
    public void Equals_TypeOfObjectIsUnknown_AreNotEqual()
    {
      // Arrange
      var dependencyBuilder = new DependencyBuilder { DependencyName = "Some Name" };
      var someObject = new Object();

      // Act + Assert
      Assert.AreNotEqual(dependencyBuilder.Build(), someObject);
    }
  }
}
