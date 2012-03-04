using System;
using System.Diagnostics; 
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;
 
//O2File:Tool_API.cs
//O2File:_Extra_methods_Windows.cs

namespace O2.XRules.Database.APIs
{

	public class LinqPad_Test
	{
		public void test()
		{
			new LinqPad().start(); 
		}
	}
	public class LinqPad : Tool_API 
	{	
		public LinqPad() : this(true)
		{
		}
		
		public LinqPad(bool installNow)
		{
			
    		
    		config("LinqPad", 
    			  "LinqPad v3.5", 
    			  @"C:\Program Files\LinqPad\",
    			  "LINQPadSetup.exe",
    			  "http://www.linqpad.net/GetFile.aspx?LINQPadSetup.exe".uri());
    		Executable = Install_Dir.pathCombine("LINQPad.exe");
    		if (installNow)
    			install();    		
		}		
		
		public bool install()
		{						
			return installFromMsi_Web();						
		}
		
		public Process start()
		{
			if (install())
				Executable.startProcess();
			return null;
		}
		
	}
}