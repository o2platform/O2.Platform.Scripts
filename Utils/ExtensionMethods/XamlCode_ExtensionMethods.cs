// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using O2.Kernel.ExtensionMethods;
using System.Windows;
using System.Windows.Controls;
using O2.API.Visualization.ExtensionMethods;
using O2.XRules.Database.APIs;

//O2File:WPF_ExtensionMethods.cs
//O2File:XamlCode.cs
//O2File:Xaml_ExtensionMethods.cs

//O2Ref:O2_API_Visualization.dll


namespace O2.XRules.Database.Utils
{    
    
    public static class XamlCode_ExtensionMethods
    {
    	public static Button add_Xaml_Button(this UIElement uiElement, string buttonText)
    	{
    		return uiElement.xaml_CreateUIElement<Button>(XamlCode.new_Button(buttonText));	   
    	}
    	public static Button add_Xaml_Button(this UIElement uiElement, string buttonText,  Action onClickCallback)
    	{
    		return uiElement.add_Xaml_Button( buttonText, -1,-1 ,onClickCallback);
    	}
    	
    	public static Button add_Xaml_Button(this UIElement uiElement, string buttonText, int left, int top,  Action onClickCallback)
    	{
    		var button = uiElement.add_Xaml_Button(buttonText);
    		button.onClick_Wpf(onClickCallback);
    		if (left > -1)
    			button.left_Wpf(left);
    		if (top > -1)
    			button.top_Wpf(top);
			  
    		return button;
    	}
    	    	    	
    	public static Button add_Xaml_Link(this UIElement uiElement, string linkText, Action onClickCallback)
    	{
    		return uiElement.add_Xaml_Link(linkText, "5",onClickCallback);
    	}
    	
    	public static Button add_Xaml_Link(this UIElement uiElement, string linkText, string margin, Action onClickCallback)
    	{
    		return (Button)uiElement.wpfInvoke(
    			()=>{
    					var link = uiElement.xaml_CreateUIElement<Button>(XamlCode.link(linkText,margin));	 
    					link.Focusable = false;
    					link.onClick_Wpf(onClickCallback);
    					return link;
    				});
    	}    	    	    	
    }
    
}
