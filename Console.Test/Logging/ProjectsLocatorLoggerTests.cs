using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.Locators;
using BuildAProject.Console.Logging;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.Console.Test.Logging
{
  [TestFixture]
  public sealed class ProjectsLocatorLoggerTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void ProjectsLocatorLogger_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture().Customize(new AutoMoqCustomization());
      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(ProjectsLocatorLogger).GetConstructors());
    }

    [Test]
    public void FindProjects_WhenCalled_ThenTheCompositeIsCalled()
    {
      // Test Constants
      const string rootDirectoryPath = ".";

      // Arrange
      var expectedProjects = new[]
                             {
                               mockRepository.Create<IProject>().Object
                             };

      var mockComposite = mockRepository.Create<IProjectsLocator>();
      mockComposite
        .Setup(composite => composite.FindProjects(rootDirectoryPath))
        .Returns(expectedProjects);

      var fakeLog = mockRepository.Create<ILog>();

      var logger = new ProjectsLocatorLogger(mockComposite.Object, fakeLog.Object);

      // Act
      var actualProjects = logger.FindProjects(rootDirectoryPath);

      // Assert
      CollectionAssert.AreEquivalent(expectedProjects, actualProjects);
    }

    [Test]
    public void FindProjects_WhenCalled_ThenTheInformationMethodOnTheLogIsCalled()
    {
      // Test Constants
      const string rootDirectoryPath = ".";

      // Arrange
      var fakeComposite = mockRepository.Create<IProjectsLocator>();
      var mockLog = mockRepository.Create<ILog>();

      var logger = new ProjectsLocatorLogger(fakeComposite.Object, mockLog.Object);

      // Act
      logger.FindProjects(rootDirectoryPath);

      // Assert
      mockLog
        .Verify(log => log.Information(It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(2));
    }
  }
}
