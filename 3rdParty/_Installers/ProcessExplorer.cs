using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;

//O2File:Tool_API.cs

public class DynamicType
{
	public void dynamicMethod()
	{
		new O2.XRules.Database.APIs.ProcessExplorer().start();
	}
}
	
namespace O2.XRules.Database.APIs
{
	
	public class ProcessExplorer : Tool_API 
	{	
		public ProcessExplorer() : this(true)
		{
		}
		
		public ProcessExplorer(bool installNow)
		{
			config("ProcessExplorer", "ProcessExplorer v14.1", "ProcessExplorer.zip");			
    		Install_Uri = "http://download.sysinternals.com/files/ProcessExplorer.zip".uri();
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
				return Install_Dir.pathCombine("procexp.exe").startProcess();
			return null;
		}		
	}
}