// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.CoreLib.Utils;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using O2.XRules.Database.Utils;

//O2File:O2MediaWikiAPI.cs
//O2File:O2PlatformWikiApi.cs
//O2File:OwaspWikiAPI.cs
//O2File:ISecretData.cs
//O2File:SecretData_ExtensionMethods.cs

namespace O2.XRules.Database.APIs
{
    public class ascx_MediaWiki_ConfigAndLogin : Control
    {    
	  	public O2MediaWikiAPI WikiApi {get ; set;}
	  	
	  	public Action<O2MediaWikiAPI> SetWikiApi {get;set;}		// we need to make sure the WikiApi objects are syncronized
	  	//config panel
	  	public ComboBox TargetMediaWiki_ComboBox { get; set; }  
	  	//login details panel
	  	public ComboBox Credential_ComboBox { get; set; }  
		public TextBox SecretsFile_TextBox { get; set; }
		public TextBox UserName_TextBox { get; set; }
		public TextBox Password_TextBox { get; set; }
		public TextBox MediaWikiUrl_TextBox { get; set; }  		
		public TextBox MediaWiki_API_php_TextBox { get; set; }
		public TextBox MediaWiki_Index_php_TextBox { get; set; }
		public Label LoggedInStatus_Label { get; set; }
		public Label MediaWikiUrlCheck_Label { get; set; }	    
		public string defaultSecretsFolder = @"C:\O2\_USERDATA\";
	    public string defaultSecretsFile = "Accounts.xml";
	    
	    public static void launchGui()
	    {	    	
	    	O2Gui.open<ascx_MediaWiki_ConfigAndLogin>("MediaWiki Config and Login", 400,400)
	    		.buildGui(new O2MediaWikiAPI(), (wikiApi)=>{});
	    }
	      
	    
	    public ascx_MediaWiki_ConfigAndLogin()
	    {	    	
	    	this.Width = 400;
	    	this.Height = 400;
	    }
	    
	  	public ascx_MediaWiki_ConfigAndLogin(O2MediaWikiAPI wikiApi, Action<O2MediaWikiAPI> setWikiApi)
	  	{	  	
	  		buildGui(wikiApi, setWikiApi);
	  	}		  						
		
		public ascx_MediaWiki_ConfigAndLogin buildGui(O2MediaWikiAPI wikiApi, Action<O2MediaWikiAPI> setWikiApi)
		{			
			WikiApi = wikiApi;	
			SetWikiApi = setWikiApi;
			var controls = this.add_1x1("Config", "Login Details",false); 
			var config_Panel = controls[0];
			var loginDetails_Panel = controls[1];
			
			//config_Panel
			TargetMediaWiki_ComboBox = config_Panel.add_Label("Target MediaWiki website",12,10)
												   .append_Control<ComboBox>()
												   .dropDownList()
												   .onSelection<string>(loadMediaWikiDetails);
												   
			MediaWikiUrl_TextBox = config_Panel.add_Label("MediaWiki Url:            ",40,10)
												   .append_TextBox("");
			
			MediaWiki_API_php_TextBox = config_Panel.add_Label("MediaWiki api.php:     ",60,10)
												   .append_TextBox("");			
			MediaWiki_Index_php_TextBox = config_Panel.add_Label("MediaWiki index.php: ",80,10)
												   .append_TextBox("");												   
			
			MediaWikiUrlCheck_Label = config_Panel.add_Link("Check MediaWiki Urls", 110,128,checkMediaWikiUrls)
												  .append_Label("")
												  .autoSize();
			
			MediaWikiUrl_TextBox.align_Right(config_Panel).leftAdd(2).widthAdd(-7);
			MediaWiki_API_php_TextBox.align_Right(config_Panel).widthAdd(-5);
			MediaWiki_Index_php_TextBox.align_Right(config_Panel).widthAdd(-5);

			//loginDetails_Panel			
			loginDetails_Panel.add_Label("A) Login using local config file:",20,10);
			SecretsFile_TextBox = loginDetails_Panel.add_TextBox(40,102,false)
												    .set_Text(defaultSecretsFolder.pathCombine(defaultSecretsFile))
												    .onEnter(loadCredentials);
												    			
			
			Credential_ComboBox = loginDetails_Panel.add_ComboBox(60,102).width(220).sorted();			
			
			Credential_ComboBox.onSelection<Credential>(loadCredentialDetails);
			loginDetails_Panel.add_Label("B) Login using username & password: ",100,10);
			UserName_TextBox = loginDetails_Panel.add_Label("Username:",125,35)
											   	 .append_TextBox("");
			Password_TextBox = loginDetails_Panel.add_Label("Password: ",150,35)
											   	 .append_TextBox("")
											   	 .isPasswordField();
						
			SecretsFile_TextBox.align_Right(loginDetails_Panel).widthAdd(-40);
						
			SecretsFile_TextBox.append_Control<Button>()	
							   .set_Text("...")
							   .width(30)
							   .heightAdd(-2)
							   .anchor_TopRight()
							   .onClick(()=>{ 
							   					var file = this.askUserForFileToOpen(defaultSecretsFolder,"Xml Files | *.xml");
							   					if (file.valid())
							   					{
							   						SecretsFile_TextBox.set_Text(file);
							   						loadCredentials(file);
							   					}
							   				});;
			
			UserName_TextBox.width(200);
			Password_TextBox.width(200);
									
			LoggedInStatus_Label = Password_TextBox.append_Link("Login", login)
												   .topAdd(5)
												   .append_Label("")
												   .autoSize();
			
			loadCredentials(SecretsFile_TextBox.get_Text());
			
			loadDefaultMediaWikiEngines();
			
			return this;						
		}
		
		public void loadCredentials(string secretsFile)
		{
			Credential_ComboBox.clear();			
			var credentials = secretsFile.credentials();
			foreach(var credential in credentials)
				Credential_ComboBox.add_Item(credential);
			Credential_ComboBox.selectFirst();
		}
		
		public void loadCredentialDetails(Credential credential)
		{
			UserName_TextBox.set_Text(credential.username());
			Password_TextBox.set_Text(credential.password());
		}
		
		public void loadDefaultMediaWikiEngines()
		{
			TargetMediaWiki_ComboBox.add_Item("O2-Platform.com");
			TargetMediaWiki_ComboBox.add_Item("OWASP.org");
			TargetMediaWiki_ComboBox.add_Item("Wikipedia.com");
			TargetMediaWiki_ComboBox.add_Item("Other MediaWiki based website");
			TargetMediaWiki_ComboBox.selectFirst();
		}
		
		public void loadMediaWikiDetails(string mediaWikiEngine)
		{
			switch(mediaWikiEngine)
			{
				case "O2-Platform.com":
					WikiApi = new O2PlatformWikiAPI();
					break;
				case "OWASP.org":
					//WikiApi = new O2MediaWikiAPI();
					//WikiApi.init("http://www.owasp.org/api.php");
					WikiApi = new OwaspWikiAPI();
					break;
				case "Wikipedia.com":
					WikiApi = new O2MediaWikiAPI();
					WikiApi.init("http://en.wikipedia.org/w/api.php");
					break;
				default :
					WikiApi = new O2MediaWikiAPI();					
					break;				
			}
			SetWikiApi(WikiApi);
			MediaWikiUrl_TextBox.set_Text(WikiApi.HostUrl);
			MediaWiki_API_php_TextBox.set_Text(WikiApi.ApiPhp);
			MediaWiki_Index_php_TextBox.set_Text(WikiApi.IndexPhp);
			if (MediaWikiUrl_TextBox.get_Text().valid())
				checkMediaWikiUrls();
		}
		
		public void checkMediaWikiUrls()
		{
			O2Thread.mtaThread(()=>
			{
				MediaWikiUrlCheck_Label.set_Text("Checking Urls: ").textColor(Color.Black);				
				try
				{								
					if (MediaWikiUrl_TextBox.get_Text().uri().getHtml().valid())
						MediaWikiUrlCheck_Label.append_Text(" Url is Ok , ").textColor(Color.Green);
					else
						MediaWikiUrlCheck_Label.append_Text(" Url failed , ").textColor(Color.Red);
						
					if (MediaWiki_API_php_TextBox.get_Text().uri().getHtml().valid())
						MediaWikiUrlCheck_Label.append_Text(" api.php is Ok , ");
					else
						MediaWikiUrlCheck_Label.append_Text(" api.php failed , ").textColor(Color.Red);
					
					if (MediaWiki_Index_php_TextBox.get_Text().uri().getHtml().valid())
						MediaWikiUrlCheck_Label.append_Text(" index.php is Ok");	
					else
						MediaWikiUrlCheck_Label.append_Text(" api.php failed , ").textColor(Color.Red);
										
					
				}
				catch(Exception ex)
				{
					ex.log("in checkMediaWikiUrls");
				}				
			});			
		}
		
		public void login()
		{					
			LoggedInStatus_Label.set_Text("trying to login to: {0}".format(MediaWikiUrl_TextBox.get_Text()))
								.textColor(Color.Black);
			var username = UserName_TextBox.get_Text();
			var password = Password_TextBox.get_Text();
						
			WikiApi.login( username,password).isFalse();
				
			if (WikiApi.loggedIn())
				LoggedInStatus_Label.set_Text("Logged in as user: {0}".format(username))
								    .textColor(Color.Green);
			else
				LoggedInStatus_Label.set_Text("Login failed for user {0}".format(username))
									.textColor(Color.Red);			
			SetWikiApi(WikiApi);								
		}
    }
}
