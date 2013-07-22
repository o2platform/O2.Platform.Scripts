// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using FluentSharp.CoreLib;
using FluentSharp.Watin;
using FluentSharp.WinForms;
using NUnit.Framework;

//O2File:WatiN_IE_ExtensionMethods.cs
//O2Ref:WatiN.Core.1x.dll
//O2Ref:nunit.framework.dll

namespace O2.UnitTests
{		
	//helper class for creating UnitTests using IE (and WatiN)
	
    public abstract class Unit_Tests_using_IE 
    {
    	public string Server { get; set; }
    	
    	public WatiN_IE ie;
    	
    	public int CLOSE_BROWSER_IN_SECONDS = 5;
    	    	    	    	
		public Unit_Tests_using_IE() 
		{
		
		}		    	
    	
    	public Unit_Tests_using_IE set_IE_Object(WatiN_IE _ie)    		
    	{
    		this.ie = _ie;
    		return this;
    	}
    	   	    	
    	public Unit_Tests_using_IE set_IE_Object(string key)    		
    	{
    		ie = key.o2Cache<WatiN_IE>(
								()=> {
										"[Test_TM_IE] Setting IE object to key ".info(key);
										ie = "IE for {0}".format(key)
														 .popupWindow(800,500)
														 .add_IE()
														 .silent(false);
										
										ie.WebBrowser.onClosed(()=> key.o2Cache(null) );
										return ie;
									 });
			Assert.That(ie.notNull(), "set_IE_Object ie was null");
			return this;
    	}
    	
    	public Unit_Tests_using_IE close_IE_Object()    		
    	{        		    		
    		Assert.That(ie.notNull(), "close_IE_Object ie was null");    		    		
    		ie.WebBrowser.closeForm_InNSeconds(CLOSE_BROWSER_IN_SECONDS);    		
    		return this;
    	}    	    	        	
    
    	public Unit_Tests_using_IE open(string virtualPath)    		
    	{
    		if (this.Server.valid())
    		{
    			if (virtualPath.isUri())			// just in case we actually get a full url here
    				ie.open(virtualPath);
    			else
    			{
	    			if (virtualPath.starts("/") || virtualPath.starts(@"\"))
	    				virtualPath = virtualPath.removeFirstChar();
	    			var fullUrl = "{0}{1}".format(this.Server , virtualPath);
					if (fullUrl.isUri())
	    				ie.open(fullUrl);
	    		}
    		}
    		else
    			"[Unit_Tests_with_IE], the local open method was called but the Server variable is not set".error();

    		return this;
    	}    	
    	
    	public Unit_Tests_using_IE minimized()
    	{    	
    		ie.minimized();
    		return this;
    	}
    	
    	public Unit_Tests_using_IE maximized<T>()
    	{    	
    		ie.maximized();
    		return this;
    	}    
    }        
}