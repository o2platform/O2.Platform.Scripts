using System;
using System.Diagnostics;
using O2.Kernel; 
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;  
using O2.XRules.Database.Utils;
//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class testInstall
	{
		public static void test()  
		{
			new Vim_Install().start(); 
		}
	}
	 
	public class Vim_Install : Tool_API    
	{				
		public Vim_Install()
		{			
			config("Vim", 				   
				   "http://ftp.vim.org/pub/vim/pc/gvim73_46.exe".uri(),
				   "vim.exe");
			DownloadedInstallerFile = download(Install_Uri);
			DownloadedInstallerFile.startProcess();			
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