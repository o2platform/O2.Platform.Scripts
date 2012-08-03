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

	
	public static class Extra_Browser
	{
		public static HtmlElement id(this WebBrowser webBrowser, string id)
		{
			return webBrowser.getElementById(id);
		}
		public static List<string> ids(this WebBrowser webBrowser)
		{
			return webBrowser.all().where((htmlElement)=>htmlElement.Id.valid())
								   .Select((htmlElement)=>htmlElement.Id).toList();
		}
		
		public static List<HtmlElement> names(this WebBrowser webBrowser)
		{
			return webBrowser.all().where((htmlElement)=>htmlElement.Name.valid());
		}
		
		public static string html(this HtmlElement htmlElement)
		{
			return htmlElement.outerHtml();
		}
		
	}	

	public static class Extra_String
	{
		public static bool eq(this string _string, params string[] values)
		{
			foreach(var value in values)
				if (_string == value)
					return true;			
			return false;		
		}
		
		public static bool fileName_Is(this string file,  params string[] values)
		{			
			var fileName = file.fileName();
			return fileName.eq(values);
		}
		
		public static bool fileName_Contains(this string file,  params string[] values)
		{			
			var fileName = file.fileName();
			return fileName.contains(values);
		}
 
	}
	
	public static class Extra_TrackBar
	{
		public static int value(this TrackBar trackBar)
		{
			return trackBar.invokeOnThread(()=>	trackBar.Value );
		}
		public static TrackBar value(this TrackBar trackBar, int value)
		{
			return trackBar.invokeOnThread(()=>{ trackBar.Value = value; return trackBar;} );
		}
		
		public static TrackBar onValueChanged(this TrackBar trackBar, Action<int> onSlideCallback)
		{
			return (TrackBar)trackBar.invokeOnThread(
				()=>{
						trackBar.ValueChanged+= (sender,e) => onSlideCallback(trackBar.Value);
						return trackBar;
					});
		}
	}
	public static class Extra_compile_Collections
	{
		public static List<T> remove_Containing<T>(this List<T> list, string text)
		{
			return list.where((value)=> value.str().contains(text).isFalse());
		}
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
    	