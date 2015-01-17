
namespace BuildAProject.BuildManagement.NuGet.Downloaders
{
  public interface INuGetDownloader
  {
    void Download(NuGetPackageFile packageFile);
  }
}
