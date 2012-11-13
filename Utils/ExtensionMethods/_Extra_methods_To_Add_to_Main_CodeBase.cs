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
using O2.Platform.BCL.O2_Views_ASCX;
using O2.External.SharpDevelop.AST;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;
using O2.XRules.Database.Utils;
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


namespace O2.XRules.Database.APIs
{	
	
	public static class Extra_TreeView
	{				
		public static List<TreeNode> colorNodes(this List<TreeNode> nodes, Color color)
		{
			if (nodes.notNull() && nodes.size()>0)
			{
				var treeView = nodes.first().treeView();
				treeView.beginUpdate();
				foreach(var node in nodes)
					node.foreColor(color);
				treeView.endUpdate();
			}
			return nodes;
		}		
	}
	public static class Extra_WinForm_Controls_Forms
	{
		public static Image formImage(this string imageKey)
		{
			return (Image)typeof(FormImages).prop(imageKey);
		}
	}
	
	public class ToolStripCheckBox : ToolStripControlHost
    {
        public ToolStripCheckBox()
            : base(new CheckBox())
        {
			this.BackColor = Color.Transparent;
			this.Margin = new Padding(1,3,1,1);			
        }
    }
    
	public static class Extra_WinForm_Controls_ToolStrip
	{
		public static ToolStrip insert_ToolStrip(this Control control)
		{
			var panel = control.insert_Above(30);
			panel.splitContainer().fixedPanel1().@fixed(true);;
			return panel.add_ToolStrip();
		}
		public static ToolStrip insert_Below_ToolStrip(this Control control)
		{
			var panel = control.insert_Below(30);
			panel.splitContainer().fixedPanel2().@fixed(true);;
			return panel.add_ToolStrip();
		}
		public static ToolStrip add_CheckBox(this ToolStrip toolStrip, string text, Action<bool> onValueChange)
		{
			CheckBox checkBox_Ref = null;
			return toolStrip.add_CheckBox(text, ref checkBox_Ref, onValueChange);
		}
		public static ToolStrip add_CheckBox(this ToolStrip toolStrip, string text, ref CheckBox checkBox_Ref)
		{
			return toolStrip.add_CheckBox(text, ref checkBox_Ref, (value)=> {});
		}
		public static ToolStrip add_CheckBox(this ToolStrip toolStrip, string text, ref CheckBox checkBox_Ref, Action<bool> onValueChange)
		{
			return toolStrip.add_CheckBox(text, null, ref checkBox_Ref, onValueChange);
		}
		
		public static ToolStrip add_CheckBox(this ToolStrip toolStrip, string text, Image image, ref CheckBox checkBox_Ref, Action<bool> onValueChange)		
		{
			var checkBox = toolStrip.add_Control<ToolStripCheckBox>().Control as CheckBox;											
			checkBox_Ref = checkBox;		// need to do this because we can't use the ref object inside the lambda method below
			return toolStrip.invokeOnThread(
				()=>{						
						checkBox.Text = text;
						checkBox.Image = image;						
						checkBox.CheckedChanged += (sender,e)=> 
							O2Thread.mtaThread(()=> onValueChange(checkBox.value()));						
						return toolStrip;
					});
			
		}
		//merge with current one add_Button (without the image)
		public static ToolStrip add_Button(this ToolStrip toolStrip, string text, Image image, Action onClick)
		{
			return toolStrip.invokeOnThread(
				()=>{
						var newButton = new ToolStripButton(text);
						newButton.Image = image;
						newButton.Click += (sender,e)=> O2Thread.mtaThread(()=> onClick());
						toolStrip.Items.Add(newButton);
						return toolStrip;
					});
		}
		
		public static ToolStripDropDown add_DropDown(this ToolStrip toolStrip, string text)
		{
			return toolStrip.add_DropDown(text,null);
		}
		public static ToolStripDropDown add_DropDown(this ToolStrip toolStrip, string text,  Image image)
		{
			return toolStrip.invokeOnThread(
				()=>{
						var dropDown = new ToolStripDropDown();
						var menuButton = new ToolStripDropDownButton(text);
						menuButton.Image = image;
						menuButton.DropDown = dropDown;
						toolStrip.Items.Add(menuButton);
						return dropDown;			
					});
		}				
		public static ToolStripDropDown add_DropDown_Button(this ToolStripDropDown dropDown, string text, Action onClick)
		{
			return dropDown.add_DropDown_Button(text, null, onClick);
		}
		
		public static ToolStripDropDown add_DropDown_Button(this ToolStripDropDown dropDown, string text, Image image, Action onClick)
		{
			return dropDown.invokeOnThread(
				()=>{
						var newButton = new ToolStripButton(text);
						newButton.Click += (sender,e)=>  O2Thread.mtaThread(()=> onClick());
						dropDown.Items.Add(newButton);
						return dropDown;
					});
		}		
	}
	public static class Extra_Processes
	{			
		public static List<ProcessModule> modules(this Process process)
		{
			var modules = new List<ProcessModule>();
			try
			{		
				foreach(ProcessModule module in process.Modules)
					modules.Add(module);				
			}
			catch(Exception ex)
			{
				ex.log();				
			}
			return modules;
		}
		public static List<string> names(this List<ProcessModule> modules)
		{
			return modules.Select((module)=> module.ModuleName).toList();
		}
		public static Dictionary<string,ProcessModule> modules_Indexed_by_ModuleName(this Process process)
		{
			//return process.modules().ToDictionary((module)=> module.ModuleName.lower());;		 //doesn't handle duplicate names
			var modulesIndexed = new Dictionary<string,ProcessModule>();
			foreach(var module in process.modules())
				modulesIndexed.add(module.ModuleName.lower(), module);
			return modulesIndexed;
		}
		
		public static Dictionary<string,ProcessModule> modules_Indexed_by_FileName(this Process process)
		{			
			var modulesIndexed = new Dictionary<string,ProcessModule>();
			foreach(var module in process.modules())
				modulesIndexed.add(module.FileName.lower(), module);
			return modulesIndexed;
		}
		
		public static Process process_WithId(this int id)
		{
			return Processes.getProcess(id);
		}
		public static Process process_WithName(this string name)
		{
			return Processes.getProcessCalled(name);
		}
		public static Process process_MainWindow_BringToFront(this Process process)
		{
			if (process.MainWindowHandle != IntPtr.Zero)
				"WindowsBase.dll".assembly()
							 	.type("UnsafeNativeMethods")					 
							 	.invokeStatic("SetForegroundWindow",new HandleRef(null, process.MainWindowHandle)) ;
			else
				"[process_MainWindow_BringToFront] provided process has no main Window".error();
			return process;
		}
		
		public static bool is64BitProcess(this Process process )
	    {
	    	if (process.isNull())
	    	{
	    		"in process.is64BitProcess provided process value was null!".error();
	    		return false;
	    	}
	        bool lIs64BitProcess = false;
	        //if ( System.Environment.Is64BitOperatingSystem ) 
	        //{
	            IsWow64Process( process.Handle, out lIs64BitProcess );
	            return ! lIs64BitProcess; // weirdly the value is reversed
	        //}
	        //"[Is Target Process 64Bit = {0}]".debug(lIs64BitProcess);
	        //return lIs64BitProcess;
	    }
	    
    	[DllImport( "kernel32.dll" )]
    	public static extern bool IsWow64Process( System.IntPtr aProcessHandle, out bool lpSystemInfo );
	}	
	
	public static class Extra_ascx_Simple_Script_Editor
	{
		public static ascx_Simple_Script_Editor add_InvocationParameter(this ascx_Simple_Script_Editor scriptEditor, string parameterName, object parameterObject)
		{
			scriptEditor.InvocationParameters.add(parameterName,parameterObject);
			return scriptEditor;
		}
		public static ascx_Simple_Script_Editor code_Insert(this ascx_Simple_Script_Editor scriptEditor, string textToInsert)
		{
			scriptEditor.Code = textToInsert.line() + scriptEditor.Code;
			return scriptEditor;
		}
		public static ascx_Simple_Script_Editor code_Append(this ascx_Simple_Script_Editor scriptEditor, string textToInsert)
		{
			scriptEditor.Code = scriptEditor.Code.line() +  scriptEditor.Code;
			return scriptEditor;
		}
	}

	public static class Extra_Screenshot
	{
		//based on code from http://stackoverflow.com/a/911225
		public static Bitmap screenshot_MainWindow(this Process process)
		{
			return PrintWindow(process.MainWindowHandle);
		}
	
			
		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
		[DllImport("user32.dll")]
		public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
	
		public static Bitmap PrintWindow(IntPtr hwnd)    
		{       
		    RECT rc;        
		    GetWindowRect(hwnd, out rc);
		
		    Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);        
		    Graphics gfxBmp = Graphics.FromImage(bmp);        
		    IntPtr hdcBitmap = gfxBmp.GetHdc();        
		
		    PrintWindow(hwnd, hdcBitmap, 0);  
		
		    gfxBmp.ReleaseHdc(hdcBitmap);               
		    gfxBmp.Dispose(); 
		
		    return bmp;   
		}
		
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
		    private int _Left;
		    private int _Top;
		    private int _Right;
		    private int _Bottom;
		
		    public RECT(RECT Rectangle) : this(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom)
		    {
		    }
		    public RECT(int Left, int Top, int Right, int Bottom)
		    {
		        _Left = Left;
		        _Top = Top;
		        _Right = Right;
		        _Bottom = Bottom;
		    }
		
		    public int X {
		        get { return _Left; }
		        set { _Left = value; }
		    }
		    public int Y {
		        get { return _Top; }
		        set { _Top = value; }
		    }
		    public int Left {
		        get { return _Left; }
		        set { _Left = value; }
		    }
		    public int Top {
		        get { return _Top; }
		        set { _Top = value; }
		    }
		    public int Right {
		        get { return _Right; }
		        set { _Right = value; }
		    }
		    public int Bottom {
		        get { return _Bottom; }
		        set { _Bottom = value; }
		    }
		    public int Height {
		        get { return _Bottom - _Top; }
		        set { _Bottom = value + _Top; }
		    }
		    public int Width {
		        get { return _Right - _Left; }
		        set { _Right = value + _Left; }
		    }
		    public Point Location {
		        get { return new Point(Left, Top); }
		        set {
		            _Left = value.X;
		            _Top = value.Y;
		        }
		    }
		    public Size Size {
		        get { return new Size(Width, Height); }
		        set {
		            _Right = value.Width + _Left;
		            _Bottom = value.Height + _Top;
		        }
		    }
		
		    public static implicit operator Rectangle(RECT Rectangle)
		    {
		        return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
		    }
		    public static implicit operator RECT(Rectangle Rectangle)
		    {
		        return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
		    }
		    public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
		    {
		        return Rectangle1.Equals(Rectangle2);
		    }
		    public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
		    {
		        return !Rectangle1.Equals(Rectangle2);
		    }
		
		    public override string ToString()
		    {
		        return "{Left: " + _Left + "; " + "Top: " + _Top + "; Right: " + _Right + "; Bottom: " + _Bottom + "}";
		    }
		
		    public override int GetHashCode()
		    {
		        return ToString().GetHashCode();
		    }
		
		    public bool Equals(RECT Rectangle)
		    {
		        return Rectangle.Left == _Left && Rectangle.Top == _Top && Rectangle.Right == _Right && Rectangle.Bottom == _Bottom;
		    }
		
		    public override bool Equals(object Object)
		    {
		        if (Object is RECT) {
		            return Equals((RECT)Object);
		        } else if (Object is Rectangle) {
		            return Equals(new RECT((Rectangle)Object));
		        }
		
		        return false;
		    }
		}
 
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
    	