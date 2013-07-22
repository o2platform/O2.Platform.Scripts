using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class Python_Install_2_7_Test
	{
		public static void test()  
		{
			new Python_Install_2_7().start(); 
		}
	}
	 
	public class Python_Install_2_7 : Tool_API    
	{				
		public Python_Install_2_7()
		{						
			config("Python-2.7", 
				   "http://www.python.org/ftp/python/2.7.5/python-2.7.5.msi".uri(),
				   @"C:\Python27\python.exe");
			
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