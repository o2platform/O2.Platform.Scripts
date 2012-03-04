using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;   
using O2.Kernel;   
using O2.Kernel.ExtensionMethods; 
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Zip;
using O2.XRules.Database.Utils;
using NUnit.Framework;  
using SecurityInnovation.TeamMentor.WebClient;
using SecurityInnovation.TeamMentor.WebClient.WebServices;    
//O2Ref:nunit.framework.dll     

//O2File:Test_TM_Config.cs  
//O2File:TM_Test_XmlDatabase.cs

//O2File:_Extra_methods_Zip.cs


namespace O2.SecurityInnovation.TeamMentor.WebClient.JavascriptProxy_XmlDatabase
{		  
	[TestFixture] 
    public class Test_OnlineStorage : TM_Test_XmlDatabase
    { 
    	
    	static Test_OnlineStorage()    
    	{    		
    		TMConfig.BaseFolder = Test_TM.tmWebSiteFolder;    		
    	} 
    	
		public Test_OnlineStorage()   
    	{       		
    		//UserGroup.Admin.setThreadPrincipalWithRoles(); // set current user as Admin
    	}
    	     	  
    	[Test]  
    	public void IJavascriptProxy_XmlDb_GetAllLibraryIds_and_GetLibraryById()
    	{ 
    		var libraries = tmWebServices.GetAllLibraryIds();
    		Assert.That(libraries.notNull() , "libraries was null");    		
    		Assert.That(libraries.size() > 0 , "libraries was empty");    		
    		foreach(var library in libraries)
    		{ 
    			var libraryWithId = tmWebServices.GetLibraryById(library.guid());
    			Assert.That(libraryWithId.notNull(), "libraryWithId was null for library with Id: {0}".format(library));
    		}       		    
    	}  
    	  
    	[Test]  
    	public void IJavascriptProxy_XmlDb_CreateLibrary_and_UpdateLibrary()
    	{  
    		var newLibrary = new Library { 
    									id = Guid.NewGuid().str(),
    			 						caption = "temp_lib_{0}".format(6.randomLetters())
    								  };										  
    		tmWebServices.CreateLibrary(newLibrary);    		     		
    		var createdLibrary = tmWebServices.GetLibraryById(newLibrary.id.guid());    		
    		Assert.That(createdLibrary.notNull(), "could not fetch new Library with Id: {0}".format(newLibrary.id));    	
    		createdLibrary.caption += "_toDelete";    		  
    		tmWebServices.UpdateLibrary(createdLibrary);    		    		    		
    		var updatedLibrary = tmWebServices.GetLibraryById(createdLibrary.id.remove("library:").guid());
    		 
    		Assert.That(updatedLibrary.id == createdLibrary.id, "in updated, id didn't match created library");
    		Assert.That(updatedLibrary.caption == createdLibrary.caption, "in updated, caption didn't match");
    		Assert.That(updatedLibrary.delete == createdLibrary.delete, "in updated, delete didn't match");
    		Assert.That(updatedLibrary.id.contains(newLibrary.id), "in updated, id didn't match new library object");
    		Assert.That(updatedLibrary.caption != newLibrary.caption, "in updated, caption should be different that newLibrary");
    		
    		updatedLibrary.delete = true;    		     		
    		tmWebServices.UpdateLibrary(updatedLibrary);    		    		    		
    		var deletedLibrary = tmWebServices.GetLibraryById(updatedLibrary.id.remove("library:").guid());
    		Assert.IsNull(deletedLibrary, "deletedLibrary");    		    		
    	}    	    	    	    	 
    	  
    	[Test]  
    	public void IJavascriptProxy_LiveWS_CreateView_and_DeleteLibrary()
    	{    
    		var createdView = createTempView();   	
    		//show.info(createdView);  
    		//show.info(tmXmlDatabase.tmLibrary(createdView.library.guid()));
    		Assert.IsNotNull(createdView, "createdView");    		
    		var result = tmWebServices.DeleteLibrary(createdView.library.guid());
    		Assert.That(result, "failed to delete library");    		    		
    	} 
    	 
    	[Test]   
    	public void IJavascriptProxy_LiveWS_UpdateView() 
    	{  
    		var tempView = createTempView();   	    		
    		tempView.caption += "_{0}".format(6.randomLetters());
    		tmWebServices.UpdateView(tempView);
    		var updatedView = tmWebServices.GetViewById(tempView.id);
    		Assert.That(updatedView.viewId == tempView.id.guid(), "ids didn't match");
    		Assert.That(updatedView.caption == tempView.caption, "caption didn't match");    		
    		tmWebServices.DeleteLibrary(tempView.library.guid());     		    
    	}
    	 
    	[Test]   
    	public void IJavascriptProxy_LiveWS_CreateGuidanceItem()
    	{  
    		var libraryId = createTempLibrary();    		    	 	
    		var tempGuidanceItemId = createTempGuidanceItem(libraryId);    		    		
    		Assert.That(tempGuidanceItemId != Guid.Empty , "tempGuidanceItem was an EmptyGuid");
    		var guidanceItem = tmWebServices.GetGuidanceItemById(tempGuidanceItemId.str());     		    		
    		
    		Assert.That(guidanceItem.notNull(), "guidanceItem was null");
    		Assert.That(guidanceItem.guidanceItemId == tempGuidanceItemId, "ids didn't match");
    		Assert.That(guidanceItem.title.valid(), "title wasn't valid");
    		Assert.That(guidanceItem.htmlContent.valid()	, "content wasn't valid"); 
    		Assert.That(guidanceItem.topic.valid(), "topic wasn't valid"); 
    		Assert.That(guidanceItem.technology.valid(), "technology wasn't valid");  
    		Assert.That(guidanceItem.category.valid(), "category wasn't valid");  
    		Assert.That(guidanceItem.rule_Type.valid(), "ruleType wasn't valid"); 
    		Assert.That(guidanceItem.status.valid(), "ruleType wasn't valid"); 
    		Assert.That(guidanceItem.priority.valid(), "priority wasn't valid"); 
    		Assert.That(guidanceItem.status.valid(), "status wasn't valid"); 
    		Assert.That(guidanceItem.author.valid(), "author wasn't valid");     		
    		tmWebServices.DeleteLibrary(libraryId);    		  
    	}  
    	 
    	[Test]    
    	public void IJavascriptProxy_LiveWS_UpdateGuidanceItem()    
    	{      		     		
    		var libraryId = createTempLibrary();  			
    		"temp library created:{0}".info(libraryId); 
    		var tempGuidanceItemId = createTempGuidanceItem(libraryId);
    		
    		var tempGuidanceItem = tmWebServices.GetGuidanceItemById(tempGuidanceItemId.str());     		
    				
    		tempGuidanceItem.title += "_{0}".format(6.randomLetters());
    		tempGuidanceItem.htmlContent += "_{0}".format(6.randomLetters());
    		tempGuidanceItem.topic += "_{0}".format(6.randomLetters());
    		tempGuidanceItem.technology += "_{0}".format(6.randomLetters());			        		    		
    		var result = tmWebServices.UpdateGuidanceItem(tempGuidanceItem);    		
    		Assert.That(result, "UpdateGuidanceItem failed");
    		var updatedGuidanceItem = tmWebServices.GetGuidanceItemById(tempGuidanceItemId.str());    		    		
    		Assert.That(updatedGuidanceItem.guidanceItemId == tempGuidanceItem.guidanceItemId, "ids didn't match");
    		Assert.That(updatedGuidanceItem.title == tempGuidanceItem.title, "title didn't match");
    		Assert.That(updatedGuidanceItem.htmlContent == tempGuidanceItem.htmlContent, "newHtmlContent didn't match");    		
    		Assert.That(updatedGuidanceItem.topic == tempGuidanceItem.topic, "topic didn't match");    		
    		Assert.That(updatedGuidanceItem.technology == tempGuidanceItem.technology, "newHtmlContent didn't match");    		
    		
    		tmWebServices.DeleteLibrary(libraryId);     		    		    		    
    	} 
    	 
    	[Test]
    	public void IJavascriptProxy_LiveWS_DeleteGuidanceItem()
    	{
    		var libraryId = createTempLibrary(); 
    		var tempGuidanceItemId = createTempGuidanceItem(libraryId);    		
    		var newGuidanceItem = tmWebServices.GetGuidanceItemById(tempGuidanceItemId.str());    		
    		Assert.That(newGuidanceItem.delete.isFalse(), "delete should be false here ");    		    		
    		var result = tmWebServices.DeleteGuidanceItem(newGuidanceItem.guidanceItemId);    			    
    		Assert.That(result, "result was false");
    		var deletedGuidanceItem = tmWebServices.GetGuidanceItemById(newGuidanceItem.guidanceItemId.str());    		
    		Assert.IsNull(deletedGuidanceItem, "deletedGuidanceItem should be null");    		
    		tmWebServices.DeleteLibrary(libraryId);    		    		
    	} 
    	  
    	[Test]  
    	public void IJavascriptProxy_LiveWS_AddGuidanceItemsToView_and_RemoveGuidanceItemsFromView()
    	{ 
    		var createdView = createTempView();  
    		var viewId = createdView.id.remove("view:").guid();
    		var guidanceItemsInView = tmWebServices.GetGuidanceItemsInView(viewId);
    		Assert.That(guidanceItemsInView.size()==0, "here guidanceItemsInView should be zero, and it was '{0}'".format(guidanceItemsInView.size()));
    		var tempGuidanceItemId_1 = createTempGuidanceItem(createdView.library.guid());    		
    		var tempGuidanceItemId_2 = createTempGuidanceItem(createdView.library.guid());    		
    		var guidanceItemIds = new List<Guid> { tempGuidanceItemId_1,tempGuidanceItemId_2 };    		 
    		var result = tmWebServices.AddGuidanceItemsToView(viewId, guidanceItemIds);
    		Assert.That(result, "result was false");
    		"view ID: {0}".info(viewId);
    		guidanceItemsInView = tmWebServices.GetGuidanceItemsInView(viewId); 
    		Assert.That(guidanceItemsInView.size()==2, "here guidanceItemsInView should be two, and it was '{0}'".format(guidanceItemsInView.size()));
    		 
    		var tempGuidanceItemId_3 = createTempGuidanceItem(createdView.library.guid());    		
    		var tempGuidanceItemId_4 = createTempGuidanceItem(createdView.library.guid());    		
    		guidanceItemIds.Clear(); 
    		guidanceItemIds.add(tempGuidanceItemId_3).add(tempGuidanceItemId_4);
    		
    		result = tmWebServices.AddGuidanceItemsToView(createdView.id.remove("view:").guid(), guidanceItemIds);    		    		
    		Assert.That(result, "2nd result was false");
    		guidanceItemsInView = tmWebServices.GetGuidanceItemsInView(viewId);    		
    		Assert.That(guidanceItemsInView.size()==4, "here guidanceItemsInView should be 4, and it was '{0}'".format(guidanceItemsInView.size()));
    		      		
    		var guidanceIdsToRemove = ( from guidanceItem in guidanceItemsInView 
   										select guidanceItem.Id).toList();
   										
    		tmWebServices.RemoveGuidanceItemsFromView(viewId, guidanceIdsToRemove);    		
    		 
    		guidanceItemsInView = tmWebServices.GetGuidanceItemsInView(viewId);
    		Assert.That(guidanceItemsInView.size()==0, "after remove the guidanceItemsInView should be zero, and it was '{0}'".format(guidanceItemsInView.size()));
    	}    	    	
    	
    	// helper methods
    	public Guid createTempLibrary()
    	{
    		"creating temp library".info();
	    	var newLibrary = new Library { 	id = Guid.NewGuid().str(),
    										caption = "temp_lib_{0}".format(6.randomLetters()) };										  
			tmWebServices.CreateLibrary(newLibrary);  
			return newLibrary.id.guid();
    	} 
    	
    	public Folder_V3 createTempFolder(Guid libraryId)
    	{
    		return tmWebServices.CreateFolder(libraryId,default(Guid), "test folder");	
    	}
    	
    	public View createTempView()
    	{ 
    		"creating temp view".info();
    		var newView = new View  {
	    								caption = "test view", 
	    								creatorCaption = "guidanceLibrary", 	    								
	    								parentFolder = "a folder",
	    								criteria = "",	    								
	    								id = Guid.NewGuid().str(),
	    								library = createTempLibrary().str()
	    							 };
			var newFolder = createTempFolder(newView.library.guid());	    							 
    		tmWebServices.CreateView(newFolder.folderId, newView);   
    		var createdView = tmWebServices.GetViewById(newView.id);
			Assert.That(createdView.viewId.str().remove("view:") == newView.id.str(), "ids didn't match");
    		Assert.That(createdView.caption == newView.caption, "captions didn't match");
    		
    		return newView;
			//return createdView;
    	}
 
    	// there methods are a variation of the ones found in OnlineStorageHelpers.cs file
    	public Guid createTempGuidanceItem( Guid libraryIdGuid)
		{ 
			return createTempGuidanceItem(	libraryIdGuid, 
											"GI title",
											"GI images",  
											DateTime.Now, 
											"Topic..",  
											"Technology....", 
											"Category...",   
											"RuleType...", 
											"Phase...",
											"Priority...", 
											"Status.." , 
											"Author...", "GI HTML content");
		}        	
    	
    	public Guid createTempGuidanceItem(Guid libraryIdGuid, string title, string images, DateTime lastUpdate, string topic, string technology, string category, string ruleType, string phase, string priority, string status, string author, string htmlContent)
		{
			var guidanceType = Guid.Empty; //  tmWebServices.GetGuidanceTypeByName("Guideline").id.guid();   
			Guid creatorId = Guid.Empty; 
			var creatorCaption = "guidanceLibrary"; 					     // we can use either one of these creator values				 					  			
			var guidanceItem =  createTempGuidanceItem(libraryIdGuid, guidanceType, creatorId, creatorCaption, title, images, lastUpdate, topic, technology, category, ruleType, phase, priority, status, author, htmlContent);			
			var guidanceItemV3 = new GuidanceItem_V3(guidanceItem);																	
			
			var newGuidanceItemId = tmWebServices.CreateGuidanceItem(guidanceItemV3);			
			Assert.AreEqual(newGuidanceItemId, guidanceItemV3.guidanceItemId, "GuidanceItemId");
			"newGuidanceItemId : {0}".debug(newGuidanceItemId);
			Assert.That(newGuidanceItemId != Guid.Empty, "in createTempGuidanceItem, newGuidanceItemId was empty");
			return newGuidanceItemId;
		} 
		
		
		
		public GuidanceItem createTempGuidanceItem(Guid libraryIdGuid, Guid guidanceType, Guid creatorId, string creatorCaption, string title, string images, DateTime lastUpdate, string topic, string technology, string category, string ruleType, string phase, string priority, string status, string author, string htmlContent)
		{
			var newGuidanceId= Guid.Empty.next(8.randomNumbers().toInt());			
			
			var guidanceItem = newGuidanceItemObject(newGuidanceId, title, guidanceType , libraryIdGuid, creatorId, creatorCaption ,htmlContent,images,lastUpdate);
			guidanceItem.AnyAttr = new List<XmlAttribute>()
				.add_XmlAttribute("Topic", topic )
				.add_XmlAttribute("Technology", technology)
				.add_XmlAttribute("Category", category)
				.add_XmlAttribute("Rule_Type", ruleType)
				.add_XmlAttribute("Phase", phase)
				.add_XmlAttribute("Priority", priority) 
				.add_XmlAttribute("Status", status) 
				.add_XmlAttribute("Author", author)
				.ToArray();
			
			//liveWS_tmWebServices.CreateGuidanceItem(guidanceItem, htmlContent);  
			"Created GuidanceItem with the title'{0}' ".info(title);
			return guidanceItem;
			//return newGuidanceId; 
		}
		
		public static GuidanceItem newGuidanceItemObject(Guid id, string title, Guid guidanceType, Guid library, Guid creator, string creatorCaption, string content, string images, DateTime lastUpdate)
		{
			var guidanceItem = new GuidanceItem() { id =id.str(),  													
													title = title, 
													guidanceType = guidanceType.str(),	
													library = library.str(),
													creator = creator.str(),
													creatorCaption = creatorCaption, 
													content = content,
													images = images, 
													lastUpdate = lastUpdate
												  };														  
			return guidanceItem;
		}

  		[Test]  
    	public void delete_Temp_Libraries()
    	{   
    		var libraries = tmWebServices.GetLibraries();
    		
    		Action<TM_Library> deleteLibrary =
    			(library)=> {
    							"deleting library: {0}".info(library.Caption);
    							var result = tmWebServices.DeleteLibrary(library.Id);
								Assert.That(result, "failed to delete library");    		
		    				};
    		
    		"Before delete there where {0} Libraries".debug(libraries.size());
    		foreach(var library in libraries)
    			if (library.Caption.starts("temp_lib_"))    			
    				deleteLibrary(library);    				    			    				
    		
    		 var libraries_Now = tmWebServices.GetLibraries();
    		"After delete there are {0} Libraries ({1} were deleted)".debug(libraries_Now.size() , libraries.size() - libraries_Now.size());    			    		
    	} 
    	
    	[Test]
    	public void create_Libraries_Backup()
    	{
    		var libraryName = "temp_lib_createAndBackup"; 
    		var newLibrary = tmWebServices.CreateLibrary(new Library() { caption = libraryName });
			Assert.That(newLibrary.notNull());
			var newGuidanceItem = new GuidanceItem_V3() { libraryId = newLibrary.libraryId };								
			tmWebServices.CreateGuidanceItem(newGuidanceItem);
			var backupFile = tmXmlDatabase.xmlDB_Libraries_BackupLibrary(newLibrary.libraryId);
			Assert.IsTrue(backupFile.fileExists());
			var backedUpfiles = new zipUtils().getListOfFilesInZip(backupFile);
			Assert.AreEqual(backedUpfiles.size(), 2, "there should be two files backup-ed files");
			tmWebServices.DeleteLibrary(newLibrary.libraryId);	
    	}
    	
    	[Test]
    	public void check_If_Backup_is_Created_on_Library_Delete()
    	{    	
    		var libraryName = "temp_lib_createAndBackup";     
    		//Create Library          
			var newLibrary = tmWebServices.CreateLibrary(new Library() { caption = libraryName }); 
			//Delete Library 
			var deleteResult = tmXmlDatabase.xmlDB_DeleteGuidanceExplorer(newLibrary.libraryId);   
			Assert.IsTrue(deleteResult.fileExists(), "backupFile existed");
			
			Assert.IsNull(tmWebServices.GetLibraryById(newLibrary.libraryId), "newLibrary.libraryId shouldn't exist by now");
		}    	
	}       
}