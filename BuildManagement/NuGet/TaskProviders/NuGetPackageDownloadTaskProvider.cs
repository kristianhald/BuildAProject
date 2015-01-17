using BuildAProject.BuildManagement.BuildManagers.TaskProviders;

namespace BuildAProject.BuildManagement.NuGet.TaskProviders
{
  public class NuGetPackageDownloadTaskProvider : TypeMatchingTaskProvider<NuGetPackageFile>
  {
    public NuGetPackageDownloadTaskProvider(IBuildTaskFactory<NuGetPackageFile> buildTaskFactory)
      : base(buildTaskFactory)
    {
    }
  }
}
