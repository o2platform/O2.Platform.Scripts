// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms; 
using O2.Kernel.ExtensionMethods;
using System.Runtime.InteropServices;
using O2.DotNetWrappers.ExtensionMethods;
//O2Ref:EasyHook\EasyHook.dll

//Installer:EasyHook_Installer.cs!EasyHook\EasyHook32.dll
//O2File:API_WinAPI.cs 

namespace O2.XRules.Database.APIs 
{
	public class API_EasyHook
	{
		 /*public void GetSystemTimeAsFileTime(Action<bool> callback)
		 {
		 	
		 }*/
		 
		 public static void GetSystemTimeAsFileTime(out long lpSystemTimeAsFileTime)
		 {
		 	WinAPI.GetSystemTimeAsFileTime(out lpSystemTimeAsFileTime);
		 	//lpSystemTimeAsFileTime; += 100000;
		 	/*
		 	var now = WinAPI.GetSystemTimeAsFileTime(out lpSystemTimeAsFileTime);
		 	
		 	var newNow = now.AddHours(1);
		 	//"***** in GetSystemTimeAsFileTime: now {0} newNew {1}  {2} {3} ".info(now, newNow, now.ToFileTimeUtc(), newNow.ToFileTimeUtc());
		 	lpSystemTimeAsFileTime = newNow.ToFileTimeUtc();		 	
		 	//lpSystemTimeAsFileTime = 125911583990000000;
		 	*/
		 }
		 
			   		
	}	
	
	public class API_EasyHook_Delegates
	{
		[UnmanagedFunctionPointer(CallingConvention.Winapi,SetLastError = true)]
        public delegate void DGetSystemTimeAsFileTime(out long lpSystemTimeAsFileTime);	
	}
}