using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class ZAProxy_Install_Test
	{
		public static void test()  
		{
			new ZAProxy_Install().start(); 
		}
	}
	 
	public class ZAProxy_Install : Tool_API    
	{				
		public ZAProxy_Install() 
		{			
			config("ZAProxy",
				   "https://zaproxy.googlecode.com/files/ZAP_WEEKLY_D-2013-08-12.zip".url(),
				  // "https://zaproxy.googlecode.com/files/ZAP_WEEKLY_D-2013-05-20.zip".uri(),
				   @"ZAP_D-2013-08-12\zap.bat");
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