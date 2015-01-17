using BuildAProject.BuildManagement.BuildManagers.TaskProviders;

namespace BuildAProject.BuildManagement.CsProjects.TaskProviders
{
  public class CsCompileTaskProvider : TypeMatchingTaskProvider<CsProject>
  {
    public CsCompileTaskProvider(IBuildTaskFactory<CsProject> buildTaskFactory)
      : base(buildTaskFactory)
    {
    }
  }
}
