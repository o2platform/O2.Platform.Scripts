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
				   "http://selenium.googlecode.com/files/selenium-dotnet-2.33.0.zip".uri(),
				   @"Selenium\WebDriver.dll");
    		installFromZip_Web(); 		
		}
		
				
		
		public Process start()
		{			
			if (isInstalled())				
				return this.Executable.parentFolder().startProcess();
			return null;
		}		
	}
}