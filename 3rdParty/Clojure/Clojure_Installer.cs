using System.Diagnostics;
using FluentSharp.CoreLib;
//O2File:Tool_API.cs


namespace O2.XRules.Database.APIs
{
	public class Clojure_Installer_Test
	{
		public void test()
		{
			new Clojure_Installer().start(); 
		}
	}
	public class Clojure_Installer : Tool_API 
	{	
		public Clojure_Installer()
		{
			config("Clojure", 			
				   "https://github.com/downloads/clojure/clojure-clr/clojure-clr-1.4.0-Debug-4.0.zip".uri(),
				   @"Debug 4.0\Clojure.Main.exe");    		    		
    		installFromZip_Web(); 		
		}
		
				
		
		public Process start()
		{			
			if (isInstalled())				
				return this.Executable.startProcess();
			return null;
		}		
	}
}