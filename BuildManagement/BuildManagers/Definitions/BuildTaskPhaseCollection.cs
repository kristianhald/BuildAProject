using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BuildAProject.BuildManagement.BuildManagers.Definitions
{
  public class BuildTaskPhaseCollection : IEquatable<BuildTaskPhaseCollection>, IOrderedEnumerable<BuildTaskPhase>
  {
    private readonly IOrderedEnumerable<BuildTaskPhase> phases;

    public BuildTaskPhaseCollection(IEnumerable<BuildTaskPhase> phases, IEnumerable<IBuildTask> unhandledTasks)
    {
      if (phases == null)
      {
        throw new ArgumentNullException("phases");
      }

      if (unhandledTasks == null)
      {
        throw new ArgumentNullException("unhandledTasks");
      }

      this.phases = phases.OrderBy(phase => phase.Order);
      
      UnhandledTasks = unhandledTasks;
    }

    public IEnumerable<IBuildTask> UnhandledTasks { get; private set; }

    public IOrderedEnumerable<BuildTaskPhase> CreateOrderedEnumerable<TKey>(Func<BuildTaskPhase, TKey> keySelector, IComparer<TKey> comparer, bool descending)
    {
      return phases.CreateOrderedEnumerable(keySelector, comparer, descending);
    }

    public IEnumerator<BuildTaskPhase> GetEnumerator()
    {
      return phases.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public bool Equals(BuildTaskPhaseCollection other)
    {
      return
        other != null &&
        phases.SequenceEqual(other.phases) &&
        UnhandledTasks.SequenceEqual(other.UnhandledTasks);
    }

    public override bool Equals(object obj)
    {
      var castedObj = obj as BuildTaskPhaseCollection;
      return Equals(castedObj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return
          phases.Sum(phase => phase.GetHashCode()) +
          UnhandledTasks.Sum(task => task.GetHashCode());
      }
    }
  }
}
