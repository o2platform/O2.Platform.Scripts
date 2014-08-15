// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using FluentSharp.CoreLib;
using FluentSharp.Web35;

//O2File:O2MediaWikiApi.cs

namespace O2.XRules.Database.APIs
{

	public class OwaspWikiAPI : O2MediaWikiAPI
	{
		public bool AllOk {get;set;}
        
        public OwaspWikiAPI() : this(true)
		{
			
		}
		
		public OwaspWikiAPI(bool loadStylesFromWebsite)
		{	
			this.ApiName = "OwaspWiki";
			init("https://www.owasp.org/api.php");
			
			this.Styles = (loadStylesFromWebsite) ? owaspStyles() : "";
		}
		
		public override string ToString()
 		{
 			return ApiName;
 		}
 		
		//TODO: add detection to see if we are a)online and b) able to access the www.owasp.org website				
		// dynamically (one per session) grab the current header scripts used in OWASP
		public string owaspStyles()
		{			
			try
			{
				"Preloading OWASP Wiki Styles".debug();		
				//var page = @"http://www.owasp.org/index.php?title=Special:UserLogin";  
				
				var cssFiles = new [] {"https://www.owasp.org/load.php?debug=false&lang=en&modules=mediawiki.action.history.diff&only=styles&skin=vector&*" ,
							 		   "https://www.owasp.org/load.php?debug=false&lang=en&modules=mediawiki.legacy.commonPrint%2Cshared%7Cskins.vector&only=styles&skin=vector&*"};

				var headerText = "<style>".line() + 
									cssFiles[0].url().get_Html().line() +
								 	cssFiles[1].url().get_Html().line() + 
								 "</style>".line();
				
					
				return headerText;
				
/*				var page = "http://www.owasp.org/index.php?title=Test&diff=cur";
				var codeToParse = page.uri().getHtml(); 
				
				var htmlDocument = new HtmlAgilityPack.HtmlDocument();
				htmlDocument.LoadHtml(codeToParse);
								
				var headerText = "".line().line();
				foreach(var node in htmlDocument.DocumentNode.SelectNodes("//link[@type='text/css']"))
				{
					var html = node.OuterHtml.line();
					html = html.replace("href=\"/","href=\""+ this.HostUrl);
					headerText += html;// + "</link>";
				}
				var scripts =  htmlDocument.DocumentNode.SelectNodes("//head//script");
				foreach(var node in htmlDocument.DocumentNode.SelectNodes("//head//script")) 
				{
					var html = node.OuterHtml.line();
					if (html.contains("ga.js").isFalse() && html.contains("_gat").isFalse())
						headerText += html;
				}
				//headerText += "AAAAAAAAAAAAAA".line();
				//headerText += "<link rel=\"stylesheet\" href=\"/skins/common/diff.css?207\" type="text/css" />"
				if (scripts.Count > 4)
				{
					headerText += scripts[0].OuterHtml.line();
					headerText += scripts[1].OuterHtml.line();
					headerText += scripts[2].OuterHtml.line();
					headerText += scripts[3].OuterHtml.line();
					headerText += scripts[4].OuterHtml.line();
			
					AllOk = true;
					return headerText.line().line();
				}			
			
				"problem retrieving owasp.org headers".error();				*/
			}
			catch(Exception ex)
			{
				ex.log("in OwaspWikiApi.owaspStyles");
				
			}
			AllOk = false;
			return "";
			
		}
    }
    
    public static class OwaspWikiAPI_ExtensionMethods
    {
    	/*public static List<string> OWASP_Projects(this OwaspWikiAPI wikiApi)
    	{
    		//var categoryTag = "Category:OWASP Project";
    		var projects = new List<string>();	
    		projects.Add("Asd"); 
    		return projects;
    	}*/
    }
    
    
}
