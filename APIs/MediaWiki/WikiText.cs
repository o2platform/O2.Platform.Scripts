// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using Irony.Parsing;
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll
//O2File:O2MediaWikiAPI.cs
namespace O2.XRules.Database.APIs
{
	/// Abstract WikiText CLASS
	public abstract class WikiText	
	{
		public List<string> TextParsed { get; set; }
		public ParserMessageList  ParserMessages { get; set; }
		public bool UseContentLocalCache { get; set; }
		public ParseTree ParseTree { get; set; }
		public ParseTreeStatus Status { get; set; }
		
		public WikiText()
		{
			TextParsed = new List<string>();
			UseContentLocalCache = true;
		}				
    	
    	public bool parse(O2MediaWikiAPI wikiApi, Grammar grammar, string page)
    		 //where T : WikiText	    		 
    	{
    		return parse(wikiApi.raw(page,UseContentLocalCache),grammar);	
    	}
    	
    	public bool parse(string wikiText, Grammar grammar)
    		//where T : WikiText	    		
    	{
    		TextParsed.Add(wikiText);
    		if (wikiText.valid().isFalse())
    			return false;// (T)this;
    		
    		//var grammar = new T1();   
			var Parser = new Parser(grammar);    
			ParseTree = Parser.Parse(wikiText);  
			Status = ParseTree.Status;
			ParserMessages = ParseTree.ParserMessages;
			if (ParserMessages.size()>0)
			{
				"There were errors processing this WikiText".error();
				foreach(var message in ParserMessages)
					"  - {0}: {1} at location {2}".error(message.Level.str(),message.str(), message.Location.str());
				return false;
			}	
			return processParseTree();
			//return (T)this;
		}
		
		//public abstract T parse<T>(O2MediaWikiAPI wikiApi, string page)
		//	where T : WikiText; 
    	public abstract bool processParseTree();
	}
	
	public static class WikiText_ExtensionMethods
	{
		public static T useCache<T>(this T wikiText, bool value) 
			where T : WikiText
    	{
    		wikiText.UseContentLocalCache = value;
    		return wikiText;
    	}
	}
	
	///WikiText_HeadersAndTemplates
	
    public class WikiText_HeadersAndTemplates : WikiText
    {    				
    	public Dictionary<string,List<string>> Templates { get; set; }  
    	public List<string> OnlySelectTemplatesWith { get; set; } 
		
		public WikiText_HeadersAndTemplates() : base()
    	{    		
    		Templates = new Dictionary<string,List<string>>();   
    		OnlySelectTemplatesWith = new List<string>();
    	}    
    	
    	public WikiText_HeadersAndTemplates(string selectionCriteria) : this()
    	{    		
    		OnlySelectTemplatesWith.Add(selectionCriteria);
    	}  
		
		//method called that will trigger the parsing
		public WikiText_HeadersAndTemplates parse(O2MediaWikiAPI wikiApi, string page)			
		{
			base.parse(wikiApi, new WikiText_HeadersAndTemplates_Grammar(), page); 
			return this;
		}

		//callback to be used to process the calltreeCreated
		public override bool processParseTree() 
		{
			if (this.ParseTree.notNull())
			{
				try
				{
					var currentTemplate = "";
					foreach(var child in ParseTree.Root.ChildNodes)
					{
						var nodeText = child.Token.ValueString.trim();
						var nodeType = child.Term.Name; 
						if (nodeType == "h2")
							currentTemplate = nodeText;
						if (currentTemplate.valid())
							if (nodeType == "template")
								if (OnlySelectTemplatesWith.size()==0 || nodeText.contains(OnlySelectTemplatesWith))
								{
									if (nodeText.contains("|"))
										nodeText = nodeText.split("|")[0].trim();
									Templates.add(currentTemplate, nodeText);
								}
					}			
					return true;
				}
				catch(Exception ex)
				{
					ex.log("In WikiText_HeadersAndTemplates.processParseTree");
				}
			}
			return false;
		}				
    }
    
   
   
   ///WikiText_HeadersAndTemplates_Grammar
   
    //These are massive hacks that should be rewritten once the MediaWiki Gramar is better
    [Language("MediaWiki", "1.0", "MediaWiki markup grammar.")]
    public class WikiText_HeadersAndTemplates_Grammar : Grammar
    {
    	public WikiText_HeadersAndTemplates_Grammar()
    	{
	     	// Terminals  	     	
	     	var text = new WikiTextTerminal("text");  
	     	//var templateText = new DsvLiteral("stringValue", TypeCode.String," ");  
	     	
	     	//Headings
		    var h1 = new WikiBlockTerminal("h1", WikiBlockType.EscapedText, "=", "\n", "h1"); 
		    var h2 = new WikiBlockTerminal("h2", WikiBlockType.EscapedText, "==", "==", "h2"); 
		    var h3 = new WikiBlockTerminal("h3", WikiBlockType.EscapedText, "===", "\n", "h3"); 
		    var h4 = new WikiBlockTerminal("h4", WikiBlockType.EscapedText, "====", "\n", "h4"); 
		    var h5 = new WikiBlockTerminal("h5", WikiBlockType.EscapedText, "=====", "\n", "h5"); 
		    var h6 = new WikiBlockTerminal("h6", WikiBlockType.EscapedText, "======", "\n", "h6"); 
		      
		    //Block tags
     		var template = new WikiBlockTerminal("template", WikiBlockType.CodeBlock, "{{", "}}", "pre"); 
	     	
	     	
	     	// Non-terminals
	     	var wikiElement = new NonTerminal("wikiElement");
      		var wikiText = new NonTerminal("wikiText"); 
      		
      		
	     	// BNF rules	     	
	     	wikiElement.Rule = 	text |
	     						h1 | h2 | h3 | h4 | h5 | h6 |
	     						template |
	     					   	NewLine
	     					   	;
	     	wikiText.Rule = MakeStarRule(wikiText, wikiElement); 
	     	
	  		// config               
			this.Root =  wikiText; 
     	 	this.WhitespaceChars = string.Empty;
      		MarkTransient(wikiElement); 
			NewLine.SetFlag(TermFlags.IsPunctuation, false); 
      		this.LanguageFlags |= LanguageFlags.DisableScannerParserLink | LanguageFlags.NewLineBeforeEOF | LanguageFlags.CanRunSample
      							| LanguageFlags.CreateAst ;
		}				
    }


	public class WikiText_Template	: WikiText
	{
		public string TemplateName { get; set; }
		public Dictionary<string,string> Variables { get; set; }
		
		public WikiText_Template() : base()
    	{    		
    		Variables = new Dictionary<string,string>();		
    	}    	    	    	    	    	    	
		
		//method called that will trigger the parsing
		public WikiText_Template parse(O2MediaWikiAPI wikiApi, string page)			
		{
			base.parse(wikiApi, new WikiText_Template_Grammar(), page); 
			return this;
		}

		//callback to be used to process the calltreeCreated
		public override bool processParseTree() 
		{
			if (this.ParseTree.notNull())
			{
				try
				{
					var root =  ParseTree.Root; 
					TemplateName = root.ChildNodes[1].ChildNodes[0].ChildNodes[0].Token.ValueString.trim();
					
					var rows = root.ChildNodes[1].ChildNodes[1].ChildNodes;
					foreach(var row in rows) 
					{	
						//var nodeValue = child.ValueString.isNull() ? "" : child.Token.ValueString.trim();  
						var nodeType = row.Term.Name; 
						if (nodeType =="dataRow")
						{
							var variableName = row.ChildNodes[0].Token.ValueString.trim();
							var variableValue = row.ChildNodes[1].Token.ValueString.trim()  ;
							Variables.add(variableName, variableValue);
							//"{0} : {1} " .info(variableName, variableValue);	
						} 
					}
				}
				catch(Exception ex)
				{
					ex.log("In WikiText_Template.processParseTree");
				}
			}
			return false;
		}				 
	}

    [Language("MediaWiki-Template", "1.0", "MediaWiki Templates markup grammar.")]
    public class WikiText_Template_Grammar : Grammar
    {
    	public WikiText_Template_Grammar()
    	{
	     	// Terminals  	     	
	     	//var text = new WikiTextTerminal("text");
	     	var pipeChar = ToTerm("|");
	     	var identifier = new IdentifierTerminal("identifier");//,"",""); 
			var variableName = new DsvLiteral("variableName", TypeCode.String,"=");   
	     	var variableValue = new DsvLiteral("variableValue", TypeCode.String,"\n");  
	     	
	     	// Non-terminals
	     	var wikiTemplate = new NonTerminal("wikiElement");
	     	var templateData = new NonTerminal("templateData");
	     	var includeOnly = new WikiBlockTerminal("includeOnly", WikiBlockType.EscapedText, "<includeonly>", "</includeonly>", "includeOnly");
	     	var noInclude = new WikiBlockTerminal("includeOnly", WikiBlockType.EscapedText, "<noinclude>", "</noinclude>", "includeOnly");
	     	
      		//var wikiText = new NonTerminal("wikiText"); 
      		var templateDetails = new NonTerminal("templateDetails");
      		var row = new NonTerminal("row");
      		var emptyRow = new NonTerminal("emptyRow");
      		var dataRow = new NonTerminal("dataRow");
      		var rows = new NonTerminal("rows");
      		//var drive = new NonTerminal("drive");    
      		
	     	// BNF rules
	     	templateDetails.Rule = identifier + ":" + includeOnly + noInclude;
	     	dataRow.Rule = pipeChar + variableName + variableValue;
	     	row.Rule = emptyRow | dataRow;	     			   
	     	emptyRow.Rule = ToTerm("|-") + NewLine;
	     	rows.Rule = MakePlusRule(rows, row);
	     	templateData.Rule = templateDetails + NewLine + rows ;
	     	wikiTemplate.Rule = ToTerm("{{")+ templateData  + "}}";// + ToTerm("}}"); 
	     	//wikiText.Rule = "{{ " + MakeStarRule(wikiText, wikiElement)  + "}}" ; 
	     	//wikiText.Rule = MakeStarRule(wikiText, wikiElement) ; 
	     	// config               
			//this.Root =  wikiText; 
			this.Root =  wikiTemplate;
			
     	 	//this.WhitespaceChars = string.Empty;
      		MarkTransient(row);       		
      		MarkPunctuation(pipeChar);
			//NewLine.SetFlag(TermFlags.IsPunctuation, true); 
			this.LanguageFlags  = LanguageFlags.NewLineBeforeEOF ;
      		//this.LanguageFlags |= LanguageFlags.DisableScannerParserLink | LanguageFlags.NewLineBeforeEOF | LanguageFlags.CanRunSample
      		//					| LanguageFlags.CreateAst ;
	    }
	}

	//    Parse_ExtensionMethods
    public static class Parse_ExtensionMethods
    {
    	public static TreeView showInTreeView(this TreeView treeView, ParseTree ParseTree)
            {
                    if (ParseTree != null)
                    {
                            treeView.clear(); 
                            if (ParseTree.HasErrors())
                            {
                                    var errorNode = treeView.add_Node("Parse errors!!").setTextColor(Color.Red);
                                    foreach(var message in ParseTree.ParserMessages)
                                    {
                                            var noteText  = "{0} [{1} : {2}]".format(message, message.Location.Line, message.Location.Column);
                                            errorNode.add_Node(noteText); 
                                            
                                    }
                                    treeView.expand();
                            }
                            else                    
                            {                                                        
                                    treeView.removeEventHandlers_BeforeExpand();                             
                                    treeView.beforeExpand<ParseTreeNode>( 
                                            (parseTreeNode)=> 
                                                    {
                                                            "in before expand".debug();
                                                            var currentNode = treeView.current();
                                                            currentNode.clear();                    
                                                            foreach(var child in parseTreeNode.ChildNodes)
                                                            {
                                                                    if (child.Token == null) 
                                                                            currentNode.add_Node(child.Term.Name ,child,true);      
                                                                    else
                                                                            currentNode.add_Node(child.Term.Name + " : " + child.Token.ValueString,child);                    
                                                            }                               
                                                    });
                                    if (ParseTree != null && ParseTree.Root != null)
                                    {
                                            treeView//.add_Node("Raw ParseTree Data")
                                                    .add_Node(ParseTree.Root.Term.Name, ParseTree.Root, true);
                                            //treeView.expandAll(); 
                                            
                                            //treeView.add_Node("DirResult Object", dirResultObject(), true);                                 
                                            //treeView.autoExpandObjects<DirResult>();
                                            //treeView.autoExpandObjects<DirResult.File>();
                                            //treeView.autoExpandObjects<DirResult.Dir>();                                    
                                            treeView.selectFirst();                                 
                                            treeView.expand(); 
                                    }                                                                                                                       
                            }
                    }
                    return treeView;
            }
    }

}
