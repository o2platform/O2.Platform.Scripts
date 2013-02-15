using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Install_Firefox_Test 
	{
		public void install()
		{
			new Install_Firefox().start();
		}
	}
	public class Install_Firefox : Tool_API
	{				 
		public Install_Firefox()
		{
/*			ToolName = "Firefox";
    		Version = "Firefox 17.0.1"; 
    		Install_File = "Firefox setup 17.0.1.exe";
    		Install_Uri = "http://download.cdn.mozilla.net/pub/mozilla.org/firefox/releases/18.0.2/win32/en-US/Firefox%20Setup%2018.0.2.exe".uri();
    		Install_Dir = ProgramFilesFolder.pathCombine("Mozilla Firefox");
    		Executable 	= Install_Dir.pathCombine("Firefox.exe");
    		VersionWebDownload = "";   
    		this.InstallProcess_Arguments = "-ms";
    		*/
    		
    		config("Firefox", 
				   "https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463C-AFF1-A69D9E530F96%7D%26iid%3D%7B1D1703E5-E132-07F9-5950-9403775F1232%7D%26lang%3Den%26browser%3D4%26usagestats%3D0%26appname%3DGoogle%2520Chrome%26needsadmin%3Dfalse%26installdataindex%3Ddefaultbrowser/update2/installers/ChromeSetup.exe".uri(),				   
				   ProgramFilesFolder.pathCombine(@"Mozilla Firefox\Firefox.exe"));
				   this.InstallProcess_Arguments = "-ms";
    		installFromMsi_Web();
		}
		
		
		/*public bool install()
		{			
			return installFromMsi_Web();			
		}*/
		
		public bool uninstall()
		{
			var uninstallExe = Install_Dir.pathCombine(@"\uninstall\helper.exe");
			Processes.startProcess(uninstallExe,"");
			// check if this also deletes the firefox folder
			return isInstalled();
		}
		
		public Process start()
		{
			if (isInstalled())
				return Executable.startProcess();
			return null;
		}
		
	}
}