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
	
	public static class Extra_AppDomains
	{				
		//public static O2AppDomainFactory appDomain(this string name) //to also be called appDomain when moved to main codebase
		public static O2AppDomainFactory appDomain_Get(this string name)
		{
			if (O2AppDomainFactory.AppDomains_ControledByO2Kernel.hasKey(name))
				return O2AppDomainFactory.AppDomains_ControledByO2Kernel[name];
			return null;
		}
		
		public static bool isNotCurrentAppDomain(this O2AppDomainFactory appDomainFactory)
		{
			return appDomainFactory.isCurrentAppDomain().isFalse();
		}
		
		public static bool isCurrentAppDomain(this O2AppDomainFactory appDomainFactory)
		{
			return appDomainFactory.appDomain != AppDomain.CurrentDomain;
		}
	}
	public static class Extra_WinForm_Controls_PropertyGrid
	{
		public static PropertyGrid add_PropertyGrid(this Control control, bool helpVisible)
		{
			return control.add_PropertyGrid().helpVisible(helpVisible);
		}
	}
	
	
    public static class Extra_WinForm_Controls_TreeView
    {
    	public static TreeView selectSecond(this TreeView treeView)
    	{
    		treeView.nodes().second().selected();
    		return treeView;
    	}

		public static TreeView selectThird(this TreeView treeView)
    	{
    		treeView.nodes().third().selected();
    		return treeView;
    	}    	
    	
    }
	public static class Extra_WinForm_Controls_ToolStrip
	{
		/*public static ToolStrip insert_Below_ToolStrip(this Control control)
		{
			var tooStrip = control.insert_Above(30).add_ToolStrip();
			tooStrip.splitContainerFixed();
			return tooStrip;
		}*/
		
		/*public static ToolStrip add(this ToolStrip toolStrip, string text)
		{
			toolStrip.add_Button(text);
			return toolStrip;
		}*/
		
		
		
		
	}
	public static class Extra_Processes
	{			
		[DllImport( "kernel32.dll" )]
    	public static extern bool IsWow64Process( System.IntPtr aProcessHandle, out bool lpSystemInfo );
		
		//this is not working correctly
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
	   
	}	
	
	public static class Extra_ascx_Simple_Script_Editor
	{
		
	}

	//move this to Win_API classes
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
    	