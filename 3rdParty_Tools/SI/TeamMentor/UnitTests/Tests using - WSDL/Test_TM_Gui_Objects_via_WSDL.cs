// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO; 
using System.Linq;
using System.Collections.Generic; 
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using O2.Kernel; 
using O2.XRules.Database.APIs;
using O2.XRules.Database.Utils;
using O2.DotNetWrappers.Network;
using O2.Kernel.ExtensionMethods;
 
//O2File:Test_TM_Config.cs
//O2File:Test_TM_Setup.cs
//O2File:TM_WebServices.cs
//O2File:_Extra_methods_Web.cs
//O2File:_Extra_methods_Collections.cs

//O2Ref:nunit.framework.dll

namespace O2.SecurityInnovation.TeamMentor
{	
	[TestFixture]
    public class Test_TM_Gui_Objects_via_WSDL
    {
    	public static TM_WebServices tmWebServices;
    	
    	public Test_TM_Gui_Objects_via_WSDL()
    	{
			tmWebServices = new TM_WebServices();    	
			tmWebServices.Url = Test_TM.tmWebServices;
    	}
    	    	
    	[Test]
    	public void check_If_Site_Is_Working()
    	{
    		new Test_TM_Setup().check_if_TM_WebServer_is_Running();
    	}
    	
    	[Test]
    	public void create_TM_WebServices()
    	{			
			Assert.That(tmWebServices.notNull(), "tmWebServices was null");
			var webServicesHtml = tmWebServices.Url.html();			
			Assert.That(webServicesHtml.valid(), "webServicesHtml was empty");			    		
    	}
    	
    	[Test]
    	public void WS_GetGUIObjects()
    	{
			var tmGuiObjects = tmWebServices.GetGUIObjects();
			Assert.That(tmGuiObjects.notNull(), "tmGuiObjects was null");
			Assert.That(tmGuiObjects.UniqueStrings.Length >0 ,"there were no UniqueStrings");
			Assert.That(tmGuiObjects.GuidanceItemsMappings.Length >0 ,"there were no GuidanceItemsMappings");    		
    	}
    	
    	[Test]  
    	public void GetGuiObjects()
    	{   
    		var guiObjects = tmWebServices.GetGUIObjects();
    		Assert.That(guiObjects.notNull(), "guiObjects was null"); 		
    		Assert.That(guiObjects.UniqueStrings.size() > 0 , "empty UniqueStrings");
    		Assert.That(guiObjects.GuidanceItemsMappings.size() > 0 , "empty GuidanceItemsMappings");    		
    	}
    	
		[Test]  
    	public void GetFolderStructure_Libraries()
    	{   
    		var librariesStructure = tmWebServices.GetFolderStructure_Libraries();
    		Assert.That(librariesStructure.notNull(), "librariesStructure was null"); 		
    		Assert.That(librariesStructure.size() > 0 , "empty librariesStructure");    		
    		var libraryStructure = librariesStructure.first(); 
    		Assert.That(libraryStructure.subFolders.size() > 0 , "first Library had no folders");
    		Assert.That(libraryStructure.subFolders[0].views.size() > 0 , "first folder in first Library had no views");    		
    	}
    	
    	
    }
}