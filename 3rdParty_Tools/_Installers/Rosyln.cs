using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;
//O2File:Tool_API.cs


namespace O2.XRules.Database.APIs
{
	public class testInstall
	{
		public static void test() 
		{
			new Roslyn();
		}
	}
	
	public class Roslyn : Tool_API 
	{				
		public Roslyn()
		{
			
			config("Roslyn", 
				   "https://nugetgallery.blob.core.windows.net/packages/Roslyn.1.0.11014.5.nupkg".uri(),
				   "Roslyn.nuspec");
			installFromZip_Web();	       		
		}
		
		public Process start()
		{
			if (this.isInstalled())
				return this.Executable.startProcess(); 
			return null;
		}		
	}
}