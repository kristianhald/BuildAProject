using System.Collections.Generic;

namespace BuildAProject.BuildManagement.BuildManagers.Definitions
{
  public interface IProject
  {
    string Name { get; }

    IEnumerable<Dependency> Dependencies { get; }
  }
}
