using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class GrailsToolSuite_Installer_Test
	{
		public void test()
		{
			new GrailsToolSuite_Installer().start();
		}
	}

	public class GrailsToolSuite_Installer : Tool_API 
	{				
		public GrailsToolSuite_Installer()
		{
			config("Grails Tool Suite", 
				   "http://download.springsource.com/release/STS/3.3.0/dist/e4.3/groovy-grails-tool-suite-3.3.0.RELEASE-e4.3-win32.zip".uri(),
				   "ReactOS-0.3.15-QEMU\\boot.bat");			
    		installFromZip_Web();    		    		
		}
				
		public Process start()
		{
			if (isInstalled())
				this.Executable.startProcess();				
			return null;
		}		
	}
}
