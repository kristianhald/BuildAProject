using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskProviders;
using BuildAProject.BuildManagement.CsProjects.Compilers;

namespace BuildAProject.BuildManagement.CsProjects.TaskProviders
{
  public class CsCompileTaskFactory : IBuildTaskFactory<CsProject>
  {
    private readonly IDotNetCompiler dotNetCompiler;

    public CsCompileTaskFactory(IDotNetCompiler dotNetCompiler)
    {
      if (dotNetCompiler == null)
      {
        throw new ArgumentNullException("dotNetCompiler");
      }

      this.dotNetCompiler = dotNetCompiler;
    }

    public IBuildTask Create(CsProject csProject)
    {
      return new CsCompileTask(csProject, dotNetCompiler);
    }
  }
}
