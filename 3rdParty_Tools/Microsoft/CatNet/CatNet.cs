using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;
//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs 
{
	public class CatNet_Test
	{
		public void test()
		{
			new CatNet().start(); 
		}
	}
	public class CatNet : Tool_API 
	{	
		public CatNet() : this(true)
		{
		}
		
		public CatNet(bool installNow)
		{
			config("CatNet", "CatNet.1.1", "CatNet-0.11-bin.7z");			
    		Install_Uri = "https://github.com/downloads/o2platform/O2_for_CAT.NET/Cat.Net%201.1.zip".uri();    		
    		if (installNow)
    			install();    		
		}
		
		
		public bool install()
		{
			"Installing {0}".info(ToolName);
			return installFromZip_Web(); 						
			return false;
		}
		
		public Process start()
		{
			if (install())
				return Install_Dir.pathCombine("Cat.Net 1.1\\CATNetCmd.exe").startProcess();
			return null;
		}		
	}
}