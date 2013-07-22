using System.Diagnostics;
using FluentSharp.CoreLib;
//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class NMap_CmdLine_Test
	{
		public void test()
		{
			new NMap_CmdLine().start();
		}
	}
	public class NMap_CmdLine : Tool_API 
	{			
		
		public NMap_CmdLine()
		{
			config( "NMap_CmdLine", 
					"http://nmap.org/dist/nmap-6.25-win32.zip".uri(), 
					@"nmap-6.25\nmap.exe");
    		
    		installFromZip_Web(); 						    		
		}
		
		
		
		public Process start()
		{
			if (isInstalled())
				Executable.parentFolder().startProcess();
				//return Executable.startProcess();
			return null;
		}		
	}
}