
namespace BuildAProject.Console.Logging
{
  public interface ILog
  {
    void Information(string informationMessage, int verboseLevel);

    void Error(string errorMessage, int verboseLevel);
  }
}
