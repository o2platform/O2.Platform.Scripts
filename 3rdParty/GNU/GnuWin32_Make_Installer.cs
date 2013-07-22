using System.Diagnostics;
using FluentSharp.CoreLib;
//O2File:Tool_API.cs


namespace O2.XRules.Database.APIs
{
	public class GnuWin32_Make_Installer_Test
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