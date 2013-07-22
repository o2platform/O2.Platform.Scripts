// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using FluentSharp.CSharpAST.Utils;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL;
using FluentSharp.REPL.Controls;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using ICSharpCode.NRefactory.Ast;

//O2File:Ast_Engine_ExtensionMethods.cs
//O2File:SharpDevelop_O2MappedAstData_ExtensionMethods.cs
//O2File:TextEditor_O2CodeStream_ExtensionMethods.cs
//O2Ref:QuickGraph.dll



namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
	public class test_ascx_SearchAST
	{		
		public void launchGui()
		{
			var astData = new O2MappedAstData();
			
			//astData.loadFile("HacmeBank_v2_Website.ascx.PostMessageForm.btnPostMessage_Click.cs".local());
			
			var control = O2Gui.open<Panel>("AST Search (.NET Static Analysis)",1000,600);
			var searchAST = control.add_Control<ascx_SearchAST>();
			searchAST.buildGui(astData);
			
			
		}
	}

	public class ascx_SearchAST : Control
	{
		public O2MappedAstData AstData {get;set;}	
		public TreeView AstValueTreeView { get; set; }
		public TreeView AstTypeTreeView { get; set; }
		public TreeView SourceCodeLines { get; set; }
		public TextBox INodeFilter_TextBox { get; set; }
		public TextBox SelectedINodeFilter_TextBox { get; set; }
		public TextBox AllINodesFilter_TextBox { get; set; }
		
		
		public ascx_SourceCodeViewer CodeViewer { get; set; }
		public ProgressBar TopProgressBar { get; set; }
		public String INodeTypeFilter { get; set; }
		public String INodeValueFilter { get; set; }
		
		public Dictionary<string,List<INode>> iNodesByType;
		
		public ascx_SearchAST()//astData astEngine)
		{		
			iNodesByType = new  Dictionary<string,List<INode>>();
		}	
		
		public void buildGui(O2MappedAstData astData) 
		{
			AstData = astData;
			INodeTypeFilter = "";
			INodeValueFilter = "";
			buildGui();
			loadDataInGui();
		}
		
		public void buildGui()
		{
			
			//..clear();
			var topPanel = this.add_1x1("AST INode Value", "Source Code", true, 400);
			
			CodeViewer = topPanel[1].add_SourceCodeViewer(); 
			
			AstValueTreeView = topPanel[0].add_TreeView()
										  .sort() 
										  .showSelection()
										  .beforeExpand<List<INode>>(
											  	(selectedNode, nodes)=>{
											  				 selectedNode.add_Nodes(nodes);
											  			 });
						
			
			AstTypeTreeView  = topPanel[0].insert_Left<GroupBox>(200)
									.set_Text("AST INode Type") 
									.add_TreeView()
									.sort()
									.showSelection()
									.afterSelect<List<INode>>(
										(iNodes)=>{ 	 															
											 		  showINodes(iNodes);
												  });
			
												 		 
			INodeFilter_TextBox = AstTypeTreeView.insert_Above(40,"Filter").add_TextBox("Filter","")
												 .onTextChange_AlertOnRegExFail()
												 .onEnter((value)=>{
																		INodeTypeFilter= value;
																		loadDataInGui();
																   });

			//nodeFilterTextBox.onTextChange_AlertOnRegExFail();
			//nodeTypeTextBox.onTextChange_AlertOnRegExFail();
						 
			SelectedINodeFilter_TextBox = AstValueTreeView.insert_Above(40,"Search on selected INode Type").add_TextBox("Filter","")
											.onTextChange_AlertOnRegExFail()
											.onEnter(
											(value)=>{
														INodeValueFilter= value;
														showINodes(null);
													 });
			AllINodesFilter_TextBox = AstValueTreeView.insert_Above(40,"Search on ALL INodes Type").add_TextBox("Filter","")
											.onTextChange_AlertOnRegExFail()
											.onEnter(
											(value)=>{
														INodeValueFilter= value;
														showINodes(AstData.iNodes());
													 });										 
			
			
			
			AstTypeTreeView.onDrop(
				(fileOrFolder)=>{
									if(fileOrFolder.fileExists())
										AstData.loadFile(fileOrFolder);
									else
										AstData.loadFiles(fileOrFolder.files(true,"*.cs","*.vb"));
									loadDataInGui();
								});
			
			SourceCodeLines = CodeViewer.insert_Below(200,"Source Code Lines").add_TreeView().sort();
			
			AstData.afterSelect_ShowInSourceCodeEditor(AstValueTreeView, CodeViewer.editor());
			AstData.afterSelect_ShowInSourceCodeEditor(SourceCodeLines, CodeViewer.editor());
			AstValueTreeView.afterSelect<List<INode>>(
				(iNodes)=>{																
								SourceCodeLines.clear();
								var addedLines = new List<string>();
								foreach(var iNode in iNodes)
								{								
									var line = iNode.StartLocation.Line;
									if ( line >0)
									{
										var file = AstData.file(iNode);																				
										if (file.valid())
										{
											var addedLine = "{0}:{1}".format(file, line);
											if (addedLines.contains(addedLine).isFalse())
											{
												addedLines.add(addedLine);
												var fileContents = file.fileContents();
												if (fileContents.valid())
												{
													var lineText = fileContents.split_onLines()[line-1];												
													var location = "[{0}:{1}]      {2} ".format(file.fileName(), line, file);
													SourceCodeLines.add_Node(lineText, iNode)
																   .add_Node(location);
												}
											}
										}
									}
								}
								//show.info(iNodes[0]);
								//SourceCodeLines.add_Nodes(iNodes, (iNode)=>iNode.sourceCode);
								//.showDetails();
								//"treeNode selected: {0}".info(tag.typeFullName());
							});																   							
		}
		
		public void showINodes(List<INode> iNodes)
		{
			if (iNodes == null)
				if (AstTypeTreeView.selected() ==null)
					return;
				else
					iNodes = (List<INode>)AstTypeTreeView.selected().tag<List<INode>>(); 						
			AstValueTreeView.backColor(Color.Azure);
            //AstValueTreeView.visible(false);
            AstValueTreeView.clear();
			O2Thread.mtaThread(
				()=>{  
						var notResolvedText = "   ____INODE Text not resolved";
						var indexedData = new Dictionary<string, List<INode>>();
						foreach(var iNode in iNodes)
						{	
							var value = notResolvedText;
							if (PublicDI.reflection.getPropertyInfo("Value", iNode.type()).notNull())
								value  = iNode.property("Value").str();
							if (PublicDI.reflection.getPropertyInfo("Name", iNode.type()).notNull())
								value  = iNode.property("Name").str();	
							else if (PublicDI.reflection.getPropertyInfo("Identifier", iNode.type()).notNull())
								value  = iNode.property("Identifier").str();
							else if (PublicDI.reflection.getPropertyInfo("MemberName", iNode.type()).notNull())
								value  = iNode.property("MemberName").str();	
							else if (PublicDI.reflection.getPropertyInfo("ParameterName", iNode.type()).notNull())
								value  = iNode.property("ParameterName").str();		
										
							if (value ==  notResolvedText)
							{								
								if (iNode.str().regEx(INodeValueFilter))
									indexedData.add(notResolvedText,iNode);
							}
							else
								if (value.regEx(INodeValueFilter))
									indexedData.add(value.trim(),iNode);
						}						
						AstValueTreeView.add_Nodes(indexedData, 1000, TopProgressBar); 
						AstValueTreeView.backColor(Color.White);
						AstValueTreeView.selectFirst();
						return;
						
						//var indexedData = iNodes.indexOnToString(INodeValueFilter);   							
						;
						//getTextForINode
						/*var typeName = iNodes[0].typeName();			 																																		 	
						var rootNode = AstValueTreeView.add_Node(typeName, null);
						rootNode.add_Nodes(indexedData, 1000, TopProgressBar); 
						AstValueTreeView.visible(true);
						if (AstValueTreeView.nodes().size() > 0 && AstValueTreeView.nodes()[0].nodes().size() >0 )
							AstValueTreeView.nodes()[0].nodes()[0].selected(); 		  																		
						rootNode.expand();														
						*/
					});
		}
		
		public void loadDataInGui()
		{
			AstTypeTreeView.clear();
			AstValueTreeView.clear();
			iNodesByType = AstData.iNodes_By_Type(INodeTypeFilter); 						
			
			foreach(var item in iNodesByType)
			{					
				var nodeText = "{0}   ({1})".format(item.Key, item.Value.size()); 	
				AstTypeTreeView.add_Node(nodeText, item.Value);						
			}
			
			AstTypeTreeView.selectFirst();
		}			
		
		public void setINodeFilter(string value)
		{
			INodeFilter_TextBox.set_Text(value);
			INodeTypeFilter= value;
			loadDataInGui();
		}
		
		public void setSearchOnSelectedINode(string value)
		{
			SelectedINodeFilter_TextBox.set_Text(value);
			INodeValueFilter= value;
			showINodes(null);			
		}
		
		public void setSearchOnAlINodes(string value)
		{
			AllINodesFilter_TextBox.set_Text(value);
			INodeValueFilter= value;
			showINodes(AstData.iNodes());			
		}		
	}
}
