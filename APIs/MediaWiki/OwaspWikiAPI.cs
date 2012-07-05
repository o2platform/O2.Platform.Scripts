// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;


//O2File:O2MediaWikiApi.cs
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:System.Xml.dll

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
				//var page = @"http://www.owasp.org/index.php?title=Special:UserLogin";  
				var page = "http://www.owasp.org/index.php?title=Test&diff=cur";
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
				"problem retrieving owasp.org headers".error();				
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
