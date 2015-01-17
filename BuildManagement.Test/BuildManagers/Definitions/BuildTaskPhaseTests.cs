using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using Moq;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.BuildManagers.Definitions
{
  [TestFixture]
  public sealed class BuildTaskPhaseTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void BuildTaskPhase_BuildTasksParameterIsNull_ThrowsError()
    {
      // Arrange + Act + Assert
      Assert.Throws<ArgumentNullException>(() => new BuildTaskPhaseBuilder { BuildTasks = null }.Build());
    }

    [Test]
    public void BuildTaskPhase_BuildTasksParameterIsEmpty_ThrowsError()
    {
      // Arrange + Act + Assert
      Assert.Throws<ArgumentException>(() => new BuildTaskPhaseBuilder { BuildTasks = new IBuildTask[0] }.Build());
    }

    [Test]
    public void Equals_TwoIdenticalPhases_AreEqual()
    {
      // Arrange
      var phase1 = new BuildTaskPhaseBuilder().Build();
      var phase2 = new BuildTaskPhaseBuilder().Build();

      // Act + Assert
      Assert.AreEqual(phase1, phase2);
    }

    [Test]
    public void Equals_TwoPhasesWithDifferentOrder_AreNotEqual()
    {
      // Arrange
      var phase1 = new BuildTaskPhaseBuilder { Order = 0 }.Build();
      var phase2 = new BuildTaskPhaseBuilder { Order = 1 }.Build();

      // Act + Assert
      Assert.AreNotEqual(phase1, phase2);
    }

    [Test]
    public void Equals_TwoPhasesWithDifferentBuildTasks_AreNotEqual()
    {
      // Arrange
      var phase1 = new BuildTaskPhaseBuilder { BuildTasks = new[] { mockRepository.Create<IBuildTask>().Object } }.Build();
      var phase2 = new BuildTaskPhaseBuilder { BuildTasks = new[] { mockRepository.Create<IBuildTask>().Object } }.Build();

      // Act + Assert
      Assert.AreNotEqual(phase1, phase2);
    }

    [Test]
    public void Equals_OnePhaseIsNull_AreNotEqual()
    {
      // Arrange
      var phase1 = new BuildTaskPhaseBuilder().Build();
      BuildTaskPhase phase2 = null;

      // Act + Assert
      Assert.IsFalse(phase1.Equals(phase2));
    }

    [Test]
    public void Equals_TwoIdenticalPhasesWhereOneIsCastedToObject_AreEqual()
    {
      // Arrange
      var phase1 = new BuildTaskPhaseBuilder().Build();
      var phase2 = new BuildTaskPhaseBuilder().Build();

      // Act + Assert
      Assert.IsTrue(phase1.Equals((object)phase2));
    }

    [Test]
    public void Equals_TwoObjectsOfDifferentTypes_AreNotEqual()
    {
      // Arrange
      var phase1 = new BuildTaskPhaseBuilder().Build();
      var otherObject = new object();

      // Act + Assert
      Assert.AreNotEqual(phase1, otherObject);
    }
  }
}
