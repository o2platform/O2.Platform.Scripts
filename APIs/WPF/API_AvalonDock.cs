// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using AvalonDock;
using FluentSharp.CoreLib;
using FluentSharp.WPF;
using FluentSharp.WinForms;
using WPF = System.Windows.Controls;

//O2File:ElementHost_ExtensionMethods.cs
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:ICSharpCode.AvalonEdit.dll
//O2Ref:GraphSharp.Controls.dll
//O2Ref:PresentationCore.dll
//O2Ref:WindowsBase.dll
//O2Ref:System.Xaml.dll
//O2Ref:PresentationFramework.dll
//O2Ref:AvalonDock.dll

namespace O2.XRules.Database.APIs
{
    public static class API_AvalonDock
    {    
    	public static DockingManager add_DockingManager<T>(this T control)
    		where T : System.Windows.Forms.Control
    	{
    		control.clear();
			var wpfHost = control.add_Wpf();
			return wpfHost.add_Control_Wpf<DockingManager>(); 
    	}
    	
    	public static ResizingPanel add_ResizingPanel(this DockingManager dockingManager)
    	{
    		return dockingManager.add_ResizingPanel(WPF.Orientation.Horizontal);
    	}
    	
    	public static ResizingPanel add_ResizingPanel(this DockingManager dockingManager, WPF.Orientation orientation)
    	{
    		return (ResizingPanel)dockingManager.wpfInvoke(
    			()=>{
    					var resizingPanel = new ResizingPanel() { Orientation =  orientation };
    					dockingManager.Content = resizingPanel;
    					return resizingPanel;
    				});
    		
    	}
    	
    	public static DockablePane add_DockablePane<T>(this T resizingPanel)
    		where T : WPF.Panel
    	{
    		return (DockablePane)resizingPanel.wpfInvoke(
    			()=>{
    					var dockablePane = new DockablePane();
    					resizingPanel.Children.Add(dockablePane);
    					return dockablePane;
    				});
    	}
    	
    	public static DocumentPane add_DocumentPane<T>(this T resizingPanel)
    		where T : WPF.Panel
    	{
    		return (DocumentPane)resizingPanel.wpfInvoke(
    			()=>{
    					var documentPane = new DocumentPane();
    					resizingPanel.Children.Add(documentPane);
    					return documentPane;
    				});
    	}    	
    	
    	public static DockableContent add_DockableContent(this Pane pane, string name, string title)    		
    	{
    		return pane.add_ManagedContent<DockableContent>(name, title);
    		/*return (DockableContent)pane.wpfInvoke(
    			()=>{
    					var dockableContent = new DockableContent()
    						{
    							Name = name,
    							Title = title
    						};

    					pane.Items.Add(dockableContent);
    					return dockableContent;
    				});*/
    	}
    	
    	public static DocumentContent add_DocumentContent(this Pane pane, string name)
    	{
    		return pane.add_DocumentContent(name.Replace(" ","_"),name);
    	}
    	
    	public static DocumentContent add_DocumentContent(this Pane pane, string name,string title)    		
    	{    		
    		return pane.add_ManagedContent<DocumentContent>(name, title);
    		/*return (DocumentContent)pane.wpfInvoke(
    			()=>{
    					var documentContent = new DocumentContent()
    						{    							
    							Title = title
    						};

    					pane.Items.Add(documentContent);
    					return documentContent;
    				});*/
    	}
    	
    	public static T add_ManagedContent<T>(this Pane pane,string name, string title)
    		where T : ManagedContent
    	{
    		return (T)pane.wpfInvoke(
    			()=>{
    					var managedContent = (T)typeof(T).ctor();
    					managedContent.Name = name;
    					managedContent.Title = title;    						

    					pane.Items.Add(managedContent);
    					return managedContent;
    				});
    	}
    	    	
    	
    	public static T add_WinForms_Control<T>(this ManagedContent managedContent)
    		where T : Control
    	{
    		try
    		{
	    		var panel =  managedContent.add_WinForms_Panel();	    		
				panel.width(400) 		// give the panel a decent size to that it doesn't cause problems during the new T Control dynamic Gui creation
					 .height(400);												
				"[add_WinForms_Control] new control size -  width: {0} height:{0}".info(panel.width(), panel.height());
				Application.DoEvents();
				var newControl = panel.add_Control<T>();
				if (newControl.isNull())
					"[add_WinForms_Control] failed the creation of control : {0}...".error(typeof(T));
				else
				{
					"[add_WinForms_Control] created ok control : {0}...".info(typeof(T));
					return newControl;
				}
			}
			catch(Exception ex)
			{
				ex.log();
				"[add_WinForms_Control] error creating control of type: {0}...".error(typeof(T));
				
			}
			return null;
		}
		
		public static T selectedItem<T>(this T managedContent)
			where T : ManagedContent
		{
			return managedContent.setAsActive();
		}
		
		public static T setAsActive<T>(this T managedContent)
			where T : ManagedContent
		{
			return (T)managedContent.wpfInvoke(
				()=>{
						managedContent.SetAsActive();
						return managedContent;
					});
		}
		
		
    	
    	// width
    	
    	public static DockablePane resizeWidth(this DockablePane dockablePane, int width)
    	{
    		return (DockablePane)dockablePane.wpfInvoke(
				()=>{	
						try
						{
							AvalonDock.ResizingPanel.SetResizeWidth(dockablePane, new System.Windows.GridLength(width)); 
						}
						catch(Exception ex)
						{
							ex.log("");
						}
						return dockablePane;
					});		
    	}
    	
    	public static ResizingPanel resizeWidth(this ResizingPanel dockablePane, int width)
    	{
    		return (ResizingPanel)dockablePane.wpfInvoke(
				()=>{	
						try
						{
							AvalonDock.ResizingPanel.SetResizeWidth(dockablePane, new System.Windows.GridLength(width)); 
						}
						catch(Exception ex)
						{
							ex.log("");
						}
						return dockablePane;
					});		
    	}
    }
}
