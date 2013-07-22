using System.Diagnostics;
using FluentSharp.CoreLib;
//O2File:Tool_API.cs


namespace O2.XRules.Database.APIs
{
	public class NuGet_Installer_Test
	{
		public void test()
		{
			new NuGet_Installer().start(); 
		}
	}
	public class NuGet_Installer : Tool_API 
	{	 
		public NuGet_Installer()
		{
			config("NuGet", 
				   "https://www.nuget.org/nuget.exe".uri(),
				   "nuget.exe");    		    
    		install_JustDownloadFile_into_TargetDir();
		}
		
		public Process start()
		{
			if (isInstalled())
				return this.Executable.startProcess();
			return null;
		}		
	}
}