// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Drawing;
using System.Windows.Forms;
using FluentSharp.CSharpAST;
using FluentSharp.CSharpAST.Utils;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL;
using FluentSharp.REPL.Controls;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.NRefactory.Ast;

//O2File:Ast_Engine_ExtensionMethods.cs
//O2File:SharpDevelop_O2MappedAstData_ExtensionMethods.cs
//O2File:TextEditor_O2CodeStream_ExtensionMethods.cs
//O2Ref:QuickGraph.dll
//O2Ref:FluentSharp.Ast.dll

namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
	public class test_ascx_ViewAST
	{		
		public void launchGui()
		{
			var astData = new O2MappedAstData();
			
			astData.loadFile("HacmeBank_v2_Website.ascx.PostMessageForm.btnPostMessage_Click.cs".local());
			
			var control = O2Gui.open<Panel>("test ascx_ViewAST",1000,600);
			var viewAST = control.add_Control<ascx_ViewAST>();
			viewAST.buildGui(astData);
			
			
		}
	}

	public class ascx_ViewAST : Control
	{
		public O2MappedAstData AstData {get;set;}	
		public ascx_SourceCodeViewer CodeViewer { get; set; }
		public TreeView DataTreeView { get; set; }
		public Panel Options { get; set; }
					
		public bool Show_Ast { get; set; }
		public bool Show_CodeDom { get; set; }
		public bool Show_NRefactory { get; set; }			
		

		
		public ascx_ViewAST()//astData astEngine)
		{		
			//iNodesByType = new  Dictionary<string,List<INode>>();
		}	
		
		public void buildGui(O2MappedAstData astData) 
		{
			AstData = astData;
			
			buildGui();
			loadDataInGui();
		}			
			
			
			
		public void buildGui()
		{		
			var topPanel = this;
			CodeViewer = topPanel.add_SourceCodeViewer();   
			DataTreeView = CodeViewer.insert_Left<TreeView>(200).showSelection().sort();     
			Options = DataTreeView.insert_Below<Panel>(40); 
			Options.add_CheckBox("View AST",0,0,(value)=> { this.Show_Ast = value;}).check();
			Options.add_CheckBox("View CodeDom",0,95,(value)=> {this.Show_CodeDom = value; }).front();
			Options.add_CheckBox("View NRefactory",20,0,(value)=> {this.Show_NRefactory = value;}).front().autoSize();

			DataTreeView.showSelection();	
			DataTreeView.configureTreeViewForCodeDomViewAndNRefactoryDom();
			AstData.afterSelect_ShowInSourceCodeEditor(DataTreeView, CodeViewer.editor());  

			DataTreeView.onDrop(
				(fileOrFolder)=>{
									DataTreeView.backColor(Color.LightPink);
									O2Thread.mtaThread(
										()=>{
												AstData.dispose();
												AstData = new O2MappedAstData();
												if (fileOrFolder.fileExists())
													AstData.loadFile(fileOrFolder);
												else
													AstData.loadFiles(fileOrFolder.files("*.cs",true));
												loadDataInGui();
												DataTreeView.backColor(Color.White);
											 });									
								});
			DataTreeView.afterSelect<string>(
				(file)=>{
						if (file.fileExists())
							CodeViewer.open(file);
						});
			
			
			DataTreeView.beforeExpand<CompilationUnit>(
				(compilationUnit)=>{																	
										var treeNode = DataTreeView.selected();																									
										treeNode.clear();	           
																			
										if (Show_Ast)
										{										
											if (compilationUnit!=null) 
												treeNode.add_Node("AST",null)
		            									.show_Ast(compilationUnit)
		            									.show_Asts(compilationUnit.types(true))
		            									.show_Asts(compilationUnit.methods());
							                		//treeNode.show_Ast(compilationUnit);
							             }
							        
							            if (Show_CodeDom)
							            {
								            var codeNamespace = AstData.MapAstToDom.CompilationUnitToNameSpaces[compilationUnit];
							            	var domNode = treeNode.add_Node("CodeDom");
	            							domNode.add_Node("CodeNamespaces").show_CodeDom(codeNamespace);
											domNode.add_Node("CodeTypeDeclarations").show_CodeDom(AstData.codeTypeDeclarations());
	            							domNode.add_Node("CodeMemberMethods").show_CodeDom(AstData.codeMemberMethods());
	            							//domNode.add_Node("CodeMemberMethods").show_CodeDom(o2MappedAstData.codeMemberMethods());
							            }
							            if (Show_NRefactory)
							            {
							            	var iCompilationUnit = AstData.MapAstToNRefactory.CompilationUnitToICompilationUnit[compilationUnit];
							            	treeNode.add_Node("NRefactory")
							            		    .add_Nodes_WithPropertiesAsChildNodes<ICompilationUnit>(iCompilationUnit);
	                                           //.show_NRefactoryDom(o2MappedAstData.iClasses())
	                                           //.show_NRefactoryDom(o2MappedAstData.iMethods());
	
							            }
							
					    });				

		}
			
		public void loadDataInGui()
		{
			DataTreeView.clear();
			foreach(var file in AstData.files())
				DataTreeView.add_Node(file.fileName(), AstData.compilationUnit(file),true);
			if (DataTreeView.nodes().size()>0)
				DataTreeView.nodes()[0].expand();
		}								
	}
}
