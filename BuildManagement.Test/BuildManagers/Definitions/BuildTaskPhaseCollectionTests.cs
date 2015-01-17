using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using Moq;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.BuildManagers.Definitions
{
  [TestFixture]
  public sealed class BuildTaskPhaseCollectionTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void BuildTaskPhaseCollection_PhasesParameterIsNull_ThrowsError()
    {
      // Arrange + Act + Assert
      Assert.Throws<ArgumentNullException>(() => new BuildTaskPhaseCollectionBuilder { Phases = null }.Build());
    }

    [Test]
    public void BuildTaskPhaseCollection_UnhandledTasksParameterIsNull_ThrowsError()
    {
      // Arrange + Act + Assert
      Assert.Throws<ArgumentNullException>(() => new BuildTaskPhaseCollectionBuilder { UnhandledTasks = null }.Build());
    }

    [Test]
    public void Equals_TwoIdenticalCollections_AreEqual()
    {
      // Arrange
      var collection1 = new BuildTaskPhaseCollectionBuilder().Build();
      var collection2 = new BuildTaskPhaseCollectionBuilder().Build();

      // Act + Assert
      Assert.IsTrue(collection1.Equals(collection2));
    }


    [Test]
    public void Equals_TwoCollectionsWithDifferentPhases_AreNotEqual()
    {
      // Arrange
      var collection1 = new BuildTaskPhaseCollectionBuilder { Phases = new[] { new BuildTaskPhaseBuilder { Order = 0 }.Build() } }.Build();
      var collection2 = new BuildTaskPhaseCollectionBuilder { Phases = new[] { new BuildTaskPhaseBuilder { Order = 111 }.Build() } }.Build();

      // Act + Assert
      Assert.IsFalse(collection1.Equals(collection2));
    }

    [Test]
    public void Equals_TwoCollectionsWithDifferentUnhandledTasks_AreNotEqual()
    {
      // Arrange
      var collection1 = new BuildTaskPhaseCollectionBuilder { UnhandledTasks = new[] { mockRepository.Create<IBuildTask>().Object } }.Build();
      var collection2 = new BuildTaskPhaseCollectionBuilder { UnhandledTasks = new[] { mockRepository.Create<IBuildTask>().Object } }.Build();

      // Act + Assert
      Assert.IsFalse(collection1.Equals(collection2));
    }

    [Test]
    public void Equals_OneCollectionIsNull_AreNotEqual()
    {
      // Arrange
      var collection1 = new BuildTaskPhaseCollectionBuilder().Build();
      BuildTaskPhaseCollectionBuilder collection2 = null;

      // Act + Assert
      Assert.IsFalse(collection1.Equals(collection2));
    }

    [Test]
    public void Equals_TwoIdenticalCollectionsWhereOneIsCastedToObject_AreEqual()
    {
      // Arrange
      var collection1 = new BuildTaskPhaseCollectionBuilder().Build();
      var collection2 = new BuildTaskPhaseCollectionBuilder().Build();

      // Act + Assert
      Assert.IsTrue(collection1.Equals((object)collection2));
    }

    [Test]
    public void Equals_TwoObjectsOfDifferentTypes_AreNotEqual()
    {
      // Arrange
      var collection1 = new BuildTaskPhaseCollectionBuilder().Build();
      var anotherObject = new object();

      // Act + Assert
      Assert.IsFalse(collection1.Equals(anotherObject));
    }
  }
}
