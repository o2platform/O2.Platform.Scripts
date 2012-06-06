using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;

//O2File:cURL_Installer.cs

namespace O2.XRules.Database.APIs
{			
	public class API_cURL : Tool_API
	{				
		public string 			CURL_Exe 		{ get; set; }
		public Process 			CURL_Process 	{ get; set;}
		public Action<string> 	OnConsoleData 	{ get; set; }
		public string 			ExtraCommands 	{get; set;}
		
		public API_cURL()
		{
			CURL_Exe = 	  new cURL_Install().Executable;			
			OnConsoleData = default_OnConsoleData;
		}
						
		
		public override bool isInstalled()
		{
			if (CURL_Exe.fileExists())
			{
				"cURL is installed because found file: {0}".debug(CURL_Exe);
				return true;
			}
			"cURL is currently not installed".info();
			return false;
		}
		
		//by default sent it to the LogViewer
		public void default_OnConsoleData(string data)
		{	
			"[cURL]: {0}".info(data);
		}
		
		
		public API_cURL execute(string arguments)
		{									
			" *** Executing cURL with commands: {0} **** ".debug(arguments);
			CURL_Process = Processes.startProcessAndRedirectIO(CURL_Exe, arguments, OnConsoleData);			
			return this;
		}
		
		public API_cURL waitForExecutionCompletion()
		{
			if (CURL_Process.notNull())
				CURL_Process.WaitForExit();
			return this;
		}
		
		public API_cURL stop()
		{
			if (CURL_Process.notNull())
			{
				OnConsoleData("[O2 Message]: User Request -> Stopping cURL Process");
				CURL_Process.stop();
			}
			return this;
		}
	}
	
	public static class API_cURL_ExtensionMethods
	{
		public static API_cURL help(this API_cURL cURL)
		{
			return cURL.execute("");
		}
		
	}
}