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
			new IronPython_Install().start(); 
		}
	}
	 
	public class IronPython_Install : Tool_API    
	{				
		public IronPython_Install()
		{			
			config("IronPython", 
				   "IronPython.msi", 
				   "http://download.codeplex.com/Download/Release?ProjectName=ironpython&DownloadId=352994&FileTime=129760937686670000&Build=18978".uri(),
				   "SourceDir\\IronPython 2.7\\ipy.exe");
			install_JustMsiExtract_into_TargetDir();	       		
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