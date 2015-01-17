using System;
using System.Diagnostics;
using System.IO;
using BuildAProject.BuildManagement.BuildManagers;
using BuildAProject.Console.Configuration;
using BuildAProject.Console.IOC;
using BuildAProject.Console.Logging;

namespace BuildAProject.Console
{
  static class Program
  {
    static void Main(string[] args)
    {
      var stopWatch = new Stopwatch();
      stopWatch.Start();

      var container = Bootstrap.CreateContainer();

      var configuration = container.GetInstance<ConsoleConfiguration>();
      if (!configuration.Parse(args))
        return;

      var buildPath = Path.Combine(Environment.CurrentDirectory, configuration.BuildPath);

      var buildManager = container.GetInstance<IProjectBuildManager>();
      buildManager.Build(buildPath);

      var consoleLog = container.GetInstance<ILog>();

      var unhandledTasksErrorMessage = UnhandledBuildTasksLogger.GetErrorMessage();
      if (unhandledTasksErrorMessage != null)
        consoleLog.Error(unhandledTasksErrorMessage, 3);

      stopWatch.Stop();
      consoleLog.Information("Build complete", 1);
      consoleLog.Information(String.Format("Time building solutions: {0} ms", stopWatch.ElapsedMilliseconds), 1);
    }
  }
}
