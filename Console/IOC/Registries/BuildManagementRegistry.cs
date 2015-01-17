using System.IO;
using BuildAProject.BuildManagement.BuildManagers;
using BuildAProject.BuildManagement.BuildManagers.TaskExecutors;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers.Dependencies;
using BuildAProject.BuildManagement.BuildManagers.TaskProviders;
using BuildAProject.BuildManagement.CsProjects;
using BuildAProject.BuildManagement.CsProjects.Compilers;
using BuildAProject.BuildManagement.CsProjects.Compilers.Logging;
using BuildAProject.BuildManagement.CsProjects.TaskProviders;
using BuildAProject.BuildManagement.DLLs.TaskProviders;
using BuildAProject.BuildManagement.Locators;
using BuildAProject.BuildManagement.Locators.FileSystem;
using BuildAProject.BuildManagement.NuGet.Configurations;
using BuildAProject.BuildManagement.NuGet.Downloaders;
using BuildAProject.BuildManagement.NuGet.TaskProviders;
using BuildAProject.BuildManagement.NUnit.Runners;
using BuildAProject.BuildManagement.NUnit.TaskProviders;
using BuildAProject.Console.Configuration;
using BuildAProject.Console.Logging;
using NuGet;
using StructureMap.Configuration.DSL;

namespace BuildAProject.Console.IOC.Registries
{
  public class BuildManagementRegistry : Registry
  {
    public BuildManagementRegistry()
    {
      Scan(scanner =>
      {
        scanner.AssemblyContainingType<IProjectBuildManager>();

        scanner.RegisterConcreteTypesAgainstTheFirstInterface();
      });

      For<BuildEngineParameters>()
        .Use(context => context.GetInstance<ConsoleConfiguration>().BuildEngineConfiguration);

      For<INuGetRepositoriesFactory>()
        .Use<NuGetPackageScannerRepositoryFactory>();

      For<IPackageRepositoryFactory>()
        .Use<PackageRepositoryFactory>();

      For<INuGetConfigFileReader>()
        .Use<HierachicalNuGetConfigFileReader>();

      For<INuGetDownloader>()
        .Use<RepositoryNuGetDownloader>()
        .Ctor<string>()
        .Is(context => context.GetInstance<ConsoleConfiguration>().NuGetPackagePath);

      For<ILocatorFileSystem>()
        .Use<LocalComputerFileSystem>()
        .DecorateWith((context, x) => new IgnoreFoldersFileSystem(x));

      For<IDependencyAlgorithm>()
        .DecorateAllWith((context, x) => new UnhandledBuildTasksLogger(x, context.GetInstance<ILog>()))
        .DecorateAllWith((context, x) => new DependencyAlgorithmLogger(x, context.GetInstance<ILog>()));

      For<IProjectsLocator>()
        .DecorateAllWith((context, x) => new ProjectsLocatorLogger(x, context.GetInstance<ILog>()));

      For<IBuildTaskManager>()
        .DecorateAllWith((context, x) => new BuildTaskManagerLogger(x, context.GetInstance<ILog>()));

      For<IBuildTaskExecutor>()
        .Use<SingleThreadedBuildTaskExecutor>();

      For<IBuildTaskExecutor>()
        .DecorateAllWith((context, x) => new BuildTaskExecutorLogger(x, context.GetInstance<ILog>()));

      For<IDotNetCompilerLogger>()
        .Use<BuildEngineCompilerLogger>();

      For<INUnitLogger>()
        .Use<NUnitLogger>();

      For<IBuildTaskProvider>()
        .Use<CsCompileTaskProvider>()
        .Ctor<IBuildTaskFactory<CsProject>>()
        .Is<CsCompileTaskFactory>();

      For<IBuildTaskProvider>()
        .Use<NUnitTestTaskProvider>()
        .Ctor<IBuildTaskFactory<CsProject>>()
        .Is<NUnitTestTaskFactory>();

      For<IBuildTaskProvider>()
        .Use<DllFileTaskProvider>();

      For<IBuildTaskProvider>()
        .Use<NuGetPackageDownloadTaskProvider>();

      For<INuGetDownloader>()
        .DecorateAllWith((context, x) => new NuGetDownloaderLogger(x, context.GetInstance<ILog>()));

      For<ILog>()
        .Singleton()
        .Use<ConsoleLog>()
        .Ctor<TextWriter>()
        .Is(System.Console.Out)
        .Ctor<int>()
        .Is(context => context.GetInstance<ConsoleConfiguration>().LogLevel);

      For<ConsoleConfiguration>()
        .Singleton();
    }
  }
}
