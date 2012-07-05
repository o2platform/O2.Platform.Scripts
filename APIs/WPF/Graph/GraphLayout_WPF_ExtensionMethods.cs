// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using GraphSharp.Controls;
using QuickGraph;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using WPFExtensions.Controls;
using O2.API.Visualization.Ascx;
using O2.External.IE.Wrapper;
using O2.API.Visualization.ExtensionMethods;

//O2File:WPF_Controls_ExtensionMethods.cs
//O2File:GraphSharp_ExtensionMethods.cs
//O2File:ElementHost_ExtensionMethods.cs

//O2Ref:GraphSharp.dll
//O2Ref:QuickGraph.dll
//O2Ref:GraphSharp.Controls.dll 
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:WPFExtensions.dll
//O2Ref:O2_API_Visualization.dll
//O2Ref:O2_External_IE.dll

namespace O2.XRules.Database.Utils
{
    public static class GraphLayout_WPF_ExtensionMethods
    {        

        #region adding and creating

        public static T add_UIElement<T>(this GraphLayout graphLayout) where T : UIElement
		{
			var uiElement = graphLayout.newInThread<T>();
            graphLayout.add_Node(uiElement);
			return uiElement;
		}
        

        public static GraphLayout loadGraph(this GraphLayout graphLayout, IBidirectionalGraph<object, IEdge<object>> graphToLoad)
        {
            return graphLayout.set_Graph(graphToLoad);
        }

        public static GraphLayout set_Graph(this GraphLayout graphLayout, IBidirectionalGraph<object, IEdge<object>> graphToLoad)
        {
            return (GraphLayout)graphLayout.wpfInvoke(
                () =>
                {
                    try
                    {
                        graphLayout.Graph = graphToLoad;
                    }
                    catch (Exception ex)
                    {
                        ex.log("in set_Graph");
                    }
                    return graphLayout;
                });
        }
        

        /*public static GraphLayout add_Node(this GraphLayout graphLayout, object vertexToAdd)
        {
            return graphLayout.add(vertexToAdd);
        }*/        

        #endregion

		#region O2BrowserIE
		
		public static O2BrowserIE add_WebBrowser(this GraphLayout graphLayout)
		{
			return (O2BrowserIE)graphLayout.add_WinForm<O2BrowserIE>(800,400);
		}
		#endregion

        #region TextBox

        public static TextBox add_TextBox(this GraphLayout graphLayout)
    	{
    		return graphLayout.add_TextBox("");
    	}
		public static TextBox add_TextBox(this GraphLayout graphLayout, string textValue)
		{
			var textBox = graphLayout.newInThread<TextBox>();
			textBox.set_Text(textValue);
			graphLayout.add_Node(textBox);
			return textBox;
		}				
		
		#endregion
		
		#region Button
		
		public static Button add_Button(this GraphLayout graphLayout, string text, int width, int height)
		{
            var button = graphLayout.add_UIElement<Button>();
			button.set(text);
			button.width_Wpf(width);
			button.height_Wpf(height);
			return button;
		}
		
		#endregion
		
		#region Image
		
		public static Image add_Image(this GraphLayout graphLayout, string uri)
		{
			return graphLayout.add_Image(new Uri(uri) , -1 , -1);
		}
		
		public static Image add_Image(this GraphLayout graphLayout, string uri, int width, int height)
		{
			return graphLayout.add_Image(new Uri(uri),width, height);
		}
		
		public static Image add_Image(this GraphLayout graphLayout, Uri uri, int width, int height)
		{
			return (Image)graphLayout.wpfInvoke(
				()=>{
                        var image = graphLayout.add_UIElement<Image>();
						var bitmap = new BitmapImage(uri);
						image.Source = bitmap; 
						if (width > -1)
							image.width_Wpf(width);
						if (height > -1)
                            image.height_Wpf(height);
						return image;
					});
		}
		
		#endregion
	
		#region ElementHost - graph and zoom        	 
    	
    	public static GraphLayout add_GraphWithZoom(this ElementHost elementHost)
		{
			var zoom = elementHost.add_Zoom();
			var graphLayout = zoom.set<GraphLayout>();
			graphLayout.newGraph();
			graphLayout.defaultLayout();
			return graphLayout;
		}
		
		public static GraphLayout add_Graph(this ElementHost elementHost)
    	{
            var graphLayout = elementHost.add_Control_Wpf<GraphLayout>();    		
    		graphLayout.background(Brushes.White);
    		graphLayout.newGraph();  			
			return graphLayout;
    	}
		    	
    	public static ZoomControl add_Zoom(this ElementHost elementHost)
    	{
            return elementHost.add_Control_Wpf<ZoomControl>();    		    		    					
    	}    	    	    	
    	    	
    	#endregion
    	
    	#region WinForms to WPF host - generic

		public static T add_WinForm<T>(this GraphLayout graphLayout, int width, int height) where T : System.Windows.Forms.Control
		{
			return (T)graphLayout.wpfInvoke(
    			()=>{
    					var controlHost = graphLayout.add_UIElement<WindowsFormsHost>();
                        controlHost.width_Wpf(width);
                        controlHost.height_Wpf(height);
						var winFormsControl = (System.Windows.Forms.Control)typeof(T).ctor();	
						winFormsControl.Dock = System.Windows.Forms.DockStyle.Fill;
						controlHost.Child = winFormsControl;
						return winFormsControl;
					});
		}
    	
    	#endregion
    	
    	#region ascx_Xaml_Host
    	
    	public static GraphLayout add_Graph(this System.Windows.Forms.Control winFormsControl)
		{
		 	var xamlHost = winFormsControl.add_Control<ascx_Xaml_Host>();
		 	if (xamlHost == null)
		 		"xamlHost was null".error();
			return xamlHost.add_Graph();
		}
		
		public static GraphLayout add_Graph(this ascx_Xaml_Host xamlHost)
		{
            return (GraphLayout)xamlHost.invokeOnThread(
                () =>
                {

                    var zoom = xamlHost.element().add_Zoom();

                    var graphLayout = zoom.set<GraphLayout>();
                    graphLayout.newGraph();
                    graphLayout.defaultLayout();
                    return graphLayout;
                });
		}
		
		#endregion
    }
}
