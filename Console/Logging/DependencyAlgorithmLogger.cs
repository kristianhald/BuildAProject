using System;
using System.Collections.Generic;
using BuildAProject.BuildManagement.BuildManagers.Definitions;
using BuildAProject.BuildManagement.BuildManagers.TaskManagers.Dependencies;

namespace BuildAProject.Console.Logging
{
  sealed class DependencyAlgorithmLogger : IDependencyAlgorithm
  {
    private readonly IDependencyAlgorithm composite;
    private readonly ILog logger;

    public DependencyAlgorithmLogger(IDependencyAlgorithm composite, ILog logger)
    {
      if (composite == null)
      {
        throw new ArgumentNullException("composite");
      }

      if (logger == null)
      {
        throw new ArgumentNullException("logger");
      }

      this.composite = composite;
      this.logger = logger;
    }

    public BuildTaskPhaseCollection OrderTasksByPhase(IEnumerable<IBuildTask> tasks)
    {
      logger.Information(String.Format("Building tasks dependency graph{0}", Environment.NewLine), 7);
      return composite.OrderTasksByPhase(tasks);
    }
  }
}
