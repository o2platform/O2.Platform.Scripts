using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Install_Test
	{
		public void test()
		{
			new BadAssProxy_Installer().start();
		}
	}
	public class BadAssProxy_Installer : Tool_API 
	{			
		
		public BadAssProxy_Installer()
		{
			config("BadAssProxy", 
				   "https://gnucitizen.googlecode.com/files/badassproxy-WINNT-0.0.zip".uri(),
				   "binary\\bap.exe");
			installFromZip_Web(); 							
		}
						
		
		public Process start()
		{			
			if (this.isInstalled())
				return this.Executable.startProcess();
			return null;
		}		
	}
}