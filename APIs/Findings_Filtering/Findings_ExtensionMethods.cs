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

//O2File:Findings_ExtensionMethods_Gui_Viewers.cs
//O2File:O2AssessmentLoad_OunceV6.cs
//O2File:O2AssessmentSave_OunceV6.cs

namespace O2.XRules.Database.Findings
{

	public static class Findings_ExtensionMethods_IO2Finding
	{
		public static IO2Finding o2Finding(this string vulnName)
		{
			return vulnName.o2Finding(vulnName);
		}
		
		public static IO2Finding o2Finding(this string vulnName, string vulnType)
		{
			return new O2Finding(vulnName, vulnType);
		}
		
		public static O2Finding o2Finding(this IO2Finding iO2Finding)
		{
			return (O2Finding)iO2Finding;
		}
	
		public static string source(this IO2Finding iO2Finding)
		{
			return iO2Finding.o2Finding().Source;
		}
		
		public static string sink(this IO2Finding iO2Finding)
		{
			return iO2Finding.o2Finding().Sink;
		}
		
		public static List<string> sources(this List<IO2Finding> iO2Findings)
		{
			return (from O2Finding o2Finding in iO2Findings
					select o2Finding.Source).toList();
		}
		
		public static List<IO2Finding> Cookies(this List<IO2Finding> iO2Findings)
		{
			return (from O2Finding o2Finding in iO2Findings
		      where ((O2Finding)o2Finding).Source.contains("Cookie") 
		      select (IO2Finding)o2Finding).toList(); 
		}
		
		public static List<string> sinks(this List<IO2Finding> iO2Findings)
		{
			return (from O2Finding o2Finding in iO2Findings
					select o2Finding.Sink).toList();
		}
		
		public static List<IO2Trace> getSources(this List<IO2Finding> iO2Findings)
		{
			return (from O2Finding o2Finding in iO2Findings
					select o2Finding.getSource()).toList();
		}
		
		public static List<IO2Trace> getSinks(this List<IO2Finding> iO2Findings)
		{
			return (from O2Finding o2Finding in iO2Findings
					select o2Finding.getSink()).toList();
		}
		
		public static List<IO2Trace> allTraces(this IO2Finding iO2Finding)
		{
			return OzasmtUtils.getListWithAllTraces(iO2Finding.o2Finding());
		}
		
		public static List<IO2Trace> allTraces(this List<IO2Finding> iO2Findings)
		{
			var allTraces = new List<IO2Trace>();
			foreach(var iO2Finding in iO2Findings)
				allTraces.AddRange(OzasmtUtils.getListWithAllTraces(iO2Finding.o2Finding()));
			return allTraces;
		}
		
		public static List<string> allTraces_SourceCode(this IO2Finding iO2Finding)
		{			
			var sourceCodes = new List<string>();
			foreach(O2Trace o2Trace in OzasmtUtils.getListWithAllTraces(iO2Finding.o2Finding()))
			{
				var sourceCode = o2Trace.SourceCode.trim();
				if (sourceCode.valid() && sourceCodes.contains(sourceCode).isFalse())
					sourceCodes.Add(sourceCode);
			}
			return sourceCodes;
		}
		
		public static List<string> allTraces_SourceCode(this List<IO2Finding> iO2Findings)
		{
			var sourceCodes = new List<string>();
			foreach(var iO2Finding in iO2Findings)
				sourceCodes.add_OnlyNewItems(iO2Finding.allTraces_SourceCode());
			return sourceCodes;
		}
		
	}
	
	public static class Findings_ExtensionMethods_IO2Trace
	{
		public static IO2Trace add_Trace(this IO2Finding o2Finding, string text)
		{
			return o2Finding.o2Traces.add_Trace(text);
		}
		
		public static IO2Trace add_Trace(this List<IO2Trace> iO2Traces, string text)
		{
			var newTrace = new O2Trace(text);
			iO2Traces.Add(newTrace);
			return newTrace;
		}
		
		public static IO2Trace add_Trace(this IO2Trace o2Trace, string text)
		{
			return o2Trace.childTraces.add_Trace(text);
		}
	
		public static List<string> values(this List<IO2Trace> iO2Traces)
		{
			return iO2Traces.signatures();
		}
		
		public static List<string> signatures(this List<IO2Trace> iO2Traces)
		{
			return (from iO2Trace in iO2Traces
				    select iO2Trace.signature).toList();
		}
	}
 
    public static class Findings_ExtensionMethods_OpenAndLoad
    {        

		public static ascx_FindingsViewer add_FindingsViewer(this Control control)
		{
			return control.add_FindingsViewer(false);
		}
		
		public static ascx_FindingsViewer add_FindingsViewer(this Control control, bool includeSourceCodeViewer)
        {
            "O2_ImportExport_OunceLabs.dll".assembly()
                                           .type("OunceAvailableEngines")
                                           .invokeStatic("addAvailableEnginesToControl", new object[] {typeof(ascx_FindingsViewer)});
            var findingsViewer = control.add_Control<ascx_FindingsViewer>();
            if (includeSourceCodeViewer)
			{
				var codeViewer = findingsViewer.insert_Right<Panel>(control.width()/2).add_SourceCodeViewer();
				findingsViewer.set_CodeViewer(codeViewer);
				
			}
            return findingsViewer;
        }
        
        public static ascx_FindingsViewer set_CodeViewer(this ascx_FindingsViewer findingsViewer, ascx_SourceCodeViewer codeViewer)                
        {        
        	return findingsViewer.set_CodeEditor(codeViewer.editor());        	
        }
         
        public static ascx_FindingsViewer set_CodeEditor(this ascx_FindingsViewer findingsViewer, ascx_SourceCodeEditor codeEditor)
        {
        	findingsViewer._onTraceSelected += 
					(trace)=>{
									codeEditor.show(trace);
									findingsViewer.controls<ascx_TraceTreeView>().focus();
							 };
				findingsViewer._onFindingSelected +=
					(finding)=> { 				
									findingsViewer.invokeOnThread(
										()=>{
												codeEditor.textEditor().LineViewerStyle = ICSharpCode.TextEditor.Document.LineViewerStyle.None;												
												codeEditor.open(finding.file);											
												//"Going to line: {0}".info(finding.lineNumber);																					
												codeEditor.editor().gotoLine((int)finding.lineNumber);														
											});									
									O2Thread.mtaThread(
										()=>{
												findingsViewer.sleep(100,false);												
												findingsViewer.getResultsTreeView().focus();												
											});
								};
			return 	findingsViewer;							
        }
        
        public static ascx_FindingEditor add_FindingEditor(this Control control)
        {
        	return control.add_Control<ascx_FindingEditor>();
        }

        public static List<IO2Finding> o2Findings(this ascx_FindingsViewer findingsViewer)
        {
            return findingsViewer.getFindingsFromTreeView();
        }

        public static Thread openFindingsInNewWindow(this List<IO2Finding> o2Findings)
        {
            return ascx_FindingsViewer.openInFloatWindow(o2Findings);
        }

        public static Thread openFindingsInNewWindow(List<IO2Finding> o2Findings, string windowTitle)
        {
            return ascx_FindingsViewer.openInFloatWindow(o2Findings, windowTitle);
        }

		public static ascx_FindingsViewer load(this ascx_FindingsViewer findingsViewer, List<IO2Finding> o2Findings)
		{
			return findingsViewer.show(o2Findings);	
		}		
		
		public static ascx_FindingsViewer show(this ascx_FindingsViewer findingsViewer, IO2Finding o2Finding)
		{
			return findingsViewer.show(o2Finding.wrapOnList());	
		}
		
		public static ascx_FindingsViewer show(this ascx_FindingsViewer findingsViewer, List<IO2Finding> o2Findings)
        {
            findingsViewer.clearO2Findings();
            findingsViewer.loadO2Findings(o2Findings);
            return findingsViewer;
        }

		public static List<IO2Finding> loadO2Findings(this string fileToLoad)
		{
			return fileToLoad.loadFindingsFile();
		}
		
        public static List<IO2Finding> loadFindingsFile(this string fileToLoad)
        {
            var o2Assessment = new O2Assessment(new O2AssessmentLoad_OunceV6(), fileToLoad);
            "there are {0} findings loaded in this file".info( o2Assessment.o2Findings.Count);
            return o2Assessment.o2Findings;
        }
        
        public static string saveFindings(this List<IO2Finding> o2Findings)
        {
            var savedFile = new O2Assessment(o2Findings).save(new O2AssessmentSave_OunceV6()); 
            "Assessemnt File saved with {0} findings: {1}".info(o2Findings.Count, savedFile);
            return savedFile;
        }
        public static void saveFindings(this List<IO2Finding> o2Findings, string pathToSaveFindings)
        {
        	new O2Assessment(o2Findings).save(new O2AssessmentSave_OunceV6(), pathToSaveFindings); 
        	"Assessemnt File saved with {0} findings: {1}".info( o2Findings.Count, pathToSaveFindings);
        }

        public static List<IO2Finding> loadFindingsFiles(this List<string> filesToLoad)
        {
            var loadedFindings = new List<IO2Finding>();
            foreach (var fileToLoad in filesToLoad)
            {
                var o2Findings = loadFindingsFile(fileToLoad);
                loadedFindings.AddRange(o2Findings);
            }
            "Total # of findings loaded: {0}".info( loadedFindings);
            return loadedFindings;
        }


        // Description: this function loads multiple Ozasmt Files (not recursively)
        public static List<IO2Finding> loadMultipleOzasmtFiles(this string pathToOzastmFilesToLoad)
        {
            return loadMultipleOzasmtFiles(pathToOzastmFilesToLoad,"*.ozasmt",  false);
        }

        // Description: this function loads multiple Ozasmt Files (recursively or not, based on filter)
        public static List<IO2Finding> loadMultipleOzasmtFiles(this string pathToOzastmFilesToLoad, string filter, bool searchRecursively)
        {
            var o2Findings = new List<IO2Finding>();
            if (Directory.Exists(pathToOzastmFilesToLoad))
                foreach (var fileToLoad in Files.getFilesFromDir_returnFullPath(pathToOzastmFilesToLoad, filter, searchRecursively))
                {
                    "loading findings from file: {0}".info(fileToLoad);
                    o2Findings.AddRange(loadFindingsFile(fileToLoad));
                }
            return o2Findings;
        }
	}

	public static class Findings_ExtensionMethods_ascx_FindingsViewer
	{				
		public static ascx_TraceTreeView afterSelect_showTrace(this ascx_TraceTreeView traceViewer, ascx_FindingsViewer findingsViewer )
		{
			findingsViewer.afterSelect_showTrace(traceViewer);
			return traceViewer;
		}
		
		public static ascx_FindingsViewer afterSelect_showTrace(this ascx_FindingsViewer findingsViewer,  ascx_TraceTreeView traceViewer)
		{					
			findingsViewer._onFindingSelected += 
				(o2Finding)=>{
								traceViewer.show(o2Finding);
								Application.DoEvents();
								findingsViewer.focus();
							 };
			return findingsViewer;
		}
	
		public static ascx_FindingsViewer filter1_Text(this ascx_FindingsViewer findingsViewer, string filterText)
		{
			findingsViewer.setFilter1TextValue(filterText,true);
			return findingsViewer;
		}
		
		public static ascx_FindingsViewer filters(this ascx_FindingsViewer findingsViewer, string filter1Value, string filter2Value)
		{
			findingsViewer.filter2(filter2Value);
			findingsViewer.filter1(filter1Value);
			return findingsViewer;
		}
		
		public static ascx_FindingsViewer filter1(this ascx_FindingsViewer findingsViewer, string filterValue)
		{
			findingsViewer.setFilter1Value(filterValue,true);
			return findingsViewer;
		}
		
		public static ascx_FindingsViewer filter2(this ascx_FindingsViewer findingsViewer, string filterValue)
		{
			findingsViewer.setFilter2Value(filterValue);
			return findingsViewer;
		}
		
		public static ascx_FindingsViewer showTraces(this ascx_FindingsViewer findingsViewer)
		{
			findingsViewer.setTraceTreeViewVisibleStatus(true);
			return findingsViewer;
		}	
		
		public static ascx_FindingsViewer expand(this ascx_FindingsViewer findingsViewer)
		{
			findingsViewer.expandAllNodes();
			return findingsViewer;
		}
		
		public static List<TreeNode> findingsNodes(this ascx_FindingsViewer findingsViewer)
		{
			return findingsViewer.getResultsTreeView().nodes();
		}		
		
		
	}
	
	public static class Findings_ExtensionMethods_ascx_TraceTreeView
	{		
		public static ascx_TraceTreeView add_TraceViewer(this Control control)
		{
			return 	control.add_TraceViewer(false);	
		}
		
		public static ascx_TraceTreeView add_TraceViewer(this Control control, bool includeSourceCodeViewer)
		{
			return (ascx_TraceTreeView)control.invokeOnThread(
				()=>{
						var traceViewer = control.add_Control<ascx_TraceTreeView>();				
						if (includeSourceCodeViewer)
						{
							var codeViewer = traceViewer.insert_Right<Panel>(control.width()/2).add_SourceCodeViewer();
							traceViewer._onTraceSelected += 
								(trace)=> codeViewer.show(trace);
						}
						return traceViewer;						
					});
		}
						
			
		public static ascx_TraceTreeView show(this ascx_TraceTreeView traceViewer, IO2Finding iO2Finding)
		{
			traceViewer.loadO2Finding(iO2Finding);
			return traceViewer;
		}		
		
		public static ascx_TraceTreeView view_MethodName(this ascx_TraceTreeView traceViewer)
		{						
			return traceViewer.view_By("Method Name");
		}
		
		public static ascx_TraceTreeView view_Context(this ascx_TraceTreeView traceViewer)
		{						
			return traceViewer.view_By("Context");
		}
		
		public static ascx_TraceTreeView view_SourceCode(this ascx_TraceTreeView traceViewer)
		{						
			return traceViewer.view_By("Source Code");
		}
		
		public static ascx_TraceTreeView view_By(this ascx_TraceTreeView traceViewer, string radioButtonText)
		{
			traceViewer.invokeOnThread(
				()=>{
						foreach(var radioButton in traceViewer.controls<RadioButton>(true))
							if (radioButton.Text == radioButtonText)
								radioButton.Checked = true;
					});
			
			return traceViewer;
		}
		
		public static List<TreeNode> nodes(this ascx_TraceTreeView traceViewer)
		{						
			return O2Forms.getListWithAllNodesFromTreeView(traceViewer.controls<TreeView>().Nodes);
		}
		
		public static TreeNode firstNodeWithSourceCodeReference(this ascx_TraceTreeView traceViewer)
		{
			foreach(var node in traceViewer.nodes())
				if (node.Tag.notNull() && node.Tag is IO2Trace)
					if ((node.Tag as IO2Trace).file.valid())
						return node;
			return null;
		}
		
		public static ascx_TraceTreeView selectNodeWithText(this ascx_TraceTreeView traceViewer, string textToMatch)
		{
			textToMatch = textToMatch.trim();
			foreach(var node in traceViewer.nodes())
				if (node.get_Text().trim() == textToMatch)
				{
					node.selected();
					break;
				}
			return traceViewer;
		}
		
		public static ascx_TraceTreeView animate(this ascx_TraceTreeView traceViewer)
		{
			return traceViewer.animate(1000);
		}
		
		public static ascx_TraceTreeView animate(this ascx_TraceTreeView traceViewer, int delay)
		{
			foreach(var node in traceViewer.nodes())
			{
				node.selected();
				traceViewer.focus();
				traceViewer.sleep(delay,false);
			}
			return traceViewer;
		}
		
		//traceViewer.controls<RadioButton>(true)
		
		public static ascx_TraceTreeView dontSelectNodeOnLoad(this ascx_TraceTreeView traceViewer, bool value = true)
		{
			traceViewer.DontSelectNodeOnLoad = value;
			return traceViewer;
		}
	}

	public static class Findings_ExtensionMethods_ascx_SourceCodeViewer
	{
		public static ascx_SourceCodeViewer show(this ascx_SourceCodeViewer codeViewer, IO2Finding o2Finding)
		{
			codeViewer.editor().show(o2Finding);
			return codeViewer;
		}
		public static ascx_SourceCodeEditor show(this ascx_SourceCodeEditor codeEditor, IO2Finding o2Finding)
		{								
			"in show".info();
			codeEditor.open(o2Finding.file);
			if (o2Finding.lineNumber > 0)
			{
				codeEditor.gotoLine((int)o2Finding.lineNumber-1);
				//codeViewer.editor().caret_Line();
				codeEditor.caret_Line((int)o2Finding.lineNumber-1);
				codeEditor.caret_Column((int)o2Finding.columnNumber);
			}
			return codeEditor;
		}
		public static ascx_SourceCodeViewer show(this ascx_SourceCodeViewer codeViewer, IO2Trace o2Trace)
		{
			codeViewer.editor().show(o2Trace);
			return codeViewer;
		}
		
		public static ascx_SourceCodeEditor show(this ascx_SourceCodeEditor codeEditor, IO2Trace o2Trace)
		{											
			codeEditor.open(o2Trace.file);
			if (o2Trace.lineNumber > 0)
			{
				codeEditor.editor().gotoLine((int)o2Trace.lineNumber-1);
				//codeViewer.editor().caret_Line();
				codeEditor.editor().caret_Line((int)o2Trace.lineNumber-1);
				codeEditor.editor().caret_Column((int)o2Trace.columnNumber);
			}
			return codeEditor;
		}

		
		public static ascx_FindingsViewer afterSelect_ShowTraceInCodeViewer(this ascx_FindingsViewer findingsViewer , ascx_SourceCodeViewer codeViewer)
		{
			findingsViewer._onFindingSelected += 
				(o2Finding)=>{
								codeViewer.show(o2Finding);
								Application.DoEvents();
								findingsViewer.focus();
							 };
			findingsViewer._onTraceSelected += 
				(o2Trace)=>{
								codeViewer.show(o2Trace);
								Application.DoEvents();
								findingsViewer.focus();
							};
			return findingsViewer;
		}
		
		public static ascx_TraceTreeView afterSelect_ShowTraceInCodeViewer(this ascx_TraceTreeView traceViewer , ascx_SourceCodeViewer codeViewer)
		{
			return traceViewer.afterSelect_ShowTraceInCodeEditor(codeViewer.editor());
		}
		
		public static ascx_TraceTreeView afterSelect_ShowTraceInCodeEditor(this ascx_TraceTreeView traceViewer , ascx_SourceCodeEditor codeEditor)
		{
			traceViewer._onTraceSelected += 
				(o2Trace)=>{
								codeEditor.show(o2Trace);
								Application.DoEvents();
								traceViewer.focus();
						   };
			return traceViewer;
		}
		
	}	
	
	public static class Findings_ExtensionMethods_VulnName_VulnType
	{	
		public static List<IO2Finding> set_VulnNameAndType(this List<IO2Finding> o2Findings, string vulnName, string vulnType)
        {
            foreach (var o2Finding in o2Findings)
            {
                o2Finding.vulnName = vulnName;
                o2Finding.vulnType = vulnType;
            }
            return o2Findings;
        }
        public static List<IO2Finding> set_VulnName(this List<IO2Finding> o2Findings, string vulnName)
        {
            foreach (var o2Finding in o2Findings)
                o2Finding.vulnName = vulnName;
            return o2Findings;
        }

        public static List<IO2Finding> set_VulnType(this List<IO2Finding> o2Findings, string vulnType)
        {
            foreach (var o2Finding in o2Findings)
                o2Finding.vulnType = vulnType;
            return o2Findings;
        }

		public static List<IO2Finding> set_VulnName_onSink(this List<IO2Finding> o2Findings, string sinkToFind, string vulnName)
        {
            var results = new List<IO2Finding>();
            foreach (var iO2Finding in o2Findings)
            {
                var o2Finding = (O2Finding)iO2Finding;
                if ((o2Finding).Sink.contains(sinkToFind))
                {
                    o2Finding.vulnName = vulnName;
                    results.add(o2Finding);
                }
            }
            return results;
        }

	}

	public static class Findings_ExtensionMethods_Filtering
	{
		//[XRule(Name="All findings")]
        public static  List<IO2Finding> allFindings(this List<IO2Finding> o2Findings)
        {        	
            return o2Findings;        	
        }

        //[XRule(Name="Only Findings With Traces")]
        public static List<IO2Finding> withTraces(this List<IO2Finding> o2Findings)
        {
        	return o2Findings.onlyTraces();
        }
        
        public static List<IO2Finding> onlyTraces(this List<IO2Finding> o2Findings)
        {        	
            return (from IO2Finding o2Finding in o2Findings 
                 	where o2Finding.o2Traces.Count > 0  select o2Finding).ToList();
            //return o2Assesment.o2Findings;
        }

		
        //[XRule(Name="Only.findings.where.vulnName.CONTAINS")]
        public static  List<IO2Finding> whereVulnName_Contains(this List<IO2Finding> o2Findings, string text)
        {        	
            return 
                (from IO2Finding o2Finding in o2Findings
                 where o2Finding.vulnName.IndexOf(text) > -1
                 select o2Finding).ToList();            
        }

        //[XRule(Name = "Only.findings.where.vulnName.IS")]
        public static  List<IO2Finding> whereVulnName_Is(this List<IO2Finding> o2Findings, string text)
        {
            return
                (from IO2Finding o2Finding in o2Findings
                 where o2Finding.vulnName == text
                 select o2Finding).ToList();
        }

        //[XRule(Name = "Only.findings.where.Source.IS")]
        public static  List<IO2Finding> whereSource_Is(this List<IO2Finding> o2Findings, string text)
        {
            return
                (from O2Finding o2Finding in o2Findings
                 where o2Finding.Source == text 
                 select (IO2Finding)o2Finding).ToList();
        }

        //[XRule(Name = "Only.findings.where.Source.CONTAINS")]
        public static  List<IO2Finding> whereSource_Contains(this List<IO2Finding> o2Findings, string text)
        {
            return
                (from O2Finding o2Finding in o2Findings
                 where o2Finding.Source.IndexOf(text) > -1
                 select (IO2Finding)o2Finding).ToList();
        }

        //[XRule(Name = "Only.findings.where.Sink.IS")]
        public static  List<IO2Finding> whereSink_Is(this List<IO2Finding> o2Findings, string text)
        {
            return
                (from O2Finding o2Finding in o2Findings
                 where o2Finding.Sink == text
                 select (IO2Finding)o2Finding).ToList();
        }

        //[XRule(Name = "Only.findings.where.Sink.CONTAINS")]
        public static  List<IO2Finding> whereSink_Contains(this List<IO2Finding> o2Findings, string text)
        {
            return
                (from O2Finding o2Finding in o2Findings
                 where o2Finding.Sink.IndexOf(text) > -1
                 select (IO2Finding)o2Finding).ToList();
        }
        
        public static List<IO2Finding> whereSink_Starts(this List<IO2Finding> o2Findings, string text)
        {
            var matches = from o2Finding in o2Findings
                          where ((O2Finding)o2Finding).Sink.starts(text)
                          select o2Finding;
            return matches.toList();
        }

        //[XRule(Name = "Only.findings.where.Context.CONTAINS")]
        public static  List<IO2Finding> whereContext_Contains(this List<IO2Finding> o2Findings, string text)
        {
            return
                (from O2Finding o2Finding in o2Findings
                 where o2Finding.context.IndexOf(text) > -1
                 select (IO2Finding)o2Finding).ToList();
        }
        
		public static List<IO2Finding> whereTracesSourceCode_Contains(this List<IO2Finding> o2Findings, string text)
		{
		return(from o2Finding in o2Findings
                   where o2Finding.tracesSourceCode_Contains(text)
                   select (IO2Finding)o2Finding).ToList();			
		}
		
		public static bool tracesSourceCode_Contains(this IO2Finding iO2Finding, string text)
		{
			return iO2Finding.allTraces_SourceCode().filter(text).size() > 0;
		}
		
		
		public static List<IO2Finding> whereTraces_Contains(this List<IO2Finding> o2Findings, string text)
        {
            return(from O2Finding o2Finding in o2Findings
                   where o2Finding.trace_Contains(text)
                   select (IO2Finding)o2Finding).ToList();
        }
        
		public static bool trace_Contains(this IO2Finding iO2Finding, string text)
		{
			return iO2Finding.allTraces().values().filter(text).size() > 0;
		}
		
		
        //[XRule(Name = "Only.findings.where.SourceAndSink.CONTAINS.Regex")]
        /*(public static  List<IO2Finding> whereSourceAndSink_ContainsRegex(List<IO2Finding> o2Findings, string source, string sink )
        {
            return o2Findings.calculateFindings(source, sink);
        }*/
        
        public static Dictionary<string,List<KeyValuePair<IO2Finding, IO2Trace>>> filterBy_Traces_SourceCode(this List<IO2Finding> iO2Findings)
        {
        	var propertyName = "SourceCode";
        	return iO2Findings.filterBy_Traces_Property(propertyName);
        }
        
        public static Dictionary<string,List<KeyValuePair<IO2Finding, IO2Trace>>> filterBy_Traces_Property(this List<IO2Finding> iO2Findings, string propertyName)
        {
        	var filteredData = new Dictionary<string,List<KeyValuePair<IO2Finding, IO2Trace>>>();
        	foreach(var iO2Finding in iO2Findings)
        		foreach(var iO2Trace in iO2Finding.allTraces())
        		{
        			var propertyValue = (iO2Trace as O2Trace).prop(propertyName).str();
        			if (propertyValue.valid())
        			{
        				var keyValuePair = new KeyValuePair<IO2Finding, IO2Trace>(iO2Finding, iO2Trace);        			
        				filteredData.add(propertyValue.trim(), keyValuePair);        				
        			}        			
        		}        
        	return filteredData;
        }	
	
		public static List<IO2Finding> o2Findings(this List<KeyValuePair<IO2Finding, IO2Trace>> list)
		{
			var o2Findings = new List<IO2Finding>();
			foreach(var item in list)
				o2Findings.add(item.Key);
			return o2Findings;
		}
	}
		
	public static class Findings_ExtensionMethods_Joining
	{
		public static IO2Finding copy(this IO2Finding o2Finding)
		{
			return OzasmtCopy.createCopy(o2Finding);
		}
		
		public static Dictionary<string,List<IO2Finding>> indexBy(this List<IO2Finding> o2Findings, Func<O2Finding,string> calculateIndex)
		{
			var indexedData = new Dictionary<string,List<IO2Finding>>();
			foreach(var o2Finding in o2Findings)
				indexedData.add(calculateIndex((O2Finding)o2Finding),o2Finding);
			return indexedData;
		}
		
		public static Dictionary<string,List<IO2Finding>> indexBy(this List<IO2Finding> o2Findings, string propertyToIndexBy)
		{
			var indexedData = new Dictionary<string,List<IO2Finding>>();
			foreach(var o2Finding in o2Findings)
				indexedData.add(o2Finding.prop(propertyToIndexBy).str(),o2Finding);
			return indexedData;
		}
		
		public static Dictionary<string,List<IO2Finding>> indexBy_Sink(this List<IO2Finding> o2Findings)
		{
			return o2Findings.indexBy("Sink");
		}
		
		public static Dictionary<string,List<IO2Finding>> indexBy_Source(this List<IO2Finding> o2Findings)
		{
			return o2Findings.indexBy("Source");
		}
		
		public static List<IO2Trace> pathToSink(this IO2Finding o2Finding)
		{
			var pathToTraceType = new List<IO2Trace>();
    		OzasmtUtils.getPathToTraceType(o2Finding.o2Traces, TraceType.Known_Sink, pathToTraceType);
    		return pathToTraceType;
		}
		public static IO2Trace firstThatStartsWith(this List<IO2Trace> o2Traces, string text)
		{
			foreach(var o2Trace in o2Traces)
				if (o2Trace.str().starts(text))
					return o2Trace;
			return null;
		}
		
	}
	
	
	
	public static class Findings_ExtensionMethods_FindingsBatchReplace
	{
		public static List<IO2Finding> replaceFilePathsInFile(this string findingsFile, string pathToFind, string pathToReplace)
		{
			var o2Findings = findingsFile.loadO2Findings();
			o2Findings.replaceFilePathsInFile(pathToFind, pathToReplace);
			o2Findings.saveFindings(findingsFile);			
			return o2Findings;
		}
		
		public static List<IO2Finding> replaceFilePathsInFile(this List<IO2Finding> o2Findings, string pathToFind, string pathToReplace)
		{
			foreach(var o2Finding in o2Findings)
			{
				o2Finding.file = o2Finding.file.replace(pathToFind,pathToReplace);			
				var allTraces = o2Finding.allTraces();
				foreach(var o2Trace in allTraces)
					o2Trace.file = o2Trace.file.replace(pathToFind,pathToReplace);			
			}
			return o2Findings;
		}
	}
	
	public static class Findings_ExtensionMethods_FindingsTrasformation
	{
		public static List<IO2Finding> removeFirstSource(this List<IO2Finding> o2Findings)
		{
			foreach(O2Finding o2Finding in o2Findings)
				o2Finding.getSource().traceType = TraceType.Type_4;
			return o2Findings;	
		}
	}
	
	public static class Findings_ExtensionMethods_DotNet_FindingsTransformations
	{
		public static List<IO2Finding> getFindingsWith_WebServicesInvoke(this List<IO2Finding> o2Findings)
		{
			var webServicesInvokeMethod = "method -> protected object[] System.Web.Services.Protocols.SoapHttpClientProtocol.Invoke(string methodName, object[] parameters)";			
			return o2Findings.whereSink_Is(webServicesInvokeMethod);
		}
			
		public static List<IO2Finding> makeSinks_WebServicesInvokeTarget(this List<IO2Finding> o2Findings)
		{			
			foreach(O2Finding o2Finding in o2Findings)
			{				
				var pathToSink = o2Finding.pathToSink(); 
				if (pathToSink.size() > 0)
				{
					pathToSink.RemoveAt(0);					
					var firstMatch = pathToSink.firstThatStartsWith("method");					
						if(firstMatch.notNull())
						{
							o2Finding.getSink().traceType = TraceType.Lost_Sink; 
							firstMatch.traceType = TraceType.Known_Sink;
						}
				}
			}
			return o2Findings;
		}
	}
}
