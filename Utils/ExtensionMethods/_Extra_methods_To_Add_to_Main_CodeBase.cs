// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.WinForms;
using Ionic.Zip;
//O2Ref:Ionic.Zip.dll


namespace FluentSharp.CoreLib
{	
	public static class Extra_Web
	{
		public static string GET(this string url, string cookies)
		{			
			return new Web().getUrlContents(url,cookies, false);
		}
		
		public static string GET(this Web web, string url)
		{
			if (web.notNull())
				return web.getUrlContents(url);
			return null;
		}
	}
	public static class Extra_Zip
	{
		public static bool isZipFile(this string zipFile)
		{
			if (zipFile.fileExists())
			{
				try
				{
					var entries = new ZipFile(zipFile).Entries;
					return entries.size() >0;
				}
				catch(Exception ex)
				{
					ex.log("[zipFile] isZipFile");
				}				
			}
			return false;
		}
	}

	public static class Extra_Process
	{
		public static Process       startProcess(this string processExe, string arguments, string workingDirectory)
		{
			return processExe.startProcess(arguments, workingDirectory, (line)=>line.info());
		}
		
		public static Process       startProcess(this string processExe, string arguments, string workingDirectory,  Action<string> onDataReceived)
		{
			return Processes.startProcessAndRedirectIO(processExe, arguments, workingDirectory, onDataReceived, onDataReceived);
		}		
	}
	public static class Extra_Icon
	{
		public static Icon file_Icon(this string filePath)
		{
			if(filePath.fileExists())
			{
				try
				{
					return Icon.ExtractAssociatedIcon(filePath);
				}
				catch(Exception ex)
				{
					ex.log("[filePath].file_Icon");
				}
			}
			return null;
		}
		
		public static Image toImage(this Icon icon)
		{
			if (icon.notNull())
				return icon.ToBitmap();
			return null;
			
		}
	}
	
	public static class Extra_IO
	{
		public static string fileContents(this string folder, string file)
		{
			return folder.pathCombine(file).fileContents();
		}
		public static List<string> fileNames(this string folder)
		{
			return folder.files().fileNames();
		}
		public static string folder(this string basePath, string folderName)
		{
			var targetFolder = basePath.pathCombine(folderName);
			if(targetFolder.dirExists())
				return targetFolder;
			return null;
		}
		
		public static bool file_Doesnt_Exist(this string file)
		{
			return file.file_Exists().isFalse();
		}
		public static bool file_Exists(this string file)
		{
			return file.fileExists();
		}
	}
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
	public static class Extra_Lines
	{
		public static List<string> lines_RegEx(this string target, string regEx)
		{
			return target.lines().containing_RegEx(regEx);
		}
		public static List<string> containing_RegEx(this List<string> items, string regEx)
		{
			return items.where(item=>item.regEx(regEx));
		}
	}

	public static class Extra_WinForm_Controls_ToolStrip
	{
		public static ToolStripDropDown add_DropDown_Menu(this ToolStrip toolStrip, string menuTitle)
		{
			return toolStrip.add_DropDown(menuTitle);

//  check if existing on looks like this (below)
			/*return toolStrip.invokeOnThread(
				()=>{
						var dropDownButton = new ToolStripDropDownButton(menuTitle);			
						var toolStripDropDown = new ToolStripDropDown();												
						dropDownButton.DropDown = toolStripDropDown;	
						toolStrip.Items.Add(dropDownButton);
						return toolStripDropDown;
					});*/
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
    	