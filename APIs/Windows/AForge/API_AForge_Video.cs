// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX;
using O2.Views.ASCX.ExtensionMethods;
using O2.XRules.Database.Utils;

using AForge.Video.VFW;
using AForge.Controls;
using AForge.Video.DirectShow;

//O2File:API_Cropper.cs

//O2Ref:Cropper.exe
//O2Ref:AForge.dll
//O2Ref:AForge.Video.VFW.dll
//O2Ref:AForge.Video.DirectShow.dll
//O2Ref:AForge.Controls.dll
//O2Ref:AForge.Video.dll

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.APIs
{
	public class API_AForge_Video_Test
	{
		public void test()
		{
			new API_AForge_Video();										
		}
	}
	
    public class API_AForge_Video
    {    
    	public AVIWriter VideoWriter { get; set; }    	
    	public API_Cropper Cropper { get; set; }  
    	public int FrameRate { get; set; }
    	public int VideoWidth {get; set; }
    	public int VideoHeight {get; set; }
    	public string VideoCodec { get; set; }
    	public string PathToAviVideo { get; set; }
    	public int Desktop_Capture_Top { get; set; }
    	public int Desktop_Capture_Left { get; set; }
    	public int Desktop_Capture_Width { get; set; }
    	public int Desktop_Capture_Height { get; set; }
    	public int FrameCaptureDelay { get; set; }
    	public bool CapturingImages { get; set; }
    	public bool AddDuplicateFrames { get; set; }
    	
        public API_AForge_Video()
    	{    	    	    		
			VideoCodec = "WMV3 ";  //use DIB for no compression
								   // to use WMV3 Codec (I think) it is required to install this
								   // http://www.microsoft.com/downloads/details.aspx?FamilyID=0c99c648-5800-4aa3-a2fe-3de948689db8&Displ
			FrameRate = 2;
			VideoWidth = 640;
			VideoHeight = 480;
			Cropper = new API_Cropper();
			Desktop_Capture_Top = 0;
			Desktop_Capture_Left = 0;
			Desktop_Capture_Width = 800;
			Desktop_Capture_Height = 600;
			FrameCaptureDelay = 25;
			CapturingImages = false;
			AddDuplicateFrames = false;			
			checkInstallation();
			//start();
		}  							
		
		public API_AForge_Video checkInstallation()
		{
			try
			{
				"Checking to see if  WMV3 Codec is installed".info();
				var aviWriter = new AVIWriter(VideoCodec);  				
				aviWriter.Open("a.avi".tempFile(),800,600);
				"WMV3 is installed".info();
			}
			catch(Exception ex)
			{				
				"WMV3 Codec is not installed, so downloading it now and starting installation".debug();
				var wmv3InstallerUrl = "http://www.microsoft.com/downloads/info.aspx?na=41&SrcFamilyId=0C99C648-5800-4AA3-A2FE-3DE948689DB8&SrcDisplayLang=en&u=http%3a%2f%2fdownload.microsoft.com%2fdownload%2f9%2f8%2fa%2f98a6cb2d-6659-485e-b1f9-2c0d9bf6c328%2fwmv9VCMsetup.exe";
				//var wmv3Installer = O2.XRules.Database.Utils.DownloadFiles_ExtensionMethods.download(wmv3InstallerUrl,"wmv9VCMsetup.exe".tempFile());				
				var wmv3Installer = wmv3InstallerUrl.download("wmv9VCMsetup.exe".tempFile());				
				wmv3Installer.startProcess();
				"wmv3Installer: {0}".info(wmv3Installer);				
			}			
			return this;
		}
		
		public API_AForge_Video newVideo()
		{
			return newVideo(PublicDI.config.getTempFileInTempDirectory(".avi"));
		}
		
		public API_AForge_Video newVideo(string pathToVideo)
		{
			PathToAviVideo = pathToVideo;
			VideoWriter = new AVIWriter(VideoCodec);  
			VideoWriter.FrameRate = FrameRate;
			
			VideoWriter.Open(PathToAviVideo,VideoWidth, VideoHeight);
			return this;			
		}
		
		public API_AForge_Video saveAndClose()
		{
			return close();
		}
		
		public API_AForge_Video close()
		{
			VideoWriter.Close();
			return this;
		}
				
    }	
	
	public static class API_AForge_ExtensionMethods_API_AForge_Video
	{
	
		public static API_AForge_Video add_Images(this API_AForge_Video aforgeVideo, List<string> pathToImages)
		{
			foreach(var pathToImage in pathToImages)
				aforgeVideo.add_Image(pathToImage);
			return aforgeVideo;
		}
		
		public static API_AForge_Video add_Image(this API_AForge_Video aforgeVideo, string pathToImage)
		{
			aforgeVideo.add_Image(Misc_ExtensionMethods.bitmap(pathToImage));		// using the extension methods creates a weird conflic with the other bitmap<T>() where T : Control
			return aforgeVideo;
		}
		
		public static API_AForge_Video add_Images(this API_AForge_Video aforgeVideo, List<Bitmap> bitmaps)
		{
			foreach(var bitmap in bitmaps)
				aforgeVideo.add_Image(bitmap);
			return aforgeVideo;
		}
		
		public static API_AForge_Video add_Image(this API_AForge_Video aforgeVideo, Bitmap image)
		{	
			try
			{
				if (image.isNull())
					return aforgeVideo;				
				if (aforgeVideo.PathToAviVideo.isNull())
					aforgeVideo.newVideo();
				if (image.isNull())
					return aforgeVideo;
				if (image.Width.neq(aforgeVideo.VideoWidth).or(
					image.Height.neq(aforgeVideo.VideoHeight)))
					image = image.resize(aforgeVideo.VideoWidth,aforgeVideo.VideoHeight);
				aforgeVideo.VideoWriter.AddFrame(image);	
			}
			catch(Exception ex)
			{
				ex.log("[API_AForge_Video] in add_Image");
			}						
			return aforgeVideo;
		}
		
		public static API_AForge_Video add_Bitmap(this API_AForge_Video aforgeVideo, Bitmap bitmap)
		{
			aforgeVideo.add_Image(bitmap);	
			return aforgeVideo;
		}
		
		public static API_AForge_Video add_Bitmaps(this API_AForge_Video aforgeVideo, List<Bitmap> bitmaps)
		{
			foreach(var bitmap in bitmaps)
				aforgeVideo.add_Bitmap(bitmap);
			return aforgeVideo;
		}


		public static string createVideo(this API_AForge_Video aforgeVideo, List<Bitmap> bitmaps)
		{			
//			show.info(bitmaps);
			aforgeVideo.newVideo();
			aforgeVideo.add_Bitmaps(bitmaps);
			aforgeVideo.saveAndClose();
			"Created Video: {0}".info(aforgeVideo.PathToAviVideo);
			return aforgeVideo.PathToAviVideo;
		}				
		
		public static string createVideo(this API_AForge_Video aforgeVideo, List<string> pathToImages)
		{
			"Creating Video with {0} images".debug(pathToImages.size());		
			aforgeVideo.newVideo();
			aforgeVideo.add_Images(pathToImages);
			aforgeVideo.saveAndClose();
			"Created Video: {0}".info(aforgeVideo.PathToAviVideo);
			return aforgeVideo.PathToAviVideo;
		}				
		
	}
    
    
    public static class API_AForge_ExtensionMethods_API_Cropper
    {
    
    	public static API_AForge_Video frameCaptureDelay(this API_AForge_Video aforgeVideo)
    	{
    		aforgeVideo.sleep(aforgeVideo.FrameCaptureDelay);
    		return aforgeVideo;
    	}
    	
    	public static Bitmap capture(this API_AForge_Video aforgeVideo, Control staControl)
    	{
    		return (Bitmap)staControl.invokeOnThread(
    			()=> aforgeVideo.Cropper.capture(aforgeVideo.Desktop_Capture_Top,
    										     aforgeVideo.Desktop_Capture_Left, 
    										     aforgeVideo.Desktop_Capture_Width, 
    										     aforgeVideo.Desktop_Capture_Height));
    	}
    }
    
    public static class API_AForge_ExtensionMethods_VideoSourcePlayer
    {
    
    	public static VideoSourcePlayer add_MoviePlayer<T>(this T control)
    		where T : Control
    	{
    		return control.add_VideoPlayer(false);
    	}
    	
    	public static VideoSourcePlayer add_VideoPlayer<T>(this T control)
    		where T : Control
    	{
    		return control.add_VideoPlayer(false);
    	}
    	
    	public static VideoSourcePlayer add_VideoPlayer<T>(this T control, bool showAddressBar)
    		where T : Control
    	{
    		return (VideoSourcePlayer)control.invokeOnThread(
    			()=>{
    					var videoPlayer =  control.add_Control<VideoSourcePlayer>();
    					if (showAddressBar)
    					{
    						var topPanel = videoPlayer.insert_Above<Panel>(25);
    						topPanel.add_LabelAndComboBoxAndButton("Video","","Play",(text)=>videoPlayer.play(text));
    					}
    					videoPlayer.add_ContextMenu().add_MenuItem("Stop",true,()=>videoPlayer.stop())  
		   					 					     .add_MenuItem("Restart",     ()=>videoPlayer.restart());
    					return videoPlayer;
    				});
    	}
    	
    	public static VideoSourcePlayer play(this VideoSourcePlayer videoPlayer, string aviFile)
    	{
    		
    		videoPlayer.invokeOnThread(
    			()=>{
    					var videoSource =  new FileVideoSource(aviFile); 
    				    videoPlayer.SignalToStop( ); 
						videoPlayer.WaitForStop( );
						videoPlayer.VideoSource = videoSource;
						videoPlayer.Start( );	
    				});
    		return videoPlayer;
    	}    		
    	
    	public static VideoSourcePlayer stop(this VideoSourcePlayer videoPlayer)
    	{
    		videoPlayer.invokeOnThread(
    			()=>{
    					videoPlayer.Stop();
    				});
    		return videoPlayer;
		}
		
    	public static VideoSourcePlayer restart(this VideoSourcePlayer videoPlayer)
    	{
    		videoPlayer.invokeOnThread(
    			()=>{
    					videoPlayer.Start();
    				});
    		return videoPlayer;
		}
		
		public static string createAndPlay(this VideoSourcePlayer videoPlayer, List<string> pathToImages)
		{	
			"in  VideoSourcePlayer createAndPlay, creating a video from {0} WPF images".info(pathToImages.size());
			var aforgeVideo = new API_AForge_Video();		
			aforgeVideo.add_Images(pathToImages);
			aforgeVideo.saveAndClose();
			videoPlayer.play(aforgeVideo.PathToAviVideo);
			return aforgeVideo.PathToAviVideo;
		}				
    }
}
