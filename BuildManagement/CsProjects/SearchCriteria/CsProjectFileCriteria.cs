using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.Locators.SearchCriteria.Templates;
using Microsoft.Build.Exceptions;

namespace BuildAProject.BuildManagement.CsProjects.SearchCriteria
{
  public class CsProjectFileCriteria : FileStreamCriteria
  {
    public CsProjectFileCriteria(ILocatorFileSystem locatorFileSystem)
      : base(new Regex(@"^.*\.csproj$", RegexOptions.Compiled | RegexOptions.IgnoreCase), locatorFileSystem)
    {
    }

    protected override IEnumerable<IProject> CreateProjectFromStream(Stream projectStream, string filePath)
    {
      try
      {
        return new[] { new CsProject(projectStream, filePath, LocatorFileSystem) };
      }
      catch (InvalidProjectFileException)
      {
        return new CsProject[0];
      }
    }
  }
}
