using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs
  
namespace O2.XRules.Database.APIs 
{ 
	public class NodeJS_MSI_Installer_Test
	{
		public void test()
		{
			new NodeJS_MSI_Installer().start();
		}
	}
	public class NodeJS_MSI_Installer : Tool_API 
	{			
		public NodeJS_MSI_Installer()
		{
			config("NodeJS_MSI", 				   
				   "http://nodejs.org/dist/v0.8.16/node-v0.8.16-x86.msi".uri(),
				   @"SourceDir\nodejs\node.exe");
			install_JustMsiExtract_into_TargetDir();
		}	
		public Process start()
		{
			if (isInstalled())
				return Executable.startProcess();
			return null;
		}				
	}	
}