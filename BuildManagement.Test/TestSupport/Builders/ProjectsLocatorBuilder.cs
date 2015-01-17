using System.Collections.Generic;
using BuildAProject.BuildManagement.Locators;
using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.Locators.SearchCriteria;
using Moq;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class ProjectsLocatorBuilder
  {
    private static readonly MockRepository MockRepository = new MockRepository(MockBehavior.Loose);

    public ILocatorFileSystem LocatorFileSystem = MockRepository.Create<ILocatorFileSystem>().Object;

    public IEnumerable<IFileCriteria> FileCriterias = new[] { MockRepository.Create<IFileCriteria>().Object };

    public CriteriaProjectsLocator Build()
    {
      return new CriteriaProjectsLocator(LocatorFileSystem, FileCriterias);
    }
  }
}
