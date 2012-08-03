using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class MsysGit : Tool_API 
	{	
		public MsysGit() : this(true)
		{
		}
		
		public MsysGit(bool installNow)
		{
			ToolName = "MsysGit"; 
    		Version = "Git 1.7.4";
    		Install_File = "Git-1.7.4-preview20110204.exe";
    		Install_Uri = "http://msysgit.googlecode.com/files/Git-1.7.4-preview20110204.exe".uri();
    		Install_Dir = @"C:\Program Files\Git\";
    		if (installNow)
    			install();    		
		}
		
		
		public bool install()
		{			
			"Installing {0}".info(ToolName);
			return installFromMsi_Web(); 				// cant automate this one
			//"{0} installation complete".info(ToolName);
		}
		
	}
}