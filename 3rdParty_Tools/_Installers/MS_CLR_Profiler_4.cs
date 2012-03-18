using System;
using System.Diagnostics;
using O2.Kernel;
using O2.XRules.Database.Utils;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 

//O2File:Tool_API.cs
//O2File:_Extra_methods_Windows.cs
//O2File:_Extra_methods_Zip.cs

namespace O2.XRules.Database.APIs
{
	public class MS_CLR_Profiler_4_Test
	{
		public void test()
		{
			new MS_CLR_Profiler_4().start(); 
		}
	}
	public class MS_CLR_Profiler_4 : Tool_API 
	{			
		public MS_CLR_Profiler_4() : this(true)
		{
		}
		
		public MS_CLR_Profiler_4(bool installNow)
		{
			config("CLR Profiler 4.0", "CLR Profiler", "CLRProfiler4.EXE");			
    		Install_Uri = "http://download.microsoft.com/download/A/4/2/A42841BC-340B-4FDA-8D6A-B06A4FDD79AA/CLRProfiler4.EXE".uri();
    		if (installNow)
    			install();    		
		}
		public string ExeFile 	
		{
			get
			{
				var currentProcessor = "32"; // 64 //Need to add way to detect this
				return Install_Dir.pathCombine(@"CLRProfiler\Binaries\{0}\CLRProfiler.exe".format(currentProcessor));
			}
		}
		
		public bool install()
		{					
			if (ExeFile.fileExists().isFalse())
			{
				"Installing {0}".info(ToolName);
				var downloadedFile = localDownloadsDir.pathCombine(Install_File);
				"Downloaded file location: {0}".info(downloadedFile);
				"Install_Dir: {0}".info(Install_Dir);
	
				var zipFile = Install_Dir.pathCombine("CLRProfiler.zip");
				if (zipFile.fileExists().isFalse())
				{
					//extract from CLRProfiler4.EXE 
					"Extracting CLRProfiler4.EXE".info();
					var arguments = "/t:\"{0}\"".format(Install_Dir);			
					var process = downloadedFile.startProcess(arguments);						
					process.WaitForExit();
				}
				"Unziping CLRProfiler.zip".info();
				
				zipFile.unzip(Install_Dir);
				"Unzipping complete".info();
			}			
			return ExeFile.fileExists();			
		}
		
		public Process start()
		{
		
			if (install())
				return ExeFile.startProcess();
			return null;
		}		
	}
}