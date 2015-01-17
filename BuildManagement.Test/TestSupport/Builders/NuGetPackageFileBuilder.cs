using System.Runtime.CompilerServices;
using BuildAProject.BuildManagement.NuGet;

[assembly: InternalsVisibleTo("BuildAProject.Console.Test")]

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class NuGetPackageFileBuilder
  {
    public string FilePath = @".\filepath\packages.config";

    public string PackageName = "NuGetPackageName";

    public string Version = "1.2.3";

    public string Framework = "2.0";

    public NuGetPackageFile Build()
    {
      return new NuGetPackageFile(FilePath, PackageName, Version, Framework);
    }
  }
}
