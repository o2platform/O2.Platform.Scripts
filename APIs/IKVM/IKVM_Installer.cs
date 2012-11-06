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
	public class IKVM_Installer_Test
	{ 
		public void test()
		{
			new IKVM_Installer().start(); 
		}
	}
	public class IKVM_Installer : Tool_API  
	{				
		public IKVM_Installer()
		{
			config("IKVM", 				   
				   "http://switch.dl.sourceforge.net/project/ikvm/ikvm/7.1.4532.2/ikvmbin-7.1.4532.2.zip".uri(),
				   @"ikvm-7.1.4532.2\bin\ikvm.exe");
			installFromZip_Web();
		}
				
		
		public Process start()
		{
			if (isInstalled())
				return Executable.startProcess();
			return null;
		}					
	}
}