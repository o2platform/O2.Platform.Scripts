// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System;
using System.Linq;
using System.Collections.Generic;
using GraphSharp.Controls;
using QuickGraph;
using GraphSharp.Algorithms.Layout.Simple.Tree;
using GraphSharp.Algorithms.Layout;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.API.AST.ExtensionMethods;
using O2.API.Visualization.ExtensionMethods;
using O2.API.Visualization;

//O2File:WPF_Controls_ExtensionMethods.cs
//O2File:GraphLayout_WPF_ExtensionMethods.cs

//O2Ref:GraphSharp.dll
//O2Ref:QuickGraph.dll
//O2Ref:GraphSharp.Controls.dll 
//O2Ref:O2_API_AST.dll
//O2Ref:O2_API_Visualization.dll

namespace O2.XRules.Database.Utils
{
    public static class GraphSharp_ExtensionMethods
    {    
    	#region thread related
    	
    	public static T newInThread<T>(this GraphLayout graphLayout) where T : UIElement
		{
			return (T)graphLayout.wpfInvoke(
    			()=>{ 
    					return (T)typeof(T).ctor();    					
    				});
		}  
		
    	#endregion
        
        #region GraphLayout

        public static BidirectionalGraph<object, IEdge<object>> graph(this GraphLayout graphLayout)
        {
            return graphLayout.get_Graph();
        }

        public static BidirectionalGraph<object, IEdge<object>> get_Graph(this GraphLayout graphLayout)
    	{
    		return (BidirectionalGraph<object, IEdge<object>>)
    			graphLayout.wpfInvoke(
    						()=> {
    								return graphLayout.Graph; 
    							 });
    	}

        public static Label add_Label(this GraphLayout graphLayout, Int32 nodeValue)
        {
            return graphLayout.add_UIElement<Label>().set_Text_Wpf(nodeValue.str());            
        }

        public static Label add_Label(this GraphLayout graphLayout, string nodeText)
        {
            return graphLayout.add_UIElement<Label>().set_Text_Wpf(nodeText);            
        }

    	public static GraphLayout add_Node(this GraphLayout graphLayout,object vertexToAdd)
    	{
            if (graphLayout == vertexToAdd)     // can't add graphLayout as an Node
                return graphLayout;
    		var graph = graphLayout.get_Graph();
    		if (graph == null)
    		{
    			"in GraphLayout.add, get_Graph() returned null".error();
    			return graphLayout;
    		}
    		return (GraphLayout)graphLayout.wpfInvoke(
    			()=>{
    					graph.add_Node(vertexToAdd);
    					return graphLayout;
    				});
    	}

        public static GraphLayout add_Edge(this GraphLayout graphLayout, object fromVertex, object toVertex)
        {
            try
            {
                if (fromVertex != toVertex)
                    return graphLayout.edge(fromVertex, toVertex);
                else
                    return graphLayout.add_Node(fromVertex);
            }
            catch (Exception ex)
            {
                ex.log("in GraphLayout.add_Edge");
                return graphLayout;
            }
        }
    	
    	public static GraphLayout edge(this GraphLayout graphLayout,object fromVertex, object toVertex)
    	{
            if (graphLayout == fromVertex || graphLayout == toVertex)  // can add graphLayout as an edge
                return graphLayout;
    		var graph = graphLayout.get_Graph();
    		return (GraphLayout)graphLayout.wpfInvoke(
    			()=>{
    					graph.edge(fromVertex,toVertex);
    					return graphLayout;
    				});
    	}
    	
    	public static GraphLayout set(this GraphLayout graphLayout, BidirectionalGraph<object, IEdge<object>> graph)
		{
			return (GraphLayout)graphLayout.wpfInvoke(
    			()=>{
    					graphLayout.Graph = graph;
    					return graphLayout;
    				});
		}    	    	    	    	    				    	    	    	    	
    	        	
    	public static GraphLayout background(this GraphLayout graphLayout, Brush brush)
    	{
	    	return (GraphLayout)graphLayout.wpfInvoke(
    			()=>{  
			    		graphLayout.Background = brush;		    		
			    		return graphLayout;    		
			    	});
    	}
    	
    	public static GraphLayout wait(this GraphLayout graphLayout, int miliSeconds)
    	{
    		return graphLayout.sleep(miliSeconds);
    	}
    	
    	public static GraphLayout sleep(this GraphLayout graphLayout, int miliSeconds)
    	{
    		System.Threading.Thread.Sleep(miliSeconds);
    		return graphLayout;
    	}
    	
    	#endregion
    	
    	#region GraphLayout - defaults
    	
    	public static GraphLayout clear(this GraphLayout graphLayout)
    	{
    		graphLayout.invoke("RemoveAllGraphElement");
			graphLayout.newGraph();
			return graphLayout;
		}
    	
    	public static GraphLayout newGraph(this GraphLayout graphLayout)
    	{
    		return (GraphLayout)graphLayout.wpfInvoke(
    			()=>{
                        try
                        {
                            graphLayout.Graph = new BidirectionalGraph<object, IEdge<object>>();
                        }
                        catch (Exception ex)
                        {
                            ex.log("in GraphLayout.newGraph");
                        }
			    		return graphLayout;
			    	});
    	}
    	
    	
    	public static GraphLayout defaultLayout(this GraphLayout graphLayout)
    	{
    		graphLayout.wpfInvoke(
    			()=>{
                        try
                        {
                            graphLayout.LayoutAlgorithmType = "Circular";
                            graphLayout.OverlapRemovalAlgorithmType = "FSA";
                            graphLayout.HighlightAlgorithmType = "Simple";
                            // this one was throwing an error on new graphs when there is not graph loaded (but the value seems to be set)
                            graphLayout.OverlapRemovalConstraint = GraphSharp.Controls.AlgorithmConstraints.Must; //Automatic;
                            
                        }
                        catch (Exception ex)
                        {
                            "in defaultLayout: {0}".format(ex.Message).debug();
                        }
			           	return graphLayout;
			         });
            return graphLayout;
        }

        public static GraphLayout overlapRemovalParameters(this GraphLayout graphLayout, float horizontalGap, float verticalGap)
        {
            return (GraphLayout)graphLayout.wpfInvoke(
                () =>
                {
                    var overlapRemovalParameters = new GraphSharp.Algorithms.OverlapRemoval.OverlapRemovalParameters();
                    overlapRemovalParameters.HorizontalGap = horizontalGap;
                    overlapRemovalParameters.VerticalGap = verticalGap;
                    graphLayout.OverlapRemovalParameters = overlapRemovalParameters;
                    return graphLayout;
                });
        }
        
/*    	public static GraphLayout betterLayout(this GraphLayout graphLayout)
    	{
    		return (GraphLayout)graphLayout.wpfInvoke(
    			()=>{
                        try
                        {                            
                            // this one was throwing an error on new graphs
                            
                        }
                        catch(Exception ex)
                        {
                            ex.log("in betterLayout");
                        }
			           	return graphLayout;
			         });
        }*/
        
       	public static GraphLayout testGraph(this GraphLayout graphLayout)
    	{    	 	        
            return graphLayout.set(GraphFactory.testGraph());            
    	 }
    	
    	
    	
    	#endregion
    	
    	#region GraphLayout - LayoutAlgorithmType
    	
    	public static List<string> layouts(this GraphLayout graphLayout)
    	{
    		return new List<string>( 
    			new [] {
				            "Circular",
				            "Tree",
				            "FR",
				            "BoundedFR",
				            "KK",
				            "ISOM",
				            "LinLog",
				            "CompoundFDP"
				            //,
				         //   "EfficientSugiyama"
				        });
    	}
        public static GraphLayout tree(this GraphLayout graphLayout)
        {
            return graphLayout.tree(false);
        }

    	public static GraphLayout tree(this GraphLayout graphLayout, bool horizontalLayout)
    	{
            graphLayout.wpfInvoke(
                () =>
                {
                    graphLayout.layout("Tree");                    
                    if (horizontalLayout)
                        if (graphLayout.LayoutParameters is SimpleTreeLayoutParameters)
                        {
                            var treeParameters = (SimpleTreeLayoutParameters)graphLayout.LayoutParameters;
                            treeParameters.Direction = LayoutDirection.LeftToRight;
                        }
                });
            return graphLayout;
    	}
        
    	
    	public static GraphLayout circular(this GraphLayout graphLayout)
    	{
    		return graphLayout.layout("Circular");
    	}
    	
    	public static GraphLayout fr(this GraphLayout graphLayout)
    	{
    		return graphLayout.layout("FR");
    	}
    	
    	public static GraphLayout boundedFR(this GraphLayout graphLayout)
    	{
    		return graphLayout.layout("BoundedFR");
    	}    
    	
    	public static GraphLayout efficientSugiyama(this GraphLayout graphLayout)
    	{
    		return graphLayout.layout("EfficientSugiyama");
    	}
    	
    	public static GraphLayout allL(this GraphLayout graphLayout)
    	{
    		return graphLayout.showAllLayouts();
    	}
    	
    	public static GraphLayout allLayouts(this GraphLayout graphLayout)
    	{
    		return graphLayout.showAllLayouts();
    	}
    	public static GraphLayout showAllLayouts(this GraphLayout graphLayout)
    	{
    		return graphLayout.showAllLayouts(1000);
    	}
    	public static GraphLayout showAllLayouts(this GraphLayout graphLayout, int miliSeconds) 
    	{
    		foreach(var layoutType in graphLayout.layouts())
    		{
    			"showing layout: {0}".format(layoutType).debug();  
    			graphLayout.layout(layoutType).sleep(miliSeconds);
    		}
			return graphLayout;
    	}
    	
    	public static GraphLayout layout(this GraphLayout graphLayout, string type)
    	{
    		return (GraphLayout)graphLayout.wpfInvoke(()=>
	    		{
	    			try
	    			{
	    				graphLayout.LayoutAlgorithmType = type;
	    			}
	    			catch (System.Exception ex)
	    			{
	    				ex.log("in GraphLayout layout ({0}) ".format(type));
	    			}
	    			return graphLayout;
	    		});
    	}
    	
    	#endregion

        #region using TreeView

        public static BidirectionalGraph<object, IEdge<object>> getGraph(this System.Windows.Forms.TreeView treeView, bool recursive)
        {
            var graph = new BidirectionalGraph<object, IEdge<object>>();
            treeView.Nodes.forEach<System.Windows.Forms.TreeNode>(
                (childNode) => graph.addFromTreeNode(childNode, recursive));

            return graph;
        }

        public static BidirectionalGraph<object, IEdge<object>> getGraph(this System.Windows.Forms.TreeNode treeNode, bool recursive)
        {
            var graph = new BidirectionalGraph<object, IEdge<object>>();
            graph.addFromTreeNode(treeNode, recursive);
            return graph;
        }

        public static object addFromTreeNode(this BidirectionalGraph<object, IEdge<object>> graph, System.Windows.Forms.TreeNode treeNode, bool recursive)
        {
            var vertex = treeNode.Text;
            graph.add_Node(vertex);
            if (recursive)
                treeNode.Nodes.forEach<System.Windows.Forms.TreeNode>(
                    (childNode) =>
                    {
                        var childVertex = graph.addFromTreeNode(childNode, true);
                        graph.edge(vertex, childVertex);
                    });

            return vertex;
        }

        #endregion
    }
    
    public static class GraphLayout_Options
    {
    	public static System.Windows.Forms.Panel add_GraphOptions(this System.Windows.Forms.Control control, GraphLayout graphLayout)
    	{    		    		 
    		var targePanel = control.insert_Below<System.Windows.Forms.Panel>(25);
			var label = targePanel.add_Label("Select graph Layout", 0,5);
			 
			var OptionsLayoutCombox = label.append_Control<System.Windows.Forms.ComboBox>().dropDownList();
			
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
			OptionsLayoutCombox.onSelection(()=> graphLayout.layout(OptionsLayoutCombox.get_Text()));
			OptionsLayoutCombox.append_Link("refresh", ()=>graphLayout.layout(OptionsLayoutCombox.get_Text()))
							   .append_Link("animate Graph Layouts", ()=> graphLayout.showAllLayouts(2000));
			return targePanel;
    	}
    }
}
