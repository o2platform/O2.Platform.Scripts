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
				   "http://garr.dl.sourceforge.net/project/ikvm/ikvm/7.2.4630.5/ikvmbin-7.2.4630.5.zip".uri(),
				   @"ikvm-7.2.4630.5\bin\ikvm.exe");
				   //"http://switch.dl.sourceforge.net/project/ikvm/ikvm/7.1.4532.2/ikvmbin-7.1.4532.2.zip".uri(),
				   //@"ikvm-7.1.4532.2\bin\ikvm.exe");
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