using System;
using System.Linq;

namespace BuildAProject.BuildManagement.BuildManagers.Definitions
{
  // TODO: The dependency handling it incorrect. The dependency must not do the check with the project. 
  //               Instead the project must have a method named 'CanHandleDependency' in which the dependency is provided.
  //               That way a single project may be able to handle different dependencies and might be able to do something
  //               project specific before determining if the dependency can be handled
  public class Dependency : IEquatable<IBuildTask>, IEquatable<Dependency>
  {
    private readonly string dependencyName;

    public Dependency(string dependency)
    {
      if (String.IsNullOrWhiteSpace(dependency))
      {
        throw new ArgumentNullException("dependency");
      }

      dependencyName = dependency;
    }

    public bool Equals(IBuildTask other)
    {
      return
        other != null &&
        dependencyName.Equals(other.Name);
    }

    public bool Equals(Dependency other)
    {
      return
        other != null &&
        dependencyName.Equals(other.dependencyName);
    }

    public override bool Equals(object obj)
    {
      if (obj is Dependency)
      {
        return Equals(obj as Dependency);
      }

      if (obj is IBuildTask)
      {
        return Equals(obj as IBuildTask);
      }

      return false;
    }

    public override int GetHashCode()
    {
      return dependencyName.GetHashCode();
    }

    public override string ToString()
    {
      return GetType().Name + " with " + dependencyName;
    }
  }
}
