// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.O2Findings;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6;
using O2.Interfaces.O2Core;
using O2.Interfaces.O2Findings;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;
using O2.Views.ASCX.O2Findings;

//O2File:Findings_ExtensionMethods.cs

namespace O2.XRules.Database.Findings
{
	public static class Findings_ExtensionMethods_IO2Finding_Gui_Viewers
    {    
    	public static T buildGUI_FilterBy_SourceCode<T>(this T control, List<IO2Finding> o2Findings)
    		where T : Control
		{
			return control.buildGUI_FilterBy_Property(o2Findings, "SourceCode");
		}
		
		public static T buildGUI_FilterBy_Property<T>(this T control, List<IO2Finding> o2Findings,string propertyName)
    		where T : Control
		{
		
			var controls = control.add_1x2("Filtered data", "Traces and Findings", "Source Code", true,control.width()/3,control.height()/2);

			var tracesSourceCode = o2Findings.filterBy_Traces_Property(propertyName);
			
			var traceSourceCode = controls[2].add_SourceCodeViewer();
			var completeTrace = controls[1].add_TraceViewer();
			var tracesList = completeTrace.insert_Left<Panel>(250).add_TreeView().showSelection();
			var findingsList = tracesList.insert_Below<Panel>(tracesList.height()/2).add_FindingsViewer();
			
			if (propertyName == "SourceCode")
				completeTrace.view_SourceCode();
			
			completeTrace.afterSelect_ShowTraceInCodeViewer(traceSourceCode);
			findingsList.afterSelect_ShowTraceInCodeViewer(traceSourceCode);
			findingsList.afterSelect_showTrace(completeTrace);
			
			var filteredData = controls[0].add_TreeViewWithFilter(tracesSourceCode)
									  	  .showSelection()
									  	  .sort();
									  
			tracesList.afterSelect<KeyValuePair<IO2Finding, IO2Trace>>(
				(keyValuePair)=>{						
									var o2Finding = keyValuePair.Key;
									var o2Trace = keyValuePair.Value;
									completeTrace.show(o2Finding);								
									completeTrace.selectNodeWithText(filteredData.selected().get_Text());
									Application.DoEvents();
									tracesList.focus();
								});
			
			
			filteredData.afterSelect<List<KeyValuePair<IO2Finding, IO2Trace>>>(			
				(list)=> {
							tracesList.clear();
							tracesList.add_Nodes(list);
							findingsList.show(list.o2Findings());
							findingsList.expand();
							tracesList.selectFirst();							
						});
			filteredData.selectFirst();

			return control;
		}
    	    	    	    	    
    }
}
