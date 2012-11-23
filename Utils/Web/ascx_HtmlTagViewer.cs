// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.External.SharpDevelop.Ascx;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.XRules.Database.Utils;
//O2File:HtmlAgilityPack_Extra_ExtensionMethods.cs
//O2File:HtmlAgilityPack_Extra_ExtensionMethods_SourceCodeViewer.cs
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll

namespace O2.XRules.Database.Utils
{
    public class ascx_HtmlTagViewer : Control
    {    
    
    	public TreeView HtmlTags_TreeView; 
    	public TextBox HtmlNodeFilter; 
    	public ascx_SourceCodeViewer HtmlCodeViewer;
    	public TextBox PageToOpen;
    	
    	public string HtmlCode { get; set; }
    	public bool ViewAsXml { get; set; }
    	public static string defaultPage = "http://www.google.com";
			
    	public static void launchGui()
    	{
    		O2Gui.open<ascx_HtmlTagViewer>("Control - Html Tag Viewer", 1000,400)
    			 .buildGui(true, true)    			 
    			 .show(defaultPage.uri());
    	}
    	
		public ascx_HtmlTagViewer()
		{
			HtmlCode = "";			
		}
		public ascx_HtmlTagViewer buildGui()
		{
			return buildGui(false,false);
		}
		
		public ascx_HtmlTagViewer buildGui(bool addLoadUrlTextBox, bool addHtmlCodeViewer)
		{
			//return (ascx_HtmlTagViewer)this.invokeOnThread(
			//	()=>{
			var TopPanel = this.add_Panel();
			HtmlTags_TreeView =  TopPanel.add_TreeView_for_HtmlTags()
									 	 .showSelection();			
			
			if (addHtmlCodeViewer)
			{
				HtmlCodeViewer = HtmlTags_TreeView.insert_Left<Panel>(TopPanel.width()/2).add_SourceCodeViewer(); 
				
				HtmlTags_TreeView.after_TagSelect_showIn_SouceCodeViewer(HtmlCodeViewer);
				
				var optionsPanel = HtmlCodeViewer.insert_Below<Panel>(50);
				optionsPanel.add_CheckBox("View as Xml",0,5, 
					(value)=>{
								ViewAsXml = value;
								reloadPage();
							 })
							 .append_Label("Search html code:")
							 .top(5)
							 .append_TextBox("")
							 .onTextChange((text)=> HtmlCodeViewer.editor().invoke("searchForTextInTextEditor_findNext",text))
							 .onEnter((text)=> HtmlCodeViewer.editor().invoke("searchForTextInTextEditor_findNext",text))
							 .align_Right(optionsPanel);
				
				optionsPanel.add_Link("Refresh tags",30,0,()=> show(HtmlCodeViewer.get_Text()));
			}
			
			if (addLoadUrlTextBox)	
			{
				PageToOpen = TopPanel.insert_Above<Panel>(20).add_TextBox().fill();
				var propertyGrid = HtmlTags_TreeView.insert_Right<Panel>(150).add_PropertyGrid();
				
				
				HtmlTags_TreeView.afterSelect<HtmlAgilityPack.HtmlNode>(
				  (htmlNode)=> propertyGrid.show(htmlNode));
				
				PageToOpen.onEnter(
							(text)=>{
										if (text.fileExists())
											show(text.fileContents());
										else 
											show(text.uri());
									}); 
			}																						
			
			HtmlNodeFilter = HtmlTags_TreeView.insert_Below_HtmlTagFilter((filter) => show(HtmlCode, filter) );
			
			return this;
		//});
		}
		
		public ascx_HtmlTagViewer reloadPage()
		{
			var url = PageToOpen.get_Text();
			if (url.fileExists())	
				show(url.fileContents());
			if (url.isUri())
				show(url.uri().getHtml());
			return this;
		}
		public ascx_HtmlTagViewer show(Uri uri)
		{	
			if (PageToOpen.notNull())
				PageToOpen.set_Text(uri.str());
			return show(uri.getHtml());
		}
		
		public ascx_HtmlTagViewer show(string htmlCode)
		{			
			HtmlCode = htmlCode;
			if (ViewAsXml)
				HtmlCode = htmlCode.htmlToXml();
				
			if (HtmlCodeViewer.notNull())
			{
				if (ViewAsXml)
					HtmlCodeViewer.set_Text(HtmlCode,".xml");									
				else
					HtmlCodeViewer.set_Text(HtmlCode,".xml"); 
			}
			return show(HtmlCode,HtmlNodeFilter.get_Text());
		}
		
		public ascx_HtmlTagViewer show(string htmlCode, string filter)
		{					
			htmlCode.showFilteredHtmlContentInTreeView(filter, HtmlTags_TreeView, HtmlNodeFilter);			
			return this;
		}		
    }
}
//O2File:HtmlAgilityPack_ExtensionMethods_SourceCodeViewer.cs