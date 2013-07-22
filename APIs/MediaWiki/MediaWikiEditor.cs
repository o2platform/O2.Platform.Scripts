// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using AvalonDock;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.WPF;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;

//O2File:OwaspWikiAPI.cs
//O2File:O2PlatformWikiAPI.cs
//O2File:ISecretData.cs
//O2File:WPF_Gui.cs
//O2File:API_AvalonDock.cs

//O2File:ascx_MediaWiki_ConfigAndLogin.cs
//O2File:ascx_MediaWiki_Backup.cs
//O2File:ascx_MediaWiki_PageEditor_Simple.cs

//O2Ref:FluentSharp.WPF.dll
//O2Ref:AvalonDock.dll
//O2Ref:PresentationCore.dll
//O2Ref:WindowsBase.dll
//O2Ref:PresentationFramework.dll
//O2Ref:WindowsFormsIntegration.dll
//O2Ref:System.Xaml.dll
//O2Ref:ICSharpCode.AvalonEdit.dll
//O2Ref:O2_External_IE.dll
//O2Ref:QuickGraph.dll
//O2Ref:GraphSharp.dll
//O2Ref:GraphSharp.Controls.dll
//O2Ref:WatiN.Core.1x.dll
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll
	
namespace O2.XRules.Database.APIs
{
    public class MediaWikiEditor : Panel
    {     
    	public String FileWithSecretData {get; set;}
    	
    	public O2MediaWikiAPI WikiApi {get ; set;}
    	
    	public ascx_MediaWiki_ConfigAndLogin ConfigAndLogin { get; set;}
    	public ascx_MediaWiki_Backup Backup { get; set;}
    	public ascx_MediaWiki_PageEditor_Simple EditPage { get; set;}
    	
    	/*public Panel EditUncategorizedPages  { get; set;}
    	public Panel EditUsers  { get; set;}
    	public Panel EditAllPages  { get; set;}
    	public Panel EditCategoriesPages  { get; set;}
    	public Panel EditTemplatePages { get; set;}
    	public Panel EditUsingCategories { get; set;}*/
    	
    	
    	//public SplitContainer TopSplitContainer { get; set; }
    	//public Panel RightPanel { get; set; }
		    	    	    			
    	public DockingManager MainDockingManager { get; set; }
    	public ResizingPanel MainResizingPanel { get; set; }
    	public DockableContent LeftMenuDockableContent { get; set; }    	    	    	
		public DocumentPane MainDocumentPane { get; set; }		
		public WPF_GUI WpfGui { get; set; }
		
		public string helpPageInO2PlatformWiki = "O2 Script/Tool - MediaWiki Editor";
		
    	public static MediaWikiEditor launch()
    	{    		
    		var wikiEditor = O2Gui.open<MediaWikiEditor>("O2 MediaWiki Editor",750,600)
    							  .buildGui();
    		//wikiEditor.setMediaWikiTo_OWASP();
    		//wikiEditor.setMediaWikiTo_O2Platform();
    		//wikiEditor.showLoginPanel();
    		//wikiEditor.showEditPage();
    		
    		//wikiEditor.showEditUsers();
    		return wikiEditor;
    	}
    	
    	public MediaWikiEditor()
    	{
    		FileWithSecretData = @"C:\O2\_USERDATA\O2TestUsers.xml";    		
    		WikiApi = new O2MediaWikiAPI();    		
    		//buildGui_Internal();
    		//setMediaWikiTo_O2Platform();
    		//setMediaWikiTo_OWASP();
    	}    	    	
    	
    	public void setMediaWikiTo_O2Platform()
    	{
    		//WikiApi = new O2PlatformWikiAPI();
    		//this.buildGui();
    	}
    	
    	public void setMediaWikiTo_OWASP()
    	{
    		WikiApi = new OwaspWikiAPI(false);
    		this.buildGui();
    	}
    	
		private void buildGui_Internal(Control topPanel)
		{		
			//buildGui(this);			
        	//var topPanel = this;//this.add_Panel();
        	//topPanel.insert_Below<Panel>(100).add_LogViewer();
        	
        	MainDockingManager = topPanel.add_DockingManager();  
			MainResizingPanel = MainDockingManager.add_ResizingPanel(); 
			
			MainResizingPanel.add_DockablePane();	// don't understand why I need to add this so that the SetResizeWidth works
 		
			//var dockablePane = MainResizingPanel.add_DockablePane(); 		
			//dockablePane.resizeWidth(200); 
			
			//LeftMenuDockableContent = dockablePane.add_DockableContent("Commands","Commands");
			
			MainDocumentPane = MainResizingPanel.add_DocumentPane();						
		}
		
		
		public MediaWikiEditor buildGui()
		{
			WpfGui = this.add_Control<WPF_GUI>();
			WpfGui.buildGui();			
			buildGui_Internal(WpfGui.WinFormPanel);
			
			showLogViewer();
			
			WpfGui.add_Section("Media Wiki Editor", "Welcome to the O2 MediaWiki Editor")
				  //.add_Link("Help",showHelp)
				  .add_Link("Login",showLoginPanel)
				  .add_Link("Edit Page", showEditPage)
				  .add_Link("Edit using Categories", showEditUsingCategories)
				  .add_Link("Edit Uncategorized Pages", showEditUncategorizedPages)
				  .add_Link("Edit Category Pages", showEditCategoryPages)
				  .add_Link("Edit Template Pages", showEditTemplatePages)
				  .add_Link("Edit ALL Pages", showEditAllPages)
				  .add_Link("Edit Users", showEditUsers)
				  .add_Link("Util: Raw WikiText", showRawWikiText)
				  .add_Link("Util: Backup",showBackup)
				  .add_Link("Util: Show O2 Log Viewer",showLogViewer);
			
/*			if(WikiApi.loggedIn())
				WpfGui.GuiSections[0].links().name("Edit Page").click(); 	
			else
				WpfGui.GuiSections[0].links().name("Login").click(); 
*/				
			//showLoginPanel();	// default page		
			//WpfGui.show_O2Browser();
			//WpfGui.show_O2Wiki(helpPageInO2PlatformWiki);
			return this;
		}
		
        /*public MediaWikiEditor buildGui_Old()
        {
        	return (MediaWikiEditor)this.invokeOnThread(
        		()=>{        					        	
			
						var sp = LeftMenuDockableContent.add_StackPanel();  
			
						sp.add_Xaml_Button("Login", showLoginPanel);
						sp.add_Xaml_Button("Edit Page", showEditPage);
						sp.add_Xaml_Button("Edit using Categories", showEditUsingCategories);			
						sp.add_Xaml_Button("Edit Uncategorized Pages", showEditUncategorizedPages);
						sp.add_Xaml_Button("Edit Category Pages", showEditCategoryPages);
						sp.add_Xaml_Button("Edit Template Pages", showEditTemplatePages);
						sp.add_Xaml_Button("Edit ALL Pages", showEditAllPages);			
						sp.add_Xaml_Button("Edit Users", showEditUsers);
						sp.add_Xaml_Button("Util: Raw WikiText", showRawWikiText);
						sp.add_Xaml_Button("Util: Backup",showBackup);
						sp.add_Xaml_Button("Util: Show O2 Log Viewer",showLogViewer);
						
						showLoginPanel();	// default page												     					
       					return this;
        			});
        }
        */
        /*public void buildCommandsPanel(Control panel)
        {
        	panel.add_Button("Login",0,0,80, 60,showLoginPanel).align_Right(panel);
        	panel.add_Button("Edit Pages",100,0,80, 60, showEditPage).align_Right(panel);
        	panel.add_Button("Backup",200,0,80, 60, showBackup).align_Right(panel);
        }*/
        
        public void hackToFixNewPanelWindowSizeBug()
        {
        	WpfGui.WinFormPanel.fill(false)			  
							   .height(WpfGui.WinFormPanel.height() + 1)
							   .fill(true);				 
        }
        
        public void showLogViewer()
        {
        	this.insert_Below<Panel>(100).add_LogViewer();
        }
        
        public void showHelp()  
        {
        	 var webBrowser = MainDocumentPane.add_DocumentContent("Help") 
			        	 				 	   .setAsActive() 
			        	 				 	   .add_WinForms_Control<Panel>()
			        	 				 	   .add_WebBrowser_Control();			        	 				         	 				 
			 //webBrowser.parent().parent().backColor(Color.White);  // because of a current bug prevents the webbrowser control from resizing to fill (on first view)
			 webBrowser.open(WpfGui.Wiki_O2.html(helpPageInO2PlatformWiki).saveWithExtension(".html"));			 
			 //WpfGui.show_O2Browser();  
			//WpfGui.show_O2Wiki(helpPageInO2PlatformWiki);        	 				 
        	 //WpfGui.show_O2Wiki(helpPageInO2PlatformWiki);				 
        			          
        }    
        
        public void showLoginPanel()   
        {        	       	 
        	var newControl = MainDocumentPane.add_DocumentContent("Login Panel")
        	 				 				 .setAsActive()
							 				 .add_WinForms_Control<ascx_MediaWiki_ConfigAndLogin>();
			hackToFixNewPanelWindowSizeBug();											
			if (newControl.notNull())				 
				newControl.buildGui(WikiApi, setWikiApi); 			
			else  
				"in showLoginPanel, could not create ascx_MediaWiki_ConfigAndLogin control".error();
        	
        	//RightPanel.clear();
        	/*if (ConfigAndLogin == null)
        		ConfigAndLogin = RightPanel.add_Control<ascx_MediaWiki_ConfigAndLogin>().buildGui(WikiApi, setWikiApi);
        	else
        		 RightPanel.add_Control(ConfigAndLogin);        		*/
        }   
        
        public void showEditPage() 
        {        	
        	MainDocumentPane.add_DocumentContent("Edit_Page", "Edit Page")
        					.setAsActive()
        					.add_WinForms_Control<ascx_MediaWiki_PageEditor_Simple>()
						   .buildGui(WikiApi);
						   
									
			//MainDocumentPane.SelectedPane = documentContent;		   
        
        	/*RightPanel.clear();
        	if (EditPage == null)
        		EditPage = RightPanel.add_Control<ascx_MediaWiki_PageEditor_Simple>().buildGui(WikiApi);
        	else
        		 RightPanel.add_Control(EditPage); */
        }
        
        public void showEditUsingCategories()
        {        	        	
        	var panel = MainDocumentPane.add_DocumentContent("Edit_Using_Categories", "Edit Using Categories")
			        					.setAsActive()
			        					.add_WinForms_Panel();						   
			createGui_EditUsingCategories(panel,"Categories", "Wiki Text");
			
        	/*RightPanel.clear();
        	if (EditUsingCategories == null)
        	{
        		EditUsingCategories = RightPanel.add_Panel();
        		createGui_EditUsingCategories(EditUsingCategories,"Categories", "Wiki Text");        		
			}
        	else
        		 RightPanel.add_Control(EditUsingCategories);*/
        }
                        
        public void showEditUncategorizedPages()
        {
        	var panel = MainDocumentPane.add_DocumentContent("Edit Uncategorized Pages")
			        					.setAsActive()
			        					.add_WinForms_Panel();						   
			showEditGui(panel,"All Content Pages", "Page's Wiki Text",WikiApi.uncategorizedPages);
			
			/*RightPanel.clear();
        	if (EditUncategorizedPages == null)
        	{
        		EditUncategorizedPages = RightPanel.add_Panel();
				showEditGui(EditUncategorizedPages,"All Content Pages", "Page's Wiki Text",WikiApi.uncategorizedPages);
			}
        	else
        		 RightPanel.add_Control(EditUncategorizedPages); */
        }
        
        public void showEditAllPages()
        {
        	var panel = MainDocumentPane.add_DocumentContent("Edit All Pages")
			        					.setAsActive()
			        					.add_WinForms_Panel();
			showEditGui(panel,"All Content Pages", "Page's Wiki Text",WikiApi.pages);			        					
			/*RightPanel.clear();
        	if (EditAllPages == null)
        	{
        		EditAllPages = RightPanel.add_Panel();
				showEditGui(EditAllPages,"All Content Pages", "Page's Wiki Text",WikiApi.pages);
			}
        	else
        		 RightPanel.add_Control(EditAllPages);           				        	*/
        }
        
        public void showEditUsers()
        {
        	var panel = MainDocumentPane.add_DocumentContent("Edit User Pages")
			        					.setAsActive()
			        					.add_WinForms_Panel();						   
			showEditGui(panel,"User list", "User's Wiki Text",WikiApi.users);        	
			/*
        	RightPanel.clear();
        	if (EditAllPages == null)
        	{
        		EditUsers = RightPanel.add_Panel();
				showEditGui(EditUsers,"User list", "User's Wiki Text",WikiApi.users);        	
			}
			else
        		 RightPanel.add_Control(EditUsers);           											 */
        }
        
        public void showEditCategoryPages()
        {
        	var panel = MainDocumentPane.add_DocumentContent("Edit Category Pages")
			        					.setAsActive()
			        					.add_WinForms_Panel();						   
			showEditGui(panel,"Categories list", "Category's Wiki Text",WikiApi.categories);			        					
			        					
        	/*RightPanel.clear();
        	if (EditCategoriesPages == null)
        	{
        		EditCategoriesPages = RightPanel.add_Panel();
				showEditGui(EditCategoriesPages,"Categories list", "Category's Wiki Text",WikiApi.categoryPages);        	
			}
			else
        		 RightPanel.add_Control(EditCategoriesPages);           											 */
        }
        
        public void showEditTemplatePages()
        {
        	var panel = MainDocumentPane.add_DocumentContent("Edit Template Pages")
			        					.setAsActive()
			        					.add_WinForms_Panel();						   
			showEditGui(panel,"Templates list", "Template's Wiki Text",WikiApi.templatePages);
			
        	/*RightPanel.clear();
        	if (EditTemplatePages == null)
        	{
        		EditTemplatePages = RightPanel.add_Panel();
				showEditGui(EditTemplatePages,"Templates list", "Template's Wiki Text",WikiApi.templatePages);        	
			}
			else
        		 RightPanel.add_Control(EditTemplatePages); */
        }
        
        public Control showEditGui(Control hostControl, string title1, string title2, Func<List<string>> getContent)
        {   
        	hostControl.clear();
        	var usersGui = hostControl.add_1x1(title1,title2,true, hostControl.width()/3);
        	
        	var pageEditor = usersGui[1].add_Control<ascx_MediaWiki_PageEditor_Simple>().buildGui(WikiApi); 
        	
        	Action<Control> loadData = 
        		(control)=>{
        						control.clear();
        						control.enabled(false);
        						O2Thread.mtaThread(
        						()=>{        				
			        					var content = getContent();
			        					var treeView = control.add_TreeViewWithFilter(content)
			        					       				  .afterSelect<string>((userPage)=> pageEditor.openPage(userPage)); 
			        					addEditMenuItemsToTreeView(treeView);
			        					control.enabled(true);       
			        				});
        					};
        	
			        	
			        	
			usersGui[0].insert_Below<Panel>(20)
					   .add_Link("Reload data",0,0,
							()=> loadData(usersGui[0]))
						.click();
			return hostControl;						        
        }                
        
        public void createGui_EditUsingCategories(Control hostControl, string title1, string title2)
        {
        	hostControl.clear();
        	var usersGui = hostControl.add_1x1(title1,title2,true,hostControl.width()/3);
        	
        	var pageEditor = usersGui[1].Parent.clear().add_Control<ascx_MediaWiki_PageEditor_Simple>().buildGui(WikiApi); 
        	
			var controls = usersGui[0].Parent.clear().add_1x1("Category Names","Pages in Selected Category",false);
			var Categories_TreeView = controls[0].add_TreeView();
			var PagesInCategories_TreeView = controls[1].add_TreeView();

        	MethodInvoker loadData = 
        		()=>{
						//Categories_TreeView.clear();
						Categories_TreeView =  controls[0].add_TreeViewWithFilter(WikiApi.categoryPages())
														    .afterSelect<string>(
							(value)=>{
										PagesInCategories_TreeView = controls[1].add_TreeViewWithFilter(WikiApi.pagesInCategory(value))
														    					.afterSelect<string>((page)=>pageEditor.openPage(page));
									 
						
										pageEditor.openPage(value); 
										addEditMenuItemsToTreeView(PagesInCategories_TreeView);	
									 });
									 
						addEditMenuItemsToTreeView(Categories_TreeView);
						
					 };
						//PagesInCategories_TreeView.clear();
						//Categories_TreeView.add_Nodes(WikiApi.categoryPages());					
					
			controls[1].insert_Below<Panel>(20)
					   .add_Link("Reload data",0,0,()=> loadData())
						.click();        					 
        }
        
        //always recreate this gui to make sure we are on the correct WikiApi
        public void showBackup()
        {        
        	var panel = MainDocumentPane.add_DocumentContent("Backup")
			        					.setAsActive()
			        					.add_WinForms_Panel();						   
			
			panel.add_Control<ascx_MediaWiki_Backup>().buildGui(WikiApi);						
        }
        
        public void showRawWikiText()
        {        
        	var defaultTopPanelText = "You can refresh the Html view using F5 or Ctrl+R (or via the context menu (right-click on WikiText))";
        	var panel = MainDocumentPane.add_DocumentContent("Raw Wiki Text")
			        					.setAsActive()
			        					.add_WinForms_Panel();	
			//panel.clear();
			//var wikiApi = new O2PlatformWikiAPI();
			
			var topPanel = panel.add_1x1(false);
			var rawWiki = topPanel[0].add_TextArea(); 
			var bottomPanel = topPanel[1].add_1x1x1("Pure Html", "Browser (pure html View)", "Browser (view using site's Styles)");
			var htmlViewer = bottomPanel[0].add_RichTextBox();
			var browserSimple = bottomPanel[1].add_WebBrowser(); 
			var browserWithSyles = bottomPanel[2].add_WebBrowser(); 
			
			Action<string> processWikiText = 
				(wikiText)=>{															
								var htmlCode = WikiApi.parseText(wikiText);
								htmlViewer.set_Text(htmlCode); 					
								browserSimple.set_Text(htmlCode); 
								browserWithSyles.set_Text(WikiApi.wrapOnHtmlPage(htmlCode)); 								
							};
							
			MethodInvoker refresh = 
				()=>{	
						topPanel[0].set_Text("Retrieving RawWiki Html code");
						rawWiki.backColor(Color.LightPink); 
						O2Thread.mtaThread(
							()=>{
									processWikiText(rawWiki.get_Text());
									rawWiki.backColor(Color.White);
									topPanel[0].set_Text(defaultTopPanelText);
								});
					};
			
			Action<KeyEventArgs> handlePressedKeys = 
           	(e)=>{           			
	           		if (e.KeyValue == 116 ||                                       // F5 (key 116) or
		                (e.Modifiers == Keys.Control && e.KeyValue == 'R'))            // Ctrl+R   it		                
		            {		                
		                refresh();
		            }
				};
           
        	rawWiki.KeyUp+= (sender,e) => handlePressedKeys(e);
			
			
			//rawWiki.onEnter(processWikiText);
			
			rawWiki.add_ContextMenu().add_MenuItem("Show Html for Wiki Text", refresh);
			
			rawWiki.set_Text("===Raw WikiText===".line() + 
							 "this is simple text".line() + 
							 "* this is a bullet point");
			refresh();
			//panel_add_1x1(			        					
        }

		public void addEditMenuItemsToTreeView(TreeView treeView)
        {
        	var contextMenu = treeView.add_ContextMenu(); 
			contextMenu.add_MenuItem("delete selected page (you must be an admin)",
							()=>{
									var pageToDelete = treeView.selected().get_Text();
									if (pageToDelete.valid()) 
										if (WikiApi.deletePage(pageToDelete)) 
											treeView.remove_Node(treeView.selected());													
								});	
								
		/*	contextMenu.add_MenuItem("reload all pages",
							()=>{
									treeView.clear();
									treeView.add_Nodes(WikiApi.allPages());
								}); 
		*/						
			var renameMenuItem = contextMenu.add_MenuItem("rename (user will copy, admin will move)");
			var renameTextBox = renameMenuItem.add_TextBox("rename");		   					   
			
			renameMenuItem.add_MenuItem("rename page with new title (set above)",
					()=> {
							var currentTitle = treeView.selected().get_Text();
							var newTitle = renameTextBox.get_Text(); 
							if (WikiApi.movePage(currentTitle, newTitle))
							{
								treeView.selected().set_Text(newTitle);								
							}
							//"renaming page '{0}' to '{1}'".info(currentTitle, newTitle);
						});
			treeView.afterSelect<string>(
					(page)=> 
							{								
								renameTextBox.set_Text(page);
								renameTextBox.width(page.size() * 7);								
							});						
        }

        
        
        
		public void setWikiApi(O2MediaWikiAPI newWikiApi)
		{
			//perform a shallow copy from newWikiApi to WikiApi)
			//"before:{0}".info(WikiApi.HostUrl);
			foreach(var property in WikiApi.type().properties())						
				Reflection_ExtensionMethods_Properties.prop(WikiApi,property.Name,newWikiApi.prop(property.Name));
			//"after:{0}".info(WikiApi.HostUrl);
		}
		
		
		


		/*
		public void buildSidePanel(Control sidePanel)
		{
			sidePanel.add_Control<ascx_MediaWiki_Categories>().buildGui(WikiApi);
		}*/
    }
    
}
