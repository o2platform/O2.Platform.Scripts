// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.For_HtmlAgilityPack;
using FluentSharp.WinForms;
using HtmlAgilityPack;
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll


namespace O2.XRules.Database.APIs
{
	public class API_Mailman
	{
		public string BaseUrl { get; set; }		
		public string TempDir { get; set; }		
	
	
	}
	
	public static class API_Mailman_ExtensionMethods_Utils
	{
		public static string getHtml_UsingCache(this API_Mailman apiMailman, string virtualPath)		
		{
			return apiMailman.getHtml_UsingCache(virtualPath, apiMailman.TempDir);
		}
		
		public static string getHtml_UsingCache(this API_Mailman apiMailman, string virtualPath, string tempDir)
		{
			tempDir.createDir();
			var urlToGet = apiMailman.BaseUrl + virtualPath;
			var cacheFilePath = tempDir.pathCombine(urlToGet.safeFileName() + (virtualPath.contains(".html") 	
																							? ""
																							: ".html") );
			"processing: {0} using cache file: {1}".info(urlToGet,cacheFilePath);
			return (cacheFilePath.fileExists())
						? cacheFilePath.fileContents()
						: urlToGet.uri()
								  .getHtml()
								  .saveAs(cacheFilePath)
								  .fileContents();							
		}	
		
		public static List<HtmlNode> getNodesFromHtmlPage(this API_Mailman apiMailman, string urlToGet, string filter)
		{
			var htmlDocument = apiMailman.getHtml_UsingCache(urlToGet).htmlDocument();
			return htmlDocument.select(filter);
		}
		
		public static string extractEmail(this string rawEmail)
		{
			return rawEmail.replace(" at ", "@").trim().ToLower();
		}
		
		
		public static Dictionary<string,string> get_HTML_for_Pages(this API_Mailman apiMailman, string virtualPathTemplate, List<string> pages)
		{
			var adminPagesHtml = new Dictionary<string,string>();
			var pagesDone = 0;			
			foreach(var page in pages)
			{
				var virtualPath = virtualPathTemplate.format(page);
				adminPagesHtml.Add(page, apiMailman.getHtml_UsingCache(virtualPath));
				if (++pagesDone % 25 == 0)
					"done {0} out of  {1}".debug(pagesDone, pages.size());				
			}			
			return adminPagesHtml;
		}
	}
	
	public static class API_Mailman_ExtensionMethods_Fetch
	{
		public static List<string> mailingLists(this API_Mailman apiMailman)
		{
			return (from htmlNode in apiMailman.mailingLists_Raw()
					select htmlNode.attribute("href")
								   .value()
								   .remove("admin/")).toList();
		}
		
		public static List<HtmlNode> mailingLists_Raw(this API_Mailman apiMailman)
		{
			var links =  apiMailman.getNodesFromHtmlPage("mailman/admin","//a");
			var htmlNodes = (from link in links
						     where link.attribute("href").value().contains("admin/")
						     select link).toList();
			return htmlNodes;
		}
		
		//admin page
		
		public static string get_AdminPage_HTML_for_MailingList(this API_Mailman apiMailman, string listName)
		{
			return apiMailman.getHtml_UsingCache("admin/{0}".format(listName));
		}
		
		public static Dictionary<string,string> get_AdminPage_HTML_for_ALL_MailingLists(this API_Mailman apiMailman)
		{
			var adminPagesHtml = new Dictionary<string,string>();
			var pagesDone = 0;
			var listNames = apiMailman.mailingLists();
			foreach(var listName in listNames)
			{
				adminPagesHtml.Add(listName, apiMailman.get_AdminPage_HTML_for_MailingList(listName));
				if (++pagesDone % 25 == 0)
					"done {0} out of  {1}".debug(pagesDone, listNames.size());
			}
			"get_AdminPage_HTML_for_ALL_MailingLists completed".debug();
			return adminPagesHtml;
		}
		
		public static List<string> get_Admins_For_MailingList(this API_Mailman apiMailman, string listName)
		{
			var emails = new List<string>();
			var links = apiMailman.getNodesFromHtmlPage("mailman/admin/{0}".format(listName), "//a"); 
				foreach(var link in links)
					if (link.InnerText.contains(" at ")) 
						foreach(var rawEmail in link.InnerText.split(",")) 
							emails.add(rawEmail.extractEmail());
					
			return emails;
		}
	
		//emails
		
		public static string get_Archive_HTML_for_MailingList(this API_Mailman apiMailman, string listName)
		{
			return apiMailman.getHtml_UsingCache("pipermail/{0}".format(listName));
		}
		
		public static string get_Archive_HTML_for_MailingList_Month(this API_Mailman apiMailman, string listName,string month)
		{
			return apiMailman.getHtml_UsingCache("pipermail/{0}/{1}/thread.html".format(listName,month));
		}
		
		public static List<string> get_Email_Archive_Months_for_MailingList(this API_Mailman apiMailman, string listName)
		{
			var links = apiMailman.getNodesFromHtmlPage("pipermail/{0}".format(listName) , "//a[contains(text(),'[ Thread ]')]");
			return (from link in links
				 	select link.attribute("href")
				 			   .value()
				 			   .remove("/thread.html")
				 			   
				 	).toList();
		}
		
		
		public static Dictionary<string,string> get_Email_Archive_HTML_For_All_Months_for_MailingList(this API_Mailman apiMailman, string listName)
		{
			var emailMonths = apiMailman.get_Email_Archive_Months_for_MailingList(listName);						
			return apiMailman.get_HTML_for_Pages("pipermail/" + listName  + "/{0}/thread.html", emailMonths);
		}
		
		public static Dictionary<string,Dictionary<string,string>> get_Email_Archive_HTML_For_All_Months_for_ALL_MailingLists(this API_Mailman apiMailman)
		{
			var allData = new Dictionary<string,Dictionary<string,string>>(); 
			var listNames = apiMailman.mailingLists();
			var listsProcessed = 0;
			foreach(var listName in listNames)
			{				
				"[{0}/{1}] Processing {2}".lineBeforeAndAfter().lineBeforeAndAfter().debug(++listsProcessed,listNames.size(), listName);
				var listData = apiMailman.get_Email_Archive_HTML_For_All_Months_for_MailingList(listName);
				allData.Add(listName, listData);
														
			}
			return allData;
		}
		
		
		public static List<string> get_Email_Messages_Urls_for_Mailing_List_Month(this API_Mailman apiMailman, string listName, string month)
		{			
			var rawHtml = apiMailman.get_Archive_HTML_for_MailingList_Month(listName, month);
			var linkNodes =  rawHtml.htmlDocument().select("//a[contains(@href,'0')]");
			return (from linkNode in linkNodes
					let value = linkNode.attribute("href").value()
					where value.contains(".html")
					select "pipermail/{0}/{1}/{2}".format(listName, month,value) ).toList();			
		}
		
		public static Dictionary<string,string> get_Email_Messages_for_Mailing_List_Month(this API_Mailman apiMailman, string listName, string month)
		{
			var emailMessages = new Dictionary<string,string>();
			var tempDir  = apiMailman.TempDir.pathCombine("emails");
			var emailUrls = apiMailman.get_Email_Messages_Urls_for_Mailing_List_Month(listName, month);
			foreach(var emailUrl in emailUrls)
				emailMessages.Add(emailUrl, apiMailman.getHtml_UsingCache(emailUrl, tempDir));
			return emailMessages;
		}
		
		public static Dictionary<string,Dictionary<string,string>> get_Email_Messages_for_All_Mailing_Lists_in_Month(this API_Mailman apiMailman, string month)
		{
			var emailMessages = new Dictionary<string,Dictionary<string,string>>();
			foreach(var listName in apiMailman.mailingLists())
			{
				emailMessages.Add(listName, apiMailman.get_Email_Messages_for_Mailing_List_Month(listName, month));				
			}
			return emailMessages;
		}
	}
	
	
	public static class API_Mailman_GUI_Helpers
	{
		public static Panel show_Gui_with_ListAdmins_Mappings(this API_Mailman apiMailman)
		{
			var topPanel = "Mailman - Gui_with_ListAdmins_Mappings".popupWindow()
																   .insert_LogViewer();
			return apiMailman.show_Gui_with_ListAdmins_Mappings(topPanel);
		}
		
		public static Panel show_Gui_with_ListAdmins_Mappings(this API_Mailman apiMailman, Panel topPanel) 		
		{
			var tableList = topPanel.add_GroupBox("OWASP Mailing list mappings").add_TableList();  
			var browser = topPanel.insert_Right("List Admin WebPage").add_WebBrowser_Control();
			tableList.add_Columns("email", "list name", "list #", "href" );
			tableList.afterSelect_get_Cell(3,
				(href)=>{ 
							browser.open(href);														
						});
						
			/*browser.onNavigated( // not working the browser still gets the focus
				(url)=> {
							"onNavigated".info();							
							tableList.listView().focus();
							tableList.focus();
							
						});*/
			
			var listNumber = 0;
			tableList.visible(false);
			foreach(var listName in apiMailman.mailingLists().Take(20))
			{
				listNumber++;
				foreach(var email in apiMailman.get_Admins_For_MailingList(listName))
				{
					tableList.add_Row(email,							
									  listName, 
									  listNumber.str() ,
									  "{0}admin/{1}".format(apiMailman.BaseUrl, listName));							
				}
			}
			tableList.visible(true);
			tableList.makeColumnWidthMatchCellWidth();
			return topPanel;
		}
	}
}	