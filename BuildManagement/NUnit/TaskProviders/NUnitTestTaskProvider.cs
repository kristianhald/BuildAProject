using BuildAProject.BuildManagement.BuildManagers.TaskProviders;
using BuildAProject.BuildManagement.CsProjects;

namespace BuildAProject.BuildManagement.NUnit.TaskProviders
{
  public class NUnitTestTaskProvider : TypeMatchingTaskProvider<CsProject>
  {
    public NUnitTestTaskProvider(IBuildTaskFactory<CsProject> buildTaskFactory)
      : base(buildTaskFactory)
    {
    }
  }
}
