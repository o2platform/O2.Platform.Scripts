using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Install_DV_Test
	{
		public void test()
		{
			new DebugView().start();
		}
	}
	public class DebugView : Tool_API 
	{	
		public DebugView() : this(true)
		{
		}
		
		public DebugView(bool installNow)
		{
			config("DebugView", "DebugView v4.7.8", "DebugView.zip");			
    		Install_Uri = "http://download.sysinternals.com/files/DebugView.zip".uri();
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
				return Install_Dir.pathCombine("Dbgview.exe").startProcess();
			return null;
		}		
	}
}