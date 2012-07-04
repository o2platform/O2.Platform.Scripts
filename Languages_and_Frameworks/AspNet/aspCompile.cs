// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.Hosting;
using System.Web.Compilation;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;

//O2Ref:System.Web.dll
//O2File:Scripts_ExtensionMethods.cs

using O2.XRules.Database.Utils;

namespace O2.Script
{
	
	public class WebServerHost_Setup 
	{
		public void setup()
		{
			var targetDir = @"C:\O2\_XRules_Local\_localAspPoCs_2\"; 	
			new WebServer(targetDir);
		}
	}
	
	public class WebServerHost : MarshalByRefObject
	{
	   public string Html {get;set;}
	   public string Page { get; set;}
	   public SimpleWorkerRequest simpleWorkerRequest;
	   
	   public WebServerHost()
	   {
	   		Page = "Hello_World.aspx";
	   }
	   
	   public string ping()
	   {
	   		return "pong";
	   }
	   
	   public void openScript()
		{
			var script = O2Gui.open<System.Windows.Forms.Panel>("Script editor", 700, 300)
				 .add_Script(false) 
			 	 .onCompileExecuteOnce();
			script.InvocationParameters.add("simpleWorkerRequest",simpleWorkerRequest);
			script.Code = "hello"; 

		}
		
				
		
	   public string ProcessRequest()	
	   {
	   		return ProcessRequest(Page);
	   }
	   public string ProcessRequest(string page)	
	   {
	   		O2Gui.open<System.Windows.Forms.Panel>("Util - LogViewer", 400,140).add_LogViewer();
	   		"Current Directory: {0}".info(Environment.CurrentDirectory);
	   		
	   		var o2Timer = new O2Timer("ProcessRequest for file: {0}".info(page)).start();			
	   		Page = page ?? Page;
			var stringWriter = new StringWriter();
		   	simpleWorkerRequest = new SimpleWorkerRequest(Page, string.Empty, stringWriter);
		   	
		   	//openScript();
		   	
		   	//"Good Morning".execute_InScriptEditor_InSeparateAppDomain();
		   	"processing request for: {0}".info(Page);
		   	HttpRuntime.ProcessRequest(simpleWorkerRequest);
		   	var Html = stringWriter.str();
		   	o2Timer.stop();
		   	return Html;	
	   }	   	   	   	
	}
	
	public class WebServer
	{
		public WebServerHost WebServerHost {get; set;}
		//public object WebServerHost_ {get; set;}
				
		public string WebRootDir {get;set; }
		public string BinFolder {get;set; }				
				
		public WebServer() : this(null)
		{			
		}
				
		public WebServer(string rootDir)
		{
			//O2Gui.open<System.Windows.Forms.Panel>("Util - LogViewer", 400,140).add_LogViewer();			
			//WebServerDll = "O2_WebServer.dll";
			WebRootDir = rootDir ?? "_WebServer".tempDir(false);
			setup();
		}
		
		public void setup()
		{										
			BinFolder = WebRootDir.pathCombine("bin");
			//endure current dll is on the BinFolder						
			copyToBinFolder(System.Reflection.Assembly.GetExecutingAssembly().Location);
			"Setup up complete for folder: {0}".info(WebRootDir);
		}
		
		public WebServer copyToBinFolder(string file)
		{
			return copyToBinFolder(file, file.fileName());
		}
		
		public WebServer copyToBinFolder(string file, string fileName)
		{			
			var targetFile = BinFolder.pathCombine(fileName);
			if (targetFile.fileExists().isFalse())
			{
				"copying *.dll and *.pdb to bin folder file: {0} - > {1} ".info(file, targetFile); 
	        	Files.Copy(file, targetFile);
	        	Files.Copy(file.replace(".dll",".pdb"), targetFile); 
	        }
	        return this;
		}
		
		public void createHost()
		{	
			//if (WebServerHost.isNull())
			{
				var o2Timer = new O2Timer("Creating WebServer host").start();			
				WebServerHost = (WebServerHost)ApplicationHost.CreateApplicationHost(typeof(WebServerHost), "/", WebRootDir);			
				//WebServerHost = ApplicationHost.CreateApplicationHost(typeof(WebServerHost), "/", WebRootDir);							
				o2Timer.stop();
			}
		}
		
/*		public object createHost<T>()
			where T : MarshalByRefObject
		{	
			//if (WebServerHost.isNull())
			{
				var o2Timer = new O2Timer("Creating dynamically WebServer host of type: {0}".format(typeof(T).typeName())).start();			
				var host = ApplicationHost.CreateApplicationHost(typeof(T), "/", WebRootDir);							
				o2Timer.stop();
				return host;
			}
		}*/
		
		public string executeRequest()
		{
			return executeRequest(null); 
		}
		
		public string executeRequest(string page)
		{				
			"executing request".info();
			createHost();
			return WebServerHost.ProcessRequest(page);
            //return WebServerHost_.invoke("ProcessRequest", page).str();                         
		}				
				
	}
	
	public static class WebServer_ExtensionMethods
	{
		public static WebServer copyToBinFolder(this WebServer webServer, string file)
		{			
			var targetFile = webServer.BinFolder.pathCombine(file.fileName());
			if (targetFile.fileExists().isFalse())
				Files.Copy(file, targetFile);
			return webServer;

		}
	}
	
}