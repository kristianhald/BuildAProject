using System;
using System.Linq;
using BuildAProject.BuildManagement.NuGet.Configurations;
using BuildAProject.BuildManagement.NuGet.Downloaders;
using Moq;
using NuGet;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.NuGet.Downloaders
{
  [TestFixture]
  public sealed class NuGetPackageScannerRepositoryFactoryTests
  {
    [Test]
    public void NuGetPackageScannerRepositoryFactory_PackageRepositoryFactoryParameterIsNull_ThrowsError()
    {
      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => new NuGetPackageScannerRepositoryFactory(null));
    }

    [Test]
    public void Create_NuGetPackageFileParameterIsNull_ThrowsError()
    {
      // Arrange
      var stubPackageFactory = new Mock<IPackageRepositoryFactory>();
      var factory = new NuGetPackageScannerRepositoryFactory(stubPackageFactory.Object);

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => factory.Create(null));
    }

    [Test]
    public void Create_SingleRepositoryIsFound_ReturnsSingleRepository()
    {
      // Arrange
      var expectedResult = new[]
                           {
                             new Mock<IPackageRepository>().Object
                           };
      const string packageSourceFound = "ThisRepositoryWasFound";

      var stubPackageFactory = new Mock<IPackageRepositoryFactory>();
      stubPackageFactory
        .Setup(packageFactory => packageFactory.CreateRepository(packageSourceFound))
        .Returns(expectedResult.Single());

      var factory = new NuGetPackageScannerRepositoryFactory(stubPackageFactory.Object);

      // Act
      var actualResult = factory.Create(new NuGetConfig("..\\packages", new[] { packageSourceFound }));

      // Assert
      CollectionAssert.AreEquivalent(expectedResult, actualResult);
    }
  }
}
