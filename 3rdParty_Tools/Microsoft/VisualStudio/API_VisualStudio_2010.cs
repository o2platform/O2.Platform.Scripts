// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualStudio.CommandBars;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
using EnvDTE;
using EnvDTE80;
using O2.FluentSharp.VisualStudio;

//O2File:O2_Add_In\O2_VS_AddIn.cs
//O2File:O2_Add_In\VisualStudio_Connect.cs

//O2Ref:EnvDTE.dll
//O2Ref:EnvDTE80.dll
//O2Ref:Extensibility.dll 
//O2Ref:Microsoft.VisualStudio.CommandBars.dll

namespace O2.XRules.Database.APIs
{
    public class API_VisualStudio_2010
    {   
    	public bool 		InsideVisualStudio 	{ get; set; }
    	public O2_VS_AddIn 	VsAddIn				{ get; set; }
    	
    	public API_VisualStudio_2010()
    	{    		
    		load_VsAddIn();
    		InsideVisualStudio = VsAddIn.notNull();
    	}
    	
    	public void load_VsAddIn()
    	{
    		VsAddIn = O2_VS_AddIn.create_FromO2LiveObjects();
    	}
    } 
      
    public static class API_VisualStudio_2010_ExtensionMethods
    {
    	public static CommandBarPopup add_TopMenu(this API_VisualStudio_2010 visualStudio, string caption)
    	{
    		return visualStudio.VsAddIn.add_TopMenu(caption);
    	}    
    	
    	public static List<Window> windows(this API_VisualStudio_2010 visualStudio)
    	{    		
    		var windows = new List<Window> ();
    		foreach(Window window in visualStudio.VsAddIn.VS_Dte.Windows)		//Lambda doesn't work
    			windows.add(window);
    		return windows;    	
    	}    	
    	
    	public static List<string> captions(this List<Window> windows)
    	{
    		return (from window in windows
    				select window.Caption).toList();
    	}
    	
    	public static Window window(this API_VisualStudio_2010 visualStudio, string name)
    	{
    		foreach(var window in visualStudio.windows())
    			if (window.Caption == name)
    				return window;
    		return null;	
    	}
    }
}