
namespace BuildAProject.BuildManagement.NuGet.Configurations
{
  public interface INuGetConfigFileReader
  {
    NuGetConfig Read(string filePath);
  }
}
