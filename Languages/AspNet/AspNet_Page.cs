// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Collections;
using System.Web.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using ICSharpCode.SharpDevelop.Dom;
//using O2.API.AST.CSharp;
using ICSharpCode.NRefactory.Ast;
//using O2.API.AST.ExtensionMethods.CSharp;
using O2.XRules.Database.Utils;
//O2Ref:System.Configuration.dll


namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{
	[Serializable]
    public class AspNet_Page
    {   
    	public string File_Path { get; set;}
		public string Virtual_Path { get; set; }				
								
		public string Server { get; set; }		
		public string Web_Address { get; set; }
						
		public Items PagesConfig { get; set; }
		public NameValueItems ConfigItems {get;set;}    
		    	    	
    	public List<string> Namespaces { get; set; }
    	public List<string> SourceDependencies { get; set; }    	
    	public CodeBlock CodeBlock { get; set; }
		
		public bool Store_AspNet_SourceCode { get; set; }
		public bool Store_Compiled_AspNet_SourceCode { get; set; }
		
		public string AspNet_SourceCode { get; set; }
		public List<string> AspNet_Compiled_SourceCode { get; set; }						
		
		
		//internal vars used in mapControlBuilder
		[XmlIgnore] public object virtualPath;				// System.Web.VirtualPath is private
		[XmlIgnore] public string tempDllFile;
		[XmlIgnore] public object buildProvidersCompiler; // System.Web.Compilation.BuildProvidersCompiler is private
		[XmlIgnore] public System.Web.Compilation.BuildProvider buildProvider;
		[XmlIgnore] public CompilationSection compConfig;
		[XmlIgnore] public AssemblyBuilder assemblyBuilder;
		[XmlIgnore] public BaseParser parser;
		
		
		
    	public AspNet_Page()
    	{
    		ConfigItems = new NameValueItems();
			Namespaces = new List<string>();
			SourceDependencies = new List<string>();
			AspNet_Compiled_SourceCode = new List<string>();
    	}
    	
    	public AspNet_Page(string file_Path, string virtual_Path, string server) : this()
    	{
    		Virtual_Path = virtual_Path;
    		File_Path = file_Path;
    		Server = server;
    		Web_Address= new Uri(server.uri() , virtual_Path).str();    		
    	}
    	
    	public override string ToString()
    	{
    		return Virtual_Path;
    	}    	
	}    			
	
	[Serializable]
	public class CodeBlock
	{	
		[XmlAttribute] public string VirtualPath { get; set; }	
		[XmlAttribute] public string BuilderType { get; set; }
		[XmlAttribute] public string BlockType { get; set; }
		[XmlAttribute] public string ControlType { get; set; }
		[XmlAttribute] public string ID { get; set; }		
		[XmlAttribute] public int Line { get; set; }		
		[XmlAttribute] public int Column { get; set; }				
		[XmlAttribute] public string TagName { get; set; }						
		
		[XmlAttribute] public string SkinID { get; set; }
		[XmlAttribute] public bool IsHtmlControl { get; set; }
		[XmlAttribute] public bool HasAspCode  { get; set; }	
		 
		[XmlAttribute] public string Filter { get; set; }
		
		[XmlAttribute] public string Content { get; set; }
		
		public NameValueItems SimplePropertyEntries { get; set; }
		public NameValueItems EventEntries { get; set; }			
		public List<CodeBlock> SubBlocks { get; set; }	
		
		
		public CodeBlock()
		{
			SubBlocks = new List<CodeBlock>();
			SimplePropertyEntries = new NameValueItems();
			EventEntries = new NameValueItems();
		}
		
		public override string ToString()
		{
			return "{0} {1}".format(this.BlockType, this.ControlType);
		}
	}
	
	public static class AspNet_Page_ExtensionMethods_Data_Mapping
	{
		public static AspNet_Page map_Page(this AspNet_Page aspNetPage)
		{
			aspNetPage.map_Parser(aspNetPage.parser);   
			
			aspNetPage.map_ControlBuilders(aspNetPage.parser);
			
			aspNetPage.map_CompiledSourceCode();
			
			var pagesConfig = (System.Web.Configuration.PagesSection)aspNetPage.parser.property("PagesConfig");
			aspNetPage.map_PagesConfig(pagesConfig);
			
			return aspNetPage;
		}
		
		public static AspNet_Page map_Parser(this AspNet_Page aspNetPage, BaseParser parser)
		{									
			aspNetPage.Virtual_Path = parser.property("CurrentVirtualPathString").str();
			aspNetPage.ConfigItems.add("CurrentVirtualPathString",parser.property("CurrentVirtualPathString").str())
					  			  .add("DefaultBaseType",parser.property("DefaultBaseType").str())
								  .add("DefaultFileLevelBuilderType",parser.property("DefaultFileLevelBuilderType").str())
								  .add("HasCodeBehind",parser.property("HasCodeBehind").str())
								  .add("IsCodeAllowed",parser.property("IsCodeAllowed").str());
								  
			if (aspNetPage.Store_AspNet_SourceCode)
			 	aspNetPage.AspNet_SourceCode = parser.property("Text").str();
			 	
			foreach(DictionaryEntry namespaceEntry in (Hashtable)parser.property("NamespaceEntries")) 			 
			 	aspNetPage.Namespaces.add(namespaceEntry.Key.str());//
			 
			foreach(var sourceDependencies in (IEnumerable)parser.property("SourceDependencies")) 			 
			 	aspNetPage.SourceDependencies.add(sourceDependencies.str());//	
			 		

			return aspNetPage;
		}
		
		public static AspNet_Page map_PagesConfig(this AspNet_Page aspNetPage, PagesSection pagesSection)
		{
			aspNetPage.PagesConfig = pagesSection.property_Values_AsStrings();
			return aspNetPage;

		}
		
		public static AspNet_Page map_CompiledSourceCode(this AspNet_Page aspNetPage) 
		{			
			if (aspNetPage.Store_Compiled_AspNet_SourceCode)
			{
				var fileMappings =(Hashtable)aspNetPage.assemblyBuilder.field("_buildProviderToSourceFileMap");				
				
				foreach(var file in fileMappings.Values) 
				{
					var sourceCode = file.str().fileContents();
					if (sourceCode.valid()) 
					{
						aspNetPage.AspNet_Compiled_SourceCode.add(sourceCode);	
						"saved contents of compiled asp.net page: {0}".info(file);
					}
					else
						"in aspNetPage parser, could not get source code for file: {0}".error(file);					
				}
			}
			return aspNetPage;
		}
		
		public static CodeBlock mapControlBuilder(this ControlBuilder controlBuilder)
		{
			var codeBlock = new CodeBlock();  
			codeBlock.VirtualPath = controlBuilder.property<string>("VirtualPathString");	
			codeBlock.ID = controlBuilder.ID;   
			codeBlock.BuilderType = controlBuilder.type().str(); 
			if (controlBuilder.ControlType.notNull()) 
				codeBlock.ControlType = controlBuilder.ControlType.str();  
			codeBlock.Line = controlBuilder.property<int>("Line");
			codeBlock.Column = controlBuilder.property<int>("Column");
			codeBlock.TagName = controlBuilder.TagName;
			codeBlock.SkinID = controlBuilder.property<string>("SkinID"); 
			codeBlock.IsHtmlControl = controlBuilder.property<bool>("IsHtmlControl");
			codeBlock.IsHtmlControl = controlBuilder.property<bool>("HasAspCode");	
			

			codeBlock.Filter = controlBuilder.property<string>("Filter");
			foreach(var simplePropertyEntry in (IEnumerable)controlBuilder.property("SimplePropertyEntries"))
			{
				if (simplePropertyEntry is SimplePropertyEntry) 																	
					codeBlock.SimplePropertyEntries.add(((SimplePropertyEntry)simplePropertyEntry).Name,
															((SimplePropertyEntry)simplePropertyEntry).PersistedValue);								
				else 
					"in controlBuilder.SimplePropertyEntries there was an unsupported type: {0}".error(simplePropertyEntry.type());
			}
			
			foreach(var eventEntry in (IEnumerable)controlBuilder.property("EventEntries")) 
				codeBlock.EventEntries.add(eventEntry.property("Name").str(), eventEntry.property("HandlerMethodName").str());

			var content = controlBuilder.property("Content");
			if (content.notNull())
				codeBlock.Content = content.str();
				
			if (controlBuilder.property("BlockType").notNull()) 
				codeBlock.BlockType = controlBuilder.property("BlockType").str();  
				
			foreach(var subBuilder in (IEnumerable)controlBuilder.property("SubBuilders"))
			{
				if (subBuilder is string)
				{
					codeBlock.SubBlocks.Add(new CodeBlock() {
																BlockType = subBuilder.type().str(), 
																Content = subBuilder.str()
															});
				}
				else
				{									
					codeBlock.SubBlocks.Add(mapControlBuilder((ControlBuilder)subBuilder));
				}
			}
			return codeBlock;
		}				
		
		public static AspNet_Page map_ControlBuilders(this AspNet_Page aspNetPage, BaseParser pageParser)
		{			
			var rootBuilder =  (ControlBuilder)pageParser.property("RootBuilder"); 

			aspNetPage.CodeBlock = rootBuilder.mapControlBuilder();   
			return aspNetPage;
		}
	}
	
	public static class AspNet_Page_ExtensionMethods_CodeBlocks
	{
	
		public static List<CodeBlock> allCodeBlocks(this AspNet_Page aspNetPage)
		{
			return aspNetPage.CodeBlock.allCodeBlocks();
		}
		
		public static List<CodeBlock> allCodeBlocks(this CodeBlock codeBlock)
		{			
			var codeBlocks = new List<CodeBlock>();
			codeBlocks.add(codeBlock);
			foreach(var subCodeBlock in codeBlock.SubBlocks)
				codeBlocks.AddRange(subCodeBlock.allCodeBlocks());
			return codeBlocks;
		}
	}
			
	public static class AspNet_Page_ExtensionMethods_Parser
	{
		public static AspNet_Page parseAspNetPage(this AspNet_Page aspNetPage, string aspNetPageToParse)
		{
			aspNetPage.virtualPath = "System.web".assembly().type("VirtualPath").ctor(aspNetPageToParse);     
			aspNetPage.tempDllFile = "o2_aspnet__{0}.dll".format(5.randomLetters());
			//return tempFile;
			
			aspNetPage.buildProvidersCompiler =  "System.web".assembly()
													  		 .type("BuildProvidersCompiler")										  
													  		 .ctor(aspNetPage.virtualPath, aspNetPage.tempDllFile);
			
			//var buildManager  
			aspNetPage.buildProvider =  (System.Web.Compilation.BuildProvider)
											typeof(System.Web.Compilation.BuildManager).invokeStatic(
											   				"CreateBuildProvider",
														   aspNetPage.virtualPath, 
														   (CompilationSection)aspNetPage.buildProvidersCompiler.property("CompConfig"), 
														   (ICollection)aspNetPage.buildProvidersCompiler.property("ReferencedAssemblies"), 
														   true); 
			
			aspNetPage.compConfig = (CompilationSection)aspNetPage.buildProvidersCompiler.property("CompConfig");
			
			aspNetPage.assemblyBuilder = (AssemblyBuilder)aspNetPage.buildProvider.property("CodeCompilerType")
											   		  .invoke("CreateAssemblyBuilder", 
																(CompilationSection)aspNetPage.buildProvidersCompiler.property("CompConfig"), 
																(ICollection) aspNetPage.buildProvidersCompiler.property("ReferencedAssemblies"));
			
			//textWriter = (TextWriter)assemblyBuilder.invoke("CreateCodeFile", buildProvider);
						
			try
			{
				aspNetPage.buildProvider.invoke("GenerateCode", aspNetPage.assemblyBuilder);
			} 
			catch(Exception ex)
			{ 
				ex.Message.error();//log();  
			}
			
			aspNetPage.parser =  aspNetPage.buildProvider.property<BaseParser>("Parser");      
			
			//var pagesConfig = (System.Web.Configuration.PagesSection)aspNetPage.pageParser.property("PagesConfig");
			
			aspNetPage.map_Page();			
			
			return aspNetPage;
		}				
	}
	
}
