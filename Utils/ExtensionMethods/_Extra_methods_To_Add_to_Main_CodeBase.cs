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

	
	public static class Extra_Embedded
	{
		public static string copyFileReferencesToEmbeddedFolder(this string targetFolder, string sourceToParse)
		{
			//"[copyFileReferencesToEmbeddedFolder] analyzing file: {0} with {1} lines".error(sourceToParse.local(), sourceToParse.local().lines().size());
			var tag = "//O2Package:";
			var sourceCode = sourceToParse.extension(".h2") ? sourceToParse.local().h2_SourceCode().fixCRLF()
															: sourceToParse.local().fileContents().fixCRLF();
			foreach(var line in sourceCode.lines())
				if(line.starts(tag))
				{
					var file = line.remove(tag).local();
					"[copyFileReferencesToEmbeddedFolder] found Package reference: {0}".debug(file);				
					file.file_Copy(targetFolder);
				}
			return targetFolder;
		}
	}	


	
	
	public static class Extra_TextArea
	{
		public static string helloMe(this string name)
		{
			return "hello {0}".info(name);
		}
		
	}
	public static class Extra_ToolStrip
	{
	

	}
	
	public static class Extra_textBox
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
	public static class Extra_Controls
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
    	