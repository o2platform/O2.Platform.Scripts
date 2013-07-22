// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FluentSharp.CSharpAST;
using FluentSharp.CSharpAST.Utils;
using FluentSharp.CoreLib;
using FluentSharp.REPL;
using FluentSharp.WinForms;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;

//O2File:Ast_Engine_ExtensionMethods.cs
//O2File:MethodMappings_ExtensionMethods.cs

//O2Ref:QuickGraph.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:System.Xml.dll

namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
    public class ascx_Interactive_MethodCalls : System.Windows.Forms.Control
    {        	
		/*public List<Forms.Control> GraphModeTopPanels {get; set;}
		public List<Forms.Control> GraphModeRightPanels {get; set;}
		//public ascx_SourceCodeViewer CodeViewer { get; set; }
		public GraphLayout GraphViewer { get; set; }
		public Forms.Panel GraphOptionsPanel { get; set; }
		*/
		public Dictionary<string,List<String>> MethodsCalledMappings { get; set; }
		public Dictionary<string,List<String>> MethodIsCalledByMappings { get; set; }
		public List<string> AllMethods { get; set; }			
		public String FileFilter { get; set; }
		
		/*public ascx_Simple_Script_Editor GraphScript { get; set; }
		public Dictionary<string, WPF.Control> GraphNodes { get; set;}
		
		public bool UseStarAsNodeText { get; set; }
		public bool AutoExpandCalledMethods { get; set; }
		public bool AutoExpandMethodsCalledBy { get; set; }
		public bool ExpandMethodsCalled {get; set;}
		public bool ExpandMethodIsCalledBy {get; set;}
		public bool ClearGraphOnMethodView { get; set; }
		*/
		public O2MappedAstData AstData {get;set;}
		
		public ascx_Interactive_MethodCalls()
		{
			AstData = new O2MappedAstData();
			//GraphNodes = new Dictionary<string, WPF.Control>();
		}
		
		public ascx_Interactive_MethodCalls setData(O2MappedAstData astData, 
												 Dictionary<string,List<String>> methodsCalledMappings, 
												 Dictionary<string,List<String>> methodIsCalledByMappings,
												 List<string> allMethods )
		{
			AstData = astData;			
			MethodsCalledMappings = methodsCalledMappings;
			MethodIsCalledByMappings = methodIsCalledByMappings;
			AllMethods = allMethods;
			
			return this;
		}
		
        public void buildGui()
		{								
			string CurrentSourceCodeFile = "";
			var topPanel = this.add_1x1x1("Files", "Selected File Source Code", "Selected AST Node details");								
			
			var CodeViewer = topPanel[1].add_SourceCodeViewer(); 
			   
			var FilesTreeView = topPanel[0].add_TreeView()
										  .sort() 
										  .showSelection();
			FilesTreeView.insert_Above<TextBox>(20).onTextChange_AlertOnRegExFail()
				   								   .onEnter((value)=>{
																		this.FileFilter = value;
																		this.loadDataInGui();
				 													 });
			//var SelectedINodePanel = topPanel[2].add_Panel();													 
			var SelectedINodeTabControl = topPanel[2].add_TabControl();  
			var SelectedINodeAstMethodCallTreeView = SelectedINodeTabControl.add_Tab("Selected Method call trees").add_TreeView();
			var SelectedINodeAstTreeView = SelectedINodeTabControl.add_Tab("Ast TreeView").add_TreeView();
			SelectedINodeAstTreeView.configureTreeViewForCodeDomViewAndNRefactoryDom();
			SelectedINodeAstTreeView.afterSelect_ShowAstInSourceCodeEditor(CodeViewer.editor()); 
			
			//Events
			
			FilesTreeView.afterSelect<string>(
				(file)=>{
							CurrentSourceCodeFile = file; 
							CodeViewer.open(file);
						});
						
			
			CodeViewer.onClick(
				()=>{
							var iNode = AstData.iNode(CurrentSourceCodeFile,CodeViewer.editor().caret());  
							if (iNode != null)
							{					
								topPanel[2].set_Text("Current INode: {0}".format(iNode.typeName()));
								
								//color current source code viewer with INode region
								CodeViewer.editor().clearBookmarksAndMarkers();
								CodeViewer.editor().selectTextWithColor(iNode);  
								IMethod resolvedIMethod = null;
								// Show INode Ast
								//var astToShow = new Dictionary<string, List<INode>>();
								//astToShow.add(iNode.typeName(),iNode);
								SelectedINodeAstTreeView.clear();
								SelectedINodeAstTreeView.add_Node("AST").show_Ast(iNode);   
								if (iNode is MethodDeclaration)
								{
									resolvedIMethod = AstData.iMethod(iNode as MethodDeclaration);
									if (resolvedIMethod != null && resolvedIMethod.FullyQualifiedName.valid())
										SelectedINodeAstTreeView.add_Node("Method: " + resolvedIMethod.FullyQualifiedName,resolvedIMethod,true); 
								}					
								else if (iNode is MemberReferenceExpression)
								{
									resolvedIMethod = AstData.fromMemberReferenceExpressionGetIMethod(iNode as MemberReferenceExpression);
									if (resolvedIMethod != null && resolvedIMethod.FullyQualifiedName.valid())
										SelectedINodeAstTreeView.add_Node("MethodRef: " + resolvedIMethod.FullyQualifiedName,resolvedIMethod,true); 
								}
								else if (iNode is Expression) 
								{
									var iMethodExpressionRef = AstData.fromExpressionGetIMethod(iNode as Expression);
									if (iMethodExpressionRef != null)
									{
										SelectedINodeAstTreeView.add_Node("MethodRef: " + iMethodExpressionRef.FullyQualifiedName,iMethodExpressionRef,true); 
									}
									else
									{
										var resolved = AstData.resolveExpression(iNode as Expression); 
										if (resolved != null & resolved.ResolvedType != null && resolved.ResolvedType.str().valid())
											SelectedINodeAstTreeView.add_Node("Resolved Type : {0} ".format(resolved.ResolvedType.FullyQualifiedName));
										else
											"resolved was null".error();
									} 
								}
								if (resolvedIMethod!= null)
								{
									SelectedINodeAstMethodCallTreeView.clear();
									
									var methodNode = SelectedINodeAstMethodCallTreeView.add_Node(resolvedIMethod.fullName()); 						
									methodNode.add_Node("Methods that call this Method", resolvedIMethod,true);
									//SelectedINodeTabControl.select_Tab(SelectedINodeAstMethodCallTab); 
								}					
														
									
									
								
									/*var calledIMethodsNode = methodsCallTreeView.add_Node("Called Methods");
									foreach(var calledIMethod in AstEngine.AstData.calledIMethods(resolvedIMethod))
									{
										if (calledIMethod != null) 
											calledIMethodsNode.add_Node(calledIMethod.fullName(), calledIMethod, true); 
									}*/										
							} 
						 });
						 
			SelectedINodeAstMethodCallTreeView.beforeExpand<IMethod>
				((treeNode, iMethod)=>{
								
									//var methodsCallTreeView = SelectedINodeAstMethodCallTab.add_TreeView();
									//methodsCallTreeView.configureTreeViewForCodeDomViewAndNRefactoryDom();	 																		
										foreach(var methodThatCallIMethod in AstData.iMethodsThatCallThisIMethod(iMethod))
										{
											if (methodThatCallIMethod != null) 
											{								
												treeNode.add_Node(methodThatCallIMethod.fullName(), methodThatCallIMethod, true); 
											}
										}
									
									 });
			 
			//load data in Gui 
			  
			FilesTreeView.add_Nodes(AstData.files());   
			FilesTreeView.selectFirst();
			CodeViewer.editor().caret(28,30).focus();   
    	}	    	    	    	    
    	
    	public void loadDataInGui()
		{				
			
			//CommentsTreeView.visible(false); 
			//CommentsTreeView.clear();
			//MethodsTreeView.add_Nodes(AstEngine.AstData.iMethods());
			//CommentsTreeView.visible(true);
		}
    }
}
