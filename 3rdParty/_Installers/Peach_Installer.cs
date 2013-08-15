using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Peach_Installer_Test
	{
		public void test() 
		{
			new Peach_Installer().start(); 
		}
	}
	public class Peach_Installer : Tool_API 
	{			
		public Peach_Installer()
		{			    		    	
			config("Peach", 				   
			   	   "http://freefr.dl.sourceforge.net/project/peachfuzz/Peach/3.0%20Nightly/peach-3.0.207-win-x86-release.zip".uri(),				   
			   	   @"deploy\Debug\WebDev.WebServer40.exe");					   
    		installFromZip_Web(); 						
		}		
		public Process start()
		{
			if (isInstalled())
				return this.Executable.startProcess();
			return null;
		}
	}
}