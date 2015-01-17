using BuildAProject.BuildManagement.DLLs;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class DllFileProjectBuilder
  {
    public string FilePath = @".\some\file\path\UnitTest.someextension";

    public DllFileProject Build()
    {
      return new DllFileProject(FilePath);
    }
  }
}
