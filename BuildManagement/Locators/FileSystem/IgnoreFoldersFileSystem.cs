using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BuildAProject.BuildManagement.Locators.FileSystem
{
  public class IgnoreFoldersFileSystem : ILocatorFileSystem
  {
    private const string IgnoreFilename = "ignorefile";

    private readonly ILocatorFileSystem fileSystem;

    public IgnoreFoldersFileSystem(ILocatorFileSystem fileSystem)
    {
      if (fileSystem == null)
      {
        throw new ArgumentNullException("fileSystem");
      }

      this.fileSystem = fileSystem;
    }

    public Stream CreateFileStream(string filePath)
    {
      return fileSystem.CreateFileStream(filePath);
    }

    public IEnumerable<string> GetAllFilenames(string rootDirectory)
    {
      var allFilenames = fileSystem
        .GetAllFilenames(rootDirectory);

      var ignoreFiles = GetIgnoreFiles(allFilenames);

      var filteredFilenames = allFilenames;
      foreach (var ignoreFilename in ignoreFiles)
      {
        var ignoreFileBaseDirectory = Path.GetDirectoryName(ignoreFilename);
        using (var ignoreFileStream = new StreamReader(CreateFileStream(ignoreFilename)))
        {
          string ignoreLine;
          while ((ignoreLine = ignoreFileStream.ReadLine()) != null)
          {
            var ignorePath = Path.Combine(ignoreFileBaseDirectory, Path.GetDirectoryName(ignoreLine) + "\\");

            filteredFilenames = FilterFilenames(filteredFilenames, ignorePath);
          }
        }
      }

      return filteredFilenames;
    }

    private static IEnumerable<string> FilterFilenames(IEnumerable<string> filteredFilenames, string ignorePath)
    {
      var tmpResult = new List<string>();
      foreach (var filteredFileName in filteredFilenames)
      {
        if (filteredFileName.StartsWith(ignorePath))
          continue;

        tmpResult.Add(filteredFileName);
      }

      filteredFilenames = tmpResult;
      return filteredFilenames;
    }

    private static IEnumerable<string> GetIgnoreFiles(IEnumerable<string> allFilenames)
    {
      var ignoreFiles = allFilenames
        .Where(filename => Path.GetFileName(filename).Equals(IgnoreFilename));
      return ignoreFiles;
    }
  }
}
