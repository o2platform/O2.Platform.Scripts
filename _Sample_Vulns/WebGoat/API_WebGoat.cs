// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods; 
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using SHDocVw;
using WatiN.Core;
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;


//O2File:WatiN_IE_ExtensionMethods.cs    
//O2File:WatiN_IE.cs
//O2Ref:Interop.SHDocVw.dll
//O2Ref:WatiN.Core.1x.dll
//O2File:DotNet_ViewState.cs
 
namespace O2.XRules.Database.WebGoat
{
	public class API_WebGoat
    {    
    	//public string Url_Website { get; set; }    	
    	public string Dir_LocalInstallation {get;set;}
 		public Process WebGoatProcess { get; set; }
 		public WatiN_IE ie;   
 		
 		public string startBatchFile = "webgoat_8080.bat";
 		public string serverUrl = "http://localhost:8080/";
 		public string homePage = "webgoat/attack";
 		
 		public API_WebGoat()
 		{
 		}
 		
 		public API_WebGoat(WatiN_IE _ie, string localDirectoryWithWebGoatInstallation)
 		{
 			ie = _ie;
 			Dir_LocalInstallation = localDirectoryWithWebGoatInstallation; 		
 		}
 		
 		public string startWebGoatCommand()
 		{
 			if (Dir_LocalInstallation.valid())
 				return Dir_LocalInstallation.pathCombine(startBatchFile);
 			return "";
 		}
 		
 		public bool serverOnWebGoatPort()
 		{
 			return serverUrl.uri().exists();
 		}
 		
 		public string url_HomePage()
 		{
 			return "{0}{1}".format(serverUrl, homePage);
 		}
 	}
 	
 	public static class API_WebGoat_ExtensionMethods_installation
 	{
 		public static API_WebGoat download(this API_WebGoat webGoat)
 		{ 			
 			webGoat.ie.open("http://www.owasp.org/index.php/Category:OWASP_WebGoat_Project");
			webGoat.ie.link("WebGoat Google code downloads")
			  .flash()
			  .click();
			webGoat.ie.link("WebGoat-OWASP_Standard-5.3_RC1.7z ")
			  .flash()
			  .click(); 
			//webGoat.ie.silent(false);				// so that we get the 'save as' pop-up
			webGoat.ie.link("WebGoat-OWASP_Standard-5.3_RC1.7z")
			  .flash()
			  .click(); 
			  
			return webGoat;
 		}
 		
 		public static bool installOk(this API_WebGoat webGoat)
 		{
 			if (webGoat.Dir_LocalInstallation.dirExists().isFalse())
 				return false;
 			if (webGoat.startWebGoatCommand().fileExists().isFalse())
 				return false;
 			return true;
 		}
 		
 		public static API_WebGoat start(this API_WebGoat webGoat)
 		{
 			if (webGoat.installOk()) 
 			{	
 				if(webGoat.serverOnWebGoatPort())
 					"[API_WebGoat] Aborting start since there is already a web server on the expected location".error();
 				else
 				{
 					var startCommand = webGoat.startWebGoatCommand();
 					"Starting webgoat with the command: {0}".info(startCommand);
 					webGoat.WebGoatProcess = Processes.startProcess(startCommand);
 					webGoat.sleep(4000);
 					if(webGoat.serverOnWebGoatPort())
 						"[API_WebGoat] Web server started ok".info();
 					else
 						"[API_WebGoat After 5 secs the web server was still not available".error();
 				}
 			}
 			return webGoat;
 		}
 		
		public static API_WebGoat openMainPage(this API_WebGoat webGoat)
		{
			
			webGoat.ie.openWithBasicAuthentication(webGoat.url_HomePage(),"guest","guest");
			
			/*var pageTitle = webGoat.ie.title();
			if (pageTitle == "How to work with WebGoat")
				"[API_WebGoat] Found expected title, all seems to be ok".info();
			else
				"API_WebGoat: Could not find expected title, current title is: '{0}'".error(pageTitle);
			*/	
			return webGoat;
		}
		
		
 	}
 
 }