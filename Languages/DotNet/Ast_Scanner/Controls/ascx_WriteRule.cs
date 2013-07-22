// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Windows.Forms;
using System.Collections.Generic;
using FluentSharp.CSharpAST.Utils;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL;
using FluentSharp.REPL.Controls;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;

//O2File:Ast_Engine_ExtensionMethods.cs
//O2File:SharpDevelop_O2MappedAstData_ExtensionMethods.cs


namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
	public class test_ascx_WriteRule
	{
		public void launchGui()
		{
			var astData = new O2MappedAstData();
			astData.loadFiles(@"C:\O2\DemoData\HacmeBank_v2.0 (Dinis version - 7 Dec 08)\HacmeBank_v2_WS\classes".files());
			
			var control = O2Gui.open<Panel>("test ascx_WriteRule",700,500);
			var writeRule = control.add_Control<ascx_WriteRule>();
			
			writeRule.buildGui(astData); 			
		}
	}
	
    public class ascx_WriteRule : Control
    {        
		//public Control HostPanel;
		public O2MappedAstData AstData {get;set;}
		public ascx_Simple_Script_Editor MethodStreamScript { get; set; }
		public ascx_Simple_Script_Editor CodeStreamScript { get; set; }
		public ascx_Simple_Script_Editor FindingsScript { get; set; }
		
		public TreeView MethodStreamViewer { get; set; }
		public TreeView CodeStreamViewer { get; set; }
		
		public ascx_FindingsViewer RawFindingsViewer { get; set; }
		public ascx_FindingsViewer FinalFindingsViewer { get; set; }
		/*public TreeView CommentsTreeView  { get; set; }
		public ascx_SourceCodeViewer CodeViewer { get; set; }
		
		public String CommentsFilter { get; set; }
		*/
		public ascx_WriteRule()
		{
		}
		
		public ascx_WriteRule(O2MappedAstData astData) //O2_DotNet_Ast_Engine astEngine)
		{
			buildGui(astData);
		}	
		
		public void buildGui(O2MappedAstData astData)
		{
			//HostPanel = hostPanel;
			AstData = astData;			
			buildGui();
			//loadDataInGui();
		}
		
		public void buildGui()
		{						
			//var controls = AstEngine.HostPanel.add_1x1x1();     						
			var tabControl = this.add_TabControl();
			var controls = new List<TabPage>();
			controls.Add(tabControl.add_Tab("Step 1: Create All Method Streams"));
			build_CreateAllMethodStreamsTab(controls[0]);
		}
		
		public void build_CreateAllMethodStreamsTab(Control hostControl)
		{	
			var allMethodStreamsPanel = hostControl.add_1x1("Options","All Method Streams",false,50); 
			
			var sourceCode = allMethodStreamsPanel[1].add_SourceCodeViewer();//add_MethodStreamViewer();
			var treeView = sourceCode.insert_Left<TreeView>(300)
									 .showSelection()
									 .sort()
									 .afterSelect<string>(
									 	(filePath)=>sourceCode.open(filePath));
			
			var targerFolder = PublicDI.config.getTempFolderInTempDirectory("_AllMethodStreams");
			allMethodStreamsPanel[0].parent<SplitContainer>().borderNone();
			var optionsPanel = allMethodStreamsPanel[0].parent().clear();
			var progressBar = optionsPanel.add_ProgressBar(25,0).align_Right(optionsPanel);
			optionsPanel.add_LabelAndTextAndButton("Target Directory", targerFolder,"Create",
				(text)=>{
							AstData.createAllMethodsStreams(text, progressBar, null);
							foreach(var file in targerFolder.files())
								treeView.add_Node(file.fileName(), file);
							treeView.selectFirst();
						});
			
			progressBar.onDrop(
				(fileOrFolder)=>{
									if (fileOrFolder.fileExists())
										AstData.loadFile(fileOrFolder);
									else
									{
										AstData.dispose();
										AstData = new O2MappedAstData();									
										AstData.loadFiles(fileOrFolder.files("*.cs",true));
									}
								});
			//tabPages.Add(tabControl.add_Tab("Step 2: Create Code Streams"));
			//tabPages.Add(tabControl.add_Tab("Step 1: Create Final Findings"));
			
			
			//Create all Method Streams
			
			
			//var controls = tabPages;
			//var MethodStreamPanel  = controls[0].add_1x1("Current ","MethodStreams", false);
			
			/*
			var CodeStreamPanel = controls[1].add_1x1("Create CodeStreams", "CodeStreams", false); 
			var FindingsPanel = controls[2].add_1x1("Create Findings", "Final Findings",false);
			
			// MethodStreamPanel  
			MethodStreamScript = MethodStreamPanel[0].add_Script(false);   
			MethodStreamViewer = MethodStreamPanel[1].add_MethodStreamViewer();	
							
			//CodeStreamPanel    
			CodeStreamScript = CodeStreamPanel[0].add_Script(false);  
			var CodeStreamScriptResult = CodeStreamPanel[1].add_1x1("Code Streams", "Raw Findings", true, CodeStreamScript.width()/2);
			
			CodeStreamViewer =  CodeStreamScriptResult[0].add_CodeStreamViewer();   
			RawFindingsViewer = CodeStreamScriptResult[1].add_FindingsViewer();
			
			
			//RawFindingsViewer = FindingsPanel[1].add_FindingsViewer();				
			//CodeStreamViewer = MethodStreamPanel[1].add_CodeStreamViewer();   
			
			
			// FindingsPanel
			FindingsScript = FindingsPanel[0].add_Script(false);				
			FinalFindingsViewer = FindingsPanel[1].add_FindingsViewer();
			
			//var controls2 = host[1].add_1x1x1(true);  

			// extra vars
							
			// script parameters				
			var scriptParameters = new Dictionary<string,object>(); 				
			scriptParameters.Add("methodStreamViewer", MethodStreamViewer);  
			scriptParameters.Add("codeStreamViewer", CodeStreamViewer); 
			
			scriptParameters.Add("rawFindingsViewer", RawFindingsViewer); 
			scriptParameters.Add("finalFindingsViewer", FinalFindingsViewer); 
			
			scriptParameters.Add("astData", AstData); 				
			
			MethodStreamScript.InvocationParameters.AddRange(scriptParameters);				
			CodeStreamScript.InvocationParameters.AddRange(scriptParameters);
			FindingsScript.InvocationParameters.AddRange(scriptParameters);

			*/
		}
				
		
		
		/*
		public void loadDataInGui()
		{
			var defaultUsingAndFileRef		 = "";//"//using ICSharpCode.NRefactory".line() + 
												//"//using ICSharpCode.NRefactory.Ast".line() + 
												//"//using ICSharpCode.SharpDevelop.Dom".line() + 
												//@"//O2File:C:\O2\_XRules_Local\Extra_methods.cs".line();
												
			var scriptFor_MethodStreamScript = 	"methodStreamViewer.clear();".line() + 
											   	"var iMethods = new List<IMethod>();".line() + 

											   	"foreach(var attribute in astData.attributes())".line() + 			
													"if (attribute.name() == \"WebMethod\")".line() + 
													"{".line() +  			
														"var methodDeclaration = attribute.parent<MethodDeclaration>();".line() + 															
														"if (methodDeclaration.parameters().size() > 0)".line() + 
															"iMethods.Add(astData.iMethod(methodDeclaration));".line() + 
													"}".line() +  

												"astData.showMethodStreams(iMethods, methodStreamViewer);".line() + 
												"".line() + 
												defaultUsingAndFileRef;
												
			var scriptFor_CodeStreamScript = 	"codeStreamViewer.clear();".line() +
												"var iMethods = (List<IMethod>)methodStreamViewer.Tag;".line() +
												"".line() +
												"var o2Findings = astData.createAndShowCodeStreams(iMethods,codeStreamViewer);".line() +
												
												"rawFindingsViewer.show(o2Findings);".line() + 
												defaultUsingAndFileRef;
												
			var scriptFor_FindingsScript = 		"var o2Findings = rawFindingsViewer.o2Findings();".line() + 
												"var filteredFindings = o2Findings.filter_SinkStartsWith(\"constructor: public System.Data.SqlClient.SqlCommand.SqlCommand\");".line() + 
												"".line() + 
												"filteredFindings.set_VulnName(\"System.Data.SqlClient.SqlCommand.SqlCommand\");".line() + 
												"filteredFindings.set_VulnType(\"Sql Injection\");".line() + 
												"".line() + 
												"finalFindingsViewer.show(filteredFindings);".line() + 													
												//"return finalFindingsViewer.save();".line() + 
												defaultUsingAndFileRef;
											
			MethodStreamScript.set_Command(scriptFor_MethodStreamScript);
			CodeStreamScript.set_Command(scriptFor_CodeStreamScript);
			FindingsScript.set_Command(scriptFor_FindingsScript);
			//FindingsScript.compile();
			//CodeStreamScript.compile();
		}*/
	}    	    	    	    	        
}
