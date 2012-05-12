// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Drawing;
using System.Collections;
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
using O2.External.SharpDevelop.Ascx;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.External.IE.ExtensionMethods;
using O2.External.IE.Wrapper;
using O2.External.IE.WebObjects;
using Jint;
using Jint.Expressions;
//O2File:Jint_ExtensionMethods.cs
//O2Ref:Jint.dll
//O2Ref:Antlr3.Runtime.dll
//O2Ref:O2_External_IE.dll


namespace O2.XRules.Database.Languages_and_Frameworks.Javascript
{
    public class ascx_Javascript_AST : Control
    {    
    	public bool RenderViewAstElementsTreeView { get; set; }    	
    	
    	public ascx_SourceCodeViewer sourceCode;
    	public ComboBox showJavascriptsFromUrl;
    	public ComboBox pagesVisited;
    	public TreeView javascriptCode;
    	public TreeView jsAST;
    	public TreeView jsFunctions;
    	public TreeView jsIdentifiers;
    	public TreeView jsValues;
		public TreeView allAST;
    	public TextBox codeSnippet;
    	public TabControl tabControl;
    	public Panel javaScriptLoadMessage;
    	
    	//public static string defaultXssPoC = @"http://ha.ckers.org/xss.js";
    	public static string defaultXssPoC = @"http://www.google.com";
    	
    	public static void launchGui()
    	{
    		O2Gui.open<ascx_Javascript_AST>("Javascript AST Viewer",1000,400)
    			 .buildGui(true /* addUrlLoadTextBox */ )
    			 .showJavascriptsFromUrl.sendKeys(defaultXssPoC.line());
    	}
    	 
		public ascx_Javascript_AST()
		{
		}
		
		public ascx_Javascript_AST buildGui()
		{
			return buildGui(false);
		}
		//bool putJavaScriptCodeViewerOnTheLeft,
		public ascx_Javascript_AST buildGui( bool addUrlLoadTextBox)
		{
			var mainGui = this.add_1x1("Files or ScriptBlocks","Javascript Source (you can edit this code and see the results in realtime)");
			var splitContainer = this.controls<SplitContainer>();
			if (addUrlLoadTextBox)			
			{
				showJavascriptsFromUrl = 
				splitContainer.insert_Above<Panel>(25)
							  .add_LabelAndComboBoxAndButton("Enter Url to load Javascripts","","Open",  loadJavascriptsFromUrl)
							  .controls<ComboBox>();								  
			}
			/*if (putJavaScriptCodeViewerOnTheLeft)
			{				
				splitContainer.splitterDistance(this.width()/3);
				javascriptCode = mainGui[1].add_TreeView().showSelection().sort();
				sourceCode = mainGui[0].add_SourceCodeViewer(); 				
			}
			else			
			{*/
				javascriptCode = mainGui[0].add_TreeView().showSelection().sort();
				sourceCode = mainGui[1].add_SourceCodeViewer(); 
			//}
			pagesVisited = javascriptCode.insert_Above<ComboBox>(25).dropDownList();
			
			codeSnippet = sourceCode.insert_Below<TextBox>(100).multiLine().scrollBars(); 
			tabControl  = javascriptCode.insert_Below<TabControl>();
			
			jsAST = tabControl.add_Tab("Javascript - View Ast Tree")
								  .add_TreeView()
								  .showSelection();
								  
			jsFunctions = tabControl.add_Tab("JScript: Functions")
								  		.add_TreeView()
								  		.showSelection()
								  		.sort();
			
			jsIdentifiers = tabControl.add_Tab("JScript: Identifiers")
								  		  .add_TreeView()
								  		  .showSelection()
								  		  .sort();

			jsValues = tabControl.add_Tab("JScript: Values")
								  	 .add_TreeView()
								  	 .showSelection()
								  	 .sort();
								  		
			
			allAST = tabControl.add_Tab("Javascript - View Ast Elements")					    
								   .add_TreeView()
								   .showSelection()
								   .sort();
			
			var searchTab = tabControl.add_Tab("Search in Code")
									  .add_LabelAndComboBoxAndButton("search for (case sensitive)","","show",
									  	(text)=> {
									  				sourceCode.editor().invoke("searchForTextInTextEditor_findNext", text);
									  			 });
									  					  
			//tabControl.select_Tab(searchTab);
			javaScriptLoadMessage = javascriptCode.insert_Below<Panel>(20);
													  
			allAST.insert_Below<Panel>(25)
				  .add_CheckBox("Render this view (some performace impact on large scripts)", 0,0, 
				  		(value)=>{
				  					RenderViewAstElementsTreeView = value;
				  					processJavascript();
				  				 })
				  .autoSize(); 
				  
				  
			allAST.jint_configure_showSelectionDetails(sourceCode, codeSnippet); 
			jsFunctions.jint_configure_showSelectionDetails(sourceCode, codeSnippet); 
			jsIdentifiers.jint_configure_showSelectionDetails(sourceCode, codeSnippet); 
			jsValues.jint_configure_showSelectionDetails(sourceCode, codeSnippet); 

			javascriptCode.afterSelect<string>(
				(jsCode) => {
								sourceCode.editor().clearBookmarksAndMarkers();
								sourceCode.set_Text(jsCode,"*.js");
								sourceCode.editor().refresh(); 
							});
			
			sourceCode.onTextChanged(processJavascript);
			
			pagesVisited.onSelection<IE_HtmlPage>(
				(htmlPage)=>{
								var allScriptsCompiledOk = javascriptCode.populateWithHtmlPageScripts(htmlPage);
								javascriptCode.add_Node("zzz [Original Html Code for: {0}]".format(htmlPage.PageUri.str()),htmlPage.PageSource);
								handleCompilationResult(allScriptsCompiledOk);								
							});
							
			pagesVisited.onSelection<Jint_Wrapper>(
				(jintWrapper)=>{									
									var allScriptsCompiledOk = javascriptCode.populateWithHtmlPageScripts(jintWrapper.JavaScripts);
									javascriptCode.add_Node("zzz_[Original Code for: {0}]".format(jintWrapper.Uri.str()),jintWrapper.Html);
									handleCompilationResult(allScriptsCompiledOk);
							   });
			
			javascriptCode.onDrop(
				(fileOrFolder)=>{
									if (fileOrFolder.fileExists()) 
										javascriptCode.add_Node(fileOrFolder.str(), fileOrFolder.fileContents());
									else if (fileOrFolder.dirExists()) 
									{
										javascriptCode.clear();
										foreach(var file in fileOrFolder.files("*.js",true))
											javascriptCode.add_Node(file.str(), file.fileContents()); 
									}
								});


			this.add_ContextMenu().add_MenuItem("Show log viewer",()=> this.insert_Below<Panel>(90).add_LogViewer());
			
			return this;

		}
		
		public void processJavascript()
		{
			processJavascript(sourceCode.get_Text());
		}
		
		public void processJavascript(string text)
		{
			O2Thread.mtaThread(
			()=>{					
					var compile =  text.jint_Compile(); 
					codeSnippet.set_Text("");
					if (compile.isNull())
					{
						javascriptCode.backColor(Color.Pink);
						allAST.clear();
						jsAST.clear();
						jsFunctions.clear();
						jsIdentifiers.clear();
						jsValues.clear();
					}
					else
					{
						"Javascripts Compile Ok (refreshing details view)".info();
						javascriptCode.backColor(Color.Azure);								
						jsAST.jint_Show_AstTree(compile, sourceCode, codeSnippet);								
						jsFunctions.jint_Show_Functions(compile);				
						jsIdentifiers.jint_Show_Statements<Identifier>(compile,false);
						jsValues.jint_Show_Statements<ValueExpression>(compile,false);
						if (RenderViewAstElementsTreeView)								 
							allAST.jint_Show_AstMappings(compile);		
						else
							allAST.clear();			
					}
					
				});
		}


		public void handleCompilationResult(bool allScriptsCompiledOk)
		{
			if (allScriptsCompiledOk)
			{				
				javaScriptLoadMessage.clear().backColor(Color.LightGreen).add_Label("All scripts compiled OK");
			}
			else
				javaScriptLoadMessage.clear().backColor(Color.LightPink).add_Label("NOT all scripts compiled OK");
			javascriptCode.selectFirst();
		}

		
		public void loadJavascriptsFromUrl(string url)
		{			
			if (url.isUri())
			{
				O2Thread.mtaThread(
					()=>{
							"loading Javascripts from url: {0}".info(url);
							var jintWrappers = new Jint_Wrapper(url.uri());				
							pagesVisited.insert_Item(jintWrappers);
							pagesVisited.select_Item(0);
						});
			}
		}
    }
}
