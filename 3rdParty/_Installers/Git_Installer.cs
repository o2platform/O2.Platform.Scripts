using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Installer_Test
	{
		public void test() 
		{
			new Git_Installer().start(); 
		}
	}
	public class Git_Installer : Tool_API 
	{			
		public Git_Installer()
		{			    		    	
			config("Git", 
			   	   "https://msysgit.googlecode.com/files/Git-1.8.1.2-preview20130201.exe".uri(),				   
			   	   ProgramFilesFolder.pathCombine(@"git\bin\sh.exe"));
			//InstallProcess_Arguments = "/SILENT";
    		installFromMsi_Web();
		}		
		public Process start()
		{
			if (isInstalled())
				return this.Executable.startProcess("--login -i");
			return null;
		}
	}
}