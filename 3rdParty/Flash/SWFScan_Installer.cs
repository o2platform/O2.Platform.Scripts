using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Installer_Test
	{
		public void test()
		{
			new SWFScan_Installer().start();
		}
	}
	public class SWFScan_Installer : Tool_API 
	{			
		
		public SWFScan_Installer()
		{
			config("SWFScan", 
				   "http://h30499.www3.hp.com/hpeb/attachments/hpeb/sws-119/721/1/HP_FREE_TOOL_SwfScan.zip".uri(),
				   "AppScanSDK.chm");
			installFromZip_Web();
		}
		
		public Process start()
		{
			if (isInstalled())
				return Executable.startProcess();
			return null;
		}		
	}
}