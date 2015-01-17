using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.NuGet;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.NuGet
{
  [TestFixture]
  public sealed class NuGetPackageFileTests
  {
    [Test]
    public void NuGetPackageFile_PackageNameParameterIsNull_ThrowsError()
    {
      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => new NuGetPackageFileBuilder { PackageName = null }.Build());
    }

    [Test]
    public void NuGetPackageFile_VersionParameterIsNull_ThrowsError()
    {
      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => new NuGetPackageFileBuilder { Version = null }.Build());
    }

    [Test]
    public void NuGetPackageFile_FrameworkParameterIsNull_NoErrorAsTheParameterIsNotNecessary()
    {
      // Act + Assert
      Assert.DoesNotThrow(() => new NuGetPackageFileBuilder { Framework = null }.Build());
    }

    [Test]
    public void NuGetPackageFile_FrameworkParameterIsEmptyString_ThrowsErrorAsIfThereIsNoValueThenItMustBeNullOrSet()
    {
      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => new NuGetPackageFileBuilder { Framework = "" }.Build());
    }

    [Test]
    public void NuGetPackageFile_FilePathIsNull_ThrowsError()
    {
      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => new NuGetPackageFileBuilder { FilePath = null }.Build());
    }

    [Test]
    public void Name_ProvidedViaConstructor_IsAccessible()
    {
      // Arrange
      const string expectedResult = "NuGetPackage";

      var package = new NuGetPackageFileBuilder
                    {
                      PackageName = expectedResult
                    }.Build();

      // Act
      var actualResult = package.Name;

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void Version_ProvidedViaConstructor_IsAccessible()
    {
      // Arrange
      const string expectedResult = "1.2.3.4.5";

      var package = new NuGetPackageFileBuilder
      {
        Version = expectedResult
      }.Build();

      // Act
      var actualResult = package.Version;

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void Framework_ProvidedViaConstructor_IsAccessible()
    {
      // Arrange
      const string expectedResult = "4.5";

      var package = new NuGetPackageFileBuilder
      {
        Framework = expectedResult
      }.Build();

      // Act
      var actualResult = package.Framework;

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void Dependencies_ListIs_AlwaysEmpty()
    {
      // Arrange
      var expectedResult = new Dependency[0];
      var package = new NuGetPackageFileBuilder().Build();

      // Act
      var actualResult = package.Dependencies;

      // Assert
      CollectionAssert.AreEquivalent(expectedResult, actualResult);
    }

    [Test]
    public void Equals_TwoIdenticalObjects_AreEquals()
    {
      // Arrange
      var packageOne = new NuGetPackageFileBuilder().Build();
      var packageTwo = new NuGetPackageFileBuilder().Build();

      // Act
      var actualResult = packageOne.Equals(packageTwo);

      // Assert
      Assert.IsTrue(actualResult);
    }

    [Test]
    public void Equals_TwoObjectsWithDifferentNameParameters_AreNotEqual()
    {
      // Arrange
      var packageOne = new NuGetPackageFileBuilder { PackageName = "One" }.Build();
      var packageTwo = new NuGetPackageFileBuilder { PackageName = "Two" }.Build();

      // Act
      var actualResult = packageOne.Equals(packageTwo);

      // Assert
      Assert.IsFalse(actualResult);
    }

    [Test]
    public void Equals_TwoObjectsWithDifferentVersionParameters_AreNotEqual()
    {
      // Arrange
      var packageOne = new NuGetPackageFileBuilder { Version = "One" }.Build();
      var packageTwo = new NuGetPackageFileBuilder { Version = "Two" }.Build();

      // Act
      var actualResult = packageOne.Equals(packageTwo);

      // Assert
      Assert.IsFalse(actualResult);
    }

    [Test]
    public void Equals_TwoObjectsWithDifferentFrameworkParameters_AreNotEqual()
    {
      // Arrange
      var packageOne = new NuGetPackageFileBuilder { Framework = "One" }.Build();
      var packageTwo = new NuGetPackageFileBuilder { Framework = "Two" }.Build();

      // Act
      var actualResult = packageOne.Equals(packageTwo);

      // Assert
      Assert.IsFalse(actualResult);
    }

    [Test]
    public void Equals_TwoObjectsWhereFrameworkIsNullOnOne_AreNotEqual()
    {
      // Arrange
      var packageOne = new NuGetPackageFileBuilder { Framework = null }.Build();
      var packageTwo = new NuGetPackageFileBuilder { Framework = "Two" }.Build();

      // Act
      var actualResult = packageOne.Equals(packageTwo);

      // Assert
      Assert.IsFalse(actualResult);
    }

    [Test]
    public void Equals_TwoObjectsWithDifferentFilePathParameters_AreNotEqual()
    {
      // Arrange
      var packageOne = new NuGetPackageFileBuilder { FilePath = "One" }.Build();
      var packageTwo = new NuGetPackageFileBuilder { FilePath = "Two" }.Build();

      // Act
      var actualResult = packageOne.Equals(packageTwo);

      // Assert
      Assert.IsFalse(actualResult);
    }

    [Test]
    public void Equals_OneObjectIsNull_AreNotEqual()
    {
      // Arrange
      var packageOne = new NuGetPackageFileBuilder().Build();
      NuGetPackageFile packageTwo = null;

      // Act
      var actualResult = packageOne.Equals(packageTwo);

      // Assert
      Assert.IsFalse(actualResult);
    }
  }
}
