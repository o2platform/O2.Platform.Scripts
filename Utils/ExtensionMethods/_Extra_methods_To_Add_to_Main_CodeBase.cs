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
	public static class Extra_compile_Control
	{
	
		public static T font<T>(this T control, Font font) where T : Control
		{
			return control.invokeOnThread(()=>{
												 control.Font = font;
												 return control;
											  });			
		}
		 
/*		public static T backColor<T>(this T control, string colorName) where T : Control
    	{
    		return control.backColor(colorName.color());    			
    	}
*/    	
    	public static T foreColor<T>(this T control, string colorName) where T : Control
    	{
    		return control.foreColor(colorName.color());    			
    	}
    	
    	public static T textColor<T>(this T control, string colorName) where T : Control
    	{
    		return control.foreColor(colorName);
    	}
    	public static T color<T>(this T control, string colorName) where T : Control
    	{
    		return control.foreColor(colorName);
    	}
    	
		//not working (more work is needed to add drag and drop support to Images (and other WinForm controls)
		/*public static T  onDrag<T, T1>(this T control, Func<T, T1> getDragData) where T : UserControl
        {
            control.ItemDrag += (sender, e) =>
            {
                var dataToDrag = getDragData(tag);                
                    if (dataToDrag != null)
                        control.DoDragDrop(dataToDrag, DragDropEffects.Copy);                    
            };
            return control;
        }*/
		
	}
	public static class Extra_WinForm_Controls_Drawing
	{
		public static Color color(this string colorName)
		{
			var color = Color.FromName(colorName);
	    	if (color.IsKnownColor.isFalse())
	    		"In color, provided colorName was not found: {0}".error(colorName);
	    	return color;
		}    	
		public static Font font(this string fontName)
		{
			return fontName.font(8);
		}
		public static Font font(this string fontName, int size)
		{
			return new Font(fontName, size);
		}
		
		public static Font style_Add(this Font font, FontStyle fontStyle)
		{
			var currentStyle = (FontStyle)font.field("fontStyle");
			var newStyle = currentStyle | fontStyle;
			font.field("fontStyle", newStyle);
			return font;
		}
		
		public static Font bold(this Font font)
		{
			return font.style_Add(FontStyle.Bold);
		}
		public static Font italic(this Font font)
		{
			return font.style_Add(FontStyle.Italic);
		}	
		/*public static Font regular(this Font font)			// this requires a different approach
		{
			return font.style_Add(FontStyle.Regular);
		}*/
		public static Font strikeout(this Font font)
		{
			return font.style_Add(FontStyle.Strikeout);
		}
		public static Font underline(this Font font)
		{
			return font.style_Add(FontStyle.Underline);
		}
	}
	
	public static class Extra_WinForm_Controls_ImageList
	{
		public static ImageList imageList(this TreeView treeView)
		{
			return treeView.invokeOnThread(
				()=>{
						if (treeView.ImageList.isNull())
							treeView.ImageList = new ImageList();
						return treeView.ImageList;
					});
		}
		/* we can't do this because of cross thread error, but I can't see how we can get the thread value from the Image list (even though it is already connected to a Treeview)
		public static ImageList add(this ImageList imageList, params string[] keys)
		{
			foreach(var key in keys)
				imageList.add(key, key.formImage());
			return imageList;
		}
		public static ImageList add(this ImageList imageList, string key, Image image)
		{
			if (key.notNull() && image.notNull())
				imageList.Images.Add(key, image);
			return imageList;
		}*/
		
		public static TreeView add_ToImageList(this TreeView treeView, params string[] keys)
		{
			foreach(var key in keys)
				treeView.add_ToImageList(key, key.formImage());
			return treeView;
		}
		public static TreeView add_ToImageList(this TreeView treeView, string key, Image image)
		{
			return treeView.invokeOnThread(
				()=>{
						if (key.notNull() && image.notNull()) 
							treeView.imageList().Images.Add(key, image);
						return treeView;
					});
		}
		
		
	}
	
	public static class Extra_WinForm_Controls_TreeView
	{
		public static TreeView checkBoxes(this TreeView treeView)
		{
			return treeView.checkBoxes(true);
		}
	}
	
	public static class Extra_WinForm_Controls_TreeNode
    {
    	public static TreeNode backColor(this TreeNode treeNode, string colorName)
    	{
    		return treeNode.backColor(colorName.color());    			
    	}
    	
    	public static TreeNode foreColor(this TreeNode treeNode, string colorName)
    	{
    		return treeNode.foreColor(colorName.color());    			
    	}
    	
    	public static TreeNode textColor(this TreeNode treeNode, string colorName)
    	{
    		return treeNode.foreColor(colorName);
    	}
    	public static TreeNode color(this TreeNode treeNode, string colorName)
    	{
    		return treeNode.foreColor(colorName);
    	}
    	
    	public static TreeNode font(this TreeNode treeNode, Font font)
    	{
			treeNode.treeView().invokeOnThread(()=> treeNode.NodeFont = font);
			return treeNode;
    	}
		public static TreeNode @checked(this TreeNode treeNode)
		{
			return treeNode.@checked(true);
		}
		public static TreeNode @checked(this TreeNode treeNode, bool value)
    	{
    		treeNode.treeView().invokeOnThread(()=> treeNode.Checked = value);
    		return treeNode;
    	}
    	
    	public static TreeNode image(this TreeNode treeNode, string key)
    	{
    		return treeNode.treeView().invokeOnThread(
    			()=>{
    					treeNode.ImageKey = key;
    					return treeNode;
    				});
    	}
    	public static TreeNode image(this TreeNode treeNode, int index)
    	{
    		return treeNode.treeView().invokeOnThread(
    			()=>{
    					treeNode.ImageIndex = index; 
    					return treeNode;
    				});
    	}
    }
	
	public static class Extra_WinForm_Controls_Label
	{
		public static Label append_Label(this Control control, string text, int top)
		{
			return control.append_Label(text).top(top);
		}
		
		public static Label append_Label(this Control control, string text, int top, ref Label label)
		{
			return label = control.append_Label(text).top(top);
		}
		
		public static Label bold(this Label label)
		{
			return label.font_bold();
		}
	}
	public static class Extra_compile_SplitContainer
	{		
		
		
	}

	public static class Extra_compile_TextBox
	{
		
	}
	public static class Extra_compile_PictureBox
	{
		
		
	}
		
	public static class Extra_Controls_ToolStip
	{
		
	 
		
	}	
	public static class _Extra_WinForms_Controls_MainMenu
	{
		
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
}
    	