using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BuildAProject.BuildManagement.Locators.FileSystem;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.Locators.FileSystem
{
  [TestFixture]
  public sealed class IgnoreFoldersFileSystemTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void IgnoreFoldersFileSystem_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(IgnoreFoldersFileSystem).GetConstructors());
    }

    [Test]
    public void CreateFileStream_FilePathParameterPassedToWrappedFileSystem_AndStreamIsReturned()
    {
      // Arrange
      const string filePath = ".";
      var expectedStream = new MemoryStream(0);

      var stubFileSystem = mockRepository.Create<ILocatorFileSystem>();
      stubFileSystem
        .Setup(wrappedFileSystem => wrappedFileSystem.CreateFileStream(filePath))
        .Returns(expectedStream);

      var fileSystem = new IgnoreFoldersFileSystem(stubFileSystem.Object);

      // Act
      var actualStream = fileSystem.CreateFileStream(filePath);

      // Assert
      Assert.AreSame(expectedStream, actualStream);
    }

    [Test]
    public void GetAllFilenames_GivenThereAreNotAnyIgnoredFilesEncountered_ThenAllFilenamesAreReturned()
    {
      // Arrange
      const string filePath = ".";
      var expectedFilenames = new[]
                              {
                                "./first/folder",
                                "./second/folder",
                                "./first/sub/folder"
                              };

      var stubFileSystem = mockRepository.Create<ILocatorFileSystem>();
      stubFileSystem
        .Setup(wrappedFileSystem => wrappedFileSystem.GetAllFilenames(filePath))
        .Returns(expectedFilenames);

      var fileSystem = new IgnoreFoldersFileSystem(stubFileSystem.Object);

      // Act
      var actualFilenames = fileSystem.GetAllFilenames(filePath);

      // Assert
      CollectionAssert.AreEquivalent(expectedFilenames, actualFilenames);
    }

    [Test]
    public void GetAllFilenames_GivenThereAreIgnoreFiles_ThenOnlyUnignoredFilenamesAreReturned()
    {
      const string filePath = ".";
      var expectedFilenames = new[]
                              {
                                ".\\first\\folder",
                                ".\\second\\folder",
                                ".\\first\\ignorefile",
                                ".\\second\\ignored\\not\\folder",
                              };

      var fileSystemFilenames = new List<string>(expectedFilenames)
                                {
                                  ".\\first\\ignored\\folder",
                                  ".\\first\\another\\ignored\\folder",
                                };

      var stubFileSystem = mockRepository.Create<ILocatorFileSystem>();
      stubFileSystem
        .Setup(wrappedFileSystem => wrappedFileSystem.GetAllFilenames(filePath))
        .Returns(fileSystemFilenames);
      stubFileSystem
        .Setup(wrappedFileSystem => wrappedFileSystem.CreateFileStream(".\\first\\ignorefile"))
        .Returns(new MemoryStream(ASCIIEncoding.ASCII.GetBytes(@"ignored\\" + Environment.NewLine + "another\\ignored\\")));

      var fileSystem = new IgnoreFoldersFileSystem(stubFileSystem.Object);

      // Act
      var actualFilenames = fileSystem.GetAllFilenames(filePath);

      // Assert
      CollectionAssert.AreEquivalent(expectedFilenames, actualFilenames);
    }

    [Test]
    public void GetAllFilesnames_GivenAnIgnoredFolderWithAnotherResemblingIt_ThenOnlyTheIgnoredFolderIsIgnored()
    {
      const string filePath = ".";
      var expectedFilenames = new[]
                              {
                                ".\\basewithsomemore\\folder",
                                ".\\ignorefile",
                              };

      var fileSystemFilenames = new List<string>(expectedFilenames)
                                {
                                  ".\\base\\ignored\\folder",
                                };

      var stubFileSystem = mockRepository.Create<ILocatorFileSystem>();
      stubFileSystem
        .Setup(wrappedFileSystem => wrappedFileSystem.GetAllFilenames(filePath))
        .Returns(fileSystemFilenames);
      stubFileSystem
        .Setup(wrappedFileSystem => wrappedFileSystem.CreateFileStream(".\\ignorefile"))
        .Returns(new MemoryStream(ASCIIEncoding.ASCII.GetBytes(@"base\\")));

      var fileSystem = new IgnoreFoldersFileSystem(stubFileSystem.Object);

      // Act
      var actualFilenames = fileSystem.GetAllFilenames(filePath);

      // Assert
      CollectionAssert.AreEquivalent(expectedFilenames, actualFilenames);
    }
  }
}
