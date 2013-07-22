// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using FluentSharp.CoreLib;
using HTTPProxyServer;

//O2File:API_WinProxy.cs 
//O2File:ProxyServer.cs


namespace O2.XRules.Database.APIs
{
	
    public class O2_Web_Proxy
    {    
    	public string CertLocation { get; set; }
    	public object WafRule	   { get; set; }
    	public string IP	   { get; set; }
    	public int Port	   { get; set; }
    	
    	public int DEFAULT_PORT = 8081;
    	
    	public O2_Web_Proxy()
    	{
    		config();
    	}
    	
    	public O2_Web_Proxy(bool startWebProxy) : this()
    	{
    		if (startWebProxy)
    			this.startWebProxy();	
    	}
    	
    	
    	public ProxyServer Proxy 
    	{
    		get {
    				return ProxyServer.Server;
    			}
    	}
    	
    	public O2_Web_Proxy config()
    	{
    		CertLocation = @"cert.cer".local();
    		IP = "127.0.0.1";
    		Port = this.DEFAULT_PORT;    		
    		return this;
    	}    	    	
	}
	
	public static class O2_Web_Proxy_ExtensionMethods_ProxyActions
	{
		public static string proxyLocation(this O2_Web_Proxy o2WebProxy)
		{
			return "http://{0}:{1}".format(o2WebProxy.IP, o2WebProxy.Port);
		}
		
		public static O2_Web_Proxy extraLogging(this O2_Web_Proxy o2WebProxy)
		{
			return o2WebProxy.extraLogging(true);
		}
		
		public static O2_Web_Proxy extraLogging(this O2_Web_Proxy o2WebProxy, bool value)
		{
			ProxyServer.ExtraLogging = value;
			return o2WebProxy;
		}
	}
	
	public static class O2_Web_Proxy_ExtensionMethods_Utils
	{			
		public static O2_Web_Proxy setBrowserProxy(this O2_Web_Proxy o2WebProxy)
		{
			var proxyLocation = o2WebProxy.proxyLocation();
			"setting proxy location to: {0}".info(proxyLocation);
			API_WinProxy.SetProxy(proxyLocation);
			return o2WebProxy;
		}
		
		public static O2_Web_Proxy clearBrowserProxy(this O2_Web_Proxy o2WebProxy)
		{
			"remove proxy proxy settings (ie setting it to \"\"".info();
			API_WinProxy.SetProxy("");
			return o2WebProxy;
		}		
		
		public static bool startWebProxy(this O2_Web_Proxy o2WebProxy)
		{
			if (o2WebProxy.Proxy.ProxyStarted)
			{
				"There was already a proxy started so reusing exising proxy object and port".info();
				return true;
			}
			//if (o2WebProxy.Port == o2WebProxy.DEFAULT_PORT)				
				o2WebProxy.Port = o2WebProxy.DEFAULT_PORT + 1000.randomNumber();
			return o2WebProxy.startWebProxy(o2WebProxy.Port);
		}
		
		public static bool startWebProxy(this O2_Web_Proxy o2WebProxy, int port)
		{
			o2WebProxy.Port = port;
			ProxyServer.Server.ListeningPort = o2WebProxy.Port;
			o2WebProxy.setBrowserProxy();
			if (ProxyServer.Server.Start(o2WebProxy.CertLocation))
			{
				"Proxy started Ok on ip:port: {0}".info(o2WebProxy.proxyLocation());
				return true;
			}			
			"Proxy failed to start on ip:port: {0}".error(o2WebProxy.proxyLocation());		
			return false;
		}	
		
		public static O2_Web_Proxy stopWebProxy(this O2_Web_Proxy o2WebProxy)
		{ 
			try
			{
				o2WebProxy.clearBrowserProxy();
				"Stopping Web Proxy".info();
				ProxyServer.Server.Stop();
			}
			catch(Exception ex)
			{
				"[O2_Web_Proxy] stopWebProxy : {0}".error(ex.Message);
			}
			return o2WebProxy;
		}												
	}			
}

	