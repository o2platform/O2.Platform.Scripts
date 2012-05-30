// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Windows.Forms;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using Microsoft.ACESec.CATNet.UI;
using O2.FluentSharp.VisualStudio;
using O2.XRules.Database.Utils;
using CefSharp.WinForms;
using EnvDTE; 


//O2File:_Extra_methods_WinForms_Controls.cs

//O2File:API_CatNet_VS2010.cs
//O2File:API_VisualStudio_2010.cs
//O2File:O2_VS_AddIn.cs
//O2File:API_Chrome.cs

//O2Ref:CefSharp\CefSharp-0.11-bin\CefSharp.WinForms.dll
//O2Ref:CefSharp\CefSharp-0.11-bin\CefSharp.dll
//O2Ref:EnvDTE.dll
//O2Ref:Extensibility.dll
//O2Ref:C:\Program Files (x86)\Microsoft\CAT.NET\Microsoft.ACESec.CATNet.UI.VSAddIn.dll

namespace O2.XRules.Database.APIs
{
	public class API_CatNet_TeamMentor_VS2010 : API_CatNet_VS2010
	{
		public string TeamMentorUrl 			{ get; set;}		
		public Action<ReportItem> onReportItem  { get; set;}
		
		public API_CatNet_TeamMentor_VS2010()
		{
			TeamMentorUrl = "http://127.0.0.1:12110/";				
			if(this.haveExtraFeaturesBeenAdded().isFalse())
			{
				this.set_OnReportItem_Rules()
					.set_OnReportItem_Callback()
					.add_Buttons_to_ToolStrip()
					.open_Window_TeamMentorArticle()
					.add_Feature_OpenReportOnFileDrop();
			}
			"API_CatNet_TeamMentor_VS2012".o2Cache(this);	
		}		
		
		public new static API_CatNet_TeamMentor_VS2010 Current
		{
			get 
			{
				return "API_CatNet_TeamMentor_VS2012".o2Cache<API_CatNet_TeamMentor_VS2010>(
							()=>{
									return new API_CatNet_TeamMentor_VS2010();
								});
			}
		}

    }
      
    public static class API_CatNet_TeamMentor_VS2010_Rules    
    {
    	public static API_CatNet_TeamMentor_VS2010 set_OnReportItem_Rules(this API_CatNet_TeamMentor_VS2010 tmCatNet)
    	{
    		//"[set_OnReportItem_Rules]".info();
    		tmCatNet.onReportItem = (reportItem)=>
    			{
    				"in set_OnReportItem_Rules V3:".info();						
					var article = "";
					switch(reportItem.HelpCaption)
					{
						case "Exception Information":
							article = "Validate Input from All Sources";
							break;
						case "SQL Injection":
							article = "How to Test For SQL%20Injection%20Bugs";
							break;
						case "Cross-Site Scripting":
							article = "Cross Site Scripting Attack";
							break;
						case "File Canonicalization":	
							article = "File Name And Path Manipulation Attack";
							break;
						default:
							"No mapping for: {0}".info(reportItem.HelpCaption);
							break;
					}					
					if (article.valid())
						tmCatNet.open_In_TeamMentorArticle(tmCatNet.TeamMentorUrl +"article/" +  article);
					else
						tmCatNet.open_In_TeamMentorArticle("https://www.google.co.uk/#q=" + reportItem.HelpCaption);
				};
    		return tmCatNet;
    	}
    }
    			
    
    public static class API_CatNet_TeamMentor_VS2012_ExtensionMethods
    {
    	public static bool haveExtraFeaturesBeenAdded(this API_CatNet_TeamMentor_VS2010 tmCatNet)
    	{
    		return tmCatNet.Actions.item("Open TeamMentor GUI").notNull();
    	}
    	
    	public static API_CatNet_TeamMentor_VS2010 set_OnReportItem_Callback(this API_CatNet_TeamMentor_VS2010 tmCatNet)
    	{
    		
		
    		"[API_CatNet_TeamMentor_VS2012] set_OnReportItem_Callback".debug();
    		//tmCatNet.onReportItem = (reportItem)=> reportItem.str().info();	    			
	    	tmCatNet.Summary.afterSelected<ReportItem>(
	    		(reportItem) => {
	    							//"in HERE".error();
		    						tmCatNet.onReportItem(reportItem);
		    					});		    
	    	return tmCatNet;
    	}
    	
    	public static API_CatNet_TeamMentor_VS2010 add_Buttons_to_ToolStrip(this API_CatNet_TeamMentor_VS2010 tmCatNet)
    	{
    		
			"[API_CatNet_TeamMentor_VS2012] add_Buttons_to_ToolStrip".debug();
			tmCatNet.add_Button_TeamMentorGui();
    		
    		return tmCatNet;
    	}
    	
    	public static API_CatNet_TeamMentor_VS2010 add_Button_TeamMentorGui(this API_CatNet_TeamMentor_VS2010 tmCatNet)
    	{
    		var summaryView = tmCatNet.SummaryView;			
			//tsActions.Items.Clear();
			var button = new ToolStripButton();
			button.Text = "Open TeamMentor GUI";
			button.Click+= (sender,e)=>
				{
					var topPanel = tmCatNet.VsAddIn.add_WinForm_Panel("TeamMentor Gui", 1000,600);
					"Opening TeamMentor GUI".info();
					var chrome = topPanel.add_Chrome();

					chrome.open(tmCatNet.TeamMentorUrl);
				};	
			tmCatNet.Actions.Items.Add(button);


    		return tmCatNet;
    	}
    	
    	public static API_CatNet_TeamMentor_VS2010 open_Window_TeamMentorArticle(this API_CatNet_TeamMentor_VS2010 tmCatNet)
    	{
    		var windowName = "TeamMentor Article"; 
    		var window = tmCatNet.window(windowName);
    		if (window.notNull())
    		{
    			window.Visible = true;    			
    		}
    		else
    		{
    			var chrome = tmCatNet.VsAddIn.add_WinForm_Panel(windowName, 700,300)
    									 	 .add_Chrome();
				"TeamMentor_Article".o2Cache(chrome);
			}
			tmCatNet.window(windowName).IsFloating = false;
			return tmCatNet;
		}
		
		public static API_CatNet_TeamMentor_VS2010 open_In_TeamMentorArticle(this API_CatNet_TeamMentor_VS2010 tmCatNet, string url)
		{
			tmCatNet.open_Window_TeamMentorArticle();
			var chrome = (WebView)"TeamMentor_Article".o2Cache();
			if (chrome.isNull())
				"[API_CatNet_TeamMentor_VS2012] open_In_TeamMentorArticle: could not get chrome object".error();
			else
			{
				"[API_CatNet_TeamMentor_VS2012] open_In_TeamMentorArticle: {0}".info(url);
				chrome.open(url);
			}
			return tmCatNet;
		}
    }
}