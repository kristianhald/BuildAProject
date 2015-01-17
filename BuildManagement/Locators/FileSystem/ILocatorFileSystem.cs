using System.Collections.Generic;
using System.IO;

namespace BuildAProject.BuildManagement.Locators.FileSystem
{
  public interface ILocatorFileSystem
  {
    /// <summary>
    /// Opens the file at the provided file path and creates a stream
    /// </summary>
    Stream CreateFileStream(string filePath);

    /// <summary>
    /// Returns all the filenames in the provided directory and all
    /// its subdirectories
    /// </summary>
    IEnumerable<string> GetAllFilenames(string rootDirectory);
  }
}
