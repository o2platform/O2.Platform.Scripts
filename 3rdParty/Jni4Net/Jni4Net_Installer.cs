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
			new Jni4Net_Installer().start(); 
		}
	}
	 
	public class Jni4Net_Installer : Tool_API    
	{				
		public Jni4Net_Installer()
		{			
			config("Jni4Net",
				   "http://switch.dl.sourceforge.net/project/jni4net/0.8.6/jni4net-0.8.6.0-bin.zip".uri(),
				   @"bin/proxygen.exe");
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