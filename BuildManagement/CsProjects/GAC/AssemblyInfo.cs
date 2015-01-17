using System;
using System.Runtime.InteropServices;

namespace BuildAProject.BuildManagement.CsProjects.GAC
{
  [StructLayout(LayoutKind.Sequential)]
  internal struct AssemblyInfo
  {
    public int cbAssemblyInfo;
    public int assemblyFlags;
    public long assemblySizeInKB;

    [MarshalAs(UnmanagedType.LPWStr)]
    public String currentAssemblyPath;

    public int cchBuf;
  }
}
