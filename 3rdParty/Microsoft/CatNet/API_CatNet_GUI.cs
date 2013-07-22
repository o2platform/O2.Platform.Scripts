// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.FluentRoslyn;
using FluentSharp.REPL;
using FluentSharp.REPL.Controls;
using FluentSharp.WinForms;
using Microsoft.ACESec.CATNet.Core.Analysis;
using Microsoft.ACESec.CATNet.Core;
using Microsoft.ACESec.CATNet.UI;
using O2.XRules.Database.Utils;

//O2File:API_CatNet.cs
//O2File:_Extra_methods_WinForms_Component.cs

//O2Ref:CatNet_1.1/SourceDir/Microsoft.ACESec.CATNet.Core.dll
//O2Ref:CatNet_1.1/SourceDir/Microsoft.ACESec.CATNet.UI.VSAddIn.dll
//O2Ref:EnvDTE.dll
//O2Ref:Extensibility.dll

//O2File:_Extra_methods_Roslyn_API.cs

//O2Ref:EnvDTE.dll
//O2Ref:Extensibility.dll
//O2Ref:FluentSharp.Roslyn.dll
//O2Ref:Roslyn.Compilers.dll
//O2Ref:Roslyn.Compilers.CSharp.dll
//O2Ref:Roslyn.Services.dll


namespace O2.XRules.Database.APIs
{
	public class API_CatNet_GUI
	{
		public Control 	HostControl 			{ get; set; }
		public VsConnect vsConnect				{ get; set; }
		public SummaryView summaryView			{ get; set; }
		public ListView lvSummary				{ get; set; }
		public DetailView detailView			{ get; set; }
		public ListView lvDataFlow				{ get; set; }
		public ToolStrip _tsActions				{ get; set; }
		
		public TextBox  CompileResults			{ get; set; }
				
		public Report report					{ get; set; }
		
		public SourceCodeEditor 	CodeViewer					{ get; set; }
		public string 				 	DroppedFile					{ get; set; }
		//public bool  					IgnoreCodeViewOpenRequests 	{ get; set; }
		public bool  					TriggerOnSelectedEvent 		{ get; set; }
		public bool 					EngineBusy					{ get; set; }
		public string 					SolutionLoaded				{ get; set; }
	
		
		public Action<string,int> 		onSelectedCodeReference;
		public Action<string>	 		onSelectedReportItem;
		public Action	 				onScanCompleted;
		
		public RulesSettings rulesSettings;

        public API_CatNet_GUI()
        {
            "Microsoft.ACESec.CATNet.UI.VSAddIn.dll".assembly().location().info();      // ensure is loaded
        }
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
			
			catNetGui.CodeViewer 	= catNetGui.detailView.insert_Below().add_SourceCodeEditor();  
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
			
			catNetGui.lvSummary.remove_Event_SelectedIndexChanged(); // remove this event since it will use a method with DTE dependencies			
			catNetGui.lvSummary.add_ContextMenu();					// also remove (for now) the context menu
			catNetGui.lvDataFlow.showSelection();
			
			catNetGui.lvDataFlow.afterSelected<DataTransformation>(
				(dataTransformation)=>{				
											
											if (catNetGui.onSelectedCodeReference.notNull())
											{
												catNetGui.onSelectedCodeReference(dataTransformation.SourceFile, dataTransformation.SourceLine);
												//"Igonoring after select event for: {0}: {1}".debug(dataTransformation.SourceFile, dataTransformation.SourceLine);
												return;
											}
											if (dataTransformation.SourceFile.fileExists())
											{
												catNetGui.CodeViewer.open(dataTransformation.SourceFile)
														  			.gotoLine(dataTransformation.SourceLine);
												catNetGui.lvDataFlow.focus();		  
											}
											else
												//codeViewer.editor().set_Text("").sPathToFileLoaded = "";
												catNetGui.CodeViewer.editor().set_Text(dataTransformation.serialize(false), ".xml").sPathToFileLoaded = "";
												
									  });
			
			catNetGui.lvSummary.afterSelected<ReportItem>(
				(reportItem) => {		
									//reportItem.script_Me();
									var ruleName = (string)reportItem.field("_ruleInfo").field("Name");
									catNetGui.onSelectedReportItem.invoke(ruleName);
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
									if (catNetGui.TriggerOnSelectedEvent)
										catNetGui.lvDataFlow.select(1);
									catNetGui.lvSummary.focus();
								});
			catNetGui._tsActions.clearItems()
					  .add_Button("Scan Again"	   , "_btAnalyze", () => catNetGui.handleDrop(catNetGui.DroppedFile))					  
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
			catNetGui.CodeViewer.set_Text(catNetGui.report.serialize(false),".xml");
			return catNetGui;
		}
			
		public static API_CatNet_GUI scanAssembly(this API_CatNet_GUI catNetGui, string file)			  
		{ 
			catNetGui.SolutionLoaded = "";
			catNetGui.TriggerOnSelectedEvent = false;
			var catNet = new API_CatNet().loadRules();
			var savedReport = catNet.scan(file).savedReport();
			catNetGui.openReport(savedReport);
			return catNetGui;
		}
		
		public static API_CatNet_GUI scanSolution(this API_CatNet_GUI catNetGui, string solutionFile)				
		{
			catNetGui.TriggerOnSelectedEvent = true;
			catNetGui.SolutionLoaded = solutionFile;
			var catNet = new API_CatNet().loadRules();
			var assemblies = solutionFile.compileSolution();
			var savedReport = catNet.scan(assemblies).savedReport();			
			catNetGui.openReport(savedReport);		
			return catNetGui;
		}
		
		public static API_CatNet_GUI scanCSharpFile(this API_CatNet_GUI catNetGui, string file)	
		{
			catNetGui.SolutionLoaded = "";
			catNetGui.TriggerOnSelectedEvent = false;
			var catNet = new API_CatNet().loadRules(); 
			var assembly = new CompileEngine().compileSourceFile(file);
			if (assembly.notNull())			
			{
				catNetGui.openReport(catNet.scan(assembly).savedReport());				
			}
			else
				catNetGui.CodeViewer.open(file);					
			return catNetGui;	
		}
		
		public static API_CatNet_GUI scanScript(this API_CatNet_GUI catNetGui, string codeSnippet)
		{
			catNetGui.SolutionLoaded = "";
			catNetGui.TriggerOnSelectedEvent = false;
			//Action<string> compileAndScan = 
			//(text)=>{	
			if (codeSnippet.notValid() || catNetGui.EngineBusy)
				return catNetGui;
			
			catNetGui.EngineBusy = true;	
			"compiling and scanning".info();
			var ast = codeSnippet.contains("namespace ") ? codeSnippet.tree()
													     : codeSnippet.ast_Script();			
			var compilation = ast.compiler("test_Assembly_".add_RandomLetters())
							     .add_Reference("mscorlib")
							     .add_Reference("System")
							     .add_Reference("System.Web")
							     .add_Reference("System.Web.Services")
							     .add_Reference("System.Data");
			var errorDetails = compilation.errors_Details();
			if (errorDetails.valid())
			{
				catNetGui.CompileResults.pink();
				catNetGui.lvSummary.pink();
				catNetGui.CompileResults.set_Text(errorDetails);
				catNetGui.EngineBusy = false;
			}
			else 
			{
				catNetGui.CompileResults.set_Text("");
				catNetGui.CompileResults.azure();
				catNetGui.lvSummary.white();
				".dll".tempFile().info();
								
				var assembly = compilation.create_Assembly(".dll".tempFile());
				//"assembly: {0}".info(assembly.Location);
				catNetGui.handleDrop(assembly.location());
				
				catNetGui.EngineBusy = false;			
				//catNetGui.script_Me();
				if (catNetGui.lvSummary.items().size() > 0)
				{
					catNetGui.lvSummary.select(1);
				}				
			}
			return catNetGui;
		}
	
		public static API_CatNet_GUI loadFile(this API_CatNet_GUI catNetGui, string file)
		{						
			return catNetGui.handleDrop(file);
		}
		
		public static API_CatNet_GUI handleDrop(this API_CatNet_GUI catNetGui, string file)
		{
			try
			{
				if (file.inValid())
				{
			//		"There was no file to scan (drop or open a file first)".error();
					return catNetGui;				
				}
				catNetGui.lvSummary.pink();					
				catNetGui.DroppedFile = file;					
				if (file.isFile().isFalse())
					catNetGui.scanScript(file);
				else
				{
					"File Dropped: {0}".info(file);				
					switch(file.extension())
					{					
						case ".dll":
						case ".exe":
							catNetGui.scanAssembly(file);
							break;
						case ".sln":
							catNetGui.scanSolution(file);
							break;
						case ".cs":
							catNetGui.scanCSharpFile(file);
							break;
						case ".xml":
							catNetGui.openReport(file);
							break;
						default:
							"Provided file extension not supported: {0}".error(file.extension());
							return catNetGui;										
					}
				}
				catNetGui.lvSummary.white();
				catNetGui.onScanCompleted.invoke();
			}
			catch(Exception ex)
			{
				ex.log("[catNetGui] in loadFile: {0}".format(file));
			}
			return catNetGui;
		}
		
		public static API_CatNet_GUI add_CatNet(this Control control)
		{
			API_CatNet_Deployment.ensure_CatNet_Instalation();
			API_CatNet_Deployment.ensure_CatNet_Data();

			return createAndSetup(control);
		}

        public static API_CatNet_GUI createAndSetup(this Control control)
        {
            var catNetGui = new API_CatNet_GUI(); 
			catNetGui.setup_VsConnect(control.clear());
			catNetGui.set_GUI();			 
			catNetGui.set_VsConnect(); 
			return catNetGui;
        }
    }
}
		