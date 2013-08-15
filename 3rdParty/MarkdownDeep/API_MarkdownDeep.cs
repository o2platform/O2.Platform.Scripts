// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms; 
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.WinForms;
using MarkdownDeep;
//O2Ref:markdownDeep/bin/markdownDeep.dll

//Installer:MarkdownDeep_Installer.cs!markdownDeep/bin/markdownDeep.dll

namespace FluentSharp.For_MarkdownDeep
{	
	public class API_MarkdownSharp
	{						
		public List<Action> 	AfterTransform 			{ get; set; } 		
		public string 			Text 					{ get; set; }
		public string 			LastText_Transformed 	{ get; set; }
		public WebBrowser 		Browser					{ get; set; }
		public TextBox 			TextArea				{ get; set; }
		public Markdown 		Markdown				{ get; set; }
		
		public API_MarkdownSharp() : this("")
		{
			
		}
		
		public API_MarkdownSharp(string text)
		{
			AfterTransform  = new List<Action>();
			Text 			= text;	
			Markdown 		= new Markdown()
				{
				 	SafeMode = true
				};
		}
	}
	
	

	public static class API_MarkdownSharp_ExtensionMethods
	{
		public static API_MarkdownSharp transform(this API_MarkdownSharp markdownApi, string text)
		{
			markdownApi.Text = text;
			return markdownApi.transform();
		}
		public static API_MarkdownSharp transform(this API_MarkdownSharp markdownApi)
		{
			markdownApi.LastText_Transformed = markdownApi.Markdown.Transform(markdownApi.Text);
			markdownApi.AfterTransform.invoke();		
			return markdownApi;
		}
		public static API_MarkdownSharp showIn_Browser(this API_MarkdownSharp markdownApi)
		{
			if (markdownApi.Browser.isNull())
				markdownApi.Browser ="Markdown Transformation".popupWindow().add_WebBrowser();
			//var browser = open.webBrowser();			
			O2Thread.mtaThread(()=>markdownApi.Browser.html(markdownApi.LastText_Transformed));
			return markdownApi;
		}
		
		public static API_MarkdownSharp showIn_TextArea(this API_MarkdownSharp markdownApi)
		{
			if (markdownApi.TextArea.isNull())
				markdownApi.TextArea ="Markdown Transformation".popupWindow().add_TextArea();
			//var browser = open.webBrowser();			
			markdownApi.TextArea.set_Text(markdownApi.LastText_Transformed);
			return markdownApi;
		}
		
		public static API_MarkdownSharp syncWith_Browser(this API_MarkdownSharp markdownApi, WebBrowser browser)
		{
			markdownApi.Browser = browser;
			markdownApi.AfterTransform.add(()=>markdownApi.showIn_Browser());
			return markdownApi;
		}
		
		public static API_MarkdownSharp syncWith_TextArea(this API_MarkdownSharp markdownApi, TextBox textBox)
		{
			markdownApi.TextArea = textBox;
			markdownApi.AfterTransform.add(()=>markdownApi.showIn_TextArea());
			return markdownApi;
		}
		
		public static string text(this API_MarkdownSharp markdownApi)
		{
			return markdownApi.Text;
		}
		public static API_MarkdownSharp text(this API_MarkdownSharp markdownApi , string value)
		{
			markdownApi.Text = value;
			return markdownApi;
		}
		
		/*public static string html(this API_MarkdownSharp markdownApi)
		{			
			return markdownApi.transform().LastText_Transformed;
		}*/
		
		
	}
	public static class API_MarkdownSharp_ExtensionMethods_String
	{	
		/*public static string markdown_Transform(this string stringToTransform)
		{
			return new Markdown().Transform(stringToTransform);						
		}*/
		
		
		
	}
}
	