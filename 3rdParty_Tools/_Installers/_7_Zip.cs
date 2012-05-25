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
	public class SZip_Test
	{
		public void test()
		{
			new _7_Zip().start(); 
		}
	}
	public class _7_Zip : Tool_API 
	{	
		public _7_Zip() : this(true)
		{
		}
		
		public _7_Zip(bool installNow)
		{
			config("7-Zip 9.2.0", "7-Zip", "7z920.exe");			
    		Install_Uri = "http://downloads.sourceforge.net/project/sevenzip/7-Zip/9.20/7z920.exe?r=&ts=1337856882&use_mirror=heanet".uri();    		
    		Install_Dir = getProgramFiles().pathCombine("7-Zip");    		
    		Executable = Install_Dir.pathCombine("7z.exe");
    		if (installNow)
    			install();    		
		}
		
		
		public bool install()
		{
			"Installing {0}".info(ToolName);
			return installFromMsi_Web(); 						
		}
		
		public string start()
		{
			if (install())
				return Executable.startProcess_getConsoleOut().info();
			return null;
		}		
		
		public string getProgramFiles()
		{
			return Environment.GetEnvironmentVariable("PROGRAMFILES(X86)") 
					?? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
		}
	}
	
	public static class _7_Zip_ExtensionMethods
	{
		public static string execute_withParams(this _7_Zip _7Zip, string _params)
		{
			return _7Zip.Executable.startProcess_getConsoleOut(_params); 
		}
	
		public static string _7zip_Unzip(string file)
		{
			return new _7_Zip().unzip(file);
		}
		public static string unzip(this _7_Zip _7Zip, string file)
		{
			var tempDir = file.fileName_WithoutExtension().tempDir();
			return _7Zip.unzip(file, tempDir);
		}
		public static string unzip(this _7_Zip _7Zip, string file, string targetDir)
		{
			"[7_Zip] Unzipping file '{0}' into folder '{1}'".info(file,targetDir);
			var unzipArguments = "x -o\"{0}\" \"{1}\" ".format(targetDir,file);
			_7Zip.execute_withParams(unzipArguments);
			return targetDir;
		}
		
		public static string get_FilesList(this _7_Zip _7Zip, string file)
		{
			"[7_Zip] Getting file list for: {0}".info(file);
			var unzipArguments = "l \"{0}\" ".format(file);
			return _7Zip.execute_withParams(unzipArguments);
		}
	}
}