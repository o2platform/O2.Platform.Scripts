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
	public class Install_HacmeBank_Test
	{
		public void test()
		{
			new HacmeBank();
		}
	}
	
	public class HacmeBank : Tool_API 
	{	
		public HacmeBank() : this(true)
		{
		}
		
		public HacmeBank(bool installNow)
		{
			config("HacmeBank", "HacmeBank v2.0", "HacmeBank_v2.0 (7 Dec 08).zip");			
    		Install_Uri = "http://owasp-hacmebank.googlecode.com/files/HacmeBank_v2.0%20%287%20Dec%2008%29.zip".uri();
    		Install_Dir = @"C:\O2\Demos\HacmeBank";
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
				return Install_Dir.pathCombine("procexp.exe").startProcess();
			return null;
		}		
	}
}