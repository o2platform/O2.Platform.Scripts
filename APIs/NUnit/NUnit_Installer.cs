using System;
using System.Linq;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.DotNetWrappers.Windows; 
//O2File:Tool_API.cs
using O2.XRules.Database.Utils;

namespace O2.XRules.Database.APIs
{
	public class NUnit_Installer_Test
	{ 
		public void test()
		{
			new NUnit_Installer().start(); 
		}
	}
	public class NUnit_Installer : Tool_API  
	{			
		
		public NUnit_Installer() : this(true)
		{
		}
		
		public NUnit_Installer(bool installNow)
		{
			//Install_Uri = "http://launchpad.net/nunitv2/2.5/2.5.10/+download/NUnit-2.5.10.11092.zip".uri();
			this.ToolName = "NUnit";
			this.Version = "2.5.10";
			this.Install_Uri = "http://launchpad.net/nunitv2/2.5/2.5.10/+download/NUnit-2.5.10.11092.zip".uri();			
			this.Executable_Name = @"NUnit-2.5.10.11092\bin\net-2.0\nunit.exe";			
			config();						
    		if (installNow)    		    			
    			install();    		    		
		}
		
		
		public bool install()
		{
			"Installing {0} {1}".info(this.ToolName, this.Version);
			return installFromZip_Web(); 						
		}
		
		public Process start()
		{
			if (install())
				return this.Executable.startProcess();
			return null;
		}		
	}
}