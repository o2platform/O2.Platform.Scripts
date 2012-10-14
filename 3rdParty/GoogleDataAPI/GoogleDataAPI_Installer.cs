using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;
//O2File:Tool_API.cs

public class DynamicType
{
	public void dynamicMethod()
	{
		new O2.XRules.Database.APIs.GoogleDataAPI_Installer();
	}
}

namespace O2.XRules.Database.APIs 
{	
	public class GoogleDataAPI_Installer : Tool_API  
	{	
		public GoogleDataAPI_Installer()
		{
			config("GoogleDataAPI", 				   
				   "http://google-gdata.googlecode.com/files/Google_Data_API_Setup_2.1.0.0.msi".uri(),
				   @"SourceDir\Redist\Google.GData.Apps.dll");
			install_JustMsiExtract_into_TargetDir();
		}
				
		
		public Process start()
		{
			if (isInstalled())
				return Executable.startProcess();
			return null;
		}				
	}	
}