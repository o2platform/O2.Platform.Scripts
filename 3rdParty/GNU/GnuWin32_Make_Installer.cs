using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
//O2File:Tool_API.cs
using O2.XRules.Database.Utils;


namespace O2.XRules.Database.APIs
{
	public class Installer_Test
	{
		public void test()
		{
			new GnuWin32_Make_Installer().start(); 
		}
	}
	public class GnuWin32_Make_Installer : Tool_API 
	{	 
		public GnuWin32_Make_Installer()
		{
			//http://heanet.dl.sourceforge.net/project/gnuwin32/sed/4.2.1/sed-4.2.1-setup.exe
			config("GNU", 
				   "http://garr.dl.sourceforge.net/project/gnuwin32/make/3.81/make-3.81.exe".uri(),				   
				   ProgramFilesFolder.pathCombine(@"GnuWin32\bin\make.exe"));
    		installFromMsi_Web();
		}
		
		public Process start()
		{
			if (isInstalled())
				return this.Executable.startProcess();
			return null;
		}		
	}
}