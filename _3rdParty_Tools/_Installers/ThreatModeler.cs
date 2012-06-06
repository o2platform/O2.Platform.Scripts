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
			new ThreatModeler_Install().start(); 
		}
	}
	 
	public class ThreatModeler_Install : Tool_API    
	{				
		public ThreatModeler_Install()
		{			
			config("ThreatModeler", 				   
				   "http://www.myappsecurity.com/ThreatModelerSetup.msi".uri(),
				   "SourceDir\\ThreatModeler.exe");
			install_JustMsiExtract_into_TargetDir();	       		
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