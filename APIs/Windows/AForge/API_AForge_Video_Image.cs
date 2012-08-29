// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using WpfWindows = System.Windows; 
using WpfControls = System.Windows.Controls;
using WpfMedia = System.Windows.Media;
using WpfImaging = System.Windows.Media.Imaging;
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
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:System.Xaml.dll
//O2Ref:GraphSharp.dll
//O2Ref:QuickGraph.dll 
//O2Ref:GraphSharp.Controls.dll

namespace O2.XRules.Database.APIs
{
    public class API_AForge_Video_Image : WpfControls.Image
	{
		//public Image FrameImage { get; set; }
		public System.Drawing.Bitmap OriginalBitmap { get; set; }
		public int FrameCount { get; set; }
						
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
	
	public static class API_AForge_Video_Image_ExtensionMethods
	{
	
		public static WpfControls.ListView add_Video_Image_Wpf(this WpfControls.ListView listView, string pathToImage)
		{
			return listView.add_Video_Image_Wpf(pathToImage,-1,-1);
		}
		
		public static WpfControls.ListView add_Video_Image_Wpf(this WpfControls.ListView listView, string pathToImage, int width, int height)
		{
			return (WpfControls.ListView)listView.wpfInvoke(
				()=>{									
						var image = new API_AForge_Video_Image().open(pathToImage);
						if (image.isNull())
							return listView;	
						if (width > -1)
							image.width_Wpf(width);
						if (height > -1)
							image.height_Wpf(height);	
						listView.add_Item(image);
						return listView;
					});
		}
	}

}
