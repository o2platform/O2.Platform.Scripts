//based on the code from http://www.codeproject.com/KB/IP/HTTPSDebuggingProxy.aspx
//originally coded by @matt_mcknight  

// see O2_Web_Proxy.cs API for a way to consume this Proxy from O2


using System;
using System.Net;
using System.Web;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace HTTPProxyServer
{	
    public class CacheKey
    {
        public String AbsoluteUri { get; set; }
        public String UserAgent { get; set; }

        public CacheKey(String requestUri, String userAgent)
        {
            AbsoluteUri = requestUri;
            UserAgent = userAgent;
        }

        public override bool Equals(object obj)
        {
            CacheKey key = obj as CacheKey;
            if (key != null)
                return (key.AbsoluteUri == AbsoluteUri && key.UserAgent == UserAgent);
            return false;
        }

        public override int GetHashCode()
        {
            String s = AbsoluteUri + UserAgent;
            return s.GetHashCode();
        }
    }
    
    public class CacheEntry
    {
        public CacheKey Key { get; set; }
        public DateTime? Expires { get; set; }
        public DateTime DateStored { get; set; }
        public Byte[] ResponseBytes { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public String StatusDescription { get; set; }
        public List<Tuple<String,String>> Headers { get; set; }
        public Boolean FlagRemove { get; set; }
    }
    
    
    //added to this code since the original version was compiled in .Net 4.0 which has equivalent classes
    public class Tuple<T>
    {
        public Tuple(T first)
        {
            Item1 = first;
        }

        public T Item1 { get; set; }
    }

    public class Tuple<T, T2> : Tuple<T>
    {
        public Tuple(T first, T2 second)
            : base(first)
        {
            Item2 = second;
        }

        public T2 Item2 { get; set; }
        
    }
}
