using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class NerdDinner_Installer_test
	{
		public static void test()  
		{
			new NerdDinner_Installer().start(); 
		}
	}
	 
	public class NerdDinner_Installer : Tool_API    
	{				
		public NerdDinner_Installer()
		{			
			config("NerdDinner", 
				   "NerdDinner.zip", 
				   "http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName=nerddinner&DownloadId=123725&FileTime=129192971514370000&Build=19692".uri(),
				   "NerdDinner_2.0\\NerdDinner.sln");
			installFromZip_Web();
		}	
		//
		
		public Process start()
		{
			if (this.isInstalled())
				return this.Install_Dir.startProcess(); 
			return null;
		}		
	}
}