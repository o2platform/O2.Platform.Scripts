using System.Diagnostics;
using FluentSharp.CoreLib;
//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Install_Snoop_Test
	{
		public void test()
		{
			new Snoop_Installer().start();
		}
	}
	public class Snoop_Installer : Tool_API 
	{	
		public Snoop_Installer() : this(true)
		{
		}
		
		public Snoop_Installer(bool installNow)
		{
			config("Snoop", "Snoop v2.7.1", "Snoop.zip");	
			Install_Uri = "http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName=snoopwpf&DownloadId=500789&FileTime=129938679247600000&Build=20928".uri();
    		//Install_Uri = "https://github.com/downloads/cplotts/snoopwpf/Snoop.zip".uri();
    		if (installNow)
    			install();		
		}
		
		
		public bool install()
		{
			"Installing {0}".info(ToolName);
			return installFromZip_Web(); 						
		}
		
		public Process start()
		{
			if (install())
				return Install_Dir.pathCombine("Snoop\\snoop.exe").startProcess();
			return null;
		}		
	}
}