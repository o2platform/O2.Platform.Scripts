// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using FluentSharp.CoreLib;

//O2File:WAF_Rule.cs

namespace O2.XRules.Database.APIs  
{ 
    public class WAF_Rule_NoPorto : WAF_Rule
    {    
		public override string InterceptRemoteUrl(string remoteUrl)	    	    	    	    	    
		{
			base.InterceptRemoteUrl(remoteUrl);
			"[WAF_Rule_NoGoogle] IN InterceptRemoteUrl".error();			
			if (remoteUrl.contains("Porto"))
			{
				"No Porto here, replacing it with Benfica".info();
				return remoteUrl.replace("Porto","Benfica");
			}
			return remoteUrl;
		}
    }
}
