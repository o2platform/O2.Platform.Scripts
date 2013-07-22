using System.Diagnostics;
using FluentSharp.CoreLib;
//O2File:Tool_API.cs


namespace O2.XRules.Database.APIs
{
	public class ILSpy_Installer_Test
	{
		public void test()
		{
			new ILSpy_Installer().start(); 
		}
	}
	public class ILSpy_Installer : Tool_API 
	{	
		public ILSpy_Installer()
		{
			config("ILSpy", 
				   "http://downloads.sourceforge.net/project/sharpdevelop/ILSpy/2.0/ILSpy_Master_2.1.0.1603_RTW_Binaries.zip".uri(),				   
				   "ILSpy.exe");
    		installFromZip_Web(); 		
		}
		
				
		
		public Process start()
		{
			if (isInstalled())
				return this.Executable.startProcess();
			return null;
		}		
	}
}