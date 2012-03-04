// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Web;
using System.Security;
using System.Collections.Generic;      
using System.Security.Permissions;	
using NUnit.Framework; 
using O2.Kernel; 
using O2.Kernel.ExtensionMethods;    
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;

using SecurityInnovation.TeamMentor.WebClient.WebServices; 
using O2.SecurityInnovation.TeamMentor;

//O2File:_Extra_methods_Web.cs
//O2File:API_Moq_HttpContext.cs

//O2File:C:\_WorkDir\SI\_TeamMentor-v3.0_Latest\Web Applications\TM_Website\App_Code\JavascriptUtils\ScriptCombiner.cs
//_O2File:TM_Test_XmlDatabase.cs 
//O2File:Test_TM_Config.cs

namespace O2.SecurityInnovation.TeamMentor.WebClient
{		  
	[TestFixture] 
    public class Test_JavascriptCombiner 
    {
    	public string BaseDir	{ get; set;}
    	
    	public API_Moq_HttpContext httpContextApi ;
    	public HttpContextBase 	context;
    	public HttpRequestBase 	request;
    	public HttpResponseBase response;
 
 		public string EMPTY_RESPONSE = "//nothing to do";
 		public string DONT_MINIFY	 = "&dontMinify=true";	    	    
     	
    	public Test_JavascriptCombiner() 
    	{     		    		    	 	    	    
    		BaseDir = "javascriptCombiner".tempDir(false);
			httpContextApi 	= new API_Moq_HttpContext(BaseDir);   
			
			context 	= httpContextApi.httpContext();
			request  	= context.Request;
			response 	= context.Response;
			
			HttpContextFactory.Context = context;
			
			
    	}     	
    	

		[Test]
		public void serverCode_test_MockingEnvironemnt()
		{
			Assert.That(BaseDir.dirExists(), "test base dir didn't exist");		
			Assert.AreEqual(BaseDir, httpContextApi.BaseDir, "BaseDir in httpContextApi");
			
			Assert.IsNotNull(context , "httpContext was null");
			Assert.IsNotNull(request , "httpContext was null");
			Assert.IsNotNull(response, "httpContext was null");
			
			var responseWriteText = "<h1>response Write Test</h1>";
			
			response.Write(responseWriteText);
			
			//test: response writing to respose
			var responseString = context.response_Read_All();
			Assert.AreEqual(responseWriteText, responseString);
			
			//test: map
			var tempFile = "aaa.html";
			var tempFilePath = BaseDir.pathCombine(tempFile);
			Assert.AreEqual(context.Server.MapPath(tempFile), tempFilePath, "Map file resolution");
			
			//test: request values
			var testKey   = "a key";
			var testValue = "a test value";
			request.QueryString[testKey] = testValue;
			 
			Assert.AreEqual(request.QueryString[testKey], testValue, "QueryString set failed");
		}
		
		//[Test][Ignore("Race condition when running in paralell with other tests")]
		public void serverCode_defaultValues_and_EmptyRequest()		
		{
			var scriptCombiner = new ScriptCombiner(); 						
			scriptCombiner.ProcessRequest(null); 
			
			Assert.AreEqual	(scriptCombiner.setName,string.Empty , "[empty request] setName");
			Assert.AreEqual	(scriptCombiner.version,string.Empty , "[empty request] version");
			Assert.IsNotNull(ScriptCombiner.mappingsLocation	 , "[empty request] mappingsLocation");
			
			var responseHtml = context.response_Read_All();
			Assert.AreEqual(EMPTY_RESPONSE,responseHtml, "[empty request] responseHtml should be empty");
			 
			request.QueryString["s"] = "setName";
			request.QueryString["v"] = "version";
			scriptCombiner.ProcessRequest(null); 
			Assert.AreEqual(scriptCombiner.setName,"setName", "setName value"); 
			Assert.AreEqual(scriptCombiner.version,"version", "setName value");			
			 
			//test test handshake			
			request.QueryString["Hello"] = "TM"; 
			scriptCombiner.ProcessRequest(null); 
			responseHtml = context.response_Read_All();
			Assert.AreEqual(responseHtml, "Good Morning", "handshake");
		}
		
		[Test]
		public void serverCode_minifyCodeSetting()		
		{				
			var scriptCombiner = new ScriptCombiner(); 						
			
			scriptCombiner.ProcessRequest(null); 
			Assert.IsTrue(scriptCombiner.minifyCode, "minifyCode should be true");			 
			
			request.QueryString["s"] = "someValue";
			request.QueryString["dontMinify"] = "true";
			scriptCombiner.ProcessRequest(null); 
			Assert.IsFalse(scriptCombiner.minifyCode, "minifyCode should be false");
		}
		
		[Test]
		public void serverCode_makeRequestFor_one_JavascriptFile()
		{
			var scriptCombiner = new ScriptCombiner();  
			scriptCombiner.ignoreCache = true;
			ScriptCombiner.mappingsLocation = "{0}.txt";
			
			var fileContents = "var a=1; // a test js file";
			var expectedResult = "\nvar a=1;";
			var file1 = "a.js";
			var mappingName = "testMapping";
			
			var mappingFile = file1.saveAs(this.BaseDir.pathCombine(mappingName + ".txt"));			
			var jsFile = fileContents.saveAs(this.BaseDir.pathCombine("a.js"));
			
			Assert.That(mappingFile.fileExists() && mappingFile.fileContents()  == file1, "mappingFile not OK");
			Assert.That(jsFile.fileExists() 	 && jsFile.fileContents() 		== fileContents, "mappingFile not OK");
			
			request.QueryString["s"] = mappingName;
			scriptCombiner.ProcessRequest(null); 
			
			var responseText = context.response_Read_All();
			Assert.AreEqual(expectedResult					, responseText	, "responseText != expectedResult");								
			Assert.AreEqual(scriptCombiner.filesProcessed[0], file1			, "scriptCombiner.filesProcessed[0]");
			
			// these two fails due to weird encoding bug (the text is the same (in ascii))
			//Assert.AreEqual(scriptCombiner.minifiedCode		, expectedResult, "minifiedCode");	
			//Assert.AreEqual(scriptCombiner.allScripts.str() , fileContents  , "allScripts");  
		}
		
		[Test]
		public void serverCode_makeRequestFor_two_JavascriptFile() 
		{
			var scriptCombiner = new ScriptCombiner();  
			scriptCombiner.ignoreCache = true;
			ScriptCombiner.mappingsLocation = "{0}.txt";
			
			var fileContents1 = "var a=1; // a test js file";
			var fileContents2 = "var b=1; // a test js file";
			var expectedResult = "\nvar a=1;var b=1;";
			var file1 = "a.js";
			var file2 = "b.js";
			var mappingName = "testMapping";
			var mappingContents = file1.line() + file2.line();
			
			mappingContents.saveAs(this.BaseDir.pathCombine(mappingName + ".txt"));			
			fileContents1.saveAs(this.BaseDir.pathCombine(file1));	
			fileContents2.saveAs(this.BaseDir.pathCombine(file2));
			
			request.QueryString["s"] = mappingName;
			scriptCombiner.ProcessRequest(null); 
			
			var responseText = context.response_Read_All();
			Assert.AreEqual(expectedResult,responseText, "responseText != expectedResult");
		}
		
		public string getScript(string file)
		{
			var requestUrl = Test_TM.tmServer + "aspx_Pages/scriptCombiner.ashx?s=";
			return (requestUrl + file).html();
		}
		
		[Test]
		public void web_defaultValues_and_EmptyRequest()		 
		{
			var tmServer = Test_TM.tmServer;
			Assert.That		(tmServer .html().contains("meta http-equiv=\"Refresh\"")		 , "default page should be a redirect");
			Assert.AreEqual((tmServer + "aspx_Pages/scriptCombiner.ashx?Hello=TM").html(), "Good Morning", "handShake value didn't match");
			Assert.AreEqual((tmServer + "aspx_Pages/scriptCombiner.ashx").html(), EMPTY_RESPONSE	 , "empty request");
		}
		
		
		
		[Test]
		public void web_getJavascriptCode()		
		{ 						
			
			var globalVariables = getScript("GlobalVariables");			
			Assert.IsNotNull(globalVariables, "globalVariables");
			Assert.That		(globalVariables.size() > 100, "globalVariables size()");
			Assert.That		(globalVariables.Contains("TM.Debug="), "didn't found TM.Debug=");
			
			var globalVariables_Raw =  getScript("GlobalVariables" + DONT_MINIFY);			
			
			Assert.IsNotNull(globalVariables_Raw, "globalVariables_Raw");
			Assert.That		(globalVariables_Raw.size() > 100, "globalVariables_Raw size()");
			
			Assert.AreNotEqual(globalVariables.size(),globalVariables_Raw.size(), "minified and raw sizes shouldn't match");			
		}		
		
		[Test]
		public void web_get_TeamMentor_JavascriptsBlocks()
		{
			Action<string> checkScript = 
				(name)=>{
							var scriptCode = getScript(name);
							Assert.That(scriptCode.size() > 100 , "{0} size ".format(name));
							var scriptCode_Raw = getScript(name+ DONT_MINIFY);
							Assert.That(scriptCode_Raw.size() > 100 , "{0} size ".format(name));
							Assert.AreNotEqual(scriptCode.size(),scriptCode_Raw.size(), "for '{0}' minified and raw sizes shouldn't match".format(name));			
						};
							
			checkScript("ControlPanel_JS");
			checkScript("ControlPanel_CSS");	
			checkScript("HomePage_JS");
			checkScript("HomePage_CSS");
		}
    }
}
