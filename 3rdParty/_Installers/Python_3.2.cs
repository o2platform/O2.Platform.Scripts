using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class Python_Install_3_2_Test
	{
		public static void test()  
		{
			new Python_Install_3_2().start(); 
		}
	}
	 
	public class Python_Install_3_2 : Tool_API    
	{				
		public Python_Install_3_2()
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