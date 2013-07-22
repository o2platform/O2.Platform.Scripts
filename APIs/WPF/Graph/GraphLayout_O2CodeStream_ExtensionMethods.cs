using System;
using FluentSharp.CSharpAST.Utils;
using FluentSharp.CoreLib;
using FluentSharp.WPF;
using GraphSharp.Controls;
using O2.API.AST.Graph;

//O2File:GraphLayout_WPF_ExtensionMethods.cs
//O2File:GraphSharp_ExtensionMethods.cs
//O2File:CodeStreamGraphNode.cs
//O2Ref:Quickgraph.dll
//O2Ref:GraphSharp.dll
//O2Ref:GraphSharp.Controls.dll


namespace O2.XRules.Database.Utils
{
    public static class GraphLayout_O2CodeStream_ExtensionMethods
    {
        public static O2CodeStream show(this O2CodeStream o2CodeStream, GraphLayout graphLayout)
        {
            try
            {                  
                //graphLayout.newGraph();
                graphLayout.tree();                
                foreach (var streamNode in o2CodeStream.StreamNode_First)
                    graphLayout.show_CodeStreamNode(o2CodeStream, streamNode, null);
            }
            catch (Exception ex)
            {
                ex.log("in O2CodeStream.show GraphLayout");
            }
            return o2CodeStream;
        }

        public static void show_CodeStreamNode(this GraphLayout graphLayout, O2CodeStream codeStream, O2CodeStreamNode codeStreamNode, CodeStreamGraphNode previousNode)
        {
            try
            {
                if (codeStreamNode == null)
                    return;

                if (previousNode == null)
                    previousNode = graphLayout.add_CodeStreamNode(codeStream, codeStreamNode);
                else
                    previousNode = graphLayout.add_CodeStreamEdge(codeStream, codeStreamNode, previousNode);


                foreach (var childNode in codeStreamNode.ChildNodes)
                {
                    if (codeStreamNode != childNode)
                        graphLayout.show_CodeStreamNode(codeStream, childNode, previousNode);
                    else
                        "in show_StreamNode, streamNode ==  childNode: {0}".error(childNode.Text);
                }
            }
            catch (Exception ex)
            {
                ex.log("in show_StreamNode");
            }
        }

        public static CodeStreamGraphNode add_CodeStreamNode(this GraphLayout graphLayout, O2CodeStream codeStream, O2CodeStreamNode codeStreamNode)
        {
            return (CodeStreamGraphNode)graphLayout.wpfInvoke(
                () =>
                {
                    var codeStreamGraphNode = new CodeStreamGraphNode(codeStream, codeStreamNode);
                    graphLayout.add_Node(codeStreamGraphNode);
                    return codeStreamGraphNode;
                });
        }

        public static CodeStreamGraphNode add_CodeStreamEdge(this GraphLayout graphLayout, O2CodeStream codeStream, O2CodeStreamNode codeStreamNode, CodeStreamGraphNode previousGraphNode)
        {
            return (CodeStreamGraphNode)graphLayout.wpfInvoke(
                () =>
                {
                    var codeStreamGraphNode = new CodeStreamGraphNode(codeStream, codeStreamNode);
                    graphLayout.add_Edge(previousGraphNode, codeStreamGraphNode);
                    return codeStreamGraphNode;
                });
        }
    }
}
