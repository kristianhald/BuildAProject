using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers;
using BuildAProject.Console.Logging;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.Console.Test.Logging
{
  [TestFixture]
  public sealed class BuildTaskManagerLoggerTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void BuildTaskManagerLogger_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture().Customize(new AutoMoqCustomization());
      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(BuildTaskManagerLogger).GetConstructors());
    }

    [Test]
    public void GetTasks_WhenCalled_ThenTheCompositeIsCalled()
    {
      // Arrange
      var expectedTasks = new[]
                          {
                            mockRepository.Create<IBuildTask>().Object
                          };

      var projects = new[]
                     {
                       mockRepository.Create<IProject>().Object
                     };

      var mockComposite = mockRepository.Create<IBuildTaskManager>();
      mockComposite
        .Setup(composite => composite.GetTasks(projects))
        .Returns(expectedTasks);
      var fakeLog = mockRepository.Create<ILog>();

      var logger = new BuildTaskManagerLogger(mockComposite.Object, fakeLog.Object);

      // Act
      var actualTasks = logger.GetTasks(projects);

      // Assert
      CollectionAssert.AreEquivalent(expectedTasks, actualTasks);
    }

    [Test]
    public void GetTasks_WhenCalled_ThenTheInformationMethodOnTheLogIsCalled()
    {
      // Arrange
      var fakeComposite = mockRepository.Create<IBuildTaskManager>();
      var mockLog = mockRepository.Create<ILog>();

      var logger = new BuildTaskManagerLogger(fakeComposite.Object, mockLog.Object);

      // Act
      logger.GetTasks(new IProject[0]);

      // Assert
      mockLog
        .Verify(log => log.Information(It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(3));
    }
  }
}
