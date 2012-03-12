// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;

//O2Ref:nunit.framework.dll     

namespace O2.SecurityInnovation.TeamMentor
{				
    public class Test_TM
    {
		public static string tmSourceCode = @"C:\_WorkDir\TeamMentor\TeamMentor-3.1-Release\"; 

    	public static string IPAddress		 = "127.0.0.1";
    	public static int 	 Port		 	 = 12355;
    	public static string tmServer 		 = "http://{0}.:{1}/".format(IPAddress,Port);     	
    	
    	public static string tmEmptyPage 	 = "{0}{1}?time={2}".format(tmServer, "html_pages/emptyPage.html" , DateTime.Now.Ticks);
    	public static string tmWebServices 	 = "{0}{1}".format(tmServer,"Aspx_Pages/TM_WebServices.asmx");
    	public static string currentHomePage = "{0}{1}".format(tmServer, "html_pages/Gui/TeamMentor.html");
    	public static string invalidPage 	 = "{0}{1}".format(tmServer, "asdasdasd.aspx");
    	
    	public static string tmWebSiteFolder  =  tmSourceCode.pathCombine(@"Web Applications\TM_Website");
    	public static string tmConfigFile  =  tmWebSiteFolder.pathCombine(@"TmConfig.config");
    	public static string cassiniWebServer =  tmSourceCode.pathCombine(@"WebServer\CassiniDev.exe");    	    	
    	
    	public static int CLOSE_WINDOW_IN_SECONDS = 0;
    	public static int CLOSE_BROWSER_IN_SECONDS = 0;
    	
		public static bool IsWebServerUp = false;  // make this static so that we only try to get the html from the server once
		
		public static string password_Admin		= "!!tmbeta";
		public static string password_Editor	= "!!tmbeta";
		public static string password_Reader	= "!!tmbeta";
		
		public static string passwordHash_Admin  = "9eff3dbd350bc5ef54fe7143658565bd45b6476db7c511f35206a143287f741d";
		public static string passwordHash_Editor = "acdc0a6478b0efa72af4813c45e6c0a488aa00c189baa3bd273c6dfc09a7c6b5";
		public static string passwordHash_Reader = "4809d305b4f6eccf66b5bdb559cc3bd8a1bdd1e67b64a2b4f8a70eb8b0ed7f79";
		
   	}
}
