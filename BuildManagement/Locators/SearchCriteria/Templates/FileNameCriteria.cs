using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.Locators.SearchCriteria.Templates
{
  public abstract class FileNameCriteria : IFileCriteria
  {
    private readonly Regex filenameMatchRegex;

    protected FileNameCriteria(Regex filenameMatchRegex)
    {
      if (filenameMatchRegex == null)
      {
        throw new ArgumentNullException("filenameMatchRegex");
      }

      this.filenameMatchRegex = filenameMatchRegex;
    }

    public bool DoesSupport(string filename)
    {
      return filenameMatchRegex.IsMatch(filename);
    }

    public IEnumerable<IProject> CreateProjectsFrom(string filePath)
    {
      if (String.IsNullOrWhiteSpace(filePath))
      {
        throw new ArgumentNullException("filePath");
      }

      if (!DoesSupport(filePath))
      {
        throw new ArgumentException(String.Format("The file path '{0}' is not supported by this criteria. Use 'DoesSupport' before calling this method.", filePath));
      }

      return CreateProjectFromFilePath(filePath);
    }

    protected abstract IEnumerable<IProject> CreateProjectFromFilePath(string filePath);
  }
}
