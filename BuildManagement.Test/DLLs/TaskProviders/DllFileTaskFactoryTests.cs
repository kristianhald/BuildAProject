using System;
using BuildAProject.BuildManagement.DLLs;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using NUnit.Framework;

namespace BuildAProject.BuildManagement.Test.DLLs.TaskProviders
{
  [TestFixture]
  public sealed class DllFileTaskFactoryTests
  {
    [Test]
    public void Create_ProjectParameterIsNull_ThrowsError()
    {
      // Arrange
      var factory = new DllFileTaskFactoryBuilder().Build();

      // Act + Assert
      Assert.Throws<ArgumentNullException>(() => factory.Create(null));
    }

    [Test]
    public void Create_GivenDllFileProject_ReturnsDllFileTask()
    {
      // Arrange
      var dllFileProject = new DllFileProjectBuilder().Build();
      var expected = new DllFileTask(dllFileProject);

      var factory = new DllFileTaskFactoryBuilder().Build();

      // Act
      var actual = factory.Create(dllFileProject);

      // Assert
      Assert.AreEqual(expected, actual);
    }
  }
}
