using System.Diagnostics;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class Installer_Debug_Diagnostic_Tool_Test
	{
		public static void test()  
		{
			new Installer_Debug_Diagnostic_Tool().start(); 
		}
	}
	 
	public class Installer_Debug_Diagnostic_Tool : Tool_API    
	{				
		public Installer_Debug_Diagnostic_Tool()
		{			
			if(this.ProgramFilesFolder.contains("x86"))
			{
				config(@"C:\Program Files\DebugDiag",
					   "http://download.microsoft.com/download/B/8/0/B80535DB-DF4D-4020-B41F-ECE9005D9A65/DebugDiagx64.msi".uri(),
					   @"DebugDiag.exe");
			}
			else
			{				
				config(@"C:\Program Files\DebugDiag", 				   
					   "http://download.microsoft.com/download/B/8/0/B80535DB-DF4D-4020-B41F-ECE9005D9A65/DebugDiagx86.msi".uri(),
					   @"DebugDiag.exe");
			}				
			
			//this MSI needs to be installed to registed the COM components	   
			//install_JustMsiExtract_into_TargetDir();	       		
			startInstaller_FromMsi_Web();
			
			//Create .NET Wrapper for DbgHost.exe
			var debugDiagDir = PublicDI.config.ToolsOrApis.pathCombine("DebugDiag").createDir();
			var dgbHostDll = debugDiagDir.pathCombine("DbgHostLib.dll");
			if (dgbHostDll.fileExists().isFalse())
			{
			
				
				var tlbimp = (this.ProgramFilesFolder.contains("x86"))
								? this.ProgramFilesFolder.pathCombine(@"\Microsoft SDKs\Windows\v7.0A\Bin\x64\tlbimp.exe")
								: this.ProgramFilesFolder.pathCombine(@"\Microsoft SDKs\Windows\v7.0A\Bin\tlbimp.exe");
							
				var fileToConvert = this.Install_Dir.pathCombine(@"DbgHost.exe");
			
				tlbimp.startProcess_getConsoleOut("\"" + fileToConvert + "\"", debugDiagDir).info();
			
				"Created file: {1}".info(dgbHostDll);
			}			
		}	
		//
		
		public Process start()
		{
			if (this.isInstalled())
				return this.Executable.startProcess(); 
			return null;
		}		
	}
}