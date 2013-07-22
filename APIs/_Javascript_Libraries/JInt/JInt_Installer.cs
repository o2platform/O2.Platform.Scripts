using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class JInt_Installer_Test
	{
		public static void test()  
		{
			new JInt_Installer().start(); 
		}
	}
	 
	public class JInt_Installer : Tool_API    
	{				
		public JInt_Installer()
		{			
			config("Jint", 
				   "Jint - Binaries.zip", 
				   "http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName=jint&DownloadId=270401&FileTime=129578982310270000&Build=20626".uri(),
//				   "http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName=jint&DownloadId=270401&FileTime=129578982310270000&Build=20091".uri(),
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