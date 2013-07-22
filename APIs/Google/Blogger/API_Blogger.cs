// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Utils;
using FluentSharp.For_HtmlAgilityPack;
using FluentSharp.Watin;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using O2.External.IE.ExtensionMethods;
using O2.External.IE.Wrapper;
using WatiN.Core;

//O2Ref:FluentSharp.Watin.dll
//O2Ref:Watin.Core.dll
//O2Ref:Interop.SHDocVw.dll
//O2Ref:O2_External_IE.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:System.Xml.dll
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll
//O2Ref:Microsoft.mshtml.dll
 
namespace O2.XRules.Database.APIs
{
	public class API_Blogger
    {     
    	public WatiN_IE IE { get; set; }
    	public bool LoggedIn{ get; set; }
    	public Credential Credential { get; set; }
    	public string UserBlogMainPage { get; set; }
 
 
    	public string LoginPage { get; set; }
    	public string LogoutPage { get; set; }
    	public string DashboardPage { get; set; }       	
    	public string ExpectedTitle_LoginPage { get; set; }
    	public string ExpectedTitle_DashboardPage { get; set; }
    	public string ExpectedTitle_NewPost { get; set; }
 
    	public static string defaultCredentialsFile = @"C:\O2\_USERDATA\Accounts.xml";
    	public static string defaultCredentialsType = "Blogger";
 
    	public static void launchGui()
    	{
    		O2Thread.mtaThread(()=> API_Blogger.buildGui());
    	}
 
    	public  API_Blogger(WatiN_IE ie)
    	{
    		if (ie !=null)
    			IE = ie;
    		else
    			IE = "".ie(0,450,800,700);  
 
    		LoginPage = "https://www.blogger.com/start";
    		LogoutPage = "http://www.blogger.com/logout.g";
    		DashboardPage = "http://www.blogger.com/home";    		
    		ExpectedTitle_LoginPage = "Blogger: Create your free blog";
    		ExpectedTitle_DashboardPage = "Blogger: Dashboard";
    		ExpectedTitle_NewPost = "blog - Create Post";
    	}
 
    	public  API_Blogger() : this (null)
    	{
 
    	}
 
    	public API_Blogger login()
    	{
    		return login("");
    	}
 
    	public API_Blogger login(string credentialsFile)
    	{
    		if (credentialsFile.fileExists())
    			this.Credential = credentialsFile.credential("Blogger");    		
    		return login(this.Credential);    		
    	}
 
    	public API_Blogger login(string username, string password)
    	{
    		return login(new Credential(username, password));
    	}
 
    	public API_Blogger login(Credential credential)
    	{    	
    		if (credential == null)
    			credential = IE.askUserForUsernameAndPassword();      	
    		this.Credential = credential;
    		IE.open(LoginPage);     		
    		if (IE.url() == LoginPage && IE.title()== ExpectedTitle_LoginPage)
    		{
    			if (IE.hasButton("Sign in"))
    			{
    				IE.set_Value("Email",this.Credential.username());
    				IE.set_Value("Passwd",this.Credential.password());    			
    				IE.click("Sign in");    				   				   				   			
    			}
    		}
    		if (inDashBoard())
			{
				UserBlogMainPage =  IE.link("View Blog").url();
				"[BloggerAPI]: logged in as user :{0}".info(this.Credential.username());
				LoggedIn = true;
				return this;
			}
			this.Credential = null;
    		"[BloggerAPI]: an error occured during the login process".error();	
			return this;
    	}
 
    	public API_Blogger logout()
    	{    		
    		//IE.link("Sign out");
    		IE.open(LogoutPage);
    		if (inLoginPage())
    		{
    			// add another logout check
    			"[BloggerAPI]: current user has been logged out ".info();
    			return this;
    		}   
    		"[BloggerAPI]: an error occured during the logout process".error();
    		return this;
    	}
 
    	public API_Blogger dashboard()
    	{
    		IE.open(DashboardPage);
    		return this;
    	}
 
    	public API_Blogger newPost()
    	{
    		if (LoggedIn)
    		{
    			IE.click("New Post");
    			inNewPostPage();
    		}
    		return this;
    	}
 
    	public API_Blogger setNewPostContents(string title, string body)
    	{    		
    		if (inNewPostPage())
    		{
    			IE.click("Edit HTML");       			
    			IE.element<TextField>("postingTitleField").value(title);  				
				IE.element<TextField>("postingHtmlBox").value(body); 
				IE.click("Compose"); 
    		}
    		else
    			"[BloggerAPI]: in setNewPostContents, not in new post page".error();     		   			
			return this;
    	}
 
		public API_Blogger viewPreview()
    	{    		
    		if (inNewPostPage())
    		{
    			IE.click("Preview");  
			}
			return this;
		}
 
		public API_Blogger hidePreview()
		{    		
    		if (inNewPostPage())
    		{
    			IE.elements().@classes("modal-dialog preview").innerHtml("AAAAA"); 
    			// this is not working
    			//IE.click("Close Preview");  
			}
			return this;
		}
 
		public API_Blogger publishPost()
		{
		if (inNewPostPage())
    		{
    			IE.click("Publish Post");  
    			IE.click("View Post");  
			}
			return this;			
		}
 
 
    	public bool inDashBoard()
    	{
    		if (IE.url().starts(DashboardPage) && IE.title(ExpectedTitle_DashboardPage))
    		{
    			"[BloggerAPI]: in dashboard".info();	
    			return true;
    		}
    		return false;
    	}
 
    	public bool inLoginPage()
    	{
    		if (IE.url(LoginPage) && IE.title(ExpectedTitle_LoginPage))
    		{
    			"[BloggerAPI]: in login page".info();	
    			return true;
    		}
    		return false;
    	}
 
    	public bool inNewPostPage()
    	{
    		if (IE.title().contains(ExpectedTitle_NewPost))
    		{
    			"[BloggerAPI]: in new post page".info();	
    			return true;
    		}
    		return false;
 
    		//if (true || IE.hasField("postingTitleField") && IE.hasField("postingHtmlBox"))
    			//"[BloggerAPI]: in setNewPostContents, could not find html elements to insert data".error();     		
    	}
 
    	private static API_Blogger buildGui()
    	{
    		var panel = O2Gui.open<Panel>("O2 Platform Blogger API", 800,600);
	   	panel.clear();
 
			var hostPanel = panel.add_1x1("write","live view",false);
			var newPostBody = (O2BrowserIE)hostPanel[0].add_O2_Browser_IE();
			var optionsTab = newPostBody.insert_Above<TabControl>(100);
			var bloggerView = hostPanel[1].add_IE();
			var bloggerApi = new API_Blogger(bloggerView);
 
			var newPostTitle = newPostBody.insert_Above<Panel>(20)
										  .add_Label("New post Title:")
										  .top(2)
										  .append_TextBox("");
			newPostTitle.align_Right(optionsTab);
 
			var postTitle = "title";
			var postBody = "body";
			newPostTitle.set_Text(postTitle);
			newPostBody.open(postBody.saveWithExtension(".html")); 
            
			newPostBody.editMode(); 
			newPostTitle.onTextChange((text)=>	postTitle = text);		
			newPostBody.onTextChange((html) =>	postBody = html.htmlDocument().select("//body").innerHtml().str().trim());
 
 
			var actionsTab= optionsTab.add_Tab("Actions");  
			var credentialsFile = actionsTab.add_Label("Use credentials from file:",5,0)
										    .append_TextBox(defaultCredentialsFile);
			credentialsFile.align_Right(actionsTab);							   
 
			var credentialsType = actionsTab.add_Label("Credential Type:              ",30,0) 
					 					    .append_TextBox(defaultCredentialsType);
			credentialsType.align_Right(actionsTab);
 
 
 
 
			var loginLink = actionsTab.add_Link("login", 55,130,
				()=>{
						if (credentialsFile.get_Text().fileExists())
							bloggerApi.login(credentialsFile.get_Text()
										   .credential(credentialsType.get_Text()));
						else
							bloggerApi.login();
					}); 
 
			loginLink.append_Link("logout", ()=> bloggerApi.logout())
					 .append_Link("dashboard",()=> bloggerApi.dashboard())
					 .append_Link("new post page",()=> bloggerApi.newPost())
					 .append_Link("preview new post",
						()=>{
								bloggerApi.setNewPostContents(postTitle, postBody);
								bloggerApi.viewPreview();
							})
					 .append_Link("submit new post",()=> bloggerApi.publishPost());
 
 
 
			bloggerApi.dashboard();
			return bloggerApi;
    	}
    }       	    	    	   
}
