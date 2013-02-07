using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
//O2File:Tool_API.cs
using O2.XRules.Database.Utils;


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