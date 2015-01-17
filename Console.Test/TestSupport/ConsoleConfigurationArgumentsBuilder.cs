using System;
using System.Text;

namespace BuildAProject.Console.Test.TestSupport
{
  sealed class ConsoleConfigurationArgumentsBuilder
  {
    public string CompilationBasePathShortFormValue = null;
    public string CompilationBasePathLongFormValue = null;

    public string LogLevelShortFormValue = null;
    public string LogLevelLongFormValue = null;

    public string BuildPathShortFormValue = null;
    public string BuildPathLongFormValue = null;
   
    public string Build()
    {
      var argumentsBuilder = new StringBuilder();

      argumentsBuilder.Append(
        SelectFormValueRequired(
          "-c",
          CompilationBasePathShortFormValue,
          "--compilationbasepath",
          CompilationBasePathLongFormValue,
          "UnitTest_CompilationBasePath_Value"));

      argumentsBuilder.Append(
        SelectFormValue(
          "-l",
          LogLevelShortFormValue,
          "--loglevel",
          LogLevelLongFormValue));

      argumentsBuilder.Append(
        SelectFormValue(
          "-b",
          BuildPathShortFormValue,
          "--buildpath",
          BuildPathLongFormValue));

      return argumentsBuilder.ToString();
    }

    private string SelectFormValueRequired(
      string shortForm,
      string shortValue,
      string longForm,
      string longValue,
      string defaultValue)
    {
      if (shortValue == null && longValue == null)
      {
        return shortForm + " " + defaultValue + " ";
      }

      return SelectFormValue(shortForm, shortValue, longForm, longValue);
    }

    private string SelectFormValue(string shortForm, string shortValue, string longForm, string longValue)
    {
      if (shortValue != null && longValue != null)
      {
        throw new ArgumentException(String.Format("Both short '{0}' and long value '{1}' must not be provided at the same time.", shortValue, longValue));
      }
      else if (shortValue != null)
      {
        return shortForm + " " + shortValue + " ";
      }
      else if (longValue != null)
      {
        return longForm + " " + longValue + " ";
      }

      return "";
    }
  }
}
