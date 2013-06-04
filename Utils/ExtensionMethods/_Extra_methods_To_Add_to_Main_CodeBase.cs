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
using O2.API.AST.ExtensionMethods.CSharp;
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

using Ionic.Zip;
//O2Ref:Ionic.Zip.dll


namespace O2.XRules.Database.APIs
{	

	public static class Extra_Misc
	{			
		// do this propely when adding to main O2 code base since this will not work as expected when there are other textboxes and buttons on the same 'control' object
		public static T add_LabelAndTextAndButton<T>(this T control, string labelText, string textBoxText, string buttonText,ref TextBox textBox, ref Button button, Action<string> onButtonClick)            where T : Control
		{
			control.add_LabelAndTextAndButton(labelText,textBoxText,buttonText, onButtonClick);
			textBox = control.control<TextBox>();
			button = control.control<Button>();
			return control;
		}
	}

	public static class Extra_WinForm_Controls_TreeView
	{		
	}
	
	public static class Extra_WinForm_Controls_Control
	{		
	}
	public static class Extra_CryptoMethods
	{
	
	}

	public static class Extra_Assembly
	{		
	}
	
	public static class Extra_compile_Collections
	{		
	}

	public static class Extra_WinForm_Controls_DnD_Images
	{	
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
	
	public static class Extra_WinForm_Controls_Menu
	{		
	}
	public static class Extra_WinForm_Controls_Drawing
	{		   
	}
	
	public static class Extra_WinForm_Controls_ImageList
	{
		
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
    	