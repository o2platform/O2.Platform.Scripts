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
			config("Python-3.2.3", 				   
				   "http://www.python.org/ftp/python/3.2.3/python-3.2.3.msi".uri(),
				   "SourceDir\\IronPython 2.7\\ipy.exe");
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