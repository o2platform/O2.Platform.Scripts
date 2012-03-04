using System;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Install_Firefox : Tool_API
	{				 
		public Install_Firefox()
		{
			ToolName = "Firefox";
    		Version = "Firefox 3.6.10"; 
    		Install_File = "Firefox Setup 3.6.10.exe";
    		Install_Dir = @"C:\Program Files\Mozilla Firefox";
    		VersionWebDownload = "";
    		
		}
		
		
		public bool install()
		{			
			return installFromMsi_Web();			
		}
		
		public bool uninstall()
		{
			var uninstallExe = Install_Dir.pathCombine(@"\uninstall\helper.exe");
			Processes.startProcess(uninstallExe,"");
			// check if this also deletes the firefox folder
			return isInstalled();
		}
		
	}
}