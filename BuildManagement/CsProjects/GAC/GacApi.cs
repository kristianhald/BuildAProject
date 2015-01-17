using System;
using System.Runtime.InteropServices;

namespace BuildAProject.BuildManagement.CsProjects.GAC
{
  internal class GacApi
  {
    [DllImport("fusion.dll")]
    internal static extern int CreateAssemblyCache(
        out IAssemblyCache ppAsmCache, int reserved);
  }
}
