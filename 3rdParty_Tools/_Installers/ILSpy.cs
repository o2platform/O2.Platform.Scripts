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
		public ILSpy() : this(true)
		{
		}
		
		public ILSpy(bool installNow)
		{
			config("ILSpy 2.0.0.15", "ILSpy_Master_2.0.0.1595", "ILSpy_Master_2.0.0.1595_RTW_Binaries.zip");			
    		Install_Uri = "http://downloads.sourceforge.net/project/sharpdevelop/ILSpy/2.0/ILSpy_Master_2.0.0.1595_RTW_Binaries.zip".uri();    		
    		if (installNow)
    			install();    		
		}
		
		
		public bool install()
		{
			"Installing {0}".info(ToolName);
			return installFromZip_Web(); 						
		}
		
		public Process start()
		{
			if (install())
				return Install_Dir.pathCombine("ILSpy.exe").startProcess();
			return null;
		}		
	}
}