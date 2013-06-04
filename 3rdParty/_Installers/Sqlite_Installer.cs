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
			new MySql_Install().start(); 
		}
	}
	 
	public class MySql_Install : Tool_API    
	{				
		public MySql_Install() 
		{			
			config("sqlite",
				   "http://www.sqlite.org/2013/sqlite-shell-win32-x86-3071700.zip".uri(),
				   @"sqlite3.exe");
			installFromZip_Web();	       		
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