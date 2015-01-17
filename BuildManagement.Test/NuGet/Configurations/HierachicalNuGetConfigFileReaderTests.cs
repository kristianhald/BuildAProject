using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildAProject.BuildManagement.NuGet.Configurations;
using BuildAProject.BuildManagement.Test.TestSupport;
using BuildAProject.BuildManagement.Test.TestSupport.Settings;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.NuGet.Configurations
{
  [TestFixture]
  [Category("IntegrationTest")]
  public sealed class HierachicalNuGetConfigFileReaderTests
  {
    private const string TestOutputDirectoryAttribute = "%TestOutputDirectory%";

    private static object[] SourceConfigurations()
    {
      return new object[]
             {
               // Single package source and relative repository path and single nuget configuration
               new object[]
               {
                 ".",
                 new Dictionary<string, string> {{".", TestFileResourceProvider.NuGetConfigWithOnlyOnePackageSource}},
                 new NuGetConfig(TestOutputDirectoryAttribute + @"\somethingelse", new[] {TestOutputDirectoryAttribute + @"\PackageSource"})
               },

               // Single nuget configuration, but two package sources
               new object[]
               {
                 ".",
                 new Dictionary<string, string> {{".", TestFileResourceProvider.NuGetConfigWithSeveralPackageSource}},
                 new NuGetConfig("..\\packages", new[] {TestOutputDirectoryAttribute + @"\PackageSourceOne", TestOutputDirectoryAttribute + @"\PackageSourceTwo"})
               },

               // Single nuget configuration and a single active package source
               new object[]
               {
                 ".",
                 new Dictionary<string, string>
                 {
                   {".", TestFileResourceProvider.NuGetConfigWithSingleActivePackageSource}
                 },
                 new NuGetConfig("..\\packages", new[] {TestOutputDirectoryAttribute + @"\PackageSourceTwo"})
               },

               // Single nuget configuration, where the package source is cleared
               new object[]
               {
                 ".",
                 new Dictionary<string, string>
                 {
                   {".", TestFileResourceProvider.NuGetConfigClearsInTheMiddleOfPackageSource}
                 },
                 new NuGetConfig("..\\packages", new[] {TestOutputDirectoryAttribute + @"\PackageSourceNotCleared"})
               },

               // Two nuget configurations, where the one in the subdirectory does not clear
               new object[]
               {
                 "subdirectory",
                 new Dictionary<string, string>
                 {
                   {".", TestFileResourceProvider.NuGetConfigWithOnlyOnePackageSource},
                   {"subdirectory", TestFileResourceProvider.NuGetConfigWithSingleNonClearingPackageSource}
                 },
                 new NuGetConfig(TestOutputDirectoryAttribute + @"\subdirectory\TheSecondFile", new[] {TestOutputDirectoryAttribute + @"\subdirectory\AdditionalPackageSource", TestOutputDirectoryAttribute + @"\PackageSource"})
               },

               // Two nuget configurations, where the package source is cleared in lowest subdirectory thereby ignoring the configuration files below
               new object[]
               {
                 "subdirectory",
                 new Dictionary<string, string>
                 {
                   {".", TestFileResourceProvider.NuGetConfigWithOnlyOnePackageSource},
                   {"subdirectory", TestFileResourceProvider.NuGetConfigWithSingleActivePackageSource}
                 },
                 new NuGetConfig(TestOutputDirectoryAttribute + @"\somethingelse", new[] {TestOutputDirectoryAttribute + @"\subdirectory\PackageSourceTwo"})
               },

               // Single configuration containing a http repository
               new object[]
               {
                 ".",
                 new Dictionary<string, string>
                 {
                   {".", TestFileResourceProvider.NuGetConfigWithHttpPackageSource}
                 },
                 new NuGetConfig("..\\packages", new[] {@"http://this.is/url"})
               }
             };
    }

    [Test]
    [TestCaseSource("SourceConfigurations")]
    public void Parse_NuGetConfigurations_Correctly(
      string relativeDirectoryToRead,
      Dictionary<string, string> nugetConfigFiles,
      NuGetConfig expectedNuGetConfig)
    {
      // Arrange
      using (var outputDirectory = TestSettings.GetTempPath(this))
      {
        SetupDirectoryStructure(nugetConfigFiles, outputDirectory.Value);

        var testDirectory = Path.Combine(outputDirectory.Value, relativeDirectoryToRead);

        var parser = new HierachicalNuGetConfigFileReader();

        // Act
        var actualNuGetConfig = parser
          .Read(testDirectory);

        // Assert
        AssertRepositoryPath(expectedNuGetConfig, outputDirectory.Value, actualNuGetConfig);
        AssertPackageSources(expectedNuGetConfig, actualNuGetConfig, outputDirectory.Value);
      }
    }

    private static void AssertRepositoryPath(NuGetConfig expectedNuGetConfig, string outputDirectory,
      NuGetConfig actualNuGetConfig)
    {
      var expectedRepositoryPath = expectedNuGetConfig
        .RepositoryPath
        .Replace(TestOutputDirectoryAttribute, outputDirectory);

      Assert.AreEqual(expectedRepositoryPath, actualNuGetConfig.RepositoryPath);
    }

    private static void SetupDirectoryStructure(Dictionary<string, string> nugetConfigFiles, string outputDirectory)
    {
      foreach (var configFileInfo in nugetConfigFiles)
      {
        var relativeDirectoryPath = configFileInfo.Key;
        var configFile = configFileInfo.Value;

        var configFileDirectory = Path.Combine(outputDirectory, relativeDirectoryPath);
        Directory.CreateDirectory(configFileDirectory);
        using (var nugetConfigFileStream =
          new StreamReader(TestFileResourceProvider.CreateResourceStream(configFile)))
        {
          File.WriteAllText(Path.Combine(configFileDirectory, HierachicalNuGetConfigFileReader.NuGetConfigFileName),
            nugetConfigFileStream.ReadToEnd());
        }
      }
    }

    private static void AssertPackageSources(
      NuGetConfig expectedNuGetConfig,
      NuGetConfig actualNuGetConfig,
      string outputDirectory)
    {
      var expectedSource = expectedNuGetConfig
        .PackageSources
        .Select(source => ReplaceOutputDirectory(outputDirectory, source));
      CollectionAssert.AreEqual(expectedSource, actualNuGetConfig.PackageSources);
    }

    private static string ReplaceOutputDirectory(string outputDirectory, string relativeFilePath)
    {
      if (!relativeFilePath.Contains(TestOutputDirectoryAttribute))
        return relativeFilePath;

      var absoluteFilePath = relativeFilePath
        .Replace(TestOutputDirectoryAttribute, "")
        .TrimStart('\\');
      absoluteFilePath = Path.Combine(outputDirectory, absoluteFilePath);

      return absoluteFilePath;
    }
  }
}
