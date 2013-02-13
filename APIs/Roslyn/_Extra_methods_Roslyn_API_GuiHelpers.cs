// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq; 
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.XRules.Database.Utils;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Compilers;
using Roslyn.Services;

//O2File:_Extra_methods_Roslyn_API.cs

//O2Ref:O2_FluentSharp_Roslyn.dll

//O2Ref:Roslyn.Services.dll
//O2Ref:Roslyn.Services.CSharp.dll 
//O2Ref:Roslyn.Compilers.dll
//O2Ref:Roslyn.Compilers.CSharp.dll
//O2Ref:Roslyn.Utilities.dll

//_O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs
//O2Ref:WeifenLuo.WinFormsUI.Docking.dll

namespace O2.XRules.Database.APIs
{	
					
	public static class _Extra_methods_Roslyn_API_GuiHelpers
	{
	
		public static Panel view_Ast(this SyntaxNode syntaxNode, bool showCode = true)
		{							
			return syntaxNode.str().view_Ast(showCode);  			
		}
		public static Panel view_Ast(this string codeToView, bool showCode = true)
		{				
			var popupPanel = "Ast Viewer".popupWindow();
			return codeToView.view_Ast(popupPanel,showCode);  			
		}				
		
		public static T view_Ast<T>(this string codeToView, T targetControl,bool showCode = true)
			where T : Control
		{
			return targetControl.add_AstViewer(codeToView,showCode);
		}
		
		public static Panel view_Ast(this SyntaxTree syntaxTree, bool showCode = true)
		{				
			var popupPanel = "Ast Viewer".popupWindow();
			return syntaxTree.view_Ast(popupPanel,showCode);  			
		}								
		
		public static T view_Ast<T>(this  SyntaxTree syntaxTree, T targetControl,bool showCode = true)
			where T : Control
		{
			return targetControl.add_AstViewer(syntaxTree,showCode);
		}
		
		public static T add_AstViewer<T>(this T targetControl, SyntaxTree syntaxTree, bool showCode = true)
			where T : Control
		{
			return targetControl.add_AstViewer(syntaxTree.str(), showCode);
		}
		
		public static T add_AstViewer<T>(this T targetControl, string codeToView, bool showCode = true)
			where T : Control
		{			
			var topPanel = targetControl.clear().add_Panel();
			var codeViewer = topPanel.title("C# code ").add_SourceCodeViewer().set_ColorsForCSharp();
			var treeView = topPanel.insert_Below("Ast Tree of C#").add_TreeView();
			var actions = treeView.insert_Below(45,"Actions");
			if (showCode.isFalse())
				codeViewer.splitContainer().panel1Collapsed(true);							
			var autoExpandAll = false;
			var parseAsScript = false;
			
			Action expandAll = ()=>
				{			
					treeView.expandAll();
				};
							 			
			treeView.afterSelect<SyntaxNode>(
				(syntaxNode)=>{			 		
								if ((syntaxNode is CompilationUnitSyntax).isFalse())
									codeViewer.selectText(syntaxNode.Span.Start, syntaxNode.Span.End);															
							  });
							  
			//recursive method to add all ChildNodes				  
			Action<TreeNode, SyntaxNode> add_SyntaxNode = null;
			add_SyntaxNode = 
				(treeNode,syntaxNode)=> {
											var newTreeNode = treeNode.add_Node(syntaxNode.Kind.str(),syntaxNode);
											foreach(var childSyntaxNode in syntaxNode.ChildNodes())
												add_SyntaxNode(newTreeNode, childSyntaxNode);								
									 	};
			Action<string> viewAstTree = 
				(code)=>{										
							var tree = (parseAsScript) ?  code.ast_Script() : code.ast();				
							var root = tree.GetRoot();
							treeView.update(()=>
								{
									treeView.clear();
									add_SyntaxNode(treeView.rootNode(), root);
									treeView.expand()
											.selectFirst();
								});
							if (autoExpandAll)
								expandAll();										 
						};
						
			Action refresh = 
				()=> viewAstTree(codeViewer.get_Text());
				
			codeViewer.onTextChange((text)=> viewAstTree(text));
			actions.add_Link("expand all", ()=> expandAll() )
				   .append_Link("colapse all", ()=> treeView.CollapseAll() )
				   .append_CheckBox("auto expand all", (value) => { autoExpandAll = value; if (value) refresh(); })
				   .append_CheckBox("parse code as script", (value) => { parseAsScript = value; refresh();}).check();
			
			treeView.onDoubleClick<SyntaxNode>((syntaxNode)=>syntaxNode.showInfo());
			codeViewer.set_Text(codeToView);	
			return targetControl;
		}
		
		
		public static TreeView view_In_TreeView(this List<SyntaxNode> syntaxNodes)
		{
			return "SyntaxNode Root Path".popupWindow()
										.add_TreeView_with_PropertyGrid()
										.add_Nodes(syntaxNodes, 
													(node) => "{0} :  {1}".format(node.typeName(), node))
										.selectFirst();
		}
		
		public static TreeView root_Path_In_TreeView(this SyntaxNode syntaxNode)
		{
			return "SyntaxNode Root Path".popupWindow()
										.add_TreeView_with_PropertyGrid()
										.add_Nodes(syntaxNode.root_Path(), 
													(node) => "{0} :  {1}".format(node.typeName(), node))
										.selectFirst();
		}
	}	
}
    	