using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.DLLs;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.DLLs
{
  [TestFixture]
  public sealed class DllFileProjectTests
  {
    [Test]
    public void DllFileProject_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(DllFileProject).GetConstructors());
    }

    [Test]
    public void Name_IsProvidedWithAFilePath_AndReturnsTheFilenameWithoutExtension()
    {
      // Arrange
      const string expectedName = "ExpectedName";

      var filePath = String.Format(@".\some\file\path\{0}.someextension", expectedName);
      var project = new DllFileProjectBuilder { FilePath = filePath }.Build();

      // Act
      var actualName = project.Name;

      // Assert
      Assert.AreEqual(expectedName, actualName);
    }

    [Test]
    public void Dependencies_List_IsAlwaysEmpty()
    {
      // Arrange
      var expected = new Dependency[0];

      var project = new DllFileProjectBuilder().Build();

      // Act
      var actual = project.Dependencies;

      // Assert
      CollectionAssert.AreEquivalent(expected, actual);
    }
  }
}
