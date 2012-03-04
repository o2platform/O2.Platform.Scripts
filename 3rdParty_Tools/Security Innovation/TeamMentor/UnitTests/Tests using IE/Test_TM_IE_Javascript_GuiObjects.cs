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

//O2File:_Extra_methods_Reflection.cs

namespace O2.SecurityInnovation.TeamMentor 
{	 
	[TestFixture] 	
    public class Test_TM_IE_Javascript_GuiObjects : Test_TM_IE
    {    	
    	public string startPage = "Html_Pages/_UnitTest_Helpers/_Javascript_Loaders/jQuery_tmWebServices.html"; 
    	
    	
		public Test_TM_IE_Javascript_GuiObjects()
		{
			Test_TM.CLOSE_BROWSER_IN_SECONDS = 4;
			
			base.set_IE_Object("Test_TM_IE_Javascript_GuiObjects");	
			
			if(ie.url().isNull() || ie.url().contains(startPage).isFalse())			
				base.open(startPage + "?time="+ DateTime.Now.Ticks);			
			
			//minimize window
			//ie.HostControl.minimized(); 
		}						


/*		[Test]
		public void ieTest()
		{
			O2Thread.mtaThread(
				()=>
				{
					var popup ="popup".popupWindow();
					popup.insert_LogViewer();
					var ie = popup.add_IE();
					ie.open("http://www.google.com");
					
				});
			this.sleep(10000);	
		}
//		[SetUp]				
	*/	
    	//Helper methods
    	public void call_onGuiObjectsLoaded()
    	{    		
	    	base.set_IE_Object(ie); 
    		ie.deleteJsVariable("TM.WebServices.Data.ExtractedAllMappings"); // in case we are invoking more that once the unit test
			ie.eval("TM.Events.onGuiObjectsLoaded()");			
			var value = ie.waitForJsVariable("TM.WebServices.Data.ExtractedAllMappings"); 
			Assert.AreEqual (true, value, "TM.WebServices.Data.ExtractedAllMappings");									
    	}
    	    	
    	
    	//UNIT TESTS
    	[Test]
    	public void onGuiObjectsLoaded()
    	{    		    	
    		lock(ie)
    		{
	    		call_onGuiObjectsLoaded();
				
				Action<string[]> check_If_Variable_Doesnt_Exists =
					(variables) => {	foreach(var variable in variables)
											Assert.IsNull(ie.getJsVariable(variable), variable); };
											
				Action<string[]> check_If_Variable_Exists =
					(variables) => {	foreach(var variable in variables)
											Assert.IsNotNull(ie.getJsVariable(variable), variable); };						
					
				check_If_Variable_Exists(new [] 
					{
						"TM.WebServices.Data.GuiObjects",
						"TM.WebServices.Data.GuiObjects.UniqueStrings",
						"TM.WebServices.Data.GuiObjects.GuidanceItemsMappings",
						"TM.WebServices.Data.GuidanceItemsIDs", 					
						"$.data[TM.WebServices.Data.GuidanceItemsIDs[0]]",
						"TM.WebServices.Data.ExtractedAllMappings"
					});
					
				check_If_Variable_Doesnt_Exists(new [] 
					{
						"TM.WebServices.Data.GuiObjects.AAAAAAStrings",
						"$.data[TM.WebServices.Data.GuidanceItemsIDs[-1]]",
						"$.data[TM.WebServices.Data.GuidanceItemsIDs[100000]]"
					});
				
	    		"ok: onGuiObjectsLoaded".jQuery_Append_Body(ie);    		
    		
    		}
			
    	}
    	
    	[Test]
    	public void onFolderMappingsLoaded()
    	{
    		lock(ie)
    		{
	    		call_onGuiObjectsLoaded();  
	    		
	    		ie.eval("var dataTable = TM.WebServices.Data.getGuidanceItemsInGuid_For_DataTable()");
				var dataTable = ie.getJsVariable("dataTable");
	    		var libraryId = ie.getJsVariable("dataTable.aaData[0][7]");
	    		
	    		ie.eval("$.data['{0}']= ['{1}','{2}']".format(libraryId, "".newGuid(),"".newGuid()));
	    		
	    		ie.eval("var guidsInLibrary = $.data['{0}']".format(libraryId));
	    		var guidsInLibrary = ie.getJsVariable("guidsInLibrary").extractList<string>();	    		
	    		Assert.IsNotNull(guidsInLibrary				, "guidsInLibrary was null");    		
	    		Assert.That		(guidsInLibrary.size() > 0 	, "guidsInLibrary had not items");
	    		Assert.That		(guidsInLibrary[0].isGuid()	, "first item from guidsInLibrary was not a guid");
	    		
	    		"ok: onFoldeMappingsLoaded".jQuery_Append_Body(ie);    		    		    			
			}
		}    	    	
    	
    	[Test]
    	public void getGuidanceItemsInGuid_For_DataTable_NoParams()
    	{    
    		lock(ie)
    		{
	    		call_onGuiObjectsLoaded();  
	    		
	    		//ie.eval("var dataTable = TM.WebServices.Data.getGuidanceItemsInGuid_For_DataTable()");
	    		ie.eval("var dataTable = TM.WebServices.Data.getGuidanceItemsInGuid_For_DataTable(TM.WebServices.Data.library('owasp').id)");
				var dataTable = ie.getJsVariable("dataTable");
				
				var bDeferRender = dataTable.get_Value<bool>("bDeferRender");
				var bProcessing  = dataTable.get_Value<bool>("bProcessing"); 
				var bPaginate 	 = dataTable.get_Value<bool>("bPaginate");
				var bInfo 		 = dataTable.get_Value<bool>("bInfo");	 						
				var bSort  	 	 = dataTable.get_Value<bool>("bSort");
					
				Assert.IsTrue (bDeferRender , "bDeferRender");
				Assert.IsTrue (bProcessing	,  "bProcessing");
				Assert.IsFalse(bPaginate	, 	 "bPaginate");
				Assert.IsFalse(bInfo		, 		 "bInfo");						
				Assert.IsTrue (bSort		, 		 "bSort");
				
				Assert.AreEqual("Index"		, ie.getJsVariable("dataTable.aoColumns[0].sTitle.toString()"), "1st Column");
				Assert.AreEqual("Title"		, ie.getJsVariable("dataTable.aoColumns[1].sTitle.toString()"), "2nd Column");
				Assert.AreEqual("Technology", ie.getJsVariable("dataTable.aoColumns[2].sTitle.toString()"), "3rd Column");
				Assert.AreEqual("Phase"		, ie.getJsVariable("dataTable.aoColumns[3].sTitle.toString()"), "4th Column");
				Assert.AreEqual("Type"		, ie.getJsVariable("dataTable.aoColumns[4].sTitle.toString()"), "5th Column");
				Assert.AreEqual("Category"	, ie.getJsVariable("dataTable.aoColumns[5].sTitle.toString()"), "6th Column");
				Assert.AreEqual("Guid"		, ie.getJsVariable("dataTable.aoColumns[6].sTitle.toString()"), "7th Column");				
				
				Assert.That(ie.getJsVariable("dataTable.aaData").extractList().size() > 0 , "no items in aaData");
				
				var firstGuidanceItem =  ie.getJsVariable("dataTable.aaData[0]").extractList();
				
				var index 		= firstGuidanceItem[0];
				var title 		= firstGuidanceItem[1];
				var technology 	= firstGuidanceItem[2];
				var phase 		= firstGuidanceItem[3];
				var type 		= firstGuidanceItem[4];
				var category 	= firstGuidanceItem[5];
				var guid	 	= firstGuidanceItem[6].str();				
				
				Assert.That(index 		is int								, "index");
				Assert.That(title 		is string && title.str().valid()	, "title");
				Assert.That(technology 	is string && title.str().valid()	, "technology");
				Assert.That(phase 		is string && title.str().valid()	, "phase");
				Assert.That(type 		is string && title.str().valid()	, "type");
				Assert.That(category 	is string && title.str().valid()	, "category");
				Assert.That(guid 	   .isGuid()							, "guid");								
				
				"ok: getGuidanceItemsInGuid_For_DataTable".jQuery_Append_Body(ie);	
			}
		}
		
    	/*[Test]
    	public string close_IE()
    	{    		    		
    		base.close_IE_Object();			 		    		
    		return "ok [Test_TM_IE_Javascript_GuiObjects]: close_IE (in {0} seconds)".format(Test_TM.CLOSE_BROWSER_IN_SECONDS)
    			;
    			//.jQuery_Append_Body(ie);
    	}*/
    	
    	[TestFixtureTearDown]
    	public void closeIE()
    	{
    		"ok [Test_TM_IE_Javascript_GuiObjects]: close_IE (in {0} seconds)".format(Test_TM.CLOSE_BROWSER_IN_SECONDS);
    		base.close_IE_Object();    		
    	}
    	
    	
    }
}