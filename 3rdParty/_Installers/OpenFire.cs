using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs
	
namespace O2.XRules.Database.APIs 
{	
	public class OpenFire_Installer_Test
	{
		public static void launch()
		{
			new OpenFire_Installer().start(); 
		}
	}
	public class OpenFire_Installer : Tool_API 
	{					
		
		public OpenFire_Installer()
		{
			config("OpenFire", 
				   "http://download.igniterealtime.org/openfire/openfire_3_8_2.zip".uri(),
				   "openfire/bin/openfire.exe");
			
    		installFromZip_Web();    		    		
		}
				
		public Process start()
		{
			if (isInstalled())
			{
				this.Executable.startProcess();				
				"http://localhost:9090".startProcess();
			}
			return null;
		}		
	}
}