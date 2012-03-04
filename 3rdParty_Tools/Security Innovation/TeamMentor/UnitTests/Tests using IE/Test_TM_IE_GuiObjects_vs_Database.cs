// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO; 
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using O2.Kernel;
using O2.XRules.Database.APIs;
using O2.XRules.Database.Utils;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Network; 
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;

//O2File:Test_TM_IE.cs
//O2File:TM_WebServices.cs

//O2File:_Extra_methods_Reflection.cs

namespace O2.SecurityInnovation.TeamMentor 
{	 
	[TestFixture] 	
    public class Test_TM_IE_GuiObjects_vs_Database : Test_TM_IE
    {    	
    	public string startPage = "Html_Pages/_UnitTest_Helpers/GuiObjects/onFolderStructureLoaded.html"; 
    	public TM_WebServices tmWebServices;
    		
    	
		public Test_TM_IE_GuiObjects_vs_Database()
		{
			Test_TM.CLOSE_BROWSER_IN_SECONDS = 4;
						
			//minimize window
			//ie.HostControl.minimized(); 
		}						

		[TestFixtureSetUp]
		public void openBrowser()
		{
			tmWebServices = new TM_WebServices();
			base.set_IE_Object("Test_TM_IE_GuiObjects_vs_Database");	
			
			if(ie.url().isNull() || ie.url().contains(startPage).isFalse())			
				base.open(startPage + "?time="+ DateTime.Now.Ticks);			
				
			var value = ie.waitForJsVariable("TM.Debug.UnitTest_Message").str();			
			Assert.AreEqual(value.str(), "Test Complete","UnitTest_Message");			
		}
		
		
    	    	
    	
    	//UNIT TESTS
    	[Test]
    	public void checkNumberOf_Libraries()
    	{    		    
    		var libraries_viaIE = ie.getJsVariable("TM.WebServices.Data.AllLibraries.length");
			var libraries_viaWSDL = tmWebServices.GetLibraries().size();
			Assert.AreEqual(libraries_viaIE, libraries_viaWSDL);    	
    	}
    	
    	[Test]
    	public void checkNumberOf_Folders()
    	{    		    
    		var folders_viaIE = ie.getJsVariable("TM.WebServices.Data.AllFolders.length");
			var folders_viaWSDL = tmWebServices.GetAllFolders().size();
			Assert.AreEqual(folders_viaIE, folders_viaWSDL);    	
    	}
    	
    	[Test]
    	public void checkNumberOf_Views()
    	{    		    
    		var views_viaIE = ie.getJsVariable("TM.WebServices.Data.AllViews.length");
			var views_viaWSDL = tmWebServices.GetAllViews().size();
			Assert.AreEqual(views_viaIE, views_viaWSDL);    	
    	}    	    	
    	
    	[Test]
    	public void checkNumberOf_GuidanceItemsInViews() 
    	{    		    
			var viewsCount = ie.getJsObject<int>("TM.WebServices.Data.AllViews.length"); 
			for(int i=0 ; i < viewsCount ; i++) 
			{	 
				var viewId					= ie.getJsObject<string>("TM.WebServices.Data.AllViews[{0}].viewId"				 .format(i));					
				var viewCaption				= ie.getJsObject<string>("TM.WebServices.Data.AllViews[{0}].caption"			 .format(i));					
				var guidanceItemsRaw_viaIE 	= ie.getJsObject		("TM.WebServices.Data.AllViews[{0}].guidanceItems"		 .format(i)); 
				var guidanceItems_via_IE 	= guidanceItemsRaw_viaIE.extractList<string>(false).removeEmpty();;								
				
				var guidanceItems_via_WSDL 	= tmWebServices.GetGuidanceItemsInView(viewId.guid());
				
				"Checking view  '{0} - {1}' : # of guidanceItems via IE: {2} , via WSDL {3}".info(viewCaption, viewId, guidanceItems_via_IE.size(), guidanceItems_via_WSDL.size());
				Assert.AreEqual(guidanceItems_via_IE.size() , guidanceItems_via_WSDL.size() ,"GuidanceItems size didn't match for viewId: {0}".format(viewId));					
				foreach(var guidanceItem in guidanceItems_via_WSDL)
					Assert.IsTrue(guidanceItems_via_IE.contains(guidanceItem.Id.str()));  // note that the guidanceItem is complete, so we could do more checks on content here					
			}
		}
		
		[Test]
    	public void checkNumberOf_GuidanceItemsInFolders() 
    	{    		    
			var foldersCount = ie.getJsObject<int>("TM.WebServices.Data.AllFolders.length"); 
			for(int i=0 ; i < foldersCount ; i++) 
			{	 
				var folderId					= ie.getJsObject<string>("TM.WebServices.Data.AllFolders[{0}].folderId"			 .format(i));					
				var folderCaption				= ie.getJsObject<string>("TM.WebServices.Data.AllFolders[{0}].name"			 	 .format(i));					
				var guidanceItemsRaw_viaIE 		= ie.getJsObject		("TM.WebServices.Data.AllFolders[{0}].guidanceItems"	 .format(i)); 
				var guidanceItems_via_IE 		= guidanceItemsRaw_viaIE.extractList<string>(false).removeEmpty();;								
				
				var guidanceItems_via_WSDL 	= tmWebServices.GetGuidanceItemsInFolder(folderId.guid());
				
				"Checking view  '{0} - {1}' : # of guidanceItems via IE: {2} , via WSDL {3}".info(folderCaption, folderId, guidanceItems_via_IE.size(), guidanceItems_via_WSDL.size());
				Assert.AreEqual(guidanceItems_via_IE.size() , guidanceItems_via_WSDL.size() ,"GuidanceItems size didn't match for folderId: {0}".format(folderId));					
				foreach(var guidanceItem in guidanceItems_via_WSDL)
					Assert.IsTrue(guidanceItems_via_IE.contains(guidanceItem.Id.str()));  // note that the guidanceItem is complete, so we could do more checks on content here					
			}
		}

		[Test]
    	public void checkNumberOf_GuidanceItemsInLibraries() 
    	{
    		var libraries = ie.getJsObject<int>("TM.WebServices.Data.AllLibraries.length"); 
			for(int i=0 ; i < libraries ; i++) 
			{	 
				var libraryId					= ie.getJsObject<string>("TM.WebServices.Data.AllLibraries[{0}].libraryId"			 .format(i));					
				var libraryCaption				= ie.getJsObject<string>("TM.WebServices.Data.AllLibraries[{0}].name"			 	 .format(i));					
				var guidanceItemsRaw_viaIE 		= ie.getJsObject		("TM.WebServices.Data.AllLibraries[{0}].guidanceItems"	 .format(i)); 
				var guidanceItems_via_IE 		= guidanceItemsRaw_viaIE.extractList<string>(false).removeEmpty();;								
				
				var guidanceItems_via_WSDL 	= tmWebServices.GetGuidanceItemsInLibrary(libraryId.guid());
				
				"Checking view  '{0} - {1}' : # of guidanceItems via IE: {2} , via WSDL {3}".info(libraryCaption, libraryId, guidanceItems_via_IE.size(), guidanceItems_via_WSDL.size());
				//if (guidanceItems_via_IE.size()  != guidanceItems_via_WSDL.size())
				//	return "ERROR: " + libraryId;
				Assert.AreEqual(guidanceItems_via_IE.size() , guidanceItems_via_WSDL.size() ,"GuidanceItems size didn't match for folderId: {0}".format(libraryId));					
				foreach(var guidanceItem in guidanceItems_via_WSDL)
					Assert.IsTrue(guidanceItems_via_IE.contains(guidanceItem.Id.str()));  // note that the guidanceItem is complete, so we could do more checks on content here					
			}
    	}
    	
    	[TestFixtureTearDown]
    	public void closeIE()
    	{
    		"ok [Test_TM_IE_Javascript_GuiObjects]: close_IE (in {0} seconds)".format(Test_TM.CLOSE_BROWSER_IN_SECONDS);
    		base.close_IE_Object();    		
    	}
    	
    	
    }
}