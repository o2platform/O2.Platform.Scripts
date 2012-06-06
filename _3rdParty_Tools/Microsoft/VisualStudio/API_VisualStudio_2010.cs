// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
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

//O2File:O2_VS_AddIn.cs
//O2File:VisualStudio_Connect.cs

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
    	
    	public static API_VisualStudio_2010 Current
		{
			get 
			{
				return "API_VisualStudio_2010".o2Cache<API_VisualStudio_2010>(
							()=>{
									return new API_VisualStudio_2010();
								});
			}
		}
    } 
      
    public static class API_VisualStudio_2010_ExtensionMethods_Objects
    {
    	public static DTE2 dte(this API_VisualStudio_2010 visualStudio)
    	{    	
    		return (DTE2)visualStudio.VsAddIn.VS_Dte;
    	}
    }
    
    public static class API_VisualStudio_2010_ExtensionMethods_CommandExecution
    {
    	public static API_VisualStudio_2010 addInManager(this API_VisualStudio_2010 visualStudio)
    	{
    		return visualStudio.executeCommand("Tools.AddinManager");    		
    	}
    }
    
    public static class API_VisualStudio_2010_ExtensionMethods_Object_Wrappers
    {    
    	public static List<Window> windows(this API_VisualStudio_2010 visualStudio)
    	{   
    		return (from Window window in visualStudio.dte().Windows
    				select window).toList();    		
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
    	public static Window window_waitFor(this API_VisualStudio_2010 visualStudio, string name, int nTimes = 5)
    	{
    		for(int i =0 ; i < nTimes; i ++)
    		{
    			var window = visualStudio.window(name);
    			if (window.notNull())
    				return window;
    			"[window_waitFor] waiting for window with name: {0}".info(name);
    		}
    		return null;
    	}
    	
    	
    	public static List<Command> commands(this API_VisualStudio_2010 visualStudio)
    	{
    		return (from Command command in visualStudio.dte().Commands
    				select command).toList();
    	}    	
    	
    	public static Command command(this API_VisualStudio_2010 visualStudio, string name)
    	{
    		return visualStudio.commands()
    						   .where((command)=> command.Name == name)
    						   .first();
    	}
    	
    	public static API_VisualStudio_2010 executeCommand(this API_VisualStudio_2010 visualStudio, string commandName, string parameter ="")
    	{
    		try
    		{
    			visualStudio.dte().ExecuteCommand(commandName, parameter);
    		}
    		catch(Exception ex)
    		{
    			ex.log("[API_VisualStudio_2010] executeCommand: ");
    		}
    		return visualStudio;
    	}
    	

    	
    }
    
    public static class API_VisualStudio_2010_ExtensionMethods_GUI
    {
        public static CommandBarPopup add_TopMenu(this API_VisualStudio_2010 visualStudio, string caption)
    	{
    		return visualStudio.VsAddIn.add_TopMenu(caption);
    	}    
    	
    	public static Panel new_Panel(this API_VisualStudio_2010 visualStudio, string caption = "New Panel", int width = -1 , int height = -1)
    	{
    		return visualStudio.VsAddIn.add_WinForm_Panel(caption, width, height);
    	}
    }

}