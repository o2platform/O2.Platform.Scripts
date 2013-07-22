using System.Diagnostics;
using FluentSharp.CoreLib;

//O2File:Tool_API.cs
	
namespace O2.XRules.Database.APIs  
{	
	public class Spark_Installer_Test
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