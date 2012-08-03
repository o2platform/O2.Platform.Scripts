using System;
using System.Diagnostics;
using O2.Kernel; 
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;  
using O2.XRules.Database.Utils;
//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class testInstall
	{
		public static void test()  
		{
			new yEd_Install();//.start(); 
		}
	}
	 
	public class yEd_Install : Tool_API    
	{				
		public yEd_Install()
		{			
			config("yEd", 				   
				   "http://www.yworks.com/products/yed/demo/yEd-3.9.2_setup.exe".uri(),
				   "yEd.exe");
			DownloadedInstallerFile = download(Install_Uri);
			DownloadedInstallerFile.startProcess();			
		}	
		//
		
		/*public Process start()
		{
			if (this.isInstalled())
				return this.Executable.startProcess(); 
			return null;
		}		*/
	}
}