// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Data; 
using System.Linq; 
using System.Data.SqlClient; 
using System.Collections.Generic;
using System.Diagnostics;   
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;  
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.XRules.Database.Utils;
using O2.XRules.Database.APIs;   
using NUnit.Framework; 
using SecurityInnovation.TeamMentor.WebClient.WebServices; 
using SecurityInnovation.TeamMentor.WebClient;

//O2File:TM_Test_XmlDatabase.cs

//O2Ref:nunit.framework.dll     

 
namespace O2.SecurityInnovation.TeamMentor.WebClient.JavascriptProxy_XmlDatabase
{		 	
	[TestFixture]
    public class Temp_Libraries : TM_Test_XmlDatabase
    {     	    	
    	public string tempUnitTestsDir;
    	public string owaspLibraryFile;
    	
     	static Temp_Libraries() { TMConfig.BaseFolder = Test_TM.tmWebSiteFolder; } 
     	
     	public Temp_Libraries()
     	{
     		 tempUnitTestsDir= "_TM_UnitTests".tempDir(false); 
     		 owaspLibraryFile = Test_TM.tmWebSiteFolder.pathCombine("_test_TM_Libraries")
											  			   .pathCombine("OWASP.zip");
     	}
     	
     	[TestFixtureSetUp] 
    	public void ensure_LibraryIs_TestUnitTestsDir() 
    	{     		    		
			Assert.True(this.tempUnitTestsDir.dirExists(), "_TM_UnitTests didn't exist: {0}".format(tempUnitTestsDir));
			var currentLibraryPath = tmWebServices.XmlDatabase_GetLibraryPath(); 
			if (currentLibraryPath != this.tempUnitTestsDir)
				tmWebServices.XmlDatabase_SetLibraryPath(tempUnitTestsDir);			
			currentLibraryPath = tmWebServices.XmlDatabase_GetLibraryPath(); 	
    		Assert.AreEqual(tempUnitTestsDir, currentLibraryPath, "XmlDatabase_GetLibraryPath != tempUnitTestsDir");    		
    		Assert.IsTrue(owaspLibraryFile.fileExists(), "owaspLibraryFile file doesn't Exists: {0}".format(owaspLibraryFile));    		
    	}    	    	      	
    	
    	public void importLibrary(string libraryName , string fileOrUrl, bool importShouldWork)
    	{
	    	var library = tmWebServices.GetLibraryByName(libraryName);
			if (library.notNull())
			{
				var deleteResult = tmWebServices.DeleteLibrary(library.id.guid());
				Assert.IsTrue(deleteResult, "first library delete failed");
			}
			
			library = tmWebServices.GetLibraryByName(libraryName); 
			Assert.IsNull(library, "after deleted owaspLibrary should be null");
				 				
			var importResult = tmWebServices.XmlDatabase_ImportLibrary_fromZipFile(fileOrUrl); 
			if(importShouldWork)
			{
				Assert.IsTrue(importResult, "import library failed");			
				library = tmWebServices.GetLibraryByName(libraryName);
				Assert.IsNotNull(library, "after import owaspLibrary should not be null");
			}
			else
			{
				Assert.IsFalse(importResult, "import library worked when it should failed");			
				library = tmWebServices.GetLibraryByName("OWASP");
				Assert.IsNull(library, "after import owaspLibrary should be null");
			}
    	}
    	
    	[Test]
    	public void importLibrary()
    	{
	    	importLibrary("OWASP", this.owaspLibraryFile, true);
    	}

		[Test]
    	public void importTestLibrary_OWASP_via_Url()
    	{
    		var badOwaspLibraryFileUrl = Test_TM.tmServer.uri().append("/_test_TM_Libraries/_OWASP.zip").str();
    		importLibrary("OWASP", badOwaspLibraryFileUrl,false);
    		var goodOwaspLibraryFileUrl = Test_TM.tmServer.uri().append("/_test_TM_Libraries/OWASP.zip").str();
    		importLibrary("OWASP", goodOwaspLibraryFileUrl,true);
    	}
    	    	
    	/*[Test]
    	public void importTestLibrary_SI()
    	{
	    	var owaspLibrary = tmWebServices.GetLibraryByName("OWASP");
			if (owaspLibrary.notNull())
			{
				var deleteResult = tmWebServices.DeleteLibrary(owaspLibrary.id.guid());  
				Assert.IsTrue(deleteResult, "first library delete failed");
			}
			owaspLibrary = tmWebServices.GetLibraryByName("OWASP");
			Assert.IsNull(owaspLibrary, "after deleted owaspLibrary should be null");
				 				
			var importResult = tmWebServices.XmlDatabase_ImportLibrary_fromZipFile(owaspLibraryFile); 
			Assert.IsTrue(importResult, "import library failed");
			
			owaspLibrary = tmWebServices.GetLibraryByName("OWASP");
			Assert.IsNotNull(owaspLibrary, "after import owaspLibrary should not be null");
    	}*/
    	
    }
} 	