// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
//O2File:O2PlatformWikiAPI.cs
//O2File:O2MediaWikiApi.cs
//O2File:ascx_AskUserForLoginDetails.cs


namespace O2.XRules.Database.APIs
{
    public class ascx_MediaWiki_PageEditor_Simple : Control
    {    
    	public O2MediaWikiAPI WikiApi { get; set; }
    	
    	public TextBox WikiPage_TextBox  {get; set;}
    	
    	public RichTextBox WikiTextEditor { get; set; }
		public WebBrowser BrowserPreview { get; set; }
		public WebBrowser BrowserCurrent { get; set; }
		public ComboBox CurrentPageUrl { get; set; }
		public Button SaveButton { get; set; }
		public Label StatusLabel { get; set; }
		public ComboBox RecentChangesComboBox { get; set; }
		
		public static void lauchGui()
		{
			var pageEditor = O2Gui.open<ascx_MediaWiki_PageEditor_Simple>("MediaWiki Page Editor", 500,500)
								  .buildGui(new O2PlatformWikiAPI());
			pageEditor.openPage("Main_Page");
		}
		
		public ascx_MediaWiki_PageEditor_Simple()
		{
			
		}
		
		public ascx_MediaWiki_PageEditor_Simple(O2MediaWikiAPI wikiApi)
	  	{	  	
	  		buildGui(wikiApi);
	  	}
		  		  		  									
		
		public ascx_MediaWiki_PageEditor_Simple buildGui(O2MediaWikiAPI wikiApi)
		{			
			try
			{
				WikiApi = wikiApi;	
				
				
				var topPanel = this.add_Panel();						
			  	
			  	
			  	var horizontalSplitterPosition = topPanel.height()/2;
			  	var verticalSplitterPosition = topPanel.width()/2;
				var controls = topPanel.add_1x2("WikiText","Preview","Live (Current page)",false,horizontalSplitterPosition,verticalSplitterPosition); 
				WikiTextEditor = controls[0].add_RichTextBox();
				BrowserPreview = controls[1].add_WebBrowser();
				BrowserCurrent = controls[2].add_WebBrowser(); 
							
				WikiPage_TextBox = 	WikiTextEditor.insert_Above<TextBox>(50);	
				var bottomPanel = WikiPage_TextBox.insert_Below<Panel>(30);  			
				
				RecentChangesComboBox = WikiPage_TextBox.insert_Right<Panel>(290)
													    .add_Label("Recent Changes / Search")
													    .top(3)
													    .append_Control<ComboBox>()
													    //.dropDownList()						    
													    .width(150)
													    .top(0);
				Action<string> populateRecentChangesComboBox = 
					(filter) => {
									RecentChangesComboBox.backColor(Color.LightPink);								
									O2Thread.mtaThread(
										()=>{
												RecentChangesComboBox.clear();
												if (filter.valid().isFalse())
													RecentChangesComboBox.add_Items(WikiApi.recentPages());			  				
												else
													RecentChangesComboBox.add_Items(WikiApi.pages(filter));			  				
												RecentChangesComboBox.backColor(Color.White);												
											});
							  	};													    
				RecentChangesComboBox.onSelection(
		    		()=>{
		    				WikiPage_TextBox.set_Text(RecentChangesComboBox.get_Text());
		    				loadCurrentPage();
		    			});
		    	RecentChangesComboBox.onEnter(
		    		(text)=>{
		    					populateRecentChangesComboBox(text);	    					
		    				});
				//WikiTextEditor.insert_Below<ascx_LogViewer>(130);
																	
							
				//default values
				BrowserPreview.silent(true);
				BrowserCurrent.silent(true);
				//WikiPage_TextBox.set_Text("Test");			 			
				
				// add controls with events	 												
				
				CurrentPageUrl = BrowserCurrent.insert_Above<Panel>(20)
							 			        .add_LabelAndComboBoxAndButton("Current page","","open",showInCurrentBrowser)
							  					.controls<ComboBox>();
				
				
				CurrentPageUrl.insert_Item("www.google.com");			
				
				bottomPanel.add_Button("Load",0).onClick(
					()=>{		
							loadCurrentPage();						
						});
				
				
				bottomPanel.add_Button("Preview",0,100).onClick(
					()=>{						
							O2Thread.mtaThread( 
									()=>{
											BrowserPreview.set_Text(WikiApi.parseText(WikiTextEditor.get_Text(),true));
										});
						});
						
				SaveButton = bottomPanel.add_Button("Save",0,200).onClick(
								()=>{
										saveCurrentPage();		
									}); 
									
				StatusLabel = SaveButton.append_Label("").topAdd(3).autoSize();
				
				/*WikiTextEditor.onKeyPress(Keys.Enter, 
					(code)=>{
								O2Thread.mtaThread(()=>
								{			
									BrowserPreview.set_Text(WikiApi.parseText(code,true));
								});
							});*/
			
				WikiTextEditor.onKeyPress((key)=> 			// add suport to paste images form Clipboard
					{ 
						if (key == (Keys.Control | Keys.V))		
							return pasteImageFromClipboard();
						return false;
					}); 
					
				WikiPage_TextBox.onEnter((text)=>loadPage(text));			
				
				
				WikiTextEditor.add_ContextMenu()
							  .add_MenuItem("Wrap with source tag (defaults to lang=csharp)",
							  		()=>{
							  				WikiTextEditor.invokeOnThread(
							  					()=>{
							  						
							  							WikiTextEditor.SelectedText = "<source lang=csharp>".line() + 
							  															WikiTextEditor.SelectedText.line() +
							  															"</source>".line();
							  						});
							  			});
						 	  			
				populateRecentChangesComboBox("");
			}
			catch(Exception ex)
			{
				ex.log("in ascx_MediaWiki_PageEditor_Simple buildGui");
			}
			return this;					
		}
   
		public void loadCurrentPage()
		{
			var pageToEdit =  WikiPage_TextBox.get_Text();	
			loadPage(pageToEdit);													
		} 
		
		//use this just to set the page name (and not load it)
		public void setPage(string pageToLoad)
		{
			WikiPage_TextBox.set_Text(pageToLoad); 		//set the page name
			WikiTextEditor.set_Text("");				//clear the current contents
			
		}
		
		public void openPage(string pageToLoad)
		{				
			loadPage(pageToLoad);
		}
		 
		public void loadPage(string pageToLoad)
		{  
			WikiPage_TextBox.set_Text(pageToLoad);
			loadPage();
		}
		
		public void loadPage()
		{
			var pageToLoad = WikiPage_TextBox.get_Text();
			WikiTextEditor.backColor(Color.LightPink);
			StatusLabel.set_Text("Opening page: {0}".format(pageToLoad)).textColor(Color.Black);
			CurrentPageUrl.insert_Item("{0}/{1}".format(WikiApi.IndexPhp,pageToLoad));
			O2Thread.mtaThread(()=>
				{			
					var wikiText = WikiApi.wikiText(pageToLoad);														
					WikiTextEditor.set_Text(wikiText);
					if (wikiText.valid().isFalse())
						StatusLabel.set_Text("This is a new Page (there was no content retrived from the server").textColor(Color.DarkBlue);
					BrowserPreview.open("about:blank");
					BrowserCurrent.open("about:blank");
					WikiTextEditor.backColor(Color.White);
					//BrowserPreview.set_Text(WikiApi.parseText(wikiText,true));
					//BrowserCurrent.set_Text(WikiApi.html(pageToLoad));	  
				});
		}	
		
		public void saveCurrentPage()
		{
			var currentPage =  WikiPage_TextBox.get_Text();			
			if (WikiApi.loggedIn().isFalse())
			{
				StatusLabel.set_Text("You need to be logged in to save pages").textColor(Color.Red);				
				var credential = ascx_AskUserForLoginDetails.ask();
		   		if (credential.notNull())
		   		{
		   			WikiApi.login(credential.UserName, credential.Password);
		   			if (WikiApi.loggedIn())
		   				StatusLabel.set_Text("You are now logged in as user: {0}".format(credential.UserName)).textColor(Color.DarkGreen);				
		   			else
		   				StatusLabel.set_Text("Login failed for user: {0}".format(credential.UserName)).textColor(Color.Red);				
		   		}
			}
			else
			{
				StatusLabel.set_Text("saving page {0} ".format(currentPage)).textColor(Color.Black);
				
				// save content
				WikiApi.save(currentPage, WikiTextEditor.get_Text()); 
				 
				// reload content and show it
				var wikiText = WikiApi.wikiText(currentPage);					
				BrowserPreview.open("about:blank");
				//BrowserPreview.set_Text(WikiApi.parseText(wikiText,true));
				BrowserCurrent.set_Text(WikiApi.html(currentPage));	
				StatusLabel.set_Text("Page {0} saved".format(currentPage)).textColor(Color.DarkGreen);
			}
		}
		
		public void showInCurrentBrowser(string url)
		{
			O2Thread.mtaThread(
				()=>{
						"opening page: {0}".info(url);
						BrowserCurrent.open(url);
					});
		}
		public bool pasteImageFromClipboard()
		{
			
			if (Clipboard.ContainsImage())
			{ 
				"Image in Clipboard".debug();
				O2Thread.mtaThread( 
					()=>{
							var tempImageTag = "[[Uploading_Image_tmp{0}]]".format(5.randomNumbers());
							WikiTextEditor.insertText(tempImageTag); 
							var imageUrl = WikiApi.uploadImage_FromClipboard();
							var wikiTextForImage = WikiApi.getWikiTextForImage(imageUrl);												
							WikiTextEditor.replaceText(tempImageTag,wikiTextForImage); 
							
						});
				return true;
			}		 			 
			return false;
		} 
		
		public void buildSidePanel_Old(Control sidePanel)
		{

			//var defaultSecretDataLocation =  @"C:\O2\_USERDATA\O2TestUsers.xml";
			//var defaultAccountType = "O2Platform-Wiki";
			
			//var treeView = browser.insert_Left<TreeView>(350);
			
			//var treeView = sidePanel.add_TreeView();
			//treeView.showSelection();	 	
						
			//Chose Wiki targe
			//treeView.insert_Above<Panel>(20)
			//        .add_Link("O2Platform WIKI", 0,0 , setMediaWikiTo_O2Platform )
			//        .append_Link("OWASP WIKI", setMediaWikiTo_OWASP );
			
			
			//Secret Data Panel
			//var credentialsPanel = treeView.insert_Below<Panel>(75); 
			//var secretFileTextBox = credentialsPanel.add_Label("Secrets File:    ",3).append_TextBox(defaultSecretDataLocation);
			//secretFileTextBox.align_Right(credentialsPanel); 
			//var accountTypeTextBox = credentialsPanel.add_Label("Account Type: ", 25).append_TextBox(defaultAccountType);
			//accountTypeTextBox.align_Right(credentialsPanel); 
			
			//var loginStatus = credentialsPanel.add_Label("not logged in", 48,170);
			//credentialsPanel.add_Button("login", 45,84, 
			//	()=>{
			//			var secretData = secretFileTextBox.get_Text().deserialize<SecretData>();
			//			var accountType = accountTypeTextBox.get_Text();
			//			WikiApi.login(secretData.username(accountType),secretData.password(accountType));
			//			if (WikiApi.loggedIn())
			//				loginStatus.set_Text("Logged in as: {0}".format(secretData.username(accountType))).textColor(Color.Green);
			//			else
			//				loginStatus.set_Text("Login failded for: {0}".format(secretData.username(accountType))).textColor(Color.Red);
			//			return;
			//		});
					
			// Context menu
			
			/*
			var contextMenu = treeView.add_ContextMenu(); 
			contextMenu.add_MenuItem("delete selected page",
							()=>{
									var pageToDelete = treeView.selected().get_Text();
									if (pageToDelete.valid()) 
										if (WikiApi.deletePage(pageToDelete)) 
											treeView.remove_Node(treeView.selected());													
								});	
								
			contextMenu.add_MenuItem("reload all pages",
							()=>{
									treeView.clear();
									treeView.add_Nodes(WikiApi.allPages());
								}); 
								
			var renameMenuItem = contextMenu.add_MenuItem("rename");
			var renameTextBox = renameMenuItem.add_TextBox("rename");		   					   
			
			renameMenuItem.add_MenuItem("rename page with new title (set above)",
					()=> {
							var currentTitle = treeView.selected().get_Text();
							var newTitle = renameTextBox.get_Text(); 
							if (WikiApi.movePage(currentTitle, newTitle))
							{
								treeView.clear();
								treeView.add_Nodes(WikiApi.allPages());
							}
							//"renaming page '{0}' to '{1}'".info(currentTitle, newTitle);
						});
						
			treeView.add_Nodes(WikiApi.allPages()).afterSelect<string>(
					(page)=> 
							{
								WikiPage_TextBox.set_Text(page);
								loadPage(page);
								renameTextBox.set_Text(page);
								renameTextBox.width(page.size() * 7);
								//browser.set_Text(wikiApi.getPageHtml(page));
							});
*/
		}		
				
		
    }
}
