using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskProviders;

namespace BuildAProject.BuildManagement.DLLs.TaskProviders
{
  public class DllFileTaskFactory : IBuildTaskFactory<DllFileProject>
  {
    public IBuildTask Create(DllFileProject project)
    {
      if (project == null)
      {
        throw new ArgumentNullException("project");
      }

      return new DllFileTask(project);
    }
  }
}
