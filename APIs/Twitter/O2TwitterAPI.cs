using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.Views.ASCX.ExtensionMethods;
using TweetSharp.Twitter.Fluent;
using TweetSharp.Twitter.Model;
using TweetSharp.Model;
using TweetSharp.Twitter.Extensions;
using O2.XRules.Database.Utils;
using O2.Views.ASCX.DataViewers;

//O2File:ISecretData.cs
//O2File:Watin_IE.cs
//O2File:Watin_IE_ExtensionMethods.cs

//O2Ref:Hammock.dll
//O2Ref:Newtonsoft.Json.dll
//O2Ref:TweetSharp.dll
//O2Ref:TweetSharp.Twitter.dll
//O2Ref:System.Data.dll
//O2Ref:System.Xml.Linq.dll 
//O2Ref:System.Xml.dll


namespace O2.XRules.Database.APIs
{
    public class O2TwitterAPI
    {
        public string Username { get; set; }        
        internal string Password { get; set; }
        public OAuthToken OAuthToken { get; set; }
        
        public TwitterUser UserLoggedIn { get; set; }
        public ITwitterStatuses Statuses { get; set; }
        public IFluentTwitter Twitter { get; set;}
        public bool IsLoggedIn { get;set; }
        //public TwitterResult LastResult { get; set;} 
		
		//Twitter O2Platform app details
		public string OAUTH_CONSUMER_KEY  {get;set;}
		public string OAUTH_CONSUMER_SECRET  {get;set;}

		public string TestAccount_UserName  {get;set;} 
		public string TestAccount_Password  {get;set;}  
		public string TwitterOAuthTokensFolder  {get;set;} 

		public O2TwitterAPI()
		{
			setDefaultValues();
		}	
		
		public void setDefaultValues()
		{
			OAUTH_CONSUMER_KEY  = "64ODjhZI0cSDb32lLIACFA";	
			OAUTH_CONSUMER_SECRET = "3SuM5vKvfiN2DJjJdSEUMSnDCpFYx39wDEQM11iQtg";
			
			TestAccount_UserName = "Test_AlDsy";  // please don't abuse :)
			TestAccount_Password = "Super!!Password";
			TwitterOAuthTokensFolder = @"C:\O2\_USERDATA\Twitter".createDir();			
		}
        
        public string getTokenFileForUser(string username)
        {
        	return TwitterOAuthTokensFolder.pathCombine("{0}_TwitterOAuth.xml".format(username));
        }
        
        public string getAuthorizationUrl()
		{
			"[O2TwitterAPI] retriving Authorization Url".info();
			
			var twitter = FluentTwitter.CreateRequest()
		   							   .Configuration.UseHttps()  
		    						   .Authentication.GetRequestToken(OAUTH_CONSUMER_KEY, OAUTH_CONSUMER_SECRET);
		
			var response = twitter.Request();
		
			if (response.ResponseHttpStatusCode.neq(200))
			{
				"error in first twitter response".error();
				return null;
			} 
		
			var unauthorizedToken = response.AsToken(); 
		
			var url =  FluentTwitter.CreateRequest().Authentication.GetAuthorizationUrl(unauthorizedToken.Token);
			"[O2TwitterAPI] Authorization Url retrived: {0}".info(url);
			return url;
		}


		public OAuthToken getAuthToken(string autorizationUrl, string username, string password) 	
		{
			var sync = new AutoResetEvent(false);
			OAuthToken oauthToken = null; 
			var tokenFile = getTokenFileForUser(username);
			if (tokenFile.valid() && tokenFile.fileExists())
			{
				"[O2TwitterAPI] found cached token for user: {0}".info(username);
				return tokenFile.load<OAuthToken>(); 
			}
			
			var ie = autorizationUrl.ie();			  // will open a new instance of IE 
			//var ie = panel.add_IE();				  // will use an Embeded version of IE
			// configure IE to handle twitter redirect
			ie.beforeNavigate(
				(url)=> {
							"[O2TwitterAPI] in BeforeNavigate for: {0}".debug(url);
							if (url.starts("http://o2platform.com/?oauth_token="))
							{
								O2Thread.mtaThread(
									()=>{											
											var splitted = url.uri().Query.split("=");
											if(splitted.size()==2 && splitted[0] == "?oauth_token")
											{
												var token = splitted[1];
												"[O2TwitterAPI] Found Token: {0}".info(token); 
											
												var twitter = FluentTwitter.CreateRequest()
						    									   .Authentication.GetAccessToken(OAUTH_CONSUMER_KEY, OAUTH_CONSUMER_SECRET,token);
												
												oauthToken = twitter.Request().AsToken();												
												
												if (oauthToken.notNull())
												{ 													
													oauthToken.saveAs(tokenFile);
													"[O2TwitterAPI] OAuthToken saved to: {0}".info(tokenFile); 													
												}										        
											}
											sync.Set(); // continue 
										});
								"[O2TwitterAPI] Found O2Platform.com Twitter redirect, stoping IE request".debug();  
								return true;
							}
							return false;
						});

			//perform login section
			
			ie.open(autorizationUrl);
			
			if (ie.hasLink("Sign out"))
				ie.link("Sign out").click();
				
			if(ie.hasField("session[password]") && ie.hasField("session[username_or_email]"))
			{				
				ie.field("session[username_or_email]").value(username);
				ie.field("session[password]").value(password);
				ie.button("Allow").click();
			}
			
			if (sync.WaitOne(10000).isFalse())			// wait until the redirect has been processed
				"[O2TwitterAPI] OAuthToken request timeout".error();
			ie.close();	
			return oauthToken;
		}
					
		public OAuthToken getAuthToken(string autorizationUrl, ICredential credential) 	
		{					
			if (credential.isNull())
				credential = ascx_AskUserForLoginDetails.ask();
			if (credential.isNull())
			{
				"[O2TwitterAPI] No credentials provided".error();
				return null;
			}			
			return getAuthToken(autorizationUrl,credential.UserName, credential.Password);
		}

        
        public bool login(ICredential credential)
		{
			return login(credential.UserName, credential.Password);
		}
		
        public bool login(string username, string password)        
        {
        	Username = username;
            Password = password;           
            return login();
        }
        
        public bool login()
        {
        	try
        	{
        		"login to Twitter under user:{0}".info(Username);
        		var oauthToken = getAuthToken(getAuthorizationUrl(), Username, Password);
        		login(oauthToken);
            	/*this.Twitter = FluentTwitter.CreateRequest().AuthenticateAs(Username, Password); //.Statuses.OnUserTimeline().AsJson();            	            	
            	var response = this.Twitter.Account().VerifyCredentials().AsJson().Request();            	            	
            	IsLoggedIn = response.ok();
            	if (IsLoggedIn)
            	{            		
            		this.Statuses = this.Twitter.Statuses();
            		this.UserLoggedIn = response.AsUser();
            		"Sucessfully connected to twitter user: '{0}' (id:{1})".info(this.UserLoggedIn.Name, this.UserLoggedIn.Id);
            	}
            	else
            		"Failed to connect to twitter user {0}".error(Username);
            	*/	
            	return IsLoggedIn;
            }
            catch(Exception ex)
            {
            	ex.log("[in O2TwitterAPI.login");
            }
            return false;
        }
        
        public bool login(string oAuthFile)
        {
        	if (oAuthFile.fileExists().isFalse())
        		oAuthFile = getTokenFileForUser(oAuthFile);
        	if (oAuthFile.fileExists())
        	{
        		var oAuthToken = oAuthFile.load<OAuthToken>();
        		return login(oAuthToken);
        	}
			return login(getAuthorizationUrl(), null);
        }
        
        public bool login(OAuthToken oAuthToken)
        {
        	if (oAuthToken.isNull())
        	{
        		"[O2TwitterApi] in login, provided OAuthToken parameter was null".error();
        		return false;
        	}
        	try
        	{
        		OAuthToken  = oAuthToken;
        		"login to Twitter via OAuth under user:{0}".info(oAuthToken.ScreenName);
            	this.Twitter = FluentTwitter.CreateRequest()
            							    .AuthenticateWith(OAUTH_CONSUMER_KEY,
						                                      OAUTH_CONSUMER_SECRET,
						                                      oAuthToken.Token,oAuthToken.TokenSecret);
            	var response = this.Twitter.Account().VerifyCredentials().AsJson().Request();            	            	
            	IsLoggedIn = response.ok();
            	if (IsLoggedIn)
            	{            		
            		this.Statuses = this.Twitter.Statuses();
            		this.UserLoggedIn = response.AsUser();
            		"Sucessfully connected to twitter user: '{0}' (id:{1})".info(this.UserLoggedIn.Name, this.UserLoggedIn.Id);
            	}
            	else
            		"Failed to connect to twitter user {0}".error(Username);
            		
            	return IsLoggedIn;
            }
            catch(Exception ex)
            {
            	ex.log("[in O2TwitterAPI.login");
            }
            return false;
        }       
        
        public ITwitterStatuses authenticatedStatuses()
        {
        	this.Twitter = FluentTwitter.CreateRequest().AuthenticateWith(OAUTH_CONSUMER_KEY,
						                                      OAUTH_CONSUMER_SECRET,
						                                      OAuthToken.Token,OAuthToken.TokenSecret);
			this.Statuses =	this.Twitter.Statuses();
			return this.Statuses;
        }

        
    }

    public static class Twitter_ExtensionClasses
    {
    
    	public static List<TwitterStatus> tweets(this O2TwitterAPI twitterAPI)
    	{
    		return twitterAPI.tweets(20);
    	}
    	
    	public static List<TwitterStatus> tweets(this O2TwitterAPI twitterAPI, int itemsToFetch)
    	{
    		return twitterAPI.user_Timeline(itemsToFetch);
    	}
    	
    	public static List<string> tweetsText(this O2TwitterAPI twitterAPI)
    	{
    		return twitterAPI.tweetsText(20);
    	}
    	
    	public static List<string> tweetsText(this O2TwitterAPI twitterAPI, int itemsToFetch)
    	{
    		return (from twitterStatus in twitterAPI.user_Timeline(itemsToFetch)
    				select twitterStatus.Text)
    			   .ToList();
    	}
    	
    	public static List<TwitterStatus> user_Timeline(this O2TwitterAPI twitterAPI)
    	{
    		return twitterAPI.user_Timeline(20);
    	}
    	
    	public static List<TwitterStatus> user_Timeline(this O2TwitterAPI twitterAPI, int itemsToFetch)
    	{
			return twitterAPI.user_Timeline(itemsToFetch,-1);
    	}
    	
    	public static List<TwitterStatus> user_Timeline(this O2TwitterAPI twitterAPI, int itemsToFetch, long beforeId)
    	{
    		try
    		{    			    			
    			var tweetsFetched = new List<TwitterStatus>();
    			try
    			{
	    			while(tweetsFetched.size() < itemsToFetch)
	    			{
	    				"Fetching tweets before id: {0}".info(beforeId);	    			
		    			var homeTimeline = twitterAPI.Statuses.OnUserTimeline();
						if (beforeId >0)
			    				homeTimeline = homeTimeline.Before(beforeId);
						var tweets = homeTimeline.Take(itemsToFetch)
						    					 .AsJson()
						    					 .Request()
						    					 .AsStatuses()			
										 		 .ToList();
						"Received {0} tweets before id: {1}".info(tweets.size(),beforeId);								 		 
						if (tweets.size() ==0)
							break;
						tweetsFetched.AddRange(tweets);
						beforeId = tweets.lastTweet().Id -1;
					}
				}
				catch(Exception ex)
				{
					ex.log("in TwitterAPi.user_Timeline");
				}
				"TOTAL ReceiveD tweets {0}".debug(tweetsFetched.size());								 		 
				return tweetsFetched;
			}
			catch(Exception ex)
			{
				ex.log("in O2TwitterAPI.user_Timeline", true);
				return new List<TwitterStatus>();
			}			
    	}
    	
    	public static List<TwitterStatus> home(this O2TwitterAPI twitterAPI)
    	{
    		return twitterAPI.home(20);
    	}
    	
    	public static List<TwitterStatus> home(this O2TwitterAPI twitterAPI, int itemsToFetch)
    	{
    		return twitterAPI.home_Timeline(itemsToFetch);
    	}
    	
    	public static List<string> homeText(this O2TwitterAPI twitterAPI)
    	{
    		return twitterAPI.homeText(20);
    	}
    	
    	public static List<string> homeText(this O2TwitterAPI twitterAPI, int itemsToFetch)
    	{
    		return (from twitterStatus in twitterAPI.home_Timeline(itemsToFetch)
    				select twitterStatus.Text)
    			   .ToList();
    	}
    	
    	public static List<TwitterStatus> home_Timeline(this O2TwitterAPI twitterAPI)
    	{
    		return twitterAPI.home_Timeline(20);
    	}
    	
    	public static List<TwitterStatus> home_Timeline(this O2TwitterAPI twitterAPI, int itemsToFetch)
    	{    		
    		return twitterAPI.Statuses
    						 .OnHomeTimeline()    						 
    						 //.OnFriendsTimeline()
    						 .Take(itemsToFetch)    						 
    						 .AsJson()
    						 .Request()    						 
    						 .AsStatuses()    						
    						 .ToList();
    	}
    	
    	public static List<TwitterUser> followers(this O2TwitterAPI twitterAPI)
    	{
    		return  twitterAPI.Twitter
    						  .Users()
    						  .GetFollowers()
    						  .AsJson()
    						  .Request()
    						  .AsUsers()
    						  .ToList();
    	}
    	
    	public static List<TwitterUser> following(this O2TwitterAPI twitterAPI)
    	{
    		return  twitterAPI.Twitter
    						  .Users()
    						  .GetFriends()
    						  .AsJson()
    						  .Request()
    						  .AsUsers()
    						  .ToList();
    	}
    	
    	
    	    	
    	
    	/*public static List<TwitterStatus> timeline(this O2TwitterAPI twitterAPI)
    	{
    		return twitterAPI.Statuses.OnUserTimeline().AsJson().Request().AsStatuses().ToList();
    	}*/
    	
    	public static List<string> str(this List<TwitterStatus> twitterStatuses)
    	{
    		var tweets = new List<String>();
    		foreach(var twitterStatus in twitterStatuses)
    			tweets.add("{0} : {1} : {2}".format(twitterStatus.CreatedDate ,  twitterStatus.User.Name, twitterStatus.Text));
    		return tweets;
    	}
    	
    	public static List<string> str(this List<TwitterUser> twitterUsers)
    	{
    		var users = new List<String>();
    		foreach(var twitterUser in twitterUsers)
    			users.add("{0} : {1} : {2}".format(twitterUser.Name ,  twitterUser.Location, twitterUser.Description));
    		return users;
    	}
    	
    	public static bool ok(this TwitterResult result)
        {
            return result.ResponseHttpStatusDescription == "OK";
        }
        
        
	}
	
	public static class Twitter_ExtensionClasses_TwitterStatus
	{
		public static TwitterStatus lastTweet(this List<TwitterStatus> twitterStatuses)
		{
			if (twitterStatuses.size() > 0)
				return twitterStatuses.Last();
			return null;
		}
		
	}
	public static class Twitter_ExtensionClasses_Search
	{    
		public static List<TwitterSearchStatus> search(this O2TwitterAPI twitterAPI, string text)
		{
			return twitterAPI.Twitter.Search().Query()
					                 .Containing(text)  
					                 .Request()
					                 .AsSearchResult()
					                 .Statuses
					                 .ToList(); 
		}
		
		public static List<TwitterUser> search_forUser(this O2TwitterAPI twitterAPI, string text)
		{
			return twitterAPI.Twitter.Users() 
			                          .SearchFor(text) 
			                          .AsJson()
			                          .Request()
			                          .AsUsers()
			                          .ToList();
		}		
	}
	
	public static class Twitter_ExtensionClasses_NewTweets
	{    	
    	public static bool newTweet(this O2TwitterAPI twitterAPI, string tweetText)
    	{
    		return twitterAPI.update(tweetText);
    	}
    	
		public static bool update(this O2TwitterAPI twitterAPI, string updateString)
        {	
        	// not sure why I can 't user the twitterAPI.Statuses for this (but I'm getting an error on the next request to user_timeline)
        	//var result = FluentTwitter.CreateRequest().AuthenticateAs(twitterAPI.Username, twitterAPI.Password).Statuses()
            //						  .Update(updateString).AsJson().Request();
			var result = twitterAPI.authenticatedStatuses().Update(updateString).AsJson().Request();            						  
            return result.ok();
        }
                                
	}
	
	public static class Twitter_ExtensionClasses_WinForms
	{
        
        public static T add_TableList_With_Tweets<T>(this T control, string description, Func<List<TwitterStatus>> twitterStatusesCallback)
        	where T : Control
        {	
        	Action loadData = null;
        	loadData = 
        		()=>{
        				var twitterStatuses = twitterStatusesCallback();
        			    var tableList = control.add_TableList_With_Tweets(description,twitterStatuses);
			            tableList.add_ContextMenu()
            		 			 .add_MenuItem("Refresh",()=> loadData());
            		 			 
						/*tableList.afterSelect(
							(selectedItems)=>{ 
												"AFTER SELECT 2".info();
											//	show.info(selectedItems);
											 });*/
			        };            
           	loadData(); 		 
        	return control;
        }
        
        public static ascx_TableList add_TableList_With_Tweets<T>(this T control, string description, List<TwitterStatus> twitterStatuses)
        	where T : Control
        {
        	"[O2TwitterAPI]: In add_TableList_With_Tweets: {0}".info(description);			
	        control.clear();	        
			var tableList = control.add_TableList();																			
			tableList.add_Columns(new List<string> {"#","Date","User","Tweet Text"});
			var item = 1;
			foreach(var twitterStatus in twitterStatuses)   
			{
				var row = tableList.add_Row(new List<string> {item++.str(), twitterStatus.CreatedDate.str() ,  twitterStatus.User.Name, twitterStatus.Text });
				//row.infoTypeName();
			}
            tableList.makeColumnWidthMatchCellWidth();            
            return tableList;
        }
        
        public static ascx_TableList add_TableList_With_TwitterSearchStatus<T>(this T control, string description, List<TwitterSearchStatus> twitterSearchStatuses)
        	where T : Control
        {
        	"[O2TwitterAPI]: In add_TableList_With_TwitterSearchStatus: {0}".info(description);			
	        control.clear();	        
			var tableList = control.add_TableList();																			
			tableList.add_Columns(new List<string> {"#","Date","FromUserScreenName","ToUserScreenName","Tweet Text"});
			var item = 1;
			foreach(var twitterSearchStatus in twitterSearchStatuses)   
			{
				var row = tableList.add_Row(new List<string> {item++.str(), twitterSearchStatus.CreatedDate.str() ,  																		
																			twitterSearchStatus.FromUserScreenName.str(), 
																			twitterSearchStatus.ToUserScreenName ?? "", 																			
																			twitterSearchStatus.Text.str() });
				//row.infoTypeName();
			}
            tableList.makeColumnWidthMatchCellWidth();            
            return tableList;
        }
        
        public static T add_TableList_With_Users<T>(this T control, List<TwitterUser> twitterUsers)
        	where T : Control
        {
	        control.clear();
			var tableList = control.add_TableList();																			
			tableList.add_Columns(new List<string> {"#","Name","Location","Description"});
            var item = 1;
			foreach(var twitterUser in twitterUsers)   
				tableList.add_Row(new List<string> {item++.str(), twitterUser.Name ,  twitterUser.Location, twitterUser.Description});
            tableList.makeColumnWidthMatchCellWidth();
        	return control;
        }
        
        
    }
}
