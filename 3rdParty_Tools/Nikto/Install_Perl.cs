using System;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Install_Perl : Tool_API 
	{
		public string localDownloadFile = @"C:\Documents and Settings\Administrator\My Documents\Downloads\strawberry-perl-5.12.1.0.msi";
		
		public Install_Perl()
		{
			ToolName = "Perl";
    		Version = "Strawberry Perl 5.12.0";
    		Install_File = "strawberry-perl-5.12.1.0.msi";
    		VersionWebDownload = "http://strawberryperl.com/download/5.12.2.0/strawberry-perl-5.12.2.0.msi";
    		Install_Dir = @"C:\strawberry\";
		}
		
		
		public bool install()
		{
			"Installing Perl".info();
			return installFromMsi_Web(localDownloadFile);
			"Perl installation complete".info();
		}
		
	}
}