using System;
using System.Diagnostics;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;

//O2File:Tool_API.cs

namespace O2.XRules.Database.APIs
{	
	public class API_Nikto_Test
	{
		/*public static void test()
		{
			new API_Nikto().setup();
		}*/
	}
	
	public class API_Nikto : Tool_API
	{				
		public string Nikto_Script { get; set; }
		public Process NiktoProcess { get; set;}
		public Action<string> OnConsoleData { get; set; }
		public string ExtraCommands {get; set;}
		
		public API_Nikto()
		{
			config("Nikto", "Nikto 2.1.3", "nikto-2.1.3.zip");
			Nikto_Script = Install_Dir.pathCombine(@"\nikto-2.1.3\nikto.pl");
			OnConsoleData = default_OnConsoleData;
		}
				
		public API_Nikto setup()
		{
			installFromZip_Web();		// install if required
			return this;
		}
		
		public override bool isInstalled()
		{
			if (Nikto_Script.fileExists())
			{
				"Nikto is installed because found file: {0}".debug(Nikto_Script);
				return true;
			}
			"Nikto is currently not installed".info();
			return false;
		}
		
		//by default sent it to the LogViewer
		public void default_OnConsoleData(string data)
		{	
			"[Nikto]: {0}".info(data);
		}
		
		
		public API_Nikto execute(string niktoCommands)
		{			
			var perlExe = @"C:\strawberry\perl\bin\perl.exe"; // change this for the Perl API					
			var workDirectory = Nikto_Script.directoryName();
			var processArguments = "{0} {1} {2}".format(Nikto_Script, niktoCommands, 
														  ExtraCommands ?? "");
			" *** Executing Nikto with commands: {0} **** ".debug(niktoCommands);

			NiktoProcess = Processes.startProcessAndRedirectIO(perlExe, processArguments, workDirectory, OnConsoleData);			
			return this;
		}
		
		public API_Nikto waitForExecutionCompletion()
		{
			if (NiktoProcess.notNull())
				NiktoProcess.WaitForExit();
			return this;
		}
		
		public API_Nikto stop()
		{
			if (NiktoProcess.notNull())
			{
				OnConsoleData("[O2 Message]: User Request -> Stopping Nikto Process");
				NiktoProcess.stop();
			}
			return this;
		}
	}
	
	public static class API_Nikto_ExtensionMethods
	{
		public static API_Nikto help(this API_Nikto nikto)
		{
			return nikto.execute("");
		}
		
		public static API_Nikto scan(this API_Nikto nikto,string ip)
		{
			return nikto.scan(ip,80);
		}
		public static API_Nikto scan(this API_Nikto nikto,string ip, int port)
		{
			var niktoCommands = "-host {0} -port {1}".format(ip, port);
			return nikto.execute(niktoCommands);
		}
		
		public static API_Nikto waitForExit(this API_Nikto nikto)
		{
			return nikto.waitForExecutionCompletion();
		}
	}
}