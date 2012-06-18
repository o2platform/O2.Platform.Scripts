// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Reflection; 
using System.Text;
using O2.Interfaces.O2Core; 
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.DotNet;
using O2.Views.ASCX;
using O2.Views.ASCX.classes.MainGUI;
using O2.External.SharpDevelop.AST;
using O2.External.SharpDevelop.ExtensionMethods; 
using O2.XRules.Database.Utils;
using O2.XRules.Database.Utils.O2;
using O2.XRules.Database.Languages_and_Frameworks.DotNet;
using WatiN.Core;
using WatiN.Core.Interfaces;
using WatiN.Core.DialogHandlers;
using SHDocVw;
using mshtml; 

//O2File:WatiN_IE.cs
//O2File:ascx_CaptchaQuestion.cs
//O2File:ascx_AskUserForLoginDetails.cs
//O2File:ISecretData.cs
//O2File:DotNet_Viewstate.cs

//O2File:_Extra_methods_WinForms_Controls.cs

//O2Ref:O2_External_IE.dll
//O2Ref:WatiN.Core.1x.dll
//O2Ref:Interop.SHDocVw.dll
//O2Ref:Microsoft.mshtml.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:System.Xml.dll
//O2Ref:O2_API_Ast.dll
//O2Ref:Microsoft.mshtml.dll

namespace O2.XRules.Database.APIs
{
    public static class WatiN_IE_ExtensionMethods
    {        	
    	//global conts
    	public static int WAITFORJSVARIABLE_MAXSLEEPTIMES = 10;
    	
    	//WatIN ExtensionMethods
 
    	public static WatiN_IE ie(this string url)
    	{
    		int top = 0;
    		int left = 0;    		
    		return url.ie(top, left);
    	}
 
    	public static WatiN_IE ie(this string url, int top, int left)
    	{
    		int width = 800;
    		int height = 600;
    		return url.ie(top, left, width, height);
    	}
 
    	public static WatiN_IE ie(this string url, int top, int left, int width, int height)
    	{    		
    		var ie = new WatiN_IE();
    		ie.createIEObject(url, top, left, width, height);
			return ie;						
    	}
 
    	public static WatiN_IE ie(this O2.External.IE.Wrapper.O2BrowserIE o2BrowserIE)
 		{ 			
			return (o2BrowserIE as System.Windows.Forms.WebBrowser).ie();
		}
 
		public static WatiN_IE ie(this System.Windows.Forms.WebBrowser webBrowser)
 		{ 			
			return new WatiN_IE(webBrowser);						
		}
 	}
 	
 	public static class WatiN_IE_ExtensionMethods_InternetExplorer
 	{
 		public static WatiN_IE  fullScreen(this WatiN_IE ie)
 		{
 			return ie.fullScreen(true);
 		}
 		public static WatiN_IE  fullScreen(this WatiN_IE ie, bool value)
 		{
 			var internetExplorer = ie.internetExplorer();
 			if (internetExplorer.notNull())
 				internetExplorer.FullScreen = value;
 			return ie;
 		}
 	}
 	
 	public static class WatiN_IE_ExtensionMethods_Cookies
 	{
 		public static string cookiesRaw(this WatiN_IE ie)
 		{
 			try
 			{
 				if (ie.IE.HtmlDocument.notNull())
 					return ie.IE.HtmlDocument.cookie;
 				return "";
 			}
 			catch(Exception ex)
 			{
 				ex.log("in WatiN_IE.cookiesRaw");
 				return "";
 			}
 		}
 		
 		public static List<string> cookies(this WatiN_IE ie)
 		{
 			if (ie.cookiesRaw().valid())
 				return (from cookie in ie.cookiesRaw().split(";")
 						select cookie.trim()).toList();
 						
 			return new List<string>(); 			
 		}
 		
 		public static Dictionary<string,string> cookies_asDictionary(this WatiN_IE ie)
 		{ 			
 			var cookies_asDictionary = new Dictionary<string,string>();
 			foreach(var cookie in ie.cookies())
 			{
 				var splittedCookie = cookie.split("=");
 				if (splittedCookie.size().neq(2))
 					"[Watin_IE] in cookies_asDictionary, there was an error splitting cookie: {0}".error(cookie);
 				else
 					cookies_asDictionary.add(splittedCookie[0], splittedCookie[1]); 					
 			}
 			return cookies_asDictionary;
 		}
 		
 		public static string cookiesRaw(this Dictionary<string,string> cookies_asDictionary)
 		{
 			var cookieRaw = "";
 			foreach(var item in cookies_asDictionary)
 				cookieRaw+= "{0}={1}&".format(item.Key, item.Value);
 			return cookieRaw;
 		}
 		
 		//Need to find a better way to do this  (check what happens if the cookie name has a space between name & =
 		public static string cookie(this WatiN_IE ie, string cookieName)
 		{
 			var stringToFind = cookieName + "=";
 			foreach(var cookie in ie.cookies()) 			
 				if (cookie.starts(stringToFind))
 					return cookie.Substring(stringToFind.size());
			return "";
 		}
 		
 		public static WatiN_IE cookies_Clear(this WatiN_IE ie)
 		{
 			ie.IE.ClearCache();
 			ie.IE.ClearCookies();
 			return ie;
 		}
 		
 		public static WatiN_IE cookies_Clear(this WatiN_IE ie, string url)
 		{
 			ie.IE.ClearCache();
 			ie.IE.ClearCookies(url);
 			return ie;
 		}
 	}
 	
 	public static class WatiN_IE_ExtensionMethods_Misc
    {
    	public static WatiN_IE showMessage(this WatiN_IE ie, string message, int sleepValue)
    	{
    		ie.showMessage(message);
    		ie.wait(sleepValue);
    		return ie;
    	}
    	
    	public static WatiN_IE showMessage(this WatiN_IE ie, string message)
    	{
    		return ie.showMessage(message,false);
    	}
    	
    	public static WatiN_IE showMessage(this WatiN_IE ie, string message, bool runOnSeparateThread)
    	{    		
    		message = message.Replace("".line(), "<br/>");
			var messageTemplate = "<html><body><div style = \"position:absolute; top:50%; width:100%; text-align: center;font-size:20pt; font-family:Arial\">{0}</div></body></html>";
			
    		if (runOnSeparateThread)    			
	    		O2Thread.mtaThread(()=>{ie.set_Html(messageTemplate.format(message));});
	    	else
	    		ie.set_Html(messageTemplate.format(message));
    		return ie;
    	}
    	// uri & url
 
  		public static string html (this HTMLHtmlElementClass htmlElementClass)
  		{
  			if (htmlElementClass.notNull())
  				return htmlElementClass.outerHTML;
  			return "";
  		}
  		
 		public static HTMLHtmlElementClass documentElement (this WatiN_IE ie)
 		{
 			try
    		{
	    		if (ie.IE.InternetExplorer.notNull() && ie.IE.InternetExplorer is IWebBrowser2)
	    		{
	    			var webBrowser = (IWebBrowser2)ie.IE.InternetExplorer;
	    			if (webBrowser.Document.notNull() && webBrowser.Document is HTMLDocumentClass)
	    			{
	    				var htmlDocument = (HTMLDocumentClass)webBrowser.Document;
	    				if (htmlDocument.documentElement.notNull() && htmlDocument.documentElement is HTMLHtmlElementClass)
	    					return (HTMLHtmlElementClass)htmlDocument.documentElement;
	    			}    			
	    		}    		
    		}
    		catch(Exception ex)
    		{
    			ex.log("in WatiN_IE documentElement()");
    		}
    		return null;
 		}
 		
 		public static string html(this WatiN_IE ie)
    	{
    		return ie.IE.Html;
    		//return ie.documentElement().html();
    		/*
			try
    		{    			
	    		if (ie.IE.InternetExplorer.notNull() && ie.IE.InternetExplorer is IWebBrowser2)
	    		{
	    			var webBrowser = (IWebBrowser2)ie.IE.InternetExplorer;
	    			if (webBrowser.Document.notNull() && webBrowser.Document is HTMLDocumentClass)
	    			{
	    				var htmlDocument = (HTMLDocumentClass)webBrowser.Document;
	    				if (htmlDocument.documentElement.notNull())
	    					return htmlDocument.documentElement.outerHTML;
	    			}    			
	    		}    		
    		}
    		catch(Exception ex)
    		{
    			ex.log("in WatiN_IE html()");
    		}
    		return "";*/
    	}
    	
    	public static WatiN_IE html(this WatiN_IE ie, string newHtml)
    	{
    		return ie.set_Html(newHtml);
    	}
    	
    	public static WatiN_IE set_Html(this WatiN_IE ie, string newHtml)
    	{
    		ie.open(newHtml.saveWithExtension(".html"));
    		return ie;
    	}
    	
    	public static Uri uri(this WatiN_IE watinIe)
    	{
    		try
    		{    			
    			return watinIe.IE.Uri;
    		}
    		catch
    		{
    			return null;
    		}
    	}
 
 		public static WatiN_IE if_NoPageLoaded(this WatiN_IE watinIe, Action callback)
 		{
 			if (watinIe.noPageLoaded())
 				callback();
 			return watinIe;
 		}
 		
 		public static bool noPageLoaded(this WatiN_IE watinIe)
 		{
 			return watinIe.url().isUri().isFalse() || 
 				   watinIe.url() == "about:blank";
 		}
 			
    	public static string url(this WatiN_IE watinIe)
    	{	
    		try
    		{    			
    			return watinIe.uri().str();
    		}
    		catch
    		{
    			return null;
    		}
    	}
    	
    	public static bool url(this WatiN_IE watinIe, string url)
		{
			return  (watinIe.url() == url);
		}

    	public static string title(this WatiN_IE watinIe)
    	{
    		return watinIe.IE.Title;
    	}
 
    	public static bool title(this WatiN_IE watinIe, string title)
    	{
    		return watinIe.IE.Title == title;
    	}
 
    	public static string processId(this WatiN_IE watinIe)
    	{
    		return watinIe.IE.ProcessID.str();
    	}
    	// region close 
 
    	public static WatiN_IE close(this WatiN_IE watinIe)
    	{
    		"closing WatIN InternetExplorer Process".info();
    		watinIe.close();
    		//watinIe.Close();
    		return watinIe;
    	}
 
    	public static WatiN_IE closeInNSeconds(this WatiN_IE watinIe, int seconds)
    	{
    		if (seconds > 60)
    		{
    			"in WatiN_IE closeInNSeconds, provided value bigger than 60 secs, so changing the delay (before close) to 60".error();
    			seconds = 60;
    		}
    		"IE instance will be closed in {0} seconds".info(seconds);
    		O2Thread.mtaThread( 
			()=>{ 
					watinIe.wait(5000);  
					watinIe.close();   
				}); 
			return watinIe;
    	}
  
    	public static InternetExplorerClass internetExplorer(this WatiN_IE watinIe)
    	{
    		return watinIe.InternetExplorer;
    	}
 
 		public static WatiN_IE silent(this WatiN_IE watinIe, bool value)
 		{
 			if (watinIe.IE.InternetExplorer != null)
 				if (watinIe.IE.InternetExplorer is IWebBrowser2)
 					(watinIe.IE.InternetExplorer as IWebBrowser2).Silent = value;
 			return watinIe; 			
 		}				
		
		public static WatiN_IE open(this WatiN_IE watinIe, string url)
		{
			return watinIe.open(url,0);
		}
 
		public static WatiN_IE open(this WatiN_IE watinIe, string url, int miliseconds)
    	{
    		if (watinIe.isNull())
    			return watinIe;
    		"[WatIN] open: {0}".info(url);
    		watinIe.execute(
    			()=>{
    					watinIe.IE.GoTo(url);
    					watinIe.wait(miliseconds);
    				});
    		return watinIe;
    	}
    	
    	public static WatiN_IE open_ASync(this WatiN_IE watinIe, string url)
    	{
    		O2Thread.mtaThread(()=> watinIe.open(url));
    		return watinIe;
    	}
    	
    	public static WatiN_IE open_usingPOST(this WatiN_IE watinIe, string postUrl,string postData)
    	{
    		return watinIe.open_usingPOST(postUrl, "application/x-www-form-urlencoded", postData);
    	}
    	
    	public static WatiN_IE open_usingPOST(this WatiN_IE watinIe, string postUrl, string contentType, string postData)
    	{    		
    		try
    		{
    			"[WatIN] open using POST: {0} ({1} bytes)".info(postUrl, postData.size());
				var browser = (watinIe.IE.InternetExplorer as IWebBrowser2); 
				
				object PostDataByte = (object)Encoding.UTF8.GetBytes(postData);
				object AdditionalHeaders = "Content-Type: " + contentType.line();;
				object nullValue = null;
				browser.Navigate(postUrl, ref nullValue, ref nullValue, ref PostDataByte, ref AdditionalHeaders);
				watinIe.IE.WaitForComplete();
			}
			catch(Exception ex)
			{
				ex.log("in WatiN_IE open_usingPOST");
			}
			return watinIe;
    	}
    	
    	public static WatiN_IE openWithBasicAuthentication(this WatiN_IE watinIe, string url, string username, string password)
    	{
    		var encodedHeader = "Authorization: Basic " + "{0}:{1}".format(username, password).base64Encode();
    		return watinIe.openWithExtraHeader(url, encodedHeader);
    	}
    	public static WatiN_IE openWithExtraHeader(this WatiN_IE watinIe, string url, string extraHeader)
    	{
    		try
    		{
    			"[WatIN] open with extra Header: {0} ({1} bytes)".info(url, extraHeader.size());
    			object headerObject = extraHeader.line();
    			object flags = null;
    			(watinIe.IE.InternetExplorer as IWebBrowser2).Navigate(url, 
    																   ref flags, 
    																   ref flags, 
    																   ref flags, 
    																   ref headerObject);
				watinIe.IE.WaitForComplete();
			}
    		catch(Exception ex)
    		{
    			ex.log("in WatiN_IE openWithExtraHeader(...)");
    		}

    		return watinIe;
    	}
    	
    	
    	public static WatiN_IE setTimeout(this WatiN_IE watinIe, int timeout)
    	{
    		Settings.WaitForCompleteTimeOut = timeout;
    		return watinIe;
    	}
    	    	
    	public static WatiN_IE resetTimeout(this WatiN_IE watinIe)
    	{
    		Settings.WaitForCompleteTimeOut = 30;
    		return watinIe;
    	}
 
    	public static WatiN_IE wait(this WatiN_IE watinIe)
    	{
    		return watinIe.wait(1000);
    	}
 
    	public static WatiN_IE wait(this WatiN_IE watinIe, int miliseconds)
    	{
    		if (WatiN_IE.WaitingEnabled && miliseconds > 0)
    			watinIe.sleep(miliseconds);
    		return watinIe;
    	}
 
    	public static WatiN_IE waitNSeconds(this WatiN_IE watinIe, int seconds)
    	{
    		if (seconds > 0)
    			watinIe.sleep(seconds* 1000);
    		return watinIe;
    	}
 
 
    	public static T wait<T>(this T element, int miliseconds)
    		where T : Element
    	{
    		if (miliseconds > 0)
    			element.sleep(miliseconds);
    		return element;
    	}
 
 		public static WatiN_IE waitForComplete(this WatiN_IE watinIe)
 		{
 			watinIe.IE.WaitForComplete(); 
 			return watinIe;
 		}
 	}
 	
 	
 	public static class WatiN_IE_ExtensionMethods_DialogHandlers
 	{
 	
 		public static WatiN_IE setDialogWatcher(this WatiN_IE watinIe)
 		{ 			
 			if (watinIe.IE.DialogWatcher == null)
			{				
				var dialogWatcher = DialogWatcher.GetDialogWatcherForProcess(Processes.getCurrentProcessID());
				dialogWatcher.CloseUnhandledDialogs = false; 
				//set the value of _dialogWatcher with the current inproc IE dialogWatcher
				var field = PublicDI.reflection.getField(typeof(DomContainer),"_dialogWatcher"); 
				PublicDI.reflection.setField(field, watinIe.IE, dialogWatcher);
			}
			return watinIe;
 		}
 		
 		public static DialogWatcher getDialogWatcher(this WatiN_IE watinIe)
 		{
 			setDialogWatcher(watinIe);
 			return watinIe.IE.DialogWatcher;
 		}
 		
 		public static WatiN_IE clear_DialogWatchers(this WatiN_IE watinIe)
 		{
 			watinIe.IE.DialogWatcher.clear();
 			return watinIe;
 		}
 		public static DialogWatcher clear(this DialogWatcher dialogWatcher)
 		{
 			if (dialogWatcher.notNull())
 				dialogWatcher.Clear();
 			return dialogWatcher;
 		}
 		
 		public static List<BaseDialogHandler> dialogHandlers(this WatiN_IE watinIe)
 		{
 			watinIe.setDialogWatcher();			// make sure this is set 			
 			var dialogHandlers = new List<BaseDialogHandler>();
 			foreach(BaseDialogHandler handler in (ArrayList)watinIe.IE.DialogWatcher.field("handlers"))
 				dialogHandlers.Add(handler);
 			return dialogHandlers;
 			//return (ArrayList)watinIe.IE.DialogWatcher.field("handlers");
 		}
 		
 		public static T dialogHandler<T>(this WatiN_IE watinIe)
 			where T : BaseDialogHandler 
 		{
 			foreach(var dialogHandler in watinIe.dialogHandlers())
 				if (dialogHandler is T)
 					return (T)dialogHandler;
 			return null;
 		}
 		
 		public static AlertAndConfirmDialogHandler getAlertsHandler(this WatiN_IE watinIe)
 		{
 			var alertHandler = watinIe.dialogHandler<AlertAndConfirmDialogHandler>();
 			if (alertHandler.isNull())
 			{
 				alertHandler = new AlertAndConfirmDialogHandler();
				watinIe.IE.AddDialogHandler(alertHandler); 
			}
			return alertHandler;
 		}
 		
 		public static AlertAndConfirmDialogHandler reset(this AlertAndConfirmDialogHandler alertHandler)
 		{
 			alertHandler.Clear();
 			return alertHandler;
 		}
 		
 		public static List<string> alerts(this AlertAndConfirmDialogHandler alertHandler)
 		{
 			return alertHandler.Alerts.toList(); 			
 		}
 		
 		public static string lastAlert(this AlertAndConfirmDialogHandler alertHandler)
 		{
 			if (alertHandler.notNull() && 
 				alertHandler.alerts().notNull() && 
 				alertHandler.alerts().size()>0) 				
 			{
	 			return alertHandler.alerts().Last();
	 		}
 			return "";
 		}
 		
 		public static string open_and_HandleFileDownload(this WatiN_IE watinIe , string url, string fileName)
 		{
			var tmpFile = fileName.tempFile();
			var waitUntilHandled = 20;
			var waitUntilDownload = 300;
			
			var fileDownloadHandler = watinIe.dialogHandler<FileDownloadHandler>();

			if (fileDownloadHandler.notNull())
			{
				watinIe.IE.RemoveDialogHandler(fileDownloadHandler); 
			}
			
			fileDownloadHandler = new FileDownloadHandler(tmpFile); 
			watinIe.IE.AddDialogHandler(fileDownloadHandler); 
			
			 
			fileDownloadHandler.field("saveAsFilename",tmpFile);
			fileDownloadHandler.field("hasHandledFileDownloadDialog",false);
			
			watinIe.open_ASync(url);
			try
			{
				fileDownloadHandler.WaitUntilFileDownloadDialogIsHandled(waitUntilHandled);			
				"after: {0}".info("WaitUntilFileDownloadDialogIsHandled");
				fileDownloadHandler.WaitUntilDownloadCompleted(waitUntilDownload);
				"after: {0}".info("WaitUntilDownloadCompleted");
			}
			catch(Exception ex)
			{
				"[WatiN_IE][open_and_HandleFileDownload] {0}".error(ex.Message);
			}
				
			if (fileDownloadHandler.SaveAsFilename.fileExists())
			{
				"[WatiN_IE] downloaded ok '{0}' into '{1}'".info(url, fileDownloadHandler.SaveAsFilename);
				watinIe.IE.RemoveDialogHandler(fileDownloadHandler); 
				return fileDownloadHandler.SaveAsFilename;
			}
			"[WatiN_IE] failed to download '{0}' ".info(url);
			return null;
		}

 	}
 	
	public static class WatiN_IE_ExtensionMethods_Image
	{
 
    	public static WatiN.Core.Image image(this WatiN_IE watinIe, string name)
    	{
    		foreach(var image in watinIe.images())
    			if (image.id() == name)//|| link.text() == name)
    				return image;
    		"in WatiN_IE could not find Image with name:{0}".error(name ?? "[null value]");
    		return null;    				
    	}
 
    	public static List<WatiN.Core.Image> images(this WatiN_IE watinIe)
    	{
    		return (from image in watinIe.IE.Images
    				select image).toList();
    	}
 
    	public static Uri uri(this WatiN.Core.Image image)
    	{
			return (image != null)
					? image.Uri
					: null;
    	}
 
    	public static string url(this WatiN.Core.Image image)
    	{
			return (image != null)
					? image.Uri.str()
					: "";
    	}
 
    	public static string src(this WatiN.Core.Image image)
    	{
			return (image != null)
					? image.Src
					: "";
    	}
    	
    	public static List<Uri> uris(this List<WatiN.Core.Image> images)
    	{
			return (from image in images
					select image.Uri).toList();
    	}
    	    	
    	public static List<string> urls(this List<WatiN.Core.Image> images)
    	{
			return (from image in images
					select image.Uri.str()).toList();
    	}
    	
    	public static List<string> srcs(this List<WatiN.Core.Image> images)
    	{
			return (from image in images
					select image.Src).toList();
    	}
 
 	}
 	
    public static class WatiN_IE_ExtensionMethods_Link
    { 	
 
    	public static Link link(this WatiN_IE watinIe, string name)
    	{
    		foreach(var link in watinIe.links())
    			if (link.id() == name || link.text() == name)
    				return link;
    		"in WatiN_IE could not find Link with name:{0}".error(name ?? "[null value]");
    		return null;    				
    	}
 
    	public static List<Link> links(this WatiN_IE watinIe)
    	{
    		if (watinIe.notNull() && watinIe.IE.notNull())
    			return (from link in watinIe.IE.Links
    					select link).toList();
    		return new List<Link>();
    	}
 
    	public static string url(this Link link)
    	{
			return (link != null)
					? link.Url
					: "";
    	}     	
    	
    	public static Link click(this Link link)
    	{
    		return link.click(0);    		
    	}    	   	
 
    	public static Link click(this Link link, int miliseconds)
    	{
    		if (link != null)
    		{
    			link.Click();
    			O2.XRules.Database.APIs.WatiN_IE_ExtensionMethods_Misc.wait(link,miliseconds); 
    		}
    		return link;
    	}
 
 
    	public static List<string> texts(this List<Link> links)
    	{
    		return (from link in links 
    				select link.text()).toList();
    	}
 
    	public static List<string> urls(this List<Link> links)
    	{
    		return (from link in links 
    				select link.url()).toList();
    	} 
    	
    	public static List<Uri> uris(this List<Link> links)
    	{
    		return links.urls().uris();
    	}
    	
    	public static List<string> ids(this List<Link> links)
		{
			return (from link in links
					where (link.Id != null)
					select link.Id).toList();
		}
 
		public static bool hasLink(this WatiN_IE watinIe, string nameOrId)
		{			
			foreach(var link in watinIe.links())
				if (link.id() == nameOrId || link.text() == nameOrId)
					return true;
			return false;
			//return watinIe.links().ids().Contains(id);
		}
		
		public static Link waitForLink(this WatiN_IE watinIe, string nameOrId)
		{
			return watinIe.waitForLink(nameOrId, 500, 10);
		}
		
		public static Link waitForLink(this WatiN_IE watinIe, string nameOrId, int sleepMiliseconds, int maxSleepTimes)
		{
		
			var count = 0;
			while(watinIe.hasLink(nameOrId).isFalse())
			{
				if (count++ >=maxSleepTimes)
					break;
				watinIe.sleep(500, false);
			}
			return watinIe.link(nameOrId);
		}

 	}
 	
    public static class WatiN_IE_ExtensionMethods_Button
    {  	    	
    	public static WatiN.Core.Button button(this WatiN_IE watinIe, string identifier)
    	{
    		identifier = identifier.trim();
    		if (identifier.valid())
    		{
    			identifier = identifier.trim();
    			foreach(var button in watinIe.buttons())
    				if ((button.id().notNull() && button.id().trim() == identifier) || 
    					(button.value().notNull() && button.value().trim() == identifier) ||
    					(button.className().notNull() && button.className().trim() == identifier) ||
    					(button.outerText().notNull() && button.outerText().trim() == identifier) )
    					return button;
			}    				
    		"in WatiN_IE could not find Button with identifier (searched on id,name, classname and outerText):{0}".error(identifier ?? "[null value]");
    		return null;    				
    	}
 
    	public static List< WatiN.Core.Button> buttons(this WatiN_IE watinIe)
    	{
    		return (from button in watinIe.IE.Buttons
    				select button).toList();
    	}
 
 
    	public static List<string> texts(this List<WatiN.Core.Button> buttons)
    	{
    		return (from button in buttons 
    				select button.text()).toList();
    	}
 
    	public static List<string> values(this List<WatiN.Core.Button> buttons)
    	{
    		return (from button in buttons 
    				select button.value()).toList();
    	}
 
    	public static List<string> ids(this List<WatiN.Core.Button> buttons)
    	{
    		return (from button in buttons 
    				select button.id()).toList();
    	}
 
    	public static List<string> names(this List<WatiN.Core.Button> buttons)
    	{
    		return buttons.ids();
    	}
 
		public static string value(this WatiN.Core.Button button)
    	{    		
    		return (button != null)
    					? button.Value
    					: "";
    	}
    	public static string outerText(this WatiN.Core.Button button)
    	{    		
    		return (button != null)
    					? button.OuterText
    					: "";
    	}
 		 
    	public static WatiN.Core.Button click(this WatiN.Core.Button button)
    	{    		
    		if (button != null)
    			button.Click();
    		return button;
    	}
 
 		public static bool hasButton(this WatiN_IE watinIe, string nameOrId)
		{	
			foreach(var button in watinIe.buttons())
				if (button.id() == nameOrId || button.value() == nameOrId)
					return true;
			return false;
			//return watinIe.buttons().ids().Contains(id);						
		}
 
 		public static WatiN.Core.Button waitForButton(this WatiN_IE watinIe, string nameOrId)
		{
			return watinIe.waitForButton(nameOrId, 500, 10);
		}
		
		public static WatiN.Core.Button waitForButton(this WatiN_IE watinIe, string nameOrId, int sleepMiliseconds, int maxSleepTimes)
		{
		
			var count = 0;
			while(watinIe.hasButton(nameOrId).isFalse())
			{
				if (count++ >=maxSleepTimes)
					break;
				watinIe.sleep(500, false);
			}
			return watinIe.button(nameOrId);
		}
 
		public static WatiN_IE click(this WatiN_IE watinIe, string id)
		{
			if (watinIe.hasButton(id))
			{
				var button = watinIe.button(id);			
				button.click();
			}
			else if (watinIe.hasLink(id))
			{
				var link = watinIe.link(id);			
				link.click();
			}
			else
				"in WatiN_IE click, could not find button or link with id: {0}".error(id);
			return watinIe;
 
		}

 	}
 	
    public static class WatiN_IE_ExtensionMethods_SelectList
    {	
 
    	public static SelectList selectList(this WatiN_IE watinIe, string name)
    	{    		
    		foreach(var selectList in watinIe.selectLists())
    			if (selectList.id() == name)
    				return selectList;
    		"in WatiN_IE could not find SelectList with name:{0}".error(name ?? "[null value]");
    		return null;    				
    	}
 
    	public static List<SelectList> selectLists(this WatiN_IE watinIe)
    	{
    		return (from selectList in watinIe.IE.SelectLists
    				select selectList).toList();
    	}
 
    	public static string id(this SelectList selectList)
    	{
    		return (selectList != null)
    					? selectList.Id
    					: "";
    	}
 
    	public static List<string> ids(this List<SelectList> selectLists)
    	{
    		return (from selectList in selectLists 
    				select selectList.id()).toList();
    	}
 
    	public static List<Option> options(this SelectList selectList)
    	{
    		return (from option in selectList.Options 
    				select option).toList();
    	}
 
    	public static Option select(this Option option)
    	{
    		try
    		{
    			if (option != null)
					option.Select();
			}
			catch(Exception ex)
			{
				ex.log("in Option select");
			}
			return option;
    	}
 
    	public static SelectList select(this SelectList selectList, int index)
    	{
    		var options = selectList.options();
    		if (index < options.size())
    			options[index].select();
    		return selectList;
    	}
    }
    
    public static class WatiN_IE_ExtensionMethods_CheckBox
    {
 
    	public static WatiN.Core.CheckBox checkBox(this WatiN_IE watinIe, string name)
    	{
    		//watinIe.textFields();   // after some events 
    		foreach(var checkBox in watinIe.checkBoxes())
    			if (checkBox.id() == name) // || checkBox.title() == name)
    				return checkBox;
    		"in WatiN_IE could not find CheckBox with name:{0}".error(name ?? "[null value]");
    		return null;    				
    	}
 
    	public static List<WatiN.Core.CheckBox> checkBoxes(this WatiN_IE watinIe)
    	{
    		return (from checkBox in watinIe.IE.CheckBoxes
    				select checkBox).toList();
    	}
 
    	public static string id(this WatiN.Core.CheckBox checkBox)
    	{
    		return (checkBox != null)
    					? checkBox.Id
    					: "";
    	}
 
    	public static List<string> ids(this List<WatiN.Core.CheckBox> checkBoxes)
    	{
    		return (from checkBox in checkBoxes 
    				select checkBox.id()).toList();
    	}
 
    	public static bool value(this WatiN.Core.CheckBox checkBox)
    	{    		
    		return (checkBox != null)
    					? checkBox.Checked
    					: false;
    	}
 
    	public static List<bool> values(this List<WatiN.Core.CheckBox> checkBoxes)
    	{
    		return (from checkBox in checkBoxes 
    				select checkBox.value()).toList();
    	}
 
    	public static WatiN.Core.CheckBox value(this WatiN.Core.CheckBox checkBox, bool value)
    	{
    		if (checkBox!= null)    
    		try
    		{
    			checkBox.Checked = value;    	
    		}
    		catch(Exception ex)
    		{
    			ex.log("in WatiN.Core.CheckBox value");
    		}
    		return checkBox;
    	}
 
    	public static WatiN.Core.CheckBox check(this WatiN.Core.CheckBox checkBox)
    	{    		
    		return checkBox.value(true);
    	}
 
    	public static WatiN.Core.CheckBox uncheck(this WatiN.Core.CheckBox checkBox)
    	{    		
    		return checkBox.value(false);
    	}
 
 	}
 	
 	
 	public static class WatiN_IE_ExtensionMethods_RadioButton
    {
 
    	/*public static WatiN.Core.RadioButton radioButton(this WatiN_IE watinIe, string id)
    	{
    		//watinIe.textFields();   // after some events 
    		foreach(var radioButton in watinIe.radioButtons())
    			if (radioButton.id() == id || radioButton.title() == id)
    				return radioButton;
    		"in WatiN_IE could not find RadioButton with id or name:{0}".error(name ?? "[null value]");
    		return null;    				
    	}*/
 
    	public static List<WatiN.Core.RadioButton> radioButtons(this WatiN_IE watinIe)
    	{
    		return (from radioButton in watinIe.IE.RadioButtons
    				select radioButton).toList();
    	}
    	
    	public static List<WatiN.Core.RadioButton> radioButtons(this WatiN_IE watinIe, string name)
    	{
    		return (from radioButton in watinIe.IE.RadioButtons
    				where (radioButton.name() == name)
    				select radioButton    				
    				).toList();
    	}
    	    	
 
    	public static string id(this WatiN.Core.RadioButton radioButton)
    	{
    		return (radioButton != null)
    					? radioButton.Id
    					: "";
    	}
 
    	public static List<string> ids(this List<WatiN.Core.RadioButton> radioButtons)
    	{
    		return (from radioButton in radioButtons 
    				select radioButton.id()).toList();
    	}
    	
    	public static string name(this WatiN.Core.RadioButton radioButton)
    	{
    		return (radioButton != null)
    					? radioButton.attribute("name")
    					: "";
    	}
 
    	public static List<string> names(this List<WatiN.Core.RadioButton> radioButtons)
    	{
    		return (from radioButton in radioButtons 
    				select radioButton.name()).Distinct().toList();
    	}
 
 		public static List<WatiN.Core.RadioButton> withName(this List<WatiN.Core.RadioButton> radioButtons, string name)
    	{
    		return (from radioButton in radioButtons 
    				where (radioButton.name() == name)
    				select radioButton ).toList();
    	}
    	
    	public static WatiN.Core.RadioButton withValue(this List<WatiN.Core.RadioButton> radioButtons, string value)
    	{
    		foreach(var radioButton in radioButtons)
    			if (radioButton.value().trim()==value)
    				return radioButton;
    		return null;
    	}
    	
    	public static string value(this WatiN.Core.RadioButton radioButton)
    	{    		
    		return (radioButton != null)
    					? radioButton.TextAfter
    					: null;
    	}
    	
    	
 
    	public static List<string> values(this List<WatiN.Core.RadioButton> radioButtons)
    	{
    		return (from radioButton in radioButtons 
    				select radioButton.value()).toList();
    	}
 		 		
 		
    	public static WatiN.Core.RadioButton check(this WatiN.Core.RadioButton radioButton, bool value)
    	{
    		if (radioButton!= null)    
    		try
    		{
    			radioButton.Checked = value;    	
    		}
    		catch//(Exception ex)
    		{    			
    			//ex.log("in WatiN.Core.RadioButton value::");  // there is an internal WatiN exception that occurs after the value is set
    		}
    		return radioButton;
    	}
 
    	public static bool @checked(this WatiN.Core.RadioButton radioButton)
    	{
    		return radioButton.Checked;
    	}
    	
    	public static WatiN.Core.RadioButton @checked(this WatiN.Core.RadioButton radioButton, bool value)
    	{
    		return radioButton.check(value);
    	}
    	
    	/*public static WatiN.Core.RadioButton check(this WatiN.Core.RadioButton radioButton)
    	{    		
    		return radioButton.value(true);
    	}
 
    	public static WatiN.Core.RadioButton uncheck(this WatiN.Core.RadioButton radioButton)
    	{    		
    		return radioButton.value(false);
    	}*/
 
 	}
 	
    public static class WatiN_IE_ExtensionMethods_TextField
    {	
    	public static TextField field(this WatiN_IE watinIe, string name)
    	{
    		return watinIe.textField(name);
    	}
 
    	public static List<TextField> fields(this WatiN_IE watinIe)
    	{
    		return watinIe.textFields();
    	}
 
    	public static bool hasField(this WatiN_IE watinIe, string name)
    	{
    	   return watinIe.textFieldExists(name);
    	}
 
    	public static TextField textField(this WatiN_IE watinIe, string name)
    	{
    		//watinIe.textFields();   // after some events     		
    		foreach(var textField in watinIe.textFields())
    			if (textField.name() == name || textField.title() == name || textField.id() == name)
    				return textField;
    		"in WatiN_IE could not find TextField with name:{0}".error(name ?? "[null value]");
    		return null;    				
    	}
 
    	public static bool textFieldExists(this WatiN_IE watinIe, string name)
    	{
    		return watinIe.textField(name).notNull();    		
    	}
    	
    	
    	public static TextField waitForField(this WatiN_IE watinIe, string nameOrId)
		{
			return watinIe.waitForField(nameOrId, 500, 10);
		}
		
		public static TextField waitForField(this WatiN_IE watinIe, string nameOrId, int sleepMiliseconds, int maxSleepTimes)
		{
		
			var count = 0;
			while(watinIe.hasField(nameOrId).isFalse())
			{
				if (count++ >=maxSleepTimes)
					break;
				watinIe.sleep(500, false);
			}
			return watinIe.field(nameOrId);
		}
    	
    	public static List<TextField> textFields(this WatiN_IE watinIe)
    	{
    		return (from textField in watinIe.IE.TextFields
    				select textField).toList();
    	}
 
    	public static string name(this TextField textField)
    	{
    		return (textField != null)
    					? textField.Name
    					: "";
    	}
 
    	public static List<string> names(this List<TextField> textFields)
    	{
    		return (from textField in textFields 
    				select textField.name()).toList();
    	}
 
    	public static string value(this TextField textField)
    	{    		
    		return (textField != null)
    					? textField.Value
    					: "";
    	}
 
    	public static List<string> values(this List<TextField> textFields)
    	{
    		return (from textField in textFields 
    				select textField.value()).toList();
    	}
 		
 		public static TextField equals(this TextField textField, string value)
 		{
 			if (textField!= null)    		
    			textField.Value = value;    	
    		return textField;
 		}
  	
    	public static TextField value(this TextField textField, string value)
    	{
    		if (textField!= null)    		
    			textField.Value = value;    	
    		return textField;
    	}
 
 		public static List<string> texts(this List<TextField> textFields)
		{
			return (from textField in textFields
					select textField.text()).toList();
		}
 
		public static List<TextField> texts(this List<TextField> textFields, string text)
		{
			return (from textField in textFields
					where textField.text() == text
					select textField).toList();
		}
		public static TextField appendLine(this TextField textField, string textToAppend)
		{
			return textField.appendText(textToAppend.line());
		}
 
		public static TextField appendText(this TextField textField, string textToAppend)
		{
			if (textField!= null)
			{
				textField.value(textField.value() + textToAppend);
			}
			return textField; 
		}
 
		public static WatiN_IE set_Value(this WatiN_IE watinIe, string textFieldId, string text)
		{
			return watinIe.value(textFieldId, text);
		}
 
 		public static WatiN_IE field(this WatiN_IE watinIe, string textFieldId, string text)
 		{
 			return watinIe.value(textFieldId, text);
 		}
 		
 		
		public static WatiN_IE value(this WatiN_IE watinIe, string textFieldId, string text)
		{
			var textField = watinIe.textField(textFieldId);
			if (textField != null)
				textField.value(text);
			else
				"in WatiN_IE value, could not find textField with id: {0}".error(text);
			return watinIe;
 
		}
		public static bool enabled(this TextField field)
		{
			return !(bool)field.htmlElement().prop("disabled");
		}
		public static TextField enabled(this TextField field, bool value)
		{			
			
			Reflection_ExtensionMethods_Properties
					 .prop(field.htmlElement(),
						   "disabled",
						   ! value);
			return field;			
		}		
    }
    
    public static class WatiN_IE_ExtensionMethods_Forms
    {
 
    	public static List<WatiN.Core.Form> forms(this WatiN_IE watinIe)
    	{
    		return (from form in watinIe.IE.Forms
    				select form).toList();
    	}
 
 
    }
    
    public static class WatiN_IE_ExtensionMethods_Elements
    {
 
    	public static List<Element> elements(this WatiN_IE watinIe, string tagName)
    	{
    		return (from element in watinIe.IE.Elements
    				where element.TagName == tagName
    				select element).toList();
    	}
 
    	public static List<Element> elements(this WatiN_IE watinIe)
    	{
    		return (from element in watinIe.IE.Elements
    				select element).toList();
    	} 		 		
 		
 		public static List<Element> elements(this List<Element> elements, string tagName)
    	{
    		return (from element in elements
    				where element.TagName == tagName
    				select element).toList();
    	}
    	public static List<string> tagNames(this List<Element> elements)
    	{    		
    		return (from element in elements
    				select element.TagName).Distinct().toList();
    	}
 
 		public static Dictionary<string, List<Element>> indexedByTagName(this List<Element> elements)
 		{
 			return elements.indexedByTagName(true);
 		}
 		
 		public static Dictionary<string, List<Element>> indexedByTagName(this List<Element> elements, bool includeEmptyFields)
    	{
    		var result = new Dictionary<string,List<Element>>();
    		foreach(var element in elements)
    			if (includeEmptyFields || element.str().valid())
    				result.add(element.TagName, element);
    		return result;
    	}     	
  
    	public static string tagName(this Element element)
    	{
    		return element.TagName;
    	}
    	
    	public static string id(this Element element)
    	{
    		return (element != null)
    					? element.Id
    					: "";
    	}
    	
    	public static string className(this Element element)
    	{
    		return (element != null)
    					? element.ClassName
    					: "";
    	}
 
    	public static string text(this Element element)
    	{
    		return (element != null)
    					? element.Text
    					: "";
    	}
 
    	public static string title(this Element element)
    	{
    		return (element != null)
    					? element.Title
    					: "";
    	}
 
    	public static string innerHtml(this Element element)
    	{
    		return (element != null)
    					? element.InnerHtml
    					: "";
    	}
 
    	public static string outerHtml(this Element element)
    	{
    		return (element != null)
    					? element.OuterHtml
    					: "";
    	}
 
    	public static string html(this Element element)
    	{
    		return element.outerHtml();
    	}
    	
    	public static IHTMLElement htmlElement(this Element element)
    	{
    		return (IHTMLElement) element.HTMLElement;
    	}    	   	
 
    	public static void remove(this Element element)
    	{
    		element.outerHtml("");	
    	}
 
    	public static void remove(this List<Element> elements)
    	{
    		foreach(var element in elements)
    			element.outerHtml("");	
    	}
 
    	public static List<T> outerHtml<T>(this List<T> elements, string outerHtml)
    		where T : Element
    	{
    		foreach(var element in elements)
    			element.outerHtml(outerHtml);	
    		return elements;
    	}
 
    	public static List<T> innerHtml<T>(this List<T> elements, string innerHtml)
    		where T : Element
    	{
    		foreach(var element in elements)
    			element.innerHtml(innerHtml);	
    		return elements;
    	}
 
    	public static T outerHtml<T>(this T element, string outerHtml)
    		where T : Element
    	{
    		if (element!= null)
    		{
    			var htmlElement = element.htmlElement();
    			if (htmlElement != null)    			
    				htmlElement.outerHTML = outerHtml;    				   			
    		}
    		return element;
    	}
 
    	public static T innerHtml<T>(this T element, string innerHtml)
    		where T : Element
    	{
    		if (element!= null)
    		{
    			var htmlElement = element.htmlElement();
    			if (htmlElement != null)    			
    				htmlElement.innerHTML= innerHtml;    				   			
    		}
    		return element;
    	}	
 
    	public static Element @class(this List<Element> elements, string className)
    	{
    		foreach(var element in elements)
				if (element.ClassName == className)
					return element;
			return null;
    	}
    	public static List<Element> @classes(this List<Element> elements, string className)
    	{
    		return (from element in elements
					where (element.ClassName == className)
					select element).toList();
    	}
 
    	public static List<string> classes(this List<Element> elements)
		{
			return (from element in elements
					where (element.ClassName != null)
					select element.ClassName).toList();
		}
 
    	public static List<Element> elements(this IElementsContainer elementsContainer)
    	{
    		return (from element in elementsContainer.Elements
    				select element).toList();
    	}
    	
    	public static List<Element> elements(this IElementsContainer elementsContainer, string tagName)
 		{
 			return elementsContainer.elements().elements(tagName);
 		}
 
    	public static List<T> elements<T>(this WatiN_IE watinIe)
    		where T : Element
		{
			return (from element in watinIe.elements()
					where element is T
					select (T)element).toList();
		}
 
		public static List<string> ids(this List<Element> elements)
		{
			return (from element in elements
					where (element.Id != null)
					select element.Id).toList();
		}
 
		public static Dictionary<string,Element> byId(this List<Element> elements)
		{		
			var result = new Dictionary<string,Element>();
			foreach(var element in elements)
				if (element.Id != null)
					result.add(element.Id, element);
			return result;
		}
 
 
		public static T element<T>(this WatiN_IE watinIe, string id)
			where T : Element
		{
			return watinIe.elements().id<T>(id);
		}
 
		public static Element element(this WatiN_IE watinIe, string id)
		{
			return watinIe.elements().id(id);
		}
 
		public static Element id(this List<Element> elements, string id)
		{
			foreach(var element in elements)
				if (element.Id != null && element.Id == id)
					return element;
			return null;
		}
 
		public static List<Element> texts(this List<Element> elements, string text)
		{
			return elements.texts(text,false);
		}
 
		public static List<Element> texts(this List<Element> elements, string text, bool useRegEx)
		{
			if (useRegEx)
				return (from element in elements
						where element.text().regEx(text)
						select element).toList();
			else
				return (from element in elements
						where element.text() == text
						select element).toList();
		}
 
		public static Element text(this List<Element> elements, string text)
		{
			foreach(var element in elements)
				if (element.Id != null && element.text() == text)
					return element;
			return null;
		}
 
		public static T id<T>(this List<Element> elements, string id)
			where T : Element
		{
			var element = elements.id(id);
			if (element is T)
				return (T)element;
			return null;			
		}

 		public static List<String> strs(this List<Element> elements)
    	{
    		return (from element in elements
    				where element.str().valid()
    				select element.str()).toList();    
    	}
    	
    	public static Element str(this List<Element> elements, string text)
		{
			foreach(var element in elements)
				if (element.str() == text)
					return element;
			return null;
		}
 
 		public static List<IHTMLDOMAttribute> attributesRaw(this Element element)
 		{ 
			var domAttributes = new List<IHTMLDOMAttribute>();  
			if (element.notNull() && element.htmlElement() is IHTMLDOMNode)
			{						
				var domNode = (IHTMLDOMNode)element.htmlElement();
				foreach(var attribute in (IHTMLAttributeCollection)domNode.attributes)
				{		
					var domAttribute = attribute as IHTMLDOMAttribute; 
					if (domAttribute.notNull() && domAttribute.specified)																	
							domAttributes.Add(domAttribute);
				}						
			}
			return domAttributes; 
		}

		public static Dictionary<string,string> attributes(this Element element)
		{
			var attributeValues = new Dictionary<string,string>();
			foreach(IHTMLDOMAttribute attribute in attributesRaw(element)) 
				attributeValues.add(attribute.nodeName.str(), attribute.nodeValue.str());
			return attributeValues;
		}
		
		public static string attribute(this Element element, string attributeName)
		{
			var attributes = element.attributes();
			if (attributes.hasKey(attributeName))
				return attributes[attributeName];
			return "";
		}
    }
    
    public static class WatiN_IE_ExtensionMethods_Divs
    { 
 
    	public static Div div(this WatiN_IE watinIe, string idOrTitle)
    	{
    		foreach(var div in watinIe.divs())
    			if ((div.Id != null && div.Id == idOrTitle) || 
    				(div.Title != null && div.Title ==idOrTitle))
    				return div;
    		return null;
    	}
    	public static List<Div> divs(this WatiN_IE watinIe)
    	{
    		return (from div in watinIe.IE.Divs
    				select div).toList();
    	}
 
    	public static List<string> ids(this List<Div> divs)
    	{
 
    		return (from div in divs 
    				where div.Id != null
    				select div.Id).toList();
    	}
 	}    	     	
 
    public static class WatiN_IE_ExtensionMethods_Captcha
    {
 
    	public static string resolveCaptcha(this WatiN_IE watinIe, string captchaImageUrl)
    	{
    		return ascx_CaptchaQuestion.askQuestion(captchaImageUrl);
    	}
 
    	public static string resolveCaptcha(this WatiN_IE watinIe, TextField textField)
    	{
    		return watinIe.resolveCaptcha(textField.value());
    	}
 
    	public static WatiN_IE resolveCaptcha(this WatiN_IE watinIe, string questionField, string answerField)
    	{
    		var questionUrl = watinIe.textField(questionField).value();
    		if (questionUrl.valid())
    		{
    			var captchaAnswer = watinIe.resolveCaptcha(questionUrl);
    			watinIe.textField(answerField).value(captchaAnswer);
    		}
			return watinIe;    		
    	}    	
 	}
 	
 	public static class WatiN_IE_ExtensionMethods_AskUser
    {
    	public static string askUserQuestion(this WatiN_IE watinIe, string question, string title, string defaultValue)
    	{
    		var assembly =  "Microsoft.VisualBasic".assembly();
			var intercation = assembly.type("Interaction");
 
			var parameters = new object[] {question,title,defaultValue,-1,-1}; 
			return intercation.invokeStatic("InputBox",parameters).str(); 
    	}
 
    	// user interaction 
 
    	public static WatiN_IE askUserToContinue(this WatiN_IE watinIe)
    	{
    		MessageBox.Show("Click OK to Continue the WatiN IE workflow", "O2 Message",MessageBoxButtons.OK, MessageBoxIcon.Question); 
    		return watinIe;
    	}
 
    	public static ICredential askUserForUsernameAndPassword(this WatiN_IE watinIe)
    	{
    		return watinIe.askUserForUsernameAndPassword("");
    	}
    	public static ICredential askUserForUsernameAndPassword(this WatiN_IE watinIe, string loginType)
    	{
		   	var credential = ascx_AskUserForLoginDetails.ask();
		   	if (loginType.valid())
			   	credential.CredentialType = loginType;
		   	return credential;
	    }
    
    }
    
    public static class WatiN_IE_ExtensionMethods_WinForms
    {
 
    	public static Panel showElementsInTreeView(this WatiN_IE watinIe)
    	{
    		var hostPanel = O2Gui.open<Panel>("WatiN element details",400,400);
			var controls = hostPanel.add_1x1("Html elements", "Propeties");
			var propertyGrid = controls[1].add_PropertyGrid();
			controls[0].add_TreeView()
					   .add_Nodes(watinIe.elements().indexedByTagName())
					   .sort()
					   .showSelection()
					   .beforeExpand<List<Element>>(
					  		(treeNode, elements) => 
					  					{
					  						try { treeNode.add_Nodes(elements);}
					  						catch(Exception ex) { ex.log("in beforeExpand<List<Element>>");}
					  					})
					   .afterSelect<Element>((element)=> propertyGrid.show(element))
					   .afterSelect<List<Element>>((elements)=> propertyGrid.show(elements[0]));
    		return hostPanel;
    	}
    	// Control Extensionmethods
 
    	public static WatiN_IE add_IE(this Control control)
    	{
            try
            {
                return WatiN_IE.window(control);
            }
            catch (Exception ex)
            {
                ex.log();
                return null;
            }
    	}
    	
    	public static WatiN_IE add_IE_with_NavigationBar(this Control control)
    	{            
    		var watinIe = control.add_IE();
    		watinIe.add_NavigationBar(control);
    		return watinIe;
    	}
    	
    	
    	public static WatiN_IE add_NavigationBar(this WatiN_IE watinIe, Control control)
    	{
            if (watinIe.isNull())
                return watinIe;
    		var urlTextBox = control.insert_Above(20)
    								 .add_TextBox("Url:","")
    								 .onEnter((text)=> watinIe.open_ASync(text));
			watinIe.onNavigate((url)=> urlTextBox.set_Text(url));
			return watinIe;
    	}
 
 		public static WatiN_IE minimized(this WatiN_IE watinIe)
 		{
 			watinIe.HostControl.minimized();
 			return watinIe;
 		}
 		
 		public static WatiN_IE maximized(this WatiN_IE watinIe)
 		{
 			watinIe.HostControl.maximized();
 			return watinIe;
 		}
     }
     
    public static class WatiN_IE_ExtensionMethods_Highlight
    {
    	public static WatiN_IE enableFlashing(this WatiN_IE watinIe)
    	{
    		WatiN_IE.FlashingEnabled = true;
    		return watinIe;
    	}
    	
    	public static WatiN_IE disableFlashing(this WatiN_IE watinIe)
    	{
    		WatiN_IE.FlashingEnabled = false;
    		return watinIe;
    	}
    	
    	public static T flash<T>(this T element)
		where T : Element
		{
			return element.flash(WatiN_IE.FlashingCount);
		}
 
		public static T flash<T>(this T element, int timesToFlash)
			where T : Element
		{
			try
			{				
				if (WatiN_IE.FlashingEnabled)
				{
					if (WatiN_IE.ScrollOnFlash)
						element.scrollIntoView();				
					element.Flash(timesToFlash);
				}
			}
			catch(Exception ex)
			{
				ex.log("in WatiN Element flash");
			}
			return element;
		}
 
		public static T select<T>(this T element)
			where T : Element
		{
			return element.highlight();			
		}
 
		public static T highlight<T>(this T element)
			where T : Element
		{
			try
			{
				element.Highlight(true);
			}
			catch(Exception ex)
			{ 
				ex.log("in WatiN Element highlight");
			}
			return element;
 
		}
		
		public static T scrollIntoView<T>(this T element)
			where T : Element
		{
			try
			{
				var htmlElement= element.htmlElement();
				htmlElement.scrollIntoView(null);
			}
			catch(Exception ex)
			{
				ex.log("in WatiN scrollIntoView");
			}
			return element;
		}
    
    }
     
    public static class WatiN_IE_ExtensionMethods_Screenshot
    {
    	public static string screenshot(this WatiN_IE watinIe)
    	{
    		if (watinIe.InternetExplorer.isNull())
    		{
    			"Screenshots can only be taken when IE is in a separate process".error();
    			return null;
    		}
    		else
    		{
    			"taking screenshot".info();
    			var targetFile = PublicDI.config.getTempFileInTempDirectory(".jpg"); 
				"JPG File: {0}".info(targetFile);
				watinIe.IE.CaptureWebPageToFile(targetFile); 
				return targetFile;
			}    		
    	}
    }
    
    public static class WatiN_IE_ExtensionMethods_TestRecorder
    {
    
    	public static WatiN_IE startRecorder(this WatiN_IE watinIe)
    	{
    		return watinIe.testRecorder(true);
    	}
    	
    	public static WatiN_IE testRecorder(this WatiN_IE watinIe)
    	{
    		return watinIe.testRecorder(true);
    	}
    	
    	public static WatiN_IE testRecorder(this WatiN_IE watinIe, bool executeInNewProcess)
    	{
    		if (executeInNewProcess)
    			Processes.startProcess("Test Recorder");
    		else
    		{
    			O2Thread.staThread(()=>{ "Test Recorder".assembly()
    													.type("Program")
    													.invokeStatic("Main", new string[] {});
    									});
	
    		}
    		return watinIe;
    	}
    }
    
    public static class WatiN_IE_ExtensionMethods_Events
    {
    	public static WatiN_IE onNavigate(this WatiN_IE ie, MethodInvoker callback, string expectedPage)
    	{
    		(ie.IE.InternetExplorer as DWebBrowserEvents2_Event).NavigateComplete2 += 
 				(object pDisp, ref object url)=>
 					{
 						if (url.str() == expectedPage) 						
	 						O2Thread.mtaThread(()=>callback());
	 				};
 			return ie;
    	}
   
   		public static WatiN_IE onNavigate(this WatiN_IE ie, MethodInvoker callback)
    	{
    		(ie.IE.InternetExplorer as DWebBrowserEvents2_Event).NavigateComplete2 += 
 				(object pDisp, ref object url)=> O2Thread.mtaThread(()=>callback());
 			return ie;
    	}
    	
    	public static WatiN_IE onNavigate(this WatiN_IE ie, Action<string> callback)
    	{
    		(ie.IE.InternetExplorer as DWebBrowserEvents2_Event).NavigateComplete2 += 
 				(object pDisp, ref object url)=> 
 					{
 						var pageUrl = url.str(); // need to pin down this value
 						O2Thread.mtaThread(()=>callback(pageUrl));
 					};
 			return ie;
    	}
    	
    	public static WatiN_IE onNavigate(this WatiN_IE ie, Action<IWebBrowser2, string> callback)
    	{
    		(ie.IE.InternetExplorer as DWebBrowserEvents2_Event).NavigateComplete2 += 
 				(object pDisp, ref object url)=>
 					{ 						
 						if (pDisp is IWebBrowser2 && url is string)
 						{	
 							var pageUrl = url.str(); // need to pin down this value
 							O2Thread.mtaThread(
 								()=> callback(pDisp as IWebBrowser2, pageUrl));	
 						}
 					};
 			return ie;
    	}
    	    
    	public static WatiN_IE beforeNavigate(this WatiN_IE ie, Func<string,bool> callback)
    	{
    		(ie.IE.InternetExplorer as DWebBrowserEvents2_Event).BeforeNavigate2 += 
 				//(object pDisp, ref object url)
 				(object pDisp,  ref object URL, ref object Flags,  ref object TargetFrameName, ref object PostData, ref object Headers, ref bool Cancel)=>
 					{ 						
 						//"in beforeNavigate of url:{0} -> h: {1}".info(URL, Headers);
 						Cancel = callback(URL.str()); 						
 					};
 			return ie;
    	}
    }
    
    
    public static class WatiN_IE_ExtensionMethods_Javascript
    {
    	
    	public static object invokeScript(this WatiN_IE ie, string functionName)
    	{
    		return ie.invokeScript(functionName, null);
    	}
    	
    	public static object invokeScript(this WatiN_IE ie, string functionName, params object[] parameters)
    	{
    		//"[WatiN_IE] invokeScript '{0}' with parameters:{1}".info(functionName ,parameters.size());
    		return ie.invokeScript(true, functionName, parameters);
    	}	
    		
    	public static object invokeScript(this WatiN_IE ie, bool waitForExecutionComplete, string functionName, params object[] parameters)
    	{
    		var sync = new AutoResetEvent(false);
    		object responseValue = null;
    		ie.WebBrowser.invokeOnThread(
    								()=>{
    										var document = ie.WebBrowser.Document;
    										if (parameters.isNull())
												responseValue = document.InvokeScript(functionName); 
											else
												responseValue = document.InvokeScript(functionName, parameters); 
											sync.Set();	
										});
    		if (waitForExecutionComplete)
    			sync.WaitOne();
    		return responseValue;	
    	}
    	
    	public static object invokeEval(this WatiN_IE ie, string evalScript)
    	{
    		var evalParam = "(function() { " + evalScript + "})();";
    		//"[WatiN_IE] invokeEval evalParam: {0}".debug(evalParam);
    		return ie.invokeScript("eval", evalParam);   
    	}
    	public static WatiN_IE.ToCSharp injectJavascriptFunctions(this WatiN_IE ie)
    	{
    		return ie.injectJavascriptFunctions(false);
    	}
    	
    	public static WatiN_IE.ToCSharp injectJavascriptFunctions(this WatiN_IE ie, bool resetHooks)
    	{
    		if (ie.WebBrowser.isNull())
    			"in InjectJavascriptFunctions, ie.WebBrowser was null".error();
    		else
    		{
    			if (ie.WebBrowser.ObjectForScripting.isNull() || resetHooks)  
    			{
    				ie.WebBrowser.ObjectForScripting = new WatiN_IE.ToCSharp();
    				
	    			"Injecting Javascript Hooks * Functions for page: {0}".debug(ie.url());
					ie.eval("var o2Log = function(message) { window.external.write(message) };");
					ie.invokeScript("o2Log","Test from Javascript (via toCSharp(message) )");
					ie.eval("$o2 = window.external");
					"Injection complete (use o2Log(...) or $o2.write(...)  to talk back to O2".info();
					return (ie.WebBrowser.ObjectForScripting as WatiN_IE.ToCSharp);
				}
				else 
				{
					if((ie.WebBrowser.ObjectForScripting is WatiN_IE.ToCSharp))
						return (ie.WebBrowser.ObjectForScripting as WatiN_IE.ToCSharp);
					else
						"in WatiN_IE injectJavascriptFunctions, unexpected type in ie.WebBrowser.ObjectForScripting: {0}".error(ie.WebBrowser.ObjectForScripting.typeName());					
				}
						
			}
			return null;
    	}
    	
    	public static object downloadAndExecJavascriptFile(this WatiN_IE ie, string url)
    	{
    		"[WatiN_IE] downloadAndExecJavascriptFile: {0}".info(url);
    		var javascriptCode = url.uri().getHtml();
    		if (javascriptCode.valid())
    			ie.eval(javascriptCode);
    		return ie;
    	}
    	    	
    	public static WatiN_IE injectJavascriptFunctions_onNavigate(this WatiN_IE ie)
    	{
    		
			ie.onNavigate((url)=> ie.injectJavascriptFunctions());
			return ie;
		}

		public static WatiN_IE setOnAjaxLog(this WatiN_IE ie, Action<string, string,string,string> onAjaxLog)
		{
			(ie.WebBrowser.ObjectForScripting as WatiN_IE.ToCSharp).OnAjaxLog = onAjaxLog;
			return ie;
		}
    
    	public static WatiN_IE eval_ASync(this WatiN_IE ie, string script)
    	{
    		O2Thread.mtaThread(()=> ie.eval(script));
    		return ie;
    	}
    	
    	public static WatiN_IE eval(this WatiN_IE ie, string script)
    	{
    		return ie.eval(script, true);
    	}
    	
    	public static WatiN_IE eval(this WatiN_IE ie, string script, bool waitForExecutionComplete)
    	{
    		var executionThread = O2Thread.staThread(()=> ie.IE.RunScript(script));			
    		if (waitForExecutionComplete)
    			executionThread.Join();
    		return ie;	
    	}
    	
    	public static WatiN_IE alert(this WatiN_IE ie, string alertScript)
    	{
    		return ie.eval("alert({0});".format(alertScript));
    	}
    	
    	public static object getJsObject(this WatiN_IE ie)
    	{
    		var toCSharpProxy = ie.injectJavascriptFunctions();
    		if (toCSharpProxy.notNull())
    			return toCSharpProxy.getJsObject();
    		return null;    	
    	}
    	
    	public static T getJsObject<T>(this WatiN_IE ie, string jsCommand)
    	{
    		var jsObject = ie.getJsObject(jsCommand);
    		if (jsObject is T)
    			return (T)jsObject;
    		return default(T);
    	}
    	
    	public static bool doesJsObjectExists(this WatiN_IE ie, string jsCommand)
    	{
    		var toCSharpProxy = ie.injectJavascriptFunctions();
    		if (toCSharpProxy.notNull())
    		{
    			var command = "window.external.setJsObject(typeof({0}))".format(jsCommand);
    			ie.invokeEval(command);
    			ie.remapInternalJsObject();    			
    			return toCSharpProxy.getJsObject().str()!="undefined";
    		}
    		return false;
    	}
    	
    	public static object getJsVariable(this WatiN_IE ie, string jsCommand)
    	{
    		return ie.getJsObject(jsCommand);
    	}
    	
    	public static object getJsObject(this WatiN_IE ie, string jsCommand)
    	{
    		var toCSharpProxy = ie.injectJavascriptFunctions();
    		if (toCSharpProxy.notNull())
    		{
    			var command = "window.external.setJsObject({0})".format(jsCommand);
    			ie.invokeEval(command);
    			ie.remapInternalJsObject();    			
    			return toCSharpProxy.getJsObject();
    		}
    		return null;
    	}    	    	
    	
		public static WatiN_IE remapInternalJsObject(this WatiN_IE ie)
		{		
			//"setting JS _jsObject variable to getJsObject()".info();
    		ie.invokeEval("_jsObject = window.external.getJsObject()"); // creates JS variable to be used from JS
    		return ie;
    	}
    	
    	public static WatiN_IE setJsObject(this WatiN_IE ie, object jsObject)
    	{
    		var toCSharpProxy = ie.injectJavascriptFunctions();
    		if (toCSharpProxy.notNull())    		
    		{
    			toCSharpProxy.setJsObject(jsObject);
    			ie.remapInternalJsObject();
    		}
    		return ie;
    	}
    	
    	public static object waitForJsObject(this WatiN_IE watinIe)
		{
			return watinIe.waitForJsObject(500, 20);
		}
		
		public static object waitForJsObject(this WatiN_IE watinIe, int sleepMiliseconds, int maxSleepTimes)
		{					
			"[WatiN_IE][waitForJsObject] trying to find jsObject for {0} x {1} ms".info(maxSleepTimes, sleepMiliseconds);
			watinIe.setJsObject(null);
			for(var i = 0; i < maxSleepTimes ; i++)
			{
				var jsObject = watinIe.getJsObject();
				if(jsObject.notNull())
				{
					"[watinIe][waitForJsObject] got value: {0} (n tries)".info(jsObject, i);
					return jsObject;
				}
					
				watinIe.sleep(500, false);
			}
			"[WatiN_IE][waitForJsObject] didn't find jsObject after {0} sleeps of {1} ms".error(maxSleepTimes, sleepMiliseconds);
			return null;
		}
		
		public static object waitForJsVariable(this WatiN_IE watinIe, string jsCommand)
		{
			return watinIe.waitForJsVariable(jsCommand,  500, WatiN_IE_ExtensionMethods.WAITFORJSVARIABLE_MAXSLEEPTIMES);
		}
		
		public static object waitForJsVariable(this WatiN_IE watinIe, string jsCommand, int sleepMiliseconds, int maxSleepTimes)
		{	
			"[WatiN_IE][waitForJsVariable] trying to find jsObject called '{0}' for {1} x {2} ms".info(jsCommand, maxSleepTimes, sleepMiliseconds);			
			watinIe.setJsObject(null);
			for(var i = 0; i < maxSleepTimes ; i++)
			{
				if (watinIe.doesJsObjectExists(jsCommand))
				{
					var jsObject = watinIe.getJsObject(jsCommand);
					"[watinIe][waitForJsVariable] got value: {0} ({1} tries)".info(jsObject, i);
					return jsObject;
				}					
				watinIe.sleep(500, false);
			}
			"[WatiN_IE][waitForJsVariable] didn't find jsObject called '{0}' after {1} sleeps of {2} ms".error(jsCommand, maxSleepTimes, sleepMiliseconds);
			return null;
		}
		
		public static WatiN_IE deleteJsVariable(this WatiN_IE watinIe, string jsVariable)
		{
			var evalString = "try { delete " + jsVariable + " } catch(exception) { }";
			watinIe.eval(evalString);
			return watinIe;
		}
		
    	    	
    }
    public static class WatiN_IE_ExtensionMethods_JavaScript_Helpers
    {
    	public static List<string> javascript_ObjectItems(this WatiN_IE ie, string targetObject)
    	{
    		return ie.invokeEval("var result = []; for(var item in " + targetObject + ") result.push(item);return result.toString();").str().split(",");
    	}
    	
    	public static T javascript_VariableValue<T>(this WatiN_IE ie, string variableName, string propertyName)
    	{
    		return ie.javascript_VariableValue<T>("{0}.{1}".format(variableName, propertyName));
    	}
    	
    	public static T javascript_VariableValue<T>(this WatiN_IE ie, string variableName)
    	{
    		var result = ie.javascript_VariableValue(variableName);
    		if (result is T)
    			return (T)result;
    		return default(T);
    	}
    	public static object javascript_VariableValue(this WatiN_IE ie, string variableName, string propertyName)
    	{
    		return ie.javascript_VariableValue("{0}.{1}".format(variableName, propertyName));
    	}
    	
    	public static object javascript_VariableValue(this WatiN_IE ie, string variableName)
    	{
    		return ie.invokeEval("return {0}".format(variableName));
    	}
    }
    
    public static class WatiN_IE_ExtensionMethods_Javascript_Format
    {
    
    //using System.Threading
						//var sync = new AutoResetEvent(false);
						//var result = "...";
						//var control = "temp".popupWindow(0,0);
						//O2Thread.staThread(
						///	()=>{ 
									//var control = new Panel();									
									//control.parentForm().sendToBack();									
									//sync.Set();
									//control.parentForm().close();
									//});												
									//sync.WaitOne(2000);
									//"after waitOne()".debug();
		public static string jsFormat(this Control tempHostControl, string codeToFormat)									
		{
			return tempHostControl.formatJsCode(codeToFormat);			
		}
		
		public static string formatJsCode(this Control tempHostControl, string codeToFormat)
		{			
			var ie = tempHostControl.add_IE().silent(true);
			var result = ie.formatJsCode(codeToFormat);
			O2Thread.mtaThread(()=>ie.close());						
			return result;
		}
		
		public static bool js_FunctionExists(this WatiN_IE ie, string functionName)
		{
			ie.injectJavascriptFunctions();
			return (bool)ie.invokeEval("return (typeof {0} == \"function\");".format(functionName));
		}
		
		public static string formatJsCode(this WatiN_IE ie, string codeToFormat)
		{			
			if (ie.url().neq("about:blank"))
			{
				"opening ABOUT:Blank".info();
				ie.open("about:blank");
			}
			else
				"already in ABOUT:Blank".info();
			
			if (ie.js_FunctionExists("js_beautify").isFalse())
			{				
				var jsBeautify = @"beautify.js".local();
				ie.eval(jsBeautify.fileContents()); 	
				if (ie.js_FunctionExists("js_beautify"))
					"Injected beautify.js into about:blank".info();
				else
					"Failed to Inject js_beautify code".error();
			}
			"formating Javascript with size: {0}".info(codeToFormat.size()); 						
			ie.setJsObject(codeToFormat);						
			ie.eval("window.external.setJsObject(js_beautify(_jsObject))"); 
			var result = ie.getJsObject().str().fixCRLF();					
//			"formated Javascript has size: {0}".info(result.size()); 			
			return result;
		}						
		
		public static WatiN_IE show_Formated_Javascript(this string codeToFormat)
		{
			var ie = "Formated Javascript".popupWindow()
										 .add_IE()
										 .show_Formated_Javascript(codeToFormat);
			return ie;
		}
		
		public static WatiN_IE show_Formated_Javascript(this WatiN_IE ie,string codeToFormat)
		{
			return ie.show_Formated_Javascript(null, codeToFormat);
		}
		
		public static WatiN_IE show_Formated_Javascript(this WatiN_IE ie, WatiN_IE temp_ie, string codeToFormat)
		{		
			var prettifyHtml = @"prettify.htm".local();
			if (prettifyHtml.fileExists().isFalse())
				return ie;			
			
			if (ie.url().isNull() || ie.url().contains("prettify.htm").isFalse())
				ie.open(prettifyHtml);  						
			var formatedJsCode = (temp_ie.isNull()) 
									? ie.HostControl.formatJsCode(codeToFormat)
									: temp_ie.formatJsCode(codeToFormat);			
						
			var codeDiv = ie.div("codeDiv");
			codeDiv.innerHtml("<pre id=\"code\" class=\"prettyprint\">{0}</pre>".format(formatedJsCode));			
			ie.invokeScript("prettyPrint"); 			
			return ie;
		}
	}
	
    public static class WatiN_IE_ExtensionMethods_Javascript_GuiViewers
    {
    	public static WatiN_IE view_JavaScriptVariable_AsTreeView(this WatiN_IE ie, string rootVariableName)
    	{
    		var treeView = "Javascript variable: {0}".format(rootVariableName).popupWindow(500,400).add_TreeView();
    		
    		Action<TreeNode,string> add_Object =
				(treeNode, objRef)=>{
										var _jsObject = ie.getJsObject(objRef);							
										if (_jsObject is IEnumerable)
											foreach(var item in _jsObject as IEnumerable)
												treeNode.add_Node(item.comTypeName(), item, true); 
										else
											treeNode.add_Node(_jsObject); 								
									}; 
			 
			treeView.beforeExpand<object>(
				(treeNode, _object) => {
											if (_object is IEnumerable)
												foreach(var item in _object as IEnumerable)
													treeNode.add_Node(item.comTypeName(), item, true); 
											else
											{
												ie.setJsObject(_object);
												foreach(var variableName in ie.javascript_ObjectItems("_jsObject"))
												{
													var variableValue = ie.javascript_VariableValue( "_jsObject.{0}".format(variableName));
													if (variableValue.typeFullName() == "System.__ComObject") 										
														treeNode.add_Node(variableName, variableValue,true);  										
													else
													{
														var nodeText = "{0}: {1}".format(variableName, variableValue);
													//add_Object(treeNode, "_jsObject.{0}".format(item));
														treeNode.add_Node(nodeText);
													}
												}
											}
									   });
						
			add_Object(treeView.rootNode(),rootVariableName);
			return ie;
    	}
    }
    
    
    public static class WatiN_IE_ExtensionMethods_Inject_Html
    {
    	public static Element injectHtml_beforeBegin(this Element element, string htmlToInject)
    	{
    		return element.injectHtml("beforeBegin", htmlToInject);
    	}
    	
    	public static Element injectHtml_afterBegin(this Element element, string htmlToInject)
    	{
    		return element.injectHtml("afterBegin", htmlToInject);
    	}
    	
    	public static Element injectHtml_beforeEnd(this Element element, string htmlToInject)
    	{
    		return element.injectHtml("beforeEnd", htmlToInject);
    	}
    	
    	public static Element injectHtml_afterEnd(this Element element, string htmlToInject)
    	{
    		return element.injectHtml("afterEnd", htmlToInject);
    	}
    	
    	public static Element injectHtml(this Element element, string location, string htmlToInject)
    	{
    		try
    		{
    			element.htmlElement().insertAdjacentHTML(location,htmlToInject);
    		}
    		catch(Exception ex)
    		{
    			ex.log("in WatiN Element injectHtml -> location:{0} payload:{1} ".format(location,htmlToInject));
    		}
    		return element;
    	}
    
    }
    
    public static class WatiN_ASPNET_ExtensionMethods
    {    
    	public static DotNet_ViewState viewState(this WatiN_IE ie)
    	{
    		return new DotNet_ViewState(ie.viewStateRaw());
    	}
    	public static string viewStateRaw(this WatiN_IE ie)
    	{
    		return ie.field("__VIEWSTATE").value();
    	}
    	
    	public static T showViewState<T>(this T control, WatiN_IE ie, bool showDetailedView)
    		where T : System.Windows.Forms.Control
    	{
    		if (showDetailedView)
    			control.showViewState(ie);
    		else
    			control.showViewStateValues(ie);
    		return control;
    	}
    	
    	public static T showViewState<T>(this T control, WatiN_IE ie)
    		where T : System.Windows.Forms.Control
    	{    		    		    		
    		return ie.viewState().show(control);    		
    	}


		public static T showViewStateValues<T>(this T control, WatiN_IE ie)
    		where T : System.Windows.Forms.Control
    	{    		    		    		
    		return ie.viewState().showValues(control);    		
    	}    	    	   	
	}
	
	public static class WatiN_IE_ExtensionMethods_FireBugLite
	{
		public static WatiN_IE inject_FirebugLite(this WatiN_IE ie)
		{			
			var firebugLiteScript = "(function(F,i,r,e,b,u,g,L,I,T,E){if(F.getElementById(b))return;E=F[i+'NS']&&F.documentElement.namespaceURI;E=E?F[i+'NS'](E,'script'):F[i]('script');E[r]('id',b);E[r]('src',I+g+T);E[r](b,u);(F[e]('head')[0]||F[e]('body')[0]).appendChild(E);E=new Image;E[r]('src',I+L);})(document,'createElement','setAttribute','getElementsByTagName','FirebugLite','4','firebug-lite.js','releases/lite/latest/skin/xp/sprite.png','https://getfirebug.com/','#startOpened');";
			ie.eval(firebugLiteScript);  
			"[Injected FirebugLite]".info();
			return ie;
		}
	}
	
	public static class WatiN_IE_ExtensionMethods_JQuery
	{	
		public static WatiN_IE inject_jQuery(this WatiN_IE ie)
		{
			var jQueryFile = "jquery-1.6.2.min.js";
			var jQueryHtml = jQueryFile.local().fileContents();
			if (jQueryHtml.valid())
			{				
				ie.eval(jQueryHtml); 
				"[Injected jQuery]".info();// (jQuery script {0} size: {1}".info(jQueryFile, jQueryHtml.size());
			}
			else
				"[Injecting jQuery] could find local jQuery file: {0}".error(jQueryFile);
			return ie;
		}
			
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
