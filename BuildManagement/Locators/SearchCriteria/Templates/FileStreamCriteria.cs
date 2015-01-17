using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.Locators.FileSystem;

namespace BuildAProject.BuildManagement.Locators.SearchCriteria.Templates
{
  public abstract class FileStreamCriteria : FileNameCriteria
  {
    protected readonly ILocatorFileSystem LocatorFileSystem;

    protected FileStreamCriteria(Regex filenameMatchRegex, ILocatorFileSystem locatorFileSystem)
      : base(filenameMatchRegex)
    {
      if (locatorFileSystem == null)
      {
        throw new ArgumentNullException("locatorFileSystem");
      }

      LocatorFileSystem = locatorFileSystem;
    }

    protected override IEnumerable<IProject> CreateProjectFromFilePath(string filePath)
    {
      using (var fileStream = LocatorFileSystem.CreateFileStream(filePath))
      {
        if (fileStream == null)
        {
          throw new Exception(String.Format("The file path '{0}' did not contain a stream that could be opened.", filePath));
        }

        return CreateProjectFromStream(fileStream, filePath);
      }
    }

    protected abstract IEnumerable<IProject> CreateProjectFromStream(Stream projectStream, string filePath);
  }
}
