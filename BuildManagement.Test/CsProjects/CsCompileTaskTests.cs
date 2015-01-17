using System;
using BuildAProject.BuildManagement.CsProjects.Compilers;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using Moq;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.CsProjects
{
  [TestFixture]
  public sealed class CsCompileTaskTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void CsCompileTask_ProjectParameterIsNull_ThrowsError()
    {
      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => new CsCompileTaskBuilder { CsProject = null }.Build());
    }

    [Test]
    public void CsCompileTask_DotNetCompilerParameterIsNull_ThrowsError()
    {
      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => new CsCompileTaskBuilder { DotNetCompiler = null }.Build());
    }

    [Test]
    public void Execute_CsProject_IsProvidedToCompiler()
    {
      // Arrange
      var mockDotNetCompiler = mockRepository.Create<IDotNetCompiler>();
      var taskBuilder = new CsCompileTaskBuilder
                 {
                   DotNetCompiler = mockDotNetCompiler.Object
                 };
      var task = taskBuilder.Build();

      // Act
      task.Execute();

      // Assert
      mockDotNetCompiler.Verify(compiler => compiler.Build(taskBuilder.CsProject));
    }

    [Test]
    public void Equals_TwoIdenticalTasks_AreEqual()
    {
      // Arrange
      var taskOne = new CsCompileTaskBuilder().Build();
      var taskTwo = new CsCompileTaskBuilder().Build();

      // Act + Assert
      Assert.IsTrue(taskOne.Equals(taskTwo));
    }

    [Test]
    public void Equals_TwoTasksWithDifferentProjects_AreNotEqual()
    {
      // Arrange
      var taskOne = new CsCompileTaskBuilder { CsProject = new CsProjectBuilder { Name = "ProjectOne" }.Build() }.Build();
      var taskTwo = new CsCompileTaskBuilder { CsProject = new CsProjectBuilder { Name = "ProjectTwo" }.Build() }.Build();

      // Act + Assert
      Assert.IsFalse(taskOne.Equals(taskTwo));
    }

    [Test]
    public void Equals_OneTaskIsNull_AreNotEqual()
    {
      // Arrange
      var taskOne = new CsCompileTaskBuilder().Build();

      // Act + Assert
      Assert.IsFalse(taskOne.Equals(null));
    }

    [Test]
    public void Equals_TwoIdenticalTasksWhereOneIsCasted_AreEqual()
    {
      // Arrange
      var taskOne = new CsCompileTaskBuilder().Build();
      var taskTwo = new CsCompileTaskBuilder().Build();

      // Act + Assert
      Assert.IsTrue(taskOne.Equals((object)taskTwo));
    }

    [Test]
    public void Equals_TaskComparedToObject_AreNotEqual()
    {
      // Arrange
      var taskOne = new CsCompileTaskBuilder().Build();
      var someObject = new object();

      // Act + Assert
      Assert.IsFalse(taskOne.Equals(someObject));
    }
  }
}
