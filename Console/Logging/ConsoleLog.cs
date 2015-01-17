using System;
using System.IO;

namespace BuildAProject.Console.Logging
{
  class ConsoleLog : ILog
  {
    private readonly TextWriter output;
    private readonly int logVerboseLevel;

    public ConsoleLog(TextWriter output, int logVerboseLevel)
    {
      if (output == null)
      {
        throw new ArgumentNullException("output");
      }

      this.output = output;
      this.logVerboseLevel = logVerboseLevel;
    }

    public void Information(string informationMessage, int verboseLevel)
    {
      if (verboseLevel > logVerboseLevel)
        return;

      output.WriteLine(informationMessage);
    }

    public void Error(string errorMessage, int verboseLevel)
    {
      if (verboseLevel > logVerboseLevel)
        return;

      var oldColor = System.Console.ForegroundColor;
      System.Console.ForegroundColor = ConsoleColor.Red;

      output.WriteLine(errorMessage);

      System.Console.ForegroundColor = oldColor;
    }
  }
}
