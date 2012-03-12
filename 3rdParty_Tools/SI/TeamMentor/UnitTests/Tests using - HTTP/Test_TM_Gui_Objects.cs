// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Linq; 
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using O2.Kernel;
using O2.XRules.Database.APIs;
using O2.XRules.Database.Utils;
using O2.DotNetWrappers.Network;
using O2.Kernel.ExtensionMethods;
using O2.Views.ASCX.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using SecurityInnovation.TeamMentor.WebClient; 
using NUnit.Framework;
 
//O2File:Test_TM_Config.cs 
//O2File:TM_GUI_Objects.cs
//O2File:_Extra_methods_Web.cs
//O2File:_Extra_methods_Winforms_Controls.cs
//O2Ref:nunit.framework.dll

namespace O2.SecurityInnovation.TeamMentor
{		
	[TestFixture]
    public class Test_TM_Gui_Objects
    {        	        	
    	    	
    	[Test] 
    	public void Connect_To_Server()
    	{
    		var webServicesHtml = Test_TM.tmWebServices.html();
    		Assert.That(webServicesHtml.contains("GetGUIObjects"), "Could not find GetGUIObjects");    		
    	}    	    	
    	
    	[Test]
    	public void Get_TMGuiObjects_Raw()
    	{
    		var rawGuiObjects = Test_TM.tmWebServices.invokeWebServiceAndGetJSON("GetGUIObjects","{}");
    		Assert.That(rawGuiObjects.valid(), "rawGuiObjects was empty");
    		Assert.That(rawGuiObjects.contains(TM_Gui_Objects_ExtensionMethods.JsonFix), "couldn't find JsonFix string");
    		Assert.That(rawGuiObjects.size() > 10000, "rawGuiObjects.size() < 10000");     		
    	}
    	
    	[Test]
    	public void Create_TMGuiObjects()
    	{
    		var rawGuiObjects = Test_TM.tmWebServices.invokeWebServiceAndGetJSON("GetGUIObjects","{}");
    		var fixedJson = rawGuiObjects.fixJsonSerialization();
    		Assert.That(fixedJson.notNull(), "fixedJson was null");
    		var tmGuiObjects = fixedJson.javascript_Deserialize<TM_GUI_Objects>();    
    		Assert.That(tmGuiObjects.notNull(), "tmGuiObjects was null");
    		Assert.That(tmGuiObjects.UniqueStrings.size() > 10, "UniqueStrings < 10");
    		Assert.That(tmGuiObjects.GuidanceItemsMappings.size() > 10, "GuidanceItemsMappings < 10");    		
    	}
    	
    	[Test]
    	public void Map_TMGuiObjects_To_Table()
    	{
    		var topPanel = "view TM Gui Objects in Table".popupWindow();    		
    		var tmGuiObjects = Test_TM.tmWebServices.tmGuiObjects();
    		Assert.That(tmGuiObjects.notNull(), "o2GuiObjects was null");    		     		    		
    		topPanel.insert_Right("Unique Strings")
	    				.add_TreeView().visible(false)
	    				.add_Nodes(tmGuiObjects.UniqueStrings).visible(true)	    				
    				.insert_Below("GuidanceItems Mappings")
	    				.add_TreeView().visible(false)
						.add_Nodes(tmGuiObjects.GuidanceItemsMappings).visible(true);
						
    		var tableList = topPanel.add_TableList()
    								.add_Columns("GuidanceItem Id","Library Id","Title","Category","Phase","Type")
    								.visible(false);

    		Func<string,string[]> resolveItem = 
				(indexesString)=>{
								 	return (from index in indexesString.split(",")
					 		    			select tmGuiObjects.UniqueStrings[index.toInt()]).ToArray();					 	
								 };
			foreach(var mapping in tmGuiObjects.GuidanceItemsMappings)
				tableList.add_Row(resolveItem(mapping));	
			tableList.visible(true);
    		    		
    		topPanel.closeForm_InNSeconds(Test_TM.CLOSE_WINDOW_IN_SECONDS);    		
    	}
    	    	
    }
    
    public static class TM_Gui_Objects_ExtensionMethods
    {
    	public static string JsonFix = "{\"d\":{\"__type\":\"SecurityInnovation.TeamMentor.WebClient.TM_GUI_Objects\",";
    	
    	public static string invokeWebServiceAndGetJSON(this string webServicesUrl, string method, string postData)
    	{
			var requestUrl  = "{0}/{1}".format(webServicesUrl, method);
			"[invokeWebServiceAndGetJSON] requestUrl: {0}".info(requestUrl);
			var web = new Web();
			var response = web.getUrlContents_POST(requestUrl,"application/json; charset=UTF-8","",postData);
			"[invokeWebServiceAndGetJSON] response size: {0}".info(response.size());
			return response;
		}
		
		public static string fixJsonSerialization(this string _rawJsonString)
		{
			if (_rawJsonString.starts(JsonFix))				
				return _rawJsonString.subString(JsonFix.size())
									 .removeLastChar()
									 .insertAt(0,"{");
			return null;
		}
		
		public static TM_GUI_Objects tmGuiObjects(this string tmWebServiceUrl)
		{
			var rawGuiObjects = tmWebServiceUrl.invokeWebServiceAndGetJSON("GetGUIObjects","{}");
    		var fixedJson = rawGuiObjects.fixJsonSerialization();    	
    		var tmGuiObjects = fixedJson.javascript_Deserialize<TM_GUI_Objects>();    
    		return tmGuiObjects;
		}
    }
}
