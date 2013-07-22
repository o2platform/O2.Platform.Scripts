using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class IIS_Express_Installer_Test
	{
		public void test() 
		{
			new IIS_Express_Installer().start(); 
		}
	}
	public class IIS_Express_Installer : Tool_API 
	{			
		public IIS_Express_Installer()
		{			    		    	
			config("IIS Express", 
			   	   "http://download.microsoft.com/download/D/C/4/DC4EC38C-A6AA-449D-9B19-7ABC6DF72B34/iisexpress_1_11_x86_en-US.msi".uri(),				   
			   	   "IIS.exe");	
			InstallProcess_Arguments = "";   	   
    		install_JustMsiExtract_into_TargetDir();
		}		
		public Process start()
		{
			if (isInstalled())
				return this.Executable.startProcess();
			return null;
		}
	}
}