using BuildAProject.BuildManagement.DLLs.TaskProviders;

namespace BuildAProject.BuildManagement.Test.TestSupport.Builders
{
  sealed class DllFileTaskFactoryBuilder
  {
    public DllFileTaskFactory Build()
    {
      return new DllFileTaskFactory();
    }
  }
}
