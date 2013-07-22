using System.Diagnostics;
using FluentSharp.CoreLib;
//O2File:Tool_API.cs


namespace O2.XRules.Database.APIs
{
	public class IOLanguage_Installer_Test
	{
		public void test() 
		{
			new IOLanguage_Installer().start(); 
		}
	}
	public class IOLanguage_Installer : Tool_API 
	{	 
		public IOLanguage_Installer()
		{			
									
			config("IOLanguage", 
				   "http://iobin.suspended-chord.info/win32/iobin-win32-current.zip".uri(),				   
				   @"\IoLanguage\bin\io.exe");
    		installFromZip_Web();    		
			if (this.Executable.fileExists().isFalse())
			{				
				this.Install_Dir.pathCombine("IoLanguage-2012.11.09-win32.exe")
								.startProcess()
								.WaitForExit();
			}
		}
		
		public Process start()
		{
			if (isInstalled())
				return this.Executable.startProcess();
			return null;
		}		
	}
}