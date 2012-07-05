// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
using HTTPProxyServer;

//O2File:O2_Web_Proxy_ExtensionMethods_GUI_Helpers.cs

namespace O2.XRules.Database.APIs
{
	public class API_WebProxy : O2_Web_Proxy
	{
		public List<RequestResponseData> Requests { get { return this.requests(); } }
		public ProxyCache ProxyCache { get { return this.proxyCache(); }  } 
	}
	
	public static class API_WebProxy_ExtensionMethods_Misc
	{
		public static ProxyCache proxyCache(this O2_Web_Proxy o2WebProxy)
		{
			return ProxyServer.proxyCache;
		}
	}
	
	public static class API_WebProxy_ExtensionMethods_CreationView_Helpers
	{
		public static Panel show_WebProxyGui(this O2_Web_Proxy o2WebProxy)
		{
			var topPanel = "O2 Web Proxy".popupWindow(300,400);
			o2WebProxy.createGui_Proxy_SimpleView(topPanel);
			return topPanel;
		}
		
		public static O2_Web_Proxy add_WebProxy(this Control panel)
		{			
			var o2WebProxy = new O2_Web_Proxy().createGui_Proxy_SimpleView(panel);
			o2WebProxy.startWebProxy();			
			return o2WebProxy;
		}
		
		public static O2_Web_Proxy insert_Right_WebProxy(this Control panel)
		{
			return panel.insert_Right("Web Proxy").add_WebProxy();
		}
		
		public static O2_Web_Proxy show_WebProxy_with_Browser(this string url)
		{
			return "Web Proxy with: {0}".format(url).popupWindow().add_WebProxy_with_Browser(url);
		}
		
		public static O2_Web_Proxy add_WebProxy_with_Browser(this Control panel)
		{
			return panel.add_WebProxy_with_Browser("about:blank");
		}
		
		public static O2_Web_Proxy add_WebProxy_with_Browser(this Control panel, string url)
		{
			var topPanel = panel.clear().add_Panel();
			var o2WebProxy = topPanel.insert_Right_WebProxy();				   
			var browser = topPanel.add_WebBrowser_Control().add_NavigationBar();			
			browser.open(url);
			return o2WebProxy;
		}
		

		
		public static O2_Web_Proxy get_WebProxy(this string url)
		{
			var o2WebProxy = new O2_Web_Proxy();
			o2WebProxy.startWebProxy();
			url.html();
			return o2WebProxy;
		}
				
	}
	
	public static class API_WebProxy_TestWebsite
	{
		public static string google(this O2_Web_Proxy o2WebProxy)
		{
			return "http://www.google.com".get_Html();			
		}
	}
	
	public static class API_WebProxy_RequestsCache
	{
		public static List<RequestResponseData> requests(this O2_Web_Proxy o2WebProxy)
		{
			return o2WebProxy.Proxy.requests();
		}
		
		public static O2_Web_Proxy clearRequests(this O2_Web_Proxy o2WebProxy)
		{
			o2WebProxy.requests().Clear();
			return o2WebProxy;
		}
	}
	
}