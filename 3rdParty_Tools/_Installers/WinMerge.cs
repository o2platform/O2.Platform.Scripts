using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
//O2File:Tool_API.cs

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs
using O2.XRules.Database.Utils;

namespace O2.XRules.Database.APIs
{
	public class Install_WinMerge_Test
	{
		public void test()
		{
			new WinMerge().start();
		}
	}
	
	public class WinMerge : Tool_API 
	{	
		public WinMerge() : this(true) 
		{
		}
		
		public WinMerge(bool installNow) 
		{
			config("WinMerge", "WinMerge v2.12.4", "WinMerge-2.12.4-exe.zip");			
    		this.Install_Uri = "http://downloads.sourceforge.net/winmerge/WinMerge-2.12.4-exe.zip".uri();
    		this.Executable = Install_Dir.pathCombine(@"WinMerge-2.12.4-exe\WinMergeU.exe");
    		if (installNow)
    			install();		
		}
		
		
		public bool install()
		{ 
			"Installing {0}".info(ToolName);
			return installFromZip_Web(); 						
		}
		
		public Process start()
		{
			return start("");
		}
		public Process start(string parameters)
		{
			if (install())
				return this.Executable.startProcess(parameters);
			return null;
		}		
	}
	
	public class API_WinMerge
	{
		public WinMerge winMerge;
		public Process WinMergeProcess;
		
		public API_WinMerge()
		{
			winMerge = new WinMerge();
		}				
	}
	
	public static class API_WinMerge_ExtensionMethods
	{
		public static API_WinMerge openGui(this API_WinMerge apiWinMerge)
		{
			apiWinMerge.WinMergeProcess = apiWinMerge.winMerge.start();
			return apiWinMerge;
		}
		
		public static API_WinMerge compareTwoFiles(this API_WinMerge apiWinMerge, string file1, string file2)
		{
			var parameters = "\"{0}\" \"{1}".format(file1,file2);
			apiWinMerge.WinMergeProcess = apiWinMerge.winMerge.start(parameters);			
			return apiWinMerge;
		}
				
		public static API_WinMerge file_Compare(this string file1, string file2)
		{
			var winMerge = new API_WinMerge();
			return winMerge.compareTwoFiles(file1,file2);
		}
		
		public static API_WinMerge file_Merge(this string file1, string file2)
		{
			return file1.file_Compare(file2);
		}
	}
}