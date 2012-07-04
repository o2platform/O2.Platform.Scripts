using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
//O2File:Tool_API.cs

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs
using O2.XRules.Database.Utils;


namespace O2.XRules.Database.APIs
{
	public class ILSpy_Test
	{
		public void test()
		{
			new ILSpy().start(); 
		}
	}
	public class ILSpy : Tool_API 
	{	
		public ILSpy()
		{
			config("ILSpy 2.1.0.1603", 
				   "http://downloads.sourceforge.net/project/sharpdevelop/ILSpy/2.0/ILSpy_Master_2.1.0.1603_RTW_Binaries.zip".uri(),
				   "ILSpy.exe");
    		
    		//http://ignum.dl.sourceforge.net/project/sharpdevelop/ILSpy/2.0/ILSpy_Master_2.1.0.1603_RTW_Binaries.zip
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