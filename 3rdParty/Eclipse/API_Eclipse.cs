using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.DotNet;

using White.Core.UIItems.WindowItems;
using White.Core.UIItems.Finders;
using White.Core.Factory;
using White.Core.UIItems;
using System.Windows.Automation;
using White.Core.AutomationElementSearch;

//O2File:API_GuiAutomation.cs
//O2File:Tool_API.cs

//O2Ref:White.Core.dll
//O2Ref:Bricks.RuntimeFramework.dll
//O2Ref:UIAutomationTypes.dll
//O2Ref:UIAutomationClient.dll


namespace O2.XRules.Database.APIs
{	
	public class API_Eclipse_Test
	{
		public static void test()
		{
			new API_Eclipse().install();
		}
	}
	
	public class API_Eclipse : Tool_API
	{				
		public string Eclipse_Exe { get; set; }		
		public Process Eclipse_Process { get; set; }		
		public API_GuiAutomation GuiAutomation { get; set;}
		public API_Eclipse()
		{
			config("Eclipse", "Eclipse Helios SR1" , "eclipse-java-helios-SR1-win32.zip");
			Eclipse_Exe = Install_Dir.pathCombine(@"\eclipse\eclipse.exe");			
		}
				
		public API_Eclipse install()
		{			
			installFromZip_Web();		// install if required
			return this;
		}
		
		public override bool isInstalled()
		{
			if (Eclipse_Exe.fileExists())
			{
				"Eclipse is installed because found file: {0}".debug(Eclipse_Exe);
				return true;
			}
			"Eclipse is currently not installed".info();
			return false;
		}				
				
	}
	
	public static class API_Eclipse_ExtensionMethods
	{
		public static API_Eclipse start(this API_Eclipse eclipse)
		{
			if (eclipse.Eclipse_Exe.fileExists())
			{
				eclipse.Eclipse_Process = Processes.startProcess(eclipse.Eclipse_Exe);
				eclipse.GuiAutomation = new API_GuiAutomation(eclipse.Eclipse_Process);  
				eclipse.GuiAutomation.waitWhileBusy();
			}
			return eclipse;
		}
		
		public static API_Eclipse stop(this API_Eclipse eclipse)
		{
			eclipse.Eclipse_Process.stop();
			eclipse.Eclipse_Process.WaitForExit();
			return eclipse;
		}
		public static API_Eclipse stopInNSeconds(this API_Eclipse eclipse, int seconds)
		{
			O2Thread.mtaThread(
				()=>{
						eclipse.sleep(seconds * 1000);
						"Stopping eclipse process".info();
						eclipse.stop();
					});
			return eclipse;
		}
	}
	
	public static class API_Eclipse_ExtensionMethods_GUI
	{
		public static Window getWindow_WorkspaceLauncher(this API_Eclipse eclipse)
		{
			var automationElementInformation = eclipse.GuiAutomation.findElement_Image();			
			if (automationElementInformation.Name == "Eclipse" || automationElementInformation.Name == "Workspace Launcher")
			{
				"Found automationElementInformation of type image with name 'Eclipse'".info();
				var workSpaceLauncher = eclipse.GuiAutomation.findWindow_viaImage();			
				if (workSpaceLauncher.Name == "Workspace Launcher")
				{				
					"Found window with name 'Workspace Launcher'".info();
					return 	workSpaceLauncher;				
				}			
			}
			"[API_Eclipse] WorkspaceLauncher is not available".info();
			return null;
		}
		
		public static TextBox workspaceLauncher_get_WorkspaceLocation_TextBox(this Window workSpaceLauncher)
		{
			var textBoxId = "1001";
			return workSpaceLauncher.Get(SearchCriteria.ByAutomationId(textBoxId)) as White.Core.UIItems.TextBox;
		}
		
		public static Button workspaceLauncher_get_Ok_Button(this Window workSpaceLauncher)
		{
			var buttonText = "OK";
			return workSpaceLauncher.Get<Button>(buttonText);   
		}
		
		public static Window workspaceLauncher_accceptCurrentWorkspace(this Window workSpaceLauncher)
		{
			workSpaceLauncher.workspaceLauncher_get_Ok_Button().Click();
			return workSpaceLauncher;			
		}
		
	}
}