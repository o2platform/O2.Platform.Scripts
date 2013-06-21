// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic; 
using System.Windows.Forms; 
using System.Text; 
using System.Reflection; 
using System.Management;
using System.Diagnostics;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
//O2Ref:Selenium\net40\WebDriver.dll
//Installer:Selenium_Installer.cs!Selenium\net40\WebDriver.dll

//O2Ref:System.Management.dll

namespace O2.XRules.Database.APIs
{
	public class API_Chrome_Hijack
	{
		public ChromeDriver 		ChromeDriver 			 { get; set; }
		public ChromeOptions 		ChromeOptions 			 { get; set; }		
		public ChromeDriverService  ChromeDriverService		 { get; set; }
		public Process				ChromeDriverProcess		 { get; set; }
		public Process				ChromeProcess			 { get; set; }
		public string  				ChromeDriver_Exe		 { get; set; }				
		public string  				WebDriver_Folder 		 { get; set; }				
		public string				ChromeDriverDownloadLink { get; set; }
		
		public API_Chrome_Hijack()
		{
			ChromeDriverDownloadLink = @"http://chromedriver.googlecode.com/files/chromedriver_win_26.0.1383.0.zip";
			WebDriver_Folder 		 = @"Selenium\net40\WebDriver.dll".assembly().location().parentFolder();
			ChromeDriver_Exe 		 = WebDriver_Folder.pathCombine("chromedriver.exe");
			ChromeOptions 			 = new ChromeOptions();
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
			var fieldInfo  = (FieldInfo)typeof(DriverService).field("driverServiceProcess");
			ChromeDriverProcess = (Process)fieldInfo.GetValue(ChromeDriverService);
			
			ChromeDriverProcess.waitFor_MainWindowHandle();
			ChromeProcess = ChromeDriverProcess.getProcessWithParentHandle();
		
			return this;
		}
		
		/*public IntPtr startChromeDriver()
		{
			"[setup_Chrome] start".info();
			//var selenium = new API_Selenium();
			//ChromeDriver = selenium.setTarget_Chrome();
			//ChromeDriver chromeDriverProcess = "ChromeDriver".process_WithName();												
			//chromeHandle= getProcessWithParentHandle(chromeDriverProcess.Id).MainWindowHandle;
			return chromeHandle;
		}*/
		
		/*Func<IntPtr, IWebDriver> setup_Chrome =
	(targetHandle)=>{
						"[setup_Chrome] start".info();
						var selenium = new API_Selenium();
						selenium.setTarget_Chrome();
						chromeDriverProcess = "ChromeDriver".process_WithName();												
						chromeHandle= getProcessWithParentHandle(chromeDriverProcess.Id).MainWindowHandle;
						chromeHandle.setParent(targetHandle);
						WinAPI.ShowWindow(chromeHandle, WinAPI.ShowWindowCommands.ShowMaximized);													
						"[setup_Chrome] done".info();
						return selenium.WebDriver; 			 			
					};*/
	
	}

	public static class API_Chrome_Test_ExtensionMethods
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
				};
				"Failed to find process with id: {0}  name: {0}".error(process.Id, process.ProcessName);
			}
			return null;
		 }		 		 		 
	}
}
	