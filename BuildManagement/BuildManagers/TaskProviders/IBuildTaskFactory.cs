using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.BuildManagers.TaskProviders
{
  public interface IBuildTaskFactory<in TProject> where TProject : IProject
  {
    IBuildTask Create(TProject project);
  }
}
