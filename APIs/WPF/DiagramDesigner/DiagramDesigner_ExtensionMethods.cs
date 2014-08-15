// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Controls;
using System.Windows;
using DiagramDesigner;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.REPL.Controls;
using FluentSharp.WPF;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using FluentSharp.WinForms.Utils;
using WinForms = System.Windows.Forms;
using System.Collections.Generic;
using O2.XRules.Database.Utils;
using FluentSharp.Xml;
//O2File:ElementHost_ExtensionMethods.cs
//O2File:Xaml_ExtensionMethods.cs

//O2Ref:WindowsFormsIntegration.dll
//O2Ref:ICSharpCode.AvalonEdit.dll
//O2Ref:FluentSharp.Xml.dll
//O2Ref:GraphSharp.Controls.dll
//O2Ref:DiagramDesigner.exe


namespace O2.XRules.Database.APIs
{		
    public static class DiagramDesigner_ExtensionMethods_Create
    {   
    
    	//launch stand alone version of DiagramDesigner
    	public static void openDiagramDesignedInNewWindow()
		{			
			var mainPanel = O2Gui.open<WinForms.Panel>("DiagramDesigner inside O2",500,400);									
			var designerCanvas = mainPanel.add_DesignerCanvas();														
			designerCanvas.add_Label("This is new DesignerCanvas", 120,100).fontSize(20);			 					
		}
    	// create
    	public static DesignerCanvas add_DesignerCanvas<T>(this T  control)
    		where T : System.Windows.Forms.Control
    	{
    		return control.add_DesignerCanvas("My Designer Canvas");
    	}
    	
    	public static DesignerCanvas add_DesignerCanvas<T>(this T  control, string name)
    		where T : System.Windows.Forms.Control
    	{
    		control.clear();
    		var wpfHost = control.add_WpfHost();
			return (DesignerCanvas)wpfHost.invokeOnThread( 
				()=>{ 
						try
						{
							var application = System.Windows.Application.Current.isNull() 
							  	? new DiagramDesigner.App()
							  	: System.Windows.Application.Current;				
							Uri resourceLocater = new Uri("/DiagramDesigner;component/app.xaml", UriKind.Relative);
			  				System.Windows.Application.LoadComponent(application, resourceLocater); 
			
			
							var designerCanvas = wpfHost.add_Control_Wpf<DiagramDesigner.DesignerCanvas>();
							designerCanvas.Background = System.Windows.Media.Brushes.White;  
							designerCanvas.Name = "MyDesigner";
							return 	designerCanvas;		
						}
						catch(Exception ex)
						{
							ex.log("in WinForms Control add_DesignerCanvas");
							return null;
						}
					});			    						    	
		}				
    }
    
    public static class DiagramDesigner_ExtensionMethods_IO
    {
    	public static string save(this DesignerCanvas designerCanvas)
    	{
    		var tempFile = PublicDI.config.getTempFileInTempDirectory(".xml");
    		return designerCanvas.save(tempFile);
    	}
    	
    	public static string save(this DesignerCanvas designerCanvas, string targetFile)
    	{
    		return (string)designerCanvas.wpfInvoke(
    			()=>{    				
						var xElementForSave = designerCanvas.getXElementForSave();
						xElementForSave.Save(targetFile);
						if (targetFile.fileExists())
							return targetFile;
						return "";
					});

    	}
    	
    	public static string load(this DesignerCanvas designerCanvas)
    	{
    		return designerCanvas.open();
    	}
    	
    	public static string load(this DesignerCanvas designerCanvas, string fileToOpen)
    	{
    		return designerCanvas.open(fileToOpen);
    	}
    	
    	public static string open(this DesignerCanvas designerCanvas)
    	{ 
    		return designerCanvas.open(O2Forms.askUserForFileToOpen(Environment.CurrentDirectory));
    	}
    	
    	public static string open(this DesignerCanvas designerCanvas, string fileToOpen)
    	{
    		return (string)designerCanvas.wpfInvoke(
    			()=>{    		
    					if (fileToOpen.fileExists())
    					{
			    			var xRoot = fileToOpen.xRoot();
			    			if (xRoot.notNull())
			    				designerCanvas.Open(xRoot);
			    			else 
				    			return "";
			    		}
			    		return fileToOpen;
			    	});
    	}
    	
    	
    }
    
    public static class DiagramDesigner_ExtensionMethods_Misc
    {
    
    	public static List<DesignerItem> designerItems(this DesignerCanvas designerCanvas)
    	{
    		return designerCanvas.controls_Wpf<DesignerItem>();
    	}
    	
    	public static DesignerItem designerItem(this FrameworkElement frameworkElement)
    	{
    		return (DesignerItem)frameworkElement.wpfInvoke(
				()=>{
						if (frameworkElement.notNull() && frameworkElement.Parent.notNull() && frameworkElement.Parent is DesignerItem)
							return (DesignerItem)frameworkElement.Parent;
						return null;
					});
    	}    	    	
    	    	
    	// to get this one we have to go trough a couple rabit holes
    	public static DesignerItem designerItem(this WinForms.Control control)
    	{
    		return (DesignerItem)control.invokeOnThread(
    			()=>{    			
			    		if (control.notNull() && control.Parent.notNull())
			    		{			    			
			    			var host = control.Parent.field("_host");			    			
			    			if (host is WinForms.Integration.WindowsFormsHost)
			    			{
			    				var formsHost = (host as WinForms.Integration.WindowsFormsHost);
			    				if (formsHost.Parent is FrameworkElement)
			    					return (formsHost.Parent as FrameworkElement).designerItem();
			    			}
			    		}
			    		return null;
			    		});
		}    
		
		public static DesignerCanvas designerCanvas(this FrameworkElement frameworkElement)
    	{
    		return (DesignerCanvas)frameworkElement.wpfInvoke(
				()=>{
						var designerItem = frameworkElement.designerItem();
						return designerItem.designerCanvas();
					});
    	}
    	
    	public static DesignerCanvas designerCanvas(this DesignerItem designerItem)
    	{
    		return (DesignerCanvas)designerItem.wpfInvoke(
				()=>{
						if (designerItem.notNull() && designerItem.Parent.notNull() && designerItem.Parent is DesignerCanvas)
							return (DesignerCanvas)designerItem.Parent;
						return null;
					});					
    	}
    	
    	public static T designMode<T>(this T frameworkElement, bool value)
    		where T : FrameworkElement
    	{
    		frameworkElement.designerItem().designMode(value);
    		return frameworkElement;
    	}
    	public static DesignerItem designMode(this DesignerItem designerItem, bool value)
    	{
    		return (DesignerItem)designerItem.wpfInvoke(
    			()=>{
    					foreach(var subItem in designerItem.controls_Wpf()) 
    						if ((subItem is DesignerItem).isFalse())
    							subItem.IsHitTestVisible = value.not();
    					//designerItem.IsHitTestVisible = value;
						return designerItem;
    				});
    	}
    	
    }
    
    public static class DiagramDesigner_ExtensionMethods_Add_Controls
    {
    	public static T add_Item<T>(this DesignerCanvas designerCanvas)
    		where T : UIElement
    	{
    		return designerCanvas.add_Item<T>("", -1,-1,-1,1);
    	}
    	
    	public static T add_Item<T>(this DesignerCanvas designerCanvas, string newControlContent, int top, int left)
    		where T : UIElement
    	{
			return designerCanvas.add_DesignerItem<T>(newControlContent, top, left, -1, -1);
    	}
    	
    	public static T add_Item<T>(this DesignerCanvas designerCanvas, int top, int left,  int width, int height)
    		where T : UIElement
    	{
			return designerCanvas.add_DesignerItem<T>(null, top, left, width, height);
    	}
    	
    	public static T add_Item<T>(this DesignerCanvas designerCanvas, string newControlContent, int top, int left,  int width, int height)
    		where T : UIElement
    	{
			return designerCanvas.add_DesignerItem<T>(newControlContent, top, left, width, height);
    	}
    	
    	public static T add_DesignerItem<T>(this DesignerCanvas designerCanvas, string newControlContent, int top, int left,  int width, int height)
			where T : UIElement
    	{    		
    		return (T)designerCanvas.wpfInvoke<DesignerCanvas,T>(
    			()=> {
    					try
    					{
	    					var newControl = (T)typeof(T).ctor();
	    					return designerCanvas.add_UIElement(newControl,newControlContent, top,  left,   width,  height);
	    								
	    				}
	    				catch(Exception ex)
	    				{
	    					ex.log("in DesignerCanvas add_DesignerItem");
	    				}
    					return default(T);
    				 });
    		
    	}
    	
    	public static T add_UIElement<T>(this DesignerCanvas designerCanvas, T uiElement,string newControlContent,int top, int left,  int width, int height)
    		where T : UIElement
    	{
    		 
    		try
    		{
    			if (uiElement.notNull())
				{
					var newDesignerItem = new DiagramDesigner.DesignerItem();
					uiElement.IsHitTestVisible = false;   							
						    						
					newDesignerItem.width_Wpf(width);
					newDesignerItem.height_Wpf(height);
					newDesignerItem.top_Wpf(top);
					newDesignerItem.left_Wpf(left);
					
					//newControl.width_Wpf(height-5);
					//newControl.width_Wpf(height-5);
					/*if (newControl is FrameworkElement)
					{
						(newControl as FrameworkElement).width_Wpf(width -5);
						(newControl as FrameworkElement).height_Wpf(height -5 );
						//(newControl as FrameworkElement).top_Wpf(top - 5);
						//(newControl as FrameworkElement).left_Wpf(left + 5);
					}*/
					
					if (newControlContent.valid() && uiElement is ContentControl)
						(uiElement as ContentControl).Content = newControlContent;							
					
					newDesignerItem.Content = uiElement; 
					
					designerCanvas.Children.Add(newDesignerItem);
					return (T)uiElement;
				}    		
    		}
    		catch(Exception ex)
			{
				ex.log("in DesignerCanvas add_DesignerItem");
			}
			return default(T);
    	}
		
		
		public static UIElement add_XamlElement(this DesignerCanvas designerCanvas, string xamlCode)
  		{
  			return designerCanvas.add_XamlElement(xamlCode, -1,-1,-1,-1);
  		}
  		public static UIElement add_XamlElement(this DesignerCanvas designerCanvas, string xamlCode, int top, int left)
  		{
  			return designerCanvas.add_XamlElement(xamlCode, top,left,-1,-1);
  		}
  		
  		public static UIElement add_XamlElement(this DesignerCanvas designerCanvas, string xamlCode, int top, int left,  int width, int height)  		
  		{
  			return (UIElement)designerCanvas.wpfInvoke(
  				()=>{
  						var uiElement = xamlCode.xaml_CreateUIElement();
  						return designerCanvas.add_UIElement(uiElement,"", top,  left,   width,  height);
  					});  			
  		}
    	
    	    	    	
    }
    
  	public static class DiagramDesigner_ExtensionMethods_WPF_Controls
  	{  		    	
    	// Label
    	
    	public static Label add_Label(this DesignerCanvas designerCanvas, string text)
  		{
  			return designerCanvas.add_Label(text, -1,-1,-1,-1);
  		}
  		public static Label add_Label(this DesignerCanvas designerCanvas, string text, int top, int left)
  		{
  			return designerCanvas.add_Label(text, top,left,-1,-1);
  		}
  		
  		public static Label add_Label(this DesignerCanvas designerCanvas, string text, int top, int left,  int width, int height)  		
  		{
  			return designerCanvas.add_Item<Label>(text,top, left, width, height);
  		}
    	
    	// TextBox
    	
    	public static TextBox add_TextBox(this DesignerCanvas designerCanvas, string text)
  		{
  			return designerCanvas.add_TextBox(text, -1,-1,-1,-1);
  		}
  		public static TextBox add_TextBox(this DesignerCanvas designerCanvas, string text, int top, int left)
  		{
  			return designerCanvas.add_TextBox(text, top,left,-1,-1);
  		}
  		
  		public static TextBox add_TextBox(this DesignerCanvas designerCanvas, string text, int top, int left,  int width, int height)  		
  		{
  			return designerCanvas.add_Item<TextBox>(text,top, left, width, height);
  		}
    	    	
    	// Button
    	
    	public static Button add_Button(this DesignerCanvas designerCanvas, string text)
  		{
  			return designerCanvas.add_Button(text, -1,-1,-1,-1);
  		}
  		
  		public static Button add_Button(this DesignerCanvas designerCanvas, string text, int top, int left)
  		{
  			return designerCanvas.add_Button(text, top,left,-1,-1);
  		}
  		
  		public static Button add_Button(this DesignerCanvas designerCanvas, string text, int top, int left,  int width, int height)  		
  		{
  			return designerCanvas.add_Item<Button>(text,top, left, width, height);
  		}
    	
    	// ComboBoc
    	
    	public static ComboBox add_ComboBox(this DesignerCanvas designerCanvas)
  		{
  			return designerCanvas.add_ComboBox(-1,-1,-1,-1);
  		}
  		
  		public static ComboBox add_ComboBox(this DesignerCanvas designerCanvas,  int top, int left)
  		{
  			return designerCanvas.add_ComboBox(top,left,-1,-1);
  		}
  		
    	public static ComboBox add_ComboBox(this DesignerCanvas designerCanvas,  int top, int left,  int width, int height)  		
  		{
  			return designerCanvas.add_Item<ComboBox>(top, left, width, height);
  		}
    	// Image
    	 
  		public static Image add_Image(this DesignerCanvas designerCanvas, string imageLocation)
  		{
  			return designerCanvas.add_Image(imageLocation, -1,-1,-1,-1);
  		}
  		
		public static Image add_Image(this DesignerCanvas designerCanvas, string imageLocation, int top, int left,  int width, int height)  		
		{
			var image = designerCanvas.add_Item<Image>(top, left, width, height);
			image.open(imageLocation);
			return image;
		}
		
		// WpfTextEditor
  		/*public static WpfTextEditor add_SourceCodeViewer(this DesignerCanvas designerCanvas)
  		{
  			return designerCanvas.add_SourceCodeViewer(0,0,200,200);
  		}
  		
  		public static WpfTextEditor add_SourceCodeViewer(this DesignerCanvas designerCanvas, int top, int left, int  width, int  height)
    	{
    		//var canvas = designerCanvas.add_Item<Grid>("",top,  left,   width,  height);
    		//canvas.backColor(System.Windows.Media.Brushes.Red); 
    		var sourceCode = designerCanvas.add_Item<WpfTextEditor>(top,  left,   width,  height);//0,0,250,250)(); 
    		sourceCode.width_Wpf(width);
    		return sourceCode;
    		//return canvas.add_WinFormToWPF<T>();
    		//return designerCanvas.add_Item<WpfTextEditor>(0,0,250,250);
    	}*/
  	}
  	
  	
  	public static class DiagramDesigner_ExtensionMethods_WinFormsControls
    {
    	public static WinForms.Panel add_WinForms_Panel(this DesignerCanvas designerCanvas, int top, int left,  int width, int height)
    	{    		
    		var canvas = designerCanvas.add_Item<Grid>("", top,  left,   width,  height);
    		return canvas.add_WinForms_Panel();
    	}
    	    	
    	public static T add_WinForms_Control<T>(this DesignerCanvas designerCanvas)
    		where T : WinForms.Control
    	{
    		return designerCanvas.add_WinForms_Control<T>(0,0,100,100);
    	}
    	
    	public static T add_WinForms_Control<T>(this DesignerCanvas designerCanvas, int top, int left)
    		where T : WinForms.Control
    	{
    		return designerCanvas.add_WinForms_Control<T>(top,left,100,100);
    	}
    	
    	public static T add_WinForms_Control<T>(this DesignerCanvas designerCanvas, int top, int left,  int width, int height)
    		where T : WinForms.Control
    	{    		
    		var canvas = designerCanvas.add_Item<Grid>("", top,  left,   width,  height);
    		return canvas.add_WinFormToWPF<T>();
    	}
      	    	
    	public static ascx_LogViewer add_LogViewer(this DesignerCanvas designerCanvas)
    	{
    		return designerCanvas.add_WinForms_Control<ascx_LogViewer>();
    	}
    	
    	public static ascx_SourceCodeViewer add_SourceCodeViewer(this DesignerCanvas designerCanvas)
  		{
  			return designerCanvas.add_SourceCodeViewer(10,10,100,100);
  		}
  		
  		public static ascx_SourceCodeViewer add_SourceCodeViewer(this DesignerCanvas designerCanvas, int top, int left, int  width, int  height)
    	{
    		return designerCanvas.add_WinForms_Control<ascx_SourceCodeViewer>(top, left,  width,  height);
    	}
    }
    
    
    public static class DiagramDesigner_ExtensionMethods_Connectors
    {
    	public static Connector connector_Right(this FrameworkElement frameworkElement)
    	{    	
			return frameworkElement.connector("Right");
    	}
    	
    	public static Connector connector_Left(this FrameworkElement frameworkElement)
    	{
    		return frameworkElement.connector("Left");
    	}
    	
    	public static Connector connector_Top(this FrameworkElement frameworkElement)
    	{
    		return frameworkElement.connector("Top");
    	}
		
		public static Connector connector_Bottom(this FrameworkElement frameworkElement)
    	{
    		return frameworkElement.connector("Bottom");
    	}
    	
    	public static Connector connector(this FrameworkElement frameworkElement, string topLeftBottomOrRight)
    	{
    		var designerItem = frameworkElement.designerItem();
    		return designerItem.connector(topLeftBottomOrRight);
    	}
    	
		public static Connector connector_Right(this DesignerItem designerItem)
    	{    	
			return designerItem.connector("Right");
    	}
    	
    	public static Connector connector_Left(this DesignerItem designerItem)
    	{
    		return designerItem.connector("Left");
    	}
    	
    	public static Connector connector_Top(this DesignerItem designerItem)
    	{
    		return designerItem.connector("Top");
    	}
		
		public static Connector connector_Bottom(this DesignerItem designerItem)
    	{
    		return designerItem.connector("Bottom");
    	}
    	
    	public static Connector connector(this DesignerItem designerItem, string topLeftBottomOrRight)
    	{
    		return (Connector)designerItem.wpfInvoke(
    			()=>{
						var designerCanvas = designerItem.designerCanvas();
						if (designerItem.notNull() && designerCanvas.notNull())			
							return (DiagramDesigner.Connector)designerCanvas.invoke("GetConnector", designerItem.ID, topLeftBottomOrRight);
						return null;
					});
    	}
    	
    	
    	
    	public static Connection connect(this DesignerCanvas designerCanvas, FrameworkElement frameworkElement1, FrameworkElement frameworkElement2)
    	{
    		return designerCanvas.add_Connection(frameworkElement1.designerItem(), frameworkElement2.designerItem(), "Right", "Left");    		
    	}
    	
    	public static Connection connect(this DesignerCanvas designerCanvas, FrameworkElement frameworkElement1, FrameworkElement frameworkElement2, string position1, string position2)
    	{
    		return designerCanvas.add_Connection(frameworkElement1.designerItem(), frameworkElement2.designerItem(), position1, position2);    		
    	}

    	public static Connection add_Connection(this DesignerCanvas designerCanvas, FrameworkElement frameworkElement1, FrameworkElement frameworkElement2)
    	{
    		return designerCanvas.add_Connection(frameworkElement1.designerItem(), frameworkElement2.designerItem(), "Right", "Left");    		
    	}
    	
    	public static Connection add_Connection(this DesignerCanvas designerCanvas, WinForms.Control winFormscontrol, FrameworkElement frameworkElement)
    	{
    		return designerCanvas.add_Connection(winFormscontrol.designerItem(), frameworkElement.designerItem());
    	}
    	
    	public static Connection add_Connection(this DesignerCanvas designerCanvas, FrameworkElement frameworkElement, WinForms.Control winFormscontrol)
    	{
    		return designerCanvas.add_Connection(frameworkElement.designerItem(), winFormscontrol.designerItem());
    	}
    	
    	public static Connection add_Connection(this DesignerCanvas designerCanvas, WinForms.Control winFormscontrol1, WinForms.Control winFormscontrol2)
    	{
    		return designerCanvas.add_Connection(winFormscontrol1.designerItem(), winFormscontrol2.designerItem());
    	}
    	    	
    	
    	public static Connection add_Connection(this DesignerCanvas designerCanvas, DesignerItem designerItem1, DesignerItem designerItem2)
    	{
    		return designerCanvas.add_Connection(designerItem1, designerItem2, "Right", "Left");    		
    	}    	    	
    	
    	public static Connection add_Connection_TopBottom(this DesignerCanvas designerCanvas, FrameworkElement frameworkElement1, FrameworkElement frameworkElement2)
    	{
    		return designerCanvas.add_Connection(frameworkElement1.designerItem(), 
    											 frameworkElement2.designerItem(), 
    											 "Top", "Bottom");
    	}
    	
    	public static Connection add_Connection_BottomTop(this DesignerCanvas designerCanvas, FrameworkElement frameworkElement1, FrameworkElement frameworkElement2)
    	{
    		return designerCanvas.add_Connection(frameworkElement1.designerItem(), 
    											 frameworkElement2.designerItem(), 
    											 "Bottom", "Top");
    	}
    	
    	public static Connection add_Connection_LeftRight(this DesignerCanvas designerCanvas, FrameworkElement frameworkElement1, FrameworkElement frameworkElement2)
    	{
    		return designerCanvas.add_Connection(frameworkElement1.designerItem(), 
    											 frameworkElement2.designerItem(), 
    											 "Left", "Right");
    	}
    	
    	public static Connection add_Connection_RightLeft(this DesignerCanvas designerCanvas, FrameworkElement frameworkElement1, FrameworkElement frameworkElement2)
    	{
    		return designerCanvas.add_Connection(frameworkElement1.designerItem(), 
    											 frameworkElement2.designerItem(), 
    											 "Right", "Left");
    	}
    	
    	public static Connection add_Connection(this DesignerCanvas designerCanvas, DesignerItem designerItem1, DesignerItem designerItem2, string position1, string position2)
    	{
    		return (Connection)designerCanvas.wpfInvoke(
    			()=>{
			    		var connector1 = designerItem1.connector(position1);
			    		var connector2 = designerItem2.connector(position2);
			    		var connection = new DiagramDesigner.Connection(connector1, connector2);
						designerCanvas.Children.Add(connection);
						return connection;				
					});
    	}
    }
    
}
