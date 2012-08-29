// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;
using O2.XRules.Database.Findings;
using O2.Views.ASCX.O2Findings;
using O2.Scanner.MsCatNet.Converter;
using O2.Interfaces.O2Findings;
using Microsoft.ACESec.CATNet.Core.Driver; 
using Rules = Microsoft.ACESec.CATNet.Core.Rules;

//O2File:Findings_ExtensionMethods.cs
//Installer:CatNet_Installer.cs!CatNet_1.1/SourceDir/Microsoft.ACESec.CATNet.Core.dll
//O2Ref:_O2_Scanner_MsCatNet.exe
//O2Ref:CatNet_1.1/SourceDir/Microsoft.ACESec.CATNet.Core.dll
//_O2Ref:CatNet_64/CAT.NET/Microsoft.ACESec.CATNet.Core.dll

namespace O2.XRules.Database.APIs
{
	public class API_CatNet
	{				
		public Engine		 	Engine 			{ get; set; }	
		public CatNetEvents	 	EventsLog 		{ get; set; }	
		public string			Rules_Folder  	{ get; set; }
				
		public API_CatNet()
		{			
			Engine = Engine.Create(EngineType.Builtin);
			EventsLog = new CatNetEvents();
			Engine.EventSink = EventsLog;
			Rules_Folder = this.engineDirectory().pathCombine("Rules");
			
			var tempReportFile = "_catNet_reports".tempDir(false)
											  .pathCombine(10.randomLetters() + ".xml");
			this.save_Report_To(tempReportFile);						  
		}	
	}			
		
	public class CatNetEvents	: EventSink
	{
		public StringBuilder OutputLines 		{ get; set;}		
		public bool 		 ShowDebugEvents	{ get; set; }
		
		public CatNetEvents()
		{
			OutputLines = new StringBuilder();
			ShowDebugEvents = false;
		}
		public override void AssemblyLoadFailure(string path)
		{
			"[CatNetEvents] AssemblyLoadFailure: {0}".error(path);			
		}

		public override void Output(EventSink.OutputType type, string format, params object[] p)
		{
			if (type == EventSink.OutputType.Debug && ShowDebugEvents.isFalse())
				return;
			var outputLine = "{0} : {1}".format(type, format.format(p));
			OutputLines.AppendLine(outputLine);
			"[CatNetEvents] {0}".info(outputLine);
		}
	}
	
	public static class API_CatNet_ExtensionMethods_Setup
	{
		public static API_CatNet loadRules(this API_CatNet catNet)
		{
			catNet.Engine.RulesEngine.LoadDirectory(catNet.Rules_Folder);
			return catNet;
		}
		
		public static List<Rules.Rule> rules(this API_CatNet catNet)
		{
			return catNet.Engine.RulesEngine.Rules.toList();
		}
		
		public static List<string> rules_Names(this API_CatNet catNet)
		{
			return (from rule in catNet.rules()
					select rule.Info.Name).toList();
		}
		
		public static Rules.Rule rule(this API_CatNet catNet, string ruleName)
		{
			return (from rule in catNet.rules()
				    where rule.Info.Name == ruleName
				    select rule).first();
		}

		public static string engineDirectory(this API_CatNet catNet)
		{
			return catNet.Engine.type().Assembly.Location.directoryName();
		}
	}
	
	public static class API_CatNet_ExtensionMethods_Scan
	{
		public static API_CatNet scan(this API_CatNet catNet, string file)
		{
			return catNet.analyze(file);
		}
		public static API_CatNet analyze(this API_CatNet catNet, string file)
		{		
			var o2Timer = new O2Timer("Scanned file in :").start();
			catNet.Engine.Analyze(file);
			o2Timer.stop();
			
			catNet.Engine.AnalysisEngine.ResetState();
			return catNet;			
		}
		
		public static string output(this API_CatNet catNet)
		{
			return catNet.EventsLog.OutputLines.str();
		}		
		
		public static string savedReport(this API_CatNet catNet)
		{
			return catNet.Engine.Configuration.ReportFile;
		}		
		
		public static API_CatNet save_Report_To(this API_CatNet catNet, string file)
		{			
			file.deleteIfExists();
			catNet.Engine.Configuration.ReportFile = file;
			catNet.Engine.Configuration.ReportXslOutputFile = file + ".html";
			return catNet;
		}
		
		public static API_CatNet save_GraphML_To(this API_CatNet catNet, string file)
		{
			catNet.Engine.Configuration.DataFlowGraphFile = file;
			return catNet;
		}	
				
	}
	
	public static class API_CatNet_ExtensionMethods_Scan_Direclty	
	{
		public static API_CatNet scan_Assembly(this API_CatNet catNet, Assembly assemblyToScan)
		{			
			return catNet.scan(assemblyToScan);
		}
		
		public static API_CatNet scan(this API_CatNet catNet, Assembly assemblyToScan)
		{			
			return catNet.scan(new List<Assembly>().add(assemblyToScan));
		}
		
		public static API_CatNet scan(this API_CatNet catNet, List<Assembly> assembliesToScan)
		{			
			return catNet.analyze(assembliesToScan);			
		}
		
				
		public static API_CatNet analyze(this API_CatNet catNet, List<Assembly> assembliesToScan)
		{			
			var o2Timer = new O2Timer("Scanned assembly in :").start();
			try
			{
				var analysisEngine = catNet.Engine.AnalysisEngine;
				
				analysisEngine.BeginAnalysis();
				
				var cci = "CatNet_1.1/SourceDir/Microsoft.Cci.dll".assembly();
				var assemblyNode = cci.type("AssemblyNode");			
				foreach(var assemblyToScan in assembliesToScan)
				{
					var module = assemblyNode.invokeStatic("GetAssembly", assemblyToScan, null, true,true,true);
				
					if (module.notNull())
					{
						analysisEngine.invoke("AnalyzeModule", module);						
					}
					else
						"[API_CatNet][analyze] could not get Module for provided assembly: {0}".error(assemblyToScan);				
				}		
				analysisEngine.FinalizeState();
				catNet.Engine.RulesEngine.Process(analysisEngine);												
				analysisEngine.EndAnalysis();
				catNet.Engine.SaveReport();														
				analysisEngine.ResetState();
				o2Timer.stop();
			}
			catch(Exception ex)
			{
				ex.log("[API_CatNet][analyze] assembly");
			}
			return catNet;
		}
	}

	public static class API_CatNet_ExtensionMethods_Scan_O2Findings
	{
		public static List<IO2Finding> findings(this string reportFile)
		{
			
			var findingsFile = "{0}.findings".format(reportFile);		
			findingsFile.deleteIfExists();
			var catNetConverter = new  CatNetConverter(reportFile);
			catNetConverter.convert(findingsFile);
			if (findingsFile.fileExists())			
				return findingsFile.loadFindingsFile();			
			return new List<IO2Finding>();	
		}
		
		public static ascx_FindingsViewer add_CatNet_FindingsViewer(this Control control, string reportFile, ascx_SourceCodeEditor codeEditor = null)
		{
			var findingsViewer = control.control<ascx_FindingsViewer>();						
			if (findingsViewer.notNull())
			{				
				findingsViewer.clearO2Findings();							
				findingsViewer.load(reportFile.findings());
				return findingsViewer;
			}
			control.clear();
			findingsViewer = control.add_FindingsViewer(codeEditor.isNull()) 
						  			.load(reportFile.findings());			
			
			return findingsViewer;
		}
		
		public static ascx_FindingsViewer show(this API_CatNet catNet, Control control, ascx_SourceCodeEditor codeEditor = null)
		{			
			var reportFile = catNet.Engine.Configuration.ReportFile;
			if (reportFile.valid())
				return control.add_CatNet_FindingsViewer(reportFile, codeEditor);
			return null;
		}
	}
	
	public class API_CatNet_Deployment
	{
		public static bool ensure_CatNet_Instalation()
		{			
			unzipEmbeddedResourceToFolder("CatNet_Files.zip" , @"..\_ToolsOrAPIs\CatNet_1.1");
			var result = @"CatNet_1.1\SourceDir\Microsoft.ACESec.CATNet.Core.dll".assembly().notNull();	
			if 	(result)
				"CatNet is Installed OK".debug();
			else
				"CatNet is NOT Installed correctly".error();
			return result;
		}
		
		public static bool ensure_CatNet_Data()
		{
			return unzipEmbeddedResourceToFolder("LocalScriptsFolder.zip" , PublicDI.config.LocalScriptsFolder);
		}
		
		public static bool unzipEmbeddedResourceToFolder(string resourceToUnzip, string virtalPathToTargetFolder)
		{
			var assembly = System.Reflection.Assembly.GetEntryAssembly();
			var targetFile = PublicDI.config.O2TempDir.pathCombine(resourceToUnzip);
			var targetFolder = PublicDI.config.O2TempDir.pathCombine(virtalPathToTargetFolder);
			if (targetFile.fileExists().isFalse())
			{
				foreach (var resourceName in assembly.GetManifestResourceNames())
				{
					"resourceName: {0}".debug(resourceName);
					if (resourceName == resourceToUnzip)
					{						
						"Extracting embeded reference and saving it to: {0}".info(targetFile);
						var assemblyStream = assembly.GetManifestResourceStream(resourceName);
						byte[] data = new BinaryReader(assemblyStream).ReadBytes((int)assemblyStream.Length);				
						Files.WriteFileContent(targetFile,data);											
						"unzing to: {0}".info(targetFolder);
						targetFile.unzip_File(targetFolder.createDir());										
					}		
				}
			}
			if (targetFile.fileExists() && targetFolder.dirExists())
				return true;
			"requested embedded resource was not found: {0}".info(resourceToUnzip);
			return false;
			
		}
	}
	
	
	
	
	[XmlRoot("CatNetRules")]
	public class CatNet_Rules_Mappings : List<Rule_Mapping>
	{
		
	}
	[XmlRoot("Mapping")]
	public class Rule_Mapping
	{
		[XmlAttribute] public string VulnName { get ; set; }
		[XmlAttribute] public string Target { get ; set; }		
	}
	
	public static class CatNet_Rules_Mappings_ExtensionMethods
	{
		public static CatNet_Rules_Mappings add(this CatNet_Rules_Mappings rulesMappings, string vulnName, string target)
		{
			rulesMappings.Add(new Rule_Mapping() { VulnName = vulnName, Target = target });
			return rulesMappings;
		} 
		
		public static string item(this CatNet_Rules_Mappings rulesMappings, string vulnName)
		{
			return rulesMappings.Where((mapping)=>mapping.VulnName == vulnName)
								.Select((mapping)=> mapping.Target).first();
		}
		public static bool hasItem(this CatNet_Rules_Mappings rulesMappings, string vulnName)
		{
			return rulesMappings.item(vulnName).notNull();
		}
	}
	
	

}