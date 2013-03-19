using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Installer_Test
	{
		public void test() 
		{
			new Cassini_Installer().start(); 
		}
	}
	public class Cassini_Installer : Tool_API 
	{			
		public Cassini_Installer()
		{			    		    	
			config("Cassini", 
				   "CassiniDev 3.5.1.8-4.1.0.8 release.zip",
			   	   "http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName=cassinidev&DownloadId=123473&FileTime=129287707016900000&Build=20006".uri(),				   
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