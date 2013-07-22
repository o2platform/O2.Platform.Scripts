using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class WireShark_Install_Test
	{
		public static void test()  
		{
			new WireShark_Install().start(); 
		}
	}
	 
	public class WireShark_Install : Tool_API    
	{				
		public WireShark_Install()
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