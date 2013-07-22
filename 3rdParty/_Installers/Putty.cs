using System.Diagnostics;
using FluentSharp.CoreLib;
//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Install_Putty_Test
	{
		public void test()
		{
			new Putty().start();
		}
	}
	public class Putty : Tool_API 
	{	
		public Putty() : this(true)
		{
		}
		
		public Putty(bool installNow)
		{
			config("Putty", "Putty 0.61", "putty.zip");			
    		Install_Uri = "http://the.earth.li/~sgtatham/putty/latest/x86/putty.zip".uri();
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
				return Install_Dir.pathCombine("Putty.exe").startProcess();
			return null;
		}		
	}
}