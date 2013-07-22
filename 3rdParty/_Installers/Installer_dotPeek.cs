using System.Diagnostics;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class Installer_DotPeek_Test
	{
		public static void test()  
		{
			new Installer_DotPeek().start(); 
		}
	}
	 
	public class Installer_DotPeek : Tool_API    
	{				
		public Installer_DotPeek()
		{						
			config("DotPeek",
				   "http://download.jetbrains.com/dotpeek/dotPeekSetup-1.0.0.8644.msi".uri(),
				   @"dotPeek.exe");
				
			var extractedFiles = this.Install_Dir.pathCombine(@"SourceDir\PFiles\JetBrains\dotPeek\v1.0\Bin");
			if (extractedFiles.dirExists().isFalse())			
				install_JustMsiExtract_into_TargetDir();	       		
			if (this.Executable.fileExists().isFalse())
				Files.copyFilesFromDirectoryToDirectory(extractedFiles, Install_Dir);			
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