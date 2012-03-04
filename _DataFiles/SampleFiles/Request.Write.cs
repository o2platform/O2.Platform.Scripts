using System;
using System.Web;

namespace O2.PoCs
{
	public class originalCode
	{
		//HttpContext Context = HttpContext.Current;
		HttpRequest Request = HttpContext.Current.Request;
		HttpResponse Response = HttpContext.Current.Response;
			
				
		public void xssVulnerability()
		{
			xssVulnerability(Request["payload"]);
		}
		
		public void xssVulnerability(string payload)
		{
			Response.Write(payload);
		}
		

		
	}
}