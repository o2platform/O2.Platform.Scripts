// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.ExtensionMethods;
using O2.XRules.Database.Utils;

//O2Ref:O2_FluentSharp_WPF.dll

//O2Ref:PresentationCore.dll
//O2Ref:PresentationFramework.dll
//O2Ref:WindowsBase.dll   
//O2Ref:System.Core.dll
//O2Ref:System.Xaml.dll
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:GraphSharp.dll
//O2Ref:QuickGraph.dll 
//O2Ref:GraphSharp.Controls.dll

namespace O2.XRules.Database.APIs
{
    public class API_AForge_Video_WPF_Image : Image
	{
		//public Image FrameImage { get; set; }
		public string _BitmapFile { get; set; }
		public int _FrameCount { get; set; }
				
		public API_AForge_Video_WPF_Image()
		{
			_FrameCount = 1;
		}
		/*public API_AForge_Video_Image(Image image, int frameCount) : this (image, null, frameCount) 
		{
			
		}
		
		public API_AForge_Video_Image(Image image, Bitmap bitmap,int frameCount) 
		{
			FrameImage = image;
			OriginalBitmap = bitmap;
			FrameCount = frameCount;
		}*/
	}
	
	public static class API_AForge_Video_WPF_Image_ExtensionMethods
	{
	
		public static ListView add_Video_Image_Wpf(this ListView listView, System.Drawing.Bitmap bitmap)
		{
			return listView.add_Video_Image_Wpf(bitmap,-1,-1);
		}
		
		public static ListView add_Video_Image_Wpf(this ListView listView, System.Drawing.Bitmap bitmap, int width, int height)
		{
			return (ListView)listView.wpfInvoke(
				()=>{					
						try
						{
							var videoImage = new API_AForge_Video_WPF_Image();							
							videoImage._BitmapFile = bitmap.save();			// I tried to use the bitmap here, but there was probs accessing it later
							videoImage.open(videoImage._BitmapFile);
							if (videoImage.isNull())
								return listView;	
							if (width > -1)
								videoImage.width_Wpf(width);
							if (height > -1)
								videoImage.height_Wpf(height);	
							listView.add_Item(videoImage);
						}
						catch(Exception ex)
						{
							ex.log("in ListView add_Video_Image_Wpf");
						}
						return listView;
					});
		}
		public static List<string> getBitmapsForVideoCreation(this API_AForge_Video_WPF_Image videoImage)
		{
			var bitmaps = new List<string>();
			for(int i=0 ; i < videoImage._FrameCount ; i ++)
				bitmaps.Add(videoImage._BitmapFile);
			return bitmaps;
		}
		
		public static List<string> getBitmapsForVideoCreation(this List<API_AForge_Video_WPF_Image> videoImages)
		{
			var bitmaps = new List<string>();
			foreach(var videoImage in videoImages)
				bitmaps.AddRange(videoImage.getBitmapsForVideoCreation());
			return bitmaps;
		}
		
	}

}
