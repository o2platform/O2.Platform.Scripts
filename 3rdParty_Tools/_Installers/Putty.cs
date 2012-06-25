using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
//O2File:Tool_API.cs

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs
using O2.XRules.Database.Utils;

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