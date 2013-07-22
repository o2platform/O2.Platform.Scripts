using System.Diagnostics;
using FluentSharp.CoreLib;
//O2File:Tool_API.cs

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