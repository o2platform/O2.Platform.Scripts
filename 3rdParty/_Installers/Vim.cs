using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class Vim_Install_Test
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