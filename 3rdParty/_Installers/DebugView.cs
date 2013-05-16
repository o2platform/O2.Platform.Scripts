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
			new DebugView_Installer().start();
		}
	}

	public class DebugView_Installer : Tool_API 
	{				
		public DebugView_Installer()
		{
			config("DebugView", 
				   "http://download.sysinternals.com/files/DebugView.zip".uri(),
				   "Dbgview.exe");			
    		installFromZip_Web();    		    		
		}
				
		public Process start()
		{
			if (isInstalled())
				this.Executable.startProcess();				
			return null;
		}		
	}
}
