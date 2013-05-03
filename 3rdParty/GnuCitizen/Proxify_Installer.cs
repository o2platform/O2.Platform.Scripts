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
			new Proxify_Installer().start();
		}
	}
	public class Proxify_Installer : Tool_API 
	{			
		
		public Proxify_Installer()
		{
			config("Proxify", 
				   "https://gnucitizen.googlecode.com/files/proxify-WINNT-1.2.zip".uri(),
				   "proxify.exe");
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