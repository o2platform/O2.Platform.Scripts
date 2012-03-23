// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using White.Core.UIItems.WindowItems;
//O2File:API_GuiAutomation.cs
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:RibbonControlsLibrary.dll 
//O2Ref:White.Core.dll

using O2.XRules.Database.Utils;
//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.APIs
{
    public class API_GUI_Automation_VisualStudio_2010
    {   
    	public string VisualStudioExe { get; set; }
    	
    	public API_GuiAutomation VS_Process { get; set; }
    	public API_GuiAutomation GUI { get; set; }
		public Window VS_MainWindow  { get; set; }

		//public static string MAIN_WINDOW_TITLE = "Start Page - Microsoft Visual Studio";
		
		public API_GUI_Automation_VisualStudio_2010()
		{
			VisualStudioExe = @"C:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe";
		}
		
		public API_VisualStudio_2010 attach()
		{
			GUI = VS_Process = new API_GuiAutomation("devenv");				
			if (VS_Process.TargetProcess.notNull())
				VS_MainWindow = VS_Process.windows()[0];//MAIN_WINDOW_TITLE);
			else
				start();
			return this;
		}
		
		public API_VisualStudio_2010 start()
		{
			 GUI = VS_Process = VisualStudioExe.startProcess().automation(); 
			 for(int i =0; i < 10; i++)
			 {
			 	var windows = VS_Process.windows();
			 	if (windows.notNull() && windows.size()>0)
			 	{
			 		VS_MainWindow = VS_Process.windows()[0];
			 		break;
			 	}
			 	this.sleep(500);
			 }
			 if (VS_MainWindow.isNull())
			 	"In API_VisualStudio_2010 could not find the main Window".error();
			 return this;
		}			    	
    	
    	public API_VisualStudio_2010 close()
    	{
    		VS_Process.TargetProcess.close(); 
    		return this;
    	}
    	
    	public API_VisualStudio_2010 closeInNSeconds(int seconds)
    	{
    		VS_Process.TargetProcess.closeInNSeconds(seconds); 
    		return this;
    		
    	}
    	public API_VisualStudio_2010 move(int left, int top, int width, int height)
    	{
    		VS_MainWindow.move(left, top, width, height);
    		return this;
    	}
    	
    	public API_VisualStudio_2010 bringToFront()
    	{
    		VS_MainWindow.bringToFront();
    		return this;
    	}
    	
    }
    
    public static class API_VisualStudio_2010_ExtensionMethods_SolutionsAndProjects
    {   
		public static API_VisualStudio_2010 open_Solution(this  API_VisualStudio_2010 vStudio, string solutionPath)			
		{
			vStudio.VS_MainWindow.menu("File").click()//.mouse().click()
								     .menu("Open").click()
								     .menu("Project/Solution...").mouse().click();  
								     
			var openProject = vStudio.VS_Process.window("Open Project");
			if (openProject.notNull())
			{
				openProject.textBox("Object name:").set_Text(solutionPath);
				openProject.button("Open").mouse().click();								    
									
				vStudio.saveChanges();
				vStudio.skipTargetOlderVersionRequest();
			}
			return vStudio;
		}
		
    	public static API_VisualStudio_2010 open_WebSite(this  API_VisualStudio_2010 vStudio, string folder, bool savePreviousSolution)
    	{
    		try
    		{   
    			//Menu item
				vStudio.VS_MainWindow.menu("File").click()//.mouse().click()
								     .menu("Open").click()
								     .menu("Web Site...").mouse().click();				
				var openWebSiteWindow = vStudio.VS_Process.window("Open Web Site");
				//Popup box to chose website to open
				if (openWebSiteWindow.notNull())
				{
					openWebSiteWindow.textBox("Folder:").set_Text(folder);
					openWebSiteWindow.label("Folder:").click();
					openWebSiteWindow.button("Open").mouse().click();  
												
					vStudio.saveChanges();				
					vStudio.skipTargetOlderVersionRequest();
				}
			}
			catch(Exception ex)
			{
				ex.log("in API_VisualStudio_2010 openWebSite");
			}
			return vStudio;
		}		
		
		public static API_VisualStudio_2010 save_Solution(this  API_VisualStudio_2010 vStudio)
		{
			return vStudio.save_SolutionAs(null);
		}
		
		public static API_VisualStudio_2010 save_SolutionAs(this  API_VisualStudio_2010 vStudio, string saveSolutionTo)
		{
			try
			{
					//select 1st node from Solutions so the  menu's are updated
				var solutionExplorer = vStudio.VS_MainWindow.treeView("Solution Explorer");
				if (solutionExplorer.notNull())
				{
					var treeNodes =  solutionExplorer.treeNodes(); 				
					treeNodes[0].mouse().click();
				
					
					// click on Save solution or SaveAs
					if (saveSolutionTo.isNull())
						vStudio.VS_MainWindow.menu("File").mouse().click()
					     	   .menus()[8].mouse().click();
					else
						vStudio.VS_MainWindow.menu("File").mouse().click()
					     	   .menus()[9].mouse().click();
					
					vStudio.saveChanges(true);
					vStudio.saveFileAs(saveSolutionTo);													
																
				}
			}
			catch(Exception ex)
			{
				ex.log("in API_VisualStudio_2010 openWebSite");
			}
			return vStudio;
		}
		
		public static API_VisualStudio_2010 close_Solution(this  API_VisualStudio_2010 vStudio)
		{	
			return vStudio.close_Solution(true);
		}
		
		public static API_VisualStudio_2010 close_Solution(this  API_VisualStudio_2010 vStudio, bool savePreviousSolution)			
		{
			vStudio.VS_MainWindow.menu("File").mouse().click() 
								 .menu("Close Solution").mouse().click();
			vStudio.saveChanges(savePreviousSolution);
			return vStudio;
		}				
        
        public static API_VisualStudio_2010 saveChanges(this  API_VisualStudio_2010 vStudio)
        {
        	return vStudio.saveChanges(true);
        }
	    public static API_VisualStudio_2010 saveChanges(this  API_VisualStudio_2010 vStudio, bool saveChangesValue)
	    {
	    	var confirmSave = vStudio.VS_Process.window("Microsoft Visual Studio");				
			if (confirmSave.notNull() && confirmSave.label("\nSave changes to the following items?").notNull())
			{
				confirmSave.clickYes(saveChangesValue);
				vStudio.saveFileAs(saveChangesValue);
			}
			return vStudio;
		}
		
		public static API_VisualStudio_2010 saveFileAs(this  API_VisualStudio_2010 vStudio, bool saveChangesValue)
		{
			return vStudio.saveFileAs(null,saveChangesValue);
		}
		
		public static API_VisualStudio_2010 saveFileAs(this  API_VisualStudio_2010 vStudio,string filePath)
		{
			return vStudio.saveFileAs(filePath,true);
		}
		
		public static API_VisualStudio_2010 saveFileAs(this  API_VisualStudio_2010 vStudio,string filePath, bool saveChangesValue)
		{
			var saveFileAs = vStudio.VS_Process.window("Save File As");
			if (saveFileAs.notNull())
				if(saveChangesValue)
				{
					if (filePath.valid())
						saveFileAs.textBox("Object name:").set_Text(filePath);
					saveFileAs.button("Save").mouse().click();
					vStudio.clickButton("Microsoft Visual Studio","Yes");
				}
				else
					saveFileAs.button("Cancel").mouse().click();
			return 	vStudio;											  		
		}
		
		public static Window clickYes(this  Window window)
		{
			return window.clickYes(true);
		}
		
		public static Window clickYes(this  Window window, bool clickYes)
		{
			if (window.notNull())
			{
				if (clickYes)
					window.button("Yes").mouse().click(); 	
				else
					window.button("No").mouse().click(); 
			}
			return window;
		}
		
		public static API_VisualStudio_2010 clickButton(this  API_VisualStudio_2010 vStudio, string windowName, string buttonName)
		{
			var window = vStudio.VS_Process.window(windowName);
			if (window.notNull())
			{
				var button = window.button(buttonName); 
				if (button.notNull())
					button.mouse().click();
			}
			return vStudio;
		}
		
		public static API_VisualStudio_2010 skipTargetOlderVersionRequest(this  API_VisualStudio_2010 vStudio)
		{
			var targetOlderVersion = vStudio.VS_Process.window("Web Site targeting older .Net Framework Found");				
			if (targetOlderVersion.notNull())
				targetOlderVersion.button("No").mouse().click(); 
			return vStudio;
		}
	}
	
	public static class API_VisualStudio_2010_ExtensionMethods_MenuItems
    {   
    	public static API_VisualStudio_2010 show_PropertiesWindow(this API_VisualStudio_2010 vStudio)
    	{
    		var propertiesWindow = vStudio.VS_MainWindow.panel("Properties");
    		if (propertiesWindow.isNull())
				vStudio.VS_MainWindow.menu_Click("View","Properties Window"); 
			return vStudio;
		}
	}
	
	public static class API_VisualStudio_2010_ExtensionMethods_ProjectType_Website
    {   
    	public static string localWebsiteUrl(this API_VisualStudio_2010 vStudio)
    	{
    		vStudio.show_PropertiesWindow();
			var propertyGrid = vStudio.VS_MainWindow.panel("Properties").propertyGrid();
			if (propertyGrid.notNull())
			{
				var portNumber = propertyGrid.properties().value("Port number");
				var virtualPath = propertyGrid.properties().value("Virtual path");
				if (portNumber.valid() && virtualPath.valid())
					return "http://127.0.0.1:{0}{1}".format(portNumber,virtualPath); 
			}
			return null;
		}
	}
	
	public static class API_VisualStudio_2010_ExtensionMethods_ProjectType_RunAndDebug
    {
		public static API_VisualStudio_2010 run(this API_VisualStudio_2010 vStudio)
		{
			return vStudio.startDebugging();
		}
		
		public static API_VisualStudio_2010 startDebugging(this API_VisualStudio_2010 vStudio)
		{
			vStudio.VS_MainWindow.menu_Click("Debug", "Start Debugging");			
			vStudio.GUI.button_Click("Debugging Not Enabled", "OK");			
			return vStudio;
		}
	}
	
	public static class API_VisualStudio_2010_ExtensionMethods_ProjectType_Misc
    {
		public static API_VisualStudio_2010 close_VisualStudio(this API_VisualStudio_2010 vStudio)
		{
			vStudio.VS_MainWindow.menu_Click("File","Exit");
			return vStudio;
		}
    }
	
}
