// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;   
using System.Threading;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Windows.Forms; 
using System.Collections;   
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Linq;
using System.Xml.Linq;  
using System.Reflection; 
using System.Text;
using System.ComponentModel;
using Microsoft.Win32;
using O2.Interfaces.O2Core;
using O2.Interfaces.O2Findings;
using O2.Kernel;
using O2.Kernel.Objects;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.O2Findings;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.H2Scripts;
using O2.DotNetWrappers.Zip;
using O2.Views.ASCX;
using O2.Views.ASCX.CoreControls;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.External.SharpDevelop.AST;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;

using ICSharpCode.TextEditor;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast; 
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using System.CodeDom;

using O2.Views.ASCX.O2Findings;
using O2.Views.ASCX.DataViewers;
using O2.Views.ASCX.Forms;
using System.Security.Cryptography;


//O2File:_Extra_methods_Browser.cs
namespace O2.XRules.Database.Utils
{	

	public class REPL_Gui
	{
		public Panel 		TopPanel				{ get; set; }
		public Panel 		Code_Panel 				{ get; set; }
		public Panel 		Output_Panel 			{ get; set; }
		public Button		Execute_Button			{ get; set; }
		public RichTextBox 	Output_View_RichTextBox { get; set; }
		public Panel 		Output_View_Object 		{ get; set; }
		public Action 		Execute					{ get; set; }
		public Thread 		ExecutionThread			{ get; set; }
		
		public Action		On_ExecuteCode			{ get; set; }		
		
		public REPL_Gui buildGui(Control targetControl)
		{			
			try
			{
				TopPanel = targetControl.clear().add_Panel();
				
				Code_Panel = TopPanel.insert_Left("Code");
	 
				Output_Panel = TopPanel.add_GroupBox("Invoke and Result")
							  		   .add_GroupBox("Output").add_Panel();
				Execute_Button = Output_Panel.parent().insert_Above(60).add_Button("Execute").fill();
				Execute_Button.insert_Below(20).add_Link("stop execution", () => this.stopCurrentExecution());
				Output_View_RichTextBox = Output_Panel.add_RichTextBox();
				Output_View_Object		= Output_Panel.add_Panel();
				//set actions
				
				Execute_Button.onClick(
					()=>{
							try
							{
								ExecutionThread = O2Thread.mtaThread(()=> On_ExecuteCode.invoke());
							}
							catch(Exception ex)
							{
								ex.log(); 
							}
						});
			}
			catch(Exception ex)
			{
				ex.log("[REPL_Gui] in buildGui");
			}
			return this;
		}	
	}
			
	public static class REPL_Gui_ExtensionMethods
	{
		public static REPL_Gui add_REPL_Gui(this Control targetControl)
		{
			return new REPL_Gui().buildGui(targetControl);
		}
		
		
		public static REPL_Gui stopCurrentExecution(this REPL_Gui replGui)
		{
		  	if (replGui.ExecutionThread.notNull() && replGui.ExecutionThread.IsAlive)
            {
                "ExecutionThread is alive, so stopping it".info();
                replGui.ExecutionThread.Abort();
                replGui.Output_View_RichTextBox.textColor(Color.Red).set_Text("...current thread stopped...");
            }
            return replGui; 
		}
		
		public static REPL_Gui showErrorMessage(this REPL_Gui replGui, string msg)
		{
			replGui.Output_View_Object.visible(false);
			replGui.Output_View_RichTextBox.visible(true).textColor(Color.Red)
                    		   							 .set_Text(msg);	
			return replGui;
		}
		public static REPL_Gui showOutput(this REPL_Gui replGui, object result)
        {
        	var richTextBox = replGui.Output_View_RichTextBox;
        	var panel = replGui.Output_View_Object;
            richTextBox.visible(false);
            panel.visible(false).clear();            

            if (result == null)
                result = "[null value]";

            switch (result.typeName())
            {
                case "Boolean":
                case "String":
                case "Int64":
                case "Int32":
                case "Int16":
                case "Byte":
                    richTextBox.visible(true).textColor(Color.Black)
                    		   				 .set_Text(result.str());
                    break;
                case "Bitmap":
                    panel.visible(true).add_PictureBox()
                    				  .load((Bitmap)result);
                    break;
                default:
                    panel.visible(true).add_Control<ascx_ShowInfo>()
                    	               .show(result);                    
                    break;
            }
            return replGui;
		}
	}

	public static class compilation
	{
		public static object compileAndExecuteCodeSnippet(this string snippet)
		{
			return snippet.compileAndExecuteCodeSnippet((msg)=> msg.info(), (msg)=>msg.error());
		}
		public static object compileAndExecuteCodeSnippet(this string snippet,  Action<string> onCompileOk, Action<string> onCompileFail)
		{
			var assembly = snippet.compileCodeSnippet(onCompileOk, onCompileFail);
			if (assembly.notNull())
				return assembly.type("DynamicType").ctor().invoke("dynamicMethod");
			return null;
		}
		
		public static Assembly compileCodeSnippet(this string snippet)
		{
			return snippet.compileCodeSnippet((msg)=> msg.info(), (msg)=>msg.error());
		}
		
		public static Assembly compileCodeSnippet(this string snippet, Action<string> onCompileOk, Action<string> onCompileFail)
		{
			try
			{
				"[compileAndExecuteCodeSnippet] Compiling code with size: {0}".info(snippet.size());
				
				var codeTemplate = @"using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Kernel;
using O2.Interfaces;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Generic;
using System;
%%EXTRA_USING%%

public class DynamicType
{
	public object dynamicMethod()
	{
//*** Code Start	

%%CODE%%

//*** Code Ends
		//return null;
	}
}";

				//extra extra usings
				var extraUsing = ""; 
				foreach(var usingStatement in snippet.lines().starting("//using"))
					extraUsing+= usingStatement.subString(2).line();
				//do replacements	
				var code = codeTemplate .replace("%%EXTRA_USING%%",extraUsing)
										.replace("%%CODE%%", snippet);
				code.info(); 
				var codeFile = code.saveAs(".cs".tempFile());				
				"Snippet code saved to : {0}".info(codeFile);
				
				var compileEngine = new CompileEngine();
				var assembly =  compileEngine.compileSourceFile(codeFile);			
				if (assembly.notNull())
				{
					onCompileOk("[compileAndExecuteCodeSnippet] Snippet assembly created OK: {0}".format(assembly.location()));
					return assembly;									
				}
				onCompileFail("[compileAndExecuteCodeSnippet] Compilation failed: ".line() + compileEngine.sbErrorMessage.str());
			}
			catch(Exception ex)
			{
				ex.log("[compileAndExecuteCodeSnippet]");
			}
			return null;
		}
	}
	public static class Extra_Collections
	{
		public static List<T> take<T>(this IEnumerable<T> data, int count)
		{
			if (count == -1)
				return data.toList();
			return data.Take(count).toList(); 
		}
	}
	public static class Extra_Items
	{		
		public static List<string> uniqueKeys(this List<Items> itemsList)
		{
			return (from items in itemsList
				    from key in items.keys()
				    select key).distinct();
		}
		
		public static List<string> uniqueKeys_WithValidValue(this List<Items> itemsList)
		{
			return (from items in itemsList
				    from item in items
				    where item.Value.valid()
				    select item.Key).distinct();
		}
		
		public static List<string> values(this Items items, List<string> columns)
		{
			return (from column in columns
					select items[column]).toList();
		}
		
		
		public static Dictionary<string,List<string>> indexBy_Key(this List<Items> itemsList)
		{
			var mappedData = new Dictionary<string, List<string>>();
			foreach(var items in itemsList)
				foreach(var item in items)
					mappedData.add(item.Key, item.Value);
			return mappedData;
		}
		
		
		
	}	
	
	public static class Extra_Controls
	{
		
		
	}

	public static class Extra_String
	{
		
				
 
	}
	
	public static class Extra_TrackBar
	{
		
	}
	public static class Extra_compile_Collections
	{
		
	}
	
	public static class Extra_compile_TreeView
	{
		
		
		

	}
		
	public static class Extra_Controls_MainMenu
	{
		
		
	}	
	public static class _Extra_WinForms_Controls_MainMenu
	{
		
	}
		
	
	/*public static class _Extra_WinForms_Controls_MsgBox
	{
		
	}
	
	public static class _Extra_WinForms_Controls_Browser
	{
				
	}*/		
}
    	