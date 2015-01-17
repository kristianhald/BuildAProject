using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskProviders;
using BuildAProject.BuildManagement.CsProjects;
using BuildAProject.BuildManagement.CsProjects.Compilers;
using BuildAProject.BuildManagement.NUnit.Runners;

namespace BuildAProject.BuildManagement.NUnit.TaskProviders
{
  public class NUnitTestTaskFactory : IBuildTaskFactory<CsProject>
  {
    private readonly ITestRunner testRunner;
    private readonly BuildEngineParameters parameters;

    public NUnitTestTaskFactory(ITestRunner testRunner, BuildEngineParameters parameters)
    {
      if (testRunner == null)
      {
        throw new ArgumentNullException("testRunner");
      }

      if (parameters == null)
      {
        throw new ArgumentNullException("parameters");
      }

      this.testRunner = testRunner;
      this.parameters = parameters;
    }

    public IBuildTask Create(CsProject csProject)
    {
      if (csProject == null)
      {
        throw new ArgumentNullException("csProject");
      }

      return new NUnitTestTask(testRunner, csProject, parameters);
    }
  }
}
