using System;
using System.Linq;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.Exceptions;
using Moq;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.BuildManagers.Exceptions
{
  public sealed class UnhandledTasksExceptionTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void UnhandledTasksException_UnhandledTasksParameterIsNull_ThrowsError()
    {
      // Arrange + Act + Assert
      Assert.Throws<ArgumentNullException>(() => new UnhandledTasksException(null));
    }

    [Test]
    public void UnhandledTasksException_UnhandledTasksParameterIsEmpty_ThrowsError()
    {
      // Arrange + Act + Assert
      Assert.Throws<ArgumentException>(() => new UnhandledTasksException(new IBuildTask[0]));
    }

    [Test]
    public void Message_TheNamesOfTheUnhandledBuildTasks_AreShownInTheExceptionString()
    {
      // Arrange
      var taskOne = mockRepository.Create<IBuildTask>();
      taskOne
        .Setup(task => task.Name)
        .Returns("Task One");
      var taskTwo = mockRepository.Create<IBuildTask>();
      taskTwo
        .Setup(task => task.Name)
        .Returns("Task Two");
      var tasks = new[]
                  {
                    taskOne.Object,
                    taskTwo.Object
                  };

      var expectedString = String.Format(
        "A number of build tasks were unhandled.{0}The tasks are:{0}{1}",
        Environment.NewLine,
        String.Join(Environment.NewLine, tasks.Select(task => task.Name)));

      var exception = new UnhandledTasksException(tasks);

      // Act
      var actualString = exception.Message;

      // Assert
      Assert.AreEqual(expectedString, actualString);
    }
  }
}
