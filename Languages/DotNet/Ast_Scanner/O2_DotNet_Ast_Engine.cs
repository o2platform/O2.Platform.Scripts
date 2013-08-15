// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;
using FluentSharp.CSharpAST;
using FluentSharp.CSharpAST.Utils;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL;
using FluentSharp.REPL.Controls;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using GraphSharp.Controls;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using O2.XRules.Database.APIs;
using O2.XRules.Database.Findings;
using O2.XRules.Database.Utils;

//O2File:WPF_GUI.cs
//O2File:GraphLayout_O2CodeStream_ExtensionMethods.cs
//O2File:VerticesAndEdges_ExtensionMethods.cs
//O2File:Wpf_TextEditor_ExtensionMethods.cs

//O2File:ascx_Execute_Scripts.cs
//O2File:Ast_Engine_ExtensionMethods.cs
//O2File:Findings_ExtensionMethods.cs
//O2File:SharpDevelop_O2MappedAstData_ExtensionMethods.cs
//O2File:TextEditor_O2CodeStream_ExtensionMethods.cs
//O2File:MethodMappings_ExtensionMethods.cs

//O2File:ascx_GraphAst_MethodCalls.cs
//O2File:ascx_WriteRule.cs
//O2File:ascx_ManualMethodStreams.cs
//O2File:ascx_SearchAST.cs
//O2File:ascx_ViewAST.cs

//O2File:ascx_GraphAst_MethodCalls.cs 

//O2Ref:System.Data.dll 
//O2Ref:O2_External_IE.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:System.Xml.dll
//O2Ref:System.Xaml.dll
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll
//O2Ref:GraphSharp.dll
//O2Ref:GraphSharp.Controls.dll
//O2Ref:ICSharpCode.AvalonEdit.dll
//O2Ref:FluentSharp.WPF.dll
//O2Ref:PresentationFramework.dll 
//O2Ref:PresentationCore.dll 
//O2Ref:QuickGraph.dll
//O2Ref:WindowsBase.dll
//O2Ref:WindowsFormsIntegration.dll



namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{	
	public class O2_DotNet_Ast_Engine_Test
	{
		public void launchGui()
		{
			new O2_DotNet_Ast_Engine().start();
		}
	}
    public class O2_DotNet_Ast_Engine : UserControl
	{	 			
	 	public Panel HostPanel { get; set; }
	 	public TreeView Step_1_TreeView_SourceFiles { get; set; }	 	
	 	public ToolStripStatusLabel StatusLabel { get; set; }
	 	public ProgressBar TopProgressBar { get ; set; }
	 	
	 	//public string step_2_WikiHelpPage = "O2 DotNet Ast Engine - Step 2: view method streams";
	 	public Dictionary<string, Assembly> ReferencedAssemblies;
	 	public O2MappedAstData AstData { get; set; }
	 	//public List<IO2Finding> CreatedO2Findings  { get; set; }

		public ascx_WriteRule WriteRule_Step { get; set; }
		
		public ascx_ManualMethodStreams ManualMethodStreams_Step  { get; set; }
		public ascx_SearchAST SearchAST_Step  { get; set; }
		public ascx_ViewAST ViewAST_Step { get; set; }
		//public Dictionary<IMethod, string> MethodStreams { get; set; }

		public void start()
	 	{
	 		var astEngine = O2Gui.open<O2_DotNet_Ast_Engine>("O2 .NET Ast Engine", 800,600);
	 		astEngine.buildGui();	
	 		//astEngine.step_LoadArtifacts();
	 		//astEngine.AstData.O2AstResolver.addReference("System.Data");
			//astEngine.loadSourceFiles(@"C:\O2\DemoData\HacmeBank_v2.0 (Dinis version - 7 Dec 08)\HacmeBank_v2_WS".files("*.cs",true));

			//astEngine.step_WhoCallsWho();
	 	}
	 	
		public O2_DotNet_Ast_Engine()
		{
			this.width(500);
			this.height(400);
			AstData = new O2MappedAstData(); 
			ReferencedAssemblies = new  Dictionary<string, Assembly>();
			//CreatedO2Findings = new List<IO2Finding>();
			//MethodStreams = new Dictionary<IMethod, string>();
		}
		
	 	
	 	
		public void buildGui() 
		{		 
								
			/*var statusStrip = this.parentForm().controls(true).control<StatusStrip>();
			
			if (statusStrip != null)
				StatusLabel = (ToolStripStatusLabel)statusStrip.Items[0];
			else
				StatusLabel = this.add_StatusStrip();
			*/
			var wpfGui = this.add_Control<WPF_GUI>();
			wpfGui.insert_Below<ascx_LogViewer>(100);
			TopProgressBar = wpfGui.insert_Below<ProgressBar>(10);
			wpfGui.buildGui();
			StatusLabel = wpfGui.StatusLabel;
			statusMessage("building Gui");
			wpfGui.add_Section("O2 AST Engine", "This is the development enviroment for performing advanced analysis on top of O2's .NET AST Engine")
				  .add_Link("Load Artifacts",()=> step_LoadArtifacts())
				  .add_Link("view AST",()=> step_ViewAST())
				  .add_Link("search AST",()=> step_SearchAST())
				  .add_Link("search Comments",()=> step_SearchComments())
				  .add_Link("who calls who?",()=> step_WhoCallsWho())
				  .add_Link("method streams",()=> step_MethodStreams())
				  .add_Link("manual method streams",()=> step_ManualMethodStream())
				  .add_Link("write rule",()=> step_WriteRule())
				  .add_Link("view findings",()=> step_ViewFindings());
						
			HostPanel = wpfGui.WinFormPanel;
			return;
			//add_StatusStrip();
			//statusStrip.set_Text("test");
			/*
			var controls = this.add_1x1("actions", "", false, 40);		
			TopProgressBar =  controls[0].add_Link("Step 1: load artifacts (C# source code)", 15, 2, () => step_LoadArtifacts())
								         .append_Link("view AST", ()=> step_ViewAST())
								         .append_Link("search AST", ()=> step_SearchAST())								         
								         .append_Link("search Comments", ()=> step_SearchComments())
								         .append_Link("who calls who?", ()=> step_WhoCallsWho())
								         .append_Link("method streams", ()=> step_MethodStreams())
								         .append_Link("manual method stream", ()=> step_ManualMethodStream())
								         .append_Link("write rule", ()=> step_WriteRule())								         
								         .append_Link("view findings",()=> step_WriteRule())
								         .append_Control<ProgressBar>();
			TopProgressBar.align_Right(controls[0]);        
			TopProgressBar.top(TopProgressBar.Top -4);
			 
			HostPanel = controls[1].add_Panel();

            HostPanel.parent<SplitContainer>().borderNone();
            HostPanel.parent<SplitContainer>().fixedPanel1();
            
            HostPanel.insert_Below<ascx_LogViewer>(100); 
			
			statusMessage("all ready...");
			*/
		}
		
		public Step_LoadArtifacts step_LoadArtifacts()
		{
			return new Step_LoadArtifacts(this);
		}
		
		public ascx_ViewAST step_ViewAST()
		{
			ViewAST_Step = show_Step<ascx_ViewAST>(ViewAST_Step);
			return ViewAST_Step;
		}
		
		public ascx_WriteRule step_SearchAST()
		{
			WriteRule_Step = show_Step<ascx_WriteRule>(WriteRule_Step);
			return WriteRule_Step;
		}			
		
		public Step_SearchComments step_SearchComments()
		{
			return new Step_SearchComments(this);
		}
		
		public Step_WhoCallsWho step_WhoCallsWho()
		{
			return new Step_WhoCallsWho(this);
		}
		
		
		public Step_MethodStreams step_MethodStreams()
		{
			return new Step_MethodStreams(this);
		}				
		
		public ascx_ManualMethodStreams step_ManualMethodStream()
		{
			ManualMethodStreams_Step = show_Step<ascx_ManualMethodStreams>(ManualMethodStreams_Step);
			return ManualMethodStreams_Step;
			
			/*this.HostPanel.clear();
			if (ManualMethodStreams_Step.isNull())
			{
				ManualMethodStreams_Step = this.HostPanel.add_Control<ascx_ManualMethodStreams>();
				ManualMethodStreams_Step.buildGui();
			}
			else
				this.HostPanel.add_Control(ManualMethodStreams_Step);
			//WriteRule_Step
			return ManualMethodStreams_Step;*/
		}
		
		
		public ascx_WriteRule step_WriteRule()
		{
			WriteRule_Step = show_Step<ascx_WriteRule>(WriteRule_Step);
			return WriteRule_Step;
			
			//WriteRule_Step = this.newInThread< new ascx_WriteRule();
			/*this.HostPanel.clear();
			if (WriteRule_Step.isNull())
			{
				WriteRule_Step = this.HostPanel.add_Control<ascx_WriteRule>();
				WriteRule_Step.buildGui(this.AstData);
			}
			else
				this.HostPanel.add_Control(WriteRule_Step);
			//WriteRule_Step
			return WriteRule_Step;*/
		}				
		
		
		public Step_ViewFindings step_ViewFindings()
		{
			return  new Step_ViewFindings(this);			
		}
		
		public T show_Step<T>(T step)
			where T : Control
			
		{
			this.HostPanel.clear();
			if (step.isNull())
			{
				step = this.HostPanel.add_Control<T>();
				step.invoke("buildGui");
			}
			else
				this.HostPanel.add_Control(step);
			//WriteRule_Step
			return step;
		}
				
		 
		public void loadSourceFiles(string fileOrFolder)
		{			
			if (fileOrFolder.fileExists())			
				loadSourceFiles(fileOrFolder.wrapOnList());
			else if (fileOrFolder.dirExists())
			{
				statusMessage("Finding (recursively) all *.cs and *.vb to process in: {0}".format(fileOrFolder));
				loadSourceFiles(fileOrFolder.files("*.cs",true));
                loadSourceFiles(fileOrFolder.files("*.vb", true));		
			}
				
			//Step_1_TreeView_SourceFiles.clear();
			//Step_1_TreeView_SourceFiles.add_Nodes(AstData.files());
		}
		
		public void loadSourceFiles(List<string> filesToLoad)
		{			
			TopProgressBar.maximum(filesToLoad.size());
			int count = 0;
			int total = filesToLoad.size();
			var o2Timer = new O2Timer("{0} files processed in".format(total)).start();
			foreach(var file in filesToLoad)
			{				
				AstData.loadFile(file);
				TopProgressBar.increment(1);
				//statusMessage("loading source code files: {0}/{1} : {2}".format(count++, total, file.fileName()));
				statusMessage("loading source code files: {0}/{1}".format(count++, total));
			}
			//statusMessage(o2Timer.stop().TimeSpanString);
			o2Timer.stop().TimeSpanString.info();;
			//calculateMethodsStreams();
			//
		}				
		
		public void resetLoadedData()
		{
			if (AstData.notNull())
				AstData.Dispose();
				
			AstData = new O2MappedAstData();
			ReferencedAssemblies.Clear();// = new Dictionary<string, Assembly>();			
		}
		
		public void statusMessage(string message)
		{
			StatusLabel.set_Text(message);			
		}
	
	
		
		public class Step_LoadArtifacts
		{
			public O2_DotNet_Ast_Engine AstEngine { get; set; }
			public DataGridView Stats_DataGridView  { get; set; }
			public TreeView LoadFiles_TreeView { get; set; }
			public TreeView References_TreeView { get; set; }
			public TabPage LoadFilesTab { get; set; }
			public TabPage StatsTab { get; set; }
			public TabPage LoadedFilesTab { get; set; }
			public TabPage ReferencedAssembliesTab { get; set; }
			public TabPage OptionsTab { get; set; }
			public TabPage HelpTab { get; set; }						
			
			public string wikiHelpPage = "O2 DotNet Ast Engine - Load Artifacts";			
			
			public Step_LoadArtifacts(O2_DotNet_Ast_Engine astEngine)
			{
				AstEngine = astEngine;
				buildGui();
				loadDataInGui();
			}
			
			public void buildGui()
			{					
				AstEngine.HostPanel.clear();
				
				var tabControl = AstEngine.HostPanel.add_TabControl();
	
				LoadFilesTab = tabControl.add_Tab("Load Files to analyze");
				StatsTab = tabControl.add_Tab("Stats");
				LoadedFilesTab = tabControl.add_Tab("Loaded Files Source code");
				ReferencedAssembliesTab = tabControl.add_Tab("Referenced Assemblies");
				HelpTab = tabControl.add_Tab("Help and 'How does this works'");
				
				// loadFilesTab  	 
				
				LoadFilesTab.add_Label("To add files, drag and drop the files or folder to analyze in the treeView below", 10)
							.append_Link("reset all loaded data",
								()=>{
										AstEngine.resetLoadedData(); 
										loadDataInGui();
									});
									
				LoadFiles_TreeView = LoadFilesTab.add_TreeView();
				LoadFiles_TreeView.top(30)
				 				 .align_Left(LoadFilesTab)
				 				 .align_Right(LoadFilesTab) 
								 .align_Bottom(LoadFilesTab);
								 
				LoadFiles_TreeView.onDrop(
					(fileOrFolder)=>{
										AstEngine.loadSourceFiles(fileOrFolder);     
										loadDataInGui();
									});
				
				//statsTab 
				Stats_DataGridView = StatsTab.add_DataGridView();
				Stats_DataGridView.noSelection();
				
				
				// loadedFilesTab
				var sourceCode = LoadedFilesTab.add_SourceCodeViewer();
				var treeView = sourceCode.insert_Left<TreeView>(300); 
				treeView.add_Nodes(AstEngine.AstData.files())			
						.afterSelect<string>((file)=> sourceCode.open(file));
	
				// ReferencedAssembliesTab
				References_TreeView = ReferencedAssembliesTab.add_TreeView();
				References_TreeView.insert_Above<Panel>(20)					
								   .add_Label("Enter Referenced Assembly to add")
								   .top(3)
								   .append_TextBox("")
								   .top(0)
								   .align_Right(References_TreeView)
								   .onEnter((text)=> addReferenceAssembly(text));
				/*References_TreeView.insert_Above<TextBox>(20)
							       .onEnter((text)=> References_TreeView.add_Node(text))
							       .insert_Left<Label>(100);*/
				//OptionsTab
				//helpTab				
				//HelpTab.add_Browser()
				//	   .silent(true)
				//	   .add_WikiHelpPage(wikiHelpPage);				
			}
			
			public void addReferenceAssembly(string assembly)
			{				
				References_TreeView.backColor(Color.DarkGray);
				References_TreeView.enabled(false);
				O2Thread.mtaThread( 
					()=>{
							AstEngine.ReferencedAssemblies.Add(assembly,null);				
							"...loading reference assembly: {0}".debug(assembly);
							
							AstEngine.AstData.O2AstResolver.addReference(assembly);							
							"...load complete".debug();
							loadDataInGui();
							References_TreeView.backColor(Color.White);
							References_TreeView.enabled(true);
						});
			}
			
			public void loadDataInGui()
			{
				// Loaded Files
				LoadFiles_TreeView.clear();
				LoadFiles_TreeView.add_Nodes(AstEngine.AstData.files()); 
				
				// Stats
				Stats_DataGridView.clear();
				Stats_DataGridView.add_Columns("name","value"); 
				Stats_DataGridView.add_Row("> Files loaded:", AstEngine.AstData.files().size());
				Stats_DataGridView.add_Row("> INodes (total)", AstEngine.AstData.iNodes().size());
				Stats_DataGridView.add_Row("> ISpecials (total)", AstEngine.AstData.iSpecials().size());
				
				foreach(var item in AstEngine.AstData.iSpecials_By_Type())
					Stats_DataGridView.add_Row(item.Key,item.Value.size());
					
				var iNodesByType = AstEngine.AstData.iNodes_By_Type();
				foreach(var item in iNodesByType)  
					Stats_DataGridView.add_Row(item.Key,item.Value.size()); 
				
				//Referenced Assemblies
				References_TreeView.clear();
				References_TreeView.add_Nodes(AstEngine.ReferencedAssemblies.Keys);
			}

		}
		
					
		
		public class Step_SearchComments
		{
			public O2_DotNet_Ast_Engine AstEngine { get; set; }
			
			public TreeView CommentsTreeView  { get; set; }
			public ascx_SourceCodeViewer CodeViewer { get; set; }
			
			public String CommentsFilter { get; set; }
			
				
			public Step_SearchComments(O2_DotNet_Ast_Engine astEngine)
			{
				AstEngine = astEngine;
				CommentsFilter = "";				
				buildGui();
				loadDataInGui();
			}	
			
			public void buildGui()
			{
				AstEngine.HostPanel.clear();												

				var topPanel = AstEngine.HostPanel.add_1x1("Comment's Values", "Source Code", true, 400);								
								
				CodeViewer = topPanel[1].add_SourceCodeViewer(); 
				  
				CommentsTreeView = topPanel[0].add_TreeView()
											  .sort() 
											  .showSelection();
				CommentsTreeView.insert_Above<TextBox>(20).onTextChange_AlertOnRegExFail()
			   											  .onEnter((value)=>{
																				CommentsFilter= value;
																				loadDataInGui();
																			 });
				
				
				AstEngine.AstData.afterSelect_ShowInSourceCodeEditor(CommentsTreeView, CodeViewer.editor());  		   			
								
				CommentsTreeView.beforeExpand_PopulateWithList<ISpecial>();				
			}
			
			public void loadDataInGui()
			{
				var comments = AstEngine.AstData.comments_IndexedByTextValue(CommentsFilter); 
				CommentsTreeView.visible(false); 
				CommentsTreeView.clear();
				CommentsTreeView.add_Nodes(comments, -1, AstEngine.TopProgressBar); 
				CommentsTreeView.visible(true);
			}
		}
		
		
		public class Step_WhoCallsWho
		{
			public O2_DotNet_Ast_Engine AstEngine { get; set; }
			
			public TreeView MethodsTreeView  { get; set; }
			public TabControl ParentTabControl { get; set; }
			public TabPage TreeViewModeTab { get; set; }
			public TabPage GraphModeTab { get; set; }
			public TabPage InteractiveBrowseModeTab { get; set; }
			
			public Dictionary<string,List<String>> MethodsCalledMappings { get; set; }
			public Dictionary<string,List<String>> MethodIsCalledByMappings { get; set; }
			public List<string> AllMethods { get; set; }			
			public String FileFilter { get; set; }
				
			public Step_WhoCallsWho(O2_DotNet_Ast_Engine astEngine)
			{
				AstEngine = astEngine;
				//CommentsFilter = "";				
				buildGui();
				//loadDataInGui();
			}				
			
			public void buildGui()
			{
				AstEngine.HostPanel.clear();															
				"Calculating Mappings fro Methods called".info();
				MethodsCalledMappings = AstEngine.AstData.calculateMappingsFor_MethodsCalled();			
				MethodIsCalledByMappings = AstEngine.AstData.calculateMappingsFor_MethodIsCalledBy(MethodsCalledMappings);			
				
				AllMethods = this.MethodsCalledMappings.Keys.toList();
				AllMethods.add_OnlyNewItems(this.MethodIsCalledByMappings.Keys.toList());
				"MethodsCalledMappings has {0} root methods".info(MethodsCalledMappings.Keys.size());
				"MethodIsCalledByMappings has {0} root methods".info(MethodIsCalledByMappings.Keys.size());
				"AllMethods has {0} root methods".info(AllMethods.size());
				
				ParentTabControl = AstEngine.HostPanel.add_TabControl();
				TreeViewModeTab = ParentTabControl.add_Tab("TreeView Mode");
				GraphModeTab = ParentTabControl.add_Tab("Graph Mode");
				InteractiveBrowseModeTab = ParentTabControl.add_Tab("Interactive Browse Mode");
				build_TreeViewMode(TreeViewModeTab);
				build_GraphMode(GraphModeTab);
			}
						
			
			public void build_TreeViewMode(Control control)
			{			
				var sourceViewer = control.add_SourceCodeViewer();
				var controls = sourceViewer.insert_Above<Panel>().add_1x1("Methods called", "Methods called by", true, AstEngine.HostPanel.width()/2);
				
				var methodsCalledTreeView = controls[0].add_TreeViewWithFilter(MethodsCalledMappings);			
				var methodIsCalledByTreeView = controls[1].add_TreeViewWithFilter(MethodIsCalledByMappings);
				
				methodsCalledTreeView.afterSelect_ShowMethodSignatureInSourceCode(AstEngine.AstData, sourceViewer);
				methodIsCalledByTreeView.afterSelect_ShowMethodSignatureInSourceCode(AstEngine.AstData, sourceViewer);
			}
			
			public void build_GraphMode(Control control)
			{
				var graphMethodCalls =  control.add_Control<ascx_GraphAst_MethodCalls>();
				graphMethodCalls.setData(AstEngine.AstData, MethodsCalledMappings, MethodIsCalledByMappings, AllMethods);
				graphMethodCalls.buildGui();
			}
			
			/*public void build_InteractiveMode(Control control)
			{
				var interctiveCalls =  control.add_Control<ascx_Interactive_MethodCalls>();
				interctiveCalls.setData(AstEngine.AstData, MethodsCalledMappings, MethodIsCalledByMappings, AllMethods);
				interctiveCalls.buildGui();
			}*/
			
		}
				
		
		
		
		public class Step_MethodStreams
		{
			public O2_DotNet_Ast_Engine AstEngine { get; set; }
			
			
			public O2MappedAstData AstData_MethodStream { get; set; }
			public O2MethodStream MethodStream { get; set; }
			public O2CodeStream CodeStream { get; set; }			
			public O2CodeStreamTaintRules TaintRules { get; set; }
			public String MethodStreamFile { get; set; }
			public bool ShowGraphInNewWindow {get; set;}
			public bool JoinGraphData {get; set;}
			
			
			public TreeView MethodsTreeView  { get; set; }
			public TreeView ParametersTreeView  { get; set; }
			public TreeView MethodsCalledTreeView { get; set; }
			public ascx_SourceCodeViewer CodeViewer { get; set; }			
			public GraphLayout CodeStreamGraph { get; set; }
			public ascx_Simple_Script_Editor CodeStreamGraphScript { get; set; }			
			public Panel CodeStreamGraphPanel { get; set; }
			public TreeView CodeStreamTreeView { get; set; }	
			public TabPage CodeStreamGraphTab { get; set; }
			public TabPage CodeStreamTreeViewTab { get; set; }
			public String MethodsFilter { get; set; }
			public ascx_SourceCodeViewer CodeStreamCodeViewer  { get; set; }	
			
			public Step_MethodStreams(O2_DotNet_Ast_Engine astEngine)
			{
				AstEngine = astEngine;
				MethodsFilter = "";		
				AstData_MethodStream = new O2MappedAstData();
				
				//CodeStream = new O2CodeStream();
				buildGui();
				loadDataInGui();
				
				TaintRules = new O2CodeStreamTaintRules(); 
				TaintRules.add_TaintPropagator("System.String.Concat");
				//taintRules.add_TaintPropagator("System.String");
			}	
			
			public void buildGui()
			{
				AstEngine.HostPanel.clear();												

				var topPanel = AstEngine.HostPanel.add_1x1("Methods & Parameters", "Source Code", true, 400);								
								
				//CodeViewer = topPanel[1].add_SourceCodeViewer(); 
				
				var tabControl = topPanel[1].add_TabControl(); 
				CodeViewer = tabControl.add_Tab("Source Code").add_SourceCodeViewer();
				
				CodeStreamTreeViewTab = tabControl.add_Tab("CodeStream TreeView");
				CodeStreamGraphTab = tabControl.add_Tab("CodeStream Graph");							
				CodeStreamCodeViewer = CodeStreamTreeViewTab.add_SourceCodeViewer();
				CodeStreamTreeView = CodeStreamCodeViewer.insert_Left<TreeView>(200);
				CodeStreamGraphPanel = CodeStreamGraphTab.add_Panel().backColor(Color.White);					
				CodeStreamGraph = CodeStreamGraphPanel.add_Graph();
				CodeStreamGraphScript = CodeStreamGraphPanel.insert_Below<Panel>().add_Script();
				CodeStreamTreeView.afterSelect<O2CodeStreamNode>
				 	((streamNode)=> CodeStreamCodeViewer.editor().setSelectionText(streamNode.INode.StartLocation, streamNode.INode.EndLocation));				
				
				MethodsTreeView = topPanel[0].add_TreeView()
											  .sort() 
											  .showSelection();
											  
				MethodsTreeView.insert_Above<TextBox>(20).onTextChange_AlertOnRegExFail()
			   											 .onEnter((value)=>{
																				MethodsFilter= value;
																				loadDataInGui();
																			});
				MethodsTreeView.afterSelect<IMethod>(
					(iMethod)=>
						{
							createMethodStreamAndShowInGui(iMethod);						
						});				
				
				var optionsPanel = MethodsTreeView.insert_Below<Panel>(25);
				optionsPanel.add_CheckBox("Open Graph in New Window", 0, 0,(value)=> ShowGraphInNewWindow = value)
							.autoSize();
							
				optionsPanel.add_CheckBox("Join Graph Data",0, 200,(value)=> JoinGraphData = value)
							.autoSize().bringToFront();							   
							   				
				ParametersTreeView =  MethodsTreeView.insert_Below<TreeView>(100);
				MethodsCalledTreeView =  ParametersTreeView.insert_Right<TreeView>(200);
				
				AstData_MethodStream.afterSelect_ShowInSourceCodeEditor(MethodsCalledTreeView, CodeViewer.editor()); 
				AstData_MethodStream.afterSelect_ShowInSourceCodeEditor(ParametersTreeView, CodeViewer.editor());
																
				MethodsCalledTreeView.afterSelect<INode>((iNode)=> createAndShowCodeStream(iNode));
				ParametersTreeView.afterSelect<INode>((iNode)=> createAndShowCodeStream(iNode));
				
				MethodsTreeView.beforeExpand_PopulateWithList<ISpecial>();	
								
				
			}
			
			public void loadDataInGui()
			{
				var iMethods = AstEngine.AstData.iMethods();
				
				MethodsTreeView.visible(false); 
				MethodsTreeView.clear();
								
				foreach(var iMethod in iMethods)
				{	
					var fullName = iMethod.fullName();
					var name = iMethod.name();
					if (MethodsFilter.valid().isFalse() || name.regEx(MethodsFilter))
						MethodsTreeView.add_Node(name,iMethod).toolTip(fullName);
				}
				//MethodsTreeView.showToolTip();
				MethodsTreeView.visible(true);				
			}
			
			
			public void createMethodStreamAndShowInGui(IMethod iMethod)
			{
				O2Thread.mtaThread(
					()=>{
							createMethodStream(iMethod);
							ParametersTreeView.clear();
							MethodsCalledTreeView.clear();
							AstData_MethodStream.showMethodStreamDetailsInTreeViews(ParametersTreeView, MethodsCalledTreeView);
						});
			}
			
			public void createMethodStream(IMethod iMethod)
			{				
				MethodStream = AstEngine.AstData.createO2MethodStream(iMethod);
				MethodStreamFile = MethodStream.csharpCode().saveWithExtension(".cs");
							
				CodeViewer.open(MethodStreamFile);
				CodeStreamCodeViewer.open(MethodStreamFile);								
				if (AstData_MethodStream.notNull())
					AstData_MethodStream.Dispose();
				AstData_MethodStream = new O2MappedAstData(); 							
				AstData_MethodStream.loadFile(MethodStreamFile);								
			}
			
			
			
			public void createAndShowCodeStream(INode iNode)
			{
				O2Thread.mtaThread(
					()=>{
							CodeStream = AstData_MethodStream.createO2CodeStream(TaintRules, MethodStreamFile,iNode);
							showCodeStream();
						});
			}
			
			public void showCodeStream()
			{							
				CodeStream.show(CodeViewer.editor());
				CodeStream.show(CodeStreamCodeViewer.editor());
				CodeStream.show(CodeStreamTreeView);
				
				
				O2Thread.mtaThread(
					()=>{							
							if (ShowGraphInNewWindow)
							{		
								if (JoinGraphData.isFalse())
									CodeStreamGraph = O2Gui.open<Panel>("O2 Ast Engine Graph",400,400).add_Graph();
								CodeStream.show(CodeStreamGraph);
							}
							else
							{
								if (JoinGraphData.isFalse())
								{
									CodeStreamGraphPanel.clear();						
									CodeStreamGraph = CodeStreamGraphPanel.add_Graph();
								}
								CodeStream.show(CodeStreamGraph);
							}							
							CodeStreamGraphScript.InvocationParameters.add("graph",CodeStreamGraph);
						});
								
			}
		}				
				
				
		public class Step_ViewFindings
		{
			public O2_DotNet_Ast_Engine AstEngine { get; set; }
						
			public ascx_FindingsViewer FindingsViewer { get; set; }
				
			public Step_ViewFindings(O2_DotNet_Ast_Engine astEngine)
			{
				AstEngine = astEngine;				
				buildGui();
				loadDataInGui();
			}	
			
			public void buildGui()
			{
				AstEngine.HostPanel.clear();												
				FindingsViewer = AstEngine.HostPanel.add_FindingsViewer();
			}
			
			public void loadDataInGui()
			{
				FindingsViewer.show(AstEngine.WriteRule_Step.FinalFindingsViewer.o2Findings());
			}
		}
	}
}
