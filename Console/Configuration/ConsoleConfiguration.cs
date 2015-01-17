using System;
using System.IO;
using System.Linq;
using System.Text;
using BuildAProject.BuildManagement.CsProjects.Compilers;
using Fclp;

namespace BuildAProject.Console.Configuration
{
  class ConsoleConfiguration
  {
    private bool isParsed;

    public bool Parse(string[] args)
    {
      var parser = new FluentCommandLineParser
                   {
                     IsCaseSensitive = false
                   };

      parser
        .Setup<int>('l', "loglevel")
        .Callback(value => logLevel = value)
        .SetDefault(9)
        .WithDescription("Sets how much information is provided from the program when executing. 0 means no information is printed, while 10 means everything is printed.");

      parser
        .Setup<string>('b', "buildpath")
        .Callback(value => buildPath = value)
        .SetDefault(Directory.GetCurrentDirectory())
        .WithDescription("Sets the root directory where the search for projects starts. The default is using the directory where the program is executing from.");

      parser
        .Setup<string>('c', "compilationbasepath")
        .Callback(value => compilationBasePath = value)
        .Required()
        .WithDescription("Sets the base compilation path where everything is compiled to.");

      parser
        .Setup<string>('n', "nugetpackagepath")
        .Callback(value => nugetPackagePath = value)
        .Required()
        .WithDescription("Sets the nuget package path where the nuget packages are downloaded to.");

      parser
        .SetupHelp("h", "help", "?")
        .Callback(message => System.Console.WriteLine(message));

      var result = parser.Parse(args);
      if (result.HelpCalled)
      {
        return false;
      }
      else if (result.AdditionalOptionsFound.Any())
      {
        ReportAdditionOptions(result);
        return false;
      }
      else if (result.HasErrors)
      {
        System.Console.WriteLine(result.ErrorText);
        return false;
      }

      isParsed = true;

      return isParsed;
    }

    private void ReportAdditionOptions(ICommandLineParserResult result)
    {
      var unknownParameters = String.Join(
        Environment.NewLine,
        result.AdditionalOptionsFound.Select(option => String.Format("{0}: {1}", option.Key, option.Value)));

      var unmatchedParameters = String.Join(
        Environment.NewLine,
        result.UnMatchedOptions.Select(option => String.Format("{0}: {1}", option.ShortName, option.Description)));

      var messageBuilder = new StringBuilder();
      messageBuilder.AppendLine("Unknown parameters were encountered.");
      messageBuilder.AppendLine(unknownParameters);
      messageBuilder.AppendLine();
      messageBuilder.AppendLine("Known unmatched parameters are.");
      messageBuilder.AppendLine(unmatchedParameters);

      System.Console.WriteLine(messageBuilder.ToString());
    }

    public BuildEngineParameters BuildEngineConfiguration
    {
      get
      {
        return new BuildEngineParameters
               {
                 GeneralOutputDirectory = Path.Combine(CompilationBasePath, "Other"),
                 LibraryOutputDirectory = Path.Combine(CompilationBasePath, "Libraries"),
                 LibrariesInSeparateDirectories = true,
                 ExecutableOutputDirectory = Path.Combine(CompilationBasePath, "Executable"),
                 ExecutablesInSeparateDirectories = true
               };
      }
    }

    private int logLevel;
    public int LogLevel
    {
      get
      {
        AssertIsParsed();
        return logLevel;
      }
    }

    private string buildPath;
    public string BuildPath
    {
      get
      {
        AssertIsParsed();
        return buildPath;
      }
    }

    private string compilationBasePath;
    public string CompilationBasePath
    {
      get
      {
        AssertIsParsed();

        return Path.GetFullPath(compilationBasePath);
      }
    }

    private string nugetPackagePath;
    public string NuGetPackagePath
    {
      get
      {
        AssertIsParsed();

        return Path.GetFullPath(nugetPackagePath);
      }
    }

    private void AssertIsParsed()
    {
      if (isParsed)
        return;

      throw new Exception("The console configuration has not parsed the provided arguments from the command line. Before this occurs the configuration properties cannot be accessed.");
    }
  }
}
