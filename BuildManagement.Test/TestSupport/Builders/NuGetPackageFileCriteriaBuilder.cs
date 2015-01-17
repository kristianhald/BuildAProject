using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.NuGet.SearchCriteria;
using Moq;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class NuGetPackageFileCriteriaBuilder
  {
    private static readonly MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    public ILocatorFileSystem LocatorFileSystem = MockRepository.Create<ILocatorFileSystem>().Object;

    public NuGetPackageFileCriteria Build()
    {
      return new NuGetPackageFileCriteria(LocatorFileSystem);
    }
  }
}
