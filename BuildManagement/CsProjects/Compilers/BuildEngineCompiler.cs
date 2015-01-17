using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildAProject.BuildManagement.CsProjects.Compilers.Logging;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;

namespace BuildAProject.BuildManagement.CsProjects.Compilers
{
  // The design principles used have been found from the following sites:
  //    http://bogdangavril.wordpress.com/2012/03/15/take-control-of-msbuild-using-msbuild-api/
  //    http://mitasoft.wordpress.com/2011/05/16/how-to-build-a-project-using-c-4-0/
  //    http://msdn.microsoft.com/en-us/library/microsoft.build.evaluation(v=vs.110).aspx
  //    http://www.odewit.net/ArticleContent.aspx?id=MsBuildApi4&format=html
  public class BuildEngineCompiler : IDotNetCompiler
  {
    private readonly BuildEngineParameters parameters;
    private readonly IDotNetCompilerLogger logger;

    public BuildEngineCompiler(BuildEngineParameters parameters, IDotNetCompilerLogger logger)
    {
      if (parameters == null)
      {
        throw new ArgumentNullException("parameters");
      }

      if (logger == null)
      {
        throw new ArgumentNullException("logger");
      }

      this.parameters = parameters;
      this.logger = logger;
    }

    public void Build(CsProject csProject)
    {
      logger.BuildStarted(new DotNetCompilerBuildStarted(csProject));

      var globalProperty = new Dictionary<string, string>();
      AddGlobalProperty("Configuration", parameters.Configuration, globalProperty);
      AddGlobalProperty("Platform", parameters.Platform, globalProperty);
      AddGlobalProperty("OutputPath", parameters.GetProjectCompilationDirectory(csProject), globalProperty);

      AddGlobalProperty(
        "ReferencePath",
        String.Format("{0}", GetProjectReferencePaths(csProject)),
        globalProperty);

      var projectCollection = new ProjectCollection();
      var buildParameters = new BuildParameters(projectCollection)
                            {
                              Loggers = new[] { new BuildEngineLogger(csProject, logger) }
                            };

      var buildRequest = new BuildRequestData(
        csProject.FilePath,
        globalProperty,
        parameters.ToolsVersion,
        parameters.TargetsToBuild,
        null);

      var buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequest);

      logger.BuildFinished(new DotNetCompilerBuildFinished(csProject, GetBuildStatus(buildResult)));
    }

    private string GetProjectReferencePaths(CsProject project)
    {
      return String.Format(
        "{0};{1};{2};{3};{4};{5}",
        parameters.GeneralOutputDirectory,
        parameters.ExecutableOutputDirectory,
        parameters.LibraryOutputDirectory,
        String.Join(";", project.References.Select(reference => Path.Combine(parameters.GeneralOutputDirectory, reference.Name))),
        String.Join(";", project.References.Select(reference => Path.Combine(parameters.ExecutableOutputDirectory, reference.Name))),
        String.Join(";", project.References.Select(reference => Path.Combine(parameters.LibraryOutputDirectory, reference.Name))));
    }

    private void AddGlobalProperty(string property, string value, IDictionary<string, string> globalProperty)
    {
      if (String.IsNullOrWhiteSpace(value))
        return;

      globalProperty.Add(property, value);
    }

    private BuildStatus GetBuildStatus(BuildResult buildResult)
    {
      switch (buildResult.OverallResult)
      {
        case BuildResultCode.Failure:
          return BuildStatus.Failed;

        case BuildResultCode.Success:
          return BuildStatus.Success;

        default:
          throw new Exception(String.Format("Unknown 'OverallResult' value: '{0}'", buildResult.OverallResult));
      }
    }
  }
}
