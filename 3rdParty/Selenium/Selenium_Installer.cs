using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
//O2File:Tool_API.cs
using O2.XRules.Database.Utils;


namespace O2.XRules.Database.APIs
{
	public class Selenium_Installer_Test
	{
		public void test()
		{
			new Selenium_Installer().start(); 
		}
	}
	public class Selenium_Installer : Tool_API 
	{	
		public Selenium_Installer()
		{
			config("Selenium", 			
				   "http://selenium.googlecode.com/files/selenium-dotnet-2.28.0.zip".uri(),
				   @"Selenium\Clojure.Main.exe");    		    		
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