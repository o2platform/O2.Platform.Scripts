using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class EasyHook_Installer_Test
	{
		public static void test()  
		{
			new EasyHook_Installer().start(); 
		}
	}
	 
	public class EasyHook_Installer : Tool_API    
	{				
		public EasyHook_Installer()
		{			
			config("EasyHook", 
				   "EasyHook-2.7.4761.0-Binaries.zip", 
				   "http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName=easyhook&DownloadId=371803&FileTime=130025275074470000&Build=20748".uri(),
				   "NetFX4.0\\README.txt");
			installFromZip_Web();	       		
		}			
		
		public Process start()
		{
			if (this.isInstalled())
				return this.Executable.startProcess(); 
			return null;
		}		
	}
}