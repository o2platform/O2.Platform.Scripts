// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using WinForms = System.Windows.Forms;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.Views.ASCX;
using O2.Views.ASCX.classes.MainGUI;
using System.Windows;
using System.Windows.Controls;
using O2.XRules.Database;
using Odyssey.Controls;
using O2.XRules.Database.Utils;

//O2File:ElementHost_ExtensionMethods.cs
//O2File:Xaml_ExtensionMethods.cs
//O2File:O2PlatformWikiAPI.cs

//O2Ref:Odyssey.dll
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:GraphSharp.dll
//O2Ref:QuickGraph.dll 
//O2Ref:GraphSharp.Controls.dll
//O2Ref:ICSharpCode.AvalonEdit.dll

namespace O2.XRules.Database.APIs
{
    public class WPF_GUI : WinForms.Control
    {        	
    	public OutlookBar GUI_OutlookBar { get; set; }
		public WinForms.Panel WinFormPanel { get; set; }
		public WinForms.WebBrowser O2Browser { get; set; }
		public List<WPF_GUI_Section> GuiSections { get; set;}
		public O2PlatformWikiAPI Wiki_O2 { get; set; }
		public WinForms.ToolStripStatusLabel StatusLabel { get; set; }
		public ascx_Execute_Scripts ExecuteScripts { get; set; }
		public int SideBarWidth { get; set; }
		
    	public static void testGui()
    	{    	 
    		var wpfGui = O2Gui.open<WPF_GUI>("Test - O2 WPF Gui");			
			wpfGui.buildGui();			
			wpfGui.add_Section("Main", "This is the intro text. Put here an explanation of what this module is all about")
				  .add_Label("this is a label 1")				  				  
				  .add_Link_O2Script("Util - Simple Html Viewer","Util - Simple Html Viewer.h2")
				  .add_Link_O2Script("script that doesn't exist","Aaaa.h2")				  
				  .add_Link("test user control",
				  	()=>{
				  			"before".info();
				  			wpfGui.WinFormPanel.clear();
				  			wpfGui.WinFormPanel.add_TreeView().backColor(Color.LightGray).add_Node("as");				  			
				  			"after".debug();
				  		})
				  .add_Link_Web("BBC news","http://news.bbc.co.uk")
				  .add_Upgrade_Link("aaa", "http://code.google.com/p/o2platform/downloads/list")
				  .add_Link("Source Code Viewer", (panel)=>{ panel.add_Control<O2.External.SharpDevelop.Ascx.ascx_SourceCodeViewer>();
				  										 })
				  .add_Link("Source Code Editor", (panel)=>{ panel.add_Control<O2.External.SharpDevelop.Ascx.ascx_SourceCodeEditor>();
				  										 });				  										 
				  										 
				  
			wpfGui.add_Section("Section 1", "Text that describes Section 1")
				  .add_Label("this is a label 1")	
				  .add_WrapPanel()
				  	.add_Link("label 2",()=>{})	
				  	.add_Link("label 3",()=>{})	
				  	.add_Link("label 4",()=>{});
			wpfGui.add_Section("Section 2");
			wpfGui.add_Section("Section 3");
			wpfGui.add_Section("Section 4");
			wpfGui.add_Section("Section 5");
			wpfGui.add_Section("Section 6");
			wpfGui.add_Section("Section 7");			
			
    	}
    
    	public WPF_GUI()
    	{
    		this.Width = 640;    		
    		this.Height = 420; 
    		SideBarWidth = 250;
    		//buildGui();
    		GuiSections = new List<WPF_GUI_Section>();
    		Wiki_O2 = new O2PlatformWikiAPI();    		    		
    	}
   
    	/*public WPF_GUI buildGui(List<WPF_GUI_Section> sections)
    	{
    		Sections = sections;
    		return buildGui();
    	}*/
    	public WPF_GUI buildGui()
    	{
    		return (WPF_GUI)this.invokeOnThread(
    			()=>{
			    		
			    		this.backColor(Color.White);
			    		
			    		var wpfHost = this.add_WpfHost();
			    		
			    		var dockPanel = wpfHost.add_Control_Wpf<DockPanel>();
						GUI_OutlookBar =  dockPanel.add_Control_Wpf<OutlookBar>(); 
						GUI_OutlookBar.IsButtonSplitterVisible=false; 
						GUI_OutlookBar.IsOverflowVisible=true;
						GUI_OutlookBar.IsPopupVisible=true;
						GUI_OutlookBar.ShowSideButtons=true;
						GUI_OutlookBar.ShowButtons=true;			
						GUI_OutlookBar.Width = SideBarWidth; 
						//GUI_OutlookBar.MaxWidth = SideBarWidth; 
						GUI_OutlookBar.ButtonHeight = 21;
						
						var userControl = dockPanel.add_WinForms_Panel()
												   .add_Control<WinForms.UserControl>();												   
						StatusLabel = userControl.add_StatusStrip(Color.White);
						WinFormPanel = userControl.add_Panel();						
						
						O2Browser  = WinFormPanel.add_WebBrowser();
						/*O2Browser.fill(false)
								 .align_Right(userControl)
								 .align_Bottom(userControl)
								 .heightAdd(-22)
						         .anchor_All()
						         .silent(true);		*/				
    					
    					WinFormPanel.fill(false)
								    .align_Right(userControl)
								    .align_Bottom(userControl)
								 	.heightAdd(-22)
						         	.anchor_All();
    					
    					statusMessage("WPF GUI built");
						//WinFormPanel.backColor(Color.White);			
						/**add_Sections();
						if (Sections.size() > 0 && Sections[0].WinFormsControl.notNull())
							WinFormPanel.add_Control(Sections[0].WinFormsControl);*/
							
					
							
						return this;
					});
    	}
		
		public WPF_GUI statusMessage(string messageFormat, params object[] messageParams)
		{
			return statusMessage(messageFormat.format(messageParams));
		}
		
		public WPF_GUI statusMessage(string message)
		{
			StatusLabel.set_Text(message);		
			return this;
		}			
    	
    	public WPF_GUI_Section add_Section(string name)
    	{
    		return add_Section(name, "");
    	}
    	
    	public WPF_GUI_Section add_Section(string name, string introText)
    	{
    		return add_Section(new WPF_GUI_Section(name,introText));
    	}
    	
    	public WPF_GUI_Section add_Section(string name, string introText,  Func<WinForms.Control> winFormsCtor)
    	{
    		return add_Section(new WPF_GUI_Section(name,introText,winFormsCtor));
    	}
    	
    	public WPF_GUI_Section add_Section(WPF_GUI_Section section)
    	{
    		section.Wpf_Gui = this;
			return (WPF_GUI_Section)this.invokeOnThread(
    			()=>{
    					var outlookSection = new  OutlookSection();    				
    					
    					section.SectionInGui = outlookSection;
						outlookSection.Header = section.Name;
						var stackPanel = outlookSection.add_StackPanel();						
						if (section.IntroText.valid())
						{
							var textBlock = stackPanel.add_TextBlock();
							textBlock.set_Text_Wpf(section.IntroText);
						}
						//section.ContentPanel = stackPanel.add_WrapPanel();						
						section.ContentPanel = stackPanel.add_StackPanel();						
						
						
						if (section.WinFormsCtor.notNull())
						{
							section.WinFormsControl = section.WinFormsCtor();							
						}						
						
						outlookSection.Click+=
							(sender,e)=>{
											if (section.WinFormsControl.notNull())
											{
												WinFormPanel.clear();
												WinFormPanel.add_Control(section.WinFormsControl);												
											}
										};
						
						GUI_OutlookBar.Sections.Add(outlookSection);												
						GuiSections.Add(section);
						return section;
					});
		}    	   
		
		public string scriptHelpPage(string scriptName)
		{
			return "http://o2platform.com/wiki/O2_Script/{0}".format(scriptName);
		}
		
		public List<Button> Links 
		{
			get{				
					var links = new List<Button>();
					foreach(var section in GuiSections)
						links.AddRange(section.links());
					return links;
				}
		}		
		
    }     
    
    public static class WPF_GUI_ExtensionMethods_Config
    {
        public static WPF_GUI setExecuteScriptsEnvironment(this WPF_GUI wpf_Gui)
    	{
    		if (wpf_Gui.ExecuteScripts.isNull())
    		{    			
    			wpf_Gui.ExecuteScripts = new ascx_Execute_Scripts();
    			wpf_Gui.ExecuteScripts.csharpCompiler_OnAstOk = 
    				()=> wpf_Gui.showMessage("Executing script: {0}".format(wpf_Gui.ExecuteScripts.currentScript.fileName()), "Ast was created Ok");
    			wpf_Gui.ExecuteScripts.csharpCompiler_OnAstFail = 
    				()=>{
    						var scriptName = wpf_Gui.ExecuteScripts.currentScript.fileName();
    						wpf_Gui.showMessage("Executing script: {0}".format(scriptName), "Ast Creation Failed!", wpf_Gui.scriptHelpPage(scriptName));
    					};
    			wpf_Gui.ExecuteScripts.csharpCompiler_OnCompileFail = 
    				()=>{
    						var compilationErrors = wpf_Gui.ExecuteScripts.csharpCompiler.CompilationErrors;
    						var errorMessage = "Compilation Failed!".line() + 
    										   "<br><hr><h4>".line() +
    										   compilationErrors.Replace("".line(), "<br>") + 
    										   "</h4><hr>".line();							    										   
    						wpf_Gui.showMessage("Executing script: {0}".format(wpf_Gui.ExecuteScripts.currentScript.fileName()), errorMessage);
    					};
    			wpf_Gui.ExecuteScripts.csharpCompiler_OnCompileOk = 
    				()=>{
    						var scriptName = wpf_Gui.ExecuteScripts.currentScript.fileName();
    						wpf_Gui.showMessage("Executing script: {0}".format(scriptName), "Compiled OK, executing first method",wpf_Gui.scriptHelpPage(scriptName));    						
    					};
    		}
    		return wpf_Gui;    	
    	}
    	
    	public static WPF_GUI add_OutlookBar(this WinForms.Control control)
    	{
    		return control.add_Control<WPF_GUI>().buildGui();
    	}
    	
    	public static WPF_GUI add_OutlookBar_Left(this WinForms.Control control)
    	{
    		var panel = control.add_Panel();
			var wpfGui = panel.insert_Left<WPF_GUI>().buildGui();
    		panel.parent<WinForms.SplitContainer>().distance(245);
    		//show.info(control.parent<WinForms.SplitContainer>());
    		return wpfGui;
    	}
    }

    public static class WPF_GUI_ExtensionMethods_Add
    {
    	
    }
    
    public static class WPF_GUI_ExtensionMethods_Show
    {
    	public static WPF_GUI showFirstWinFormsPanel(this WPF_GUI wpf_Gui)
    	{
    		foreach(var section in wpf_Gui.GuiSections)
    			if (section.WinFormsControl.notNull())
    				wpf_Gui.WinFormPanel.clear()
    									.add_Control(section.WinFormsControl);
			return wpf_Gui;
    	}
   
    	public static WPF_GUI show_O2Browser(this WPF_GUI wpf_Gui)
    	{
    		if (wpf_Gui.WinFormPanel.controls().contains(wpf_Gui.O2Browser).isFalse())
    		{    		    			
				wpf_Gui.WinFormPanel.clear();    		
				wpf_Gui.WinFormPanel.add_Control(wpf_Gui.O2Browser);				
			}			
			return wpf_Gui;
    	}
    	public static WPF_GUI show_Url(this WPF_GUI wpf_Gui, string url)
    	{       		
			wpf_Gui.show_O2Browser();
			wpf_Gui.statusMessage("Showing URL:{0}",url);
    		if (url.uri().exists())    		
    			wpf_Gui.O2Browser.open(url);    		
    		else
    			wpf_Gui.showOffineMessage("Could not open url: {0}".format(url));
    		return wpf_Gui;
    	}
    	
    	public static WPF_GUI show_YouTubeVideo(this WPF_GUI wpf_Gui,string youTubeVideoId)
    	{
    		wpf_Gui.statusMessage("Showing YouTube Video with Id:{0}",youTubeVideoId);
    		wpf_Gui.WinFormPanel.clear();    		
			wpf_Gui.WinFormPanel.add_Control(wpf_Gui.O2Browser);
			var code = ("<html><body cellspacing=\"0\" cellpadding=\"0\">" + 
					    "<object><param name=\"movie\" width=\"450\" height=\"380\" "+
						"value=\"http://www.youtube.com/v/{0}&amp;hl=en_GB&amp;fs=1\"></param><param name=\"allowFullScreen\" "+
						"value=\"false\"></param><param name=\"allowscriptaccess\" value=\"always\"></param><embed "+
						"src=\"http://www.youtube.com/v/{0}&amp;hl=en_GB&amp;fs=1\" type=\"application/x-shockwave-flash\" "+
						"allowscriptaccess=\"always\" allowfullscreen=\"false\" width=\"450\" height=\"380\"></embed></object>"  + 
						"</body></html>")
						.format(youTubeVideoId);
			wpf_Gui.O2Browser.set_Text(code);
			return wpf_Gui;
    	}    	    	
    	
    	public static WPF_GUI show_O2Wiki(this WPF_GUI wpf_Gui,string wikiPageToShow)
    	{
    		wpf_Gui.statusMessage("Showing O2Platform Wiki page: {0}",wikiPageToShow);
    		wpf_Gui.O2Browser.set_Text(wpf_Gui.Wiki_O2.html(wikiPageToShow));//"O2_Videos_on_YouTube"));    		
    		return wpf_Gui;
    	}
    	
    	public static WPF_GUI start_Process(this WPF_GUI wpf_Gui,string processToStart)
    	{
    		wpf_Gui.statusMessage("Starting process: {0}",processToStart);
    		Processes.startProcess(processToStart);
    		return wpf_Gui;
    	}
    	
    	public static WPF_GUI showOffineMessage (this WPF_GUI wpf_Gui,string message)
    	{
    		wpf_Gui.show_O2Browser();
    		return wpf_Gui.showOffineMessage(wpf_Gui.O2Browser,message);
    	}
    	    	    	    	
    	public static WPF_GUI showOffineMessage (this WPF_GUI wpf_Gui,WinForms.WebBrowser browser, string message)
    	{
			return wpf_Gui.showMessage(browser,"You are offline at the moment", message);
    	}    	
    	
    	public static WPF_GUI showMessage(this WPF_GUI wpf_Gui, string title, string message)
    	{
    		return wpf_Gui.showMessage(title,message,"");
    	}
    	    	    	
    	public static WPF_GUI showMessage (this WPF_GUI wpf_Gui, string title, string message, string url)
    	{
    		wpf_Gui.show_O2Browser();
    		return  wpf_Gui.showMessage(wpf_Gui.O2Browser, title, message,url);
    	}
    	
    	public static WPF_GUI showMessage (this WPF_GUI wpf_Gui,WinForms.WebBrowser browser, string title, string message)
    	{
    		return wpf_Gui.showMessage(title, message,"");
    	}
    	
    	public static WPF_GUI showMessage (this WPF_GUI wpf_Gui,WinForms.WebBrowser browser, string title, string message, string url)
    	{
    		O2Thread.mtaThread(
    			()=>{
			    		var htmlMessage = ("<html><body cellspacing=\"0\" cellpadding=\"0\"><font face=Arial><center>".line() +     						   
			    						   "   <h2>{1}</h2>".line() + 
			    						   "   <h3>{2}</h3>".line() + 	
			    						   "   <img src=\"{0}\"/>" + 
			    						   "</center>".line() + 
			    						  ((url).valid() 
			    						  		? "<div style=\"position:absolute; bottom:0px;width:100%;font-size:xx-small;\"><center>showing help page: <a href=\"{3}\"target=blank>{3}</a></center></div>".line() + 
			    						  		  "<iframe src =\"{3}\" style=\"position:absolute; bottom:15px; height=70%; width:100%;\"/>".line()
			    						  		  
			    						  		: "") + 
			    						   "</font></body></html>")
			    						   .format("O2Logo_Small.gif".local(), title, message, url);
						browser.set_Text(htmlMessage);
					});
			return wpf_Gui;
    	}
    }
    
    
    /*public static class WPF_GUI_ExtensionMethods_Items
    {
    	
    }*/
    
    public class WPF_GUI_Section
    {
    	public WPF_GUI Wpf_Gui { get; set;}
    	public WinForms.Control WinFormsControl { get; set;}
    	public Func<WinForms.Control> WinFormsCtor { get; set;}
    	public string Name { get; set;}
    	public string IntroText { get; set;}
    	//public WrapPanel ContentPanel { get; set;}
    	public Panel ContentPanel { get; set;}
    	public OutlookSection SectionInGui { get; set;}    	
    	
    	public WPF_GUI_Section (string name, string introText) : this (name,introText,null)
    	{    		
    	}
    	
    	public WPF_GUI_Section (string name, string introText, Func<WinForms.Control> winFormsCtor)
    	{
    		Name = name;    	 	
    	 	IntroText = introText;
    	 	WinFormsCtor = winFormsCtor;
    	}
    	
    	  
    	public List<Button> Links 
		{
			get{				
					return this.links();
				}
		}		  
    }
    
    
    public static class WPF_GUI_Section_ExtensionMethods
    {
    	public static WPF_GUI_Section add_Section(this List<WPF_GUI_Section> sections, string name)
    	{
    		return sections.add_Section(name,"");
    	}
   
    	public static WPF_GUI_Section add_Section(this List<WPF_GUI_Section> sections, string name, string introText)
    	{
			var newSection = new WPF_GUI_Section(name, introText);
			sections.Add(newSection);
			return newSection;
    	}

		public static WPF_GUI_Section add_WrapPanel(this WPF_GUI_Section section)
    	{
			section.ContentPanel = section.ContentPanel.add_WrapPanel();
			return section;
    	}
    	
    	public static WPF_GUI_Section add_TextBlock(this WPF_GUI_Section section, string text)
    	{
			var textBlock = section.ContentPanel.add_TextBlock();
			textBlock.set_Text_Wpf(text);
			return section;
    	}    	    						

		public static WPF_GUI_Section add_Upgrade_Link(this WPF_GUI_Section section, string latestVersion, string upgradeLink)
		{							
			if (PublicDI.config.CurrentExecutableDirectory.contains("OWASP O2 Platform") &&
    			PublicDI.config.CurrentExecutableDirectory.contains(latestVersion).isFalse())
    		{
    			section.add_Label("There is an UPGRADE available",true);    			
    			section.add_Link_Web("download version: {0}".format(latestVersion), upgradeLink);    			
    		}
    		return section;
		}
		
		public static WPF_GUI_Section add_Label(this WPF_GUI_Section section, string labelText)
		{
			return section.add_Label(labelText,false);
		}
		
		public static WPF_GUI_Section add_Label(this WPF_GUI_Section section, string labelText, bool bold)
		{				
			section.ContentPanel.add_Label_Wpf(labelText, bold);			
			return section;
		}
		
		public static WPF_GUI_Section add_Link(this WPF_GUI_Section section, string linkText, Action<WinForms.Panel> onClickCtor)
		{			
			Action<WinForms.Panel> pinnedCtor = onClickCtor;
			WinForms.Panel pinnedPanel = null;
			return (WPF_GUI_Section)section.SectionInGui.wpfInvoke(
				()=>{
						section.ContentPanel
							   .add_Xaml_Link(linkText,"20 0 0 10",
									()=>{
											if (pinnedPanel ==null)
											{
												pinnedPanel  = section.Wpf_Gui.WinFormPanel.add_Panel();
												pinnedCtor(pinnedPanel);
											}
											section.Wpf_Gui.WinFormPanel.clear();
											section.Wpf_Gui.WinFormPanel.add_Control(pinnedPanel);	
										});
						return section;
					});								
		}
		
		public static WPF_GUI_Section add_Link(this WPF_GUI_Section section, string linkText, Action onClick)
		{			
			return (WPF_GUI_Section)section.SectionInGui.wpfInvoke(
				()=>{
						section.ContentPanel.add_Xaml_Link(linkText,"20 0 0 10", onClick);						
						return section;
					});								
		}
		
		public static WPF_GUI_Section add_Link_Web(this WPF_GUI_Section section, string linkText, string linkUrl)
		{			
			return section.add_Link(linkText, ()=> section.Wpf_Gui.show_Url(linkUrl));						
		}
		
		public static WPF_GUI_Section add_Link_YouTube(this WPF_GUI_Section section, string linkText, string youTubeVideoId)
		{			
			return section.add_Link(linkText, ()=> section.Wpf_Gui.show_YouTubeVideo(youTubeVideoId));						
		}
		 
		public static WPF_GUI_Section add_Link_O2Wiki(this WPF_GUI_Section section, string linkText, string wikiPageToShow)
		{			
			return section.add_Link(linkText, ()=> section.Wpf_Gui.show_O2Wiki(wikiPageToShow));						
		}
		
		public static WPF_GUI_Section add_Link_Process(this WPF_GUI_Section section, string linkText, string processToStart)
		{			
			return section.add_Link(linkText, ()=> section.Wpf_Gui.start_Process(processToStart));						
		}
		
		public static WPF_GUI_Section add_Link_O2Script(this WPF_GUI_Section section, string linkText, string o2ScriptToExecute)
		{			
			return section.add_Link(linkText,
				()=> {										
						try
						{
							section.Wpf_Gui.show_O2Browser();
							var scriptPath = o2ScriptToExecute.local();
							if (scriptPath.fileExists())
							{
								section.Wpf_Gui.showMessage("Executing script: {0}".format(o2ScriptToExecute), "");
								section.Wpf_Gui.setExecuteScriptsEnvironment();
								section.Wpf_Gui.ExecuteScripts.loadFile(scriptPath);
							}
							else
								section.Wpf_Gui.showMessage("Provided script not found: {0}".format(o2ScriptToExecute), "");
						}
						catch(Exception ex)
						{
							ex.log("in WPF_GUI_Section.add_Link_O2Script");
						}
					 });
		}
				
				
		public static List<Button> links(this WPF_GUI_Section section)
		{
			return section.ContentPanel.controls_Wpf<Button>();
		}
    }
}
