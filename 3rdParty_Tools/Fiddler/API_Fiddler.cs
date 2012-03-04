// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using O2.Interfaces.O2Core; 
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.Windows;
using White.Core.UIItems.WindowItems;
//O2File:API_GuiAutomation.cs
//O2File:Tool_API.cs


//using O2.XRules.Database.Utils;
//_O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.APIs  
{	
 
	public class API_Fiddler : Tool_API
    {    
    	public string Fiddler_Exe {get;set;}    	    	
    	public string Uninstall_Exe {get;set;}
    	public Process Fiddler_Process {get;set;}
    	public API_GuiAutomation Fiddler_GuiAutomation {get;set;}
    	public Window Fiddler_Window {get;set;}
    	
    	int PROCESS_START_MAX_SLEEP_SECONDS = 10;
    	string FIDDLER_MAIN_WINDOW_TITLE = "Fiddler - HTTP Debugging Proxy";
    	
    	public API_Fiddler()
		{
			config("Fiddler", "Fiddler 2 Beta", "Fiddler2BetaSetup.exe");			
    		Install_Uri = "http://www.fiddler2.com/dl/Fiddler2BetaSetup.exe".uri();    		    		    		
    		Fiddler_Exe = Install_Dir.pathCombine("fiddler.exe"); 
    		Install_Dir = @"C:\Program Files\Fiddler2";
    		Uninstall_Exe = Install_Dir.pathCombine("uninst.exe");
		}
		 
		public override bool isInstalled()
    	{
    		return Fiddler_Exe.fileExists();
    	}
    		 
		public bool install()
		{			
			if (this.isInstalled().isFalse())
			{  
				"[API_Fiddler] Starting Fiddler installation process".info();
				var fiddlerInstaller = this.installerFile(); 
				var guiAutomation = new API_GuiAutomation();
				guiAutomation.launch(fiddlerInstaller);
				var installWindow = guiAutomation.window("Fiddler2 Setup: License Agreement");
				installWindow.button("I Agree").click();   
				var installTextBox = installWindow.textBox(@"C:\Program Files\Fiddler2");
				installTextBox.set_Text(this.Install_Dir); 
				if (installTextBox.get_Text()==this.Install_Dir)
				{
					installWindow.button("Install").click();
				} 
				else
					"[API_Fiddler] in install automation, rrror setting the target directory".error(); 				
				installWindow.button("Close").click();													
				"[API_Fiddler] Fiddler installation complete".info();	
			}
			if (this.isInstalled())
			{
				"[API_Fiddler] Fiddler is installed in folder:{0}".info(this.Install_Dir);
				return true;
			}
			"[API_Fiddler] Counld NOT find Fiddler installation in folder:{0}".info(this.Install_Dir);
			return false;
		}
	
		public override bool unInstall()
		{
			if (this.isInstalled() && this.Uninstall_Exe.fileExists())
			{
				"[API_Fiddler] Starting Fiddler UnInstall process".info();
				var guiAutomation = new API_GuiAutomation();
				guiAutomation.launch(this.Uninstall_Exe);
				 
				var processID = guiAutomation.TargetProcess.Id;
				//var finder = new AutomationElementFinder(AutomationElement.RootElement);    
				var uninstallWindow = guiAutomation.desktopWindow("Fiddler2 Uninstall: Confirmation");
				uninstallWindow.button("Uninstall").click();
				uninstallWindow.button("Close").click(); 				
				"[API_Fiddler] Fiddler UnInstall complete".info();
				if (this.isInstalled().isFalse())
					return true;
				"[API_Fiddler] There was an error in the UnInstall process (since fiddler is still installed)".info();
				return false;
			}
			"[API_Fiddler] in unInstall, Fiddler is not installed, so nothing to do".debug();
			return false;
		}
				
		
		public API_Fiddler start()
		{		
			install();
			if (Fiddler_Process.notNull())
			{
				"[API_Fiddler] in start, the Fiddler_Process is already mapped to a running process. Stopping request".error();
				return this;
			}
			attach();
			if (Fiddler_Process.isNull())		// means we were NOT able find a running instance and get its process object
			{		
				if (Fiddler_Exe.fileExists().isFalse())
				{
					"[API_Fiddler] Could not find Fiddler Exe file at location: {0}".error(Fiddler_Exe);
					return null;
				}
				Fiddler_Process = Processes.startProcess(Fiddler_Exe);
			}									
			Fiddler_GuiAutomation = new API_GuiAutomation(Fiddler_Process);
			if (Fiddler_GuiAutomation.isNull())
			{
				"[API_Fiddler] Could not set Fiddler_GuiAutomation".error();
				return null;				
			}				
			Fiddler_Window = Fiddler_GuiAutomation.window(FIDDLER_MAIN_WINDOW_TITLE);
			if (Fiddler_Window.isNull())
			{
				"[API_Fiddler] Could not get Fiddler main window with title: {0}".error(FIDDLER_MAIN_WINDOW_TITLE);
				return null;				
			}			
			return this;
		}				
    
    	public API_Fiddler attach()
    	{
    		this.Fiddler_Process = null;
    		// try to attach to a running instance of WebScarab
			var mainWindowTitle = "Fiddler";// - HTTP Debugging Proxy";
			this.Fiddler_Process = Processes.getProcessCalled(mainWindowTitle);  
			if (this.Fiddler_Process.notNull())						
				"[API_Fiddler] Found a running instance of Fiddler (process ID: {0})".info(this.Fiddler_Process.Id);
				
			return this;				
    	}
    }
    
    public static class API_Fiddler_ExtensionMethods
    {
    	
		public static API_Fiddler startAndSync(this API_Fiddler fiddler, Control control)
		{
			fiddler.start();
			fiddler.Fiddler_Window.syncWithControl(control);
			return fiddler;
		}
		
		public static API_Fiddler restartAndSync(this API_Fiddler fiddler, Control control)
    	{
    		fiddler.stop();
    		return fiddler.startAndSync(control);    		
    	}
    	
    	public static API_Fiddler restart(this API_Fiddler fiddler)
    	{
    		fiddler.stop();
    		return fiddler.start();    		
    	}
    	
    	public static API_Fiddler stop(this API_Fiddler fiddler)
    	{
    		"[API_Fiddler] Stopping Fiddler".info();
    		if (fiddler.Fiddler_Process.notNull())
    		{
    			fiddler.Fiddler_Process.stop();
    			fiddler.waitForExit();
    			fiddler.Fiddler_Process = null;
    		}
    		return fiddler;
    	}
    	
    	public static API_Fiddler waitForExit(this API_Fiddler fiddler)
    	{
    		fiddler.Fiddler_Process.WaitForExit();
    		return fiddler;
    	}
    	
    	public static bool started(this API_Fiddler fiddler)
    	{
    		return (fiddler.Fiddler_Process.notNull() && 
					fiddler.Fiddler_Process.HasExited.isFalse() && 
					fiddler.Fiddler_Process.MainWindowHandle != IntPtr.Zero);
    	}
    	
    }
    /*
    public static class API_WebScarab_ExtensionMethods_Gui
    {
    
    	public static API_WebScarab bringToFront(this API_WebScarab webScarab)
    	{    		
    		webScarab.InputSimulator.bringToFront(webScarab.WebScarab_Process);
    		return webScarab;
    	}
    	
    	public static API_WebScarab alwaysOnTop(this API_WebScarab webScarab)
    	{
    		return webScarab.alwaysOnTop(true);
    	}
    	public static API_WebScarab alwaysOnTop(this API_WebScarab webScarab, bool value)
    	{    		
    		webScarab.InputSimulator.alwaysOnTop(webScarab.WebScarab_Process,value);  
    		return webScarab;
    	}
    	
    	
    	public static API_WebScarab moveWindow(this API_WebScarab webScarab, int left, int top, int width, int height)
    	{    		
    		webScarab.InputSimulator.moveWindow(webScarab.WebScarab_Process, left, top, width, height);
    		return webScarab;
    	}
        	
    	public static API_WebScarab startAndSync(this API_WebScarab webScarab, Control control)
    	{
    		webScarab.start();
    		webScarab.syncGuiPositionWithControl(control);
    		return webScarab;
    	}
    	    	
    	
    	// this should only be executed once per Form
    	public static API_WebScarab syncGuiPositionWithControl(this API_WebScarab webScarab, Control control)
    	{
		    Action moveToControl = 
				()=>{
						webScarab.alwaysOnTop(true); 
						var xPos =  control.PointToScreen(System.Drawing.Point.Empty).X;
						var yPos =  control.PointToScreen(System.Drawing.Point.Empty).Y;
						var width = control.width();
						var height = control.height();
						webScarab.moveWindow(xPos, yPos, width, height);  
					};	
						
			control.parentForm().Move += 
				(sender,e)=> moveToControl();
			 
			control.Resize +=  
				(sender,e)=> moveToControl();
			moveToControl();							
			return webScarab;
		}
		
    }
    */
    
    
}
