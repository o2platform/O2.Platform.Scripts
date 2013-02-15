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
			new Apache_HttpServer_Installer().start(); 
		}
	}
	public class Apache_HttpServer_Installer : Tool_API 
	{	 
		public Apache_HttpServer_Installer()
		{			
			config("Apache_HttpServer", 
				   "http://mirrors.enquira.co.uk/apache//httpd/binaries/win32/httpd-2.2.22-win32-x86-no_ssl.msi".uri(),				   
				   ProgramFilesFolder.pathCombine(@"Apache Software Foundation\Apache2.2\bin\httpd.exe"));
    		installFromMsi_Web();
		}
		
		public Process start()
		{
			if (isInstalled())
			{
				var process = this.Executable.startProcess();
				2000.wait();
				open.browser("http://localhost:8080"); // assuming appache was installed locally
				return process;
			}
			return null;
		}		
	}
}