using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs
	
namespace O2.XRules.Database.APIs 
{	
	public class Launcher
	{
		public static void launch()
		{
			new JabberNet_Installer().start(); 
		}
	}
	public class JabberNet_Installer : Tool_API 
	{					
		
		public JabberNet_Installer()
		{
			config("JabberNet", 
				   "https://jabber-net.googlecode.com/files/JabberNet.Mono.v2.1.0.710.zip".uri(),
				   "jabber-net.dll");
			
    		installFromZip_Web();    		    		    		
		}
				
		public Process start()
		{
			if (isInstalled())
			{
				
				//this.Executable.startProcess();				
				//"http://localhost:9090".startProcess();
			}
			return null;
		}		
	}
}