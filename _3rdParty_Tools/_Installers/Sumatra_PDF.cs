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
	public class Sumatra_PDF_Test
	{
		public void test()
		{
			new Sumatra_PDF();
		}
	}
	public class Sumatra_PDF : Tool_API 
	{	
		public Sumatra_PDF() : this(true)
		{
		}
		
		public Sumatra_PDF(bool installNow)
		{
			config("Sumatra PDF", "Sumatra PDF v1.4", "SumatraPDF-1.4-install.exe");			
    		Install_Uri = "http://kjkpub.s3.amazonaws.com/sumatrapdf/rel/SumatraPDF-1.4-install.exe".uri();
    		if (installNow)
    			install();		
		}
		
		
		public bool install()
		{
			"Installing {0}".info(ToolName);
			return installFromMsi_Web(); 						
		}
				
	}
}