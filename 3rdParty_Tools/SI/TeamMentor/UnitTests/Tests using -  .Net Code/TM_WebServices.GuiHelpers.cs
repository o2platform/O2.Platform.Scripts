using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;    
using O2.Kernel;   
using O2.Kernel.ExtensionMethods; 
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
using NUnit.Framework;  
using SecurityInnovation.TeamMentor.WebClient;
using SecurityInnovation.TeamMentor.WebClient.WebServices;    
//O2Ref:nunit.framework.dll     
  
//O2File:TM_Test_XmlDatabase.cs 

namespace O2.SecurityInnovation.TeamMentor.WebClient
{		  
	[TestFixture]  
    public class Test_TM_WebServices_GuiHelpers : TM_Test_XmlDatabase
    { 
    	
		static Test_TM_WebServices_GuiHelpers()    
    	{    		
    		TMConfig.BaseFolder = Test_TM.tmWebSiteFolder;    		
    	} 
    	
    	public Test_TM_WebServices_GuiHelpers()    
    	{    		    		
    		//UserGroup.Admin.setThreadPrincipalWithRoles(); // set current user as Admin
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