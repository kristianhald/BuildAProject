using BuildAProject.BuildManagement.NuGet.Downloaders;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using BuildAProject.Console.Logging;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.Console.Test.Logging
{
  [TestFixture]
  public sealed class NuGetDownloaderLoggerTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void NuGetDownloaderLogger_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture().Customize(new AutoMoqCustomization());
      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(NuGetDownloaderLogger).GetConstructors());
    }

    [Test]
    public void Download_WhenCalled_ThenTheCompositeIsCalled()
    {
      // Arrange
      var expectedPackageFile = new NuGetPackageFileBuilder().Build();

      var mockComposite = mockRepository.Create<INuGetDownloader>();
      var fakeLog = mockRepository.Create<ILog>();

      var logger = new NuGetDownloaderLogger(mockComposite.Object, fakeLog.Object);

      // Act
      logger.Download(expectedPackageFile);

      // Assert
      mockComposite
        .Verify(executor => executor.Download(expectedPackageFile));
    }

    [Test]
    public void Download_WhenCalled_ThenTheInformationMethodOnTheLogIsCalled()
    {
      // Arrange
      var expectedPackageFile = new NuGetPackageFileBuilder().Build();

      var fakeComposite = mockRepository.Create<INuGetDownloader>();
      var mockLog = mockRepository.Create<ILog>();

      var logger = new NuGetDownloaderLogger(fakeComposite.Object, mockLog.Object);

      // Act
      logger.Download(expectedPackageFile);

      // Assert
      mockLog
        .Verify(log => log.Information(It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(2));
    }
  }
}
