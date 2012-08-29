// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.Windows;
//using O2.Views.ASCX.classes.MainGUI;
//using O2.Views.ASCX.ExtensionMethods;
//O2File:WebscarabConversation.cs
//O2File:API_InputSimulator.cs
//O2File:Tool_API.cs
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll

//O2Ref:WindowsFormsIntegration.dll
//O2Ref:PresentationFramework.dll
//O2Ref:PresentationCore.dll
//O2Ref:WindowsBase.dll
//O2Ref:System.Xaml.dll

using O2.XRules.Database.Utils;

namespace O2.XRules.Database.APIs
{	
 
	public class API_WebScarab : Tool_API
    {    
    	public string WebScarab_Jar {get;set;}
    	public Process WebScarab_Process {get;set;}
    	public bool LiteMode {get;set;}
    	public API_InputSimulator InputSimulator { get; set;} 
    	
    	public int PROCESS_START_MAX_SLEEP_SECONDS = 10;
    	
    	public API_WebScarab()
		{
			config("WebScarab", "WebScarab 2010-08-20", "webscarab-one-20100820-1632.jar");			
    		Install_Uri = "http://dawes.za.net/rogan/webscarab/webscarab-one-20100820-1632.jar".uri();    		    		
    		WebScarab_Jar = Install_Dir.pathCombine(Install_File);
    		InputSimulator = new API_InputSimulator();   
    		install();
		}
		 
		
		public bool install()
		{			
			return installFromWeb_Jar();			
		}
		
		public API_WebScarab start()
		{		
			if (WebScarab_Process.notNull())
			{
				"in API_WebScarab start, the WebScarab_Process is already mapped to a running process. Aborting request".error();
				return this;
			}
			attach();
			if (WebScarab_Process.notNull())		// means we were able to find a running instance and get its process object
				return this;
			//if not launch it
			if (WebScarab_Jar.fileExists().isFalse())
			{
				"[WebScarab_API] Could not find WebScarab Jar file at location: {0}".error(WebScarab_Jar);
				return null;
			}
			WebScarab_Process = Processes.startProcess(WebScarab_Jar);
			for(int i=0; i < PROCESS_START_MAX_SLEEP_SECONDS ; i ++)
			{
				this.sleep(1000); 
				WebScarab_Process.Refresh();
				var mainWindowTitle = WebScarab_Process.MainWindowTitle;
				if (mainWindowTitle.valid())			
				{
					"Found MainWindowTitle: {0}".info(mainWindowTitle);
					this.LiteMode = (WebScarab_Process.MainWindowTitle == "WebScarab Lite");
					return this;
				}
			}
			"Could not retreive main window for launched instance of webscarab".error();
			return this;
		}				
    
    	public API_WebScarab attach()
    	{
    		// try to attach to a running instance of WebScarab
			var liteMode = "javaw".getProcessWithWindowTitle("WebScarab Lite"); 
			if (liteMode.notNull())
			{
				"Found a running instance of WebScarab in Lite Mode".info();
				WebScarab_Process = liteMode;
				LiteMode = true;
				
			}
			else
			{
				var advancedMode = "javaw".getProcessWithWindowTitle("WebScarab");
				if (advancedMode.notNull())
				{
					"Found a running instance of WebScarab in Advanced Mode".info();
					WebScarab_Process = advancedMode;
					LiteMode = false;
					
				}
			}
			return this;
    	}
    }
    
    public static class API_WebScarab_ExtensionMethods
    {
    	
    	public static API_WebScarab restart(this API_WebScarab webScarab)
    	{
    		webScarab.stop();
    		return webScarab.start();    		
    	}
    	
    	public static API_WebScarab stop(this API_WebScarab webScarab)
    	{
    		if (webScarab.WebScarab_Process.notNull())
    		{
    			webScarab.WebScarab_Process.stop();
    			webScarab.waitForExit();
    			webScarab.WebScarab_Process = null;
    		}
    		return webScarab;
    	}
    	
    	public static API_WebScarab waitForExit(this API_WebScarab webScarab)
    	{
    		webScarab.WebScarab_Process.WaitForExit();
    		return webScarab;
    	}
    	
    	public static bool started(this API_WebScarab webScarab)
    	{
    		return (webScarab.WebScarab_Process.notNull() && 
					webScarab.WebScarab_Process.HasExited.isFalse() && 
					webScarab.WebScarab_Process.MainWindowHandle != IntPtr.Zero);
    	}
    }
    
    public static class API_WebScarab_ExtensionMethods_Gui
    {
    
    	public static API_WebScarab bringToFront(this API_WebScarab webScarab)
    	{    		
    		webScarab.InputSimulator.bringToFront(webScarab.WebScarab_Process);
    		return webScarab;
    	}
    	
    	public static API_WebScarab alwaysOnTop(this API_WebScarab webScarab)
    	{
    		return webScarab.alwaysOnTop(true);
    	}
    	public static API_WebScarab alwaysOnTop(this API_WebScarab webScarab, bool value)
    	{    		
    		webScarab.InputSimulator.alwaysOnTop(webScarab.WebScarab_Process,value);  
    		return webScarab;
    	}
    	
    	
    	public static API_WebScarab moveWindow(this API_WebScarab webScarab, int left, int top, int width, int height)
    	{    		
    		webScarab.InputSimulator.moveWindow(webScarab.WebScarab_Process, left, top, width, height);
    		return webScarab;
    	}
        	
    	public static API_WebScarab startAndSync(this API_WebScarab webScarab, Control control)
    	{
    		webScarab.start();
    		webScarab.syncGuiPositionWithControl(control);
    		return webScarab;
    	}
    	    	
    	
    	// this should only be executed once per Form
    	public static API_WebScarab syncGuiPositionWithControl(this API_WebScarab webScarab, Control control)
    	{
		    Action moveToControl = 
				()=>{
						webScarab.alwaysOnTop(true); 
						var xPos =  control.PointToScreen(System.Drawing.Point.Empty).X;
						var yPos =  control.PointToScreen(System.Drawing.Point.Empty).Y;
						var width = control.width();
						var height = control.height();
						webScarab.moveWindow(xPos, yPos, width, height);  
					};	
						
			control.parentForm().Move += 
				(sender,e)=> moveToControl();
			 
			control.Resize +=  
				(sender,e)=> moveToControl();
			moveToControl();							
			return webScarab;
		}

    }
    
    public static class API_WebScarab_ExtensionMethods_Gui_Automation
    {
    	public static API_WebScarab setInterface_Lite(this API_WebScarab webScarab)
    	{
    		if (webScarab.LiteMode)
    		{
    			"in API_WebScarab setInterface_Lite, we are already using the 'Lite' Interface, aborting request".error();
    			return webScarab;
    		}
    		    	    
			
			//API_InputSimulator_NativeMethods.MoveWindow(windowHandle,0,0,600,400,true); 
			webScarab.bringToFront();    	    
			webScarab.InputSimulator.set_XY_OffsetToWindow(webScarab.WebScarab_Process); // this will make the mouse_MoveTo below to be relative to the current webscarab window
			
			// open menu			
			webScarab.InputSimulator.mouse_MoveTo(20, 30).click(); 
						
			//move to 'Use Lite Interface' Menu Item
			webScarab.InputSimulator.mouse_MoveTo(90,30);  
			webScarab.InputSimulator.mouse_MoveTo(90,220).click(); 
			
			// Press Ok			
			webScarab.InputSimulator.send_Tab();			
			webScarab.InputSimulator.send_Enter();
//			webScarab.InputSimulator.mouse_MoveTo(390,215).click(); 			
			// click 'exit' menu item
			webScarab.InputSimulator.mouse_MoveTo(20,30).click(); 
			webScarab.InputSimulator.mouse_MoveTo(20,120).click();	
			
			webScarab.waitForExit();
			return webScarab;
		}
		
		public static API_WebScarab setInterface_FullFeatured(this API_WebScarab webScarab)
    	{
    		if (webScarab.LiteMode.isFalse())
    		{
    			"in API_WebScarab setInterface_FullFeatured, we are already using the 'FullFeatured(' Interface, aborting request".error();
    			return webScarab;
    		}
    	    	
			//var windowHandle = webScarab.WebScarab_Process.MainWindowHandle; 
			//WindowsInput.Native.NativeMethods.SetForegroundWindow(windowHandle);  
			//API_InputSimulator_NativeMethods.MoveWindow(windowHandle,0,0,600,400,true); 
			
			webScarab.bringToFront();    	    
			webScarab.InputSimulator.set_XY_OffsetToWindow(webScarab.WebScarab_Process); // this will make the mouse_MoveTo below to be relative to the current webscarab window
			
			// open menu
			webScarab.InputSimulator.mouse_MoveTo(20,30).click(); 
			
			//move to 'Use Full_Featured Interface' Menu Item
			webScarab.InputSimulator.mouse_MoveTo(90,30);  
			webScarab.InputSimulator.mouse_MoveTo(90,150).click(); 
			
			// Press Ok			
			webScarab.InputSimulator.send_Tab();
			webScarab.InputSimulator.send_Enter();
//			webScarab.InputSimulator.mouse_MoveTo(390,215).click(); 			
			// click 'exit' menu item
			webScarab.InputSimulator.mouse_MoveTo(20,30).click(); 
			webScarab.InputSimulator.mouse_MoveTo(20,120).click();		
			
			webScarab.waitForExit();
			return webScarab;
		}
		
		public static API_WebScarab saveConversations(this API_WebScarab webScarab, Control hostControl, string conversationsSavePath)
		{
		
			"Saving Current webscarab conversations to: {0}".info(conversationsSavePath); 				
			webScarab.bringToFront();    	    
			webScarab.InputSimulator.set_XY_OffsetToWindow(webScarab.WebScarab_Process); // this will make the mouse_MoveTo below to be relative to the current webscarab window
			
			// open menu			
			webScarab.InputSimulator.mouse_MoveTo(20, 30).click(); 
			// save button
			webScarab.InputSimulator.mouse_MoveTo(20, 100).click();  					
			hostControl.sendKeys(conversationsSavePath).sleep(1000);  // this API is more robust for WebScarab
			Application.DoEvents();			
			webScarab.InputSimulator.send_Tab().sleep(500);
			webScarab.InputSimulator.send_Tab().sleep(500);
			webScarab.InputSimulator.send_Enter().sleep(500); 
			"API_WebScarab conversation save complete".info();
			return webScarab;
		}
		
		public static API_WebScarab loadConversations(this API_WebScarab webScarab, Control hostControl, string conversationsSavePath)
		{		
			// open menu			
			webScarab.bringToFront();    	    
			webScarab.InputSimulator.set_XY_OffsetToWindow(webScarab.WebScarab_Process); // this will make the mouse_MoveTo below to be relative to the current webscarab window   				
			webScarab.InputSimulator.mouse_MoveTo(20, 30).click(); 
			// save button 
			webScarab.InputSimulator.mouse_MoveTo(20, 80).click();  		
			hostControl.sendKeys("").sleep(1000);
			hostControl.sendKeys(conversationsSavePath).sleep(1000);  // this API is more robust for WebScarab			
			Application.DoEvents();
			hostControl.sendKeys("").sleep(1000);
			webScarab.InputSimulator.send_Tab().sleep(500);
			webScarab.InputSimulator.send_Tab().sleep(500);
			webScarab.InputSimulator.send_Enter().sleep(500); 
			"API_WebScarab conversation load complete".info();
			return webScarab;
		}
    }
    
    public static class API_WebScarab_ExtensionMethods_ConversationsFile
    {
    
    	public static List<IWebscarabConversation> loadConversationsFile(this API_WebScarab webScarab, string conversationFile)    	
    	{
    		var webScarabConversations = new List<IWebscarabConversation>();
            if (!File.Exists(conversationFile))
            {
                "Could not find webscarab conversation file: {0}".error(conversationFile);
            }
            else
            {
                List<string> fileLines = Files.getFileLines(conversationFile);
                string requestAndResponseFiles = Path.Combine(Path.GetDirectoryName(conversationFile), "conversations");
                "There are {0} lines in the loaded file: {1}".info(fileLines.Count, conversationFile );
                IWebscarabConversation currentConversation = null;
                foreach (string line in fileLines)
                {
                    DictionaryEntry parsedLine = getParsedLine(line);
                    if (parsedLine.Key != null)
                    {
                        switch (parsedLine.Key.ToString())
                        {
                            case "### Conversation ":
                                if (currentConversation != null)
                                {
                                    webScarabConversations.Add(currentConversation);
                                }
                                currentConversation = new WebscarabConversation();
                                currentConversation.id = parsedLine.Value.ToString();
                                goto Label_039B;

                            case "RESPONSE_SIZE":
                                currentConversation.RESPONSE_SIZE = parsedLine.Value.ToString();
                                goto Label_039B;

                            case "WHEN":
                                currentConversation.WHEN = parsedLine.Value.ToString();
                                goto Label_039B;

                            case "METHOD":
                                currentConversation.METHOD = parsedLine.Value.ToString();
                                goto Label_039B;

                            case "COOKIE":
                                currentConversation.COOKIE = parsedLine.Value.ToString();
                                goto Label_039B;

                            case "STATUS":
                                currentConversation.STATUS = parsedLine.Value.ToString();
                                goto Label_039B;

                            case "URL":
                                currentConversation.URL = parsedLine.Value.ToString();
                                goto Label_039B;

                            case "TAG":
                                currentConversation.TAG = parsedLine.Value.ToString();
                                goto Label_039B;

                            case "ORIGIN":
                                currentConversation.ORIGIN = parsedLine.Value.ToString();
                                goto Label_039B;

                            case "XSS-GET":
                                currentConversation.XSS_GET.Add(parsedLine.Value.ToString());
                                goto Label_039B;

                            case "CRLF-GET":
                                currentConversation.CRLF_GET.Add(parsedLine.Value.ToString());
                                goto Label_039B;

                            case "SET-COOKIE":
                                currentConversation.SET_COOKIE.Add(parsedLine.Value.ToString());
                                goto Label_039B;

                            case "XSS-POST":
                                currentConversation.XSS_POST.Add(parsedLine.Value.ToString());
                                goto Label_039B;
                        }
                        "Key value not handled: {0} for {1}".error(parsedLine.Key.ToString(), parsedLine.Value.ToString());
                    }
                Label_039B:
                    if (currentConversation != null)
                    {
                        currentConversation.request = string.Format(@"{0}\{1}-request", requestAndResponseFiles, currentConversation.id);
                        currentConversation.response = string.Format(@"{0}\{1}-response", requestAndResponseFiles, currentConversation.id);
                    }
                }
            }
            return webScarabConversations;
    	}
    	
    	public static DictionaryEntry getParsedLine(string line)
        {
            int indexOfFirstColon = line.IndexOf(':');
            if (indexOfFirstColon > -1)
            {
                string key = line.Substring(0, indexOfFirstColon);
                return new DictionaryEntry(key, line.Substring(indexOfFirstColon + 2));
            }
            return new DictionaryEntry(null, null);
        }
    }
    
    
}
