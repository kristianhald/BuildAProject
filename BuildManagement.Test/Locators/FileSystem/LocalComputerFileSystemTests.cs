using System;
using System.IO;
using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.Test.TestSupport.Settings;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.Locators.FileSystem
{
  [TestFixture]
  public sealed class LocalComputerFileSystemTests
  {
    private static readonly TestDirectory TestDirectory = TestSettings.GetTempPath(new LocalComputerFileSystemTests());

    [TestFixtureSetUp]
    public void AllTestsSetup()
    {
      Directory.CreateDirectory(TestDirectory.Value);
    }

    [TestFixtureTearDown]
    public void AllTestsTearDown()
    {
      TestDirectory.Dispose();
    }

    [Test]
    public void CreateFileStream_FilePathParameterIsNull_ThrowsError()
    {
      // Arrange
      var fileSystem = new LocalComputerFileSystem();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => fileSystem.CreateFileStream(null));
    }

    [Test]
    [Category("IntegrationTest")]
    public void CreateFileStream_FilePathDoesNotContainAFile_ThrowsError()
    {
      // Arrange
      var fileSystem = new LocalComputerFileSystem();
      var notExistantFilePath = Path.Combine(TestDirectory.Value, "FileDoesNotExist.Not");

      // Act + Assert
      Assert.Throws<FileNotFoundException>(() => fileSystem.CreateFileStream(notExistantFilePath));
    }

    [Test]
    [Category("IntegrationTest")]
    public void CreateFileStream_FileIsFound_AndStreamIsCreated()
    {
      // Arrange
      const string expectedResult = "This text String is expected";

      var testFile = Path.Combine(TestDirectory.Value, "test.txt");
      using (var outputStream = File.CreateText(testFile))
      {
        outputStream.WriteLine(expectedResult);
      }

      var fileSystem = new LocalComputerFileSystem();

      // Act
      string actualResult;
      using (var foundStream = fileSystem.CreateFileStream(testFile))
      {
        actualResult = new StreamReader(foundStream).ReadToEnd().TrimEnd('\r', '\n');
      }

      // Assert
      Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    [Category("IntegrationTest")]
    public void GetAllFilenames_FilePathProvidedContainsFiles_AndTheirNamesAreReturned()
    {
      // Arrange
      var allFilesDirectory = Path.GetFullPath(Path.Combine(TestDirectory.Value, "GetAllFilenames"));
      Directory.CreateDirectory(allFilesDirectory);
      Directory.CreateDirectory(Path.Combine(allFilesDirectory, "ExtraDirectory"));

      var expectedResult = new[]
                           {
                             Path.Combine(allFilesDirectory, "File1.txt"),
                             Path.Combine(allFilesDirectory, "ExtraDirectory", "File2.txt")
                           };

      File.AppendAllText(Path.Combine(allFilesDirectory, "File1.txt"), "SomeText");
      File.AppendAllText(Path.Combine(allFilesDirectory, "ExtraDirectory", "File2.txt"), "AnotherText");

      var fileSystem = new LocalComputerFileSystem();

      // Act
      var actualResult = fileSystem.GetAllFilenames(allFilesDirectory);

      // Assert
      CollectionAssert.AreEquivalent(expectedResult, actualResult, "The root directory used was {0}.", allFilesDirectory);
    }

    [Test]
    public void GetAllFilenames_RootDirectoryParameterIsNull_ThrowsError()
    {
      // Arrange
      var fileSystem = new LocalComputerFileSystem();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => fileSystem.GetAllFilenames(null));
    }
  }
}
