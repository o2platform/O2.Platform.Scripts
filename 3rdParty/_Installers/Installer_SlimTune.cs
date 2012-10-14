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
	public class Installer_SlimTune_Test
	{ 
		public void test()
		{
			new Installer_SlimTune().start(); 
		}
	}
	public class Installer_SlimTune : Tool_API  
	{			
		
		public Installer_SlimTune() : this(true)
		{
		}
		 
		public Installer_SlimTune(bool installNow)
		{
			//Install_Uri = "http://launchpad.net/nunitv2/2.5/2.5.10/+download/NUnit-2.5.10.11092.zip".uri();
			config("SlimTune", "http://slimtune.googlecode.com/files/SlimTune-0.3.0.exe".uri(), "SlimTune-.exe");										
			Install_Dir = ProgramFilesFolder;
			this.Executable =  this.ProgramFilesFolder.pathCombine(@"SlimTune Profiler\SlimTuneUI.exe");			    		
			//this.showInfo();
			this.installFromMsi_Web(); 						  		    		
		}				
		
		public Process start()
		{
			if (isInstalled())
				return this.Executable.startProcess();
			return null;
		}		
	}
}