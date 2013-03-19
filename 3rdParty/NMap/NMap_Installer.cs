using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
//O2File:Tool_API.cs
using O2.XRules.Database.Utils;
 
namespace O2.XRules.Database.APIs
{
	public class Install_NMap_Test
	{
		public void test()
		{
			new NMap().start();
		}
	}
	public class NMap : Tool_API 
	{	
		public NMap() : this(true)
		{
		}
		
		public NMap(bool installNow)
		{
			config("MNap", "http://nmap.org/dist/nmap-6.25-setup.exe".uri(), "nmap-5.51-setup.exe");			
    		
    		//Install_Dir = @"C:\Program Files\Nmap";
    		if (installNow)
    			install();		
		}
		
		
		public bool install()
		{
			"Installing {0}".info(ToolName);
			return installFromExe_Web(); 						
		}
		
		public Process start()
		{
			if (install())
				return Install_Dir.pathCombine("zenmap.exe").startProcess(); 
			return null;
		}		
	}
}