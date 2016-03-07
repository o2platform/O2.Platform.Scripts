// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Management;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using FluentSharp.CoreLib;
using FluentSharp.REPL;
using FluentSharp.REPL.Controls;
using FluentSharp.WinForms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using FluentSharp.Zip;

//O2File:API_Win32_Handle_Hijack.cs
//O2Ref:Selenium\net40\WebDriver.dll
//Installer:Selenium_Installer.cs!Selenium\net40\WebDriver.dll

//O2Ref:System.Management.dll

namespace O2.XRules.Database.APIs
{
	public class API_Chrome_Hijack
	{
		public ChromeDriver 				ChromeDriver 			 { get; set; }
		public ChromeOptions 				ChromeOptions 			 { get; set; }		
		public ChromeDriverService  		ChromeDriverService		 { get; set; }
		public Process						ChromeDriverProcess		 { get; set; }
		public Process						ChromeProcess			 { get; set; }
		public Panel						Panel_Chrome			 { get; set; }
		public Panel						Panel_ChromeDriver		 { get; set; }
		public ascx_Simple_Script_Editor	WebDriver_Script_Me		 { get; set; }
		public string  						ChromeDriver_Exe		 { get; set; }				
		public string  						WebDriver_Folder 		 { get; set; }				
		public string						ChromeDriverDownloadLink { get; set; }
		
		public API_Chrome_Hijack()
		{
			ChromeDriverDownloadLink = @"http://chromedriver.storage.googleapis.com/2.21/chromedriver_win32.zip";
			WebDriver_Folder 		 = @"Selenium\net40\WebDriver.dll".assembly().location().parentFolder();
			ChromeDriver_Exe 		 = WebDriver_Folder.pathCombine("chromedriver.exe");
			ChromeOptions 			 = new ChromeOptions();
			
			ensureChromeDriverExists();
			
			ChromeDriverService 	 = ChromeDriverService.CreateDefaultService();
		}
		
		public  API_Chrome_Hijack ensureChromeDriverExists()
		{		                    			
            if (ChromeDriver_Exe.fileExists().isFalse())
            {
               var downloadedZipFile = ChromeDriverDownloadLink.download();
			   downloadedZipFile.unzip_File(WebDriver_Folder);
            }
			return this;
		} 
		
		public API_Chrome_Hijack open_ChromeDriver()
		{			
			ChromeDriver = new ChromeDriver(ChromeDriverService, ChromeOptions);
			
			//resolve driverServiceProcess
			//var fieldInfo  = (FieldInfo)typeof(DriverService).field("driverServiceProcess");
			var fieldInfo = typeof(DriverService).fieldInfo("driverServiceProcess");
			if (fieldInfo.notNull())
			{
				ChromeDriverProcess = (Process)fieldInfo.GetValue(ChromeDriverService);
			
				ChromeDriverProcess.waitFor_MainWindowHandle();
				ChromeProcess = ChromeDriverProcess.getProcessWithParentHandle();
			}
			return this;
		}
				
	}


	public static class API_Chrome_Test_ExtensionMethods
	{
		public static ChromeDriver add_Chrome(this Panel targetPanel)
		{
			return new API_Chrome_Hijack()
							.open_ChromeDriver() 
							.hijack_Chrome(targetPanel,true)
							.ChromeDriver;
		}
		
		public static API_Chrome_Hijack add_Chrome_To_Panel(this API_Chrome_Hijack chromeHijack, Panel targetPanel)
		{
			var chrome_Panel 	   = targetPanel.add_GroupBox("Chrome").add_Panel();
			var chromeDriver_Panel = chromeHijack.Panel_Chrome.parent().insert_Below(150,"Chrome WebDriver");			
						
			return chromeHijack.hijack_Chrome	   (chrome_Panel,false)
							   .hijack_ChromeDriver(chromeDriver_Panel);						
		}
		
		public static API_Chrome_Hijack hijack_Chrome(this API_Chrome_Hijack chromeHijack, Panel targetPanel, bool hijackJustViewer)
		{
			if (chromeHijack.notNull() && chromeHijack.ChromeProcess.notNull() && targetPanel.notNull())
			{
				chromeHijack.Panel_Chrome = targetPanel;
				var hijackGui = chromeHijack.Panel_Chrome.add_Handle_HijackGui(false);
				if (hijackJustViewer)
				{
					hijackGui.hijackProcessWindow(chromeHijack.ChromeProcess, 
						(mainWindowHandle)=>
							{
								// this doesn't work well now since the main chrome window (with the address bar) still gains focus)
								// need to look at how to capture and filter events from the child to the parent (and vice versa)
								//
								 var targetWindow = mainWindowHandle.child_Windows().second();								
								 return targetWindow;
								 
								//return mainWindowHandle;  // for now hijack the whole thing
								});
						
				}
				else
					hijackGui.hijackProcessMainWindow(chromeHijack.ChromeProcess);
			}
			return chromeHijack;
		}
		
		public static API_Chrome_Hijack hijack_ChromeDriver(this API_Chrome_Hijack chromeHijack, Panel targetPanel)
		{
			if (chromeHijack.notNull() && chromeHijack.ChromeDriverProcess.notNull() && targetPanel.notNull())
			{
				chromeHijack.Panel_ChromeDriver = targetPanel;
				chromeHijack.Panel_ChromeDriver.add_Handle_HijackGui(false) 
			 		    			 		   .hijackProcessMainWindow(chromeHijack.ChromeDriverProcess);
			}
			return chromeHijack;
		}
		
		public static API_Chrome_Hijack add_WebDriver_ScriptMe_To(this API_Chrome_Hijack chromeHijack, Panel targetPanel)
		{
			
			chromeHijack.WebDriver_Script_Me = chromeHijack.ChromeDriver.script_Me(targetPanel);			
			return chromeHijack;		
		}
		
		public static Win32_Handle_Hijack hijack_Process_Into_Panel(this API_Chrome_Hijack chromeHijack, Panel targetPanel, Process targetProcess)
		{
			var hijackGui = targetPanel.add_Handle_HijackGui(false)
			 				     	   .hijackProcessMainWindow(targetProcess);	
			return hijackGui;
		}
	}
	
	public static class Process_Extra_ExtensionMethods
	{					
		public static Process getProcessWithParentHandle(this Process process)
		{				
			if(process.isNotNull())
			{
				var selectQuery = "Select ProcessId from Win32_Process Where ParentProcessId = {0}".format(process.Id);;
				foreach(var proc in new ManagementObjectSearcher(selectQuery).Get())
				{	
					var procId = (int)(UInt32)proc["ProcessId"];
					var foundProcess = procId.process_WithId();
					"foundProcess: {0}".debug(foundProcess);
					return foundProcess;							
				}
				"Failed to find process with id: {0}  name: {0}".error(process.Id, process.ProcessName);
			}
			return null;
		 }		 		 		 
	}
}
	