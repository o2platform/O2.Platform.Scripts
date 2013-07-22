using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class NetSparkerCommunity_Install_Test
	{
		public static void test()  
		{
			new NetSparkerCommunity_Install().start(); 
		}
	}
	 
	public class NetSparkerCommunity_Install : Tool_API    
	{				
		public NetSparkerCommunity_Install()
		{			
			Install_Uri = "http://www.mavitunasecurity.com/communityedition/download".uri();
			Install_File = "NetsparkerCommunityEditionSetup.exe"; 
			Install_Dir = ProgramFilesFolder;
			
			Executable = ProgramFilesFolder.pathCombine(@"Mavituna Security\Netsparker - Community Edition\Netsparker.exe");
				   			
			startInstaller_FromMsi_Web();		
			//show.info(this);
		}	
		//
		
		public Process start()
		{
			if (this.isInstalled())
				return this.Executable.startProcess(); 
			return null;
		}		
	}
}