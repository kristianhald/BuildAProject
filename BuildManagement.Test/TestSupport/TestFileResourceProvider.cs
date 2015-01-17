using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BuildAProject.BuildManagement.Test.TestSupport
{
  sealed class TestFileResourceProvider : IDisposable
  {
    // CsProjects resources
    public const string AnotherWorldProjectResource = ".\\CsProjects\\ProjectFiles\\AnotherWorld.csproj";
    public const string HelloWorldProjectResource = ".\\CsProjects\\ProjectFiles\\HelloWorld.csproj";
    public const string UnsupportedProject = ".\\CsProjects\\ProjectFiles\\UnsupportProject.csproj";

    // Content Project resources
    public const string ContentProjectResource = ".\\ContentProjects\\ContentProject.contentproj";

    // NuGet Package resources
    public const string NuGetPackageResource = ".\\NuGet\\PackageFiles\\packages.config";

    // NuGet Package Library files
    public const string NuGetMoqLibrary = ".\\NuGet\\Dependencies\\Moq.dll";
    public const string NuGetNUnitLibrary = ".\\NuGet\\Dependencies\\NUnit.dll";
    public const string NuGetStructureMapLibrary = ".\\NuGet\\Dependencies\\StructureMap.dll";

    // NuGet Config Files
    public const string NuGetConfigWithOnlyOnePackageSource = ".\\NuGet\\ConfigFiles\\ConfigWithOnlyOnePackageSource.config";
    public const string NuGetConfigWithSeveralPackageSource = ".\\NuGet\\ConfigFiles\\ConfigWithSeveralPackageSource.config";
    public const string NuGetConfigWithSingleActivePackageSource = ".\\NuGet\\ConfigFiles\\ConfigWithSingleActivePackageSource.config";
    public const string NuGetConfigClearsInTheMiddleOfPackageSource = ".\\NuGet\\ConfigFiles\\ConfigClearsInTheMiddleOfPackageSource.config";
    public const string NuGetConfigWithSingleNonClearingPackageSource = ".\\NuGet\\ConfigFiles\\ConfigWithSingleNonClearingPackageSource.config";
    public const string NuGetConfigWithHttpPackageSource = ".\\NuGet\\ConfigFiles\\ConfigWithHttpPackageSource.config";

    // Zip Files
    public const string IntegrationTestBuildProjects = ".\\ZipFiles\\IntegrationTestBuildProject.zip";
    public const string NuGetPackageRepository = ".\\ZipFiles\\NuGetLocalRepository.zip";
    public const string CompileErrorProject = ".\\ZipFiles\\CompileErrorProject.zip";
    public const string NUnitTestProject = ".\\ZipFiles\\NUnitTestProject.zip";
    public const string BindingRedirectTest = ".\\ZipFiles\\BindingRedirectTest.zip";
    public const string ProjectLibraryWithNoDependencies = ".\\ZipFiles\\ProjectLibraryWithNoDependencies.zip";
    public const string ProjectExecutableWithNoDependencies = ".\\ZipFiles\\ProjectExecutableWithNoDependencies.zip";

    // OtherFiles resources
    public const string AppConfigResource = ".\\OtherFiles\\App.config";
    public const string HelloWorldSolutionResource = ".\\OtherFiles\\HelloWorld.sln";
    public const string HelloWorldLibraryClassResource = ".\\OtherFiles\\HelloWorldLibrary.cs";
    public const string ProgramClassResource = ".\\OtherFiles\\Program.cs";

    private readonly IDictionary<string, Stream> resourcesLoaded;
    private bool isDisposed = false;

    public TestFileResourceProvider()
    {
      var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Static);
      resourcesLoaded = fields
        .Select(field => field.GetValue(this))
        .OfType<string>()
        .ToDictionary(resourceName => resourceName, CreateResourceStream);
    }

    public Stream GetResourceStream(string nameOfResource)
    {
      if (!resourcesLoaded.ContainsKey(nameOfResource))
      {
        throw new ArgumentException(String.Format("The resource named '{0}' could not be found. Known resources are: {1}", nameOfResource, String.Join(", ", resourcesLoaded.Keys)));
      }

      return resourcesLoaded[nameOfResource];
    }

    public IEnumerable<string> GetPathsToAllResources()
    {
      return resourcesLoaded
        .Keys;
    }

    public static Stream CreateResourceStream(string nameOfResource)
    {
      var correctResourcePath = HandleChangeDirectory(nameOfResource);
      var correctResourceName = correctResourcePath.Replace("\\", ".").Trim(new[] { '.' });

      var assembly = Assembly.GetExecutingAssembly();
      var absoluteResourceLocation = typeof(TestFileResourceProvider).Namespace + "." + correctResourceName;

      var resourceStream = assembly.GetManifestResourceStream(absoluteResourceLocation);
      if (resourceStream != null)
        return resourceStream;

      var knownResources = assembly.GetManifestResourceNames();
      throw new ArgumentException(
        String.Format("The resource named '{0}' could not be found. Known resources are: {1}", absoluteResourceLocation, String.Join(", ", knownResources)));
    }

    private static string HandleChangeDirectory(string nameOfResource)
    {
      // ".\\CsProjects\\ProjectFiles\\..\\..\\ContentProjects\\ContentProject.contentproj"
      // It must be able to handle .. and go a directory down
      var correctResourcePath = nameOfResource;

      for (
        var subDirectoryIndex = correctResourcePath.IndexOf("\\..\\", StringComparison.InvariantCultureIgnoreCase);
        subDirectoryIndex != -1;
        subDirectoryIndex = correctResourcePath.IndexOf("\\..\\", StringComparison.InvariantCultureIgnoreCase))
      {
        var startOfSubDirectory = correctResourcePath
          .LastIndexOf("\\", subDirectoryIndex - 1, StringComparison.InvariantCultureIgnoreCase);

        correctResourcePath = correctResourcePath
          .Remove(startOfSubDirectory, (subDirectoryIndex - startOfSubDirectory) + 3);
      }

      return correctResourcePath;
    }

    public void Dispose()
    {
      if (!isDisposed)
      {
        // If you need thread safety, use a lock around these  
        // operations, as well as in your methods that use the resource. 
        // Free the necessary managed disposable objects. 
        if (resourcesLoaded.Any())
        {
          foreach (var resource in resourcesLoaded)
          {
            resource.Value.Dispose();
          }
        }

        // Indicate that the instance has been disposed.
        isDisposed = true;
      }

      // Call SupressFinalize in case a subclass implements a finalizer.
      GC.SuppressFinalize(this);
    }
  }
}
