using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;

//O2File:Tool_API.cs
	
namespace O2.XRules.Database.APIs  
{	
	public class Launcher
	{
		public static void launch()
		{
			new Spark_Installer().start(); 
		}
	}
	public class Spark_Installer : Tool_API 
	{					
		
		public Spark_Installer()
		{
			config("Spark", 
				   "http://download.igniterealtime.org/spark/spark_2_6_3.exe".uri(),		   
				   //"http://download.igniterealtime.org/spark/online/spark_2_6_3_online.exe".uri(),
				   ProgramFilesFolder.pathCombine(@"Spark\\Spark.exe"));
			
    		installFromExe_Web();    		    		
		}
				
		public Process start()
		{
			if (isInstalled())
				this.Executable.startProcess();											
			return null;
		}		
	}
}