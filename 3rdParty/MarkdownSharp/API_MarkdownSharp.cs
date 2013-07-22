// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms; 
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.WinForms;
using MarkdownSharp;

//O2Ref:MarkdownSharp.dll

namespace O2.XRules.Database.APIs
{	
	public class API_MarkdownSharp
	{						
		public List<Action> AfterTransform 			{ get; set; } 		
		public string 		Text 					{ get; set; }
		public string 		LastText_Transformed 	{ get; set; }
		public WebBrowser 	Browser					{ get; set; }
		public TextBox 		TextArea				{ get; set; }
		
		public API_MarkdownSharp() : this("")
		{
			
		}
		
		public API_MarkdownSharp(string text)
		{
			AfterTransform  = new List<Action>();
			Text = text;			
		}
	}
	
	

	public static class API_MarkdownSharp_ExtensionMethods
	{
		public static API_MarkdownSharp transform(this API_MarkdownSharp markdown, string text)
		{
			markdown.Text = text;
			return markdown.transform();
		}
		public static API_MarkdownSharp transform(this API_MarkdownSharp markdown)
		{
			markdown.LastText_Transformed = markdown.Text.markdown_Transform();
			markdown.AfterTransform.invoke();		
			return markdown;
		}
		public static API_MarkdownSharp showIn_Browser(this API_MarkdownSharp markdown)
		{
			if (markdown.Browser.isNull())
				markdown.Browser ="Markdown Transformation".popupWindow().add_WebBrowser();
			//var browser = open.webBrowser();			
			O2Thread.mtaThread(()=>markdown.Browser.html(markdown.LastText_Transformed));
			return markdown;
		}
		
		public static API_MarkdownSharp showIn_TextArea(this API_MarkdownSharp markdown)
		{
			if (markdown.TextArea.isNull())
				markdown.TextArea ="Markdown Transformation".popupWindow().add_TextArea();
			//var browser = open.webBrowser();			
			markdown.TextArea.set_Text(markdown.LastText_Transformed);
			return markdown;
		}
		
		public static API_MarkdownSharp syncWith_Browser(this API_MarkdownSharp markdown, WebBrowser browser)
		{
			markdown.Browser = browser;
			markdown.AfterTransform.add(()=>markdown.showIn_Browser());
			return markdown;
		}
		
		public static API_MarkdownSharp syncWith_TextArea(this API_MarkdownSharp markdown, TextBox textBox)
		{
			markdown.TextArea = textBox;
			markdown.AfterTransform.add(()=>markdown.showIn_TextArea());
			return markdown;
		}
		
		public static string text(this API_MarkdownSharp markdown)
		{
			return markdown.Text;
		}
		public static API_MarkdownSharp text(this API_MarkdownSharp markdown , string value)
		{
			markdown.Text = value;
			return markdown;
		}
		
		public static string html(this API_MarkdownSharp markdown)
		{			
			return markdown.transform().LastText_Transformed;
		}
		
		
	}
	public static class API_MarkdownSharp_ExtensionMethods_String
	{	
		public static string markdown_Transform(this string stringToTransform)
		{
			return new Markdown().Transform(stringToTransform);						
		}
		
		
		
	}
}
	