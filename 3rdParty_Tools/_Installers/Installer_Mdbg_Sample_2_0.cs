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
			config("MDbg_Sample_2_0", 				    
				   "http://download.microsoft.com/download/1/2/f/12f5dde6-695d-4003-a451-739ab3be6098/mdbgSample21.EXE".uri(),
				   "MDbg//mdbg.exe");
			install_MDbg();
		}		
		
		public Installer_MDbg_Sample install_MDbg()
		{
			if (this.isInstalled().isFalse())
			{
				"Installing MDbg".debug();
	    		var zipFile = this.Install_Dir.pathCombine("mdbg.zip");
	    		var unzipFolder = this.Install_Dir.pathCombine("unzipped files");	    		
	    		var srcDir = unzipFolder.pathCombine(@"mdbg\src");
	    		
	    		if (srcDir.dirExists().isFalse())
	    		{
		    		if (zipFile.fileExists().isFalse())
		    		{
		    			var installFile = installerFile();    		
		    			var args = "/Q /C /T:\"{0}\"".format(this.Install_Dir);    		
		    			var process = installFile.startProcess(args);
		    			process.WaitForExit();
		    		}
		    		zipFile.unzip(unzipFolder);
		    	}		    	
				var slnFile = srcDir.pathCombine("mdbg.sln");
				var msBuild = new API_MSBuild() {  
													LogConsoleOut = true,
													ExtraBuildArguments = "/p:RunCodeAnalysis=False"
												};
				msBuild.MSBuild_Exe = msBuild.MSBuild_Exe_2_0;
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