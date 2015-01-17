using System;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.CsProjects.TaskProviders
{
  [TestFixture]
  public sealed class CsCompileTaskFactoryTests
  {
    [Test]
    public void CsCompileTaskFactory_DotNetCompilerParameterIsNull_ThrowsError()
    {
      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => new CsCompileTaskFactoryBuilder { DotNetCompiler = null }.Build());
    }

    [Test]
    public void Create_ProjectParameterIsNull_ThrowsError()
    {
      // Arrange
      var factory = new CsCompileTaskFactoryBuilder().Build();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => factory.Create(null));
    }

    [Test]
    public void Create_TaskIsCreated_WithProject()
    {
      // Arrange
      var project = new CsProjectBuilder().Build();
      var expectedResult = new CsCompileTaskBuilder { CsProject = project }.Build();

      var factory = new CsCompileTaskFactoryBuilder().Build();

      // Act
      var actualResult = factory.Create(project);

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }
  }
}
