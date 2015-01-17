using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BuildAProject.BuildManagement.CsProjects.Compilers.Logging;

namespace BuildAProject.Console.Logging
{
  sealed class BuildEngineCompilerLogger : IDotNetCompilerLogger
  {
    private readonly ConcurrentDictionary<string, List<DotNetCompilerBuildError>> buildErrors = new ConcurrentDictionary<string, List<DotNetCompilerBuildError>>();
    private readonly ILog logger;

    public BuildEngineCompilerLogger(ILog logger)
    {
      if (logger == null)
      {
        throw new ArgumentNullException("logger");
      }

      this.logger = logger;
    }

    public void BuildStarted(DotNetCompilerBuildStarted buildStarted)
    {
      List<DotNetCompilerBuildError> projectBuildErrors;
      buildErrors.TryRemove(buildStarted.ProjectName, out projectBuildErrors);

      logger.Information(String.Format("Building '{0}'", buildStarted.ProjectName), 9);
    }

    public void ErrorRaised(DotNetCompilerBuildError buildError)
    {
      buildErrors.AddOrUpdate(
        buildError.ProjectName,
        projectName => new List<DotNetCompilerBuildError>
                        {
                          buildError
                        },
        (projectName, errors) =>
        {
          errors.Add(buildError);
          return errors;
        });
    }

    public void BuildFinished(DotNetCompilerBuildFinished buildFinished)
    {
      List<DotNetCompilerBuildError> projectBuildErrors;
      buildErrors.TryRemove(buildFinished.ProjectName, out projectBuildErrors);

      if (buildFinished.Status == BuildStatus.Success)
      {
        BuildSucceded(buildFinished, projectBuildErrors);
      }
      else
      {
        BuildFailed(buildFinished, projectBuildErrors);
      }
    }

    private void BuildSucceded(DotNetCompilerBuildFinished buildFinished, List<DotNetCompilerBuildError> projectBuildErrors)
    {
      if (projectBuildErrors != null && projectBuildErrors.Any())
      {
        throw new Exception(
          String.Format(
            "Invalid state encountered while building {0}. The build states that it succeded, but build errors were provided.",
            buildFinished.ProjectName));        
      }

      logger.Information(String.Format("Finished building '{0}'{1}", buildFinished.ProjectName, Environment.NewLine), 9);
    }

    private void BuildFailed(DotNetCompilerBuildFinished buildFinished, List<DotNetCompilerBuildError> projectBuildErrors)
    {
      if (projectBuildErrors == null || !projectBuildErrors.Any())
      {
        throw new Exception(
          String.Format(
            "Invalid state encountered while building {0}. The build states that it failed, but no build errors were provided.",
            buildFinished.ProjectName));        
      }

      logger.Error(String.Format("{0} error while building {1}", projectBuildErrors.Count, buildFinished.ProjectName), 2);
      foreach (var projectBuildError in projectBuildErrors)
      {
        logger.Error(String.Format("\t{0}",
          projectBuildError.Message.Replace(Environment.NewLine, String.Format("\t{0}", Environment.NewLine))), 3);
      }
      logger.Error(String.Format("Failed building '{0}'{1}", buildFinished.ProjectName, Environment.NewLine), 3);
    }
  }
}
