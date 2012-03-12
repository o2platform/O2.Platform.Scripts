// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using NUnit.Framework;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;

//O2File:Test_TM_Config.cs
//O2Ref:nunit.framework.dll

//O2File:_Extra_methods_Collections.cs
//O2File:_Extra_methods_Misc.cs
//O2File:_Extra_methods_WinForms_Controls.cs
//O2File:_Extra_methods_Web.cs
    	
namespace O2.SecurityInnovation.TeamMentor
{			
	[TestFixture]
    public class Test_TM_Setup
    {	    
    	public static bool IsWebServerUp = false;  // make this static so that we only try to get the html from the server once
    	
    	//[SetUp]
    	public void startServer()
    	{
    		startServer(Test_TM.Port, Test_TM.tmWebSiteFolder);	
    	}
    	 
    	[Test]	
    	public void check_Config_Settings()
    	{
    		Assert.That(Test_TM.tmServer.isUri(), 				"tmServer not Uri");    		
    		Assert.That(Test_TM.tmEmptyPage.isUri(), 			"tmEmptyPage not Uri");
    		Assert.That(Test_TM.invalidPage.isUri(), 			"invalidPage not Uri");
    		Assert.That(Test_TM.tmWebServices.isUri(), 			"tmWebServices not Uri");
    		Assert.That(Test_TM.currentHomePage.isUri(), 		"currentHomePage not Uri");
    		Assert.That(Test_TM.tmSourceCode.dirExists(),		"Test.TM.tmSourceCode not found");
    		Assert.That(Test_TM.tmWebSiteFolder.dirExists(),	"Test.TM.tmWebSiteFolder not found");
    		Assert.That(Test_TM.cassiniWebServer.fileExists(),	"Test.TM.cassiniWebServer not found");     		
    	}    
    	
    	public void startServer(int port, string folder)
    	{
    		var parameters = "/port:{0} /portMode:Specific /path:\"{1}\"".format(port,folder);
			Test_TM.cassiniWebServer.startProcess(parameters);		    		
    	}
    	
    	//[Test]	
    	public void startLocalWebServer()
    	{
    		var webServerProcessName= "CassiniDev";    		
    		 
    		Action<int,string> stopAndStartServer =  
    			(port, folder)=>{    			
    								var currentServers = Processes.getProcessesCalled(webServerProcessName);
    								foreach(var currentServer in currentServers)
    								{
    									currentServer.stop();
//    									Processes.getProcessesCalled(webServerProcessName).stop();						    			
						    			currentServer.WaitForExit();
						    		}
						    		Assert.That(Processes.getProcessCalled(webServerProcessName).isNull(), "There should be no {0} processes at this stage".format(webServerProcessName));    		
						    		startServer(port, folder);
						    		Assert.That(Processes.getProcessesCalled(webServerProcessName).size()==1, "There should be 1 {0} processes at this stage".format(webServerProcessName));
						    		Assert.That("http://127.0.0.1:{0}/".format(port).html().valid(), "could not get html from website");
								};	
			
			stopAndStartServer(Test_TM.Port+1000, Test_TM.tmWebSiteFolder);			
			stopAndStartServer(Test_TM.Port, Test_TM.tmWebSiteFolder);									    		
    	}
    	

    	[Test]
    	public void check_TM_Urls()
    	{
    		Assert.That(Test_TM.tmServer.html().valid(), 		"tmServer html not valid");
    		Assert.That(Test_TM.tmEmptyPage.html().valid(), 	"tmEmptyPage html not valid");
    		Assert.That(Test_TM.invalidPage.html().inValid(), 	"invalidPage html valid");
    		Assert.That(Test_TM.tmWebServices.html().valid(), 	"tmWebServices html not valid");
    		Assert.That(Test_TM.currentHomePage.html().valid(), "currentHomePage html not valid");    		
    	}
		
		
		public void check_if_TM_WebServer_is_Running()
    	{
    		if(IsWebServerUp)	
				return;
			"[check_if_TM_WebServer_is_Running] IsWebServerUp variable false, so checking if server is up (this should only happen once per main execution".info();
			
			var homePageHtml = Test_TM.tmServer.get_Html();
			if (homePageHtml.valid().isFalse())
			{
				"It looks like the server is down, lets start it".info();
				var port = Test_TM.Port;
				var folder = Test_TM.tmWebSiteFolder;
				var parameters = "/port:{0} /portMode:Specific /path:\"{1}\"".format(port,folder);
				Test_TM.cassiniWebServer.startProcess(parameters);
			}
			homePageHtml = Test_TM.tmServer.get_Html();
			Assert.That(homePageHtml.valid(),"The server is not up");									
			IsWebServerUp = true;
			
    	}
   	}
}
