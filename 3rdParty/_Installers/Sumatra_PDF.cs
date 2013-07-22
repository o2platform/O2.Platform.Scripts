using FluentSharp.CoreLib;
//O2File:Tool_API.cs

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
			config("Sumatra PDF", "Sumatra PDF v2.2.1", "SumatraPDF-2.2.1-install.exe");			
    		Install_Uri = "https://kjkpub.s3.amazonaws.com/sumatrapdf/rel/SumatraPDF-2.2.1-install.exe".uri();
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