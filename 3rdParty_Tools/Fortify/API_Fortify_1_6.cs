// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.XRules.Database.Utils;
using xmlns.www.fortifysoftware.com.schema.fvdl;
//O2File:Fortify.fvdl.1.6.xsd.cs
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll
//O2File:_Extra_methods_Items.cs
//O2File:_Extra_methods_Collections.cs 

namespace O2.XRules.Database.APIs
{
	public class Fortify_Scan
	{
		public FVDL _fvdl;
		
		public string BuildID { get; set; }
		public uint Loc { get; set; }
		public string SourceBasePath { get; set; }
		public uint ScanTime { get; set; }		
		public DateTime CreatedDateTime { get; set; } 
		public string Errors { get; set; }
		public List<Fortify_ScannedFile> ScannedFiles { get; set; }	
		public List<Fortify_Context> Contexts { get; set; }
		public List<Fortify_Description> Descriptions { get; set; }
		public List<Fortify_CalledWithNoDef> CalledWithNoDefs { get; set; }		
		public List<Fortify_Sink> Sinks { get; set; }
		public List<Fortify_Source> Sources { get; set; }
		public List<Fortify_Snippet> Snippets { get; set; }
		public List<Fortify_Vulnerability> Vulnerabilities { get; set; }
		
		public Fortify_Scan()
		{
			ScannedFiles = new List<Fortify_ScannedFile>();
			Contexts = new List<Fortify_Context>();
			Descriptions = new List<Fortify_Description>();
			CalledWithNoDefs = new List<Fortify_CalledWithNoDef>();
			Sinks = new List<Fortify_Sink>();
			Sources = new List<Fortify_Source>();
			Snippets = new List<Fortify_Snippet>();
			Vulnerabilities = new List<Fortify_Vulnerability>();
		}
	}		
	
	public class Fortify_ScannedFile
	{
		public uint Loc { get; set; }
		public uint Size { get; set; }
		public ulong Timestamp { get; set; }
		public string Type { get; set; }
		public string Path { get; set; }
	}
	public class Fortify_Context
	{
		public string Id { get; set; }
		public Fortify_Function Function { get; set; }
	}	
	public class Fortify_Function
	{
		public string FunctionName { get; set; }
		public Fortify_CodeLocation CodeLocation { get; set; }	
		
		public Fortify_Function()
		{}
				
		public Fortify_Function(string functionName, object location)
		{
			FunctionName =  functionName;	
			CodeLocation = new Fortify_CodeLocation(location);
		}
		
		public override string ToString()
		{
			return "{{ name: {0} , path: {1} , line: {2}, lineEnd: {3}, colStart: {4}, colEnd: {5} }}"
						.format(FunctionName, CodeLocation.Path, CodeLocation.Line, CodeLocation.LineEnd, CodeLocation.ColStart, CodeLocation.ColEnd);
		}
	}	
	public class Fortify_CodeLocation
	{
		public string Path { get; set; }
		public uint Line { get; set; }
		public uint LineEnd { get; set; }
		public uint ColStart { get; set; }
		public uint ColEnd { get; set; }
		
		public Fortify_CodeLocation()
		{}
		
		public Fortify_CodeLocation(object location)
		{									
			Path = (string)location.prop("path");															
			Line = (uint)location.prop("line");
			LineEnd = (uint)location.prop("lineEnd");
			ColStart = UInt16.Parse(location.prop("colStart").str());
			ColEnd = UInt16.Parse(location.prop("colEnd").str());														
		}
		public override string ToString()
		{
			if (ColStart == 0 && ColEnd==0)
				return "{{ path: {0} , line: {1}, lineEnd: {2}}}".format(Path, Line, LineEnd);
				
			return "{{ path: {0} , line: {1}, lineEnd: {2}, colStart: {3}, colEnd: {4} }}"
						.format(Path, Line, LineEnd, ColStart, ColEnd);
		}
	}	
	public class Fortify_Description
	{
		public string Abstract { get; set; }
		public string ClassID { get; set; }
		public string ContentType { get; set; }
		public string Explanation { get; set; }
		public string Recommendations { get; set; }
		public List<string> Tips { get; set; }		
		
		public string Tips_as_String { get { return Tips.join(" , ");} } //viewer helper
		
		public Fortify_Description()
		{
			Tips = new List<string>();
		}
		
	}	
	
	public class Fortify_CalledWithNoDef
	{
		public string Name { get; set; }
		public string Namespace { get; set; }
		public string EnclosingClass { get; set; }
	}
	
	public class Fortify_Sink
	{
		public string RuleID { get; set; }
		public Fortify_Function Function_Call{ get; set;}				
	}
	public class Fortify_Source
	{
		public string RuleID { get; set; }
		public Fortify_Function Function_Call{ get; set;}
		public Fortify_Function Function_Entry{ get; set;}				
		public List<string> TaintFlags { get; set;}
		
		public string TaintFlags_as_String { get { return TaintFlags.join(" , ");} } //viewer helper
		
		public Fortify_Source()
		{
			TaintFlags = new List<string>();
		}
	}		
	public class Fortify_Snippet
	{
		public string Id { get; set; }
		public Fortify_CodeLocation CodeLocation { get; set; }		
		public string Text { get; set;}		
	}
	
	public class Fortify_ReplacementDefinitions
	{
		public Items Definitions { get; set; }
		//public List<Fortify_Function> LocationDefinitions { get; set; }  // add later if needed (Fortify_Function is not the best since the LocationDef has a 'key'  value
		
		public Fortify_ReplacementDefinitions()
		{
			Definitions = new Items();			
		}
		public override string ToString()
		{	
			return "{0} definitions".format(Definitions.size());
		}
	}
	
	public class Fortify_TraceEntry
	{
		public uint NodeRefId 								{ get; set; }	// when this one is set, the ones below are not set						
		public string ActionType 							{ get; set; }
		public string ActionValue 							{ get; set; }
		public bool DetailsOnly 							{ get; set; }
		public bool IsDefault 								{ get; set; }		
		public List<Fortify_TraceEntryFact> KnowledgeFacts	{ get; set; }
		public string Reason_RuleId							{ get; set; }
		public string Reason_TraceRef						{ get; set; }		
		public string Reason_Internal						{ get; set; }		
		public Fortify_CodeLocation SourceLocation 			{ get; set; }		
		public Fortify_CodeLocation SecundaryLocation 		{ get; set; }
		public uint SourceLocation_ContextId 				{ get; set; }
		public string SourceLocation_Snippet 				{ get; set; }		
		public string SecundaryLocation_Snippet				{ get; set; }				
		public string Label									{ get; set; }				
		
		public Fortify_TraceEntry()
		{
			KnowledgeFacts = new List<Fortify_TraceEntryFact>();			
		}
	}
	public class Fortify_TraceEntryFact
	{
		public bool Primary {get;set;}
		public string Type {get;set;}
		public string Value {get;set;}
	}	
	
	public class Fortify_Vulnerability
	{
		public Fortify_Function Context { get; set; }	// from AnalysisInfo
		public Fortify_ReplacementDefinitions ReplacementDefinitions { get; set; }
		public List<Fortify_TraceEntry> Traces { get; set; }
		public string Kingdom { get; set; }				// from ClassInfo
		public string AnalyzerName { get; set; }
		public string ClassId { get; set; }
		public decimal DefaultSeverity { get; set; }
		public string  Type { get; set; }
		public string  SubType { get; set; }
		
		public decimal Confidence { get; set; }			// from InstanceInfo
		public string InstanceId { get; set; }
		public decimal InstanceSeverity { get; set; }							
		
		public Fortify_Vulnerability()
		{
			ReplacementDefinitions = new Fortify_ReplacementDefinitions();
			Traces = new List<Fortify_TraceEntry>();
		}
		
		public override string ToString()
		{
			return "{0} {1} {2} [{3} / {4}]".format(Kingdom, Type, SubType, InstanceSeverity, Confidence);
		}
	}
	
    public class API_Fortify
    {    	    	
		public API_Fortify()
		{
		}				
		
		public Fortify_Scan fortifyScan(string fvdlFile)
		{
			return convertToFortifyScan(fvdlFile);
		}
		
    	public Fortify_Scan convertToFortifyScan(string fvdlFile)
    	{
    		var scan = new Fortify_Scan();
    		scan._fvdl = loadFvdl_Raw(fvdlFile);
    		scan.mapFvdlData();
    		return scan;
    	}
    	
    	public FVDL loadFvdl_Raw(string fvdlFile)
    	{
    		"Loading fvdl file: {0}".info(fvdlFile);
			try
			{
				var chachedFvdl = (FVDL)O2LiveObjects.get(fvdlFile);
				if (chachedFvdl.notNull())
					return chachedFvdl;
			}
			catch { }
			 
			var o2Timer = new O2Timer("loading {0} file".format(fvdlFile.fileName())).start();		
	 		var _fvdl = FVDL.Load(fvdlFile);	
	 		O2LiveObjects.set(fvdlFile,_fvdl);
	 		o2Timer.stop();
		 	return _fvdl;  
		}
	}
	
	public static class Fortify_Scan_ExtensionMethods
	{
		public static Fortify_Scan fortifyScan(this string fvdlFile)
		{
			return new API_Fortify().fortifyScan(fvdlFile);
		}
	}
	
	public static class Fortify_Scan_ExtensionMethods_MappingFvdl
	{
		public static Fortify_Scan mapFvdlData(this Fortify_Scan fortifyScan)
		{
			var o2Timer = new O2Timer("Mapped Fvdl Data").start();
			fortifyScan.mapScanDetails()
					   .mapContextPool()
					   .mapDescriptions()
					   .mapCalledWithNoDefs()
					   .mapSinks()
					   .mapSources()
					   .mapSnippets()
					   .mapVulnerabilities();
			o2Timer.stop();		   
			return fortifyScan;
		}
		
		public static Fortify_Scan mapScanDetails(this Fortify_Scan fortifyScan)
		{
			var fvdl = fortifyScan._fvdl;
			fortifyScan.BuildID = fvdl.Build.BuildID;
			fortifyScan.Loc = fvdl.Build.LOC;
			fortifyScan.SourceBasePath = fvdl.Build.SourceBasePath;
			fortifyScan.ScanTime = fvdl.Build.ScanTime.value;
			fortifyScan.CreatedDateTime = DateTime.Parse("{0} {1}".format(fortifyScan._fvdl.CreatedTS.date.ToShortDateString() , 
																		  fortifyScan._fvdl.CreatedTS.time.ToLongTimeString() ));						
			fortifyScan.Errors = fvdl.EngineData.Errors.str()!= "<Errors xmlns=\"xmlns://www.fortifysoftware.com/schema/fvdl\" />" 
										? fvdl.EngineData.Errors.str()
										: "";
			foreach(var file in fvdl.Build.SourceFiles.File)
				fortifyScan.ScannedFiles.Add(new Fortify_ScannedFile () 
													{
														Loc = file.loc, 
														Size = file.size, 
														Timestamp = file.timestamp, 
														Type = file.type, 
														Path = file.TypedValue
													});
			return fortifyScan;
		}
		
		public static Fortify_Scan mapContextPool(this Fortify_Scan fortifyScan)
		{
			foreach(var context in fortifyScan._fvdl.ContextPool.Context)
			{
				try
				{
					fortifyScan.Contexts.Add( 
						new Fortify_Context()
								{
									Id = context.id.str() ,	
									Function = new Fortify_Function(context.Function.name,context.FunctionDeclarationSourceLocation)											
							});										
				}
				catch(Exception ex)
				{
					"Error Adding ContextPool Item: {0}".error(ex.Message);
				}
			}
			return fortifyScan;		
		}				
		
		public static Fortify_Scan mapDescriptions(this Fortify_Scan fortifyScan)
		{			 	
				foreach(var description in fortifyScan._fvdl.Description)
				{
	  		    	var fortifyDescription = new Fortify_Description
	  		    			{
	  		    				Abstract = description.Abstract,
	  		    				ClassID = description.classID,
	  		    				ContentType = description.contentType,	  		    				
	  		    				Explanation = description.Explanation,
	  		    				Recommendations = description.Recommendations,
			    			};
			    	if (description.Tips.notNull())
			    		foreach(var tip in description.Tips.Tip)
			    			fortifyDescription.Tips.Add(tip);
			    	fortifyScan.Descriptions.add(fortifyDescription);	
			    }
			    return fortifyScan;
		}
		
		public static Fortify_Scan mapCalledWithNoDefs(this Fortify_Scan fortifyScan)
		{
			//have to map this using the xElement because the xsd/cs schema doesn't support it (at the moment)
			//return _fortifyScan._fvdl.ProgramData.CalledWithNoDef[0].attribute("name").value();   
			foreach(var function in fortifyScan._fvdl.ProgramData.CalledWithNoDef.xElement().elements())
			{				
				fortifyScan.CalledWithNoDefs.Add( 
					new Fortify_CalledWithNoDef()
							{
								Name = function.attribute("name").value(),
								Namespace = function.attribute("namespace").value(),
								EnclosingClass = function.attribute("enclosingClass").value()
							});										
			}
			return fortifyScan;		
		}	
		
		public static Fortify_Scan mapSinks(this Fortify_Scan fortifyScan)
		{
			foreach(var sink in fortifyScan._fvdl.ProgramData.Sinks.SinkInstance)
			{				
				try
				{
					fortifyScan.Sinks.Add( 
						new Fortify_Sink()
								{
									RuleID = sink.ruleID.str() ,
									Function_Call = new Fortify_Function(sink.FunctionCall.Function.name,sink.FunctionCall.SourceLocation)
								});										
				}
				catch(Exception ex)
				{
					"Error Adding Sink: {0}".error(ex.Message);
				}							
			}
			return fortifyScan;		
		}
		
		public static Fortify_Scan mapSources(this Fortify_Scan fortifyScan)
		{
			foreach(var source in fortifyScan._fvdl.ProgramData.Sources.SourceInstance)
			{		
				try
				{
					var fortifySource = new Fortify_Source()
												{
													RuleID = source.ruleID.str() ,												
												};
					if (source.FunctionCall.notNull())											
						fortifySource.Function_Call = new Fortify_Function(source.FunctionCall.Function.name,source.FunctionCall.SourceLocation);
					if (source.FunctionCall.notNull())	
						fortifySource.Function_Entry = new Fortify_Function(source.FunctionCall.Function.name,source.FunctionCall.SourceLocation);
					if (source.TaintFlags.notNull())
						fortifySource.TaintFlags = (from taintFlag in source.TaintFlags.TaintFlag
													select taintFlag.name).toList();
					fortifyScan.Sources.Add(fortifySource); 
				}
				catch(Exception ex)
				{
					"Error Adding Source: {0}".error(ex.Message);
				}
			}
			return fortifyScan;		
		}		
		
		public static Fortify_Scan mapSnippets(this Fortify_Scan fortifyScan)
		{
			foreach(var snippet in fortifyScan._fvdl.Snippets.Snippet)
			{				
				fortifyScan.Snippets.Add( 
					new Fortify_Snippet()
							{
								Id = snippet.id.str() ,	
								Text = snippet.Text,
								CodeLocation = new Fortify_CodeLocation()
														{
															Path = snippet.File,
															Line = snippet.StartLine,
															LineEnd = snippet.EndLine															
														}											
							});										
			}			
			return fortifyScan;		
		}				

		public static Fortify_Scan mapVulnerabilities(this Fortify_Scan fortifyScan)
		{			 	
			foreach(var vulnerability in fortifyScan._fvdl.Vulnerabilities.Vulnerability)				
				if (vulnerability.notNull())
				{
	  		    	var fortifyVulnerability = new Fortify_Vulnerability(); 
			    			
			    	//from ClassInfo			    				    	
			    	fortifyVulnerability.AnalyzerName 	 = vulnerability.ClassInfo.AnalyzerName;
			    	fortifyVulnerability.ClassId 		 = vulnerability.ClassInfo.ClassID;
			    	fortifyVulnerability.DefaultSeverity = vulnerability.ClassInfo.DefaultSeverity;
			    	fortifyVulnerability.Kingdom 		 = vulnerability.ClassInfo.Kingdom;			    	
			    	fortifyVulnerability.Type 			 = vulnerability.ClassInfo.Type;			    	
			    	fortifyVulnerability.SubType 		 = vulnerability.ClassInfo.Subtype;			    	
			    	
			    	//from 
			    	fortifyVulnerability.InstanceId 		= vulnerability.InstanceInfo.InstanceID;
			    	fortifyVulnerability.InstanceSeverity 	= vulnerability.InstanceInfo.InstanceSeverity;
			    	fortifyVulnerability.Confidence 		= vulnerability.InstanceInfo.Confidence;
			    	
			    	//
			    	
			    	//from AnalysisInfo		
			    	var analysisInfo = 	vulnerability.AnalysisInfo;			    	
			    	if (analysisInfo.Unified.notNull())
			    	{
			    		if (analysisInfo.Unified.Context.notNull() && analysisInfo.Unified.Context.Function.notNull())			    		
				    		fortifyVulnerability.Context = new Fortify_Function(analysisInfo.Unified.Context.Function.name,
				    															analysisInfo.Unified.Context.FunctionDeclarationSourceLocation);				    	
						if (analysisInfo.Unified.ReplacementDefinitions.notNull())			    														
				    		foreach(var def in analysisInfo.Unified.ReplacementDefinitions.Def)			    		
				    			fortifyVulnerability.ReplacementDefinitions.Definitions.add(def.key, def.value);
				    	foreach(var trace in analysisInfo.Unified.Trace)
				    		foreach(var entry in trace.Primary.Entry)
					    	{
					    		var traceEntry = new Fortify_TraceEntry();
					    		if (entry.NodeRef.notNull())
					    			traceEntry.NodeRefId = entry.NodeRef.id;
					    		if (entry.Node.notNull())
					    		{
					    			var node = entry.Node;
					    			traceEntry.DetailsOnly = node.detailsOnly ?? false;
					    			traceEntry.IsDefault = node.isDefault ?? false;
					    			traceEntry.Label = node.label ?? "";
					    			
					    			if (node.Action.notNull())
					    			{
					    				traceEntry.ActionType = node.Action.type;
					    				traceEntry.ActionValue = node.Action.TypedValue;
					    			}					    			
					    			if (node.Knowledge.notNull())
					    			{
					    				foreach(var fact in node.Knowledge.Fact)
					    					traceEntry.KnowledgeFacts.Add(new Fortify_TraceEntryFact()
			    																{
			    																	Primary = fact.primary,
			    																	Type = fact.type,
			    																	Value = fact.TypedValue
			    																});
									}									
									if (node.Reason.notNull())
									{
										traceEntry.Reason_RuleId = node.Reason.Rule.notNull()
																		? node.Reason.Rule.ruleID
																		: "";
										traceEntry.Reason_TraceRef = node.Reason.TraceRef.notNull()
											 							? node.Reason.TraceRef.str()
											 							: "";
										traceEntry.Reason_Internal = node.Reason.Internal.notNull()
											 							? node.Reason.Internal.str()
											 							: "";		 							
									}	
									if (node.SourceLocation.notNull())
									{
										traceEntry.SourceLocation = new Fortify_CodeLocation(node.SourceLocation);
										traceEntry.SourceLocation_ContextId = node.SourceLocation.contextId ?? 0;
										traceEntry.SourceLocation_Snippet = node.SourceLocation.snippet;
									}
									if (node.SecondaryLocation.notNull())
									{
										traceEntry.SecundaryLocation = new Fortify_CodeLocation(node.SecondaryLocation);										
										traceEntry.SecundaryLocation_Snippet = node.SecondaryLocation.snippet;										
									}									
						    	}
					    		fortifyVulnerability.Traces.Add(traceEntry);					    				    	
					    	}
					}    	
		    		fortifyScan.Vulnerabilities.add(fortifyVulnerability);	
		    	}
		    	
			return fortifyScan;
		}
    }        
    
    public static class Fortify_Fvdl_GUI_Helpers
    {
    	public static PropertyGrid createFvdlViewer_PropertyGrid(this Control control)
    	{    		
			var propertyGrid = control.add_PropertyGrid().helpVisible(false);    
			Action<string> loadAndShowFile =    
				(file)=> O2Thread.mtaThread(
					()=>	propertyGrid.show(new API_Fortify().convertToFortifyScan(file)));
			
			propertyGrid.onDrop(loadAndShowFile);	
			return propertyGrid;
    	}
    	
    	
    }
}    
    
    	