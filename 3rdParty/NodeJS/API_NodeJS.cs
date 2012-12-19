using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils; 

//Installer:NodeJS_Exe_Installer.cs!NodeJS\node.exe
  
namespace O2.XRules.Database.APIs 
{ 
	public class API_NodeJS
	{
		public string Executable { get; set;}
		
		public API_NodeJS()
		{
			Executable = PublicDI.config.ToolsOrApis.pathCombine(@"NodeJS_Exe\node.exe");
		}
	}
	
	public static class NodeJS_ExtensionMethods
	{
		public static bool isInstalled(this API_NodeJS nodeJS)
		{
			return nodeJS.Executable.fileExists();
		}
		
		public static string execute(this API_NodeJS nodeJS, string arguments)
		{			
			return nodeJS.Executable.startProcess_getConsoleOut(arguments);
		}
		
		public static Process execute(this API_NodeJS nodeJS, string arguments, Action<string> onConsoleOut)
		{			
			return nodeJS.Executable.startProcess(arguments, onConsoleOut);
		}
		
		public static string help(this API_NodeJS nodeJS)
		{
			return nodeJS.execute("-h");
		}
		
		public static Process eval(this API_NodeJS nodeJS, string evalScript, Action<string> onConsoleOut)
		{
			return nodeJS.execute("-p -e {0}".format(evalScript), onConsoleOut);
		}
		
		public static string eval(this API_NodeJS nodeJS, string evalScript)
		{
			return nodeJS.execute("-p -e {0}".format(evalScript));
		}
	}
}