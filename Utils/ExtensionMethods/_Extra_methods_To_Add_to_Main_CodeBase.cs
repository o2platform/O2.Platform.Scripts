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
	
	public static class Extra_Object
	{				
		public static T wait_n_Seconds<T>(this T _object, int seconds)
		{
			return _object.wait(seconds * 1000);
		}
	}
	
	public static class Extra_WinForm_Controls_ToolStripLayout
	{
		public static ToolStrip layoutStyle(this ToolStrip toolStrip, ToolStripLayoutStyle layoutStyle)
		{
			return toolStrip.invokeOnThread(
				()=>{ 
						toolStrip.LayoutStyle = layoutStyle;
						return toolStrip;
					});			
		}
		
		public static ToolStrip layout_Flow(this ToolStrip toolStrip)
		{
			return toolStrip.layoutStyle(ToolStripLayoutStyle.Flow);
		}
		
		public static ToolStrip layout_HorizontalStackWithOverflow(this ToolStrip toolStrip)
		{
			return toolStrip.layoutStyle(ToolStripLayoutStyle.HorizontalStackWithOverflow);
		}
		
		public static ToolStrip layout_VerticalStackWithOverflow(this ToolStrip toolStrip)
		{
			return toolStrip.layoutStyle(ToolStripLayoutStyle.VerticalStackWithOverflow);
		}
		
		public static ToolStrip layout_Table(this ToolStrip toolStrip)
		{
			return toolStrip.layoutStyle(ToolStripLayoutStyle.Table);
		}
		
		public static ToolStrip layout_StackWithOverflow(this ToolStrip toolStrip)
		{
			return toolStrip.layoutStyle(ToolStripLayoutStyle.StackWithOverflow);
		}				
	}
	public static class Extra_Items
	{						
		
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
    	