using System;
using System.Diagnostics;
using O2.DotNetWrappers.ExtensionMethods;  
//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class testInstall
	{
		public static void test()  
		{
			new Jad_Installer().start(); 
		}
	}
	 
	public class Jad_Installer : Tool_API    
	{				
		public Jad_Installer()
		{			
			config("Jad",
				   "http://www.varaneckas.com/jad/jad158g.win.zip".uri(),
				   @"jad.exe");
			installFromZip_Web();	       		
		}	
		//
		
		public Process start()
		{			
			if (this.isInstalled())
				return this.Executable.startProcess(); 
			return null;
		}		
	}
}