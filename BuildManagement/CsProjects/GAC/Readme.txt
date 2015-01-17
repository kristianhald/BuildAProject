Code located in this folder has been copied directly from:
http://trikks.wordpress.com/2011/07/13/programmatically-check-if-an-assembly-is-loaded-in-gac-with-c/

Corrected the 'CreateAssemblyCache' interface method and 'QueryAssemblyInfo' interface method from IntPtr to int.
Using IntPtr instead of int resulted in an overflow. The implementation should always use int32 as the HResult return is 
always an int32. The IntPtr is either 32 bit or 64 bit depending on system.