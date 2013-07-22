using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class MySql_Install_Test
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
			config("MySql",
				   "http://cdn.mysql.com/Downloads/MySQL-5.6/mysql-5.6.11-win32.zip".uri(),
				   @"mysql-5.6.11-win32\bin\mysql.exe");
			installFromZip_Web();	       		
		}	
		//
		
		public Process start()
		{			
			if (this.isInstalled())
				return this.Executable.startProcess("--help"); 
			return null;
		}		
	}
}