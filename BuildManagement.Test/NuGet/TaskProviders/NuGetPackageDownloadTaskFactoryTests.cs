using System;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.NuGet.TaskProviders
{
  [TestFixture]
  public sealed class NuGetPackageDownloadTaskFactoryTests
  {
    [Test]
    public void NuGetPackageDownloadTaskFactory_NuGetDownloaderParameterIsNull_ThrowsError()
    {
      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => new NuGetPackageDownloadTaskFactoryBuilder { NuGetDownloader = null }.Build());
    }

    [Test]
    public void Create_PackageParameterIsNull_ThrowsError()
    {
      // Arrange
      var factory = new NuGetPackageDownloadTaskFactoryBuilder().Build();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => factory.Create(null));
    }

    [Test]
    public void Create_TaskIsCreated_WithPackage()
    {
      // Arrange
      var package = new NuGetPackageFileBuilder().Build();
      var expectedResult = new NuGetPackageDownloadTaskBuilder { Package = package }.Build();

      var factory = new NuGetPackageDownloadTaskFactoryBuilder().Build();

      // Act
      var actualResult = factory.Create(package);

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }
  }
}
