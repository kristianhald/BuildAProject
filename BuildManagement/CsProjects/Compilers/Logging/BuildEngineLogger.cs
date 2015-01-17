using System;
using Microsoft.Build.Framework;

namespace BuildAProject.BuildManagement.CsProjects.Compilers.Logging
{
  sealed class BuildEngineLogger : ILogger
  {
    private readonly CsProject project;
    private readonly IDotNetCompilerLogger logger;

    public BuildEngineLogger(CsProject project, IDotNetCompilerLogger logger)
    {
      if (project == null)
      {
        throw new ArgumentNullException("project");
      }

      if (logger == null)
      {
        throw new ArgumentNullException("logger");
      }

      this.project = project;
      this.logger = logger;
    }

    public void Initialize(IEventSource eventSource)
    {
      eventSource.ErrorRaised += (sender, args) => logger.ErrorRaised(new DotNetCompilerBuildError(project, args.Message));
    }

    public void Shutdown()
    {

    }

    public LoggerVerbosity Verbosity { get; set; }

    public string Parameters { get; set; }
  }
}
