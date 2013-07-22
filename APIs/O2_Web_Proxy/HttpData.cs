using System;
using System.Net;
using System.Collections.Generic;
using FluentSharp.CoreLib;

namespace O2.XRules.Database.APIs 
{         
	public class RequestResponseData 
	{
		public HttpWebRequest  WebRequest 	 { get; set; }
		public HttpWebResponse WebResponse   { get; set; }				
		
		public String    	   				Request_Headers_Raw { get; set; }	
		public String    	   				Response_Headers_Raw { get; set; }			
		public List<Tuple<String,String>>	Response_Headers { get; set; }			
		
		public String    	   	Request_PostString { get; set; }			
		public String    	   	Response_String { get; set; }	
		
		public byte[]    	   	Request_PostBytes;
		public byte[]    	   	Response_Bytes;
		
		public int				CacheHits { get ; set;}		
		
		public override string ToString()
		{
			return "{0} : {1}".format(WebRequest.RequestUri, Response_Bytes.Length);
		}
		
		public string	Request_QueryString
		{
			get { return WebRequest.RequestUri.Query.firstChar("?") 
							? WebRequest.RequestUri.Query.removeFirstChar()
							: WebRequest.RequestUri.Query; }
			set {}
		}
		
		public Uri	Request_Uri
		{
			get { return WebRequest.RequestUri; } 
			set {}
		}
	}
			
		
/*	public class Tuple<T>
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
*/
	
}
	