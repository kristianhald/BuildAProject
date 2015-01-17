using System;
using System.Collections.Generic;
using System.IO;

namespace BuildAProject.BuildManagement.Locators.FileSystem
{
  public class LocalComputerFileSystem : ILocatorFileSystem
  {
    public Stream CreateFileStream(string filePath)
    {
      return File.OpenRead(filePath);
    }

    public IEnumerable<string> GetAllFilenames(string rootDirectory)
    {
      return Directory.GetFiles(rootDirectory, "*", SearchOption.AllDirectories);
    }
  }
}
