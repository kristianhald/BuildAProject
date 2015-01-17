using BuildAProject.BuildManagement.NuGet;
using BuildAProject.BuildManagement.NuGet.Downloaders;
using Moq;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class NuGetPackageDownloadTaskBuilder
  {
    private static readonly MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    public NuGetPackageFile Package = new NuGetPackageFileBuilder().Build();

    public INuGetDownloader Downloader = MockRepository.Create<INuGetDownloader>().Object;

    public NuGetPackageDownloadTask Build()
    {
      return new NuGetPackageDownloadTask(Package, Downloader);
    }
  }
}
