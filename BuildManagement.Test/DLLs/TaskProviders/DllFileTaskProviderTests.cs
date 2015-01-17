using System;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskProviders;
using BuildAProject.BuildManagement.DLLs;
using BuildAProject.BuildManagement.DLLs.TaskProviders;
using BuildAProject.BuildManagement.Test.TestSupport.Builders;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace BuildAProject.BuildManagement.Test.DLLs.TaskProviders
{
  [TestFixture]
  public sealed class DllFileTaskProviderTests
  {
    private readonly MockRepository mockRepository = new MockRepository(MockBehavior.Loose);

    [Test]
    public void DllFileTaskProvider_AllParameters_DoNotAcceptNull()
    {
      // Arrange
      var fixture = new Fixture()
        .Customize(new AutoMoqCustomization());

      var assertion = new GuardClauseAssertion(fixture);

      // Act + Assert
      assertion.Verify(typeof(DllFileTaskProvider).GetConstructors());
    }

    [Test]
    public void GetTasks_WithListOfProjects_ForEachDllProjectCreateDllTask()
    {
      // Arrange
      var dllProjectOne = new DllFileProjectBuilder().Build();
      var dllProjectTwo = new DllFileProjectBuilder().Build();

      var taskOne = new DllFileTask(dllProjectOne);
      var taskTwo = new DllFileTask(dllProjectTwo);
      var expectedTasks = new[]
                          {
                            taskOne,
                            taskTwo
                          };

      var fakeBuildTaskFactory = mockRepository.Create<IBuildTaskFactory<DllFileProject>>();
      fakeBuildTaskFactory
        .Setup(factory => factory.Create(dllProjectOne))
        .Returns(taskOne);
      fakeBuildTaskFactory
        .Setup(factory => factory.Create(dllProjectTwo))
        .Returns(taskTwo);
      fakeBuildTaskFactory
        .Setup(factory => factory.Create(null))
        .Throws<Exception>();

      var provider = new DllFileTaskProvider(fakeBuildTaskFactory.Object);

      var projects = new[]
                     {
                       dllProjectOne,
                       mockRepository.Create<IProject>().Object,
                       dllProjectTwo,
                       mockRepository.Create<IProject>().Object,
                     };

      // Act
      var actualTasks = provider.GetTasks(projects);

      // Assert
      CollectionAssert.AreEquivalent(expectedTasks, actualTasks);
    }

    [Test]
    public void GetTasks_WithListOfProjectsWhereSubSetHasSameName_ForEachDllProjectNotMatchingNameOfOtherProjectDllTaskIsCreated()
    {
      // Arrange
      var dllProjectOne = new DllFileProjectBuilder
                          {
                            FilePath = @"Matching\Another\Non\Dll\Project\Name.dll"
                          }.Build();
      var dllProjectTwo = new DllFileProjectBuilder().Build();

      var taskTwo = new DllFileTask(dllProjectTwo);
      var expectedTasks = new[]
                          {
                            taskTwo
                          };

      var fakeBuildTaskFactory = mockRepository.Create<IBuildTaskFactory<DllFileProject>>();
      fakeBuildTaskFactory
        .Setup(factory => factory.Create(dllProjectTwo))
        .Returns(taskTwo);
      fakeBuildTaskFactory
        .Setup(factory => factory.Create(null))
        .Throws<Exception>();

      var provider = new DllFileTaskProvider(fakeBuildTaskFactory.Object);

      var fakeMatchingProject = mockRepository.Create<IProject>();
      fakeMatchingProject
        .Setup(project => project.Name)
        .Returns(dllProjectOne.Name);
      var projects = new[]
                     {
                       dllProjectOne,
                       fakeMatchingProject.Object,
                       dllProjectTwo,
                       mockRepository.Create<IProject>().Object,
                     };

      // Act
      var actualTasks = provider.GetTasks(projects);

      // Assert
      CollectionAssert.AreEquivalent(expectedTasks, actualTasks);
    }
  }
}
