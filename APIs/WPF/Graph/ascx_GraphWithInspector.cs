// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.REPL;
using FluentSharp.WPF.Controls;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;


//O2File:GraphSharp_ExtensionMethods.cs
//O2File:VerticesAndEdges_ExtensionMethods.cs

//O2Ref:FluentSharp.WPF.dll
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:GraphSharp.dll
//O2Ref:QuickGraph.dll
//O2Ref:GraphSharp.Controls.dll
//O2Ref:WPFExtensions.dll
//O2Ref:O2_External_IE.dll

//O2Ref:PresentationCore.dll
//O2Ref:PresentationFramework.dll
//O2Ref:WindowsBase.dll
//O2Ref:System.Xaml.dll

namespace O2.XRules.Database.Utils
{
    public class ascx_GraphWithInspector : Control
    {        

		public static void runControl()
		{		
		 	O2Gui.load<ascx_GraphWithInspector>("Graph"); 		 
		}
		
		public ascx_GraphWithInspector()
		{						
			this.Width = 600;
			this.Height = 400;
			/*var editor = this.add_SourceCodeViewer();
			editor.set_Text("asd");
			return;
			*/
			var controls = this.add_1x1("Graph","Inspector",false,200);
			
			var graph = controls[0].add_Graph();
			
			//graph.testGraph();
			var xamlHost = (ascx_Xaml_Host)(controls[0].Controls[0]);
            var script = controls[1].add_Script(true); 

            script.Code = "graph.testGraph();".line() +
                          "return \"comment (using //) this line to see more graph actions\";".line()
                          .line() +
						  "graph.node(100);".line() + 
						  "graph.edge(100,\"A\");".line() + 
						  "graph.edge(100,\"F\");" .line() +
                          "graph.edge(200,\"F\");".line() + 
                          "graph.edge(200,300);" .line() + 
                          "var blueLabel = graph.add_UIElement<WPF.Label>();" .line() + 
                          "blueLabel.set_Content(\"blue label\");" .line() + 
                          "Control_ExtensionMethods.color(blueLabel,\"Blue\");" .line() + 
                          "graph.edge(200,blueLabel);" .line() + 
                          "var redLabel = graph.add_UIElement<WPF.TextBox>().set_Text(\"red textbox\");" .line() +
                          "Control_ExtensionMethods.color(redLabel,\"Red\");".line() + 
                          "graph.edge(blueLabel,redLabel);".line() + 
                          "//graph.edgeToAll(200);".line() + 
                          "graph.defaultLayout();".line() + 
                          "//graph.overlapRemovalParameters(20, 50);".line() +  
						  "graph.showAllLayouts(2000);".line() + 
						  "graph.circular();".line().line() + 
                          "return graph;".line().line() +                           
                          "//using O2.XRules.Database.Utils;".line() +
                          "//using FluentSharp.WPF".line() +
						  "//using System.Xml.Linq".line()+
						  "//using System.Linq".line()+
						  "//using WPF=System.Windows.Controls".line() + 						  
						  "//O2File:GraphSharp_ExtensionMethods.cs".line()+
						  "//O2File:VerticesAndEdges_ExtensionMethods.cs".line()+						  					  
						  "//O2Ref:FluentSharp.WPF.dll".line()+
						  "//O2Ref:System.Xml.dll".line()+
						  "//O2Ref:System.Xml.Linq.dll".line()+						  
						  "//O2Ref:PresentationCore.dll".line()+
						  "//O2Ref:PresentationFramework.dll".line()+
						  "//O2Ref:WindowsBase.dll   ".line()+
						  "//O2Ref:System.Core.dll".line()+
						  "//O2Ref:WindowsFormsIntegration.dll".line()+
						  "//O2Ref:GraphSharp.dll".line()+
						  "//O2Ref:QuickGraph.dll".line()+
						  "//O2Ref:GraphSharp.Controls.dll".line()+
						  "//O2Ref:ICSharpCode.AvalonEdit.dll".line() +
						  "//O2Ref:System.Xaml".line();

			script.InvocationParameters.Add("graph", graph);
			script.InvocationParameters.Add("elementHost", xamlHost.element());
            script.onCompileExecuteOnce();
            script.compileCodeSnippet(script.Code);
        }    	    	    	    	    
    }
}