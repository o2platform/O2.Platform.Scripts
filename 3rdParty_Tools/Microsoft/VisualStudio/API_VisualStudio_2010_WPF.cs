// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.PlatformUI.Shell;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
using O2.FluentSharp.VisualStudio;
using O2.API.Visualization.ExtensionMethods;

//O2File:API_VisualStudio_2010.cs
//O2File:WPF_Controls_ExtensionMethods.cs

//O2Ref:EnvDTE.dll
//O2Ref:EnvDTE80.dll
//O2Ref:Extensibility.dll 
//O2Ref:Microsoft.VisualStudio.CommandBars.dll
//O2Ref:Microsoft.VisualStudio.Shell.ViewManager.dll
//O2Ref:O2_API_Visualization.dll

//O2Ref:PresentationCore.dll 
//O2Ref:PresentationFramework.dll
//O2Ref:System.Xaml.dll
//O2Ref:WindowsBase.dll
//O2Ref:WindowsFormsIntegration.dll

namespace O2.XRules.Database.APIs
{
    public class API_VisualStudio_2010_WPF : API_VisualStudio_2010
    {   
    	
    	   
    	public API_VisualStudio_2010_WPF()
    	{    		
    		//VisualStudio = new API_VisualStudio_2010();
    	}    	
    } 
      
    public static class API_VisualStudio_2010_WPF_ExtensionMethods
    {
    	public static Window mainWindow(this API_VisualStudio_2010_WPF visualStudio)
    	{
    		var vsAddin = visualStudio.VsAddIn;
			var impl = vsAddin.VS_Dte.MainWindow.field("_impl");
			var window = (System.Windows.Window)impl.prop("Window");
			return window;
    	}
    	
		public static MainSite mainSite(this API_VisualStudio_2010_WPF visualStudio)
		{
			 var window =  visualStudio.mainWindow(); 
			 var contentControl = (ContentControl)window.controls_Wpf().second();
			 return (MainSite)contentControl.wpfInvoke( 
			 	()=>{			 			
			 			return contentControl.Content; //returns Microsoft.VisualStudio.PlatformUI.Shell.MainSite
			 		});	

		}
    }
    
    // Microsoft.VisualStudio.PlatformUI.Shell.ViewElement
    public static class PlatformUI_ExtensionMethods
    {
    
    	public static ViewElement control(this ViewElement viewElement, string title)
    	{
    		return (ViewElement)viewElement.wpfInvoke(
				()=>{	
		    			foreach(var control in viewElement.controls())
    						if (control.str() == title)
    							return control;
    					return null;
    				});    						
    	}
    	
    	public static  List<ViewElement> controls(this ViewElement viewElement)//, Predicate<ViewElement> predicate)    	
    	{
    		return viewElement.controls((ViewElement v) => v != null);
    	}
    	
    	public static  List<ViewElement> controls(this ViewElement viewElement, Predicate<ViewElement> predicate)    	
    	{
    		return (List<ViewElement>)viewElement.wpfInvoke(
				()=>{						
						return viewElement.FindAll(predicate).toList();
					});    	
		}
		
		public static  List<string> titles(this List<ViewElement> viewElements)
    	{
    		return (List<string>)viewElements.first().wpfInvoke(
				()=>{	
						return (from viewElement in viewElements
								select viewElement.str()).toList();						
					});    	
		}
		
		public static T content<T>(this View view)
		{
			var content = view.content();
			if (content is T)
				return (T)content;
			return default(T);
		}
		
		public static object content(this View view)
        {
        	return view.wpfInvoke(
        		()=>{
        				return view.Content;
        			});
        }
		
    }
}