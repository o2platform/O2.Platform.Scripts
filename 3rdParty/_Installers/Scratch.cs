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
			new Scratch_Installer().start();
		}
	}
	public class Scratch_Installer : Tool_API 
	{	
		public Scratch_Installer()
		{
			config("Scratch", 
				   "http://download.scratch.mit.edu/WinScratch1.4.zip".uri(),				   
				   @"WinScratch1.4\Scratch\Scratch.exe");			
    		installFromZip_Web();
		} 

		public Process start()
		{
			if (isInstalled())
				return Executable.startProcess();
			return null;
		}		
	}
}