using System.Collections.Generic;
using System.Text.RegularExpressions;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.Locators.SearchCriteria.Templates;

namespace BuildAProject.BuildManagement.DLLs.SearchCriteria
{
  public class DllFileCriteria : FileNameCriteria
  {
    public DllFileCriteria()
      : base(new Regex(@"^.*\.dll$", RegexOptions.Compiled | RegexOptions.IgnoreCase))
    {
    }

    protected override IEnumerable<IProject> CreateProjectFromFilePath(string filePath)
    {
      return new[] { new DllFileProject(filePath) };
    }
  }
}
