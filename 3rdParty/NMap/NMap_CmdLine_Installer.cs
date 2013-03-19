using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
//O2File:Tool_API.cs
using O2.XRules.Database.Utils;
 
namespace O2.XRules.Database.APIs
{
	public class Install_Test
	{
		public void test()
		{
			new NMap_CmdLine().start();
		}
	}
	public class NMap_CmdLine : Tool_API 
	{			
		
		public NMap_CmdLine()
		{
			config( "NMap_CmdLine", 
					"http://nmap.org/dist/nmap-6.25-win32.zip".uri(), 
					@"nmap-6.25\nmap.exe");
    		
    		installFromZip_Web(); 						    		
		}
		
		
		
		public Process start()
		{
			if (isInstalled())
				Executable.parentFolder().startProcess();
				//return Executable.startProcess();
			return null;
		}		
	}
}