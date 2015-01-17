using System;
using BuildAProject.BuildManagement.NuGet.Downloaders;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using Moq;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.NuGet
{
  [TestFixture]
  public sealed class NuGetPackageDownloadTaskTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void NuGetPackageDownloadTask_PackageParameterIsNull_ThrowsError()
    {
      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => new NuGetPackageDownloadTaskBuilder { Package = null }.Build());
    }

    [Test]
    public void NuGetPackageDownloadTask_NuGetDownloaderParameterIsNull_ThrowsError()
    {
      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => new NuGetPackageDownloadTaskBuilder { Downloader = null }.Build());
    }

    [Test]
    public void Execute_Package_IsProvidedToCompiler()
    {
      // Arrange
      var mockNuGetDownloader = mockRepository.Create<INuGetDownloader>();
      var taskBuilder = new NuGetPackageDownloadTaskBuilder
      {
        Downloader = mockNuGetDownloader.Object
      };
      var task = taskBuilder.Build();

      // Act
      task.Execute();

      // Assert
      mockNuGetDownloader.Verify(downloader => downloader.Download(taskBuilder.Package));
    }

    [Test]
    public void Equals_TwoIdenticalTasks_AreEqual()
    {
      // Arrange
      var taskOne = new NuGetPackageDownloadTaskBuilder().Build();
      var taskTwo = new NuGetPackageDownloadTaskBuilder().Build();

      // Act + Assert
      Assert.IsTrue(taskOne.Equals(taskTwo));
    }

    [Test]
    public void Equals_TwoTasksWithDifferentPackages_AreNotEqual()
    {
      // Arrange
      var taskOne = new NuGetPackageDownloadTaskBuilder { Package = new NuGetPackageFileBuilder { PackageName = "One" }.Build() }.Build();
      var taskTwo = new NuGetPackageDownloadTaskBuilder { Package = new NuGetPackageFileBuilder { PackageName = "Two" }.Build() }.Build();

      // Act + Assert
      Assert.IsFalse(taskOne.Equals(taskTwo));
    }

    [Test]
    public void Equals_OneTaskIsNull_AreNotEqual()
    {
      // Arrange
      var taskOne = new NuGetPackageDownloadTaskBuilder().Build();

      // Act + Assert
      Assert.IsFalse(taskOne.Equals(null));
    }

    [Test]
    public void Equals_TwoIdenticalTasksWhereOneIsCasted_AreEqual()
    {
      // Arrange
      var taskOne = new NuGetPackageDownloadTaskBuilder().Build();
      var taskTwo = new NuGetPackageDownloadTaskBuilder().Build();

      // Act + Assert
      Assert.IsTrue(taskOne.Equals((object)taskTwo));
    }

    [Test]
    public void Equals_TaskComparedToObject_AreNotEqual()
    {
      // Arrange
      var taskOne = new NuGetPackageDownloadTaskBuilder().Build();
      var someObject = new object();

      // Act + Assert
      Assert.IsFalse(taskOne.Equals(someObject));
    }
  }
}
