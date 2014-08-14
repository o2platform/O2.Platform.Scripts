using System;
using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class ESAPI_Installer_Test
	{
		public void test()
		{
			new ESAPI_Installer().start();
		}
	}
	public class ESAPI_Installer : Tool_API 
	{			
		
		public ESAPI_Installer()
		{
			config("OWASP_ESAPI", 
				   "https://owasp-esapi-java.googlecode.com/files/esapi-2.1.0-dist.zip".uri(),
				   "esapi-2.1.0.jar");
			installFromZip_Web(); 							
		}
						
		
		public Process start()
		{			
			if (this.isInstalled())
				return this.Install_Dir.startProcess();
			return null;
		}		
	}
}