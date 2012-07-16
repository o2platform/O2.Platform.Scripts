// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.XRules.Database.Utils;
using O2.Scanner.MsCatNet.Converter;
using Microsoft.ACESec.CATNet.Core.Driver; 
using Microsoft.ACESec.CATNet.Core.Analysis;
using Microsoft.ACESec.CATNet.Core;
using Microsoft.ACESec.CATNet.UI;
using Rules = Microsoft.ACESec.CATNet.Core.Rules;

//O2File:API_CatNet.cs
//O2File:_Extra_methods_WinForms_Component.cs

//O2Ref:_O2_Scanner_MsCatNet.exe
//O2Ref:CatNet_1.1/SourceDir/Microsoft.ACESec.CATNet.Core.dll
//O2Ref:CatNet_1.1/SourceDir/Microsoft.ACESec.CATNet.UI.VSAddIn.dll
//O2Ref:EnvDTE.dll
//O2Ref:Extensibility.dll

//_O2File:_Extra_methods_Roslyn_API.cs

//O2Ref:EnvDTE.dll
//O2Ref:Extensibility.dll
// O2Ref:O2_FluentSharp_Roslyn.dll
// O2Ref:Roslyn.Compilers.dll
// O2Ref:Roslyn.Compilers.CSharp.dll
// O2Ref:Roslyn.Services.dll


namespace O2.XRules.Database.APIs
{
	public class API_CatNet_GUI
	{
		public Control 	HostControl 	{ get; set; }
		public VsConnect vsConnect;
		public SummaryView summaryView;
		public ListView lvSummary;
		public DetailView detailView;
		public ListView lvDataFlow;
		public ToolStrip _tsActions;	
		
		public ascx_SourceCodeEditor codeViewer;
		public Report report;
		
		public string droppedFile;
		
		public RulesSettings rulesSettings;
	}
	
	public static class API_CatNet_GUI_ExtensionMethods_Setup
	{
		public static API_CatNet_GUI setup_VsConnect(this API_CatNet_GUI catNetGui, Control topPanel)
		{
			catNetGui.HostControl 	= topPanel;
			catNetGui.vsConnect	 	= new VsConnect();		
			catNetGui.summaryView 	= topPanel.clear().add_Control<SummaryView>();
			catNetGui.lvSummary 	= (ListView)	catNetGui.summaryView.field("_lvSummary");
			catNetGui.detailView 	= (DetailView)	catNetGui.summaryView.field("_detailView");
			catNetGui.lvDataFlow 	= (ListView)	catNetGui.detailView.field("_lvDataFlow"); 
			catNetGui._tsActions 	= (ToolStrip)	catNetGui.summaryView.field("_tsActions");		
			
			catNetGui.codeViewer 	= catNetGui.detailView.insert_Below().add_SourceCodeEditor();  
			return catNetGui;
		}
		
		public static API_CatNet_GUI set_VsConnect(this API_CatNet_GUI catNetGui)
		{		
			catNetGui.rulesSettings = new RulesSettings();		
						
			catNetGui.vsConnect.invoke("AddDefaultSettingsProvider", new GeneralSettings());
			catNetGui.vsConnect.invoke("AddDefaultSettingsProvider", catNetGui.rulesSettings);
			catNetGui.vsConnect.invoke("AddDefaultSettingsProvider", new TargetsSettings());
			catNetGui.vsConnect.invoke("AddDefaultSettingsProvider", new SuppressionSettings());
			catNetGui.vsConnect.invoke("AddDefaultSettingsProvider", new VectorsSettings());
			var defaultSettings = catNetGui.vsConnect.field("_defaultSettings");
			catNetGui.vsConnect.field("_currentSettings", defaultSettings);
			
			catNetGui.rulesSettings.Activate();
						
			catNetGui.summaryView.Controller = catNetGui.vsConnect;								
			return catNetGui;
		}
		
		
		public static API_CatNet_GUI set_GUI(this API_CatNet_GUI catNetGui)
		{
			
			catNetGui._tsActions.clearItems() 
					  .add_Label("..... loading engine.....");
			
			catNetGui.lvSummary.remove_Event_SelectedIndexChanged(); //remove this event since it will use a method with DTE dependencies
			catNetGui.lvDataFlow.showSelection();
			
			catNetGui.lvDataFlow.afterSelected<DataTransformation>(
				(dataTransformation)=>{	
											if (dataTransformation.SourceFile.fileExists())
											{
												catNetGui.codeViewer.open(dataTransformation.SourceFile)
														  			.gotoLine(dataTransformation.SourceLine);
												catNetGui.lvDataFlow.focus();		  
											}
											else
												//codeViewer.editor().set_Text("").sPathToFileLoaded = "";
												catNetGui.codeViewer.editor().set_Text(dataTransformation.serialize(false), ".xml").sPathToFileLoaded = "";
												
									  });
			
			catNetGui.lvSummary.afterSelected<ReportItem>(
				(reportItem) => {					
									catNetGui.detailView.Clear();
									reportItem.DisplayText(catNetGui.detailView);
									
									var transformations = (ArrayList)reportItem._result.Transformations;
									foreach(DataTransformation dataTransformation in transformations)
									{
										var fileSnippet = (dataTransformation.SourceFile.fileExists()) 
																? 	dataTransformation.SourceFile.fileContents().split_onLines().value(dataTransformation.SourceLine-1).trim()
																: 	"";
										catNetGui.lvDataFlow.add_Row(dataTransformation.StatementMethodName ?? "", 
														   dataTransformation.SourceLine.ToString(),
														   dataTransformation.InputVariableString ?? "", 
														   dataTransformation.OutputVariableString ?? "",
														   fileSnippet)
												  .tag(dataTransformation);
									} 
									catNetGui.detailView.AutoResizeColumns();
									catNetGui.lvDataFlow.select(1);
									catNetGui.lvSummary.focus();
								});
			catNetGui._tsActions.clearItems()
					  .add_Button("Scan Again"	   , "_btAnalyze", () => catNetGui.handleDrop(catNetGui.droppedFile))					  
					  .add_Button("View loaded report xml" 			, "_btGeneralSettings" ,()=> catNetGui.viewReportXml());
			
			catNetGui.summaryView.onDrop((file)=>O2Thread.mtaThread(()=>catNetGui.handleDrop(file)));
		
			return catNetGui;
		}
		
		
		//public static API_CatNet_GUI set_Actions(this API_CatNet_GUI catNetGui)
		
		public static API_CatNet_GUI openReport(this API_CatNet_GUI catNetGui, string path)
		{
			catNetGui.summaryView.invokeOnThread(
				()=>{
						catNetGui.report = Report.Load(path);				 
						catNetGui.summaryView.LoadReport(catNetGui.report, Path.ChangeExtension(path, ".htm"));
					 });
			return 	catNetGui;
		}
		
		public static API_CatNet_GUI viewReportXml(this API_CatNet_GUI catNetGui)
		{
			catNetGui.codeViewer.set_Text(catNetGui.report.serialize(false),".xml");
			return catNetGui;
		}
			
		public static API_CatNet_GUI scanAssembly(this API_CatNet_GUI catNetGui, string file)			  
		{ 
			var catNet = new API_CatNet().loadRules();
			var savedReport = catNet.scan(file).savedReport();
			catNetGui.openReport(savedReport);
			return catNetGui;
		}
		
/*		public static API_CatNet_GUI scanSolution(this API_CatNet_GUI catNetGui, string solutionFile)				
		{
			var catNet = new API_CatNet().loadRules();
			var assemblies = solutionFile.compileSolution();
			var savedReport = catNet.scan(assemblies).savedReport();
			catNetGui.openReport(savedReport);		
			return catNetGui;
		}*/
		
		public static API_CatNet_GUI scanCSharpFile(this API_CatNet_GUI catNetGui, string file)	
		{
			var catNet = new API_CatNet().loadRules(); 
			var assembly = new CompileEngine().compileSourceFile(file);
			if (assembly.notNull())			
			{
				catNetGui.openReport(catNet.scan(assembly).savedReport());				
			}
			else
				catNetGui.codeViewer.open(file);					
			return catNetGui;	
		}
		
		
		public static API_CatNet_GUI loadFile(this API_CatNet_GUI catNetGui, string file)
		{			
			return catNetGui.handleDrop(file);
		}
		
		public static API_CatNet_GUI handleDrop(this API_CatNet_GUI catNetGui, string file)
		{
			if (file.inValid())
			{
				"There was no file to scan (drop or open a file first)".error();
				return catNetGui;				
			}
			catNetGui.lvSummary.pink();					
			catNetGui.droppedFile = file;					
			"File Dropped: {0}".info(file);
			switch(file.extension())
			{					
				case ".dll":
				case ".exe":
					catNetGui.scanAssembly(file);
					break;
		//		case ".sln":
		//			catNetGui.scanSolution(file);
		//			break;
				case ".cs":
					catNetGui.scanCSharpFile(file);
					break;
				case ".xml":
					catNetGui.openReport(file);
					break;
				default:
					"dropped file extension not supported: {0}".error(file.extension());
					break;
			}
			catNetGui.lvSummary.white();
			return catNetGui;
		}
		
		public static API_CatNet_GUI add_CatNet(this Control control)
		{
			API_CatNet_Deployment.ensure_CatNet_Instalation();
			API_CatNet_Deployment.ensure_CatNet_Data();

			var catNetGui = new API_CatNet_GUI(); 
			catNetGui.setup_VsConnect(control.clear());
			catNetGui.set_GUI();			 
			catNetGui.set_VsConnect(); 
			return catNetGui;
		}
	}
}
		