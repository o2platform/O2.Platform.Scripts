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
			config("Python", 
				   "http://www.python.org/ftp/python/3.3.2/python-3.3.2.msi".uri(),
				   @"C:\Python33\python.exe");
			
    		startInstaller_FromMsi_Web();   
		}			
		
		public Process start()
		{
			if (this.isInstalled())
				return this.Executable.startProcess(); 
			return null;
		}		
	}
}