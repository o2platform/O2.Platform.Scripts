using System.Diagnostics;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Install_Firefox_Test 
	{
		public void install()
		{
			new Firefox_Installer().start();
		}
	}
	public class Firefox_Installer : Tool_API
	{				 
		public Firefox_Installer()
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
				   "http://download.cdn.mozilla.net/pub/mozilla.org/firefox/releases/18.0.2/win32/en-US/Firefox%20Setup%2018.0.2.exe".uri(),				   
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