using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppDomainExecutor;
using NUnit.Core;

namespace BuildAProject.BuildManagement.NUnit.Runners
{
  public class NUnitFileTestRunner : ITestRunner
  {
    private readonly INUnitLogger logger;

    public NUnitFileTestRunner(INUnitLogger logger)
    {
      if (logger == null)
      {
        throw new ArgumentNullException("logger");
      }

      this.logger = logger;
    }

    public void Test(string filePath)
    {
      if (String.IsNullOrWhiteSpace(filePath))
      {
        throw new ArgumentNullException("filePath");
      }

      if (!File.Exists(filePath))
      {
        // TODO: If the file cannot be found, then it might be because of a build error. Though it should probably be logged.
        return;
      }

      try
      {
        var result = ExecuteTests(filePath);
        var methodTestResults = ParseResults(new[] { result });

        logger.TestResult(new NUnitExecutionResult(Path.GetFullPath(filePath), methodTestResults));
      }
      catch (Exception e)
      {
        logger.TestResult(
          new NUnitExecutionResult(
            Path.GetFullPath(filePath),
            new[]
            {
              new NUnitTestMethodResult("", NUnitStatus.Failed, e.ToString())
            }));
      }
    }

    private static TestResult ExecuteTests(string testDllFilePath)
    {
      var testPackage = new TestPackage(testDllFilePath);

      var newAppDomain = new NewAppDomain(Path.GetDirectoryName(testDllFilePath));
      var testResult = newAppDomain.Execute(
        testPackage,
        package =>
        {
          var remoteTestRunner = new RemoteTestRunner();
          remoteTestRunner.Load(package);
          var result = remoteTestRunner.Run(new NullListener(), TestFilter.Empty, false, LoggingThreshold.All);

          return result;
        },
        testDllFilePath + ".config");

      return testResult;
    }

    private IEnumerable<NUnitTestMethodResult> ParseResults(IEnumerable<TestResult> results)
    {
      var methodTestResults = new List<NUnitTestMethodResult>();
      foreach (var result in results)
      {
        if (
          result.FailureSite == FailureSite.Parent ||
          result.FailureSite == FailureSite.Child ||
          (result.FailureSite == FailureSite.Test && result.HasResults))
        {
          methodTestResults.AddRange(ParseResults(result.Results.Cast<TestResult>()));
        }
        else if (result.IsError || result.IsFailure || result.IsSuccess)
        {
          methodTestResults.Add(new NUnitTestMethodResult(
            ParseNUnitMethodName(result),
            ParseNUnitStatus(result),
            ParseNUnitMessage(result)));
        }
      }

      return methodTestResults;
    }

    private static string ParseNUnitMethodName(TestResult result)
    {
      return result.FullName;
    }

    private static NUnitStatus ParseNUnitStatus(TestResult result)
    {
      switch (result.ResultState)
      {
        case ResultState.Success:
          return NUnitStatus.Success;

        case ResultState.Failure:
          return NUnitStatus.Failed;

        case ResultState.Error:
          return NUnitStatus.Failed;

        default:
          throw new Exception(String.Format("Unknown result state encountered: {0}", result.ResultState));
      }
    }

    private static string ParseNUnitMessage(TestResult result)
    {
      return result.Message ?? "";
    }
  }
}
