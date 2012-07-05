// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
  
namespace O2.XRules.Database.APIs
{
    public class WAF_Rule
    {    
		public bool LogCallbacks { get; set;}
		public Action<string> OnUrlRequest { get; set;}
		public Action<object> OnResponseReceived { get; set;}
		
		public WAF_Rule()
		{
			LogCallbacks = false;
		}
		
		public virtual string InterceptRemoteUrl(string remoteUrl)
		{			
		 	if (LogCallbacks)
				"InterceptRemoteUri: {0}".info(remoteUrl);
			if (OnUrlRequest.notNull())
				OnUrlRequest(remoteUrl); 	
		 	return remoteUrl;
		}
		
		public virtual void InterceptWebRequest(HttpWebRequest webRequest)
		{
			if (LogCallbacks)
				"InterceptWebRequest: {0}".info(webRequest);			
			//return webRequest;
		}		 		
		 
		 public virtual bool InterceptResponseHtml(Uri remoteUri)
		 {
		 	if (LogCallbacks)
				"InterceptResponseHtml: {0}".info(remoteUri);
		 	return false;
		 }
		 
		 
		 public virtual string HtmlContentReplace(Uri remoteUri, string htmlContent)
		 {
		 	if (LogCallbacks)
				"HtmlContentReplace: {0} with html content with size: {1}".info(remoteUri, htmlContent.size());
		 	return htmlContent;
		 }		 		 		
    }
}
