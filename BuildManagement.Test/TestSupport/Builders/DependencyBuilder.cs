using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class DependencyBuilder
  {
    public string DependencyName = "UnitTest_DependencyName";

    public Dependency Build()
    {
      return new Dependency(DependencyName);
    }
  }
}
