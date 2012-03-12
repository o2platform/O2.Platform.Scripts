// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Net;
using System.Linq;
using System.Collections.Generic; 
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;

//O2File:TM_WebServices.cs			
//O2File:_Extra_methods_Misc.cs

namespace SecurityInnovation.TeamMentor
{
	public class TM_WebServices_Configured : TM_WebServices
	{
		public static string	WEBSERVICES_PATH = "/aspx_pages/TM_WebServices.asmx";
		
		public TM_WebServices_Configured(Uri websiteUrl)
		{			  
			this.Url = websiteUrl.append(WEBSERVICES_PATH).str();			
			this.CookieContainer =  new System.Net.CookieContainer(); 			
		}
		
		protected override System.Net.WebRequest GetWebRequest(System.Uri Uri)
		{
			var req = (HttpWebRequest)base.GetWebRequest(Uri);
				    	
	    	var cookies = this.CookieContainer.GetCookies(Uri);
	    	if (cookies.notNull() && cookies.size() > 0 && cookies["Session"].notNull())
	    	{
	    		var crsf_Token = cookies["Session"].Value.hash().str();
	    		req.Headers.Add("CSRF_Token",crsf_Token);	    		
	    	}
	        return req;
	    }

	}
	
	public class API_TeamMentor_WebServices 
    {    
    	public Uri 				WebSite_Url 	{ get; set; }    	
    	public string 			WebSite_Path 	{ get; set; }    	
    	public TM_WebServices 	webServices;
    	
    	public static string 	DEFAULT_TM_SITE	 = "http://localhost.:12345";    	
    	    	
    	public API_TeamMentor_WebServices() : this(DEFAULT_TM_SITE)
    	{
    	}
    	
    	public API_TeamMentor_WebServices(string websiteUrl)
    	{
    		this.WebSite_Url 		= websiteUrl.uri();
    		this.webServices  	 = new TM_WebServices_Configured(this.WebSite_Url);					
    	}		
    	
/*    	Action setAdminPriviledges = 
	()=>{
			"setAdminPriviledges".info();
			UserGroup.Admin.setThreadPrincipalWithRoles();				
		};		*/				
    }
    
    public class Base_Structure
    {
    	public string 						Name	 { get; set; }
		public Guid 						Id		 { get; set; }
		
		public override string ToString()
		{
			return this.Name;
		}
    }
    
    public class Library_WS : Base_Structure
	{
		public API_TeamMentor_WebServices 	tmWebServices  ;
		
		
		public Library_WS(API_TeamMentor_WebServices _tmWebServices , string name, Guid id)
		{
			this.tmWebServices 	= _tmWebServices;
			this.Name 	 		= name;
			this.Id				= id;
		}
	}
	
	public class Folder_WS : Base_Structure
	{
		public Library_WS 			Library	 { get; set; }		
		public List<View_WS>		Views	 { get; set; }
		
		public Folder_WS()
		{
			Views = new List<View_WS>();
		}
		
	}
	
	public class View_WS : Base_Structure
	{
		public Library_WS 		Library	 { get; set; }		
		public Folder_WS		Folder	 { get; set; }				
		public List<Article_WS> Articles { get; set; }
		
		public View_WS()
		{
			Articles = new List<Article_WS>();
		}
	}
	
	public class Article_WS : Base_Structure
	{
		public Library_WS 		Library	   { get; set; }		
		public View_WS			View	   { get; set; }		
		
		public string 			Title	   { get; set; }
		public string 			Type 	   { get; set; }
		public string 			Phase 	   { get; set; }		
		public string 			Category   { get; set; }		
		public string 			Technology { get; set; }
		
		public string 			Content    { get; set; }
	}    
    
    //************** EXTENSION METHODS
    
    public static class API_TeamMentor_WebServices_ExtensionMethods_Libraries
    {
    	public static List<Library_WS> libraries(this API_TeamMentor_WebServices tmWebServices)
    	{
    		return (from ws_library in tmWebServices.webServices.GetLibraries()
    			    select new Library_WS(tmWebServices,ws_library.Caption, ws_library.Id)) .toList();
    	}
    	
    	public static Library_WS library(this API_TeamMentor_WebServices tmWebServices , string libraryName)
    	{
    		var ws_library = tmWebServices.webServices.GetLibraryByName(libraryName);
    		if (ws_library.isNull())
    		{    			
    			tmWebServices.webServices.CreateLibrary(new Library { caption = libraryName } );
    			ws_library = tmWebServices.webServices.GetLibraryByName(libraryName);
    		}	
    		return new Library_WS(tmWebServices,ws_library.caption, ws_library.id.guid());
    	}
    	
    	public static Library library(this API_TeamMentor_WebServices tmWebServices , Guid libraryId)
    	{
    		return tmWebServices.webServices.GetLibraryById(libraryId);
    	}    	    	
    }
            
    public static class API_TeamMentor_WebServices_ExtensionMethods_Folders
    {
    	public static List<Folder_WS> folders(this Library_WS library)
    	{
    		return library.tmWebServices.webServices.GetFolders(library.Id).toList()
    										        .folders(library);
    	} 
    	
    	public static List<Folder_WS> folders(this List<Folder_V3> foldersV3 , Library_WS library)
    	{
    		return (from folderV3 in foldersV3
    				select folderV3.folder(library) ).toList();
    	}
    	
    	public static Folder_WS folder(this Folder_V3 folderV3 , Library_WS library)
    	{
    		var folder = new Folder_WS()
		    					{
		    						Library = library,
		    						Name 	= folderV3.name,
		    						Id 		= folderV3.folderId
		    					};
			foreach(var view in folderV3.views)
				folder.Views.Add(library.tmWebServices.webServices.GetViewById(view.viewId.str())
																  .view(library, folder));
			return folder;
    	}    	
    	
    	public static Folder_WS folder(this Library_WS library, string name)
    	{    		
    		var folder = (from   _folder in library.folders()
    					  where  _folder.Name == name
    					  select _folder).toList().first();
			if (folder.notNull())
				return folder;
			
			return library.add_Folder(name);
    	}
    	
    }    
    
    public static class API_TeamMentor_WebServices_ExtensionMethods_Views
    {    	
    	public static List<View_WS> views(this Folder_WS folder)
    	{
    		return folder.Views;
    	}
    
    	public static View_WS view(this Folder_WS folder, string name)
    	{    		
    		var view = (from   _view in folder.views()
    					where  _view.Name == name
    					select _view).toList().first();
    		if (view.notNull())    		
    			return view;
    		return folder.add_View(name);
    	}
    	
    	public static View_WS view(this View_V3 viewV3, Library_WS library, Folder_WS folder)
    	{
    		var view = new View_WS()
    						{
    							Name 	= viewV3.caption,
    							Id   	= viewV3.viewId,
    							Library = library,
    							Folder  = folder
    						};
			foreach(var guid in viewV3.guidanceItems)		
				view.Articles.Add(guid.article(library, view));
			return view;
    	}
    }  
    
    public static class API_TeamMentor_WebServices_ExtensionMethods_Articles
    {    	    	
    	public static List<Article_WS> articles(this View_WS view)
    	{
    		return view.Articles;
    	}
    	    	    	
    	public static Article_WS article(this View_WS view, string name)
    	{
    		var article = ( from   _article in view.articles()
    						where  _article.Name == name
    						select _article).toList().first();
    		if (article.notNull())    		
    			return article;
    		return view.add_Article(name, "");
    	}
    	    	    	
    	public static Article_WS article(this Guid guid, Library_WS library)
    	{
    		return guid.article(library, null);
    	}
    	
    	public static Article_WS article(this Guid guid, Library_WS library, View_WS view)
    	{
    		var guidanceItemV3 = library.tmWebServices.webServices.GetGuidanceItemById(guid.str());    		
    		return guidanceItemV3.article(library,view);
    	}
    	
    	public static Article_WS article(this GuidanceItem_V3 guidanceItemV3, Library_WS library, View_WS view)
    	{    		
    		
    		var article = new Article_WS()
	    						{
	    							Name 	= guidanceItemV3.title,
	    							Id   	= guidanceItemV3.guidanceItemId,	    							
	    							Library = library,
	    							View  	= view,
	    							
	    							Title	   = guidanceItemV3.title,
	    							Type 	   = guidanceItemV3.rule_Type,
	    							Phase 	   = guidanceItemV3.phase,
	    							Category   = guidanceItemV3.category,
	    							Technology = guidanceItemV3.technology,
	    							
	    							Content    = guidanceItemV3.htmlContent
	    							
	    						};
			return article;
    	}    	
    	
    	/*public static GuidanceItem_V3 guidanceItemV3(this Article_WS article)
    	{    		
    		
    		var guidanceItemV3 = new GuidanceItem_V3()
		    						{
		    							title = article.Title,
		    							guidanceItemId = article.Id,		    							
		    									    							
		    							rule_Type 	= article.Type,
		    							phase 		= article.Phase,
		    							category 	= article.Category,
		    							technology 	= article.Technology,
		    							
		    							htmlContent 	= article.Content		    							
		    						};
			return guidanceItemV3;
    	}*/
    	
    	public static string content(this Article_WS article)
    	{
    		return article.Content;
    	}
    	
    	public static Article_WS content(this Article_WS article, string newContent)
    	{
    		article.Content = newContent;
    		return article.save();
    		//var guidanceItemV3 = article.guidanceItemV3();
    	//	article.Library.tmWebServices.webServices.UpdateGuidanceItem(guidanceItemV3);
    		//return article;
    	}
    	
    	public static Article_WS save(this Article_WS article)
    	{
    		"SAVING Article_WS".info();
    		var guidanceItemV3 =  article.Library.tmWebServices.webServices.GetGuidanceItemById(article.Id.str()); 
    		
    		guidanceItemV3.title 			= article.Title;
			guidanceItemV3.guidanceItemId 	= article.Id;
		    									    							
			guidanceItemV3.rule_Type 		= article.Type;
			guidanceItemV3.phase 			= article.Phase;
			guidanceItemV3.category 		= article.Category;
			guidanceItemV3.technology 		= article.Technology;
		    							
			guidanceItemV3.htmlContent 		= article.Content;
			
			article.Library.tmWebServices.webServices.UpdateGuidanceItem(guidanceItemV3);
    		return article;
    	}
    	
    }
    
    public static class API_TeamMentor_WebServices_ExtensionMethods_Edit_Data
    {
    
    	//Add
    	
    	public static Folder_WS add_Folder(this Library_WS library, string name)    
    	{
    		return library.add_Folder(null, name);
    	}
    	
    	public static Folder_WS add_Folder(this Library_WS library, Folder_WS parentFolder, string name)    
    	{
    		var parentId = (parentFolder.notNull()) 
    							? parentFolder.Id
    							: Guid.Empty;    		
    		var newFolder = library.tmWebServices.webServices.CreateFolder(library.Id, parentId, name);
    		return newFolder.folder(library);
    	}
    		
    	public static View_WS add_View(this Library_WS library, string name)  
    	{
    		return library.add_View(null, name);	
    	}
    	
    	public static View_WS add_View(this Folder_WS folder, string name)  
    	{
    		return folder.Library.add_View(folder,name);
    	}
    	
    	public static View_WS add_View(this Library_WS library, Folder_WS folder, string name) 
    	{
    		var folderId = (folder.notNull()) 
    							? folder.Id
    							: Guid.Empty;   
		    	var view = new View();
				view.library = library.Id.str();
				view.caption = name;
				view.id = Guid.Empty.str();
			var newView = library.tmWebServices.webServices.CreateView(folderId,view);
			return newView.view(library, folder);
		}
		
		public static Article_WS add_Article(this View_WS view, string title, string content)
		{
			return view.add_Article(title, content, "","","","");			
		}
		
		public static Article_WS add_Article(this View_WS view, string title, string content, string technology, string phase, string type, string category) 
		{
			var article = view.Library.add_Article(title, content, technology, phase, type, category);			
			view.Library.tmWebServices.webServices.AddGuidanceItemsToView(view.Id, new Guid[] { article.Id });
			article.View = view;
			return article;
		}
		
		public static Article_WS add_Article(this Library_WS library, string title, string content, string technology, string phase, string type, string category) 
		{
			var guidanceItem = new GuidanceItem_V3();
			guidanceItem.guidanceItemId = Guid.Empty;
			guidanceItem.title = title;
			guidanceItem.htmlContent = content;
			guidanceItem.technology = technology;
			guidanceItem.phase = phase;
			guidanceItem.rule_Type = type;
			guidanceItem.category = category;
			
			guidanceItem.libraryId = library.Id;


			var guid = library.tmWebServices.webServices.CreateGuidanceItem(guidanceItem);
			
			return guid.article(library);
		}
		
		//Delete
		
		public static API_TeamMentor_WebServices delete(this Library_WS library)
		{
			var tmWebServices = library.tmWebServices;
			if (tmWebServices.webServices.DeleteLibrary(library.Id))
				"[API_TeamMentor_WebServices] library deleted OK: {0} - {1}".info(library.Name, library.Id);
			return tmWebServices;
		}
    }
    
    	
    
    public static class API_TeamMentor_WebServices_ExtensionMethods_Users
    {
    	public static Guid login(this API_TeamMentor_WebServices tmWebServices, string username, string password)
    	{
    		return tmWebServices.webServices.Login_PwdInClearText(username, password);
    	}
    	
    	public static List<TMUser> users(this API_TeamMentor_WebServices tmWebServices)
    	{
    		return tmWebServices.webServices.GetUsers().toList();
    	}
    	
    	public static TMUser user(this API_TeamMentor_WebServices tmWebServices, string userName)
    	{
    		return tmWebServices.webServices.GetUser_byName(userName);
    	}    	
    	
    	public static int add_User(this API_TeamMentor_WebServices tmWebServices, string username, 	string passwordHash)
    	{
    		return tmWebServices.add_User(username,passwordHash,"","","",0,"","",""); 
    	}
    	
    	public static int add_User(this API_TeamMentor_WebServices tmWebServices, string userName, 	string passwordHash, 
    																			  string email,  	string firstName,
    																			  string lastName,	int    groupId,
    																			  string title,		string company,
    																			  string note)
		{
    		var newUser = new NewUser();
			newUser.username = userName;
			newUser.passwordHash = passwordHash;
			newUser.note = note;			
			
			var userId =  tmWebServices.webServices.CreateUser(newUser);
			if (userId >0)
				if (tmWebServices.webServices.UpdateUser(userId, userName, firstName, lastName, title, company, email, groupId))
					return userId;
			return 0;
		}
		public static bool delete_User(this API_TeamMentor_WebServices tmWebServices, string userName)
		{
			var tmUser = tmWebServices.user(userName);
			if(tmUser.notNull())
				return tmWebServices.webServices.DeleteUser(tmUser.UserID);
			return false;
		}
		public static bool delete_User(this API_TeamMentor_WebServices tmWebServices, int userId)
		{
			return tmWebServices.webServices.DeleteUser(userId);
		}
    }
}
        