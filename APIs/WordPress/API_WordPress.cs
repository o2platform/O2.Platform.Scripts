// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.Utils;
using FluentSharp.WinForms;
using JoeBlogs;
using JoeBlogs.Structs;

//O2File:O2MediaWikiApi.cs
//O2Ref:WordPress_XmlRpc_JoeBlogs.dll
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:System.Xml.dll

namespace O2.XRules.Database.APIs
{
    public class API_WordPress
    {    
    	public string WordPressServer {get; set;}
        public string WordPressXmlRpc { get; set; } 
    	public WordPressWrapper WordPress { get; set; }
    	public MetaWeblogWrapper MetaWeblog { get; set; }
    	
    	public bool LoggedIn { get; set; }    	    
		public API_WordPress() : this ("https://o2platform.wordpress.com")
		{			
		}
		
    	public API_WordPress(string wordPressServer)
    	{               
            WordPressXmlRpc = wordPressServer;
            if (WordPressXmlRpc.Contains("/xmlrpc.php").isFalse())
                WordPressXmlRpc += "/xmlrpc.php";
            if (WordPressXmlRpc.lower().starts("http").isFalse())
                WordPressXmlRpc = "http://" + WordPressXmlRpc;
            WordPressServer = WordPressXmlRpc.replace("/xmlrpc.php","");
    	}    	    	      	
    }
    
    [Serializable]
    public class methodCall
    {
    	public string methodName { get; set; }
    	public List<param> @params {get;set;}
    	
    	public methodCall()
    	{
    		@params = new List<param>();
    	}
    }
    
    public class param
    {
    	[XmlElement("param")]
    	public string_Value username { get; set; }
    	[XmlElement("param")]
    	public string_Value password { get; set; }
    }
    
    public class  string_Value
    {       	
    	public string @string {get;set;}
    }
    
    
    public static class API_WordPress_ExtensionMethods
    {
    
    	public static API_WordPress login(this API_WordPress wpApi, Credential credential)
    	{
    		return wpApi.login(credential.UserName,credential.Password);
    	}
    
    	public static API_WordPress login(this API_WordPress wpApi, string username, string password)
    	{
            wpApi.WordPress = new WordPressWrapper(wpApi.WordPressXmlRpc, username, password);    
            wpApi.MetaWeblog = new MetaWeblogWrapper(wpApi.WordPressXmlRpc, username, password);    
    		wpApi.LoggedIn = wpApi.loggedIn();
    		return wpApi;
    	}  
    	
    	//DC: need to find a better way to do this
    	public static bool loggedIn(this API_WordPress wpApi)
    	{
    		try
    		{
    			wpApi.WordPress.GetUserInfo();
    			return true;
    		}
    		catch
    		{
    			return false;
    		}
    	}
    	
    	public static string post(this API_WordPress wpApi, string title, string body)
    	{
    	
    		var post = new Post();
			post.dateCreated = DateTime.Now;
			post.title = title;
			post.description = body;
			return wpApi.WordPress.NewPost(post, true);

		}
		
		public static List<Page> pages(this API_WordPress wpApi)
		{
			return wpApi.WordPress.GetPages().toList();
		}
		
	}
	public static class WordPressAPI_ExtensionMethods_MediaWiki_Integration
    {
		public static string post_from_MediaWiki(this API_WordPress wpApi, O2MediaWikiAPI wikiApi, string mediaWikiPage)
		{
			return wpApi.post_from_MediaWiki(wikiApi, mediaWikiPage, mediaWikiPage);
		}
		
		public static string post_from_MediaWiki(this API_WordPress wpApi, O2MediaWikiAPI wikiApi, string mediaWikiPage, string postTitle)
		{
			try
			{
				var code = wikiApi.parsePage(mediaWikiPage);  
				var htmlDocument = new HtmlAgilityPack.HtmlDocument();
				htmlDocument.LoadHtml(code);
				
				// remove MediaWiki comments
				foreach(var node in  htmlDocument.DocumentNode.ChildNodes)
					if (node is HtmlAgilityPack.HtmlCommentNode)
						htmlDocument.DocumentNode.RemoveChild(node);
				
				// fix images links
				foreach(var a in htmlDocument.DocumentNode.SelectNodes("//img"))
				{
					var src = a.Attributes["src"];
					if (src!= null)	
						if (src.Value.starts("/"))
							src.Value = wikiApi.HostUrl + src.Value; 		
				}
				
				// fix a href links
				foreach(var a in htmlDocument.DocumentNode.SelectNodes("//a"))
				{
					var href = a.Attributes["href"];
					if (href!= null)	
						if (href.Value.starts("/"))
							href.Value = wikiApi.HostUrl + href.Value; 		
				}
					
				var postBody = htmlDocument.DocumentNode.OuterHtml;
                var messageToAppend = "[automatic O2 comment]:" + "<hr>" +
                                      "<b>Note:</b> This blog post was created by an <a href='http://www.o2platform.com'>O2 Script</a> and is a copy of the MediaWiki page with the title <i>'{0}'</i> located at: <a href='{1}'>{1}</a>"
                                      .format(mediaWikiPage, wikiApi.pageUrl(mediaWikiPage)).line() +
                                      "<b>Exported on</b>:{0}"
                                      .format(DateTime.Now.ToShortDateString());
								      
				postBody = postBody.add(messageToAppend);
				return wpApi.post(postTitle, postBody);
			}
			catch(Exception ex)
			{
                ex.logWithStackTrace("in WordPressAPI.post_from_MediaWiki, for MediaWikiAPI page '{0}'".format(mediaWikiPage));
				return "";
			}			
		}
    }
    
    
    public static class WordPressAPI_ExtensionMethods_IE_Utils
    {
    	public static API_WordPress addRequiredSitesToIETrustedZone(this API_WordPress wordPress)
    	{
    		"o2platform".makeDomainTrusted("com");
    		"stats.wordpress.com".makeDomainTrusted("s");
    		"scorecardresearch.com".makeDomainTrusted("b");
    		"wp.com".makeDomainTrusted("sp1");
    		"wp.com".makeDomainTrusted("s2");
    		"wp.com".makeDomainTrusted("s1");
    		"wp.com".makeDomainTrusted("s0");
    		"quantserve.com".makeDomainTrusted("edge");
    		"gravatar.com".makeDomainTrusted("s");    		
    		return wordPress;
    	}
    }
    
    
    
    
    public static class WordPressAPI_ExtensionMethods_SourceCode_Utils
    {
    	public static string wrapClipboardTextInSourceCodeTags(this API_WordPress wordPress)
    	{    		
    		var clipboardText = "".clipboardText_Get();
    		var startText = "[sourcecode language=\"csharp\" wraplines=\"false\"]".line();
    		var endText = "[/sourcecode]".line();
    		if (clipboardText.contains(startText))
    			return clipboardText;
    		"Current Clipboard Text:{0}".info(clipboardText);
    		clipboardText = clipboardText.Replace("\t","    ").fix_CRLF();    		
    		var wrapedText = startText + 
							 clipboardText + "".line() + 
 							 endText;

    		wrapedText.clipboardText_Set();
    		return clipboardText;
    	}
    }
        
}
