using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildAProject.BuildManagement.BuildManagers.Definitions
{
  public class BuildTaskPhase : IEquatable<BuildTaskPhase>
  {
    public BuildTaskPhase(int order, IEnumerable<IBuildTask> buildTasks)
    {
      if (buildTasks == null)
      {
        throw new ArgumentNullException("buildTasks");
      }

      if (!buildTasks.Any())
      {
        throw new ArgumentException("The 'buildTasks' parameter must contain at least one entry else this phase is unnecessary.");
      }

      Order = order;
      Tasks = buildTasks;
    }

    public int Order { get; private set; }

    public IEnumerable<IBuildTask> Tasks { get; private set; }

    public bool Equals(BuildTaskPhase other)
    {
      return
        other != null &&
        Order == other.Order &&
        Tasks.SequenceEqual(other.Tasks);
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as BuildTaskPhase;
      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return
          Order.GetHashCode() +
          Tasks.Count().GetHashCode();
      }
    }
  }
}
