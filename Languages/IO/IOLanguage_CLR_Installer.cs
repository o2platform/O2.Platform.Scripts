using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
//O2File:Tool_API.cs
using O2.XRules.Database.Utils;


namespace O2.XRules.Database.APIs
{
	public class Installer_Test
	{
		public void test() 
		{
			new IOLanguage_CLR_Installer().start(); 
		}
	}
	public class IOLanguage_CLR_Installer : Tool_API 
	{	 
		public IOLanguage_CLR_Installer()
		{												 
			config("IOLanguage",  
				   "SynrcIo-1.0.rar",
				   "http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName=io&DownloadId=65740&FileTime=128844177523700000&Build=20154".uri(),
				   @"ioclr\bin\IoCLI.exe");
				   
    		installFromZip_Web();    		
			/*if (this.Executable.fileExists().isFalse())
			{				
				this.Install_Dir.pathCombine("IoLanguage-2012.11.09-win32.exe")
								.startProcess()
								.WaitForExit();
			}*/
		}
		
		public Process start()
		{
			if (isInstalled())
				return this.Executable.startProcess();
			return null;
		}		
	}
}