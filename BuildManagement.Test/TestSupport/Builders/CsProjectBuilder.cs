using System.Collections.Generic;
using BuildAProject.BuildManagement.CsProjects;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  public sealed class CsProjectBuilder
  {
    public string Name = "UnitTest_Name";

    public string OutputType = "UnitTest_OutputType";

    public IEnumerable<Reference> References = new[]
                                                  {
                                                    new ReferenceBuilder().Build()

                                                  };

    public CsProject Build()
    {
      return new CsProject(Name, References, OutputType);
    }
  }
}
