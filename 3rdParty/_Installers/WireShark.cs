using System;
using System.Diagnostics;
using O2.Kernel; 
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;  
using O2.XRules.Database.Utils;
//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class testInstall
	{
		public static void test()  
		{
			new Python_Install().start(); 
		}
	}
	 
	public class Python_Install : Tool_API    
	{				
		public Python_Install()
		{						
			config("WireShark", 
				   "http://wiresharkdownloads.riverbed.com/wireshark/win32/Wireshark-win32-1.10.0.exe".uri(),
				   ProgramFilesFolder.pathCombine(@"WireShark/Wireshark.exe"));
			
    		startInstaller_FromMsi_Web();   
			
		}			
		
		public Process start()
		{
			if (this.isInstalled())
				return this.Executable.startProcess(); 
			return null;
		}		
	}
}