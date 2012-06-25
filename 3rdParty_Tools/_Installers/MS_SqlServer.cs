using System;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class MS_SqlServer_Test : Tool_API
	{
		public static void test()
		{
			new MS_SqlServer().install();
		}
	}
	public class MS_SqlServer : Tool_API
	{				 
		public MS_SqlServer()
		{
			ToolName = "SqlServer";
    		Version = "SqlServer Express 2008"; 
    		Install_File = "SQLEXPR32_x86_ENU.exe";    		
    		Install_Dir = @"C:\Program Files\Microsoft SQL Server\test";
    		VersionWebDownload = "http://www.microsoft.com/downloads/info.aspx?na=46&SrcFamilyId=01AF61E6-2F63-4291-BCAD-FD500F6027FF&SrcDisplayLang=en&u=http%3a%2f%2fdownload.microsoft.com%2fdownload%2f8%2fE%2f5%2f8E53FAA8-1129-4621-903F-3F8DB6D066AC%2fSQLEXPR32_x86_ENU.exe";    	
		}
		
		
		public bool install()
		{			
			return installFromMsi_Web();			
		}				
		
	}
}