using BuildAProject.BuildManagement.DLLs;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.DLLs
{
  [TestFixture]
  public sealed class DllFileTaskTests
  {
    [Test]
    public void DllFileTask_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(DllFileTask).GetConstructors());
    }

    [Test]
    public void Name_FromDllFileProject_IsSameAsDllFileTask()
    {
      // Arrange
      var dllFileProject = new DllFileProjectBuilder().Build();
      var dllFileTask = new DllFileTask(dllFileProject);

      // Act
      var actualName = dllFileTask.Name;

      // Assert
      Assert.AreEqual(dllFileProject.Name, actualName);
    }

    [Test]
    public void Dependencies_FromDllFileProject_IsSameAsDllFileTask()
    {
      // Arrange
      var dllFileProject = new DllFileProjectBuilder().Build();
      var dllFileTask = new DllFileTask(dllFileProject);

      // Act
      var actualDependencies = dllFileTask.Dependencies;

      // Assert
      CollectionAssert.AreEquivalent(dllFileProject.Dependencies, actualDependencies);
    }

    [Test]
    public void Execute_WhenCalled_DoesNothing()
    {
      // Arrange
      var dllFileProject = new DllFileProjectBuilder().Build();
      var dllFileTask = new DllFileTask(dllFileProject);

      // Act + Assert
      dllFileTask.Execute();
    }
  }
}
