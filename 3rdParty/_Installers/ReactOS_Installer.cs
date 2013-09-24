using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class ReactOS_Installer_Test
	{
		public void test()
		{
			new ReactOS_Installer().start();
		}
	}

	public class ReactOS_Installer : Tool_API 
	{				
		public ReactOS_Installer()
		{
			config("ReactOS", 
				   "http://kent.dl.sourceforge.net/project/reactos/ReactOS/0.3.15/ReactOS-0.3.15-REL-QEMU.zip".uri(),
				   "ReactOS-0.3.15-QEMU\\boot.bat");			
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
