using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
//O2File:Tool_API.cs
using O2.XRules.Database.Utils;

namespace O2.XRules.Database.APIs
{
	public class Install_PM_Test
	{
		public void test()
		{
			new ProcessMonitor().start();
		}
	}
	public class ProcessMonitor : Tool_API 
	{	
		public ProcessMonitor() : this(true)
		{
		}
		
		public ProcessMonitor(bool installNow)
		{
			config("ProcessMonitor", "ProcessMonitor v14.1", "ProcessMonitor.zip");			
    		Install_Uri = "http://download.sysinternals.com/files/ProcessMonitor.zip".uri();
    		if (installNow)
    			install();		
		}
		
		
		public bool install()
		{
			"Installing {0}".info(ToolName);
			return installFromZip_Web(); 						
		}
		
		public Process start()
		{
			if (install())
				return Install_Dir.pathCombine("procmon.exe").startProcess();
			return null;
		}		
	}
}