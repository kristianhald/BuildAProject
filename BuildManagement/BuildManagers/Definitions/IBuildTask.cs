using System.Collections.Generic;

namespace BuildAProject.BuildManagement.BuildManagers.Definitions
{
  public interface IBuildTask
  {
    IEnumerable<Dependency> Dependencies { get; }

    string Name { get; }

    void Execute();
  }
}
