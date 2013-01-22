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
			ToolName = "Firefox";
    		Version = "Firefox 17.0.1"; 
    		Install_File = "Firefox setup 17.0.1.exe";
    		Install_Uri = "http://download.cdn.mozilla.net/pub/mozilla.org/firefox/releases/17.0.1/win32/en-US/Firefox setup 17.0.1.exe".uri();
    		Install_Dir = ProgramFilesFolder.pathCombine("Mozilla Firefox");
    		Executable 	= Install_Dir.pathCombine("Firefox.exe");
    		VersionWebDownload = "";   
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