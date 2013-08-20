using System;
using System.Text;
using System.Diagnostics;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

//Installer:ZAProxy_Installer.cs!ZAP_D-2013-08-12\jar.jar

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace OWASP
{
	public class API_Zap
	{
		public Process 		  ZapProcess   	{ get; set; }
		public string  		  ZapLaunchCmd 	{ get; set; }
		public string  		  ZapDir 		{ get; set; }
		public Action<string> OnLogEntry    { get; set; }
		public StringBuilder  ExecutionLog 	{ get; set; }
		
		public API_Zap()
		{
			ZapDir =  PublicDI.config.ToolsOrApis.folder("ZAProxy").folders().first();
			ZapLaunchCmd = "-Xmx512m -XX:PermSize=256M -jar zap.jar %*";
			ExecutionLog = new StringBuilder();
			OnLogEntry  = (line)=> ExecutionLog.AppendLine(line.info());
		}
		
		public API_Zap Launch()
		{
			ZapProcess = "java".startProcess(ZapLaunchCmd, ZapDir, OnLogEntry); 
			return this;
		}
	}
	
	
}