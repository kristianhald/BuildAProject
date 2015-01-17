using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace BuildAProject.BuildManagement.NuGet.Configurations
{
  public class HierachicalNuGetConfigFileReader : INuGetConfigFileReader
  {
    public const string NuGetConfigFileName = "nuget.config";

    public NuGetConfig Read(string filePath)
    {
      var configurationsFound = FindConfigurationFilePaths(filePath);
      var nugetConfiguration = ReadConfiguration(configurationsFound);

      return nugetConfiguration;
    }

    private static IEnumerable<string> FindConfigurationFilePaths(string filePath)
    {
      var configurationsFound = new Stack<string>();

      var directory = Path.GetFullPath(filePath);

      while (true)
      {
        var possibleNuGetConfigFilePath = Path.Combine(directory, NuGetConfigFileName);
        if (File.Exists(possibleNuGetConfigFilePath))
          configurationsFound.Push(possibleNuGetConfigFilePath);

        var directoryInfo = Directory.GetParent(directory);
        if (directoryInfo == null)
          break;

        directory = directoryInfo.FullName;
      }

      // Adding the global configuration path
      var globalNugetConfigurationFilePath = Path.GetFullPath(@"%AppData%\NuGet\nuget.config");
      if (File.Exists(globalNugetConfigurationFilePath))
        configurationsFound.Push(globalNugetConfigurationFilePath);

      return configurationsFound;
    }

    private NuGetConfig ReadConfiguration(IEnumerable<string> configurationsFound)
    {
      // This is an assumption as it should have been ${solutiondir}\packages, but I do not want to search for the sln file currently.
      var repositoryPath = @"..\packages";
      var packageSources = new List<string>();
      var activePackageSources = new List<string>();

      foreach (var configurationFilePath in configurationsFound)
      {
        var configurationXml = XDocument.Load(configurationFilePath);

        var repositoryPathElement = configurationXml.XPathSelectElement(@"/configuration/config/add[@key='repositorypath']");
        if (repositoryPathElement != null)
          repositoryPath = GetRepositoryPath(repositoryPathElement, configurationFilePath);

        var packageSourceElement = configurationXml.XPathSelectElement(@"/configuration/packageSources");
        ParsePackageSources(packageSourceElement, configurationFilePath, packageSources);

        var activePackageSourceElement = configurationXml.XPathSelectElement(@"/configuration/activePackageSource");
        ParsePackageSources(activePackageSourceElement, configurationFilePath, activePackageSources);
      }

      var configPackageSources = packageSources as IEnumerable<string>;
      if (activePackageSources.Any())
        configPackageSources = packageSources.Intersect(activePackageSources);

      return new NuGetConfig(repositoryPath, configPackageSources);
    }

    private static string GetRepositoryPath(XElement repositoryPathElement, string configurationFilePath)
    {
      var repositoryPath = repositoryPathElement.Attribute("value").Value;
      if (IsPathRelative(repositoryPath))
        repositoryPath = GetAbsolutePathFromConfigurationFile(configurationFilePath, repositoryPath);

      return repositoryPath;
    }

    private static void ParsePackageSources(XElement packageSourceElement, string configurationFilePath, List<string> packageSources)
    {
      var newPackageSources = new List<string>();
      foreach (var childElement in packageSourceElement.Elements())
      {
        switch (childElement.Name.LocalName)
        {
          case "clear":
            packageSources.Clear();
            newPackageSources.Clear();
            break;

          case "add":
            var packageSource = childElement.Attribute("value").Value;
            if (IsPathRelative(packageSource))
              packageSource = GetAbsolutePathFromConfigurationFile(configurationFilePath, packageSource);

            newPackageSources.Add(packageSource);
            break;

          default:
            throw new Exception(
              String.Format(
                "The element '{0}' cannot be parsed. Either this tool is missing functionality or the file is in a bad format.",
                childElement.Name.LocalName));
        }
      }

      for (var packageIndex = 0; packageIndex < newPackageSources.Count; packageIndex++)
        packageSources.Insert(packageIndex, newPackageSources[packageIndex]);
    }

    private static bool IsPathRelative(string packageSource)
    {
      return !Path.IsPathRooted(packageSource) && !Uri.IsWellFormedUriString(packageSource, UriKind.Absolute);
    }

    private static string GetAbsolutePathFromConfigurationFile(string configurationFilePath, string relativePath)
    {
      return Path.GetFullPath(Path.Combine(Directory.GetParent(configurationFilePath).FullName, relativePath));
    }
  }
}
