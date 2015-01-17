using System;
using System.Collections.Generic;
using System.Linq;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.BuildManagers.Exceptions
{
  public class UnhandledTasksException : Exception
  {
    private readonly IEnumerable<IBuildTask> unhandledTasks;

    public UnhandledTasksException(IEnumerable<IBuildTask> unhandledTasks)
    {
      if (unhandledTasks == null)
      {
        throw new ArgumentNullException("unhandledTasks");
      }

      if (!unhandledTasks.Any())
      {
        throw new ArgumentException("The parameter 'unhandledTasks' must contain at least one unhandled build task else throwing this exception does not make sense.");
      }

      this.unhandledTasks = unhandledTasks;
    }

    public override string Message
    {
      get
      {
        return String.Format(
          "A number of build tasks were unhandled.{0}The tasks are:{0}{1}",
          Environment.NewLine,
          String.Join(Environment.NewLine, unhandledTasks.Select(task => task.Name)));
      }
    }
  }
}
