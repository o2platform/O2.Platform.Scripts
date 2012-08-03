using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;
//O2File:Tool_API.cs


namespace O2.XRules.Database.APIs 
{	
	/*public class LessMsi_Test
	{
		public void test()
		{
			new LessMsi_Install().start(); 
		}
	}*/
	
	public class LessMsi_Install : Tool_API 
	{	
		public LessMsi_Install() : this(true)
		{
		}
		 
		public LessMsi_Install(bool installNow)
		{
			config("LessMsi", "LessMsi.1.0.8", "lessmsi-v1.0.8");
    		Install_Uri = "http://lessmsi.googlecode.com/files/lessmsi-v1.0.8.zip".uri();    		
    		Executable = Install_Dir.pathCombine("lessmsi.exe");
    		if (installNow)
    			install();    		
		}
		 
		
		public bool install()
		{
			"Installing {0}".info(ToolName);
			return installFromZip_Web(); 						
			return false;
		}
		
		public Process start()
		{
			if (install())
				return Executable.startProcess();
			return null;
		}		
	}	
}