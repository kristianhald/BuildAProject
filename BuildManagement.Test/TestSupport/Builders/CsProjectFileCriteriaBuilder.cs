using BuildAProject.BuildManagement.CsProjects.SearchCriteria;
using BuildAProject.BuildManagement.Locators.FileSystem;
using Moq;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class CsProjectFileCriteriaBuilder
  {
    private readonly static MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    public ILocatorFileSystem LocatorFileSystem = MockRepository.Create<ILocatorFileSystem>().Object;

    public CsProjectFileCriteria Build()
    {
      return new CsProjectFileCriteria(LocatorFileSystem);
    }
  }
}
