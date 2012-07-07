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
using System.Security.Cryptography;

using Ionic.Zip;

//O2Ref:Ionic.Zip.dll

//O2File:_Extra_methods_AppDomain.cs

namespace O2.XRules.Database.Utils
{	
	public class Native
	{
		public string test { get;set; }
		 // P/Invoke declarations
	    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	    public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
	
	    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	    public static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);
	
	    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	    public static extern bool InsertMenu(IntPtr hMenu, int uPosition, int uFlags, int uIDNewItem, string lpNewItem);
	}

	public static class _Extra_WinForms_Controls
	{
		public static T onThread<T>(this T control, Action action) where T : Control
		{
			return control.update((c)=> action());
		}
		
		public static T onThread<T>(this T control, Action<T> action) where T : Control
		{
			return (T)control.invokeOnThread(
					()=>{						
							action(control);
							return control;
						});
		}
		
		public static T update<T>(this T control, Action updateAction) where T : Control
		{
			return control.update((c)=> updateAction());
		}
		
		public static T update<T>(this T control, Action<T> updateAction) where T : Control
		{
			return (T)control.invokeOnThread(
						()=>{
								control.refresh_Disable();
								updateAction(control);
								control.refresh_Enable();
								return control;
							});
		}
		
		public static T refresh_Disable<T>(this T control) where T : Control
		{
			return control.beginUpdate();
		}
		
		public static T refresh_Enable<T>(this T control) where T : Control
		{
			return control.endUpdate();
		}
		
		public static T beginUpdate<T>(this T control) where T : Control
		{
			control.invoke("BeginUpdateInternal");
			return control;
		}
		
		public static T endUpdate<T>(this T control) where T : Control
		{
			control.invoke("EndUpdateInternal");
			return control;
		}
	}
	
	public static class _Extra_SourceCodeViewer
	{		
		public static Location location(this TextLocation textLocation)
		{
			return new Location(textLocation.X+1, textLocation.Y +1);		
		}
		
		public static ascx_SourceCodeViewer selectText(this ascx_SourceCodeViewer codeViewer, int offsetStart, int offsetEnd)
		{
			codeViewer.editor().selectText(offsetStart, offsetEnd);
			return codeViewer;
		}
		
		public static ascx_SourceCodeEditor selectText(this ascx_SourceCodeEditor codeEditor, int offsetStart, int offsetEnd)		
		{
			codeEditor.textEditor().invokeOnThread(
				()=>{
						try
						{
							var position1 = codeEditor.document().OffsetToPosition(offsetStart).location();
							var position2 = codeEditor.document().OffsetToPosition(offsetEnd).location();											
							codeEditor.setSelectionText(position1, position2);
						}
						catch(Exception ex)
						{
							ex.log("in ascx_SourceCodeEditor selectText");
						}
					});
			return codeEditor;
		}
		public static ascx_SourceCodeViewer add_CodeMarker(this ascx_SourceCodeViewer codeViewer, int offsetStart, int offsetEnd)
		{
			codeViewer.editor().add_CodeMarker(offsetStart, offsetEnd);
			return codeViewer;
		}
		
		public static ascx_SourceCodeEditor add_CodeMarker(this ascx_SourceCodeEditor codeEditor, int offsetStart, int offsetEnd)
		{
			codeEditor.textEditor().invokeOnThread(
				()=>{
						var position1 = codeEditor.document().OffsetToPosition(offsetStart);
						var position2 = codeEditor.document().OffsetToPosition(offsetEnd);					
						codeEditor.clearMarkers();
						codeEditor.selectTextWithColor(position1, position2)
											  .refresh();
						codeEditor.document().MarkerStrategy.TextMarker.first().field("color", Color.Azure);					  
					});
			return codeEditor;
		}
		
		public static ascx_SourceCodeViewer markerColor(this ascx_SourceCodeViewer codeViewer, Color color)
		{
			codeViewer.editor().markerColor(color);
			return codeViewer;
		}
		
		public static ascx_SourceCodeEditor markerColor(this ascx_SourceCodeEditor codeEditor, Color color)
		{
			codeEditor.textEditor().invokeOnThread(
				()=>{
						foreach(var marker in codeEditor.document().MarkerStrategy.TextMarker)
							marker.field("color", color);
					});
			return codeEditor;	
		}
			
	}
	public static class _Extra_Assembly
	{
		public static object invokeStatic(this Assembly assembly, string type, string method, params object[] parameters)
		{
			return assembly.type(type).invokeStatic(method, parameters);
		}
	}
	
	public static class _Extra_String
	{
		public static string subString_Before(this string stringToProcess, string untilString)
		{
			var lastIndex = stringToProcess.indexLast(untilString);
			return (lastIndex > 0) 
						? stringToProcess.subString(0, lastIndex) 
						: stringToProcess;		
		}
	}
	
	public static class _Extra_Methods_MessageBox
	{
		public static DialogResult alert(this string message, string title = null)
		{
			return message.messageBox(title);
		}
		
		public static DialogResult msgbox(this string message, string title = null)
		{
			return message.messageBox(title);
		}
		
		public static DialogResult messageBox(this string message, string title = null)
		{
			title = title ?? "O2 MessageBox";
			return MessageBox.Show(message,title);
		}
	}
	
	public static class _Extra_O2_BitMap
	{
		public static Icon asIcon(this Bitmap bitmap)
		{
			return Icon.FromHandle(bitmap.GetHicon());
		}
	}
	
	
	
	// Other extension method classes	
	
	public static class _Extra_ConfigFiles_extensionMethods
	{
		// Config files (can't easily put this on the main
        public static Panel editLocalConfigFile(this string file)
        {
            var panel = O2Gui.open<Panel>("Editing local config file: {0}".format(file), 700, 300);
            return file.editLocalConfigFile(panel);
        }
	}	
	
	public static class _Extra_Xml_XSD_ExtensionMethods
	{
		//replace current xml_CreateCSharpFile with this one (inside O2.External.SharpDevelop.ExtensionMethods)
		public static string xmlCreateCSharpFile_Patched(this string xmlFile)
		{
			var csharpFile = "{0}.cs".format(xmlFile); //xmlFile.replace(".xml",".cs");
			return xmlFile.xmlCreateCSharpFile_Patched(csharpFile);
		}
		
		public static string xmlCreateCSharpFile_Patched(this string xmlFile, string csharpFile)
		{
			var xsdFile = "{0}.xsd".format(xmlFile) ;// xmlFile.replace(".xml",".xsd");
			return xmlFile.xmlCreateCSharpFile_Patched(xsdFile, csharpFile);
		}
		
		public static string xmlCreateCSharpFile_Patched(this string xmlFile, string xsdFile, string csharpFile)
		{								
			if (xsdFile.dirExists())
				xsdFile = xsdFile.pathCombine("{0}.xsd".format(xmlFile.fileName()));
			if (csharpFile.dirExists())
				csharpFile = csharpFile.pathCombine("{0}.cs".format(xmlFile.fileName()));
				
			xmlFile.xmlCreateXSD().saveAs(xsdFile);
			if (xsdFile.fileExists())
			{
				"Created XSD for Xml File: {0}".info(xmlFile);	 
				var tempCSharpFile = xsdFile.xsdCreateCSharpFile();
				tempCSharpFile.fileContents()
					          .insertBefore("//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll".line())
					   	      .saveAs(csharpFile);				
				if (csharpFile.fileExists())
				{
					"Created CSharpFile for Xml File: {0}".info(csharpFile);	
					if (tempCSharpFile != csharpFile)
						File.Delete(tempCSharpFile);
				}	
				return csharpFile;
			}
			return null;	
		}
	}		
}
    	