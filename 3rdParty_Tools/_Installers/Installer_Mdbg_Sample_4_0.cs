using System;
using System.Diagnostics; 
using O2.Kernel; 
using O2.Kernel.ExtensionMethods; 
using O2.DotNetWrappers.ExtensionMethods;  
using O2.DotNetWrappers.Windows;  
using O2.XRules.Database.Utils;
//O2File:API_MsBuild.cs 
//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class testInstall
	{
		public static void test()  
		{
			new Installer_MDbg_Sample().start(); 
		}
	}
	 
	public class Installer_MDbg_Sample : Tool_API    
	{				
		public Installer_MDbg_Sample()
		{			
			config("MDbg_Sample", 				    
				   "http://download.microsoft.com/download/E/5/B/E5BF5F79-45FB-4ACA-AA6F-4F2C0DFE29C8/MDbgSample4.EXE".uri(),
				   "MDbg//mdbg.exe");
			install_MDbg();
		}		
		
		public Installer_MDbg_Sample install_MDbg()
		{
			if (this.isInstalled().isFalse())
			{
				"Installing MDbg".debug();
	    		var zipFile = this.Install_Dir.pathCombine("MDbg Sample.zip");
	    		var srcDir = this.Install_Dir.pathCombine(@"MDbg Sample\src");
	    		
	    		if (srcDir.dirExists().isFalse())
	    		{
		    		if (zipFile.fileExists().isFalse())
		    		{
		    			var installFile = installerFile();    		
		    			var args = "/Q /C /T:\"{0}\"".format(this.Install_Dir);    		
		    			var process = installFile.startProcess(args);
		    			process.WaitForExit();
		    		}
		    		zipFile.unzip(this.Install_Dir);
		    	}		    	
		    	
		    	
				/*Action<string> buildForPlatform = 
	    		(platform)=>{  
	    						var targetDir = this.Install_Dir.pathCombine("MDbg_{0}".format(platform)); 
	    						var compiledFiles = this.Install_Dir.pathCombine(@"MDbg Sample\bin\Debug");
	    						Files.deleteAllFilesFromDir(compiledFiles);
								var slnFile = srcDir.pathCombine("mdbg.sln");
								var msBuild = new API_MSBuild() {  
																	LogConsoleOut = true,
																	ExtraBuildArguments = "/p:RunCodeAnalysis=False", // /p:PlatformTarget=\"x86\""																	
																	PlatformTarget = platform
																};								
								if (msBuild.build_Ok(slnFile))
								{																				
									Files.copyFilesFromDirectoryToDirectory(compiledFiles, targetDir);
								}
							};
				buildForPlatform("x86");
				buildForPlatform("AnyCpu");
				*/
				
				var slnFile = srcDir.pathCombine("mdbg.sln");
				var msBuild = new API_MSBuild() {  
													LogConsoleOut = true,
													ExtraBuildArguments = "/p:RunCodeAnalysis=False"
												};
				if (msBuild.build_Ok(slnFile))
				{
					var compiledFiles = this.Install_Dir.pathCombine(@"MDbg Sample\bin\Debug");
					var targetDir = this.Install_Dir.pathCombine("MDbg"); 
					Files.copyFilesFromDirectoryToDirectory(compiledFiles, targetDir);
				}
			}    
			return this;
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