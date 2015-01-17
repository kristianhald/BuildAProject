using BuildAProject.BuildManagement.CsProjects.Compilers;
using BuildAProject.BuildManagement.NUnit;
using BuildAProject.BuildManagement.NUnit.Runners;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.NUnit
{
  [TestFixture]
  public sealed class NUnitTestTaskTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void NUnitTestTask_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());
      fixture.Register(() => new CsProjectBuilder().Build());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(NUnitTestTask).GetConstructors());
    }

    [Test]
    public void Dependencies_WhenCalled_ReturnsTheCsProjectAsDependency()
    {
      // Test Constants
      const string dependencyName = "MyDependency";

      // Arrange
      var expectedDependency = new DependencyBuilder
                               {
                                 DependencyName = dependencyName
                               }.Build();

      var csProject = new CsProjectBuilder
                      {
                        Name = dependencyName
                      }.Build();

      var task = new NUnitTestTaskBuilder
                 {
                   CsProject = csProject
                 }.Build();

      // Act
      var actualDependencies = task.Dependencies;

      // Assert
      CollectionAssert.AreEquivalent(new[] { expectedDependency }, actualDependencies);
    }

    [Test]
    public void Name_WhenCalled_ReturnsTheNameOfTheCsProjectPrefixedWithTest()
    {
      // Test Constants
      const string csProjectName = "Project";

      // Arrange
      const string expectedName = "Test" + csProjectName;

      var csProject = new CsProjectBuilder
                      {
                        Name = csProjectName
                      }.Build();

      var task = new NUnitTestTaskBuilder
                 {
                   CsProject = csProject
                 }.Build();

      // Act
      var actualName = task.Name;

      // Assert
      Assert.AreEqual(expectedName, actualName);
    }

    [Test]
    public void Execute_DeterminesTheLocationOfTheTestFile_AndCallsTheTestRunner()
    {
      // Arrange
      const string expectedFilePath = @"GeneralDirectory\ProjectName.dll";

      var mockTestRunner = mockRepository.Create<ITestRunner>();

      var csProject = new CsProjectBuilder
                      {
                        Name = "ProjectName",
                        OutputType = "Library"
                      }.Build();

      var parameters = new BuildEngineParameters
                       {
                         GeneralOutputDirectory = "GeneralDirectory"
                       };

      var task = new NUnitTestTaskBuilder
                 {
                   TestRunner = mockTestRunner.Object,
                   CsProject = csProject,
                   BuildEngineParameters = parameters
                 }.Build();

      // Act
      task.Execute();

      // Assert
      mockTestRunner
        .Verify(runner => runner.Test(expectedFilePath), Times.Once);
    }
  }
}
