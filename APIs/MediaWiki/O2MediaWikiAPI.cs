// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Xml.Linq;
using System.Reflection;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.Views.ASCX;
using O2.Views.ASCX.DataViewers;
using O2.Views.ASCX.ExtensionMethods;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using O2.External.IE.Wrapper;
using O2.External.IE.ExtensionMethods;
using O2.XRules.Database.Utils;

//O2Ref:O2_External_IE.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:System.Xml.dll
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll

namespace O2.XRules.Database.APIs
{
    public class O2MediaWikiAPI
    {            
    	public string ApiName { get; set; }
    	public string HostUrl { get; set; }		
    	public string ApiPhp { get; set; }
    	public string IndexPhp { get; set; }
    	public string ReturnFormat { get; set; }
		public string Styles { get; set; }    	
		
		public string Login_Result { get; set; }    	
		public string Login_UserId { get; set; }    	
		public string Login_Username { get; set; }    	
		public string Login_Token { get; set; }    	
		public string Login_CookiePrefix { get; set; }    	
		public string Login_SessionId { get; set; }    	
		public string Login_Cookie { get; set; }	
		public string BasicAuth { get; set; }
		
		public Web LastWebRequest { get; set; }		
		
		public string SEPARATOR_CHAR = "?";
		
		//fetched data cache (only setup when used
		public FileCache WikiContentLocalCache {get;set;}			
		
		
		public O2MediaWikiAPI()
		{
			ApiName = "MediaWiki";
		}
		
    	public O2MediaWikiAPI(string apiPhp)   : this()  		
    	{
    	  	init(apiPhp);
    	}
 
 		public override string ToString()
 		{
 			return ApiName;
 		}
 		
 		public Web web()
 		{
 			LastWebRequest = new Web();
 			if (BasicAuth.valid())
 				LastWebRequest.Headers_Request.add("Authorization","Basic " + BasicAuth);
 			return LastWebRequest;
 		}
    	public void init(string apiPhp)
    	{
    		init(apiPhp.lower().replace("api.php",""), apiPhp,apiPhp.lower().replace("api.php","index.php"));
    	}
    	
    	public void init(string hostUrl, string apiPhp, string indexPhp)
    	{
    		HostUrl = hostUrl;
    		ApiPhp = apiPhp;
    		IndexPhp = indexPhp;
    		ReturnFormat = "xml";
    		Styles = "";
    		//ReturnFormat = "yaml";
    		//ReturnFormat = "json";
    		//ReturnFormat = "php";
    		//ReturnFormat = "wddx";
    		//ReturnFormat = "rawfm";    		
    		//ReturnFormat = "txt";        		
    		//ReturnFormat = "dbg";    
    	}
    	
    	
    	
    	public bool okApiSpec()
    	{
    		return apiSpec() != "";
    	}
    	
    	public string apiSpec()
    	{
            return web().getUrlContents(ApiPhp);
    	}
    	
    	public void format(string returnFormat)
    	{
    		ReturnFormat = returnFormat;
    	}
    			  
		
		public string parsePage_Raw(string page)
		{
			try
			{
				var getData = "action=parse&page={0}&format=xml".format(page);			
				return getApiPhp(getData);
            }
            catch (Exception ex)
            {
                ex.log("in O2MediaWikiAPI.parsePage_Raw");
                return null;
            }
        }

        public string parsePage(string page)
        {
            try
            {
                var data = parsePage_Raw(page);
				var value = data.xRoot().element("parse").element("text").value();
				return value.fixCRLF();				
			}
			catch(Exception ex)
			{
				ex.log("in O2MediaWikiAPI.parsePage");				
			}
            return null;
		}
		
        public string parseText_Raw(string textToParse)
		{
			try
			{
				var getData = "action=parse&text={0}&format=xml".format(textToParse.urlEncode());			
				var data = postApiPhp(getData);				
				if (data.starts("\n"))              // fix prob with some wikis that return a enter at the top
					data = data.trim();
                return data;
            }            
			catch(Exception ex)
			{
				ex.log("in O2MediaWikiAPI.parseText_Raw");
				return null;
			}
		}

		public string parseText(string textToParse)
		{
			try
			{
                var data = parseText_Raw(textToParse);
                if (data != null)
                {
                    var value = data.xRoot().element("parse").element("text").value();
                    return value.fixCRLF();
                }
			}
			catch(Exception ex)
			{
				ex.log("in O2MediaWikiAPI.parseText");				
			}
            return null;
		}
		
		// version of raw(string page) with cache support
		public string raw(string page, bool useCache)
		{
			if (useCache)
			{
				if (WikiContentLocalCache.isNull())
					WikiContentLocalCache = new FileCache("MediaWiki_ContentLocalCache");
				return WikiContentLocalCache.cacheGet(page,()=>{ return this.raw(page);});   
			}
			return this.raw(page);
		}
		
		public string raw(string page)
		{					 	
			var getData = "action=raw&title={0}".format(page.urlEncode());
			"getData: {0}".debug(getData);
			return getIndexPhp(getData).fixCRLF();
			//var getData = "action=raw&title={0}&format=xml".format(page);			
			//var data = get(getData);
			//return data.xRoot();//.element("parse").element("text").value();			
		}
		
		public string getIndexPhp(string getData)
		{			
			try
			{				
				var uri = new Uri("{0}{1}{2}".format(IndexPhp,SEPARATOR_CHAR,getData));		
				return getRequest(uri);
			}			
			catch(Exception ex)
			{
				ex.log("in O2MediaWikiAPI.getIndexPhp");
				return "";
			}
			
		}
		
		public string getApiPhp(string getData)
		{	
			try
			{
				var uri = new Uri("{0}{1}{2}".format(ApiPhp,SEPARATOR_CHAR,getData));		
				return getRequest(uri);
			}			
			catch(Exception ex)
			{
				ex.log("in O2MediaWikiAPI.getApiPhp");
				return "";
			}
		}
		
		public string getRequest(Uri uri)
		{
			try
			{
				"sending GET request with: {0}".format(uri.str()).debug();
				
                var responseData = web().getUrlContents(uri.str(), Login_Cookie, false);
				if (responseData != null && responseData.valid())
					"responseData size: {0}".format(responseData.size()).info(); 
				else
					"invalid response data".error();				
				return responseData.trim();
			}			
			catch(Exception ex)
			{
				ex.log("in O2MediaWikiAPI.get");
				return "";
			}
		}
		
		public string postApiPhp(string postData)
		{
			try
			{
				"sending POST request with: {0}".format(postData).debug();
                return web().getUrlContents_POST(ApiPhp, Login_Cookie, postData);
			}		
			catch(Exception ex)
			{
				ex.log("in O2MediaWikiAPI.postApiPhp");
				return "";
			}
		}
		
		
    }
    
    public static class O2MediaWikiAPI_ExtensionMethods
    {	
    	#region login & security 
    	
        public static bool loggedIn(this O2MediaWikiAPI wikiApi)
        {
            return wikiApi.Login_Result == "Success";
        }

    	public static string editToken(this O2MediaWikiAPI wikiApi, string page)
    	{
    		return wikiApi.token("edit",page);
    	}
    	
    	public static string token(this O2MediaWikiAPI wikiApi, string action, string page)
    	{
    		if (wikiApi.loggedIn().isFalse())
    			return "";
    		var xmlData = wikiApi.getApiPhp("action=query&prop=info&intoken={0}&titles={1}&format=xml".format(action,page));
            var attributeName = "{0}token".format(action);
            return xmlData.xRoot().elementsAll().element("page").attribute(attributeName).value();
    	}
    	
    	public static bool login(this O2MediaWikiAPI wikiApi, string username, string password)
		{
			var postData = @"action=login&lgname={0}&lgpassword={1}&format=xml".format(username,password);			
			var login = wikiApi.postApiPhp(postData).xRoot().element("login");
			wikiApi.Login_Result = login.attribute("result").value();
			var setCookie = "";			
			
			if (wikiApi.Login_Result == "NeedToken")
			{					
				if (wikiApi.LastWebRequest.Headers_Response.hasKey("Set-Cookie"))
				{
					setCookie = wikiApi.LastWebRequest.Headers_Response.value("Set-Cookie");
					wikiApi.Login_Cookie = setCookie;					
				}
				var token=login.attribute("token").value();
				postData = "{0}&lgtoken={1}".format(postData, token);
				login = wikiApi.postApiPhp(postData).xRoot().element("login");
				wikiApi.Login_Result = login.attribute("result").value();				
			}
			
			if (wikiApi.Login_Result == "Success")
			{
				wikiApi.Login_UserId = login.attribute("lguserid").value();
				wikiApi.Login_Username = login.attribute("lgusername").value();
				wikiApi.Login_Token = login.attribute("lgtoken").value();
				wikiApi.Login_CookiePrefix = login.attribute("cookieprefix").value();
				wikiApi.Login_SessionId = login.attribute("sessionid").value();																

				wikiApi.Login_Cookie = "{0}UserName={1};{0}UserID={2};{0}Token={3};{0}_session={4};".format(
										wikiApi.Login_CookiePrefix, wikiApi.Login_Username,wikiApi.Login_UserId,
										wikiApi.Login_Token, wikiApi.Login_SessionId);																		
				wikiApi.Login_Cookie += setCookie;  // append any cookies we got from the NeedToken request
				return true;
			}
			else
			{
				wikiApi.Login_UserId = "";
				wikiApi.Login_Username = "";
				wikiApi.Login_Token = "";
				wikiApi.Login_CookiePrefix = "";
				wikiApi.Login_SessionId = "";
				return false;
			}						
		} 
		
		public static bool login_BasicAuth(this O2MediaWikiAPI wikiApi, string userName, string password)
		{
			wikiApi.Login_UserId = userName;
			wikiApi.Login_Username = userName;
			wikiApi.BasicAuth = "{0}:{1}".format(userName, password).base64Encode();
			var page = wikiApi.raw("Main_Page");			
			var result = page.valid();			
			if (result)
				wikiApi.Login_Result = "Success";
			return result;
		}
    	
    	#endregion
    	
		#region create edit, move, delete pages
		
		
		//note: the createonly option doesn't seem to be working
		public static string createPage(this O2MediaWikiAPI wikiApi,string page, string pageContent)
    	{
    		if (wikiApi.loggedIn().isFalse())
    			return "";
    		
			var urlData = "action=edit&format=xml&createonly&token={0}&title={1}&text={2}".format(wikiApi.editToken(page).urlEncode(),page.urlEncode(),pageContent.urlEncode()); 
			return wikiApi.postApiPhp(urlData);    		
    	}

        public static string save(this O2MediaWikiAPI wikiApi, string page, string pageContent)
        {
            return wikiApi.editPage(page, pageContent);
        }		        

        public static string editPage(this O2MediaWikiAPI wikiApi,string page, string pageContent)
    	{
    		if (wikiApi.loggedIn().isFalse())
    			return "";    		
            try
            {
                var response = wikiApi.editPage(page, pageContent, "", "");
                var result = response.xRoot().element("edit").attribute("result").value();
                if (result == "Failure")
                {
                    "trying to save using captcha".info();
                    var captchaId = response.xRoot().element("edit").element("captcha").attribute("id").value();
                    var captchaQuestion = response.xRoot().element("edit").element("captcha").attribute("question").value();
                    "MediaWiki Captcha Question:{1}".info(captchaId, captchaQuestion);
                    var code = "return {0};".format(captchaQuestion);
                    var assembly = code.compile_CodeSnippet();
                    var captchaAnswer = assembly.executeFirstMethod();
                    "MediaWiki Captcha Answer: {0}".info(captchaAnswer);
                    response = wikiApi.editPage(page, pageContent, captchaId, captchaAnswer.str());                    
                    return response;
                }
            }
            catch (Exception ex)
            {
                ex.log("in O2MediaWikiAPI.save");
            }
            return "";
        }

        public static string editPage(this O2MediaWikiAPI wikiApi, string page, string pageContent, string captchaId, string captchaWord)
        {
	        if (wikiApi.loggedIn().isFalse())
    			return "";
            var urlData = "action=edit&format=xml&nocreate&token={0}&title={1}&text={2}".format(wikiApi.editToken(page).urlEncode(), page.urlEncode(), pageContent.urlEncode());
            if (captchaId.valid() && captchaWord.valid())
                urlData = "{0}&captchaid={1}&captchaword={2}".format(urlData, captchaId, captchaWord);
            return wikiApi.postApiPhp(urlData);
        }

		public static bool exists(this O2MediaWikiAPI wikiApi,string page)
		{
			return wikiApi.raw(page).valid();
		}

		public static bool deletePages(this O2MediaWikiAPI wikiApi, List<string> pagesToDelete)
		{
			var result = true;
			foreach(var pageToDelete in pagesToDelete)			
				if (wikiApi.deletePage(pageToDelete).isFalse())
					result = false;
		return result;			
				
		}
        public static bool deletePage(this O2MediaWikiAPI wikiApi, string pageToDelete)
        {
        	"[O2MediaWikiAPI] deleting page: {0}".info(pageToDelete);
            var deleteToken = wikiApi.token("delete", pageToDelete).urlEncode();
            if (deleteToken.valid())
            {
                var postData = "action=delete&format=xml&token={0}&title={1}".format(deleteToken, pageToDelete.urlEncode());
                var response = wikiApi.postApiPhp(postData);
                if (response.xRoot().elements("error").size() > 0)
                    "error deleting page {0}: {1}".error(pageToDelete, response);
                else
                    return true;
            }
            else
                "in O2MediaWikiAPI.deletePage, it was not possible to get a delete token".error();
            return false;
        }

        public static bool movePage(this O2MediaWikiAPI wikiApi, string fromPage, string toPage)
        {
            return wikiApi.movePage(fromPage, toPage, true);
        }

        public static bool movePage(this O2MediaWikiAPI wikiApi, string fromPage, string toPage, bool deleteFromPage)
        {
            var infoToken = wikiApi.token("edit", fromPage);
            var postData = "action=move&token={0}&from={1}&to={2}&format=xml".format(infoToken.urlEncode(), fromPage.urlEncode(), toPage.urlEncode());
            var response = wikiApi.postApiPhp(postData);
            if (response.xRoot().elements("error").size() ==0)
                if (deleteFromPage)
                    return wikiApi.deletePage(fromPage);
            return false;
        }
		
		#endregion

        #region categories

        public static List<string> categories(this O2MediaWikiAPI wikiApi)
        {
            return wikiApi.categories(false);
        }

        public static List<string> categories(this O2MediaWikiAPI wikiApi, bool autoRemoveCategoryPrefix)
        {
            var categoryString = "Category:";
            var categories = new List<string>();
            var results = wikiApi.categoriesRaw().attributes("title").values();
            if (autoRemoveCategoryPrefix)
            {
                foreach (var category in results)
                    if (category.starts(categoryString))
                        categories.add(category.Substring(categoryString.size()));
                    else
                        categories.add(category);
            }
            else
                categories = results;
            return categories;
        }

        public static List<XElement> categoriesRaw(this O2MediaWikiAPI wikiApi)
        {
            string pages = "";
            string limitVar = "aclimit";
            int limitValue = 200;
            string properyType = "generator";
            string propertyName = "allcategories";
            string continueVarName = "gacfrom";
            string continueValue = "";
            string dataElement = "page";
            int maxItemsToFetch = -1;
            bool resolveRedirects = false;
            //"info&generator=allcategories",""
            return wikiApi.getQueryContinueResults(pages, limitVar, limitValue, properyType,
                                                    propertyName, continueVarName, continueValue,
                                                    dataElement, maxItemsToFetch, resolveRedirects);
        }

        public static List<string> pagesInCategory(this O2MediaWikiAPI wikiApi, string category)
        {
            return wikiApi.pagesInCategory(category,false);
        }

        public static List<string> pagesInCategory(this O2MediaWikiAPI wikiApi, string category, bool autoAddCategoryPrefix)
        {
            if (autoAddCategoryPrefix)
                category = "Category:" + category;
            return wikiApi.pagesInCategoryRaw(category).attributes("title").values();
        }

        public static List<XElement> pagesInCategoryRaw(this O2MediaWikiAPI wikiApi, string category)
        {
            string pages = "&gcmtitle=" + category;
            string limitVar = "cmlimit";
            int limitValue = 200;
            string properyType = "generator";
            string propertyName = "categorymembers";
            string continueVarName = "gcmcontinue";
            string continueValue = "";
            string dataElement = "page";
            int maxItemsToFetch = -1;
            bool resolveRedirects = false;
            //"info&generator=allcategories",""
            return wikiApi.getQueryContinueResults(pages, limitVar, limitValue, properyType,
                                                    propertyName, continueVarName, continueValue,
                                                    dataElement, maxItemsToFetch, resolveRedirects);
        }

        public static List<string> uncategorizedPages(this O2MediaWikiAPI wikiApi)
        {
            var maxToFetchPerRequest = 500;
            var maxItemsToFetch = 5000;
            return wikiApi.getIndexPhp_UsingXPath_AttributeValues("title=Special:UncategorizedPages",
                                                                  "//div[@class='mw-spcontent']//li//a", "",
                                                                  "limit", maxToFetchPerRequest,
                                                                  "offset", maxItemsToFetch);
        }
		
		public static List<string> allImages(this O2MediaWikiAPI wikiApi)
		{			                         
            return wikiApi.allImagesRaw().attributes("url").values();
		}

		public static List<XElement> allImagesRaw(this O2MediaWikiAPI wikiApi)
        {
            string pages = "";
            string limitVar = "ailimit";
            int limitValue = 200;
            string properyType = "list";
            string propertyName = "allimages";
            string continueVarName = "aifrom";
            string continueValue = "";
            string dataElement = "img";
            int maxItemsToFetch = -1;
            bool resolveRedirects = false;
            //"info&generator=allcategories",""
            return wikiApi.getQueryContinueResults(pages, limitVar, limitValue, properyType,
                                                    propertyName, continueVarName, continueValue,
                                                    dataElement, maxItemsToFetch, resolveRedirects);
        }

        #endregion

        #region get wrappers
        public static string html(this O2MediaWikiAPI wikiApi, string page)
    	{
    		return wikiApi.getPageHtml(page);
    	}
		
		public static string htmlRaw(this O2MediaWikiAPI wikiApi, string page)
    	{    	
    		return wikiApi.getIndexPhp("action=render&title={0}".format(page));    		
    	}
		
    	public static string getPageHtml(this O2MediaWikiAPI wikiApi, string page)
    	{    	
    		//var htmlCode = wikiApi.getIndexPhp("action=render&title={0}".format(page));
    		var htmlCode = wikiApi.htmlRaw(page);
    		return wikiApi.wrapOnHtmlPage(htmlCode);
    	}

		public static string id(this O2MediaWikiAPI wikiApi, int id)
		{
			return wikiApi.id(id.str());
		}
		
		public static string id(this O2MediaWikiAPI wikiApi, string id)
    	{
    		return wikiApi.getRevision(id);
    	}    	
    	
		public static string getRevision(this O2MediaWikiAPI wikiApi, string id)
    	{
    		var htmlCode = wikiApi.getIndexPhp("action=render&oldid={0}".format(id));
    		return wikiApi.wrapOnHtmlPage(htmlCode);
    	}    	    	
    	
    	public static string wikiText(this O2MediaWikiAPI wikiApi, string page)
    	{
    		return wikiApi.getPageWikiText(page); 
    	}    	    	
    	
    	public static string getPageWikiText(this O2MediaWikiAPI wikiApi, string page)
    	{
    		return wikiApi.raw(page);
    	}
    	
    	#endregion    	    	
    	
    	#region  action=query&prop=*  - helpers
    	
    	public static string action_query_prop(this O2MediaWikiAPI wikiApi,string query, string pages)
    	{
            return wikiApi.action_query("prop", query, pages);
    		//var urlData = "action=query&prop={0}&titles={1}&format={2}".format(query,pages,wikiApi.ReturnFormat);
    		//return wikiApi.getApiPhp(urlData);
    	}

        public static string action_query(this O2MediaWikiAPI wikiApi, string queryType, string query,string pages)
        {
            var urlData = "action=query&{0}={1}&format={2}".format(queryType, query, wikiApi.ReturnFormat);
            if (pages.valid())
                urlData += "&titles={0}".format(pages);
            
            return wikiApi.getApiPhp(urlData);
        }

        public static List<XElement> getQueryContinueResults(this O2MediaWikiAPI wikiApi, string pages, int rvlimit,
                                                             string propertyName, string continueVarName, string continueValue,
                                                             string dataElement)
        {
            return wikiApi.getQueryContinueResults(pages,"rvlimit", rvlimit, "prop", propertyName, continueVarName,
                                                   continueValue, dataElement, -1, true);
        }

        public static List<XElement> getQueryContinueResults(this O2MediaWikiAPI wikiApi, string pages, string limitVar, int limitValue, 
                                                             string properyType,string propertyName , string continueVarName , string continueValue, 
    														 string dataElement, int maxItemsToFetch, bool resolveRedirects) 
    	{
    		var results = new List<XElement>();
    		var cmd = "{0}&{1}={2}".format(propertyName, limitVar, limitValue);
            if (resolveRedirects)
    		    cmd += "&redirects";		// to automatically resolve redirects
    		if (continueValue != "")
    			cmd += "&{0}={1}".format(continueVarName, continueValue);
            var data = wikiApi.action_query(properyType,cmd, pages).xRoot();
    		if (data == null || data.elements("query-continue").size() == 0)
    			continueValue = "";
    		else
    			continueValue = data.elements("query-continue").element(propertyName).attribute(continueVarName).Value;
    		if (data!= null)
    			results.AddRange(data.elementsAll(dataElement));
    		if (maxItemsToFetch > -1 && maxItemsToFetch < results.size())
            {
                "in O2MediaWikiAPI.getQueryContinueResults, maxItemsToFetch reached ({0}), so stoping recursive fetch".debug(maxItemsToFetch);
                return results;
            }
                
    		//continueValue.error();
    		if (continueValue != "")
                results.AddRange(wikiApi.getQueryContinueResults(pages, limitVar, limitValue, 
                                 properyType, propertyName, 
                                 continueVarName, continueValue, dataElement,
                                 maxItemsToFetch, resolveRedirects));
    								//wikiApi.templates(pages, rvlimit, continueValue)
    							//);
    		return results;    		
    	}
    	
    	#endregion 
    	
    	#region  action=query&prop=*  -specific
    	
    	public static XElement info(this O2MediaWikiAPI wikiApi, string pages)
    	{
    		return wikiApi.action_query_prop("info",pages).xRoot();
    	}

        public static List<string> pages(this O2MediaWikiAPI wikiApi)
        {
            return wikiApi.allPages();
        }

        public static List<string> allPages(this O2MediaWikiAPI wikiApi)
        {
            return wikiApi.allPagesRaw().attributes("title").values();
        }

        public static List<XElement> pagesRaw(this O2MediaWikiAPI wikiApi)
        {
            return wikiApi.allPagesRaw();
        }

        public static List<XElement> allPagesRaw(this O2MediaWikiAPI wikiApi)
        {
            var propertyName = "allpages";
            var continueVarName = "apfrom";
            var dataElement = "p";
            var rvlimit = 500;          
            var response = wikiApi.getQueryContinueResults("", "aplimit", rvlimit, "list", propertyName, continueVarName, "", dataElement, -1, false);
            return response;
            //response.size().str().info();
            //return response.attibute("title");
        }
        
        public static List<string> pages(this O2MediaWikiAPI wikiApi, string withTitle)
        { 
        	var result =  wikiApi.getApiPhp("action=query&list=search&srwhat=text&srsearch={0}&format=xml&srlimit=200".format(withTitle));
        	return result.xRoot().elementsAll("p").attributes("title").values().sort();
    	}
    	public static List<string> revisions(this O2MediaWikiAPI wikiApi, string page)
    	{
			var revisions = from revision in wikiApi.revisionsRaw(page)
			   		        select (revision.attribute("revid").value() + " - " + 
				   		            revision.attribute("user").value() + " - " + 
				   		            revision.attribute("timestamp").value());    	
			return revisions.ToList();    	
    	}
    	
    	public static List<string> revisionsIds(this O2MediaWikiAPI wikiApi, string page)
    	{
			var revisions = from revision in wikiApi.revisionsRaw(page)
			   		        select (revision.attribute("revid").value());
			return revisions.ToList();    	
    	}
    	
    	public static List<string> revisionsPages(this O2MediaWikiAPI wikiApi, string page)
    	{
    		var pages = new List<string>();
			foreach(var id in wikiApi.revisionsIds(page))
				pages.add(wikiApi.getRevision(id));
			return pages;
    	}
    	
    	public static List<XElement> revisionsRaw(this O2MediaWikiAPI wikiApi, string page)    	
    	{
    		var propertyName = "revisions";
    		var continueVarName = "rvstartid";    			
    		var dataElement = "rev";    		
			var rvlimit = 100;
			
    		return wikiApi.getQueryContinueResults(page, rvlimit, propertyName, continueVarName, "", dataElement);  
    	}
    	
    	public static string diff_Current(this O2MediaWikiAPI wikiApi, string page)
    	{
    		var requestData = "title={0}&diff=cur&action=render".format(page);
    		var diffContent = wikiApi.getIndexPhp(requestData);
			return wikiApi.wrapOnHtmlPage(diffContent);
    	}
    	
    	public static List<String> links(this O2MediaWikiAPI wikiApi, string pages)
    	{
    		var links = new List<String>();
    		links.AddRange(wikiApi.links_Internal(pages));
    		links.AddRange(wikiApi.links_External(pages));
    		return links;
    	}
    	
    	public static List<String> links_Internal(this O2MediaWikiAPI wikiApi, string pages)
    	{    		
    		try	
    		{
	    		pages = pages.replace(" ","_");
	    		var internalLinks = wikiApi.linksRaw_Internal(pages).attributes("title").values();
	    		
	    		// map the internal links that are linked has external
	    		var externalLinks = wikiApi.links_External(pages);
	    		foreach(var externalLink in externalLinks)
	    			if (externalLink.starts(wikiApi.IndexPhp))
	    			{
	    				var newLink = externalLink.replace(wikiApi.IndexPhp,"");
	    				if (newLink.starts("/"))
	    					newLink = newLink.removeFirstChar();    				
	    				var indexOfSharp = newLink.index("#");
	    				if (indexOfSharp > -1)
	    					newLink = newLink.Substring(0, indexOfSharp);
	    				internalLinks.add(newLink);
	    			}
	    		return internalLinks;	    		
    		}		
			catch(Exception ex)
			{
				ex.log("in O2MediaWikiAPI.links_Internal");
				return new List<String>();
			}
    	}
    	
    	public static List<XElement> linksRaw_Internal(this O2MediaWikiAPI wikiApi, string pages)
    	{
    		var propertyName = "links";
    		var continueVarName = "plcontinue";    			
    		var dataElement = "pl";    		
			var rvlimit = 100;
			
    		return wikiApi.getQueryContinueResults(pages, rvlimit, propertyName, continueVarName, 
    											   "", dataElement);
    		//var xml = wikiApi.action_query_prop("links",pages);
    		//return xml.xRoot().elementsAll("pl");//.attributes("title").values();  		
    	}

		public static List<string> links_External(this O2MediaWikiAPI wikiApi, string pages)
		{
			try
			{
				return wikiApi.linksRaw_External(pages).values();
			}		
			catch(Exception ex)
			{
				ex.log("in O2MediaWikiAPI.links_External");
				return new List<String>();
			}
		}
		
		public static List<XElement> linksRaw_External(this O2MediaWikiAPI wikiApi, string pages)
    	{    		
    		var propertyName = "extlinks";
    		var continueVarName = "eloffset";    		
    		var dataElement = "el";    		
    		var rvlimit = 10;
    		
    		return wikiApi.getQueryContinueResults(pages, rvlimit, propertyName, continueVarName, "", dataElement);
    	}    	
    	
    	
    	public static XElement langlinks(this O2MediaWikiAPI wikiApi, string pages)
    	{
    		return wikiApi.action_query_prop("langlinks",pages).xRoot();
    	}
    	//var xml = wikiApi.parsePage_Raw(targetPage).xmlFormat(); 
		//xRoot.element("parse").element("images").elements("img").values();
    	public static List<string> images(this O2MediaWikiAPI wikiApi, string pages)
    	{
    		var xml =  wikiApi.action_query_prop("images",pages);
    		return xml.xRoot().elementsAll("im").attributes("title").values();
    	}
    	
    	public static XElement imageinfo(this O2MediaWikiAPI wikiApi, string pages)
    	{
    		return wikiApi.action_query_prop("imageinfo",pages).xRoot();
    	}
    	
    	public static List<string> templates(this O2MediaWikiAPI wikiApi, string pages)
    	{
    		return wikiApi.templatesRaw(pages).attributes("title").values();
    	}
    	
    	public static List<XElement> templatesRaw(this O2MediaWikiAPI wikiApi, string pages)
    	{
    		var propertyName = "templates";
    		var continueVarName = "tlcontinue";    			
    		var dataElement = "tl";    		
			var rvlimit = 100;
			
    		return wikiApi.getQueryContinueResults(pages, rvlimit, propertyName, continueVarName, 
    											   "", dataElement);
    	}    	    	    	    	
    	
    	public static string categories(this O2MediaWikiAPI wikiApi, string pages)
    	{
    		return wikiApi.action_query_prop("categories",pages);
    	}    	    	
    	
    	public static string categoryinfo(this O2MediaWikiAPI wikiApi, string pages)
    	{
    		return wikiApi.action_query_prop("categoryinfo",pages);
    	}
    	
    	public static string duplicatefiles(this O2MediaWikiAPI wikiApi, string pages)
    	{
    		return wikiApi.action_query_prop("duplicatefiles",pages);
    	}
    	
		#endregion    		
		
		#region action=query&list=*
    	
    	public static string action_query_list(this O2MediaWikiAPI wikiApi,string query)
    	{
    		var urlData = "action=query&list={0}&format={1}".format(query,wikiApi.ReturnFormat);
    		return wikiApi.getApiPhp(urlData);
    	}
    	
    	public static string allusers(this O2MediaWikiAPI wikiApi)
    	{
    		return wikiApi.action_query_list("allusers");
    	}
    	public static List<string> recentPages(this O2MediaWikiAPI wikiApi)
    	{
    		return wikiApi.recentPages(100);
    	}
    	public static List<string> recentPages(this O2MediaWikiAPI wikiApi, int itemsToFetch)
    	{    		
    		var pages = new List<String>();
    		foreach(var change in wikiApi.recentChangesRaw(itemsToFetch))
    		{
    			if (change.attribute("type").value() != "log")
    			{
    				var title = change.attribute("title").value() ;
    				if (pages.Contains(title).isFalse())
    					pages.Add(title);
    			}    			
    		}
    		return pages;
    	}
    	
    	public static List<string> recentChanges(this O2MediaWikiAPI wikiApi)
    	{
    		return wikiApi.recentChanges(100);
    	}
    	
    	public static List<string> recentChanges(this O2MediaWikiAPI wikiApi, int itemsToFetch)
    	{    		
    		var changes = new List<String>();
    		foreach(var change in wikiApi.recentChangesRaw(itemsToFetch))
    		{
    			var text = "page:\"{0}\" type:{1} by:{2} at:{3}".format(change.attribute("title").value(), 
    											     change.attribute("type").value(),
    											     change.attribute("user").value(),
    											     change.attribute("timestamp").value());
    			changes.add(text);
    		}
    		return changes;    			
    	}
    	
    	public static List<XElement> recentChangesRaw(this O2MediaWikiAPI wikiApi)
    	{
    		return wikiApi.recentChangesRaw(100);	
    	}
    	
    	public static List<XElement> recentChangesRaw(this O2MediaWikiAPI wikiApi, int itemsToFetch)
		{
			try
			{
				return wikiApi.action_query_list("recentchanges&rcprop=user|title|ids|type|timestamp&rclimit=" + itemsToFetch.str()).xRoot().elementsAll("rc");
			}
			catch(Exception ex)
			{
				ex.log("in O2MediaWikiAPI.recentChangesRaw");
				return new List<XElement>();
			}
	
		}				
		
		public static List<string> exUrlUsage(this O2MediaWikiAPI wikiApi, string url)
		{
			var xml =  wikiApi.exUrlUsageRaw(url);
			var xRoot = xml.xRoot();
			return xRoot.element("query")
						.element("exturlusage")
						.elements("eu")
						.attributes("title")
						.values();
		}
		
		public static string exUrlUsageRaw(this O2MediaWikiAPI wikiApi, string url)
		{
			var queryList = "exturlusage&euquery={0}".format(url);
			return wikiApi.action_query_list(queryList);
			//api.php?action=query&list=exturlusage&euquery=www.mediawiki.org
		}
		//allimages, allpages, alllinks, allcategories, , backlinks, blocks, categorymembers, deletedrevs, embeddedin, imageusage, logevents, , search, usercontribs, watchlist, watchlistraw, exturlusage, users, random, protectedtitles
		
		#endregion

        #region other requests with loop support

        public static List<string> getIndexPhp_UsingXPath_AttributeValues(this O2MediaWikiAPI wikiApi,
                                                                          string wikiQueryString, string xPathQuery, string xPathAttribute,
                                                                          string limitVarName, int maxToFetchPerRequest,
                                                                          string offsetVarName, int maxItemsToFetch)
        {
            var currentOffset = 0;
            var result = new List<string>();
            while (result.size() < maxItemsToFetch)
            {
                //var getRequest = "http://www.o2platform.com/index.php?title=Special:UncategorizedPages&limit={0}&offset={1}".format(maxToFetch,currentOffset);
                var getRequest = "{0}&{1}={2}&{3}={4}".format(wikiQueryString, limitVarName, maxToFetchPerRequest, offsetVarName, currentOffset);
                var htmlCode = wikiApi.getIndexPhp(getRequest);
                //"uncategorizedPages GET request: {0}".info(getRequest);
                var htmlDocument = htmlCode.htmlDocument();
                var pages = (xPathAttribute.valid()) 
                				? htmlDocument.select(xPathQuery).attributes(xPathAttribute).values()
                				: htmlDocument.select(xPathQuery).values();
                result.add(pages);
                if (pages.size() == 0 || pages.size() < maxToFetchPerRequest)
                    break;
                currentOffset += maxToFetchPerRequest;
            }
            return result;
        }

        #endregion

        #region text parsing

        public static string parse(this O2MediaWikiAPI wikiApi, string textToParse)
        {
            var html = wikiApi.parseText(textToParse);
            var xRoot = html.xRoot();
            if (xRoot != null && xRoot.Name == "p")
                return xRoot.innerXml();
            return "";
        }

        public static string parseText(this O2MediaWikiAPI wikiApi, string textToParse, bool padWithHtmlPage)
        {
            var parsedText = wikiApi.parseText(textToParse);
            if (padWithHtmlPage)
                return wikiApi.wrapOnHtmlPage(parsedText);
            return parsedText;
        }

        public static List<string> parseText_getImages(this O2MediaWikiAPI wikiApi, string textToParse)
        {
            var images = new List<String>();
            return images;
        }

        #endregion

        #region uploadImage

        public static string uploadImage_FromClipboard(this O2MediaWikiAPI wikiApi)
        {
            var result = "";
            if (wikiApi.loggedIn().isFalse())
                "[in uploadImage_FromClipboard] cannot upload images if the user is not logged in".error();
            else
            {
                var thread = O2Thread.staThread(
                () =>
                {
                    if (Clipboard.ContainsImage())
                    {
                        var bitmap = (Bitmap)Clipboard.GetImage();
                        MemoryStream memoryStream = new MemoryStream();
                        bitmap.Save(memoryStream, ImageFormat.Jpeg);
                        byte[] bitmapData = memoryStream.ToArray();
                        var tempImageName = Files.getSafeFileNameString("{0}_{1}".format(DateTime.Now, Files.getTempFileName().replace(".tmp", ""))) + ".jpg";
                        "[in O2MediaWikiAPI.uploadImage_fromClipboard]: got image, now uploading using under the tempName: {0}".info(tempImageName);
                        result = wikiApi.uploadImage(tempImageName, bitmapData);
                        "[in O2MediaWikiAPI.uploadImage_fromClipboard]: result ={0}".info(result);
                    }
                    else
                        "[in uploadImage_fromClipboard]: There is no image on the clipboard".error();
                }
                );
                thread.Join();
            }
            return result;
        }

        public static string uploadImage(this O2MediaWikiAPI wikiApi, string fileToUpload)
        {        	
            if (fileToUpload.fileExists().isFalse())
            {
            	"[O2MediaWikiAPI] could not find file to upload: {0}".error(fileToUpload);
                return "";
			}
			"[O2MediaWikiAPI] uploading as Image: {0}".info(fileToUpload);
            var fileName = fileToUpload.fileName();
            var fileContents = fileToUpload.fileContents_AsByteArray();
            return wikiApi.uploadImage(fileName, fileContents);
        }

        public static string uploadImage(this O2MediaWikiAPI wikiApi, string fileName, byte[] fileContents)
        {
            if (wikiApi.loggedIn().isFalse())
            {
            	"[O2MediaWikiAPI] cannot upload images if user is not logged in: {0}".error(fileName);
                return "";
            }
            try
            {
                string sessionId = wikiApi.Login_SessionId; // "__c451ccaca794893f041024657b8ed498";
                string userId = wikiApi.Login_UserId;
                string userName = wikiApi.Login_Username;

                string uploadDescription = "File uploaded using an O2 Platform script";
                string postURL = wikiApi.IndexPhp + "/Special:Upload";
                string fileFormat = fileName.extension();
                string fileContentType = "image/" + fileFormat;
                string userAgent = "O2 Platform";
                //string cookies = "wiki_db_session={0}; wiki_dbUserID={1}; wiki_dbUserName={2};{3}".format(sessionId, userId, userName);
                var cookies = wikiApi.Login_Cookie;

                string postedFileHttpFieldName = "wpUploadFile";
                var postParameters = new Dictionary<string, object>();
                postParameters.Add("wpSourceType", "file");
                postParameters.Add("wpDestFile", fileName);
                postParameters.Add("wpUpload", "Upload File");
                postParameters.Add("wpIgnoreWarning", "True");
                postParameters.Add("wpUploadDescription", uploadDescription);				
                var httpMultiPartForm = new HttpMultiPartForm();
                if (wikiApi.BasicAuth.valid())
 					httpMultiPartForm.add_Header("Authorization","Basic " + wikiApi.BasicAuth);
 					
                var httpWebResponse = httpMultiPartForm.uploadFile(fileContents, postParameters, fileName, fileContentType, postURL, userAgent, postedFileHttpFieldName, cookies);
                if (httpWebResponse != null)
                    if (httpWebResponse.ResponseUri.AbsoluteUri.extension() == fileName.extension())
                        return httpWebResponse.ResponseUri.AbsoluteUri.str();
            }
            catch (Exception ex)
            {
                ex.log("in O2MediaWiki.uploadImage");
            }
            return "";
        }
        #endregion

        #region misc

        public static string padWithHtmlPage(this O2MediaWikiAPI wikiApi, string textToParse)
        {
            return wikiApi.wrapOnHtmlPage(textToParse);
        }

        public static string wrapOnHtmlPage(this O2MediaWikiAPI wikiApi, string htmlCode)
        {
            return "<html><head><base href=\"{0}\"> {1} </head><body>{2}</body></html>".format(wikiApi.HostUrl, wikiApi.Styles, htmlCode);
        }

        public static string getWikiTextForImage(this O2MediaWikiAPI wikiApi, string imageUrl)
        {
            var indexOfFile = imageUrl.indexLast("File:");
            if (indexOfFile > -1)
            {
                var file = imageUrl.Substring(indexOfFile + 5);
                return "[[Image:{0}]]".format(file);
            }
            return "";
        }

        public static string getMediaHref(this O2MediaWikiAPI wikiApi, string mediaName)
        {
            try
            {
                var textToParse = "[[Media:{0}]]".format(mediaName);
                var html = wikiApi.parseText(textToParse);
                var xRoot = html.xRoot();
                if (xRoot != null && xRoot.Name == "p")
                {
                    var href = xRoot.FirstNode.xElement().attribute("href").value();
                    if (href.contains("Special:Upload"))		// happens when the file doesn't exist
                        return "";
                    return href;
                }
            }
            catch (Exception ex)
            {
                ex.log("in getMediaHref for media:{0}".format(mediaName));
            }
            return "";
        }

        public static string pageUrl(this O2MediaWikiAPI wikiApi, string page)
        {
            return wikiApi.IndexPhp + "/" + page;
        }
        
        public static WebBrowser add_WikiHelpPage(this Control control, O2MediaWikiAPI wikiApi,  string wikiPage)
        {
            var browser = control.add_WebBrowser();
            O2Thread.mtaThread(
                () =>
                {
                    var htmlText = wikiApi.getPageHtml(wikiPage);
                    browser.set_Text(htmlText);
                });
            return browser;
        }
        
        
        public static T show_Diff_LatestChanges<T>(this T control, O2MediaWikiAPI wikiApi)
        	where T : Control
        {
        	return 	control.show_Diff_LatestChanges(wikiApi,100);
        }
        public static T show_Diff_LatestChanges<T>(this T control, O2MediaWikiAPI wikiApi, int itemsToShow)
        	where T : Control
        {        	
        	var cancelLoad = false;
			var browser = control.add_WebBrowser();
			var treeView = browser.insert_Left<TreeView>(200)
								  .showSelection()	
								  .afterSelect<string>((html)=>browser.set_Text(html));
			treeView.backColor(Color.LightPink);		  
			var recentPages = wikiApi.recentPages(itemsToShow);
			treeView.backColor(Color.Azure); 
			treeView.add_ContextMenu().add_MenuItem("Cancel Load", ()=>cancelLoad = true);
			foreach(var recentPage in recentPages)
			{
				var diffHtml = wikiApi.diff_Current(recentPage); 	
				treeView.add_Node(recentPage, diffHtml);
				if (treeView.nodes().size()==1)
					treeView.selectFirst();		
				if (cancelLoad)
					break;
			}
			treeView.backColor(Color.White);
			return control;

        }

        #endregion
		
		#region fetch from Namespace (templates, users, categories, users)
		
		public static List<String> templatePages(this O2MediaWikiAPI wikiApi)
    	{
    		return wikiApi.allPagesFromNamespace(10);
    	}
    	
    	public static List<String> users(this O2MediaWikiAPI wikiApi)
    	{
    		return wikiApi.userPages();
    	}
    	
    	public static List<String> userPages(this O2MediaWikiAPI wikiApi)
    	{
    		return wikiApi.allPagesFromNamespace(2);
    	}
    	
    	public static List<String> categoryPages(this O2MediaWikiAPI wikiApi)
    	{
    		return wikiApi.allPagesFromNamespace(14);
    	}
    	
    	public static List<String> allPagesFromNamespace(this O2MediaWikiAPI wikiApi, int namespaceID)
    	{
    		var results = new List<string>();
    		try
    		{
    			
				Func<HtmlAgilityPack.HtmlDocument,string> resolveNextLink = 
					(htmlDocument)=>{		 
										var nextLink = "";
										foreach(var link in htmlDocument.select("//td[@id='mw-prefixindex-nav-form']//a"))
											if (link.value().starts("Next page ("))  
											{ 
												nextLink = link.attribute("href").value().remove("/index.php" + wikiApi.SEPARATOR_CHAR);
												nextLink = System.Web.HttpUtility.HtmlDecode(nextLink);
											break;
											}
										return nextLink;
									};
									
				var nextRequest = "title=Special:PrefixIndex&from=&namespace={0}".format(namespaceID);
												
				
				while(nextRequest.valid()) 
				{					
					var html = wikiApi.getIndexPhp(nextRequest); 		
					var htmlDocument = html.htmlDocument();
					results.AddRange(htmlDocument.select("//table[@id='mw-prefixindex-list-table']//a").attributes("title").values());
					"there are {0} results".info(results.size()); 
					nextRequest = resolveNextLink(htmlDocument);	
				}
			}
			catch (Exception ex)
			{
				ex.log("[O2MediaWikiAPI] in allPagesFromNamespace");
				
			}			
			return results;
    	}
		
		#endregion

    }
    
    public static class O2MediaWikiAPI_ExtensionMethods_GuiHelpers
    {
    	public static ascx_TableList add_Table_with_RecentChanges(this O2MediaWikiAPI wikiApi, Control control)
    	{    		
    		return wikiApi.add_Table_with_XElements(control, wikiApi.recentChangesRaw(20));
    	}
    	
    	public static ascx_TableList add_Table_with_XElements(this O2MediaWikiAPI wikiApi, Control control, List<XElement> xElements)
    	{
    		var tableList = control.add_TableList();
			tableList.add_Column("#");
									
			if (xElements.size() > 0)
				foreach(var attribute in xElements.first().attributes())
					tableList.add_Column(attribute.Name.str()); 
			var id = 1;		
			foreach(var xElement in xElements) 
			{
				var rowValues = new List<string>().add(id++.str());
				foreach(var attribute in xElement.attributes())
					rowValues.add(attribute.Value);
				tableList.add_Row(rowValues);
			}
			tableList.makeColumnWidthMatchCellWidth(); 
			return tableList;
    	}
    }
 }
