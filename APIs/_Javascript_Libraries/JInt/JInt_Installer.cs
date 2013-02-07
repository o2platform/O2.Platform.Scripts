using System;
using System.Diagnostics;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;  
//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class testInstall
	{
		public static void test()  
		{
			new Jint_Installer().start(); 
		}
	}
	 
	public class Jint_Installer : Tool_API    
	{				
		public Jint_Installer()
		{			
			config("Jint", 
				   "Jint - Binaries.zip", 
				   "http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName=jint&DownloadId=270401&FileTime=129578982310270000&Build=20091".uri(),
				   "jint.dll");
			installFromZip_Web();	       		
		}			
		
		public Process start()
		{
			if (this.isInstalled())
				return this.Install_Dir.startProcess(); 
			return null;
		}		
	}
}