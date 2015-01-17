using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildAProject.BuildManagement.NUnit.Runners
{
  public class NUnitExecutionResult : IEquatable<NUnitExecutionResult>
  {
    public NUnitExecutionResult(string testFilePath, IEnumerable<NUnitTestMethodResult> methodResults)
    {
      if (String.IsNullOrWhiteSpace(testFilePath))
      {
        throw new ArgumentNullException("testFilePath");
      }

      if (methodResults == null)
      {
        throw new ArgumentNullException("methodResults");
      }

      TestFilePath = testFilePath;
      MethodResults = methodResults;
    }

    public string TestFilePath { get; private set; }

    public IEnumerable<NUnitTestMethodResult> MethodResults { get; private set; }

    public bool Equals(NUnitExecutionResult other)
    {
      return
        other != null &&
        String.Equals(TestFilePath, other.TestFilePath) &&
        MethodResults.SequenceEqual(other.MethodResults);
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as NUnitExecutionResult;
      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      return TestFilePath.GetHashCode();
    }

    public override string ToString()
    {
      return String.Format(
        "{0}:{1}{2}",
        TestFilePath,
        Environment.NewLine,
        String.Join(Environment.NewLine, MethodResults.Select(result => result)));
    }
  }
}
