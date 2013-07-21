using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;

//O2File:Tool_API.cs
	
namespace O2.XRules.Database.APIs 
{	
	public class Launcher
	{
		public static void launch()
		{
			new Gource_Installer().start(); 
		}
	}
	public class Gource_Installer : Tool_API 
	{					
		
		public Gource_Installer()
		{
			config("gource", 
				   "https://gource.googlecode.com/files/gource-0.40.win32.zip".uri(),
				   "gource.exe");
			
    		installFromZip_Web();    		    		
		}
				
		public Process start()
		{
			if (isInstalled())			
			{
				var path = "What is the path of the repo to view".askUser();
				if (path.dirExists())
					return this.Executable.startProcess(path);											
			}
			return null;
		}		
	}
}