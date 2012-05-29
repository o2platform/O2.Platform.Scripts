// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using CefSharp;
using CefSharp.WinForms;

//O2File:CefSharp.cs

//O2Ref:CefSharp\CefSharp-0.11-bin\CefSharp.WinForms.dll
//O2Ref:CefSharp\CefSharp-0.11-bin\CefSharp.dll

namespace O2.XRules.Database.APIs
{
	public class API_Chrome_Test
	{
		public void launchChrome()
		{
			"Chrome Browser test".popupWindow()
								 .add_Chrome()
								 .Load("http://news.bbc.co.uk");
		}
	}
	
    public class API_Chrome
    {    
    	public string CefSharp_Download { get; set;}
    	
    	public API_Chrome()
    	{
    		new CefSharp(); // will download CefSharp on first execution    		
    	}
    }
    
    public static class API_Chrome_WinForms
   	{
   		public static WebView add_Chrome(this Control control)
   		{
   			return  (WebView)control.invokeOnThread(
						()=>{
								var autoResetEvent = new AutoResetEvent(false);
								var webView = new WebView("http://about:blank",  new BrowserSettings());			
								webView.fill();
								control.Controls.Add(webView);
								webView.PropertyChanged+=  (browserCore, eventArgs)=>
									{
										if (eventArgs.PropertyName == "IsBrowserInitialized")
											autoResetEvent.Set();
								  	};			
								autoResetEvent.WaitOne(2000);			
								return webView;
							});
   		}
   		
   		public static WebView add_NavigationBar(this WebView webView)
    	{
    		webView.insert_Above(20)
				   .add_TextBox("Url:","")
				   .onEnter((text)=> webView.open_ASync(text));
			//webView.onNavigate((url)=> urlTextBox.set_Text(url));
			return webView;
    	}
    	
    	public static WebView open(this WebView webView, string url)
    	{
    		webView.Load(url);
    		return webView;
    	}
    	
    	public static WebView open_ASync(this WebView webView, string url)
    	{
    		O2Thread.mtaThread(
    			()=>{
    					webView.Load(url);
    				});
    		return webView;
    	}
   	}
}