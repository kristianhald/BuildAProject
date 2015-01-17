using System;
using System.Runtime.InteropServices;

namespace BuildAProject.BuildManagement.CsProjects.GAC
{
  // GAC Interfaces - IAssemblyCache. As a sample, non used vtable entries 
  [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
  Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
  internal interface IAssemblyCache
  {
    int Dummy1();
    [PreserveSig()]
    int QueryAssemblyInfo(
        int flags,
        [MarshalAs(UnmanagedType.LPWStr)]
            String assemblyName,
        ref AssemblyInfo assemblyInfo);

    int Dummy2();
    int Dummy3();
    int Dummy4();
  }
}
