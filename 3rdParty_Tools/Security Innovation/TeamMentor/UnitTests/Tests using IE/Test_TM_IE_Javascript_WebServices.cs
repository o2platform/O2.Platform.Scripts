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
//O2File:_Extra_methods_Web.cs  
  
namespace O2.SecurityInnovation.TeamMentor 
{	 
	[TestFixture] 
    public class Test_TM_IE_Javascript_WebServices : Test_TM_IE
    {    	
    	public string startPage = "Html_Pages/_UnitTest_Helpers/_Javascript_Loaders/jQuery_tmWebServices.html"; 
    	
		public Test_TM_IE_Javascript_WebServices()
		{
			Test_TM.CLOSE_BROWSER_IN_SECONDS = 4;
			
			base.set_IE_Object("Test_TM_IE_Javascript_WebServices");	
			
			if(ie.url().isNull() || ie.url().contains(startPage).isFalse())			
				base.open(startPage + "?time="+ DateTime.Now.Ticks);			
		}		
		
		//UNIT TESTS
		
    	//open_IE_and_get_EmptyPage
    	[Test]  
    	public void open_IE_and_get_EmptyPage()
    	{			 
    		lock(ie)
    		{
				Assert.That(ie.notNull(), "ie object was null");			
				Assert.That(ie.url().contains(startPage) , "startPage");	
				Assert.That(ie.html().valid(), "invalid html");
	    		"ok: open_IE_and_get_EmptyPage".jQuery_Append_Body(ie);
	    	}
    	}  
    	    	
    	//inject_Javascript_jQuery_and_TM_WebServices    	
    	[Test] 
    	public void inject_Javascript_jQuery_and_TM_WebServices()
    	{    		 
    		lock(ie)
    		{
				var jQuery = ie.setJsObject(null).getJsObject("jQuery");
				var tm = ie.setJsObject(null).getJsObject("TM");
				var tmWebServices = ie.setJsObject(null).getJsObject("TM.WebServices");
				var tmWebServicesVersion = ie.setJsObject(null).getJsObject("TM.WebServices.Config.Version");
				Assert.IsNotNull(jQuery);
				Assert.IsNotNull(tm,"TM");
				Assert.IsNotNull(tmWebServices, "TM.WebServices");
				Assert.IsNotNull(tmWebServicesVersion, "TM.WebServices.Config.Version");
				Assert.AreEqual(tmWebServicesVersion , "v0.3", "not expected TM.WebServices.Config.Version");
				
				"ok: inject_Javascript_jQuery_and_TM_WebServices".jQuery_Append_Body(ie);
			}
    	}
    	
    	//WS_via_IE_getTime
    	[Test]
    	public void WS_via_IE_getTime()
    	{    		    		
    		lock(ie)
    		{
	    		ie.eval("TM.WebServices.WS_Utils.getTime(function(data) { TM.getTimeValue = data });");
	    		var getTimeValue = ie.waitForJsVariable("TM.getTimeValue");
	    		Assert.That(getTimeValue.notNull(), "getTimeValue was null");
	    		var nowValue = DateTime.Now.ToShortDateString();
	    		Assert.That(getTimeValue.str().contains(nowValue), "getTimeValue didn't contain now time: {0} : {1}".format(getTimeValue, nowValue));
	    		    		
	    		"ok: WS_via_IE_getTime".jQuery_Append_Body(ie);
	    	}
    	}
    	  
    	//WS_via_IE_getTime 
    	[Test]  
    	public void get_GuiObjects_uniqueStrings_and_guidanceItemsMappings()
    	{			
    		lock(ie)
    		{
				"fetching GuiObjects".jQuery_Append_Body(ie);
				
				Action getGuiObjects = 
					()=>{			
							var o2Timer = new O2Timer("Got GuiObjects").start();
							ie.eval("delete TM.WebServices.Data.GuiObjects"); 
							ie.eval("TM.WebServices.WS_Data.getGUIObjects()");			
							ie.waitForJsVariable("TM.WebServices.Data.GuiObjects"); 
							o2Timer.stop().str().jQuery_Append_Body(ie);
						};
				
				//5.loop(getGuiObjects);
				getGuiObjects();
				
				var uniqueStrings = ie.getJsVariable("TM.WebServices.Data.GuiObjects.UniqueStrings");
				var guidanceItemsMappings =  ie.getJsVariable("TM.WebServices.Data.GuiObjects.GuidanceItemsMappings");
				Assert.That(uniqueStrings.notNull(), "uniqueStrings was null");
				Assert.That(guidanceItemsMappings.notNull(), "guidanceItemsMappings was null");			
				
				var uniqueStrings_StringList = uniqueStrings.extractList_String();
				var guidanceItemsMappings_StringList = guidanceItemsMappings.extractList_String();
				Assert.That(uniqueStrings_StringList.size() > 0 , "no items in uniqueStrings_StringList");
				Assert.That(guidanceItemsMappings_StringList.size() > 0 , "no items in guidanceItemsMappings_StringList");
				
				"Got GuiObjects via Javascriot, with {0} uniqueStrings and {1}".info(uniqueStrings_StringList.size(), 
																					 guidanceItemsMappings_StringList.size());
				"ok: get_GuiObjects_uniqueStrings_and_guidanceItemsMappings".jQuery_Append_Body(ie);
			}
    	}  
    	
    	//from_GuiObjects_Create_Raw_Table
		[Test]  
    	public void from_GuiObjects_Create_Raw_Table()
    	{		
    		lock(ie)
    		{
				"fetching GuiObjects".jQuery_Append_Body(ie);
				ie.eval("TM.WebServices.WS_Data.getGUIObjects()");
				
				ie.waitForJsVariable("TM.WebServices.Data.GuiObjects"); 
				"got GuiObjects".jQuery_Append_Body(ie);
				
				Action createTable = 
					()=>{
							var o2Timer = new O2Timer("created table").start();
							ie. eval(@"$('body').append('<div id=""_test"">....</div>');
									  	uniqueStrings = TM.WebServices.Data.GuiObjects.UniqueStrings;
										guidanceItemsMappings = TM.WebServices.Data.GuiObjects.GuidanceItemsMappings;
										var getMapping = function(rawMapping) { 
							                                        var mapping = '';  
							                                        $.each(rawMapping.split(','), function() { mapping += '<td>' + uniqueStrings[this] + '</td>' }); 
							                                        return mapping; 
							                                      };
										var data = '<table border=1>'; 
										$.each(guidanceItemsMappings,function()
													{ 	
														data += '<tr>' + getMapping(this) + '</tr>'; return;
													});
							
										data+='</table>'; 
										$('#_test').html(data);	
								    ");
							ie.eval("$('#_test').html('....')");
							"Created table in: {0}".format(o2Timer.stop()).jQuery_Append_Body(ie);
					};
				
				
				//test multiple table creations:
				//for(int i = 0 ; i < 3 ; i++)  
					createTable();
					
				"ok from_GuiObjects_Create_Raw_Table".jQuery_Append_Body(ie);;
			}
    	} 
    	
    	//from_GuiObjects_ExtractMappings_And_Create_Raw_Table
    	[Test]  
    	public void from_GuiObjects_ExtractMappings_And_Create_Raw_Table()
    	{			 
    		lock(ie)
    		{
				"fetching GuiObjects and extracting all mappings".jQuery_Append_Body(ie);
				ie.eval("TM.WebServices.Data.extractGuiObjects()");			
				ie.waitForJsVariable("TM.WebServices.Data.ExtractedAllMappings"); 
				"got GuiObjects and mappings".jQuery_Append_Body(ie);
				
				Action createTable = 
					()=>{
							var o2Timer = new O2Timer("created table").start();
													
							ie. eval(@"$('body').append('<div id=""_test"">....</div>');
							
										var giTable = '<table border=1>' ; 
				
										$.each(TM.WebServices.Data.GuidanceItemsIDs, function()
										          {
										              var giData = $.data[this];
										              giTable += '<tr>';
										              giTable += '<td>' + giData.guidanceItemId + '</td>';
										              giTable += '<td>' + giData.libraryId+ '</td>';
										              giTable += '<td>' + giData.title+ '</td>';
										              giTable += '<td>' + giData.technology+ '</td>';
										              giTable += '<td>' + giData.phase+ '</td>';
										              giTable += '<td>' + giData.type+ '</td>';
										              giTable += '<td>' + giData.category+ '</td>';
										              giTable += '</td>';
										              giTable += '</tr>';
										          });
										
										giTable += '</table>'; 
										$('#_test').html(giTable);
								    ");							    
		
							ie.eval("$('#_test').html('....')");
							"Created table in: {0}".format(o2Timer.stop()).jQuery_Append_Body(ie);
					};			
				
				//test multiple table creations:
				//for(int i = 0 ; i < 3 ; i++)  
					createTable();
							
				"ok from_GuiObjects_ExtractMappings_And_Create_Raw_Table".jQuery_Append_Body(ie);;
			}
    	}
    	
    	//get_FolderStructure_Libraries
    	[Test]
    	public void get_FolderStructure_Libraries()
    	{			 
    		lock(ie)
    		{
				"fetching Libraries FolderStrucure".jQuery_Append_Body(ie);
				ie.eval("TM.WebServices.WS_Data.getFolderStructure();");			
				var folderStructure = ie.waitForJsVariable("TM.WebServices.Data.folderStructure"); 
				Assert.That(folderStructure.notNull(), "folderStructure was null");
				"got FolderStrucure".jQuery_Append_Body(ie);
				
				ie.eval("var firstLibrary = TM.WebServices.Data.folderStructure[0]");
				Assert.That(ie.getJsVariable("firstLibrary").notNull(), "firstLibrary was null") ;
				
				var libraryId = ie.getJsVariable("firstLibrary.libraryId");
				var libraryName = ie.getJsVariable("firstLibrary.name");
				Assert.That(libraryId.notNull(), "libraryId was null");
				Assert.That(libraryId.str().isGuid(), "libraryId was not a guid");
				Assert.That(libraryName.notNull(), "libraryName was null");
				
				ie.eval("var firstFolder = firstLibrary.subFolders[0]");
				Assert.That(ie.getJsVariable("firstFolder").notNull(), "firstFolder was null") ;
				Assert.That(ie.getJsVariable("firstFolder.folderId").notNull(), "firstFolder.name was null") ;
				Assert.That(ie.getJsVariable("firstFolder.name").notNull(), "firstFolder.name was null") ;
				
				ie.eval("var firstViewInFirstFolder = firstFolder.views[0]");
				Assert.That(ie.getJsVariable("firstViewInFirstFolder").notNull(), "firstViewInFirstFolder was null") ;
				Assert.That(ie.getJsVariable("firstViewInFirstFolder.viewId").notNull(), "firstViewInFirstFolder.viewId was null") ;
				Assert.That(ie.getJsVariable("firstViewInFirstFolder.caption").notNull(), "firstViewInFirstFolder.caption was null") ;
				Assert.That(ie.getJsVariable("firstViewInFirstFolder.guidanceItems").notNull(), "firstViewInFirstFolder.guidanceItems was null") ;
				
				"ok get_FolderStructure_Libraries".jQuery_Append_Body(ie);
			}
		}
		
		// show_FolderStructure_Libraries
		[Test]  
    	public void show_FolderStructure_Libraries()
    	{			
    		lock(ie)
    		{
				"fetching Libraries FolderStrucure".jQuery_Append_Body(ie);
				ie.eval("TM.WebServices.WS_Data.getFolderStructure();");			
				var folderStructure = ie.waitForJsVariable("TM.WebServices.Data.folderStructure"); 
				Assert.That(folderStructure.notNull(), "folderStructure was null");
				"got FolderStrucure".jQuery_Append_Body(ie);
										
							
				Action<bool> showLibraryTreeStructure =  
					(showGuidanceItemsCount)=>{																					
							ie. eval(@"	var addSubItems = function(target, items, type, property, color)
										{
										    if (typeof(items) != 'undefined')
										    {
										        var subItem = $('<ul>');
										        target.append(subItem);
										        addItems(subItem,items, type,property,color);
										    }
										}
										var addItems = function(target, items, type, property, color)
										  {
										
										       $.each(items, function()
										               {									               
										                    var text = this[property] ?  this[property] : this;  
										                    if(" + showGuidanceItemsCount.str().lower() + @")
										                       if (typeof(this.guidanceItems) != 'undefined')
										                    	   text += '\t\t\t\t: with ' + this.guidanceItems.length + ' guidance items';
										                    	
										                    var itemNode = target.append($('<li>').css('color',color).addClass(type).append(type + ': ' + text));									                    
										
										                    addSubItems(itemNode , this.subFolders, 'folder', 'name', 'darkGreen')
										                    addSubItems(itemNode , this.views, 'folder', 'caption', 'blue')
										                    //addSubItems(itemNode , this.guidanceItems, 'guidanceItem', 'caption', 'black')
										                    									                    
										               });
										  }
										
										var treeStructure  = $('<div id=treeStructure />');
										var libraries = TM.WebServices.Data.folderStructure;
										
										addSubItems(treeStructure , libraries, 'library', 'name', 'orange');
										
										$('body').append(treeStructure);									
									");
							ie.eval("$('#treeStructure').remove();");
	/*						this.sleep(1000);		
							ie.eval(@"$('#treeStructure').fadeOut(2000,
															function () {
																			$('#treeStructure').remove() 
																		})");						
							this.sleep(2000); */
					};						
				showLibraryTreeStructure(false);			
				showLibraryTreeStructure(true);		
				"ok show_FolderStructure_Libraries".jQuery_Append_Body(ie);
			}
    	}  
    	    	
    	/*[Test]
    	public string close_IE()
    	{    		    		
    		base.close_IE_Object();			 		    		
    		return "Test_TM_IE_Javascript_WebServices: ok : close_IE (in {0} seconds)".format(Test_TM.CLOSE_BROWSER_IN_SECONDS)
    			.jQuery_Append_Body(ie);
    	}*/
    	
    	[TestFixtureTearDown]
    	public void close_IE()
    	{    		
    		"IN TEAR DOWN".error();
    		lock(ie)
    		{
    			"Test_TM_IE_Javascript_WebServices: ok : close_IE (in {0} seconds)".format(Test_TM.CLOSE_BROWSER_IN_SECONDS)
    				.jQuery_Append_Body(ie);
    			base.close_IE_Object();
    			"IN TEAR DOWN 2".error();
    		}
    		"IN TEAR DOWN 3".error();
    	}
    	
    	
    }
}