using BuildAProject.BuildManagement.CsProjects;
using BuildAProject.BuildManagement.Locators;
using BuildAProject.BuildManagement.Test.TestSupport;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using BuildAProject.BuildManagement.Test.TestSupport.FileSystems;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.Locators
{
  [TestFixture]
  [Category("ModuleTest")]
  public sealed class LocatorsModuleTests
  {
    // TODO: I believe the 'Locators' folder should be in its own project
    //       and this test should not use the CsProject classes but instead
    //       mock these so that the project has unit tests and a single 
    //       internal module test (this test) that ensures the code is working 
    //       with no dependencies to other libraries.
    //       If it uses other libraries then an integration test using these 
    //       libraries must be created.
    [Test]
    public void PrimaryFlowTest()
    {
      using (var resourceProvider = new TestFileResourceProvider())
      {
        // Arrange
        var fileSystem = new EmbeddedResourceFileSystem(resourceProvider);

        var expectedProjects = new[]
                               {
                                 new CsProject(
                                   resourceProvider.GetResourceStream(TestFileResourceProvider.HelloWorldProjectResource),
                                   TestFileResourceProvider.HelloWorldProjectResource,
                                   fileSystem),
                                 new CsProject(
                                   resourceProvider.GetResourceStream(TestFileResourceProvider.AnotherWorldProjectResource),
                                   TestFileResourceProvider.AnotherWorldProjectResource,
                                   fileSystem)
                               };

        var locator = new CriteriaProjectsLocator(fileSystem, new[] { new CsProjectFileCriteriaBuilder { LocatorFileSystem = fileSystem }.Build() });

        // Act
        var actualProjects = locator.FindProjects(".");

        // Assert
        CollectionAssert.AreEquivalent(expectedProjects, actualProjects);
      }
    }
  }
}
