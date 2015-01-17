using System;
using System.IO;

namespace BuildAProject.BuildManagement.Test.TestSupport.Settings
{
  sealed class TestDirectory : IDisposable
  {
    public TestDirectory(string directory)
    {
      if (directory == null)
        throw new ArgumentNullException("directory");

      Value = directory;
    }

    public string Value { get; private set; }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (Directory.Exists(Value))
          Directory.Delete(Value, true);
      }
    }
  }
}
