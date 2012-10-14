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
			new NodeJS_Installer().start();
		}
	}
	public class NodeJS_Installer : Tool_API 
	{	
		public NodeJS_Installer() : this(true)
		{
		}
		
		public NodeJS_Installer(bool installNow)
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
}