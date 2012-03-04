// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Data; 
using System.Data.SqlClient;  
using System.Collections.Generic;
using System.Diagnostics;   
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;   
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;   
using NUnit.Framework; 
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using SecurityInnovation.TeamMentor.WebClient;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using SecurityInnovation.TeamMentor.Authentication.ExtensionMethods;
 
//O2File:TM_Test_XmlDatabase.cs

namespace O2.SecurityInnovation.TeamMentor.WebClient.JavascriptProxy_XmlDatabase
{		 
	[TestFixture]
    public class Test_DbEngine  : TM_Test_XmlDatabase
    { 
    	static bool skipLongerTests { get; set;}
    	
     	static Test_DbEngine()
     	{     		
     		TMConfig.BaseFolder = Test_TM.tmWebSiteFolder;    		
     	}
     	 
    	public Test_DbEngine() 
    	{   
    		skipLongerTests = true;    		    	    		
    		UserGroup.Admin.setThreadPrincipalWithRoles(); // set current user as Admin
    	}     		
    	 
    	[Test]    
    	public void Test_XmlDatabase_Setup()
    	{
    		Assert.IsNotNull(tmXmlDatabase_JavascriptProxy,"JavascriptProxy");
    		var proxyType = tmXmlDatabase_JavascriptProxy.ProxyType; 
    		Assert.IsNotNull(proxyType,"proxyType");
    		Assert.AreEqual(proxyType,"TM Xml Database", "proxyType value");     		
    	}
    	
    	[Test]       	  
    	public void Test_getGuidanceExplorerObjects() 
    	{    
			var guidanceExplorers = TM_Xml_Database.Path_XmlLibraries.getGuidanceExplorerObjects();			
			Assert.IsNotNull(guidanceExplorers, "guidanceExplorers");
			Assert.That(guidanceExplorers.size()>0 , "guidanceExplorers was empty");			
			Assert.That(TM_Xml_Database.GuidanceExplorers_XmlFormat.size() > 0, "GuidanceExplorers_XmlFormat was empty");    		
    	}
    	
    	
    	[Test] 
    	public void Test_getLibraries()
    	{ 
    		//var guidanceExplorers = TM_Xml_Database.loadGuidanceExplorerObjects();    		
    		//Assert.That(guidanceExplorers.size() > 0, "guidanceExplorers was empty");
    		var guidanceExplorers = TM_Xml_Database.GuidanceExplorers_XmlFormat.Values.toList();
    		var tmLibraries = tmXmlDatabase.tmLibraries();
    		Assert.IsNotNull(tmLibraries,"tmLibraries"); 
    		for(var i=0;  i < guidanceExplorers.size() ; i++)
    		{
    			Assert.AreEqual(tmLibraries[i].Caption,  guidanceExplorers[i].library.caption, "caption");
    			Assert.AreEqual(tmLibraries[i].Id, guidanceExplorers[i].library.name.guid(), "caption");
    		}
    		Assert.That(TM_Xml_Database.GuidanceExplorers_XmlFormat.size()>0, "GuidanceExplorers_XmlFormat empty");    		
    	}
    	 
    	   
    	[Test] 
    	public void Test_getFolders()
    	{
    		//var guidanceExplorers = TM_Xml_Database.loadGuidanceExplorerObjects();    		
    		var libraryId = TM_Xml_Database.GuidanceExplorers_XmlFormat.Keys.first();
    		var guidanceExplorerFolders = TM_Xml_Database.GuidanceExplorers_XmlFormat[libraryId].library.libraryStructure.folder;    		
    		Assert.That(guidanceExplorerFolders.size() > 0,"guidanceExplorerFolders was empty");
    		
    		var tmFolders = tmXmlDatabase.tmFolders(libraryId);
    		Assert.IsNotNull(tmFolders,"folders"); 
    		Assert.That(tmFolders.size() > 0,"folders was empty");
    		//show.info(guidanceExplorerFolders);
    		//show.info(tmFolders);	
    		var mappedById = new Dictionary<Guid,Folder_V3>();
    		    		
    		foreach(var tmFolder in tmFolders)
    			mappedById.Add(tmFolder.folderId, tmFolder);
    			
    		//Add checks for sub folders	
    		foreach(var folder in guidanceExplorerFolders)
    		{
				Assert.That(mappedById.hasKey(folder.folderId.guid()), "mappedById didn't have key: {0}".format(folder.folderId));    				
				var tmFolder = mappedById[folder.folderId.guid()];				
				Assert.That(tmFolder.name == folder.caption);				
				Assert.That(tmFolder.libraryId == libraryId, "libraryId");	
    		}      		
    	} 
    	
    	//DC: these tests below where created during first version of the TM_Xml_Database and where made redundant 
    	//    during a code refactoring (when the guidance items cache file was introduced)
    	
    	/*[Test]
    	public string Test_getGuidanceItems_StandAlone()    	    	
    	{    
			var guidanceItems = tmXmlDatabase.loadGuidanceItems_StandAlone();
			Assert.IsNotNull(guidanceItems, "getGuidanceItems_StandAlone");
			Assert.That(guidanceItems.size()>0 , "getGuidanceItems_StandAlone was empty");						
			Assert.That(TM_Xml_Database.GuidanceItems_XmlFormat.size()>0, "TmXmlDatabase.GuidanceItems_XmlFormat was empty");
			//show.info(guidanceItems);
    		return "ok: Test_getGuidanceItems_StandAlone";
    	} 
    	
    	[Test]  
    	public string Test_loadGuidanceItems_All()
    	{     
    		if (skipLongerTests)
    			return "skipLongerTests is set, so skipping test";
    		var guidanceItems = tmXmlDatabase.loadGuidanceItems_All();
			Assert.IsNotNull(guidanceItems, "getGuidanceItems_All");
			Assert.That(guidanceItems.size()>0 , "getGuidanceItems_All was empty"); 
			//show.info(guidanceItems);
    		return "ok: Test_getGuidanceItems_All";
    	}   */ 	    	
    	
    	/*[Test]
    	public string Test_getGuidanceItem()
    	{
    		tmXmlDatabase.loadGuidanceItems_StandAlone(); 	 
    	
    		var guidanceItems_inXmlFormat = TM_Xml_Database.GuidanceItems_XmlFormat;
			var guidanceItem_inXmlFormat = guidanceItems_inXmlFormat.Values.first();
    		var guidanceItem = tmXmlDatabase.getGuidanceItem(guidanceItem_inXmlFormat.id.guid());
    		Assert.IsNotNull(guidanceItem, "guidanceItem");    		
    		Assert.AreEqual(guidanceItem.Author		,guidanceItem_inXmlFormat.Author		,"Author");
    		Assert.AreEqual(guidanceItem.Category	,guidanceItem_inXmlFormat.Category		,"Category");
    		Assert.AreEqual(guidanceItem.Priority	,guidanceItem_inXmlFormat.Priority		,"Priority");
    		Assert.AreEqual(guidanceItem.RuleType	,guidanceItem_inXmlFormat.Type1			,"RuleType");
    		Assert.AreEqual(guidanceItem.Status		,guidanceItem_inXmlFormat.Status		,"Status");
    		Assert.AreEqual(guidanceItem.Technology	,guidanceItem_inXmlFormat.Technology	,"Technology");
    		Assert.AreEqual(guidanceItem.Title		,guidanceItem_inXmlFormat.title			,"Title");
    		Assert.AreEqual(guidanceItem.Topic		,guidanceItem_inXmlFormat.Topic			,"Topic");
    		Assert.AreEqual(guidanceItem.LastUpdate,DateTime.Parse(guidanceItem_inXmlFormat.Date)			,"LastUpdate");    		
    		return "ok: Test_getGuidanceItem"; 
    	}*/
    	
    	[Test]
    	public void Test_getGuidanceHtml()
    	{
    		//tmXmlDatabase.loadGuidanceItems_StandAlone();	
    		//var guidanceItem_inXmlFormat = TM_Xml_Database.GuidanceItems_XmlFormat.Values.first();
    		var guidanceItems = tmXmlDatabase.tmGuidanceItems();
    		var firstGuidanceItem = guidanceItems.first();
    		Assert.IsNotNull(firstGuidanceItem,"firstGuidanceItem");
    		var guid = firstGuidanceItem.guidanceItemId;
    		Assert.IsNotNull(guid,"guid");
    		Assert.AreNotEqual(guid, Guid.Empty,"guid.isGuid");    		
    		var html = tmXmlDatabase.getGuidanceItemHtml(guid);
    		Assert.IsNotNull(html, "html");    		
    		Assert.That(html.valid(), "html was empty");    		
    	}
    	
    	
    } 
}    
	