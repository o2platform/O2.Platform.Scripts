using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class Sqlite_Install_Test
	{
		public static void test()  
		{
			new Sqlite_Install().start(); 
		}
	}
	 
	public class Sqlite_Install : Tool_API    
	{				
		public Sqlite_Install() 
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