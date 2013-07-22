using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{

	public class LinqPad_Installer_Test
	{
		public void test()
		{
			new LinqPad_Installer().start(); 
		}
	}
	public class LinqPad_Installer : Tool_API 
	{	
		public LinqPad_Installer() : this(true)
		{
		}
		
		public LinqPad_Installer(bool installNow)
		{			    		
    		/*config("LinqPad", 
    			  "LinqPad v3.5", 
    			  @"C:\Program Files\LinqPad\",
    			  "LINQPadSetup.exe",
    			  "http://www.linqpad.net/GetFile.aspx?LINQPadSetup.exe".uri());
    		
    		*/    		
    		config("_LinqPad", "LinqPad.4.0", "LINQPad4.zip");			
    		Install_Uri = "http://www.linqpad.net/GetFile.aspx?LINQPad4.zip".uri();    		
    		Executable = Install_Dir.pathCombine("LINQPad.exe");
    		if (installNow)
    			install();    		
		}		
		
		public bool install()
		{						
			return installFromZip_Web();						
		}
		
		public Process start()
		{
			if (install())
				Executable.startProcess();
			return null;
		}
		
	}
}