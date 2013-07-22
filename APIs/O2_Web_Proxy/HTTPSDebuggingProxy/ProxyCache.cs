using System;
using System.Net;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.WinForms;
using O2.XRules.Database.APIs;

//O2File:HttpData.cs


namespace HTTPProxyServer
{	
	public class ProxyCache
	{				
		public static List<RequestResponseData> 				Requests	 	{ get; set; }				
		public static Dictionary<string, RequestResponseData> 	RequestCache 	{ get; set; }
		
		public ProxyCache_Brain_DefaultMode		 		ProxyCache_Brain 	{ get; set; }
		public bool 									CacheEnabled 	 	{ get; set; }			
		
		public Action<string, RequestResponseData>		ProxyCacheLogEntry	{ get; set; }				
		public Func<string, RequestResponseData,bool>	ShowItemInLog		{ get; set; }				
		
		static ProxyCache()
		{
			Requests = new List<RequestResponseData>();
			RequestCache = new Dictionary<string, RequestResponseData>();			
		}	
		
		public ProxyCache()
		{
			ProxyCache_Brain = new ProxyCache_Brain_DefaultMode(this);
			CacheEnabled = false;
			ProxyCacheLogEntry = (line, requestResponseData) => "[ProxyCache] {0}".info();
			ShowItemInLog =  (line, requestResponseData) => true;
		}
	}
	
	public class ProxyCache_Brain_DefaultMode
	{
		public ProxyCache proxyCache;
		
		public ProxyCache_Brain_DefaultMode(ProxyCache _proxyCache)
		{
			this.proxyCache = _proxyCache;
		}
		
		public virtual string cacheKey(RequestResponseData requestResponseData)
		{
			return cacheKey(requestResponseData.WebRequest, requestResponseData.Request_PostBytes);
		}
		
		public virtual string cacheKey(HttpWebRequest webRequest, byte[] requestPostBytes)
		{
			//var cacheKey = "{0}: {1}".format(webRequest.Method, webRequest.RequestUri.pathNoQuery());
			var cacheKey = "{0}: {1}".format(webRequest.Method, webRequest.RequestUri.str());
			if (requestPostBytes.notNull() && requestPostBytes.Length > 0)
				//cacheKey = "{0} PostBytesHash: {1}".format(cacheKey, requestPostBytes.ascii().hash());				
				cacheKey = "{0}      [{1}]".format(cacheKey, requestPostBytes.ascii().hash());				
			return cacheKey;
		}
		
		public virtual bool shouldCache(RequestResponseData requestRessponseData)
		{
			return true;
		}
			
		public virtual RequestResponseData getMapping(string cacheKey)
		{			
			if (ProxyCache.RequestCache.hasKey(cacheKey))							
				return ProxyCache.RequestCache[cacheKey];
			return null;
		}
		
		public virtual bool hasMapping(string cacheKey)
		{
			return 	getMapping(cacheKey).notNull();
		}
	}
	
	
	public static class ProxyCache_ExtensionMethods
	{
		public static bool enabled(this ProxyCache proxyCache)
		{
			return proxyCache.CacheEnabled;
		}
		
		public static ProxyCache enabled(this ProxyCache proxyCache, bool value)
		{
			proxyCache.CacheEnabled = value;
			return proxyCache;
		}
		
		public static Dictionary<string, RequestResponseData> requestMappings(this ProxyCache proxyCache)
		{
			return ProxyCache.RequestCache;
		}
		
		public static RequestResponseData getMapping(this ProxyCache proxyCache, HttpWebRequest webRequest, byte[] requestPostBytes)
		{
			var cacheKey = proxyCache.ProxyCache_Brain.cacheKey(webRequest,requestPostBytes);			
			var cacheObject = proxyCache.ProxyCache_Brain.getMapping(cacheKey);			
			proxyCache.ProxyCacheLogEntry(cacheObject.isNull()
												? "{0}	         (not in cache)".format(cacheKey)
												: "{0}           (from cache)".format(cacheKey) ,
										 cacheObject);			
			if (cacheObject.notNull())	
				cacheObject.CacheHits++;			
			return cacheObject;
		}
		
		public static bool hasMapping(this ProxyCache proxyCache, HttpWebRequest webRequest, byte[] requestPostBytes)
		{
			var cacheKey = proxyCache.ProxyCache_Brain.cacheKey(webRequest,requestPostBytes);
			return proxyCache.ProxyCache_Brain.hasMapping(cacheKey);
		}
		
		/*public static byte[] response_Bytes(this ProxyCache proxyCache, string key)
		{
			if (proxyCache.hasMapping(key))
				return proxyCache.requestMappings().value(key).Response_Bytes;
			return null;
		}
		
		public static string response_String(this ProxyCache proxyCache, string key)
		{
			if (proxyCache.hasMapping(key))
				return proxyCache.requestMappings().value(key).Response_String;
			return null;
		}*/
		
		public static ProxyCache add_ToCache(this ProxyCache proxyCache, RequestResponseData requestResponseData)
		{			
			var requestPostBytes = requestResponseData.Request_PostBytes;			
			var cacheKey = proxyCache.cacheKey(requestResponseData);
			
			ProxyCache.Requests.Add(requestResponseData);
			ProxyCache.RequestCache.add(cacheKey,requestResponseData);
			return proxyCache;
		}
		
		
		public static string cacheKey(this ProxyCache proxyCache, RequestResponseData requestResponseData)
		{
			return proxyCache.ProxyCache_Brain.cacheKey(requestResponseData);
		}
		
	}
	
	public static class RequestResponseData_ExtensionMethods
	{
		public static RequestResponseData add(	this  ProxyCache proxyCache, 
												HttpWebRequest webRequest,
												HttpWebResponse webResponse,
												byte[] requestPostBytes,
												byte[] responseBytes,
												string responseString )
{			
			var requestResponseData = new RequestResponseData()
            									{
            										WebRequest = webRequest, 
													WebResponse = webResponse, 													
													Response_Bytes = responseBytes,
													Response_String = responseString 
            									};	
			if (requestPostBytes.notNull())
        	{
        		requestResponseData.Request_PostBytes = requestPostBytes; 
				requestResponseData.Request_PostString = requestPostBytes.ascii();
			}
			proxyCache.add_ToCache(requestResponseData);
			return requestResponseData;
		}
	}
	
	
	public static class ProxyCache_ExtensionMethods_GuiHelpers
	{
		public static T add_ProxyCacheLogViewer<T>(this T control, ProxyCache proxyCache)
			where T : Control
		{
			proxyCache.add_ProxyCacheLogViewer(control);
			return control;
		}				
		
		public static ProxyCache add_ProxyCacheLogViewer(this ProxyCache proxyCache, Control control)
		{
			var cacheLog = control.add_TreeView()
								  .hideSelection()
								  .onDoubleClick<RequestResponseData>((requestResponseData) => requestResponseData.details());
								  
			proxyCache.ProxyCacheLogEntry = 
				(logLine, requestResponseData)
					=> {
						//cacheLog.insert_TreeNode(logLine, requestResponseData, 0).nodes()[0]
							if (proxyCache.ShowItemInLog(logLine, requestResponseData))
							{								
								if (requestResponseData.notNull())
										logLine = "{0}  size: {1}".format(logLine, requestResponseData.Response_Bytes.notNull()
																					? requestResponseData.Response_Bytes.Length
																					: -1);								
								cacheLog.add_Node(logLine, requestResponseData)
								    .color(requestResponseData.isNull()
								   			? Color.Gray
								   			: Color.DarkGreen)
								   	.selected();
							}
						};
			return proxyCache;
		}
		
		public static Control insert_Below_ProxyCacheActionsPanel(this Control control, ProxyCache proxyCache)			
		{
			return proxyCache.add_ProxyCacheActionsPanel(control.insert_Below(60,""));			
		}
		
		public static Control insert_Above_ProxyCacheActionsPanel(this Control control, ProxyCache proxyCache)			
		{
			return proxyCache.add_ProxyCacheActionsPanel(control.insert_Above(60,"actions"));			
		}
		
		public static Control add_ProxyCacheActionsPanel(this ProxyCache proxyCache, Control actionsPanel)
		{
			actionsPanel.add_Label("Settings") 
					    .append_CheckBox("cache Enabled", (value)=> proxyCache.enabled(value)).@checked(proxyCache.enabled())				   
						.append_Below_Link("show Cache entries",()=> ProxyCache.RequestCache.details())
						.append_Link("clear Cache",()=> ProxyCache.RequestCache.Clear())					
						.append_Link("open a WebBrowser", ()=> "ProxyChache - test WebBrowser".popupWindow()
																							  .add_WebBrowser_Control()
																							  .add_NavigationBar()
																							  .open("http://www.google.com"));
			return actionsPanel;
		}
	}
}



/*


//based on the code from http://www.codeproject.com/KB/IP/HTTPSDebuggingProxy.aspx
//originally coded by @matt_mcknight  

// see O2_Web_Proxy.cs API for a way to consume this Proxy from O2

    public static class ProxyCache
    {
        public static Hashtable _cache  	{ get; set;} 
        public static Int32 _hits			{ get; set;} 
        
        private static Object _cacheLockObj = new object();
        private static Object _statsLockObj = new object();
        
        
        static ProxyCache()
        {
        	_cache = new Hashtable();
        }

        public static CacheEntry GetData(HttpWebRequest request)
        {
            CacheKey key = new CacheKey(request.RequestUri.AbsoluteUri, request.UserAgent);
            if (_cache[key] != null)
            {
                CacheEntry entry = (CacheEntry)_cache[key];
                if (entry.FlagRemove || (entry.Expires.HasValue && entry.Expires < DateTime.Now))
                {
                    //don't remove it here, just flag
                    entry.FlagRemove = true;
                    return null;
                }
                Monitor.Enter(_statsLockObj);
                _hits++;
                Monitor.Exit(_statsLockObj);
                return entry;
            }
            return null;
        }

        public static CacheEntry MakeEntry(HttpWebRequest request, HttpWebResponse response,List<Tuple<String,String>> headers, DateTime? expires)
        {
            CacheEntry newEntry = new CacheEntry();
            newEntry.Expires = expires;
            newEntry.DateStored = DateTime.Now;
            newEntry.Headers = headers;
            newEntry.Key = new CacheKey(request.RequestUri.AbsoluteUri, request.UserAgent);
            newEntry.StatusCode = response.StatusCode;
            newEntry.StatusDescription = response.StatusDescription;
            if (response.ContentLength > 0)
                newEntry.ResponseBytes = new Byte[response.ContentLength];
            return newEntry;
        }

        public static void AddData(CacheEntry entry)
        {

            Monitor.Enter(_cacheLockObj);
            if (!_cache.Contains(entry.Key))
                _cache.Add(entry.Key, entry);
            Monitor.Exit(_cacheLockObj);


        }

        public static Boolean CanCache(WebHeaderCollection headers, ref DateTime? expires)
        {

            foreach (String s in headers.AllKeys)
            {
                String value = headers[s].ToLower();
                switch (s.ToLower())
                {
                    case "cache-control":
                        if (value.Contains("max-age"))
                        {
                            int seconds;
                            if (int.TryParse(value, out seconds))
                            {
                                if (seconds == 0)
                                    return false;
                                DateTime d = DateTime.Now.AddSeconds(seconds);
                                if (!expires.HasValue || expires.Value < d)
                                    expires = d;

                            }
                        }

                        if (value.Contains("private") || value.Contains("no-cache"))
                            return false;
                        else if (value.Contains("public") || value.Contains("no-store"))
                            return true;

                        break;

                    case "pragma":

                        if (value == "no-cache")
                            return false;

                        break;
                    case "expires":
                        DateTime dExpire;
                        if (DateTime.TryParse(value, out dExpire))
                        {
                            if (!expires.HasValue || expires.Value < dExpire)
                                expires = dExpire;
                        }
                        break;
                }
            }
            return true;
        }

        public static void CacheMaintenance()
        {        
        	Console.WriteLine("Log is disabled");
        	"Cache is disabled...".debug();
        	return;
            try
            {
                while (true)
                {                	
                    Thread.Sleep(30000);
                    List<CacheKey> keysToRemove = new List<CacheKey>();
                    foreach (CacheKey key in _cache.Keys)
                    {
                        CacheEntry entry = (CacheEntry)_cache[key];
                        if (entry.FlagRemove || entry.Expires < DateTime.Now)
                            keysToRemove.Add(key);
                    }

                    foreach (CacheKey key in keysToRemove)
                        _cache.Remove(key);

                    Console.WriteLine(String.Format("....Cache maintenance complete.  Number of items stored={0} Number of cache hits={1} ", _cache.Count, _hits));
                }
            }
            catch (ThreadAbortException) { }
        }

    }
}
*/    

