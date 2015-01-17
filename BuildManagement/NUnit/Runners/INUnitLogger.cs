
namespace BuildAProject.BuildManagement.NUnit.Runners
{
  /// <summary>
  /// Defines the logger used by the NUnit test runner
  /// It stores information on how the test execution went for different projects
  /// </summary>
  public interface INUnitLogger
  {
    void TestResult(NUnitExecutionResult result);
  }
}
