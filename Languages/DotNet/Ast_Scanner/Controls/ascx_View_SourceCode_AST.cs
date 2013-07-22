// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using FluentSharp.CSharpAST.Utils;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL;
using FluentSharp.REPL.Controls;
using FluentSharp.WinForms;

namespace O2.XRules.Database.Utils
{		
    public class Main
    {    
        public static void openAscx()
		{
			var sourceCodeAST =  "View SourceCode AST".popupWindow<ascx_View_SourceCode_AST>(700,500);
            				
			sourceCodeAST.loadFile(testFileToUse());		
		}
		
		private static string testFileToUse()
		{
		    //var testFile = @"http://o2platform.googlecode.com/svn/trunk/O2 - All Active Projects/O2_XRules_Database/_Rules/_Samples/HelloWorld.cs";
		    //const string testFile = @"http://o2platform.googlecode.com/svn/trunk/O2_Scripts/Languages_and_Frameworks/DotNet/Mono_and_Reflection/ascx_View_SourceCode_AST.cs.o2";
            //return new Web().checkIfFileExistsAndDownloadIfNot(Path.GetFileName(testFile), testFile);
            return "ascx_View_SourceCode_AST.cs".local();
		}
    }
	
	public class ascx_View_SourceCode_AST : UserControl
	{
		public Ast_CSharp 			 	AstCSharp							{ get; set; }
		public AstDetails 			 	AstDetails							{ get; set; }
		
		public SourceCodeEditor 	    sourceCodeEditor 					{ get; set; }
        public TabControl 			 	tabControl				  			{ get; set; }
        public TreeView 				ast_TreeView 				  		{ get; set; }
        public TreeView 				usingDeclarations_TreeView			{ get; set; }
        public TreeView 				types_TreeView						{ get; set; }
        public TreeView 				methods_TreeView					{ get; set; }
        public TreeView 				fields_TreeView						{ get; set; }
        public TreeView 				properties_TreeView					{ get; set; }
        public TreeView 				comments_TreeView					{ get; set; }
        public SourceCodeEditor 	    rewritenCSharpCode_SourceCodeEditor	{ get; set; }
        public SourceCodeEditor 	    rewritenVBNet_SourceCodeEditor		{ get; set; }
		public RichTextBox 				errors_RichTextBox					{ get; set; }
		public bool						AutoExpand_AstTreeView				{ get; set; }
		
		public ascx_View_SourceCode_AST()
		{
			createGUI();					
		}			
		
		public void createGUI()
		{
			var splitControl = this.add_SplitContainer(
        						true, 		//setOrientationToHorizontal
        						true,		// setDockStyleoFill
        						true);		// setBorderStyleTo3D)
        	var topGroupBox = splitControl.Panel1.add_GroupBox("SourceCode");        	        	
        	
            //var bottomGroupBox = splitControl.Panel2.add_GroupBox("Ast");
            
        	sourceCodeEditor = topGroupBox.add_SourceCodeEditor();
        	
        	tabControl = splitControl.Panel2.add_TabControl();
        	    
        	ast_TreeView = tabControl.add_Tab("AST").add_TreeView();
        	usingDeclarations_TreeView = tabControl.add_Tab("Using Declarations").add_TreeView();
        	types_TreeView = tabControl.add_Tab("Types").add_TreeView();       	
        	methods_TreeView = tabControl.add_Tab("Methods").add_TreeView();
        	fields_TreeView = tabControl.add_Tab("Fields").add_TreeView();
        	properties_TreeView = tabControl.add_Tab("Properties").add_TreeView();
        	comments_TreeView = tabControl.add_Tab("Comments").add_TreeView();
        	errors_RichTextBox = tabControl.add_Tab("Errors").add_RichTextBox();
        	rewritenCSharpCode_SourceCodeEditor = tabControl.add_Tab("Re-writen Code : CSharp").add_SourceCodeEditor();        	
        	rewritenVBNet_SourceCodeEditor = tabControl.add_Tab("Re-writen Code : VB.Net").add_SourceCodeEditor();
        	
        	sourceCodeEditor.eDocumentDataChanged += updateView;    
        	
        	usingDeclarations_TreeView.AfterSelect += showInSourceCode;
        	types_TreeView.AfterSelect += showInSourceCode;
        	methods_TreeView.AfterSelect += showInSourceCode;
        	fields_TreeView.AfterSelect += showInSourceCode;
        	properties_TreeView.AfterSelect += showInSourceCode;
        	comments_TreeView.AfterSelect += showInSourceCode;
 		}
							
		public void loadFile(string fileToLoad)
		{				
			PublicDI.log.info("Loading file into SourceCode Editor: {0}", fileToLoad);
			sourceCodeEditor.loadSourceCodeFile(fileToLoad);
		}
				
		
		public void updateView(string sourceCode)
		{	
			AstCSharp 		= new Ast_CSharp(sourceCode);
			AstDetails 		= AstCSharp.AstDetails; 
			
			ast_TreeView.beginUpdate();
			ast_TreeView.show_Ast (AstCSharp);
			if (this.AutoExpand_AstTreeView)
				ast_TreeView.expandAll();
			ast_TreeView.endUpdate();
			
			types_TreeView				.show_List(AstDetails.Types, "Text");
			usingDeclarations_TreeView	.show_List(AstDetails.UsingDeclarations,"Text");
			methods_TreeView			.show_List(AstDetails.Methods,"Text");
			fields_TreeView				.show_List(AstDetails.Fields,"Text");
			properties_TreeView			.show_List(AstDetails.Properties,"Text");
			comments_TreeView			.show_List(AstDetails.Comments,"Text");
			errors_RichTextBox			.set_Text (AstCSharp.Errors);		
			
			rewritenCSharpCode_SourceCodeEditor.setDocumentContents (AstDetails.CSharpCode, ".cs");
			rewritenVBNet_SourceCodeEditor		.setDocumentContents(AstDetails.VBNetCode , ".vb");								
		}




        //NOTE: these two methods need to be moved to the SourceCodeEditor control
        public void showInSourceCode(Object sender, TreeViewEventArgs e)
        {
            var treeNoteTag = e.Node.Tag;
            // temp Hack to handle the fact that AstValue<obTject> is current using  AstValue<object>           
            var endLocation = (ICSharpCode.NRefactory.Location)PublicDI.reflection.getProperty("EndLocation",treeNoteTag);
            var startLocation = (ICSharpCode.NRefactory.Location)PublicDI.reflection.getProperty("StartLocation", treeNoteTag);
            var originalObject = PublicDI.reflection.getProperty("OriginalObject", treeNoteTag);
            var text = (string)PublicDI.reflection.getProperty("Text", treeNoteTag);
            var astValue = new AstValue<object>(text,originalObject,startLocation,endLocation);

            //if (treeNoteTag is AstValue<object>)
            //{
            //var astValue = (AstValue<object>)treeNoteTag;            

            var textEditorControl = sourceCodeEditor.getObject_TextEditorControl();
            var start = new ICSharpCode.TextEditor.TextLocation(astValue.StartLocation.X - 1, astValue.StartLocation.Y - 1);
            var end = new ICSharpCode.TextEditor.TextLocation(astValue.EndLocation.X - 1, astValue.EndLocation.Y - 1);
            var selection = new ICSharpCode.TextEditor.Document.DefaultSelection(textEditorControl.Document, start, end);
            textEditorControl.ActiveTextAreaControl.SelectionManager.SetSelection(selection);
            setCaretToCurrentSelection(textEditorControl);
            //}
        }

        public void setCaretToCurrentSelection(ICSharpCode.TextEditor.TextEditorControl textEditorControl)
        {
            var finalCaretPosition = textEditorControl.ActiveTextAreaControl.TextArea.SelectionManager.SelectionCollection[0].StartPosition;
            var tempCaretPosition = new ICSharpCode.TextEditor.TextLocation
                                        {
                                            X = finalCaretPosition.X,
                                            Y = finalCaretPosition.Y + 10
                                        };
            textEditorControl.ActiveTextAreaControl.Caret.Position = tempCaretPosition;
            textEditorControl.ActiveTextAreaControl.TextArea.ScrollToCaret();
            textEditorControl.ActiveTextAreaControl.Caret.Position = finalCaretPosition;
            textEditorControl.ActiveTextAreaControl.TextArea.ScrollToCaret();
        }
	}
}
