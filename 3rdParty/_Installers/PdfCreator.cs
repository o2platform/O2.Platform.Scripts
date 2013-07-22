using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs 
 
namespace O2.XRules.Database.APIs 
{
	public class PdfCreator_Install_Test
	{
		public static void test()  
		{
			new PdfCreator_Install().start(); 
		}
	}
	 
	public class PdfCreator_Install : Tool_API    
	{				
		public PdfCreator_Install()
		{			
			config("PdfCreator", 				   
				   "http://green.download.pdfforge.org/pdfcreator/1.4.0/PDFCreator-1_4_0_setup.exe".uri(),
				   "PdfCreator.exe");
			DownloadedInstallerFile = download(Install_Uri);
			DownloadedInstallerFile.startProcess();			
		}	
		//
		
		public Process start()
		{
			if (this.isInstalled())
				return this.Executable.startProcess(); 
			return null;
		}		
	}
}