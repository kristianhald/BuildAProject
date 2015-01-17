using System;
using System.Collections.Generic;
using System.Linq;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskExecutors;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers.Dependencies;
using BuildAProject.BuildManagement.Locators;

namespace BuildAProject.BuildManagement.BuildManagers
{
  public class BuildAllTasksManager : IProjectBuildManager
  {
    private readonly IProjectsLocator projectsLocator;
    private readonly IBuildTaskManager buildTaskManager;
    private readonly IBuildTaskExecutor buildTaskExecutor;
    private readonly IDependencyAlgorithm dependencyAlgorithm;

    public BuildAllTasksManager(
      IProjectsLocator projectsLocator,
      IBuildTaskManager buildTaskManager,
      IBuildTaskExecutor buildTaskExecutor,
      IDependencyAlgorithm dependencyAlgorithm)
    {
      if (projectsLocator == null)
      {
        throw new ArgumentNullException("projectsLocator");
      }

      if (buildTaskManager == null)
      {
        throw new ArgumentNullException("buildTaskManager");
      }

      if (buildTaskExecutor == null)
      {
        throw new ArgumentNullException("buildTaskExecutor");
      }

      if (dependencyAlgorithm == null)
      {
        throw new ArgumentNullException("dependencyAlgorithm");
      }

      this.projectsLocator = projectsLocator;
      this.buildTaskManager = buildTaskManager;
      this.buildTaskExecutor = buildTaskExecutor;
      this.dependencyAlgorithm = dependencyAlgorithm;
    }

    public void Build(string rootDirectoryPath)
    {
      if (String.IsNullOrWhiteSpace(rootDirectoryPath))
      {
        throw new ArgumentNullException("rootDirectoryPath");
      }

      IEnumerable<IBuildTask> executedBuildTasks = new IBuildTask[0];
      IEnumerable<IProject> oldProjects = new IProject[0];
      IEnumerable<IProject> projects;
      while (!(projects = projectsLocator.FindProjects(rootDirectoryPath)).SequenceEqual(oldProjects))
      {
        var buildTasks = buildTaskManager.GetTasks(projects);

        var phasedBuildTasks = dependencyAlgorithm.OrderTasksByPhase(buildTasks);

        var filteredPhasedBuildTasks = FilterAlreadyExecutedBuildTasks(phasedBuildTasks, executedBuildTasks);

        if (!filteredPhasedBuildTasks.Any())
          return;

        buildTaskExecutor.Execute(filteredPhasedBuildTasks);

        oldProjects = projects;
        executedBuildTasks = phasedBuildTasks.SelectMany(phase => phase.Tasks);
      }
    }

    // TODO: This method should probably have been added aspect wise as a special implementation of the dependency algorithm
    private BuildTaskPhaseCollection FilterAlreadyExecutedBuildTasks(BuildTaskPhaseCollection phasedBuildTasks, IEnumerable<IBuildTask> executedBuildTasks)
    {
      return new BuildTaskPhaseCollection(
        phasedBuildTasks
          .Where(phase => phase.Tasks.Except(executedBuildTasks).Any())
          .Select(phase => new BuildTaskPhase(phase.Order, phase.Tasks.Except(executedBuildTasks))),
        phasedBuildTasks.UnhandledTasks);
    }
  }
}
