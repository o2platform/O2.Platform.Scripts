using System;
using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{
	public class Install_Test
	{
		public void test()
		{
			new Antlr_Installer().start();
		}
	}
	public class Antlr_Installer : Tool_API 
	{			
		
		public Antlr_Installer()
		{
			config("Antlr", 
				   "http://www.tunnelvisionlabs.com/downloads/antlr/antlr-dotnet-csharpruntime-3.4.1.9004.7z".uri(),
				   "Antlr3.Runtime.dll");
			installFromZip_Web(); 							
		}
						
		
		public Process start()
		{			
			if (this.isInstalled())
				return this.Install_Dir.startProcess();
			return null;
		}		
	}
}