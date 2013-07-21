using System.Diagnostics;
using FluentSharp.CoreLib;

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
