// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;   
using O2.DotNetWrappers.ExtensionMethods; 
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;    
using NUnit.Framework; 
using Microsoft.Practices.Unity;  
using SecurityInnovation.TeamMentor.WebClient.WebServices;
using SecurityInnovation.TeamMentor.WebClient;
using SecurityInnovation.TeamMentor.Authentication.WebServices.AuthorizationRules;
using SecurityInnovation.TeamMentor.Authentication.ExtensionMethods;

//O2File:Test_TM_Config.cs 
 
//O2Ref:nunit.framework.dll
//O2Ref:System.Web.Services.dll 
//O2Ref:Microsoft.Practices.Unity.dll      
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll

//_O2File:C:\_WorkDir\SI\_TeamMentor-v3.0_Latest\Web Applications\TM_Website\App_Code\WebServices\TM_WebServices.asmx.cs
//O2File:C:\_WorkDir\TeamMentor\TeamMentor-3.1-Release\Web Applications\TM_Website\App_Code\WebServices\TM_WebServices.asmx.cs
//_O2Ref:TM_WebServices.asmx.dll 

namespace O2.SecurityInnovation.TeamMentor.WebClient
{		 
//	[TestFixture]
    public class TM_Test_XmlDatabase
    { 
    	public static IUnityContainer moq_Container { get; set; }
    	public static TM_Xml_Database_JavaScriptProxy tmXmlDatabase_JavascriptProxy {get;set;}    	
    	public static TM_Xml_Database tmXmlDatabase { get; set;}    	  
    	public static TM_WebServices tmWebServices  { get; set; }    
    	
     	static TM_Test_XmlDatabase() 
     	{  
     		"[in TM_Test_XmlDatabase]".info();
     		"[TM_Test_XmlDatabase] TMConfig.BaseFolder: {0}".debug(TMConfig.BaseFolder.fullPath() );
     		"[TM_Test_XmlDatabase] PublicDI.config.CurrentExecutableDirectory: {0}".info(PublicDI.config.CurrentExecutableDirectory.fullPath());
     		if (TMConfig.BaseFolder.inValid() || TMConfig.BaseFolder.contains(PublicDI.config.CurrentExecutableDirectory))
     		{
	     		TMConfig.BaseFolder = PublicDI.CurrentScript.directoryName()
															.pathCombine(@"..\..\Web Applications\TM_Website");   					
				"Mapped TMConfig.BaseFolder:{0}".debug(TMConfig.BaseFolder);
			}														
			if (TMConfig.BaseFolder.dirExists())
			{
				"in TM_Test_XmlDatabase set TMConfig to :{0}".info(TMConfig.BaseFolder);
	     		testsSetUp();     	 
	     	} 
	     	else
	     		"Note:Couldn't find TMConfig.BaseFolder, so you will need to set-it manually and directly call the testsSetUp method".error();			
     	}
     	 
    	public TM_Test_XmlDatabase() 
    		: this(null)
    	{       		
    	}
    	
    	public TM_Test_XmlDatabase(string configDir) 
    		: this(configDir, UserGroup.Admin) // default to current user as Admin
    	{    		
    	}
    	
    	public TM_Test_XmlDatabase(string configDir, UserGroup userGroup)
    	{
    		"in TM_Test_XmlDatabase.ctor".info();
    		if (configDir.valid())
    		{
    			TMConfig.BaseFolder = configDir;
    			testsSetUp();	
    		}
    		else
    			if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Shift)
    				testsSetUp();		
    		    		
    		userGroup.setThreadPrincipalWithRoles(); 
    	}
    	
		public static void testsSetUp()
		{			
			"in testsSetUp".info();
    		ActivityDB.DontLog = true;     		    		    		
			//TM_Xml_Database.setDataFromCurrentScript("..\\..");
			tmXmlDatabase_JavascriptProxy = UnityInjection.useEnvironment_XmlDatabase();
			tmXmlDatabase = tmXmlDatabase_JavascriptProxy.tmXmlDatabase;			
			moq_Container = UnityInjection.container;	
			tmWebServices = moq_Container.Resolve<TM_WebServices>();  // new TM_WebServices(); 
			"done in testsSetUp".info();
		}
		
		public TM_Xml_Database xmlDatabase()
		{						
			return tmXmlDatabase;
		}
		
		public TM_WebServices webServices()
		{						
			return tmWebServices;
		}
		
		
/*		[Test]
		public string test_If_all_is_OK()
		{
			var libraries = tmWebServices.GetLibraries();
			Assert.That(libraries.notNull(), "libraries was null");
			Assert.That(libraries.size() > 0, "libraries was null");
			return "all ok: got {0} libraries".format(libraries.size());
		}		
*/    		
    }

}    
	