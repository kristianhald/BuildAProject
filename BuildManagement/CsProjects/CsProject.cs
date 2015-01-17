using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.CsProjects.GAC;
using BuildAProject.BuildManagement.CsProjects.SearchCriteria;
using BuildAProject.BuildManagement.Locators.FileSystem;
using Microsoft.Build.Evaluation;

[assembly: InternalsVisibleTo("BuildAProject.BuildManagement.Test")]

namespace BuildAProject.BuildManagement.CsProjects
{
  public class CsProject : IProject, IEquatable<CsProject>
  {
    public CsProject(Stream projectFile, string filePath, ILocatorFileSystem locatorFileSystem)
    {
      if (projectFile == null)
      {
        throw new ArgumentNullException("projectFile");
      }

      if (String.IsNullOrWhiteSpace(filePath))
      {
        throw new ArgumentNullException("filePath");
      }

      if (locatorFileSystem == null)
      {
        throw new ArgumentNullException("locatorFileSystem");
      }

      FilePath = filePath;

      ReadProjectFile(projectFile, filePath, locatorFileSystem);
    }

    internal CsProject(string name, IEnumerable<Reference> references, string outputType)
    {
      if (String.IsNullOrWhiteSpace(name))
      {
        throw new ArgumentNullException("name");
      }

      if (references == null)
      {
        throw new ArgumentNullException("references");
      }

      if (String.IsNullOrWhiteSpace(outputType))
      {
        throw new ArgumentNullException("outputType");
      }

      Name = name;
      References = references;
      OutputType = outputType;
    }

    private void ReadProjectFile(Stream projectFile, string filePath, ILocatorFileSystem locatorFileSystem)
    {
      var projectCollection = ProjectCollection.GlobalProjectCollection;
      var project = projectCollection.LoadProject(new XmlTextReader(projectFile));

      Name = project.GetPropertyValue("AssemblyName");
      OutputType = project.GetPropertyValue("OutputType");

      var referenceCollection = new List<Reference>();

      var references = project.GetItems("Reference");
      foreach (var reference in references)
      {
        string response;
        if (GacAssemblyLookup.AssemblyExist(reference.EvaluatedInclude, out response))
          continue;

        var hintPath = reference
          .Metadata
          .FirstOrDefault(metaData => metaData.Name == "HintPath");
        referenceCollection.Add(new Reference(reference.EvaluatedInclude, hintPath == null ? "" : hintPath.EvaluatedValue));
      }

      var csProjectFileCriteria = new CsProjectFileCriteria(locatorFileSystem);

      var projectReferences = project.GetItems("ProjectReference");
      foreach (var projectReference in projectReferences)
      {
        var projectReferenceLocation = projectReference.EvaluatedInclude;
        if (!Path.IsPathRooted(projectReferenceLocation))
        {
          var currentProjectPath = Path.GetDirectoryName(filePath);
          projectReferenceLocation = Path.Combine(currentProjectPath, projectReferenceLocation);
          projectReferenceLocation = projectReferenceLocation.Replace(@"\.\", @"\");
        }

        if (!csProjectFileCriteria.DoesSupport(projectReferenceLocation))
          continue;

        using (var projectReferenceStream = locatorFileSystem.CreateFileStream(projectReferenceLocation))
        {
          var referenceProject = new CsProject(projectReferenceStream, projectReferenceLocation, locatorFileSystem);
          referenceCollection.Add(new Reference(referenceProject.Name, ""));
        }
      }

      References = referenceCollection;
    }

    public string Name { get; private set; }

    public IEnumerable<Reference> References { get; private set; }

    public IEnumerable<Dependency> Dependencies
    {
      get
      {
        return References.Select(reference => new Dependency(reference.Name));
      }
    }

    public string FilePath { get; private set; }

    public string OutputType { get; private set; }

    public bool Equals(CsProject other)
    {
      return
        other != null &&
        Name.Equals(other.Name) &&
        Dependencies.SequenceEqual(other.Dependencies) &&
        OutputType.Equals(other.OutputType);
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as CsProject;
      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return
          Name.GetHashCode() +
          Dependencies.Count().GetHashCode() +
          OutputType.GetHashCode();
      }
    }

    public override string ToString()
    {
      return String.Format(
        "CsProject Name: {0}; OutputType: {1}; Dependencies: {2}",
        Name,
        OutputType,
        Environment.NewLine + String.Join(Environment.NewLine, Dependencies.Select(dependency => dependency.ToString())));
    }
  }
}
