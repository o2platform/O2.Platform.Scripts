// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.Views.ASCX.CoreControls;

//O2File:O2MediaWikiApi.cs


namespace O2.XRules.Database.APIs
{
    public class ascx_MediaWiki_Backup : Control
    {    
	  	public O2MediaWikiAPI WikiApi {get ; set;}
	    
	    public TextBox BackupFolder_TextBox { get; set; }
	    public ascx_Directory Backup_Directory { get; set; }
	    public Button StartBackup_Button { get; set; }
	    public Button CancelBackup_Button { get; set; }	    
		public CheckBox BackupPages_CheckBox { get; set; }
		public CheckBox BackupFiles_CheckBox { get; set; }
		public CheckBox BackupCategoryPages_CheckBox { get; set; }
		public CheckBox BackupTemplates_CheckBox { get; set; }
		public CheckBox BackupUsers_CheckBox { get; set; }
		
		public ProgressBar Status_ProgressBar { get; set; }
		
		public bool CancelBackup { get; set;}
		
		//public static void launchGui()
		//{
		//	O2Gui.open<ascx_MediaWiki_Backup>("MediaWiki backup", 500,500).buildGui(new O2MediaWikiAPI());
		//}
		
	    public ascx_MediaWiki_Backup()
	    {
	    }
	    
	  	public ascx_MediaWiki_Backup(O2MediaWikiAPI wikiApi)
	  	{	  	
	  		buildGui(wikiApi);
	  	}
		  		  
		  
		public ascx_MediaWiki_Backup buildGui(O2MediaWikiAPI wikiApi)
		{			
			WikiApi = wikiApi;		
						
			//var controls = this.add_1x1(false);
			//var configPanel = controls[0].add_Panel();
			//controls[1].add_LogViewer();
			var configPanel = this.add_Panel();					
			configPanel.add_Label("Folder to store backup files",5,0);
			BackupFolder_TextBox  = configPanel.add_TextBox().top(25);
			BackupFolder_TextBox.align_Right(configPanel)
							 	.widthAdd(-5);
							 	
			BackupPages_CheckBox = configPanel.add_CheckBox("Backup content pages", 50, 0, (value)=>{}).autoSize().tick();	
			BackupFiles_CheckBox = configPanel.add_CheckBox("Backup files", 70, 0,(value)=>{}).autoSize().untick();
			BackupCategoryPages_CheckBox = configPanel.add_CheckBox("Backup category Pages", 90, 0,(value)=>{}).autoSize().tick();
			BackupTemplates_CheckBox = configPanel.add_CheckBox("Backup template pages", 110, 0,(value)=>{}).autoSize().tick();
			BackupUsers_CheckBox = configPanel.add_CheckBox("Backup users pages", 50, 150,(value)=>{}).autoSize().tick();
			
			StartBackup_Button = configPanel.add_Button("Start Backup",140,0)
											.onClick(backup);
			CancelBackup_Button = configPanel.add_Button("Cancel Backup",140,100)
											 .onClick(cancelBackup).enabled(false);						 
			
			Status_ProgressBar = configPanel.add_ProgressBar(170,0);
			Status_ProgressBar.align_Right(configPanel)
							  .widthAdd(-5);
			
			var rightPanel = configPanel.insert_Right<Panel>(configPanel.width()/3);	// there is a small gui frezee here which needs to be solved
			Backup_Directory = rightPanel.add_Directory(@"C:\O2");
			
			setup();
			
			return this;
		}
		
		public void setup()
		{
			var backupFolder = @"C:\O2\_USERDATA\WikiBackup".pathCombine(WikiApi.typeName())
														    .pathCombine(Files.getFileSaveDateTime_Now());
			BackupFolder_TextBox.set_Text(backupFolder);
															
		}
				
		public void backup()
		{			
			O2Thread.mtaThread(
				()=>{
						StartBackup_Button.enabled(false);	  	
						CancelBackup_Button.enabled(true);	  	
						CancelBackup = false;
						
					  	backup(BackupFolder_TextBox.get_Text(),
					  		   BackupPages_CheckBox.value(),
					  		   BackupFiles_CheckBox.value(),
					  		   BackupCategoryPages_CheckBox.value(),
					  		   BackupTemplates_CheckBox.value(),
					  		   BackupUsers_CheckBox.value());
					  						  						  	
					  	StartBackup_Button.enabled(true);
					  	CancelBackup_Button.enabled(false);	  	
					 });
		}
		
		public void cancelBackup()
		{
			"Canceling MediaWik Backup".error();
			CancelBackup = true;
		}
		public void backup(string backupFolder, bool backupPages, bool backupFiles, bool backupCategoryPages, bool backupTemplates,  bool backupUsers)
		{				
			var o2Timer = new O2Timer("Backed up Wiki in").start();
			Backup_Directory.open(backupFolder);
			if (backupPages && CancelBackup.isFalse())
				backup_Pages(backupFolder);
			if (backupFiles && CancelBackup.isFalse())
				backup_Files(backupFolder);
			if (backupCategoryPages && CancelBackup.isFalse())
				backup_CategoryPages(backupFolder);
			if (backupTemplates && CancelBackup.isFalse())
				backup_Templates(backupFolder);
			if (backupUsers && CancelBackup.isFalse())
				backup_Users(backupFolder);
			//backupFolder.createDir();	
			o2Timer.stop(); 
		}
		
		public void backup_Pages(string backupFolder)
		{		
		
			var pages_BackupFolder = backupFolder.pathCombine("Pages"); 
			pages_BackupFolder.createDir();

			var categories =  WikiApi.pages();						
			savePages(backupFolder, pages_BackupFolder,categories, "pagesMappings.txt");
			
			
			/*var pages_BackupFolder = backupFolder.pathCombine("Pages"); 
			pages_BackupFolder.createDir();
			
			string pageMappings = "";

			// backup pages
			var pages =  WikiApi.pages();			
			"there are: {0} pages to backup".info(pages.size());
			foreach(var page in pages)
			{
				pageMappings += "{0}	{1}".format(page, page.base64Encode()).line();
				var saveFileName = "{0}.wikitext.txt".format(Files.getSafeFileNameString(page));
				WikiApi.raw(page).saveAs(pages_BackupFolder.pathCombine(saveFileName)); 
			}
			pageMappings.saveAs(backupFolder.pathCombine("pagesMappings.txt")); */
		}
		
		
		public void backup_Files(string backupFolder)
		{
			var files_BackupFolder = backupFolder.pathCombine("Files");
			files_BackupFolder.createDir();

			var files = WikiApi.allImages();
			"there are {0} files to download".info(files.size()); 
			Status_ProgressBar.maximum(files.size());
			Status_ProgressBar.value(0);
			foreach(var file in files)	
			{
				if (CancelBackup)
				{
					"In backup_Files, CancelBackup was set, so aborting backup".error();
					break;
				}
				var web = new Web();
				if (WikiApi.BasicAuth.valid())
 					web.Headers_Request.add("Authorization","Basic " + WikiApi.BasicAuth);
				web.downloadBinaryFile(file, files_BackupFolder);
				Status_ProgressBar.increment(1);
			}			

		}
		
		public void backup_CategoryPages(string backupFolder)
		{
			var categories_BackupFolder = backupFolder.pathCombine("Categories"); 
			categories_BackupFolder.createDir();

			var categories =  WikiApi.categoryPages();						
			savePages(backupFolder, categories_BackupFolder,categories, "categoryPagesMappings.txt");
		}
		
		public void backup_Templates(string backupFolder)
		{
			var templates_BackupFolder = backupFolder.pathCombine("Templates"); 
			templates_BackupFolder.createDir();

			var categories =  WikiApi.templatePages();						
			savePages(backupFolder, templates_BackupFolder,categories, "templatePagesMappings.txt");
		}
		
		public void backup_Users(string backupFolder)
		{
			var templates_BackupFolder = backupFolder.pathCombine("Users"); 
			templates_BackupFolder.createDir();

			var categories =  WikiApi.userPages();						
			savePages(backupFolder, templates_BackupFolder,categories, "userPagesMappings.txt");
		}
		
		public void savePages(string backupFolder, string targetFolder, List<string> pages, string mappingsFileName)
		{					
			string pageMappings = "";				
			"there are: {0} pages to save".info(pages.size());
			Status_ProgressBar.maximum(pages.size());
			Status_ProgressBar.value(0);
			foreach(var page in pages)
			{
				if (CancelBackup)
				{
					"In savePages, CancelBackup was set, so aborting backup".error();
					break;
				}	
				pageMappings += "{0}	{1}".format(page, page.base64Encode()).line();
				var saveFileName = "{0}.wikitext.txt".format(Files.getSafeFileNameString(page));
				WikiApi.raw(page).saveAs(targetFolder.pathCombine(saveFileName)); 
				Status_ProgressBar.increment(1);				
			}
			pageMappings.saveAs(backupFolder.pathCombine(mappingsFileName)); 
		}
    }
}