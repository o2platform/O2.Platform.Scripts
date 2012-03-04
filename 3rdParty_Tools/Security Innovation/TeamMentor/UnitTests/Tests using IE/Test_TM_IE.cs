// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using O2.Kernel;
using O2.XRules.Database.APIs;
using O2.XRules.Database.Utils;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Kernel.ExtensionMethods;

//O2File:Test_TM_Config.cs
//O2File:WatiN_IE_ExtensionMethods.cs
//O2File:_Extra_methods_Web.cs

//O2Ref:WatiN.Core.1x.dll
//O2Ref:nunit.framework.dll
  
namespace O2.SecurityInnovation.TeamMentor
{			
    public class Test_TM_IE
    {
    	public static bool IsWebServerUp = false;  // make this static so that we only try to get the html from the server once
    	
    	public WatiN_IE ie;
    	
    	public int CLOSE_BROWSER_IN_SECONDS = 5;
    	
    	public WatiN_IE set_IE_Object(WatiN_IE _ie)
    	{
    		this.ie = _ie;
    		return _ie;
    	}    	    
    	
    	public WatiN_IE set_IE_Object(string key)
    	{
    		ie = key.o2Cache<WatiN_IE>(
								()=> {
										"[Test_TM_IE] Setting IE object to key ".info(key);
										ie = "IE for {0}".format(key)
														 .popupWindow(800,500)
														 .add_IE()
														 .silent(true);
										
										ie.WebBrowser.onClosed(()=> key.o2Cache(null) );
										return ie;
									 });
			Assert.That(ie.notNull(), "set_IE_Object ie was null");
//			check_if_TM_WebServer_is_Running();
			return ie;
    	}
    	
    	public void close_IE_Object()
    	{        		    		
    		Assert.That(ie.notNull(), "close_IE_Object ie was null");    		    		
    		ie.WebBrowser.closeForm_InNSeconds(Test_TM.CLOSE_BROWSER_IN_SECONDS);    		
    	}    	    	        	
    
    	public WatiN_IE open(string virtualPath)
    	{
    		var fullUrl = "{0}{1}".format(Test_TM.tmServer , virtualPath);
    		if (fullUrl.isUri())
    			ie.open(fullUrl);
    		return ie;
    	}
    	//Javascript Libraries    	
    	
    	public Test_TM_IE load_Javascript_jQuery()
    	{
    		Assert.That(ie.notNull(), "[load_Javascript_jQuery] ie object was null");
    		ie.downloadAndExecJavascriptFile(Test_TM.tmServer + "Javascript/jQuery/jquery-1.6.2.min.js");
    		return this;
    	}
    	
		public Test_TM_IE load_Javascript_TM_WebServices()
    	{    		
			ie.downloadAndExecJavascriptFile(Test_TM.tmServer + "Javascript/TM_WebServices/TM_WebServices.js");
			return this;
		}
		
	
	
    }
    
    public class Test_TM_IE_Tests : Test_TM_IE
    {    	
    	/*    	
    	[Test]
    	//O2File:Test_TM_Setup.cs
    	public void check_If_Site_Is_Working()
    	{
    		new Test_TM_Setup().check_if_TM_WebServer_is_Running();
    	}
*/   
/*    	//[Test]
		public void test_check_if_TM_WebServer_is_Running()
		{			
			base.check_if_TM_WebServer_is_Running();
		}*/
    }
    
    public static class WatiN_JQuery_ExtensionMethods
    {
    	public static string jQuery_Append_Body(this string htmlToAppend, WatiN_IE ie)
    	{    		
    		ie.jQuery_Append_Body(htmlToAppend);
    		return htmlToAppend;
    	}
    	
    	public static WatiN_IE jQuery_Append_Body(this WatiN_IE ie, string htmlToAppend)
    	{
    		ie.eval("$('body').append('<div>{0}<div>')".format(htmlToAppend));
    		return ie;
    	}
    }
}