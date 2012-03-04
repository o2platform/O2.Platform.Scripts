using System;
using System.Linq;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;
//O2File:WatiN_IE_ExtensionMethods.cs
//O2File:Tool_API.cs
//O2File:API_GuiAutomation.cs 
//O2Ref:White.Core.dll 
//O2Ref:UIAutomationClient.dll 

namespace O2.XRules.Database.APIs
{
	public class GitExtensions : Tool_API 
	{	
		public bool unInstallIfAlreadyInstalled { get; set; }
		
		public GitExtensions() : this(true)
		{
		}
		
		public GitExtensions(bool installNow)
		{
			ToolName = "GitExtensions"; 
    		Version = "GitExtensions 2.17";
    		Install_File = "GitExtensions217Setup.msi";
    		//Install_Uri = ";
    		Install_Dir = @"C:\Program Files\GitExtensions\";
    		if (installNow)
    			install();    		
		}
		
		// need to convert to Tool_API format (this will uninstall
		public bool install()
		{			
			"Installing {0}".info(ToolName);
			var downloadUrl = "http://downloads.sourceforge.net/project/gitextensions/Git%20Extensions/Version%202.17/GitExtensions217Setup.msi?r=http%3A%2F%2Fsourceforge.net%2Fprojects%2Fgitextensions%2Ffiles%2FGit%2520Extensions%2FVersion%25202.17%2F&ts=1300699408&use_mirror=netcologne"; 
			var targetDir = @"..\_O2Downloads".tempDir(false).fullPath() ; 
			var targetFile = targetDir.pathCombine(downloadUrl.uri().Segments.Last());
			downloadUrl.download(targetFile);
			var process = targetFile.startProcess();
			var guiAutomation = new API_GuiAutomation(process);
			guiAutomation.button("Next").click(); 
			if (guiAutomation.hasButton("Repair"))
			{
				"App is already installed".error();	
				if (unInstallIfAlreadyInstalled)
				{
					guiAutomation.button("Repair").click();
					guiAutomation.button("Next").click();  
					guiAutomation.button("Finish",20).click();	
				}
			} 
			else
			{
			  	guiAutomation.button("Next").click(); 
				guiAutomation.button("Next").click();  
				guiAutomation.button("Next").click(); 
				guiAutomation.button("Next").click();  
				guiAutomation.button("Install").click();		
				guiAutomation.button("Finish",20).click();	
			};
			return false;
		}
		
	}
}