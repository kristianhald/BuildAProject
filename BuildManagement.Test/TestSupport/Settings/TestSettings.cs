using System;
using System.IO;
using BuildAProject.BuildManagement.Test.TestSupport.Tools;

namespace BuildAProject.BuildManagement.Test.TestSupport.Settings
{
  static class TestSettings
  {
    /// <summary>
    /// Returns a random path based on the test class
    /// </summary>
    public static TestDirectory GetTempPath(object testClass)
    {
      if (testClass == null)
        throw new ArgumentNullException("testClass");

      return GetTempPath(testClass.GetType());
    }

    /// <summary>
    /// Returns a random path based on the test class
    /// </summary>
    public static TestDirectory GetTempPath(Type testClassType)
    {
      if (testClassType == null)
        throw new ArgumentNullException("testClassType");

      var baseTestOutputPath = Path.Combine(Path.GetTempPath(), testClassType.Name + "_" + DateTime.Now.ToFileTimeUtc());

      return new TestDirectory(Path.GetFullPath(baseTestOutputPath));
    }

    /// <summary>
    /// Setups up the provided folder with data from a specified embedded zip file
    /// </summary>
    public static void SetupTestEnvironmentFromZipFile(string testOutputPath, string zipFileName)
    {
      if (String.IsNullOrWhiteSpace(testOutputPath))
      {
        throw new ArgumentNullException("testOutputPath");
      }

      if (String.IsNullOrWhiteSpace(zipFileName))
      {
        throw new ArgumentNullException("zipFileName");
      }

      Directory.CreateDirectory(testOutputPath);

      using (
        var projectsZipFileStream =
          TestFileResourceProvider.CreateResourceStream(zipFileName))
      {
        UnzipTool.UnzipStream(projectsZipFileStream, testOutputPath);
      }
    }
  }
}
