using BuildAProject.BuildManagement.CsProjects;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.CsProjects
{
  [TestFixture]
  public sealed class ReferenceTests
  {
    [Test]
    public void Reference_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(Reference).GetConstructors());
    }

    [Test]
    public void Name_WhenCalled_ReturnsTheProvidedReferenceName()
    {
      // Arrange
      const string expectedName = "Name";

      var reference = new ReferenceBuilder
                      {
                        Name = expectedName
                      }.Build();

      // Act
      var actualName = reference.Name;

      // Assert
      Assert.AreEqual(expectedName, actualName);
    }

    [Test]
    public void Name_WhenProvidedWithFulleAssemblyInformation_ReturnsOnlyTheNameOfTheAssembly()
    {
      // Arrange
      const string expectedName = "NuGet.Core";
      var reference = new ReferenceBuilder
                      {
                        Name =
                          "NuGet.Core, Version=2.8.50126.400, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL"
                      }.Build();

      // Act
      var actualName = reference.Name;

      // Assert
      Assert.AreEqual(expectedName, actualName);
    }

    [Test]
    public void HintPath_WhenCalled_ReturnsTheProvidedHintPath()
    {
      // Arrange
      const string expectedHintPath = "HintPath";
      var reference = new ReferenceBuilder
                      {
                        HintPath = expectedHintPath
                      }.Build();

      // Act
      var actualHintPath = reference.HintPath;

      // Assert
      Assert.AreEqual(expectedHintPath, actualHintPath);
    }

    [Test]
    public void HintPath_WhenProvidedWithAnEmptyString_ReturnsTheEmptyStringWithoutErrorAsItStatesThereAreNoHintPath()
    {
      // Arrange
      const string expectedHintPath = "";
      var reference = new ReferenceBuilder
                      {
                        HintPath = expectedHintPath
                      }.Build();

      // Act
      var actualHintPath = reference.HintPath;

      // Assert
      Assert.AreEqual(expectedHintPath, actualHintPath);
    }
  }
}
