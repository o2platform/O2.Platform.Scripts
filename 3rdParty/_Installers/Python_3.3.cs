using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class Python_Install_3_3_Test
	{
		public static void test()  
		{
			new Python_Install_3_3().start(); 
		}
	}
	 
	public class Python_Install_3_3 : Tool_API    
	{				
		public Python_Install_3_3()
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