using System;
using System.Collections.Generic;
using System.IO;
using BuildAProject.BuildManagement.Locators.FileSystem;

namespace BuildAProject.BuildManagement.Test.TestSupport.FileSystems
{
  sealed class EmbeddedResourceFileSystem : ILocatorFileSystem
  {
    private readonly TestFileResourceProvider resourceProvider;

    public EmbeddedResourceFileSystem(TestFileResourceProvider resourceProvider)
    {
      if (resourceProvider == null)
      {
        throw new ArgumentNullException("resourceProvider");
      }

      this.resourceProvider = resourceProvider;
    }

    public Stream CreateFileStream(string filePath)
    {
      return TestFileResourceProvider.CreateResourceStream(filePath);
    }

    public IEnumerable<string> GetAllFilenames(string rootDirectory)
    {
      return resourceProvider.GetPathsToAllResources();
    }
  }
}
