// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

//O2Tag:SkipGlobalCompilation

// the CefSharp.dll can not being used to compiled on 4.0 CLR

using System.Threading;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.WPF;
using FluentSharp.WinForms;
using CefSharp.Wpf;

//O2Ref:FluentSharp.WPF.dll

//O2Ref:CefSharp\CefSharp-1.19.0\CefSharp.Wpf.dll
//O2Ref:CefSharp\CefSharp-1.19.0\CefSharp.dll

//O2Ref:WindowsFormsIntegration.dll
//O2Ref:PresentationFramework.dll
//O2Ref:PresentationCore.dll
//O2Ref:WindowsBase.dll
//O2Ref:System.Xaml.dll

//You will need to execute CefSharp.cs first (to download the CefSharp files)

namespace O2.XRules.Database.APIs
{
	public class API_Chrome_WPF_Test
	{
		public void launchChrome()
		{
			"Chrome Browser test".popupWindow()
								 .add_Chrome_Wpf(true)
								 .Load("http://news.bbc.co.uk");
		}
	}
	
    public class API_Chrome_WPF
    {        	    	
    	public API_Chrome_WPF()
    	{    		
    	}
    }
    
    public static class API_Chrome_WPF_Extension_Methods
   	{   		
   		public static WebView add_Chrome_Wpf(this System.Windows.Forms.Control control, bool addNavigationBar = false)
   		{
   			var autoResetEvent = new AutoResetEvent(false);
   			var _webView = (WebView)control.invokeOnThread(
							()=>{									
									var webView = control.add_WPF_Control<WebView>();
									
									webView.PropertyChanged+=  (browserCore, eventArgs)=>
										{
											if (eventArgs.PropertyName == "IsBrowserInitialized")
												autoResetEvent.Set();
									  	};												
									return webView;
								});
			autoResetEvent.WaitOne(2000);		
			if (addNavigationBar)
			{
				control.insert_Above(20)
					   .add_TextBox("Url:","")
					   .onEnter((text)=> _webView.open_ASync(text.info()));
				//webView.onNavigate((url)=> urlTextBox.set_Text(url));
			}
			return _webView;
   		}
   		
   		public static WebView add_NavigationBar(this WebView webView)
    	{
    		//
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