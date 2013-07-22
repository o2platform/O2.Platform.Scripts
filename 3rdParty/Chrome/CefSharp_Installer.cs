using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs 
{
	public class CefSharp_Installer_Test
	{
		public void test()
		{
			new CefSharp_Installer().start(); 
		}
	}
	public class CefSharp_Installer : Tool_API 
	{	
		public CefSharp_Installer() : this(true)
		{
		}
		
		public CefSharp_Installer(bool installNow)
		{
			config("CefSharp", "CefSharp.1.19", "CefSharp-1.19.0.7z");			
    		Install_Uri = "https://github.com/downloads/ataranto/CefSharp/CefSharp-1.19.0.7z".uri();    		
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
				return Install_Dir.pathCombine("CefSharp-1.19.0\\CefSharp.Wpf.Example.exe").startProcess();
			return null;
		}		
	}
}