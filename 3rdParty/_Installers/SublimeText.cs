using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs
	
namespace O2.XRules.Database.APIs 
{	
	public class SublimeText_Installer_Test
	{
		public static void launch()
		{
			new SublimeText_Installer().start(); 
		}
	}
	public class SublimeText_Installer : Tool_API 
	{					
		
		public SublimeText_Installer()
		{
			config("SublimeText", 
				   "http://c758482.r82.cf2.rackcdn.com/Sublime%20Text%202.0.1%20x64.zip".uri(),
				   "sublime_text.exe");
			
    		installFromZip_Web();    		    		
		}
				
		public Process start()
		{
			if (isInstalled())
			{
				this.Executable.startProcess();								
			}
			return null;
		}		
	}
}