// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.DataViewers;
using O2.Views.ASCX.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;
using Jint;
using Jint.Native;
using Jint.Debugger;
using Jint.Expressions;
using O2.External.IE.ExtensionMethods;
using O2.External.IE.Interfaces;
using O2.External.IE.Wrapper;
using O2.External.IE.WebObjects;
using O2.XRules.Database.Utils;

//O2File:HtmlAgilityPack_ExtensionMethods.cs

//O2Ref:Jint.dll
//O2Ref:Antlr3.Runtime.dll
//O2Ref:O2_External_SharpDevelop.dll
//O2Ref:O2_External_IE.dll

namespace O2.XRules.Database.Languages_and_Frameworks.Javascript
{
	public class Jint_Wrapper
	{
		public Uri Uri { get; set ; }
		public string Html { get; set ; }
		public HtmlAgilityPack.HtmlDocument HtmlDocument { get; set ; }
		public List<KeyValuePair<string,string>> JavaScripts { get; set ; }
		
		public Jint_Wrapper(Uri uri)
		{	
			this.Uri = uri;
			downloadHtmlAndJavascripts();			
		}
		
		public Jint_Wrapper downloadHtmlAndJavascripts()
		{
			this.Html = this.Uri.getHtml();					
			if (this.Uri.str().extension(".js"))
			{			
				this.JavaScripts = new List<KeyValuePair<string,string>>();
				this.JavaScripts.add(this.Uri.str(),this.Html);
			}
			else
			{
				this.HtmlDocument = Html.htmlDocument(); 									
				this.JavaScripts = this.HtmlDocument.javaScripts(this.Uri); 
			}
			return this;
		}
		
		public override string ToString()
		{
			return this.Uri.str();
		}
	}

    public static class Jint_ExtensionMethods
    {   
    	public static Statement jint_Compile(this string javascriptCode)
    	{
    		try
    		{	
				var compiled =  JintEngine.Compile(javascriptCode,true);
				if (compiled.statements(true).size()  < 2)
				{
					"in jint_Compile there was no AST created, so returning null".debug();					
					return null;
				}
				return compiled;
    		}
    		catch(Exception ex)
    		{
    			ex.log("in Jint_ExtensionMethods.jint_Compile(...)");
    			return null;
    		}
    	}
    	public static Statement jint_Compile(this List<KeyValuePair<string,string>> javaScripts)
    	{    		
    		var allScripts = javaScripts.getStringWithJointJavascripts();
    		var compiledAst = allScripts.jint_Compile();
    		if (compiledAst.isNull())
    			"in jint_Compile javaScripts, error compiling the joint javascript file".error();
    		return compiledAst;
    	}
    	
    	public static String getStringWithJointJavascripts(this List<KeyValuePair<string,string>> javaScripts)
    	{
    		var stringBuilder = new StringBuilder();
    		foreach(var javaScript in javaScripts.values())
    		{
    			if (javaScript.jint_Compile().notNull())
    				stringBuilder.AppendLine(javaScript.lineBeforeAndAfter());
    		}    		
    		return stringBuilder.str();
    	}    	
    	
    	
    	public static List<T> statements<T>(this Statement statement)
    		where T : Statement
    	{
    		return statement.statements<T>(false);
    	}
    	
		public static List<T> statements<T>(this Statement statement, bool recursive)
			where T : Statement
    	{
    		return (from childStatement in statement.statements(recursive)
    				where childStatement is T
    				select (T) childStatement).toList();    		
    	}

    	public static List<Statement> statements(this Statement statement)
    	{
    		return statement.statements(false);
    	}
    	
    	public static List<Statement> statements(this Statement statement, bool recursive)
    	{	    		
    		var results = new List<Statement>();
    		results.Add(statement);
    		foreach(var property in statement.type().properties()) 
			{ 
				var propValue = statement.prop(property.Name);
				if (propValue is IEnumerable && (propValue is String).isFalse())
				{
					foreach(var item in (propValue as IEnumerable))											
						if (item is Statement)
							if (recursive)							
								results.AddRange((item as Statement).statements(recursive));
							else 
								results.Add(item as Statement);							
				}
				else if (propValue is Statement)
				{
						if (recursive)
							results.AddRange((propValue as Statement).statements(recursive));
						else 
							results.Add(propValue as Statement);								
				}				
						//results.Add(propValue as Statement);														
			}    		
    		return results;
    	}
    	
    	public static Dictionary<string,List<Statement>> statements_by_Type(this Statement statement)
    	{
    		Dictionary<string, List<Statement>> dictionary = new Dictionary<string, List<Statement>>();
		    foreach (var childStatement in statement.statements(true))
		    {
		        string key = childStatement.typeName();
		        dictionary.add<string, Statement>(key, childStatement);
		    }
		    return dictionary;
    	}
    	
    	public static List<Statement> functions(this Statement statement)
    	{
    		var functions = new List<Statement>();  
    		foreach(var functionDeclarationStatement in statement.statements<FunctionDeclarationStatement>(true))
    			functions.Add(functionDeclarationStatement);
    		foreach(var functionExpression in statement.statements<FunctionExpression>(true))
    			functions.Add(functionExpression);       		
    		return functions;    		
    	}
    	
    	public static List<ValueExpression> values(this Statement statement)
    	{
    		return statement.statements<ValueExpression>(true);
    		/*var functions = new List<Statement>();  
    		foreach(var functionDeclarationStatement in statement.statements<FunctionDeclarationStatement>(true))
    			functions.Add(functionDeclarationStatement);
    		foreach(var functionDeclarationStatement in statement.statements<FunctionExpression>(true))
    			functions.Add(functionDeclarationStatement);    		
    		return functions;    		*/
    	}
    	
    	public static List<Identifier> identifiers(this Statement statement)
    	{
    		return statement.statements<Identifier>(true);
    	}
    	
    	public static List<MethodCall> methods(this Statement statement)
    	{
    		return statement.statements<MethodCall>(true);  
    	}
    	
    	public static string text(this Statement statement)
    	{
    		if (statement is Identifier)
    			return (statement as Identifier).Text;
    		if (statement is ValueExpression)
    			return (statement as ValueExpression).Value.str();
    			
    		return statement.str();
    	}
    	
    	public static Dictionary<string, object> jsonValues(this Statement statement)
    	{
    		if(statement is JsonExpression)
    			return (statement as JsonExpression).jsonValues();
    		return null; 
    	}
    	
    	public static Dictionary<string, object> jsonValues(this Expression expression)
    	{
    		if(expression is JsonExpression)
    			return (expression as JsonExpression).jsonValues();
    		return null; 
    	}
    	
    	public static Dictionary<string, object> jsonValues(this JsonExpression jsonExpression)
    	{
			var mappedValues = new Dictionary<string, object>();
			foreach(var item in jsonExpression.Values)
			{
				if (item.Value is PropertyDeclarationExpression)
				{					
					var valueExpression = (item.Value as PropertyDeclarationExpression).Expression;	
					if (valueExpression is ArrayDeclaration) 
					{																	
						mappedValues.add(item.Key, (valueExpression as ArrayDeclaration).Parameters);
						continue;
					}
					if (valueExpression is UnaryExpression)
					{
						mappedValues.add(item.Key, (valueExpression as UnaryExpression).Expression.str());
						continue;
					}					
					mappedValues.add(item.Key, valueExpression.text()); 
					continue;
				}																	
				mappedValues.add(item.Key, item.Value); 
			}
			return mappedValues;
  		}
    }
    
    public static class Jint_ExtensionMehtods_SourceCode
    {
    	public static SourceCodeDescriptor firstSourceCodeReference(this Statement statement)
    	{
    		if (statement.isNull())
    			return null;
    		return statement.firstSourceCodeReference(false);
    	}
    	
    	public static SourceCodeDescriptor firstSourceCodeReference(this Statement statement, bool removeCodeSnippet)
    	{
    		if (statement.isNull())
    			return null;
    		if (statement.Source.notNull())
    			return statement.Source;
    		foreach(var childStatement in statement.statements(true))
    			if (childStatement.Source.notNull())
    				if (removeCodeSnippet)
    					return childStatement.Source.copyWithoutCodeSnippet();
    				else
    					return childStatement.Source;			
    		return null;
    	}
    	
    	public static SourceCodeDescriptor copyWithoutCodeSnippet(this SourceCodeDescriptor sourceCode)
    	{
    		if (sourceCode.isNull())
    			return null;
    		return new SourceCodeDescriptor(sourceCode.Start.Line, sourceCode.Start.Char, 
    										sourceCode.Stop.Line, sourceCode.Stop.Char, "");
    	}
    
    }
    
    public static class Jint_ExtensionMethods_Javascripts
    {
    	// calculate Javascripts from url 
    	
    	public static List<KeyValuePair<string,string>> javaScripts(this Uri baseUri)
    	{
    		return baseUri.getHtml(true).javaScripts(baseUri);
    	}
    	
    	public static List<KeyValuePair<string,string>> javaScripts(this string htmlCode, Uri baseUri)
    	{    		
    		var htmlDocument = htmlCode.htmlDocument();
    		return htmlDocument.javaScripts(baseUri);
    	}
    	
    	public static List<KeyValuePair<string,string>> javaScripts(this HtmlAgilityPack.HtmlDocument htmlDocument, Uri baseUri)
    	{
    		var javaScripts = new List<KeyValuePair<string,string>>();
    		var scripts = htmlDocument.select("//script");
    		foreach(var script in scripts)
    		{
    			var srcAttribute = script.Attributes["src"];
    			if (srcAttribute.notNull())
    			{
    				var link = srcAttribute.Value;    				    				
    				link = link.starts("/")
    							? "{0}{1}".format(baseUri.hostUrl(), link)
    							: link;    							
    				var scriptCode = link.uri().getHtml(true);    				
					javaScripts.add("{0}       (size:  {1:0.00} k )".format(link,scriptCode.size().kBytes()), scriptCode);
															
    			}
    			var scriptValue = script.value();
				if (scriptValue.valid())
					javaScripts.add("Inside ScriptBlock       (size: {0:0.00} k )".format(scriptValue.size().kBytes()),scriptValue);

    		}
    		return javaScripts;
    		
    	}    	    
    	
    	public static bool allJavaScriptsCompileOk(this List<KeyValuePair<string,string>> javaScripts)
    	{
    		foreach(var javaScript in javaScripts.values())
    			if (javaScript.jint_Compile().isNull())
    				return false;
    		"All Javascripts Compiled OK".info();
    		return true;
    	}
    	
    	// calculate JavaScripts from IE_HtmlPage
    	
    	public static List<KeyValuePair<string,string>> javaScripts(this IO2HtmlPage htmlPage)
    	{
    		var javaScripts = new List<KeyValuePair<string,string>>();
			foreach(var script in htmlPage.Scripts)
			{				
				if (script.Text.valid())
					javaScripts.add("Inside ScriptBlock       (size: {0:0.00} k )".format((double)script.Text.size() / 1024),script.Text);
					
				if(script.Src.valid() && script.Src.isUri())
				{
					var scriptFileContents = script.Src.uri().getHtml();
					javaScripts.add("{0}       (size:  {1:0.00} k )".format(script.Src, (double)scriptFileContents.size() / 1024),scriptFileContents);
				}
			}
			return javaScripts;
    	}
    }


	//View helpers    
    public static class Jint_ExtensionMethods_ViewHelpers
    {	    	    
	    public static bool 	populateWithHtmlPageScripts(this TreeView treeView, IE_HtmlPage htmlPage)
	    {	    	
	    	var scripts = htmlPage.javaScripts();
	    	return treeView.populateWithHtmlPageScripts(scripts);
	    }
	    
	    public static bool 	populateWithHtmlPageScripts(this TreeView treeView, List<KeyValuePair<string,string>> javaScripts)
	    {		    	
	    	treeView.clear();	
			var allCode = new StringBuilder();
			var allJavaScriptsCompiledOk = true;
			
			foreach(var item in javaScripts)
			{
				treeView.add_Node(item.Key, item.Value);
				allCode.AppendLine("/* [O2 - Javascript source]: {0} */".format(item.Key).lineBeforeAndAfter());
				if (item.Value.jint_Compile().notNull())
					allCode.AppendLine("{0}".format(item.Value));
				else
				{
					allCode.AppendLine("/* [O2 - Jint Error]: Could not compile this javascript using Jint */".lineBeforeAndAfter());
					allJavaScriptsCompiledOk = false;
				}
			}
			
			if (allCode.Length > 0)
			{
				treeView.add_Node("ALL Javascript CODE        (size:  {0:0.00} k )".format((double)allCode.Length / 1024) , allCode.str());
				treeView.backColor((allJavaScriptsCompiledOk) ? Color.White : Color.LightPink);
			}
			
			return allJavaScriptsCompiledOk;
		}
				
	    public static TreeView jint_Show_AstTree(this TreeView treeView, Statement statement)
	    {
	    	return treeView.jint_Show_AstTree(statement, null, null);
	    }
	    
	    public static TreeView jint_Show_AstTree(this TreeView treeView, Statement statement, ascx_SourceCodeViewer sourceCode, TextBox textBox)
	    {
	    	//jint_configureTreeViewFor_JintView(treeView);
	    	jint_configure_showSelectionDetails(treeView,sourceCode, textBox,true);
	    	treeView.rootNode().jint_Show_AstTree(statement);	    	
	    	return treeView;
	    }
	    
	    public static TreeNode jint_Show_AstTree(this TreeNode treeNode, Statement statement)
	    {
	    	treeNode.clear();
	    	treeNode.add_Node(statement.typeName(),statement,true);	    		    	
			return treeNode;
	    }
	    
	    public static TreeView jint_Show_AstMappings(this TreeView treeView, Statement statement)
	    {
	    	treeView.visible(false);
	    	treeView.rootNode().jint_Show_AstMappings(statement);
	    	treeView.visible(true);
	    	return treeView;
	    }
	    
	    public static TreeNode jint_Show_AstMappings(this TreeNode treeNode, Statement statement)
	    {
	    	treeNode.clear();	    	   			
	    	foreach(var item in statement.statements_by_Type())
	    	{
	    		var statementType = item.Key;
	    		var statementsInType = item.Value;
	    		var nodeForType = treeNode.add_Node(statementType,statementsInType.first().firstSourceCodeReference(true));
	    		foreach(var statementInType in statementsInType)
	    			nodeForType.add_Node(statementInType.str(), statementInType.firstSourceCodeReference(true))
	    					   .jint_Show_AstTree(statementInType);	    					   
	    	}
			//treeNode.add_Nodes(statement.statements_by_Type());
//	    	treeNode.add_Nodes(statement.astMappings());
	    	return treeNode;
	    }
	    
	    public static TreeView jint_Show_Functions(this TreeView treeView, Statement statement)
	    {
	    	treeView.rootNode().jint_Show_Functions(statement);
	    	return treeView;	
	    }
	    
	    public static TreeNode jint_Show_Functions(this TreeNode treeNode, Statement statement)
	    {
	    	treeNode.clear();
	    	foreach(var functionDeclaration in statement.statements<FunctionDeclarationStatement>(true))
	    	{		    			    												
	    		treeNode.add_Node(functionDeclaration.Name, functionDeclaration.firstSourceCodeReference(true))
	    				.jint_Show_AstTree(functionDeclaration);
	    	}
	    	
	    	foreach(var functionExpression in statement.statements<FunctionExpression>(true))
	    	{		    			    												
	    		treeNode.add_Node("[FunctionExpression]" + functionExpression.Name, functionExpression.firstSourceCodeReference(true))
	    				.jint_Show_AstTree(functionExpression);
	    	}
	    		//, functionDeclaration.statements(true), true);
	    	return treeNode;
	    }
	    
	    public static TreeView jint_Show_Statements<T>(this TreeView treeView, Statement statement)
	    	where T : Statement
	    {
			return treeView.jint_Show_Statements<T>(statement,true);
	    }
	    
	    public static TreeView jint_Show_Statements<T>(this TreeView treeView, Statement statement, bool addChildStatements)
	    	where T : Statement
	    {
	    	treeView.rootNode().jint_Show_Statements<T>(statement, addChildStatements);
	    	return treeView;	
	    }
	    
	    public static TreeNode jint_Show_Statements<T>(this TreeNode treeNode, Statement statement, bool addChildStatements)
	    	where T : Statement
	    {
	    	treeNode.clear();
	    	var nodesAdded = new Dictionary<string, TreeNode>();
	    	foreach(var childStatement in statement.statements<T>(true))
	    	{		    	
	    		var codeReference = childStatement.firstSourceCodeReference(true);
	    		var nodeText = childStatement.str();
	    		if (nodeText.valid())
	    		{
	    			if (codeReference.isNull())
	    				nodeText += "   (no source code mapping)";
	    				
					if (nodesAdded.hasKey(nodeText).isFalse())
						nodesAdded.add(nodeText, treeNode.add_Node(nodeText, codeReference));
						
	    			var newNode= nodesAdded[nodeText].add_Node(nodeText, codeReference);
	    			if (addChildStatements)
	    					newNode.jint_Show_AstTree(childStatement);
	    		}
	    	}
			return treeNode;
	    }
	    
	    public static TreeView jint_configureTreeViewFor_JintView(this TreeView treeView)
	    {
	    	treeView.beforeExpand<Statement>( 
				(treeNode, statement)=> {										
								foreach(var property in statement.type().properties()) 
			 					{ 
									var propValue = statement.prop(property.Name);
									if (propValue is IEnumerable && (propValue is String).isFalse())
										foreach(var item in (propValue as IEnumerable))											
												treeNode.add_Node(item.str() ,item,item is Statement);
									else										
										if (propValue is Statement)
											treeNode.add_Node(propValue.str() ,propValue,true);  
								}								
							});
			treeView.beforeExpand<List<Statement>>(
				(treeNode, statements)=>{
											foreach(var statement in statements)
												treeNode.add_Node(statement.str(),statement); 
										});
			return treeView;							
		}
		
		public static Statement showDetails(this Statement statement, ascx_SourceCodeViewer sourceCode, TextBox textBox)
		{
			if (sourceCode!= null)
				statement.highlightInCode(sourceCode);
			if (textBox != null)
				statement.showCodeSnippet(textBox);	
			return statement;
		}
		
		public static SourceCodeDescriptor showDetails(this SourceCodeDescriptor sourceCodeDescriptor, ascx_SourceCodeViewer sourceCode, TextBox textBox)
		{			
			if (sourceCode!= null)
				sourceCodeDescriptor.highlightInCode(sourceCode);
			if (textBox != null)
				sourceCodeDescriptor.showCodeSnippet(textBox);	
			return sourceCodeDescriptor;
		}
		
		public static TreeView jint_configure_showSelectionDetails(this TreeView treeView, ascx_SourceCodeViewer sourceCode, TextBox textBox)
		{
			return treeView.jint_configure_showSelectionDetails(sourceCode,textBox,true);
		}
		
		public static TreeView jint_configure_showSelectionDetails(this TreeView treeView, ascx_SourceCodeViewer sourceCode, TextBox textBox, bool alsoConfigureForJintView)
		{
			if (alsoConfigureForJintView && (sourceCode != null || textBox != null))
				treeView.jint_configureTreeViewFor_JintView();
				
			treeView.afterSelect<Statement>((statement)=> 	statement.showDetails(sourceCode,textBox));
							 
			treeView.afterSelect<List<Statement>>(
				(statements)=>{ 
								if (statements.size() > 0)	
									statements[0].showDetails(sourceCode,textBox);							
							 });			
			treeView.afterSelect<SourceCodeDescriptor>(
				(sourceCodeDescriptor) => sourceCodeDescriptor.showDetails(sourceCode,textBox));
			
			return treeView;
	    }
	    
	    public static Statement highlightInCode(this Statement statement, ascx_SourceCodeViewer codeViewer)
	    {	    	
			//"expression: {0}".info(statement);
			codeViewer.editor().clearBookmarksAndMarkers();
			if (statement.Source.notNull())
				statement.Source.highlightInCode(codeViewer);
			else
				statement.firstSourceCodeReference().highlightInCode(codeViewer);
			codeViewer.editor().refresh();
			return statement;
	    }
	    
	    public static SourceCodeDescriptor highlightInCode(this SourceCodeDescriptor sourceCodeDescriptor, ascx_SourceCodeViewer codeViewer)
	    {
	    	if ( sourceCodeDescriptor!= null)
			{			
				var start = sourceCodeDescriptor.Start;
				var stop = sourceCodeDescriptor.Stop; 								
				codeViewer.editor().selectTextWithColor(start.Line , start.Char + 1, stop.Line , stop.Char+1);
				codeViewer.editor().caret(start.Line, start.Char +1); 								
			}
			return sourceCodeDescriptor;
	    }
	    	    
	    
	    public static Statement showCodeSnippet(this Statement statement, TextBox textBox)
	    {
			textBox.set_Text("");
	    	statement.Source.showCodeSnippet(textBox);
			return statement;			
	    }
	    
		public static SourceCodeDescriptor showCodeSnippet(this SourceCodeDescriptor sourceCodeDescriptor, TextBox textBox)
		{
			if (sourceCodeDescriptor!= null)
	    		textBox.set_Text(sourceCodeDescriptor.Code);
	    	return sourceCodeDescriptor;
		}				
	}
	
	//Jint Stats
	public static class Jint_ExtensionMethods_Stats_View
	{
		public static ascx_TableList add_Jint_Stats_Columns(this ascx_TableList tableList)
		{
			var columnsNames = new string[] { "pos","url","# links", "# images", "# forms", "# js" ,"# all js ok" , "# js functions", "# js values","analyzed js size",  "original js size", "html size"};
			tableList.add_Columns(columnsNames)		 
		 			  .height(tableList.height()-1);  // this will reset the columns widths
			return tableList;
		}
		
		public static ascx_TableList add_Jint_Stats_Row(this ascx_TableList tableList, int index, string url, string html, HtmlAgilityPack.HtmlDocument htmlDocument,  List<KeyValuePair<string,string>> javaScripts)
		{
			var links =  htmlDocument.select("//a");
			var images =  htmlDocument.select("//img");
			var forms =  htmlDocument.select("//form");
			var allJavaScriptsCompileOk = javaScripts.allJavaScriptsCompileOk();
			var allJavaScriptCompiledAst = javaScripts.jint_Compile();
			var sizeOfJointJavaScripts = javaScripts.getStringWithJointJavascripts().size();
			//var scripts =  htmlDocument.select("//script");
			tableList.add_Row( 
							  "{0:00}".format(index),  //(mapped++).str(),  
							  url,							  
							  "{0:000}".format(links.size()),
							  "{0:000}".format(images.size()),
							  "{0:000}".format(forms.size()),
							  "{0:000}".format(javaScripts.size()),
							  allJavaScriptsCompileOk.str(),
							  "{0:0000}".format(allJavaScriptCompiledAst.functions().size()),
							  "{0:0000}".format(allJavaScriptCompiledAst.values().size()),							  
							  sizeOfJointJavaScripts.kBytesStr(),
							  javaScripts.totalValueSize().kBytesStr(),
							  html.size().kBytesStr()
							  );
			tableList.lastRow().textColor( (allJavaScriptsCompileOk) ?  Color.DarkGreen : Color.DarkRed); 
			return tableList;
		}
    }
}
