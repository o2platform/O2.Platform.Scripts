// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.XRules.Database.APIs;
//O2File:WatiN_IE_ExtensionMethods.cs
//O2Ref:WatiN.Core.1x.dll

namespace O2.XRules.Database.APIs
{
	[System.Runtime.InteropServices.ComVisible(true)]
    public class IE_JQuery
    {
    	public WatiN_IE ie;    	
    	public object _jQueryObject { get; set;}
    	public bool DebugMode { get; set; }
    	public bool IsInstalled { get { return this.isJQueryAvaialble(); }  }
    	public string PathToJQueryFile { get; set; }
    	
    	public IE_JQuery(WatiN_IE _ie)
    	{    		
    		this.ie = _ie;
    		this.PathToJQueryFile = @"jquery-1.5.2.min.js".local();
    		this.DebugMode = false;
    		
    		ie.onNavigate((url)=> this.installJQuery() );
    		this.installJQuery();
    	}
    	
    	public object getJQueryObject()
    	{    		
    		return this._jQueryObject ?? ie.invokeScript("jQuery");;
    	}
    	
    	public void setJQueryObject(object jQueryObject)
    	{    		
    		this._jQueryObject = jQueryObject;
    	}    	    	
    	
        public void write(string message)
        {
            "[IE to ToCSharp] : {0}".info(message);
        }
        
        public string ping(string message)
        {
            "[ping from IE] : {0}".info(message);
            return "pong...: " + message;
        }
               
        public IE_JQuery this[string elementName]
        {
        	get {
        			this.element(elementName);
        			return this;
        		}
        }
	}	    	    	    	        

	public static class IE_JQuery_ExtensionMethods_Install
	{
		public static bool isJQueryAvaialble(this IE_JQuery jQuery)
        {
        	return 	jQuery.getJQueryObject().notNull();
        }
                
        public static IE_JQuery installJQuery(this IE_JQuery jQuery)
        {
        	if (jQuery.PathToJQueryFile.fileExists().isFalse())
        	{
        		"[IE_JQuery][installJQuery]: could not file PathToJQueryFile: {0}".error(jQuery.PathToJQueryFile);
        		return jQuery;
        	}
        	if(jQuery.isJQueryAvaialble())
        	{
				"[IE_JQuery][installJQuery]: JQuery was already instaled in this page".info();				
        	}
        	else
        	{
        		var jQueryCode = jQuery.PathToJQueryFile.fileContents();
        		jQuery.ie.eval(jQueryCode); 
        		if(jQuery.isJQueryAvaialble())
        			"[IE_JQuery][installJQuery]: JQuery was instaled ok in this page".info();
        		else        		
        			"[IE_JQuery][installJQuery: there was a problem installing JQuery is this page".error();
        	}        	
			jQuery.ie.WebBrowser.ObjectForScripting = jQuery;         		
        	return jQuery;
        }
	}
	
	public static class IE_JQuery_ExtensionMethods_Invoke
	{	
	
		public static IE_JQuery invokeJQuery(this IE_JQuery jQuery, string parameters)
    	{
    		var script = "window.external.setJQueryObject(jQuery);" +
		 				 "var jQueryObject = window.external.getJQueryObject();" +   
		 				 "window.external.setJQueryObject(jQueryObject({0}));".format(parameters);
			if (jQuery.DebugMode)	 				  
				"[IE_JQuery][invokeJQuery]: {0}".debug(script);
    		jQuery.ie.invokeEval(script);
		 	return jQuery;
    	}
    	
    	
    	public static IE_JQuery invokeJQuery(this IE_JQuery jQuery, string method, string parameters)
    	{	
    		var script = "var jQueryObject = window.external.getJQueryObject();" +   
		 				 "window.external.setJQueryObject(jQueryObject.{0}({1}));".format(method,parameters);
			if (jQuery.DebugMode)	 				  
				"[IE_JQuery][invokeJQuery]: {0}".debug(script);		 				  				
			jQuery.ie.invokeEval(script);
		 	return jQuery;			    
    	}    	    	
	}
	
	public static class IE_JQuery_ExtensionMethods_Css
	{
				
		public static IE_JQuery css(this IE_JQuery jQuery, string cssCode)
		{
			return jQuery.invokeJQuery("css", cssCode);
		}
		
		public static IE_JQuery css(this IE_JQuery jQuery, string name, string value)
		{
			return jQuery.invokeJQuery("css", "'{0}' , '{1}'".format(name, value));
		}
		
		public static IE_JQuery attr(this IE_JQuery jQuery, string name, string value)
		{
			return jQuery.invokeJQuery("attr", "'{0}' , '{1}'".format(name, value));
		}
		
		public static IE_JQuery border(this IE_JQuery jQuery, int value)
		{
			return jQuery.border("{0}px solid".format(value));
		}
		
		public static IE_JQuery border(this IE_JQuery jQuery, string value)
		{
			return jQuery.css("border", value);
		}
		
		public static IE_JQuery fontSize(this IE_JQuery jQuery, int value)
		{
			return jQuery.fontSize("{0}px".format(value));
		}
		
		public static IE_JQuery fontSize(this IE_JQuery jQuery, string value)
		{
			return jQuery.css("font-size", value);
		}
		
		public static IE_JQuery bgColor(this IE_JQuery jQuery, string value)
		{
			return jQuery.backgroundColor(value);
		}
		
		public static IE_JQuery backgroundColor(this IE_JQuery jQuery, string value)
		{
			return jQuery.css("background-color", value);
		}		
	}
	
	public static class IE_JQuery_ExtensionMethods_Html
	{
		public static string html(this IE_JQuery jQuery)
		{
			jQuery.invokeJQuery("html","");
			return jQuery.getJQueryObject().str();
		}
		
		public static string html(this IE_JQuery jQuery, string value)
		{
			if (value.valid())
				value = "'{0}'".format(value);
			jQuery.invokeJQuery("html",value);
			return jQuery.getJQueryObject().str();
		}
		
		public static string attr(this IE_JQuery jQuery)
		{
			jQuery.invokeJQuery("attr","");
			return jQuery.getJQueryObject().str();
		}
		
		public static string attr(this IE_JQuery jQuery, string value)
		{
			if (value.valid())
				value = "'{0}'".format(value);
			jQuery.invokeJQuery("attr",value);
			return jQuery.getJQueryObject().str();
		}
	}
	
	public static class IE_JQuery_ExtensionMethods_HtmlElements
	{	
	
		public static IE_JQuery id(this IE_JQuery jQuery, string elementName)
		{
			return jQuery.invokeJQuery("'#{0}'".format(elementName));
		}
		
		public static IE_JQuery element(this IE_JQuery jQuery, string elementName)
		{
			return jQuery.invokeJQuery("'{0}'".format(elementName));
		}
		
		public static IE_JQuery value(this IE_JQuery jQuery, string value)
		{
			return jQuery.attr("value",value);
		}
		
		public static IE_JQuery h1(this IE_JQuery jQuery)
		{
			return jQuery.element("h1");
		}
		
		public static IE_JQuery p(this IE_JQuery jQuery)
		{
			return jQuery.element("p");
		}
		
		public static IE_JQuery a(this IE_JQuery jQuery)
		{
			return jQuery.element("a");
		}
		
		public static IE_JQuery img(this IE_JQuery jQuery)
		{
			return jQuery.element("img");
		}		
	}
	
	public static class IE_JQuery_ExtensionMethods_MiscMethods
	{
		public static IE_JQuery parent(this IE_JQuery jQuery)
		{
			return jQuery.invokeJQuery("parent", "");		
		}
		
		public static IE_JQuery click(this IE_JQuery jQuery)
		{
			return jQuery.invokeJQuery("click", "");		
		}
	}
	
	public static class IE_JQuery_ExtensionMethods_Events
	{
		public static IE_JQuery trigger(this IE_JQuery jQuery, string eventName)
		{
			return jQuery.invokeJQuery("trigger", "'{0}'".format(eventName));	
		}
		
		public static IE_JQuery keydown(this IE_JQuery jQuery)
		{
			return jQuery.trigger("keydown");
		}
		
	}
	
	public static class IE_JQuery_ExtensionMethods_Utils
	{
		public static IE_JQuery wait(this IE_JQuery jQuery, int miliSeconds)
		{
			jQuery.sleep(miliSeconds);
			return jQuery;
		}
		
		public static IE_JQuery addToPage(this IE_JQuery jQuery)
		{
			return jQuery.installJQuery();			
		}			
	}
}
