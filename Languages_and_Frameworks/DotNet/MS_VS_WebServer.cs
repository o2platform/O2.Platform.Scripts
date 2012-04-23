// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;

namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
	public class MS_VS_WebServer
	{
		public static Dictionary<string, MS_VS_WebServer> serverCache = new Dictionary<string, MS_VS_WebServer>();
		
		public string DefaultUrl {get;set;}
		public string LocalPath {get;set;}
		public string Port {get;set;}
		public string VirtualPath {get;set;}				
		public Process WebServerProcess {get; set;}		
		public string WebServerExe { get;set;}
		
		public MS_VS_WebServer()
		{
			
			WebServerExe = PublicDI.config.ReferencesDownloadLocation.pathCombine("MS_VS_WebDev.WebServer.exe"); 		
		}
		
		public MS_VS_WebServer(string localPath, int port, string virtualPath) : this()
		{					
			start(localPath, port, virtualPath);
		}
		
		public bool start(string localPath, int port, string virtualPath)
		{	
			LocalPath = localPath;
			Port = port.str();
			VirtualPath = virtualPath;
			DefaultUrl = "http://127.0.0.1.:{0}{1}".format(Port, VirtualPath);
			return start();
			
		}
		public bool start()
		{							
			if (DefaultUrl.uri().getHtml().valid().isFalse())
			{				
				//var webServerExe = @"C:\Program Files\Common Files\Microsoft Shared\DevServer\10.0\WebDev.WebServer20.EXE";			
				if (WebServerExe.fileExists().isFalse())
				{
					"error could not find WebServer at: {0}".error(WebServerExe);
					return false; 
				} 
				
				var webServerStartArguments = "/port:\"{0}\" /path:\"{1}\" /vpath:\"{2}\"".format(Port, LocalPath,VirtualPath);
				WebServerProcess = Processes.startProcess(WebServerExe, webServerStartArguments);			
				WebServerProcess.sleep(2000);
				"website should be up now".debug();
				return true;
			}
			else
				"webserver is already setup".debug();
			return true;
		}
		
		//public MS_VS_WebServer openPage()
		
		public void stop()
		{
			if (WebServerProcess.notNull())
			{
				if (WebServerProcess.HasExited)
					"WebServer process already has Exited".error();
				else
				{
					WebServerProcess.Kill();
					WebServerProcess.WaitForExit();			
				}
			}
			else
				"WebServerProcess was null".error();
			serverCache.Remove(serverCacheKey()); 
		}
		
		public static List<Process> currentWebServerProcesses()
		{
			return Processes.getProcessesCalled("MS_VS_WebDev.WebServer");
		}
		
		public static void stopCurrentWebServerProcesses()
		{
			currentWebServerProcesses().stop(); 
			serverCache.Clear();		
		}
		
		public string serverCacheKey()
		{
			return serverCacheKey(this.LocalPath, this.Port, this.VirtualPath);
		}
		
		public static string serverCacheKey(string localPath, string port, string virtualPath)
		{
			return "{0} {1} {2}".format(localPath, port, virtualPath);
		}
	}

    public static class MS_VS_WebServer_ExtensionMethods
    {
    	public static Uri uri(this MS_VS_WebServer webServer, string virtualPath)
    	{    		
    		if (virtualPath.valid() && (virtualPath.starts("/") || virtualPath.starts("\\")))
    			virtualPath = virtualPath.removeFirstChar();
    		return (webServer.DefaultUrl + virtualPath).uri();
    	}
    	
    	public static string html(this MS_VS_WebServer webServer, string virtualPath)
    	{
    		return webServer.uri(virtualPath).getHtml();
    	}
    }
    
    public static class MS_VS_WebServer_ExtensionMethods_Start
    {   
    	
    	public static MS_VS_WebServer startWebServer(this int portNumber)
    	{
    		var localPath = "_localWebServer".tempDir();
    		return localPath.startWebServer(portNumber);    			
    	}
    	
    	public static MS_VS_WebServer startWebServer(this string localPath)
    	{    					
			var port = ("112" + 2.randomNumbers()).toInt(); 
			return localPath.startWebServer(port);			
    	} 
    	
    	public static MS_VS_WebServer startWebServer(this string localPath, int port)
    	{
    		var virtualPath = "/";
    		return localPath.startWebServer(port, virtualPath);
    	}
    	
		public static MS_VS_WebServer startWebServer(this string localPath, int port, string virtualPath)
		{						
			var serverCacheKey = MS_VS_WebServer.serverCacheKey(localPath, port.str(), virtualPath);
			if (MS_VS_WebServer.serverCache.hasKey(serverCacheKey))
			{
				"found MS_VS_WebServer object in cache".info();
				var msWebServer = MS_VS_WebServer.serverCache[serverCacheKey];
				if (msWebServer.WebServerProcess.notNull() && msWebServer.WebServerProcess.HasExited.isFalse())
					return MS_VS_WebServer.serverCache[serverCacheKey];
			}
			"creating new instance of MS_VS_WebServer".info();
			var webServer = new MS_VS_WebServer(localPath, port, virtualPath);			
			//if (webServer.WebServerProcess.isNull());	
			//	return null;			
			MS_VS_WebServer.serverCache.add(serverCacheKey, webServer);
			return webServer;
		}			
    }
}
