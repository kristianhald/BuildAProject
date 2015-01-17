using System;
using BuildAProject.BuildManagement.NUnit.TaskProviders;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.NUnit.TaskProviders
{
  [TestFixture]
  public sealed class NUnitTestTaskFactoryTests
  {
    [Test]
    public void NUnitTestTaskFactory_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(NUnitTestTaskFactory).GetConstructors());
    }

    [Test]
    public void Create_ProjectParameterIsNull_ThrowsError()
    {
      // Arrange
      var factory = new NUnitTestTaskFactoryBuilder().Build();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => factory.Create(null));
    }

    [Test]
    public void Create_TaskIsCreated_WithProject()
    {
      // Arrange
      var project = new CsProjectBuilder().Build();
      var expectedResult = new NUnitTestTaskBuilder { CsProject = project }.Build();

      var factory = new NUnitTestTaskFactoryBuilder().Build();

      // Act
      var actualResult = factory.Create(project);

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }
  }
}
