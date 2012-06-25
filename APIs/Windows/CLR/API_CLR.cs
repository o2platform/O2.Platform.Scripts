// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Kernel;

namespace O2.XRules.Database.APIs
{
	public class API_CLR
	{
		
	}
	
	public static class API_CLR_ExtensionMethods
	{		
		public static bool isRuntime_V2(this Process process)
		{
			return isRuntime(process,"v2.0");
		}
		public static bool isRuntime_V4(this Process process)
		{
			return isRuntime(process,"v4.0");
		}
		
		public static bool isRuntime(this Process process, string version)
		{
			if (process.isNull())
			{
				"in process.isRuntime , process value was null".error();
				return false;
			}
			Guid clsid = new Guid("9280188D-0E8E-4867-B30C-7FA83884E8DE");
			Guid riid = new Guid("D332DB9E-B9B3-4125-8207-A14884F53216");
			var metahost = (IClrMetaHost)_NativeMethods.nCLRCreateInstance(clsid, riid);

			IEnumUnknown runtimes = metahost.EnumerateLoadedRuntimes(process.Handle); 
			IClrRuntimeInfo runtime = _NativeMethods.GetRuntime(runtimes, version);
			//IClrRuntimeInfo runtime = _NativeMethods.GetRuntime(runtimes, "v2.0");
			return runtime.notNull();
		}
	}
	
    public class _NativeMethods
    {    
    	[SecurityCritical]
		[DllImport("mscoree.dll", EntryPoint = "CLRCreateInstance", PreserveSig = false)]
		[return: MarshalAs(UnmanagedType.Interface)]
		public static extern object nCLRCreateInstance([MarshalAs(UnmanagedType.LPStruct)] Guid clsid, [MarshalAs(UnmanagedType.LPStruct)] Guid iid);


		public static IClrRuntimeInfo GetRuntime(IEnumUnknown runtimes, String version)
		{			
		    Object[] temparr = new Object[3];
		    UInt32 fetchedNum;
		    do
		    {			    	
		        runtimes.Next(Convert.ToUInt32(temparr.Length), temparr, out fetchedNum);
		
		        for (Int32 i = 0; i < fetchedNum; i++)
		        {
		            IClrRuntimeInfo t = (IClrRuntimeInfo)temparr[i];
		
		            // version not specified we return the first one
		            if (String.IsNullOrEmpty(version))
		            {
		                return t;
		            }
		
		            // initialize buffer for the runtime version string
		            StringBuilder sb = new StringBuilder(16);
		            UInt32 len = Convert.ToUInt32(sb.Capacity);
		            t.GetVersionString(sb, ref len);
		            "version: {0}".info(sb.str());
		            if (sb.ToString().StartsWith(version, StringComparison.Ordinal))
		            {
		                return t;
		            }
		        }
		    } while (fetchedNum == temparr.Length);
		
		    return null;
		}	
    }
    
    [Guid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SecurityCritical]
	[ComImport]
	public interface IClrRuntimeInfo
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetVersionString([MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder buffer, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint bufferLength);
		
		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetRuntimeDirectory([MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder buffer, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint bufferLength);
		
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		bool IsLoaded([In] IntPtr processHandle);
		
		[LCIDConversion(3)]
		[MethodImpl(MethodImplOptions.InternalCall)]		
		void LoadErrorString([MarshalAs(UnmanagedType.U4)] [In] int resourceId, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder buffer, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref int bufferLength);
		
		[MethodImpl(MethodImplOptions.InternalCall)]				
		IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPWStr)] [In] string dllName);

		[MethodImpl(MethodImplOptions.InternalCall)]				
		IntPtr GetProcAddress([MarshalAs(UnmanagedType.LPStr)] [In] string procName);
		
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		object GetInterface([MarshalAs(UnmanagedType.LPStruct)] [In] Guid coClassId, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid interfaceId);
	}
	
    [Guid("D332DB9E-B9B3-4125-8207-A14884F53216"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SecurityCritical]
	[ComImport]
	public interface IClrMetaHost
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		object GetRuntime([MarshalAs(UnmanagedType.LPWStr)] [In] string version, [MarshalAs(UnmanagedType.LPStruct)] [In] Guid interfaceId);
		
		[MethodImpl(MethodImplOptions.InternalCall)]
		void GetVersionFromFile([MarshalAs(UnmanagedType.LPWStr)] [In] string filePath, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder buffer, [MarshalAs(UnmanagedType.U4)] [In] [Out] ref uint bufferLength);
		
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		IEnumUnknown EnumerateInstalledRuntimes();
		
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		
		IEnumUnknown EnumerateLoadedRuntimes([In] IntPtr processHandle);
		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
		int Reserved01([In] IntPtr reserved1);
	}

	[Guid("00000100-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SecurityCritical]
	[ComImport]
	public interface IEnumUnknown
	{	
		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
		uint Next([MarshalAs(UnmanagedType.U4)] [In] uint elementArrayLength, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.IUnknown, SizeParamIndex = 0)] [Out] object[] elementArray, [MarshalAs(UnmanagedType.U4)] out uint fetchedElementCount);
		[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall)]
		uint Skip([MarshalAs(UnmanagedType.U4)] [In] uint count);
		[MethodImpl(MethodImplOptions.InternalCall)]
		void Reset();
		[MethodImpl(MethodImplOptions.InternalCall)]
		void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumUnknown enumerator);
	}
	
}
