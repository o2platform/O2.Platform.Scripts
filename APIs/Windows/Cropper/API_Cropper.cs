// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL.Utils;
using FluentSharp.WinForms;
using Fusion8.Cropper.Core;
using Splicer.Renderer;
using Splicer.Timeline;
using Splicer.WindowsMedia;
using Program = Fusion8.Cropper.Program;

//O2Ref:Cropper.exe

namespace O2.XRules.Database.APIs
{
    public class API_Cropper : Control			// make this a windows form so that we get the STA thread from it 
    {        	
        public ImageCapture CropperImageCapture { get; set;}
        public Image LastImage { get; set; }
		public int MaxImageCaptureWait  { get; set; }
		public AutoResetEvent captureComplete;		
		public Action<Image> onCapture;
		
		
		public API_Cropper()
		{			
			setup();			
		}  
		
		public API_Cropper setup()
		{			
			captureComplete = new AutoResetEvent(false);
			MaxImageCaptureWait = 2000; // 2 seconds
			var configuration = Configuration.Current;								
			CropperImageCapture = new ImageCapture();	
			
			CropperImageCapture.ImageCaptured+= 
				(sender,e)=>{
								LastImage = new Bitmap(e.FullSizeImage);								
								if (onCapture.notNull())
									onCapture(LastImage);
								captureComplete.Set();
							};
			return this;
		}	
		
		public static ITimeline timeline()
		{
			var timeline = new DefaultTimeline(); 					
			var audioTrack = timeline.add_AudioTrack();
			return timeline;
		}
		
		public API_Cropper waitForCapture()
		{
			if (captureComplete.WaitOne(MaxImageCaptureWait).isFalse())
				"in API_Cropper waitForCapture(), MaxImageCaptureWait reached".error();			
			return this;
		}
		
		//static for the cases where we have no live Control to get the STA thread from
		public static Bitmap captureSta(int x, int y, int width, int height)
		{
    		Bitmap bitmap = null;
			var sync = new AutoResetEvent(false);
			O2Thread.staThread(
				()=>{
						bitmap = new API_Cropper().capture(x,y,width,height);
						sync.Set();
					});
			sync.WaitOne();
			return bitmap;	
		}
		
		public static void openGui()
		{
			O2Thread.staThread(Program.Main);
		}
    }
    
    public static class API_Cropper_ExtensionMethods
    {
    	public static API_Cropper cropper<T>(this T control)
    		where T : Control
    	{
    		return control.newInThread<API_Cropper>();     		
    	}
    	
    	public static Settings config(this API_Cropper cropper)
    	{
    		return cropper.settings();
    	}
    	
    	public static Settings settings(this API_Cropper cropper)
    	{
    		return Configuration.Current;
    	}
    	public static API_Cropper showConfig(this API_Cropper cropper)
    	{
    		cropper.invokeOnThread(()=> show.info(Configuration.Current));
    		return cropper;    		
    	}
    	
    	public static API_Cropper toClipboard(this API_Cropper cropper)
    	{    		
    		cropper.CropperImageCapture.ImageFormat = ImageCapture.ImageOutputs["Clipboard"];    		
    		return cropper;
    	}
    	
    	public static API_Cropper includeMouse(this API_Cropper cropper)
    	{
    		cropper.config().IncludeMouseCursorInCapture = true;
    		return cropper;
    	}
    	
    	
    	public static Bitmap capture(this API_Cropper cropper, int x, int y, int width, int height)
    	{
    		return (Bitmap)cropper.invokeOnThread(
    			()=>{    					    					
    					//"__current ApartmentState: {0}".info(System.Threading.Thread.CurrentThread.GetApartmentState());
    					try	
    					{    							    					
	    					cropper.toClipboard();
	    					cropper.captureComplete.Reset();
	    					cropper.CropperImageCapture.Capture(x,  y,  width,  height);
	    					cropper.waitForCapture();	    					
	    					return cropper.LastImage;
							//return cropper.fromClipboardGetImage();
						}
						catch(Exception ex)
						{
							ex.log("in API_Cropper capture");
							return null;
						}					
					});
    	}
    	
    	public static Bitmap capture_Desktop(this API_Cropper cropper)
    	{
    		return (Bitmap)cropper.invokeOnThread(
    			()=>{
			    		cropper.toClipboard();
			    		cropper.CropperImageCapture.CaptureDesktop();
			    		return cropper.fromClipboardGetImage();
			    	});
    	}
    	
    	public static Bitmap capture<T>(this API_Cropper cropper, T control)
    		where T : Control
    	{
    		return (Bitmap)control.invokeOnThread(
    			()=>{
    					var location = control.PointToScreen(Point.Empty); 
    					return cropper.capture(location.X, location.Y, control.Width, control.Height);
    				});
    	}
    	
    	
    	public static Bitmap screenshot<T>(this T control)
    		where T : Control
    	{
    		return control.capture<T>();
    	}
    	
    	public static Bitmap bitmap<T>(this T control)
    		where T : Control
    	{
    		return control.capture<T>();
    	}
    	
    	public static Bitmap capture<T>(this T control)
    		where T : Control
    	{    		
    		return (Bitmap)control.invokeOnThread(
    			()=>{    					
    					var cropper = new API_Cropper();        					
    					return 	cropper.capture(control);
    				});
    	}
    	
    	public static Bitmap desktop<T>(this T control)
    		where T : Control
    	{    		
    		return (Bitmap)control.invokeOnThread(
    			()=>{
    					var cropper = new API_Cropper();        					
    					return 	cropper.capture_Desktop();
    				});
    	}
    	
    	public static Bitmap capture<T>(this T control, int x, int y, int width, int height)
    		where T : Control
    	{
    		return (Bitmap)control.invokeOnThread(
    			()=>{
    					var cropper = new API_Cropper();        					
    					return 	cropper.capture(x,  y,  width,  height);
    				});
    	}    	    	
	}
       
   	public static class API_Cropper_ExtensionMethods_Create_WMV_Videos
   	{
		
		public static ITrack add_VideoTrack(this ITimeline timeline)
		{			
			//var group = timeline.AddVideoGroup(32 /*bitCount*/, 160 /*width*/ ,100 /*height*/);
			var group = timeline.AddVideoGroup(32 /*bitCount*/, 640 /*width*/ ,480 /*height*/);
			var videoTrack = group.AddTrack();			
			return videoTrack;
		}
		
		public static ITrack add_AudioTrack(this ITimeline timeline)
		{
			return timeline.AddAudioGroup().AddTrack();			
		}
		
   		public static IClip add_Image(this ITrack videoTrack , string imageToAdd)
   		{
   			try
   			{
   				"adding image: {0}".debug(imageToAdd);
   				return videoTrack.AddImage(imageToAdd,0,1);//,0 /*offset*/ ,2/*clipEnd*/);
   			}
   			catch(Exception ex)
   			{
   				ex.log("in API_Cropper videoTrack add_Image");
   				return null;
   			}   			
   		}
   		
   		public static IClip add_Image(this ITrack videoTrack , Image imageToAdd)
   		{
   			try
   			{
   				return videoTrack.AddImage(imageToAdd);//,0,2);//,0  ,2);
   			}
   			catch(Exception ex)
   			{
   				ex.log("in API_Cropper videoTrack add_Image");
   				return null;
   			}   			
   		}
   		
   		//	var imagesFolder = @"C:\Documents and Settings\Administrator\My Documents\Downloads\splicer-49146\src\Splicer\bin\Debug";
		//		var images = imagesFolder.files("*.jpg"); 
				
				
				//foreach(var bitmat in 
		
				//var clip2 = videoTrack.AddImage(images[1],0,2);
				//var clip3 = videoTrack.AddImage(images[2],0,2);
				//var clip4 = videoTrack.AddImage(images[3],0,2);
				
				//show.info(timeline);
			
   
   		public static string renderVideo(this ITimeline timeline)
   		{
   			var tempOutputFile = PublicDI.config.getTempFileInTempDirectory(".wmv");	
   			return 	timeline.renderVideo(tempOutputFile);   			
   		}
   		
   		public static string renderVideo(this ITimeline timeline , string outputFile)
   		{   
   			try
   			{
	   			if (outputFile.fileExists())
		   			Files.deleteFile(outputFile);
		   			
		    	//var renderer = new WindowsMediaRenderer(timeline, outputFile, WindowsMediaProfiles.HighQualityVideo);
		    	var renderer = new WindowsMediaRenderer(timeline, outputFile, WindowsMediaProfiles.BiggerHighQualityVideo);
		    	
	    		"[API_Cropper] starting video rendering process".info();
	    		renderer.Render();
				"[API_Cropper] video rendering complete".debug();
				if (outputFile.fileExists())
				{
					"[API_Cropper] video saved to {0}".info(outputFile);
					return outputFile;
				}
				
				"[API_Cropper] video file was not created".error();
				return "";
			}
   			catch(Exception ex)
   			{
   				ex.log("in API_Cropper videoTrack renderVideo");
   				return "";   				
   			}
   			   			   			
		}
		
		public static string video(this List<Bitmap> bitmaps)
		{
			var timeline = API_Cropper.timeline();
			var videoTrack = timeline.add_VideoTrack();
			foreach(var bitmap in bitmaps)
			{
				var file = bitmap.jpg();
				//"file_: {0}".info(file);
				videoTrack.add_Image(file);
				//videoTrack.add_Image(bitmap);
			}	
			return timeline.renderVideo();
		}
		
	}	
				
				
				//var audio = audioTrack.AddAudio(imagesFolder.pathCombine("testinput.wav"),videoTrack.Duration);
				
				
				
				
			//	if (outputFile.fileExists())					
			//		videoPlayer.play(outputFile);
			//	"file exists:{0}".info(outputFile.fileExists());
}
