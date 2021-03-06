﻿
using System.Collections.Generic;
using BuildAProject.BuildManagement.BuildManagers.Definitions;

namespace BuildAProject.BuildManagement.BuildManagers.TaskManagers
{
  public interface IBuildTaskProvider
  {
    IEnumerable<IBuildTask> GetTasks(IEnumerable<IProject> projects);
  }
}
