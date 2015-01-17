using System;
using System.Linq;
using System.Runtime.CompilerServices;
using BuildAProject.BuildManagement.NUnit.Runners;

[assembly: InternalsVisibleTo("BuildAProject.Console.Test")]

namespace BuildAProject.Console.Logging
{
  sealed class NUnitLogger : INUnitLogger
  {
    private readonly ILog log;

    public NUnitLogger(ILog log)
    {
      if (log == null)
      {
        throw new ArgumentNullException("log");
      }

      this.log = log;
    }

    public void TestResult(NUnitExecutionResult result)
    {
      if (result == null)
      {
        throw new ArgumentNullException("result");
      }

      var failedMethodResults = result.MethodResults
        .Where(x => x.Status == NUnitStatus.Failed)
        .ToList();

      if (failedMethodResults.Any())
      {
        log.Error(String.Format("{0} errors while testing {1}", failedMethodResults.Count(), result.TestFilePath), 2);

        var methodCount = 1;
        foreach (var methodResult in failedMethodResults)
        {
          log.Error(String.Format("{0}\t{1}", methodCount, methodResult.MethodName), 3);
          log.Error(String.Format("{0}", methodResult.Message), 3);

          methodCount++;
        }
        log.Information("", 2);
      }
      else
      {
        log.Information(String.Format("Tests executed successfully in assembly: {0}{1}", result.TestFilePath, Environment.NewLine), 9);
      }
    }
  }
}
