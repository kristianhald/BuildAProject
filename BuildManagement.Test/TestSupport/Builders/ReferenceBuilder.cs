using BuildAProject.BuildManagement.CsProjects;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class ReferenceBuilder
  {
    public string Name = "UnitTest_Name";

    public string HintPath = "UnitTest_HintPath";

    public Reference Build()
    {
      return new Reference(Name, HintPath);
    }
  }
}
