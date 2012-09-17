// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods; 
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.External.IE.ExtensionMethods;
using SHDocVw;
using WatiN.Core;
using O2.XRules.Database.Utils;

//O2File:WatiN_IE_ExtensionMethods.cs    
//O2File:WatiN_IE.cs

//O2Ref:Interop.SHDocVw.dll
//O2Ref:WatiN.Core.1x.dll
//O2Ref:O2_External_IE.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:System.Xml.dll
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll

 
namespace O2.XRules.Database.APIs
{
    public class API_EventBrite
    {     
        public bool IsLoggedIn { get; set;}
        
    	public WatiN_IE ie;    	    
    	public string defaultCredentialType  = "EventBrite";
 
 		public static API_EventBrite gui()
 		{
 			return API_EventBrite.createGui();
 		}
 
    	public API_EventBrite()
    	{
    		ie = "".ie(0,450,800,700);      		
    	}
    	
    	public API_EventBrite(WatiN_IE watinIe)
    	{
    		ie = watinIe;
    		
    	}
 
    	public API_EventBrite login()
    	{
    		return login(@"C:\O2\_USERDATA\Accounts.xml");
    	}
 
    	public API_EventBrite login(string fileWithLoginDetails)
    	{							
    		ICredential credential = fileWithLoginDetails.credential(defaultCredentialType); 
			if (credential==null)
			{
				"[API EventBrite] no file with credentials provided, or no credential of type {0} found".debug(defaultCredentialType);
				credential = ie.askUserForUsernameAndPassword();	
			}
			return login(credential.username(), credential.password());			
    	}
    	
    	public API_EventBrite login(string username, string password)
    	{
    		if (username.valid().isFalse())    		
    			"[API EventBrite] invalid username provided, aborting login sequence".error();    			
    		else
    		{    	
    			logout();
	    		ie.open("http://www.eventbrite.com/login");  			
	    		if (ie.hasField("email") && ie.hasField("passwd"))
	    		{	    		
					ie.field("email", username); 
					ie.field("passwd", password);		
							
					ie.buttons()[0].click();			    		    		
		    		checkLogin();    		 	
		    	}
		    	else
		    		"[API EventBrite] in login page, could not find fields: email or passwd".error();
    		}
    		return this;    	
    	}
 
    	public API_EventBrite logout()
    	{
    		ie.open("http://www.eventbrite.com/signout");
    		
    		if (ie.title("Online Event Registration – Sell Tickets Online with Eventbrite") && 
    			ie.hasLink("Log in"))
				"logout ok".info();
			else
				"logout failed".error();	
    		    		
			return this;
    	}
    	
    	public API_EventBrite createAccount()
    	{
    		var credentials = ie.askUserForUsernameAndPassword();
    		if (credentials.isNull())
    			return this;
	    	return createAccount(credentials.username(), credentials.password());	    	
    	}    	    	
    	 
    	public API_EventBrite createAccount(string email, string password)
    	{
    		ie.open("http://www.eventbrite.com/signup");  			
			ie.field("email", email); 
			ie.field("passwd1", password);		
						
			ie.buttons()[0].click();			

    		//ie.open("http://www.eventbrite.com/myevents");
    		
    		 if (checkLogin())
    		 	"[API EventBrite] account created ok for user: {0}".info(email);
    		 else
    		 	"[API EventBrite] failed to create account for for user: {0}".error(email);
    		 return this;
    	}
    	
    	public bool checkLogin()
    	{
    		ie.open("http://www.eventbrite.com/myevents");
    		IsLoggedIn = ie.hasLink("Logout");
    		if (IsLoggedIn)
    			"[API EventBrite] User is logged in".info();
    		else
    			"[API EventBrite] User is NOT logged in".error();
    		return IsLoggedIn;
    			
    	}
    	
    	public API_EventBrite homePage()
 		{
 			ie.open("http://www.eventbrite.com");
 			return this;
 		}
 
 		public API_EventBrite myEvents()
 		{
 			ie.open("http://www.eventbrite.com/myevents");
 			return this;
 		}
 		
 		public API_EventBrite createEvent()
 		{
 			ie.open("http://www.eventbrite.com/create");
 			return this;
 		}
 		
    	/*public API_BTOpenZone closeInNSeconds(int seconds)
    	{
    		ie.closeInNSeconds(seconds);     
    		return this;
    	}
 
    	public API_BTOpenZone waitNSeconds(int seconds)
    	{
    		ie.waitNSeconds(seconds);     
    		return this;
    	}*/
 
 		private static API_EventBrite createGui()
 		{ 			
			var topPanel = O2Gui.open<Panel>("EventBrite API", 500,500);
			var ie = topPanel.add_IE();
			ie.silent(true); 
			var eventBrite = new API_EventBrite(ie); 
			var actionsPanel = topPanel.insert_Above<Panel>(20); 
			
			actionsPanel.add_Link("login", 2,0, ()=> eventBrite.login())
						.append_Link("logout", ()=> eventBrite.logout())
						.append_Link("my events", ()=> eventBrite.myEvents())
						.append_Link("create event", ()=> eventBrite.createEvent())
						.append_Link("create account", ()=> eventBrite.createAccount());
			eventBrite.homePage();			 
			return eventBrite;
 		}
     }
}

