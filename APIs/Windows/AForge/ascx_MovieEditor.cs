// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WPF = System.Windows.Controls;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;
using O2.Views.ASCX.classes.MainGUI;
//O2File:API_WPF_ToolKit.cs
//O2File:WPF_ExtensionMethods.cs
//O2File:API_AForge_Video.cs
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:O2_API_Visualization.dll

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.APIs
{
    public class ascx_MovieEditor : Control
    {        	
		public int IMAGE_WIDTH = 410; 
		public int IMAGE_HEIGHT = 300;
		public bool OnImageDeleteAlsoDeleteFromDisk {get;set;}
		public bool ViewMultipleSelectedImages {get;set;}
		public API_AForge_Video AForge_Video { get; set; }
		public string TempVideoImages { get;set;}
		
		public string ListOfImagesToDelete {get;set;}		
		public WPF.ListView FramesList { get; set;}				
		public Control FrameViewer_Panel { get; set; }
		public WPF.ListView MultiImageViewer { get; set; }
		public Panel PreviewPanel { get; set; }
		
        
        public ascx_MovieEditor()
    	{    	    		
			FrameViewer_Panel = this;				
			AForge_Video = new API_AForge_Video(); 
			AForge_Video.FrameRate = 3;
			AForge_Video.FrameCaptureDelay = 10;
			AForge_Video.AddDuplicateFrames = false;
			OnImageDeleteAlsoDeleteFromDisk = false;
			ViewMultipleSelectedImages = true;
    	}
    	
    	public ascx_MovieEditor buildGui()
    	{
    		buildFrameViewer();
    		this.newMovie();
    		return this;
    	}
    	
    	public ascx_MovieEditor buildFrameViewer()
    	{       		    		    		
    		PreviewPanel =  FrameViewer_Panel.insert_Right<Panel>();
			PreviewPanel.parent<SplitContainer>().distance(300);
			PreviewPanel.backColor(Color.White);
			
    		var wpfHost = FrameViewer_Panel.add_WpfHost(); 
			FramesList  = wpfHost.add_ListView_Wpf();   
			
			FramesList.onKeyPress_Wpf(System.Windows.Input.Key.Delete, 
				()=>{
						if (OnImageDeleteAlsoDeleteFromDisk)
						{					
							/*var pictureBox = PreviewPanel.control<PictureBox>();
							if (pictureBox.notNull())
							{
								pictureBox.Image=null;
								pictureBox.clear();
								Application.DoEvents();
							}*/
							GC.Collect();		// because the Framework doesn't always releases immediately the file's handles
							foreach(var item in FramesList.selectedValues())
							{
								var fileToDelete = item.str();
								Files.deleteFile(fileToDelete);
								if (fileToDelete.fileExists())
									addFileToListOfImagesToDelete(fileToDelete);
							}										
						}
						FramesList.remove_SelectedItems();
					});
			
			FramesList.enableDrag(); 
			FramesList.enableDrop();	 
							 
			FramesList.afterSelects<String>(
				(imagePaths)=>{
							 	 if (imagePaths.size()==1)		// if there is only one image show it an PictureBox
							 	 {
							 	 	MultiImageViewer = null;
							 	 	var pictureBox = PreviewPanel.control<PictureBox>();
									if (pictureBox.isNull())
										pictureBox = PreviewPanel.clear().add_PictureBox(); 
									pictureBox.Image=null;
									pictureBox.show(imagePaths[0]);
							 	 }
							 	 else
							 	 {
							 	 	if (MultiImageViewer.isNull())
							 	 	{
							 	 		PreviewPanel.clear();
							 	 		MultiImageViewer = PreviewPanel.add_WpfHost().add_ListView_Wpf();
										MultiImageViewer.useWrapPanel();
									}
									else
									{										
										MultiImageViewer.clear();
									}
									if (ViewMultipleSelectedImages)
										foreach(var imagePath in imagePaths)
											MultiImageViewer.add_Image_Wpf(imagePath, IMAGE_WIDTH, IMAGE_HEIGHT);    									
							 	 }
							 	 
							 	 
							  });

    		return this;
    	}
    	
    	
    	public ascx_MovieEditor newMovie()
    	{	    		
    		TempVideoImages = "_TempVideoImages".tempDir();
    		"[ascx_MovieEditor] TempVideoImages set to: {0}".info(TempVideoImages);
    		
    		FramesList.clear();
    		PreviewPanel.clear();
    		return this;
    	}
    	
    	public ascx_MovieEditor setListOfImagesToDelete()
    	{
    		ListOfImagesToDelete = TempVideoImages.pathCombine("ListOfImagesToDelete.xml");
    		return this;
    	}
    	
    	public ascx_MovieEditor setMoviePath(string path)
    	{
    		if (path.valid() && path.dirExists())
    		{
    			TempVideoImages = path;
    			setListOfImagesToDelete();
    			loadImages(path);
    		}
    		return this;
    	}
    	
    	public ascx_MovieEditor loadImages(string folder)
    	{
    		if (folder.valid() && folder.dirExists())
    			loadImages(folder.files(true,"*.gif","*.png","*.jpg","*.jpeg","*.bmp"));
    		return this;
    	}
    	
    	public ascx_MovieEditor loadImages(List<string> images)
    	{    		
			FramesList.clear();
			foreach(var file in images)
			{
				"adding: {0}".info(file);
				FramesList.add_Item(file); 
			}
			FramesList.selectFirst();	
			return this;
		}
		
		public List<string> getListOfImagesToDelete()
		{
			return ListOfImagesToDelete.fileExists()
							? ListOfImagesToDelete.load<List<string>>()
							: new List<string>();		
		}
		
		public List<string> addFileToListOfImagesToDelete(string file)
		{
			var list = getListOfImagesToDelete();
			list.Add(file);
			list.saveAs(ListOfImagesToDelete);
			return list;
		}
		
		public void deleteListOfImagesToDelete()
		{
			GC.Collect();
			var stillThere = new List<string>();
			foreach(var file in getListOfImagesToDelete())
			{
				"Deleting: {0}".info(file);
				Files.deleteFile(file);
				if (file.fileExists())
					stillThere.add(file);
			}
			stillThere.saveAs(ListOfImagesToDelete);
		}
    }
    
    public static class ascx_MovieEditor_ExtensionMethods
    {
    	public static List<string> imagesPaths(this ascx_MovieEditor movieEditor)
    	{
    		return movieEditor.FramesList.items<string>();
    		//var imagesPaths = new List<string>();
    		//foreach(var item in movieEditor.FramesList.items())
    		//imagesPaths.Add("asd");
    		//return imagesPaths;
    	}    
    	public static ascx_MovieEditor add_Bitmaps(this ascx_MovieEditor movieEditor, List<Bitmap> bitmaps)
    	{
    		foreach(var bitmap in bitmaps)
    			movieEditor.add_Bitmap(bitmap);
    		return movieEditor;
    	}
    	public static string add_Bitmap(this ascx_MovieEditor movieEditor, Bitmap bitmap)
    	{    		
    		var tempFile = bitmap.save();
    		var savedBitmap = Files.MoveFile(tempFile, movieEditor.TempVideoImages);
    		movieEditor.FramesList.add_Item(savedBitmap);
    		return savedBitmap;
    	}
    }
    
    public static class ascx_MovieEditor_ExtensionMethods_AForge
    {
    	public static bool movieExists(this ascx_MovieEditor movieEditor)
    	{
    		return movieEditor.AForge_Video.PathToAviVideo.valid() && 
				   movieEditor.AForge_Video.PathToAviVideo.fileExists();
    	}
    	
    	public static ascx_MovieEditor createMovie(this ascx_MovieEditor movieEditor)
    	{
    		return movieEditor.createVideo();
    	}
    	
    	public static ascx_MovieEditor createVideo(this ascx_MovieEditor movieEditor)
    	{
    		movieEditor.AForge_Video.PathToAviVideo = "";
    		var currentImages = movieEditor.imagesPaths();
 			"Creating movie with {0} images".info(currentImages.size());
 			movieEditor.AForge_Video.createVideo(currentImages);	
 			if (movieEditor.movieExists())
 				"Movie successfuly created: {0}".debug(movieEditor.AForge_Video.PathToAviVideo);
 			return movieEditor;
 		}
		
		public static ascx_MovieEditor playLastMovieCreated(this ascx_MovieEditor movieEditor)
		{
			return movieEditor.playLastVideoCreated();
		}
		
    	public static ascx_MovieEditor playLastVideoCreated(this ascx_MovieEditor movieEditor)
    	{
    		if (movieEditor.movieExists())				
					O2Gui.open<Panel>("Video Player",640,480).add_VideoPlayer().play(movieEditor.AForge_Video.PathToAviVideo);				
			return movieEditor;
		}
		
		public static ascx_MovieEditor start_ImageCapture(this ascx_MovieEditor movieEditor, Action<int> capturedFramesCount)
		{
			"Starting Image Capture".debug();
			var aforgeVideo = movieEditor.AForge_Video;
			aforgeVideo.CapturingImages = true;
			Bitmap lastBitMap = null;
			var capturedImages = new List<Bitmap>();
			while(aforgeVideo.CapturingImages) 
			{
				var bitmap = aforgeVideo.capture(movieEditor);
				// only add if the bimap is different from the previous frame
				if (aforgeVideo.AddDuplicateFrames || bitmap.isNotEqualTo(lastBitMap))
				{					
					capturedImages.Add(bitmap);					
					capturedFramesCount(capturedImages.size());
					aforgeVideo.sleep(10,false);     // small sleep to prevent a rare race-condition where a number of images are added in big numbers
				}
				else
					aforgeVideo.frameCaptureDelay();  // only sleep if we didn't add 
				lastBitMap = bitmap;
				
			} 					
			"There where {0} images captured which will now be imported into the Movie Editor".info(capturedImages.size());			
			movieEditor.add_Bitmaps(capturedImages);					 								
			return movieEditor;
		}
		
		public static ascx_MovieEditor stop_ImageCapture(this ascx_MovieEditor movieEditor)
		{
			"Stopping Image Capture".debug();
			var aforgeVideo = movieEditor.AForge_Video;
			aforgeVideo.CapturingImages = false;
			return movieEditor;
		}
		
		public static ascx_MovieEditor showMovieCaptureLocation(this ascx_MovieEditor movieEditor)
		{
			//var handle = movieEditor.parentForm().Handle;
			//"Current Form Handle: {0}".info(handle);
			var panel = O2Gui.open<Panel>("Capture Location", movieEditor.AForge_Video.Desktop_Capture_Width, 
		 													   movieEditor.AForge_Video.Desktop_Capture_Height);
			var form = panel.parentForm();
			form.top(movieEditor.AForge_Video.Desktop_Capture_Top)
				.left(movieEditor.AForge_Video.Desktop_Capture_Left)
				.opacity(0.5);
			return movieEditor;
		
		}
		
		/* this could be used to allow clickthough a form
		
			var handle = form.Handle;
							  //.opacity(0.5) 
			int initialStyle = GetWindowLong(handle, -20);
			SetWindowLong(handle, -20, initialStyle | 0x80000 | 0x20);
			
			
			[DllImport("user32.dll", CharSet=CharSet.Auto)]
			public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
		
			[DllImport("user32.dll", EntryPoint="SetWindowLong", CharSet=CharSet.Auto)]
			public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
		*/
 

	}

	//public static class ascx_MovieEditor_ExtensionMethods_YouTube
    //{	
    //	public static ascx_MovieEditor publishMovieToYouTube(this ascx_MovieEditor movieEditor)
	
}
