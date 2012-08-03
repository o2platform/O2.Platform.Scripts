using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Installer_DebugAnalyzer_Test
	{
		public void test()
		{
			new Installer_DebugAnalyzer().start();
		}
	}
	public class Installer_DebugAnalyzer : Tool_API 
	{	
		public Installer_DebugAnalyzer()
		{
			
			Action<string, string,Uri> install = (installFolder, installFile, installUri)=>
				{					
					config("DebugAnalyzer", "DebugAnalyzer v1.2.5", installFile);			
		    		this.Install_Uri = installUri;
		    		this.Executable = this.Install_Dir.pathCombine(installFolder + @"\DebugAnalyzer.exe");
		    		installFromZip_Web();
		    	};		  
		    			    
		    install	("x86", "DAx86.zip", "http://www.debuganalyzer.net/file.axd?file=DAx86.zip".uri());
		    install	("x64", "DAx64.zip", "http://www.debuganalyzer.net/file.axd?file=DAx64.zip".uri());
    		//install 2.0 version    		
    		//install("Acorns.Hawkeye.125.N2", "Acorns.Hawkeye.125.N2.zip", "http://download.codeplex.com/Download/Release?ProjectName=hawkeye&DownloadId=196206&FileTime=129391670111230000&Build=18924".uri());
    		//install 4.0 version
    		//install("Acorns.Hawkeye.125.N4", "Acorns.Hawkeye.125.N4.zip", "http://download.codeplex.com/Download/Release?ProjectName=hawkeye&DownloadId=196207&FileTime=129391675391630000&Build=18924".uri());    		
		}
		
		public Process start()
		{
			if (isInstalled())
				return this.Executable.startProcess();			
			return null;
		}		
	}
}