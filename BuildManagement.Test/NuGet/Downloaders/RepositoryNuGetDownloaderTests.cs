using System;
using System.Collections.Generic;
using System.IO;
using BuildAProject.BuildManagement.NuGet;
using BuildAProject.BuildManagement.NuGet.Configurations;
using BuildAProject.BuildManagement.NuGet.Downloaders;
using BuildAProject.BuildManagement.Test.TestSupport;
using BuildAProject.BuildManagement.Test.TestSupport.Settings;
using BuildAProject.BuildManagement.Test.TestSupport.Tools;
using Moq;
using NuGet;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.NuGet.Downloaders
{
  [TestFixture]
  [Category("IntegrationTest")]
  public sealed class RepositoryNuGetDownloaderTests
  {
    private TestDirectory localDirectory;

    [SetUp]
    public void SetUp()
    {
      localDirectory = TestSettings.GetTempPath(this);
    }

    [TearDown]
    public void TearDown()
    {
      localDirectory.Dispose();
    }

    [Test]
    public void RepositoryNuGetDownloader_RepositoriesFactoryParameterIsNull_ThrowsError()
    {
      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => new RepositoryNuGetDownloader(null, new Mock<INuGetConfigFileReader>().Object));
    }

    [Test]
    public void RepositoryNuGetDownloader_ConfigFileReaderParameterIsNull_ThrowsError()
    {
      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => new RepositoryNuGetDownloader(new Mock<INuGetRepositoriesFactory>().Object, null));
    }

    [Test]
    public void Download_PackageFileParameterIsNull_ThrowsError()
    {
      // Arrange
      Directory.CreateDirectory(localDirectory.Value);

      var downloader = new RepositoryNuGetDownloader(new Mock<INuGetRepositoriesFactory>().Object,
        new Mock<INuGetConfigFileReader>().Object);

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => downloader.Download(null));
    }

    [Test]
    public void Download_ProvidedPackageFoundAndPut_IntoSpecifiedFolder()
    {
      // Arrange
      var localRepositoryDirectory = Path.Combine(localDirectory.Value, "Packages");
      SetupLocalPackageRepositoryFolder(localRepositoryDirectory);

      var packageFile = new NuGetPackageFile(localDirectory.Value, "Moq", "4.2.1312.1622", "net45");
      var localRepository = new LocalPackageRepository(localRepositoryDirectory);

      var nugetConfig = new NuGetConfig(localDirectory.Value, new[] { "NotUsed" });
      var stubConfigFileReader = BuildStubConfigFileReader(packageFile, nugetConfig);
      var stubRepositoriesFactory = BuildStubRepositoriesFactory(nugetConfig, new[] { localRepository });

      var downloader = new RepositoryNuGetDownloader(stubRepositoriesFactory, stubConfigFileReader);

      // Act
      downloader.Download(packageFile);

      // Assert
      var expectedFilePath = Path.Combine(localDirectory.Value, "Moq.4.2.1312.1622", "lib", "net40", "moq.dll");
      Assert.IsTrue(File.Exists(expectedFilePath), "File could not be found at {0}", expectedFilePath);
    }

    [Test]
    public void Download_ProvidedPackageFoundInSecondRepositoryAndPut_IntoSpecifiedFolder()
    {
      // Arrange
      var localPackageDirectoryWithFile = Path.Combine(localDirectory.Value, "Packages");
      SetupLocalPackageRepositoryFolder(localPackageDirectoryWithFile);
      var localRepositoryWithFile = new LocalPackageRepository(localPackageDirectoryWithFile);

      var packageFile = new NuGetPackageFile(localDirectory.Value, "Moq", "4.2.1312.1622", "net45");
      var localPackageDirectoryWithoutFile = Path.Combine(localDirectory.Value, "OtherPackages");
      var localRepositoryWithoutFile = new LocalPackageRepository(localPackageDirectoryWithoutFile);

      var nugetConfig = new NuGetConfig(localDirectory.Value, new[] { "NotUsed" });
      var stubConfigFileReader = BuildStubConfigFileReader(packageFile, nugetConfig);
      var stubRepositoriesFactory = BuildStubRepositoriesFactory(nugetConfig, new[] { localRepositoryWithoutFile, localRepositoryWithFile });

      var downloader = new RepositoryNuGetDownloader(stubRepositoriesFactory, stubConfigFileReader);

      // Act
      downloader.Download(packageFile);

      // Assert
      var expectedFilePath = Path.Combine(localDirectory.Value, "Moq.4.2.1312.1622", "lib", "net40", "moq.dll");
      Assert.IsTrue(File.Exists(expectedFilePath), "File could not be found at {0}", expectedFilePath);
    }

    [Test]
    public void Download_ProvidedPackageCouldNotBeFound_ThrowsError()
    {
      // Arrange
      Directory.CreateDirectory(localDirectory.Value);

      var packageFile = new NuGetPackageFile(localDirectory.Value, "DoesNotExist", "4.2.1", "net45");

      var nugetConfig = new NuGetConfig(localDirectory.Value, new[] { "NotUsed" });
      var stubConfigFileReader = BuildStubConfigFileReader(packageFile, nugetConfig);
      var stubRepositoriesFactory = BuildStubRepositoriesFactory(nugetConfig, new[] { new LocalPackageRepository(localDirectory.Value) });

      var downloader = new RepositoryNuGetDownloader(stubRepositoriesFactory, stubConfigFileReader);

      // Act + Assert
      Assert.Throws<InvalidOperationException>(() => downloader.Download(packageFile));
    }

    /// <summary>
    /// Sets up the package environment for the integration test
    /// </summary>
    private void SetupLocalPackageRepositoryFolder(string testPackageRepositoryPath)
    {
      Directory.CreateDirectory(testPackageRepositoryPath);

      using (
        var nugetPackageZipFileStream =
          TestFileResourceProvider.CreateResourceStream(TestFileResourceProvider.NuGetPackageRepository))
      {
        UnzipTool.UnzipStream(nugetPackageZipFileStream, testPackageRepositoryPath);
      }
    }

    /// <summary>
    /// Creates a nuget repositories factory stub with the provided repositories
    /// </summary>
    private static INuGetRepositoriesFactory BuildStubRepositoriesFactory(
      NuGetConfig nuGetConfig,
      IEnumerable<LocalPackageRepository> repositories)
    {
      var stubRepositoriesFactory = new Mock<INuGetRepositoriesFactory>();
      stubRepositoriesFactory
        .Setup(factory => factory.Create(nuGetConfig))
        .Returns(repositories);

      return stubRepositoriesFactory.Object;
    }

    private static INuGetConfigFileReader BuildStubConfigFileReader(NuGetPackageFile packageFile, NuGetConfig nugetConfig)
    {
      var stubConfigFileReader = new Mock<INuGetConfigFileReader>();
      stubConfigFileReader
        .Setup(reader => reader.Read(packageFile.FilePath))
        .Returns(nugetConfig);

      return stubConfigFileReader.Object;
    }
  }
}
