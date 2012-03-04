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

namespace O2.SecurityInnovation.TeamMentor 
{	 
	[TestFixture] 
    public class Develop_for_Test_TM_IE_Javascript_WebServices : Test_TM_IE
    {    			 
		public Develop_for_Test_TM_IE_Javascript_WebServices()
		{			
			base.set_IE_Object("Develop_for_Test_TM_IE_Javascript_WebServices");	
			if (ie.url().isNull())
				base.open("Html_Pages/_UnitTest_Helpers/_Javascript_Loaders/jQuery_tmWebServices.html?time="+ DateTime.Now.Ticks);
			//ie.open(Test_TM.tmEmptyPage);
			//this.load_Javascript_jQuery()
			//	.load_Javascript_TM_WebServices();		 						
		}		
		
    	//This file should only have one UnitTest here (so that it makes it development faster
    	//after the test is completed put the created UnitTest in the file: Test_TM_IE_Javascript_WebServices.cs
    	
    	[Test]  
    	public string show_FolderStructure_Libraries()
    	{			 									
			"fetching Libraries FolderStrucure".jQuery_Append_Body(ie);
			ie.eval("TM.WebServices.WS_Data.getFolderStructure();");			
			var folderStructure = ie.waitForJsVariable("TM.WebServices.Data.folderStructure"); 
			Assert.That(folderStructure.notNull(), "folderStructure was null");
			"got FolderStrucure".jQuery_Append_Body(ie);
									
						
			Action showLibraryTreeStructure =  
				()=>{						
									
						//var o2Timer = new O2Timer("Library tree").start();
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
						//"showed showLibraryTreeStructure in: {0}".format(o2Timer.stop()).jQuery_Append_Body(ie);
						this.sleep(1000);
						ie.eval("$('#treeStructure').fadeOut(1000)");
				};			
			
			//test multiple table creations:
			showLibraryTreeStructure();
			
			return "ok show_FolderStructure_Libraries".jQuery_Append_Body(ie);
    	}  
    	    	
    	[Test]
    	public string close_IE()
    	{    		
    		Test_TM.CLOSE_BROWSER_IN_SECONDS = 4;
    		
    		base.close_IE_Object();
			    		    		
    		return "ok: close_IE (in {0} seconds)".format(Test_TM.CLOSE_BROWSER_IN_SECONDS)
    					.jQuery_Append_Body(ie);
    	}
    	
    	
    }
}