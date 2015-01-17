using BuildAProject.BuildManagement.NuGet.Downloaders;
using BuildAProject.BuildManagement.NuGet.TaskProviders;
using Moq;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class NuGetPackageDownloadTaskFactoryBuilder
  {
    private static readonly MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    public INuGetDownloader NuGetDownloader = MockRepository.Create<INuGetDownloader>().Object;

    public NuGetPackageDownloadTaskFactory Build()
    {
      return new NuGetPackageDownloadTaskFactory(NuGetDownloader);
    }
  }
}
