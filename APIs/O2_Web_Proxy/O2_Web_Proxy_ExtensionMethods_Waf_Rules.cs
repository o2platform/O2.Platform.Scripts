// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using FluentSharp.CoreLib;
using FluentSharp.REPL;
using HTTPProxyServer;

//O2File:O2_Web_Proxy.cs

  
  
namespace O2.XRules.Database.APIs
{	
	public static class O2_Web_Proxy_ExtensionMethods_WafRules
	{
		public static bool loadWafRule(this O2_Web_Proxy o2WebProxy, string wafRuleFile)
		{
			return o2WebProxy.loadWafRule(wafRuleFile, false);
		}
		
		public static bool loadWafRule(this O2_Web_Proxy o2WebProxy, string wafRuleFile, bool logCallBacks)
		{				
			if (wafRuleFile.fileExists().isFalse())
				wafRuleFile = wafRuleFile.local();
			if (wafRuleFile.fileExists().isFalse())
				"[O2_Web_Proxy] in loadWafRule, could not find rule file: {0}".error(wafRuleFile);	
			else 
			{
				"[O2_Web_Proxy]: loading Waf Rule file: {0}".info(wafRuleFile);
				//var assembly = new O2.DotNetWrappers.DotNet.CompileEngine().compileSourceFile(ruleToLoad);
				var assembly = wafRuleFile.compile();
				if (assembly.isNull())
					"failed to compiled rule: {0}".error(wafRuleFile); 
				else
				{
					"[O2_Web_Proxy]: Compiled ok".info();
					var ruleType = assembly.type(wafRuleFile.fileName_WithoutExtension());
					if (ruleType.isNull())
					{
						"failed to find type: {0}".error(wafRuleFile.fileName_WithoutExtension()); 						
					}
					else
					{
						"[O2_Web_Proxy]: Found rule type ok".info();
						o2WebProxy.WafRule = ruleType.ctor();
						o2WebProxy.WafRule.prop("LogCallbacks", logCallBacks);
						"[O2_Web_Proxy]: Rule Loaded".info();
						//setWafRuleLinkViewerCallback();
						//browserPanel.set_Text("Browser with rule loaded: {0}".format(ruleToLoad.fileName_WithoutExtension())); 						
						 
						ProxyServer.InterceptWebRequest		= (webRequest)=> 							{ o2WebProxy.WafRule.invoke("InterceptWebRequest", webRequest);};
						//ProxyServer.OnResponseReceived 		= (requestResponseData)=> 					{ o2WebProxy.WafRule.invoke("InterceptOnResponseReceived", requestResponseData);};
						ProxyServer.InterceptRemoteUrl		= (remoteUrl) => 							{ return (string) o2WebProxy.WafRule.invoke("InterceptRemoteUrl",remoteUrl); };
						ProxyServer.InterceptResponseHtml	= (uri) => 		 							{ return (bool)   o2WebProxy.WafRule.invoke("InterceptResponseHtml", uri); };  
						ProxyServer.HtmlContentReplace		= (uri,content)=>							{ return (string) o2WebProxy.WafRule.invoke("HtmlContentReplace", uri, content);} ; 
						
						return true;
					}
				}
				//var wafRule = "WAF_Rule.cs".local().compile().types()[0].ctor();			
				//WafRule.o2WebProxy = wafRuleFile.compile().type("fileName_WithoutExtension").ctor();			
				//object wafRuleObject = 	"WAF_Rule_NoPorto.cs".local().compile().types()[0].ctor();	 
				
			}
			//setWafRuleLinkViewerCallback();
			return false;
		}
	}
}	
	