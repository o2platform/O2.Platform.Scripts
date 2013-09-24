using System;
using System.Diagnostics;
using FluentSharp.CoreLib;  
 
//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Sandcastle_Installer_test
	{
		public void test()
		{
			new Sandcastle_Installer().start();
		}
	}
	public class Sandcastle_Installer : Tool_API 
	{			
		  
		public Sandcastle_Installer()
		{
			config("Sandcastle",  
				  "SHFBGuidedInstaller_1970.zip",
				   buildCodePlexDownloadUri("shfb", 652438, 130098389357400000).uri(),
				   "doxygen.exe");
			installFromZip_Web(); 							
		}
						
		
		public Process start()
		{			 
			if (this.isInstalled())
				return this.Install_Dir.startProcess();
			return null;
		}
	}
}