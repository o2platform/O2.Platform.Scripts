// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using FluentSharp.XObjects;
using https.www.veracode.com.schema.reports.export.Item1.Item0;
//O2File:detailedreport.cs

//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll


namespace O2.XRules.Database.APIs
{
	public class API_Veracode_DetailedXmlFinding_Test
	{
		public API_Veracode_DetailedXmlFindings launch()
		{
			var apiVeracode = new API_Veracode_DetailedXmlFindings(); 
			apiVeracode.add_MultipleGuiViewers();
			return apiVeracode;
		}
	}
	

    public class API_Veracode_DetailedXmlFindings
    {    	
    	public string ReportXmlFile { get; set;}
    	public string WorkDir { get; set;}
    	public List<string> SourceCodePaths { get; set;}
    	public List<string> FilesThatCouldNotMappedLocally { get;set;}
    	public Dictionary<string, string> LocalFileMappingsCache { get; set; }
    	public string schemaName = "https://www.veracode.com/schema/reports/export/1.0";
    	
    	public API_Veracode_DetailedXmlFindings()
    	{
    		SourceCodePaths = new List<string>();
    		FilesThatCouldNotMappedLocally = new List<string>();
    		LocalFileMappingsCache = new Dictionary<string, string> ();
    	}
    	
    	public detailedreport DetailedReport  { get; set; }	
    	    	
    }
    
    public static class API_Veracode_DetailedXmlFindings_ExtensionMethod_Load
    {
    	public static API_Veracode_DetailedXmlFindings load(this API_Veracode_DetailedXmlFindings apiVeracode, string file)
    	{
    		"[API_Veracode_DetailedXmlFindings]: loading file: {0}".info(file);
    		if (file.extension(".xml"))
    			apiVeracode.ReportXmlFile = file;
    		else 
    			if (file.extension(".zip"))   		
	    		{
	    			var unzipedFiles = file.unzip_FileAndReturnListOfUnzipedFiles();
	    								//(apiVeracode.WorkDir.isNull())
	    								//	? 
	    								//	: file.unzip_FileAndReturnListOfUnzipedFiles(apiVeracode.WorkDir);;
	    			if (unzipedFiles.size()!=2 || unzipedFiles[1].extension(".xml").isFalse())    			
	    				"unexpected {0} files in zipfile: {0}".error(unzipedFiles.size(),unzipedFiles[1]);    				
	    			else    			
	    				if (apiVeracode.WorkDir.isNull())
	    					apiVeracode.ReportXmlFile = unzipedFiles[1];	
	    				else
	    					apiVeracode.ReportXmlFile = Files.copy(unzipedFiles[1],apiVeracode.WorkDir);	
	    				
	    				apiVeracode.DetailedReport = detailedreport.Load(apiVeracode.ReportXmlFile);	    				
	    		}    		
    			else
    					"unsupported file extension: {0}".error(file);			
    		return apiVeracode.loadFromXmlFile(apiVeracode.ReportXmlFile);
    	}
    	
		public static API_Veracode_DetailedXmlFindings loadFromXmlFile(this API_Veracode_DetailedXmlFindings apiVeracode, string xmlFile)    	    	    
		{
			if (xmlFile.inValid())
			{
				"[API_Veracode_DetailedXmlFindings] in loadFromXmlFile, there was no xmlFile provided".error();
				return apiVeracode;
			}
			apiVeracode.DetailedReport = detailedreport.Load(xmlFile);
			if(apiVeracode.DetailedReport.isNull())
				"[API_Veracode_DetailedXmlFindings] in loadFromXmlFile, failed to create serialized detailedreport from file: {0}".error(xmlFile);
			else
			{	
				apiVeracode.LocalFileMappingsCache.Clear();
				"Loaded Ok file {0} which had {1} flaws".debug(xmlFile.fileName(), apiVeracode.flaws().size());
			}
			return apiVeracode;
		}
    }
    public static class API_Veracode_DetailedXmlFindings_ExtensionMethod_Flaw
    {
    	public static string sourceCodeFile(this FlawType flaw)
    	{
    		return "{0}{1}".format(flaw.sourcefilepath, flaw.sourcefile);
    	}
    	
    	public static string sourceCodeFile(this API_Veracode_DetailedXmlFindings apiVeracode, FlawType flaw)
    	{
    		var sourceCodeFile = flaw.sourceCodeFile();
    		if (apiVeracode.LocalFileMappingsCache.hasKey(sourceCodeFile))
    			return apiVeracode.LocalFileMappingsCache[sourceCodeFile];
    			
    		foreach(var path in apiVeracode.SourceCodePaths)
    		{
    			var sourceCodePath = path.pathCombine(sourceCodeFile);
    			if (sourceCodePath.fileExists())
    			{
    				apiVeracode.LocalFileMappingsCache.add(sourceCodeFile, sourceCodePath);
    				return sourceCodePath;
    			}
    		}
    		apiVeracode.FilesThatCouldNotMappedLocally.add_If_Not_There(sourceCodeFile);    		
    		apiVeracode.LocalFileMappingsCache.add(sourceCodeFile, sourceCodeFile);
    		return sourceCodeFile;
    	}
    	
    	public static bool hasLocalSourceCodeFile(this API_Veracode_DetailedXmlFindings apiVeracode, FlawType flaw)
    	{
    		return apiVeracode.sourceCodeFile(flaw).fileExists(); 
    	}
    }
    
    public static class API_Veracode_DetailedXmlFindings_ExtensionMethods_Linq_Queries
    {
    	public static List<FlawType> flaws(this API_Veracode_DetailedXmlFindings apiVeracode)
    	{
    		if(apiVeracode.DetailedReport.isNull())
    			return new List<FlawType>();
    			
    		var flaws = from severity in apiVeracode.DetailedReport.severity
				        from category in severity.category 	        
				        from cwe in category.cwe 
				        from flaw in cwe.staticflaws.flaw 	        				        
				        select flaw;
			return flaws.toList();
    	}
    	
    	public static List<FlawType> @fixed(this List<FlawType> flaws)
    	{
    		return (from flaw in flaws
    				where flaw.remediation_status == "Fixed"
    				select flaw).toList();
    	}
    	
    	public static List<FlawType> notFixed(this List<FlawType> flaws)
    	{
    		return (from flaw in flaws
    				where flaw.remediation_status != "fixed"
    				select flaw).toList();
    	}
    }
    

	public static class API_Veracode_DetailedXmlFindings_ExtensionMethod_XSD
	{
		public static string createSharpFileFromXSD(this API_Veracode_DetailedXmlFindings apiVeracode, string xsdFile)
    	{
    		return apiVeracode.createSharpFileFromXSD(xsdFile, apiVeracode.WorkDir);
    	}
    	
    	public static string createSharpFileFromXSD(this API_Veracode_DetailedXmlFindings apiVeracode, string xsdFile, string targetDir)
    	{    		
			var cSharpFile = xsdFile.xsdCreateCSharpFile()						
				        .fileInsertAt(0,"//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll".line());
			if (cSharpFile.fileExists())
			{
				if (targetDir.valid())
					return Files.copy(cSharpFile, targetDir);
				return cSharpFile;
			}
			return null;
    	}    
    }
    public static class API_Veracode_DetailedXmlFindings_ExtensionMethod_GuiHelpers
    {	
    	public static API_Veracode_DetailedXmlFindings add_MultipleGuiViewers(this API_Veracode_DetailedXmlFindings apiVeracode)
    	{
    		return apiVeracode.add_MultipleGuiViewers("Tool - View Veracode Detailed Findings reports".showAsForm());		
    	}
    
    	public static API_Veracode_DetailedXmlFindings add_MultipleGuiViewers(this API_Veracode_DetailedXmlFindings apiVeracode , Control control)
    	{
    		var topPanel = control.clear();
    		topPanel.insert_LogViewer();
    		var viewersPanel = topPanel.insert_Left(200,"Viewers");

			var viewInTreeView = viewersPanel.add_Link("View in TreeView",0, 0, ()=> apiVeracode.show_In_TreeView(topPanel)); 
			var viewInTableList = viewersPanel.add_Link("View in TableList",20, 0, ()=> apiVeracode.show_In_TableList(topPanel));  
			var viewInSourceCodeViewer = viewersPanel.add_Link("View in SourceCodeViewer",40, 0, ()=> apiVeracode.show_Flaws_In_SourceCodeViewer(topPanel));
			
			var sourceCodePaths = viewersPanel.insert_Below("Source Code Paths").add_TextArea();
			sourceCodePaths.onTextChange(
				(text)=>{
							apiVeracode.SourceCodePaths.Clear();
							apiVeracode.SourceCodePaths.AddRange(text.split_onLines().ToArray());
						});
				
			viewersPanel.onDrop(
				(file)=>{
							if (file.fileExists())
							{										
								apiVeracode.load(file);   
								viewInSourceCodeViewer.click();
							}
						});
			
			viewInSourceCodeViewer.click();
			
			
			viewersPanel.add_Label("DROP XML FILE HERE to load it", 100,0).font_bold(); 
			return apiVeracode;
		}
    
    	public static ctrl_TableList show_In_TableList(this List<FlawType> flaws)
    	{
    		return flaws.show_In_TableList("TreeView of Veracode Xml File".showAsForm());		
    	}
    
    	public static ctrl_TableList show_In_TableList(this API_Veracode_DetailedXmlFindings apiVeracode , Control control)
    	{
    		return apiVeracode.flaws().show_In_TableList(control);
    	}
    	
    	public static ctrl_TableList show_In_TableList(this List<FlawType> flaws , Control control)
    	{    	
    		control.clear();
    		var tableList = control.add_TableList();
    		Action showData = 
    			()=>{
		    			
						var selectedRows =  from flaw in flaws
											select new {flaw.severity, flaw.categoryname, flaw.issueid,
														flaw.module, flaw.type, flaw.description, flaw.cweid,  
														flaw.exploitLevel, flaw.categoryid, 
														flaw.sourcefile, flaw.line,  flaw.sourcefilepath,
														flaw.scope, flaw.functionprototype, flaw.functionrelativelocation};
				  
		
						tableList.show(selectedRows);
						tableList.makeColumnWidthMatchCellWidth();
					};
			tableList.onDrop(
				(file)=>{
							var apiVeracode = new API_Veracode_DetailedXmlFindings().load(file);
							flaws = apiVeracode.flaws();
							showData();
						});
			if (flaws.size()>0)
				showData(); 
			else
				tableList.add_Column("note")
						 .add_Row("drop a Veracode DetailedFindings Xml (or zip) file to view it")
						 .makeColumnWidthMatchCellWidth();
				
			return tableList;
		}
    
    	public static API_Veracode_DetailedXmlFindings show_In_TreeView(this API_Veracode_DetailedXmlFindings apiVeracode)
    	{
    		return apiVeracode.show_In_TreeView("TreeView of Veracode Xml File".showAsForm());
    	}
    	
    	public static API_Veracode_DetailedXmlFindings show_In_TreeView(this API_Veracode_DetailedXmlFindings apiVeracode, Control control)
    	{
    		var treeView = control.clear().add_TreeView();
    		Action<TreeNode,string> removeSchemaReference = 
				(treeNode, textToRemove) => {
												treeNode.set_Text(treeNode.get_Text().remove(textToRemove));					
											};
			treeView.afterSelect(
				(treeNode)=>{
								removeSchemaReference(treeNode,"{" + apiVeracode.schemaName + "}"); 
							});
			
			Action<string> loadFile = 
				(file) =>{
								apiVeracode.load(file);
								treeView.rootNode().showXml(apiVeracode.ReportXmlFile); 			
							};
			treeView.onDrop(
				(fileOrFolder)=>{
									treeView.azure();
									O2Thread.mtaThread(
										()=>{									
												if (fileOrFolder.dirExists())						
													foreach(var file in fileOrFolder.files(true,"*.xml", "*.zip"))
														try
														{
															loadFile(file);
														}
														catch(Exception ex)
														{
															"error loading file: {0} : {1}".error(file.fileName(), ex.Message);
														}
												else
													loadFile(fileOrFolder);
												treeView.white();
											});
						});
						
			if (apiVeracode.ReportXmlFile.valid())
				treeView.rootNode().showXml(apiVeracode.ReportXmlFile); 
			else
				treeView.rootNode().add_Node("drop a Veracode DetailedFindings Xml (or zip) file to view it"); 
			
			treeView.selectFirst().expand();			
			return apiVeracode;
    	}
    		
    	
    	public static API_Veracode_DetailedXmlFindings show_Flaws_In_SourceCodeViewer(this API_Veracode_DetailedXmlFindings apiVeracode, Control control)
    	{
    		var topPanel = control.clear();
    		var codeViewer = topPanel.add_GroupBox("Flaw SourceCode reference").add_SourceCodeViewer();
			var treeView = codeViewer.parent().insert_Left("Flaws").add_TreeView(); 
			var propertyGrid = treeView.insert_Below(150,"Flaw properties")
									   .add_PropertyGrid().helpVisible(false);
			var description = codeViewer.insert_Below(150,"Flaw description")
									    .add_TextArea();
									    
			treeView.afterSelect<FlawType>(
				(flaw)=>{				
							propertyGrid.show(flaw);				
							if (apiVeracode.hasLocalSourceCodeFile(flaw))
							{
								codeViewer.open(apiVeracode.sourceCodeFile(flaw));
								codeViewer.editor().gotoLine((int)flaw.line); 
							}
							else
								codeViewer.set_Text(".. no source code available...");
							description.set_Text(flaw.description.fix_CRLF());
							treeView.focus();
						});
			 												
			treeView.beforeExpand<List<FlawType>>(
				(flaws)=>{
							var selectedNode = treeView.selected();
							if (selectedNode.nodes().size()== 1)
							{
								selectedNode.clear();
								selectedNode.add_Nodes(flaws, 
													   (flaw)=> flaw.type, 
													   (flaw) => apiVeracode.hasLocalSourceCodeFile(flaw)
																		? Color.DarkGreen
																		: Color.DarkRed
													   );
							}
						 });
										
			Action<TreeNode, Dictionary<string, List<FlawType>>> addFlawsToTreeNode =
				(treeNode, mappedFlaws) 
				   =>{
						foreach(var item in mappedFlaws)			
							treeNode.add_Node(item.Key,item.Value,item.Value.size()>0);
									/*.*/
					  };
					  
			Action showData = 		  
				()=>{
						treeView.clear();
						var o2Timer = new O2Timer("Building XRefs for flaws").start();
						var mappedFlawsByType = new Dictionary<string, List<FlawType>>();
						var mappedFlawsByCategoryName = new Dictionary<string, List<FlawType>>();
						var mappedFlawsByFile = new Dictionary<string, List<FlawType>>();
						var mappedFlawsBySeverity = new Dictionary<string, List<FlawType>>();
						
						foreach(var flaw in apiVeracode.flaws()) 
						{
							mappedFlawsByCategoryName.add(flaw.categoryname, flaw);
							mappedFlawsByType.add(flaw.type, flaw);
							mappedFlawsByFile.add(flaw.sourceCodeFile(), flaw);
							mappedFlawsBySeverity.add(flaw.severity.str(), flaw);	
						}
						o2Timer.stop();
						o2Timer = new O2Timer("Populating treeview").start();			
						addFlawsToTreeNode(treeView.add_Node("by Category Name"), mappedFlawsByCategoryName);
						addFlawsToTreeNode(treeView.add_Node("by Type"), mappedFlawsByType);
						addFlawsToTreeNode(treeView.add_Node("by File"), mappedFlawsByFile);
						addFlawsToTreeNode(treeView.add_Node("by Severity"),mappedFlawsBySeverity);
						o2Timer.stop();		
					};
			
			treeView.onDrop(
				(file)=>{
							apiVeracode.load(file);
							showData(); 			
						});
			if (apiVeracode.ReportXmlFile.valid())
				showData(); 
			else
				treeView.add_Node("drop a Veracode DetailedFindings Xml (or zip) file to view it"); 			
			
			
//			"There were {0} Files That Could Not Mapped Locally".error(apiVeracode.FilesThatCouldNotMappedLocally.size());			
			/*if (treeView.nodes()>0))
			{
				treeView.nodes()[0]
					 	.expand().nodes()[2].selected();
			}
			*/
			return apiVeracode;
    	}
    }
    
}