using System;
using System.Collections.Generic;
using System.IO;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.CsProjects;
using BuildAProject.BuildManagement.CsProjects.Compilers;
using BuildAProject.BuildManagement.NUnit.Runners;

namespace BuildAProject.BuildManagement.NUnit
{
  public class NUnitTestTask : IBuildTask, IEquatable<NUnitTestTask>
  {
    private readonly ITestRunner testRunner;
    private readonly CsProject csProject;
    private readonly BuildEngineParameters parameters;

    public NUnitTestTask(ITestRunner testRunner, CsProject csProject, BuildEngineParameters parameters)
    {
      if (testRunner == null)
      {
        throw new ArgumentNullException("testRunner");
      }

      if (csProject == null)
      {
        throw new ArgumentNullException("csProject");
      }

      if (parameters == null)
      {
        throw new ArgumentNullException("parameters");
      }

      this.testRunner = testRunner;
      this.csProject = csProject;
      this.parameters = parameters;
    }

    public IEnumerable<Dependency> Dependencies { get { return new[] { new Dependency(csProject.Name) }; } }

    public string Name { get { return "Test" + csProject.Name; } }

    public void Execute()
    {
      var testFilePath = Path.Combine(parameters.GetProjectCompilationDirectory(csProject), parameters.GetProjectCompilationFile(csProject));
      testRunner.Test(testFilePath);
    }

    public bool Equals(NUnitTestTask other)
    {
      return
        other != null &&
        csProject.Equals(other.csProject);
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as NUnitTestTask;
      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      return csProject.GetHashCode();
    }

    public override string ToString()
    {
      return String.Format("NUnit Name: {0}", csProject.Name);
    }
  }
}
