using System;
using System.Diagnostics;
using FluentSharp.CoreLib;
//O2File:Tool_API.cs


namespace O2.XRules.Database.APIs
{
	public class Chrome_Installer_Test
	{
		public void test() 
		{
			new Chrome_Installer().start(); 
		}
	}
	public class Chrome_Installer : Tool_API 
	{	 
		public Chrome_Installer()
		{
			var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var chromeExePath = localAppData.pathCombine(@"Google\Chrome\Application\Chrome.exe");
									
			config("Chrome", 
				   "https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463C-AFF1-A69D9E530F96%7D%26iid%3D%7B1D1703E5-E132-07F9-5950-9403775F1232%7D%26lang%3Den%26browser%3D4%26usagestats%3D0%26appname%3DGoogle%2520Chrome%26needsadmin%3Dfalse%26installdataindex%3Ddefaultbrowser/update2/installers/ChromeSetup.exe".uri(),				   
				   chromeExePath);
			//this.showInfo();			
    		installFromMsi_Web();
		}
		
		public Process start()
		{
			if (isInstalled())
				return this.Executable.startProcess();
			return null;
		}		
	}
}