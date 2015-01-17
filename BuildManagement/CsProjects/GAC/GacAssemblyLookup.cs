using System;
using System.Runtime.InteropServices;

namespace BuildAProject.BuildManagement.CsProjects.GAC
{
  internal class GacAssemblyLookup
  {
    public static bool AssemblyExist(string assemblyname, out string response)
    {
      try
      {
        response = QueryAssemblyInfo(assemblyname);
        return true;
      }
      catch (System.IO.FileNotFoundException e)
      {
        response = e.Message;
        return false;
      }
    }

    // If assemblyName is not fully qualified, a random matching may be 
    public static String QueryAssemblyInfo(string assemblyName)
    {
      var assemblyInfo = new AssemblyInfo { cchBuf = 512 };
      assemblyInfo.currentAssemblyPath = new String('\0', assemblyInfo.cchBuf);

      IAssemblyCache assemblyCache;

      // Get IAssemblyCache pointer
      var hr = GacApi.CreateAssemblyCache(out assemblyCache, 0);
      if (hr == 0)
      {
        hr = assemblyCache.QueryAssemblyInfo(1, assemblyName, ref assemblyInfo);
        if (hr != 0)
        {
          Marshal.ThrowExceptionForHR(hr);
        }
      }
      else
      {
        Marshal.ThrowExceptionForHR(hr);
      }

      return assemblyInfo.currentAssemblyPath;
    }
  }
}
