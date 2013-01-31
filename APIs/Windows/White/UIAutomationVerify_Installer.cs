using System;
using System.Diagnostics;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;  
//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class UIAutomationVerify_Installer_test
	{
		public static void test()  
		{
			new UIAutomationVerify_Installer().start(); 
		}
	}
	 
	public class UIAutomationVerify_Installer : Tool_API    
	{				
		public UIAutomationVerify_Installer()
		{			
			config("UI Automation Verify", 
				   "UIAVerify2.0_x86.zip", 
				   "http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName=uiautomationverify&DownloadId=220103&FileTime=129453037998770000&Build=20006".uri(),
				   "VisualUIAVerify.exe");
			installFromZip_Web();	       		
		}			
		
		public Process start()
		{
			if (this.isInstalled())
				return this.Executable.startProcess(); 
			return null;
		}		
	}
}