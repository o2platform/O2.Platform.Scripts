using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
//O2File:Tool_API.cs
//O2File:API_GuiAutomation.cs 
//O2Ref:White.Core.dll 
//O2Ref:UIAutomationClient.dll 

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs
using O2.XRules.Database.Utils;


namespace O2.XRules.Database.APIs
{
	public class TortoiseSVN_Test
	{
		public void test()
		{
			new TortoiseSVN();//.start(); 
		}
	}
	public class TortoiseSVN : Tool_API 
	{	
		public TortoiseSVN() : this(true)
		{
		}
		
		public TortoiseSVN(bool installNow)
		{
			config("TortoiseSVN", "TortoiseSVN 1.6.13.20954", "TortoiseSVN-1.6.13.20954-win32-svn-1.6.16.msi");			
    		Install_Uri = "http://downloads.sourceforge.net/project/tortoisesvn/1.6.13/Application/TortoiseSVN-1.6.13.20954-win32-svn-1.6.16.msi".uri();
    		Install_Dir = @"C:\Program Files\TortoiseSVN";
    		if (installNow)
    			install();    		
		}
		
		
		public bool install()
		{
			"Installing {0}".info(ToolName);
			startInstaller_FromMsi_Web(); 						
			if (this.Install_Process.notNull())
			{
				var guiAutomation = new API_GuiAutomation(this.Install_Process);  

				//step 1
				guiAutomation.windows()[0]
							 .button("Next >")
							 .mouse().click();
				//step 2
				var acceptTerms  = guiAutomation.windows()[0];  
				acceptTerms.radioButton("I accept the terms in the License Agreement")
						   .mouse().click() ;
				acceptTerms.button("Next >")
						   .mouse().click();  
				//step 3
				guiAutomation.windows()[0]
							 .button("Next >")
							 .mouse().click();
				//step 4
				guiAutomation.windows()[0]
							 .button("Install")
							 .mouse().click(); 
				//step5					
							for(int i = 0 ; i< 20; i ++) 
							{
								this.sleep(2000,true); // wait 2 secs and try again  
								var tortoiseGitSetup  = guiAutomation.windows()[0];  
								if (tortoiseGitSetup.button("Next >").isNull())
								{
									tortoiseGitSetup.button("Finish").mouse().click();   
									break;
								}						
							}
			}
			return isInstalled();
		}
		
		/*public Process start()
		{
			if (install())
				return Install_Dir.pathCombine("ILSpy.exe").startProcess();
			return null;
		}*/		
	}
}