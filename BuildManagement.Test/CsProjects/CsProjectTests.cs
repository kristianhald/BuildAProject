using System.IO;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.CsProjects;
using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.Test.TestSupport;
using Microsoft.Build.Exceptions;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.CsProjects
{
  [TestFixture]
  public sealed class CsProjectTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void CsProject_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());
      fixture.Register<Stream>(() => new MemoryStream());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(CsProject).GetConstructors());
    }

    [Test]
    public void CsProject_UnsupportProject_ThrowsError()
    {
      // Arrange

      using (
        var unsupportedProjectStream =
          TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.UnsupportedProject))
      {
        var mockFileSystem = mockRepository.Create<ILocatorFileSystem>();

        // Act + Assert
        Assert.Throws<InvalidProjectFileException>(() => new CsProject(
          unsupportedProjectStream,
          TestFileResourceProvider.UnsupportedProject,
          mockFileSystem.Object));
      }
    }

    [Test]
    public void Name_ReadFromStream_Correctly()
    {
      using (
        Stream projectStream =
          TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.HelloWorldProjectResource),
          dependencyProjectStream =
            TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.AnotherWorldProjectResource))
      {
        // Arrange
        const string expectedName = "HelloWorld.AssemblyName";
        var mockFileSystem = mockRepository.Create<ILocatorFileSystem>();
        mockFileSystem
          .Setup(fileSystem => fileSystem.CreateFileStream(@".\CsProjects\ProjectFiles\AnotherWorld.csproj"))
          .Returns(dependencyProjectStream);

        var project = new CsProject(
          projectStream,
          TestFileResourceProvider.HelloWorldProjectResource,
          mockFileSystem.Object);

        // Act
        var actualName = project.Name;

        // Assert
        Assert.AreEqual(expectedName, actualName);
      }
    }

    [Test]
    public void References_ReadFromProvidedStream_MatchExpectedReferences()
    {
      using (
        Stream projectStream =
          TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.HelloWorldProjectResource),
          dependencyProjectStream =
            TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.AnotherWorldProjectResource))
      {
        // Arrange
        var expectedReferences = new[]
                                 {
                                   new Reference("Moq", @"..\packages\Moq.4.2.1312.1622\lib\net40\Moq.dll"),
                                   new Reference("AnotherWorld.AssemblyName", "")
                                 };

        var mockFileSystem = mockRepository.Create<ILocatorFileSystem>();
        mockFileSystem
          .Setup(fileSystem => fileSystem.CreateFileStream(It.IsAny<string>()))
          .Returns(dependencyProjectStream);

        var project = new CsProject(
          projectStream, 
          TestFileResourceProvider.HelloWorldProjectResource,
          mockFileSystem.Object);

        // Act
        var actualReferences = project.References;

        // Assert
        CollectionAssert.AreEquivalent(expectedReferences, actualReferences);
      }
    }

    [Test]
    public void Dependencies_ReadFromStream_OnlyExternalCsProjectDependencies()
    {
      using (
        Stream projectStream =
          TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.HelloWorldProjectResource),
          dependencyProjectStream =
            TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.AnotherWorldProjectResource))
      {
        // Arrange
        var expectedDependencies = new[]
                                   {
                                     new Dependency("Moq"),
                                     new Dependency("AnotherWorld.AssemblyName")
                                   };

        var mockFileSystem = mockRepository.Create<ILocatorFileSystem>();
        mockFileSystem
          .Setup(fileSystem => fileSystem.CreateFileStream(It.IsAny<string>()))
          .Returns(dependencyProjectStream);

        var project = new CsProject(projectStream, TestFileResourceProvider.HelloWorldProjectResource,
          mockFileSystem.Object);

        // Act
        var actualDependencies = project.Dependencies;

        // Assert
        CollectionAssert.AreEquivalent(expectedDependencies, actualDependencies);
      }
    }

    [Test]
    public void OutputType_ReadFromStream_Correctly()
    {
      using (
        Stream projectStream =
          TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.HelloWorldProjectResource),
          dependencyProjectStream =
            TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.AnotherWorldProjectResource))
      {
        // Arrange
        const string expectedOutputType = "Library";
        var mockFileSystem = mockRepository.Create<ILocatorFileSystem>();
        mockFileSystem
          .Setup(fileSystem => fileSystem.CreateFileStream(@".\CsProjects\ProjectFiles\AnotherWorld.csproj"))
          .Returns(dependencyProjectStream);

        var project = new CsProject(projectStream, TestFileResourceProvider.HelloWorldProjectResource,
          mockFileSystem.Object);

        // Act
        var actualOutputType = project.OutputType;

        // Assert
        Assert.AreEqual(expectedOutputType, actualOutputType);
      }
    }
  }
}
