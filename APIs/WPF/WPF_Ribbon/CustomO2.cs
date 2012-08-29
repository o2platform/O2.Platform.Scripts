// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Windows;
using WinForms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Threading;
using O2.Kernel;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Kernel.ExtensionMethods;
//using O2.API.Visualization.ExtensionMethods;
using Microsoft.Windows.Controls.Ribbon;
using O2.XRules.Database.APIs;
using O2.External.SharpDevelop.ExtensionMethods;

//O2File:WPF_Ribbon.cs

//O2Ref:RibbonControlsLibrary.dll
//O2Ref:System.Xaml.dll
//O2Ref:PresentationCore.dll
//O2Ref:PresentationFramework.dll
//O2Ref:WindowsBase.dll

namespace O2.XRules.Database.Utils
{
	public class CustomO2
	{
		public static WPF_Ribbon create(string title)
		{
			return create(title, 800,263);
		}
		
		public static WPF_Ribbon create(string title, int width, int height)
		{				
			var formTitle = "{0} (Custom O2)".format(title);		
			var panel = O2Gui.open<WinForms.Panel>(formTitle, width, height);       
			return create(panel, title);
		}
		
		public static WPF_Ribbon create(WinForms.Control control, string title)
		{
			var ribbon = control.add_Ribbon_WithLogViewer(title);       
			ribbon.title(title);    
			return ribbon;
		}
	}
	
	//number of template Ribbon Tabs and Groups
	public static class CustomO2_ExtensionMethods_Tabs
	{
		public static  WPF_Ribbon add_Tab_BrowserAutomation(this WPF_Ribbon ribbon)
		{
			var browserAutomation = ribbon.add_Tab("Browser Automation");   
			browserAutomation.add_RibbonGroup("IE Script development environments") 				 
							 .add_RibbonButton_Script("IE Automation","ascx_IE_ScriptExecution.cs")
							 .add_RibbonButton_H2Script("IE Automation Development","IE Automation using WatiN.h2")
							 .add_RibbonButton_H2Script("WatiN Recorder","WatiN - Open 'Test Recorder' in new process.h2");
			browserAutomation.add_RibbonGroup("Javascript")
							 .add_RibbonButton_H2Script("XSS PoC Builder","Web - XSS PoC Builder.h2") 
							 .add_RibbonButton_H2Script("Javascript AST Viewer","Web - Javascript AST Viewer.h2")	  
							 .add_RibbonButton_H2Script("JavaScript Stats Viewer","Web - JavaScript Stats Viewer.h2");  
				  
			browserAutomation.add_RibbonGroup("Html") 
							 .add_RibbonButton_H2Script("Html Tag Viewer","ascx_HtmlTagViewer.cs")
							 .add_RibbonButton_H2Script("View WebPage details","ascx_View_WebPage_Details.cs.o2")
							 .add_RibbonButton_H2Script("Quick HtmlCode Viewer","Quick HtmlCode Viewer.h2")
							 .add_RibbonButton_H2Script("Simple Html Viewer","Util - Simple Html Viewer.h2")
							 .add_RibbonButton_H2Script("Html Editor","ascx_Html_Editor.cs.o2");      
			return ribbon;		
		}		
		
		public static  WPF_Ribbon add_Tab_MiscTools(this WPF_Ribbon ribbon)
		{
			var miscTools = ribbon.Ribbon.add_RibbonTab("Misc Tools");		 

			miscTools.add_RibbonGroup("Media Tools")
					 .add_RibbonButton_H2Script("open ScreenShot tool (Cropper)","Util - Show ScreenShot Tool.h2")
					 .add_Button("save Image From Clipboard (to temp file)",()=> 0.saveImageFromClipboard())
					 .add_Button("save Image From Clipboard (to user's location)",()=> "".saveImageFromClipboardToFile());		 
					 	     
			miscTools.add_RibbonGroup("Media Tools")
					 .add_RibbonButton_H2Script("Image Editor", "Util - DiagramDesigner Editor.h2")	  	  
					 .add_RibbonButton_H2Script("Movie Creator","Util - Movie Creator (Simple).h2");
				     
			miscTools.add_RibbonGroup("Files Utils") 		 		
					 .add_RibbonButton_H2Script("Map Files by Extension","Util - File Mapping (by extension).h2")	  
					 .add_RibbonButton_H2Script("Quick File Search","Util - Quick File Search.h2")	  
					 .add_RibbonButton_H2Script("Quick File Viewer","Util - Quick File Viewer.h2")
					 .add_RibbonButton_H2Script("Simple Text Editor","Util - Simple Text Editor.h2")					 
					 .add_RibbonButton_H2Script("Search Engine","Search Engine Tool.h2");					 					 
			miscTools.add_RibbonGroup("O2 Utils") 		 				 
					 .add_Script("Execute Scripts","Util - Execute O2 Scripts.h2")
					 .add_RibbonButton_Script("Quick development GUI","ascx_Quick_Development_GUI.cs.o2")
					 .add_RibbonButton_Script("IE Automation","ascx_IE_ScriptExecution.cs")
					 .add_RibbonButton_H2Script("CSharp String Encoder","CSharp_String_Encoder.h2");		 
			miscTools.add_RibbonGroup("Windows Processes and Services")
					 .add_Script("Stop Processes","ascx_Processes_Stop.cs.o2")
					 .add_Script("View Running Process Details", "ascx_Running_Processes_Details.cs.o2")
					 .add_Script("Stop Services","ascx_Services_Stop.cs.o2");
			var currentScript = PublicDI.CurrentScript; 
			miscTools.add_RibbonGroup("This Custom O2")		 	
					.add_RibbonButton("Edit this Custom O2 Script", 
						() => O2Gui.open<System.Windows.Forms.Panel>("Custom O2",800,400)
								   .add_SourceCodeEditor()
								   .open(currentScript))				    
					.add_RibbonButton_H2Script("Open a Log Viewer window","Util - LogViewer.h2");
			return ribbon;
		}
	}
	
	public static class CustomO2_ExtensionMethods_Groups
	{
		public static WPF_Ribbon add_Group_developmentGuis(this WPF_Ribbon ribbon, string tabName)
		{
			ribbon.tab(tabName).add_Group_developmentGuis();
			return ribbon;
		}
		
		public static RibbonTab add_Group_developmentGuis(this RibbonTab tab)
		{		
			if (tab.notNull())
				tab.add_RibbonGroup("Development GUIs")
						//.add_RibbonButton_Script("Simple Script Editor", show.S "ascx_Simple_Script_Editor.cs.o2")
						.add_RibbonButton("Quick development GUI", ()=> open.scriptEditor() )//"ascx_Quick_Development_GUI.cs.o2")			
						.add_RibbonButton("O2 Development Environment", ()=> open.devEnvironment() )//"Util - O2 Development Environment.h2")					
						.add_RibbonButton_H2Script("Source Code Viewer","Util - SourceCodeViewer.h2")
						.add_RibbonButton_H2Script("Source Code Editor","Util - SourceCodeEditor.h2")
						.add_RibbonButton("O2 Object Model", () => open.o2ObjectModel());
			return tab;
		}
	}
}