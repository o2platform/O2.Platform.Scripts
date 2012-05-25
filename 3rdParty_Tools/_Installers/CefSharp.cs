using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;
//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class CefSharp_Test
	{
		public void test()
		{
			new CefSharp().start(); 
		}
	}
	public class CefSharp : Tool_API 
	{	
		public CefSharp() : this(true)
		{
		}
		
		public CefSharp(bool installNow)
		{
			config("CefSharp", "CefSharp.0.11", "CefSharp-0.11-bin.7z");			
    		Install_Uri = "https://github.com/downloads/ataranto/CefSharp/CefSharp-0.11-bin.7z".uri();    		
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
				return Install_Dir.pathCombine("CefSharp-0.11-bin\\CefSharp.Wpf.Example.exe").startProcess();
			return null;
		}		
	}
}