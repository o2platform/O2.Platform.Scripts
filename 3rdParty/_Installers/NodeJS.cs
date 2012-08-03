using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;

//O2File:Tool_API.cs
  
namespace O2.XRules.Database.APIs 
{ 
	public class Install_NodeJS_Test
	{
		public void test()
		{
			new NodeJS().start();
		}
	}
	public class NodeJS : Tool_API 
	{	
		public NodeJS() : this(true)
		{
		}
		
		public NodeJS(bool installNow)
		{
			ToolName = "NodeJS";
			Install_Uri = "http://nodejs.org/dist/v0.6.18/node.exe".uri();
			Install_Dir = ToolsDir.pathCombine(ToolName);
			Executable = Install_Dir.pathCombine("node.exe");    		
    		
    		if (installNow)
    			install();		
		}
		
		
		public bool install()
		{
			"Installing {0}".info(ToolName);
			return install_JustDownloadFile_into_TargetDir(); 						
		}
		
		public Process start()
		{
			if (install())
				return Install_Dir.pathCombine("node.exe").startProcess();
			return null;
		}		
	}
	
	public static class NodeJS_ExtensionMethods
	{
		public static string execute(this NodeJS nodeJS, string arguments)
		{			
			return nodeJS.Executable.startProcess_getConsoleOut(arguments);
		}
		
		public static Process execute(this NodeJS nodeJS, string arguments, Action<string> onConsoleOut)
		{			
			return nodeJS.Executable.startProcess(arguments, onConsoleOut);
		}
		
		public static string help(this NodeJS nodeJS)
		{
			return nodeJS.execute("-h");
		}
		
		public static Process eval(this NodeJS nodeJS, string evalScript, Action<string> onConsoleOut)
		{
			return nodeJS.execute("-p -e {0}".format(evalScript), onConsoleOut);
		}
		
		public static string eval(this NodeJS nodeJS, string evalScript)
		{
			return nodeJS.execute("-p -e {0}".format(evalScript));
		}
	}
}