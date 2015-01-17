using System;
using System.IO;
using System.IO.Compression;

namespace BuildAProject.BuildManagement.Test.TestSupport.Tools
{
  static class UnzipTool
  {
    public static void UnzipStream(Stream zipStream, string outputDirectory)
    {
      if (zipStream == null)
      {
        throw new ArgumentNullException("zipStream");
      }

      if (!Directory.Exists(outputDirectory))
      {
        throw new ArgumentException(String.Format("Output directory '{0}' does not exist. Create it before unzipping.", outputDirectory));
      }

      var zipArchive = new ZipArchive(zipStream);
      foreach (var zipEntry in zipArchive.Entries)
      {
        var absolutePath = Path.Combine(outputDirectory, zipEntry.FullName);

        // If the zip entry does not contain any name then its a directory
        if (String.IsNullOrWhiteSpace(zipEntry.Name))
        {
          Directory.CreateDirectory(absolutePath);
        }
        else
        {
          zipEntry.ExtractToFile(absolutePath);
        }
      }
    }
  }
}
