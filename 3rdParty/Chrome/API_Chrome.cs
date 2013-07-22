// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Threading;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.WinForms;
using CefSharp;
using CefSharp.WinForms;  

//Installer:CefSharp_Installer.cs!CefSharp\CefSharp-1.19.0\CefSharp.WinForms.dll
//O2Ref:CefSharp\CefSharp-1.19.0\CefSharp.WinForms.dll
//O2Ref:CefSharp\CefSharp-1.19.0\CefSharp.dll 

//CLR_3.5

namespace O2.XRules.Database.APIs
{
	public class API_Chrome_Test
	{
		public void launchChrome()
		{
			API_Chrome_WinForms.add_Chrome("Chrome Browser test".popupWindow())
							   .Load("http://news.bbc.co.uk");
		}
	}
	
    public class API_Chrome
    {    
    	
    	public API_Chrome()
    	{
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
    	
    	public static object eval(this WebView webView, string scriptToEval)
    	{
    		try
    		{
    			return webView.EvaluateScript(scriptToEval);
    		}
    		catch(Exception ex)
    		{
    			ex.log();
    			return null;
    		}
    	}
    	
    	public static object alert(this WebView webView, string alertContents)
    	{
    		var alertCode = "alert({0});".format(alertContents);
    		return webView.EvaluateScript(alertCode);
    	}
    	
    	public static WebView inject_FirebugLite(this WebView webView)
    	{
    		var firebugLiteScript = "(function(F,i,r,e,b,u,g,L,I,T,E){if(F.getElementById(b))return;E=F[i+'NS']&&F.documentElement.namespaceURI;E=E?F[i+'NS'](E,'script'):F[i]('script');E[r]('id',b);E[r]('src',I+g+T);E[r](b,u);(F[e]('head')[0]||F[e]('body')[0]).appendChild(E);E=new Image;E[r]('src',I+L);})(document,'createElement','setAttribute','getElementsByTagName','FirebugLite','4','firebug-lite.js','releases/lite/latest/skin/xp/sprite.png','https://getfirebug.com/','#startOpened');";
			webView.eval(firebugLiteScript);  
			"[Injected FirebugLite]".info();
			return webView;
    	}
    	
    	public static WebView inject_JQuery(this WebView webView)
    	{
    		webView.eval("jquery-1.9.1.min.js".local().fileContents());
    		"[Injected jquery 1.9]".info();
    		return webView;
    	}
    	
    	public static bool is_JQuery_Installed(this WebView webView)
    	{
    		return webView.eval("$.toString();").str() == "function (e,t){return new b.fn.init(e,t,r)}";
    	}
    	
    	
   	}
}