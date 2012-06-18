// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Collections.Generic;
using Forms = System.Windows.Forms;
using WPF = System.Windows.Controls;
using Media = System.Windows.Media;
using System.Linq;
using System.Reflection;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.API.AST.CSharp;
using O2.API.AST.ExtensionMethods;
using O2.API.AST.ExtensionMethods.CSharp;
using O2.API.Visualization.ExtensionMethods;
using O2.Views.ASCX;
using GraphSharp.Controls;
using O2.XRules.Database.Utils;

//O2File:WPF_ExtensionMethods.cs
//O2File:GraphLayout_O2CodeStream_ExtensionMethods.cs
//O2File:VerticesAndEdges_ExtensionMethods.cs
//O2File:Wpf_TextEditor_ExtensionMethods.cs
//O2File:Ast_Engine_ExtensionMethods.cs
//O2File:ascx_Simple_Script_Editor.cs.o2
//O2File:Scripts_ExtensionMethods.cs

//O2Ref:GraphSharp.dll
//O2Ref:GraphSharp.Controls.dll
//O2Ref:QuickGraph.dll
//O2Ref:PresentationFramework.dll
//O2Ref:PresentationCore.dll
//O2Ref:WindowsBase.dll
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:O2_API_Visualization.dll
//O2Ref:O2_API_AST.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:System.Xml.dll



namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
    public class ascx_GraphAst_MethodCalls : System.Windows.Forms.Control
    {        	
		public List<Forms.Control> GraphModeTopPanels {get; set;}
		public List<Forms.Control> GraphModeRightPanels {get; set;}
		//public ascx_SourceCodeViewer CodeViewer { get; set; }
		public GraphLayout GraphViewer { get; set; }
		public Forms.Panel GraphOptionsPanel { get; set; }
		
		public Dictionary<string,List<String>> MethodsCalledMappings { get; set; }
		public Dictionary<string,List<String>> MethodIsCalledByMappings { get; set; }
		public List<string> AllMethods { get; set; }			
		public String FileFilter { get; set; }
		
		public ascx_Simple_Script_Editor GraphScript { get; set; }
		public Dictionary<string, WPF.Control> GraphNodes { get; set;}
		
		public bool UseStarAsNodeText { get; set; }
		public bool AutoExpandCalledMethods { get; set; }
		public bool AutoExpandMethodsCalledBy { get; set; }
		public bool ExpandMethodsCalled {get; set;}
		public bool ExpandMethodIsCalledBy {get; set;}
		public bool ClearGraphOnMethodView { get; set; }
		
		public O2MappedAstData AstData {get;set;}
		
		public ascx_GraphAst_MethodCalls()
		{
			AstData = new O2MappedAstData();
			GraphNodes = new Dictionary<string, WPF.Control>();
		}
		
		public ascx_GraphAst_MethodCalls setData(O2MappedAstData astData, 
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
			GraphModeTopPanels = this.add_1x1("Select root node", "Graph", true, this.width() / 2);
			GraphModeRightPanels = this.GraphModeTopPanels[1].add_1x1(false);
			
			var GraphTreeViewWithMethods = this.GraphModeTopPanels[0].add_TreeViewWithFilter(AllMethods).sort();
			this.GraphModeRightPanels[0].backColor(Color.White);
			
			GraphViewer = this.GraphModeRightPanels[0].add_Graph();				
			GraphScript = this.GraphModeRightPanels[1].add_Script(false);
			
			//GraphModeRightPanels[1].parent<SplitContainer>().panel2Collapsed(true);
			
			GraphTreeViewWithMethods.afterSelect<string>(createAndAddGraphNode);
			
			GraphOptionsPanel = GraphScript.insert_Left<Forms.Panel>(200);
			
			// GraphOptionsPanel options & actions
			
			GraphOptionsPanel.add_Link("Auto Expand All Methods" , 0,0,()=> graphAutoExpandAllMethods());							
			
			GraphOptionsPanel.add_Label("Select graph Layout", 20,0);
			
			var OptionsLayoutCombox = GraphOptionsPanel.add_ComboBox(35,0);
			OptionsLayoutCombox.add_Items(new List<string> {
																"Tree",
																"CompoundFDP", 
																"LinLog",
																"ISOM",
																"KK",
																"BoundedFR",
																"FR",
																"Circular"
															});
			OptionsLayoutCombox.select_Item(0);												
			OptionsLayoutCombox.onSelection(()=> GraphViewer.layout(OptionsLayoutCombox.get_Text()));
			
			GraphOptionsPanel.add_Link("Load All Methods" , 60,0,()=> graphAllMethods());
			
			GraphOptionsPanel.add_CheckBox("Expand Methods Called", 80,0, (value)=> ExpandMethodsCalled = value).check().autoSize();
			GraphOptionsPanel.add_CheckBox("Expand Method Is CalledBy", 100,0, (value)=> ExpandMethodIsCalledBy = value).check().autoSize();			
			GraphOptionsPanel.add_CheckBox("Clear Graph on Method View", 120,0, (value)=> ClearGraphOnMethodView = value).check().autoSize();
			GraphOptionsPanel.add_CheckBox("use '*' as node Text", 140,0, (value)=> UseStarAsNodeText = value).autoSize();
			
			GraphOptionsPanel.add_Link("set note text to: '*' " , 160,0,()=> setNodeTextTo_Star());
			//GraphOptionsPanel.add_Link("set note text to: method name " , 190,0,()=> setNodeTextTo_MethodName());
			//GraphOptionsPanel.add_Link("set note text to: method signature " , 180,0,()=> setNodeTextTo_MethodSignature());
		}
		
		public void createAndAddGraphNode(string methodSignature)
		{
			O2Thread.mtaThread(
				()=>{
						"in createAndAddGraphNode".debug();
						"Creating Graph for: {0}".debug(methodSignature);
						if (ClearGraphOnMethodView)
						{
							GraphNodes = new Dictionary<string, WPF.Control>();
						
			 				this.GraphModeRightPanels[0].clear();			
							GraphViewer = this.GraphModeRightPanels[0].add_Graph();
							GraphViewer.tree();
						}
						
						
						GraphScript.InvocationParameters.Clear(); 
						GraphScript.InvocationParameters.add("graph",GraphViewer);
						GraphScript.InvocationParameters.add("astData",AstData);							
						
						addGraphNode(methodSignature);
						
						/*var graphNode = createGraphNode(methodSignature);
															
						this.GraphViewer.add_Node(graphNode);
						addCallerAndCalleesToGraphNode(graphNode, methodSignature);*/
						//expandGraph();
					});				
		}
		
		public void addGraphNode(string methodSignature)
		{
			var graphNode = createGraphNode(methodSignature);																
			this.GraphViewer.add_Node(graphNode);
			addCallerAndCalleesToGraphNode(graphNode, methodSignature);
		}
		public void addCallerAndCalleesToGraphNode(WPF.Control graphNode)
		{
			if (graphNode != null)
				addCallerAndCalleesToGraphNode(graphNode,graphNode.get_Tag<string>());
		}
		
		public void addCallerAndCalleesToGraphNode(WPF.Control graphNode, string methodSignature)
		{
			if (graphNode != null && methodSignature.valid())
			{
				if (ExpandMethodsCalled)
					if (this.MethodsCalledMappings.hasKey(methodSignature))
						foreach (var methodCalled in this.MethodsCalledMappings[methodSignature]) 
							//if (GraphNodes.hasKey(methodCalled).isFalse())
								GraphViewer.add_Edge(graphNode, createGraphNode(methodCalled));
				if (ExpandMethodIsCalledBy)
					if (MethodIsCalledByMappings.hasKey(methodSignature))
						foreach (var methodIsCalledBy in this.MethodIsCalledByMappings[methodSignature])
							//if (GraphNodes.hasKey(methodIsCalledBy).isFalse())
								GraphViewer.add_Edge(createGraphNode(methodIsCalledBy), graphNode);
								
				graphNode.color("Green");
			}
		}
		
		public WPF.Control createGraphNode(string methodSignature)
		{
			if (GraphNodes.hasKey(methodSignature))
				return GraphNodes[methodSignature];
				
			var graphNode = GraphViewer.newInThread<WPF.Label>();
			var iMethod = AstData.iMethod_withSignature(methodSignature);	
			if (iMethod != null)											
			{						
				//this.GraphModeRightPanels[0].clear();
				//GraphViewer = this.GraphModeRightPanels[0].add_Graph();					
				graphNode.set_Content(iMethod.Name);
				//graphNode.set_Tag(iMethod);				
				graphNode.color("Black"); 					
			}
			else
			{					
				graphNode.color("Red"); 
				graphNode.set_Content(methodSignature);
			}
			if (UseStarAsNodeText)
				graphNode.set_Content("*");
			graphNode.set_Tag(methodSignature);
			
			graphNode.onMouseDoubleClick<string, WPF.Label>(
				(signature)=>{
								addCallerAndCalleesToGraphNode(graphNode, signature);
							 });
								
			GraphNodes.add(methodSignature, graphNode);
			return graphNode;				
		}
		 
		public void graphAutoExpandAllMethods()
		{
			O2Thread.mtaThread(
				()=>{
						var maxNodeSize = 300;
						var nodeToExpand = this.unexpandedNode();
						while (maxNodeSize > GraphViewer.nodes().size() && nodeToExpand != null)
						{				
							addCallerAndCalleesToGraphNode(nodeToExpand);
							"after expand, there are: {0} nodes".info(GraphViewer.nodes().size());
							nodeToExpand = unexpandedNode();
						}
						if (maxNodeSize < GraphViewer.nodes().size())
						{
							"in graphAutoExpandAllMethods, maxNode size reached: {0}".error(maxNodeSize);
						}
						"graphAutoExpandAllMethods completed".info(); 				
					});
		}
		
		public void graphAllMethods()
		{
			O2Thread.mtaThread(
				()=>{
						var maxNodeSize = 300;
						foreach(var methodSignature in AllMethods)
						{
							addGraphNode(methodSignature);
							if (maxNodeSize < GraphViewer.nodes().size())
							{
								"in graphAutoExpandAllMethods, maxNode size reached: {0}".error(maxNodeSize);
								break;
							}
						}
						"graphAllMethods completed".info(); 				
					});
		}
		
		public WPF.Label unexpandedNode()
		{
			foreach(var node in GraphViewer.nodes())
				if (node is WPF.Label)
				{
					var label = (node as WPF.Label);
					if (label.get_Color() != Media.Brushes.Green)
						return label;
				}
			return null;					
		}
		
		
		public void setNodeTextTo_Star()
		{	
			var command = "foreach(WPF.Label node in Graph.nodes())".line() +
					      "		node.set_Text(\"*\");";
			GraphScript.set_Command(command);
			
		}
		public void setNodeTextTo_MethodName()
		{
			GraphScript.set_Command("setNodeTextTo_MethodName");
		}
		public void setNodeTextTo_MethodSignature()
		{
			GraphScript.set_Command("setNodeTextTo_MethodSignature");
		
		}
    	    	    	    	    
    }
}
