using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.Locators.SearchCriteria.Templates;

namespace BuildAProject.BuildManagement.NuGet.SearchCriteria
{
  public class NuGetPackageFileCriteria : FileStreamCriteria
  {
    public NuGetPackageFileCriteria(ILocatorFileSystem locatorFileSystem)
      : base(new Regex(@"^.*\\packages\.config$", RegexOptions.Compiled | RegexOptions.IgnoreCase), locatorFileSystem)
    {
    }

    protected override IEnumerable<IProject> CreateProjectFromStream(Stream projectStream, string filePath)
    {
      var packages = new List<IProject>();

      var packageDocument = XDocument.Load(projectStream);
      var packageElements = packageDocument.Descendants("package");
      foreach (var packageElement in packageElements)
      {
        var packageName = packageElement.Attribute("id").Value;
        var version = packageElement.Attribute("version").Value;

        string framework = null;
        var frameworkAttribute = packageElement.Attribute("targetFramework");
        if (frameworkAttribute != null)
          framework = frameworkAttribute.Value;

        packages.Add(new NuGetPackageFile(filePath, packageName, version, framework));
      }

      return packages;
    }
  }
}
