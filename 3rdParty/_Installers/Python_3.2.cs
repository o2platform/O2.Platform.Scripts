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
			new Python_Install().start(); 
		}
	}
	 
	public class Python_Install : Tool_API    
	{				
		public Python_Install()
		{						
			Install_Uri = "http://www.python.org/ftp/python/3.2.3/python-3.2.3.msi".uri();
			Install_File = "python-3.2.3.msi";
			Install_Dir = @"C:\Python32\";
			Executable = Install_Dir.pathCombine("python.exe");	   
			startInstaller_FromMsi_Web();	       		
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