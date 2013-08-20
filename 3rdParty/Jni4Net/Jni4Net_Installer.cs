using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class Jni4Net_Installer_Test
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
			config("Jni4Net_Patched",
				   "https://dl.dropboxusercontent.com/u/81532342/O2%20Files/jni4net-0.8.7.0-bin_Patched.zip".uri(),
				   //"http://switch.dl.sourceforge.net/project/jni4net/0.8.6/jni4net-0.8.6.0-bin.zip".uri(),
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