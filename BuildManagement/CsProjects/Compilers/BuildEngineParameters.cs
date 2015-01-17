using System;
using System.Collections.Generic;
using System.IO;

namespace BuildAProject.BuildManagement.CsProjects.Compilers
{
  public class BuildEngineParameters
  {
    /// <summary>
    /// Tells in what directory the compiled files must be placed if the specific
    /// project type directory property has not been set.
    /// If this is not set and neither the project specific for the project being 
    /// compiled then the directory in the project file will be used.
    /// </summary>
    public string GeneralOutputDirectory { get; set; }

    private string executableOutputDirectory;

    /// <summary>
    /// Tells in what directory the compiled executable files must be placed.
    /// If not set then the value from the general output directory property 
    /// will be used
    /// </summary>
    public string ExecutableOutputDirectory
    {
      get
      {
        return String.IsNullOrWhiteSpace(executableOutputDirectory)
          ? GeneralOutputDirectory
          : executableOutputDirectory;
      }
      set
      {
        executableOutputDirectory = value;
      }
    }

    private string libraryOutputDirectory;

    /// <summary>
    /// Tells in what directory the compiled library files must be placed.
    /// If not set then the value from the general output directory property 
    /// will be used
    /// </summary>
    public string LibraryOutputDirectory
    {
      get
      {
        return String.IsNullOrWhiteSpace(libraryOutputDirectory)
          ? GeneralOutputDirectory
          : libraryOutputDirectory;
      }
      set
      {
        libraryOutputDirectory = value;
      }
    }

    /// <summary>
    /// If set the compiled executable projects will be put in their own directories for 
    /// easy grouping between the projects.
    /// </summary>
    public bool ExecutablesInSeparateDirectories { get; set; }

    /// <summary>
    /// If set the compiled library projects will be put in their own directories for
    /// easy grouping between the projects. This solves the problem where multiple 
    /// projects use the same external dependency, but with different versions.
    /// If not set in that case, then only a single version of the dependency will be 
    /// used during runtime of the projects.
    /// </summary>
    public bool LibrariesInSeparateDirectories { get; set; }

    /// <summary>
    /// Tells which build tool version to use
    /// If not set, then the default defined in the project is selected
    /// </summary>
    public string ToolsVersion { get; set; }

    private readonly List<string> targetsToBuild = new List<string>();

    /// <summary>
    /// Tells which configuration in the project to use for the build
    /// If empty then the default defined in the project is selected
    /// </summary>
    public string Configuration { get; set; }

    /// <summary>
    /// Tells which platform to compile to
    /// If empty then the default defined in the project is selected
    /// </summary>
    public string Platform { get; set; }

    /// <summary>
    /// Tells which targets in the project to build
    /// If empty then the default defined in the project is selected
    /// </summary>
    public string[] TargetsToBuild { get { return targetsToBuild.ToArray(); } }

    /// <summary>
    /// Adds a target to build to the parameters
    /// </summary>
    public void AddTargetToBuild(string targetToBuild)
    {
      if (String.IsNullOrWhiteSpace(targetToBuild))
      {
        throw new ArgumentNullException("targetToBuild");
      }

      targetsToBuild.Add(targetToBuild);
    }

    /// <summary>
    /// Removes a target to build from the parameters
    /// </summary>
    public void RemoveTargetToBuild(string targetToBuild)
    {
      if (String.IsNullOrWhiteSpace(targetToBuild))
      {
        throw new ArgumentNullException("targetToBuild");
      }

      targetsToBuild.Remove(targetToBuild);
    }

    public string GetProjectCompilationDirectory(CsProject csProject)
    {
      if (csProject == null)
      {
        throw new ArgumentNullException("csProject");
      }

      switch (csProject.OutputType)
      {
        case "Library":
          return LibrariesInSeparateDirectories
            ? Path.Combine(LibraryOutputDirectory, csProject.Name)
            : LibraryOutputDirectory;

        case "Exe":
          return ExecutablesInSeparateDirectories
            ? Path.Combine(ExecutableOutputDirectory, csProject.Name)
            : ExecutableOutputDirectory;

        default:
          return GeneralOutputDirectory;
      }
    }

    public string GetProjectCompilationFile(CsProject csProject)
    {
      if (csProject == null)
      {
        throw new ArgumentNullException("csProject");
      }

      switch (csProject.OutputType)
      {
        case "Library":
          return csProject.Name + ".dll";

        case "Exe":
          return csProject.Name + ".exe";

        case "WinExe":
          return csProject.Name + ".exe";

        default:
          throw new Exception(String.Format("An unknown project output type was encountered '{0}' in project '{1}' with filepath '{2}'", csProject.OutputType, csProject.Name, csProject.FilePath));
      }
    }
  }
}
