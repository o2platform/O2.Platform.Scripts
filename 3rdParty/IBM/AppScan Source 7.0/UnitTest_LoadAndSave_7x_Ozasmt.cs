// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using O2.Interfaces.O2Findings;
using O2.DotNetWrappers.O2Findings;
using O2.DotNetWrappers.ExtensionMethods;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6;
using O2.ImportExport.OunceLabs.Ozasmt_OunceV6_1;
using O2.XRules.ThirdPary.IBM;
using O2.XRules.Database.Findings;

//O2File:O2AssessmentSave_OunceV7_0.cs
//O2File:Findings_ExtensionMethods.cs 
//O2File:Findings_ExtensionMethods.cs

//O2Ref:nunit.framework.dll

namespace O2.XRules.Database.UnitTests
{		
	[TestFixture]
    public class LoadAndSave_Ozasmt8
    {        
    	public string 			TestFile 		{ get; set; }
    	public List<IO2Finding> Findings_Load	{ get; set; }    	
    	
    	public LoadAndSave_Ozasmt8()
    	{
    		TestFile = "JpetStore_8.6.ozasmt".local();
    		Findings_Load = TestFile.loadO2Findings();
    	}
    	
    	[Test]
    	public void IsTestFileOk()
    	{    		
    		Assert.That(TestFile.fileExists(),"Couldn't find test file");    	    		    		
    		Assert.That(Findings_Load.size()>0, "There where no findings loaded");
    	}
 
		[Test]
    	public void SaveAndLoad_Work()
    	{ 
    		// save findings using new O2AssessmentSave_OunceV7 engine    		
			var savedFile = new O2AssessmentSave_OunceV7().save(Findings_Load);  									
			
			// check that it exists
			Assert.That(TestFile.fileExists(),"Couldn't find O2AssessmentSave_OunceV7 saved file"); 
			
			// check that we can load the saved file with 7x 
			var o2Assessment = new O2AssessmentLoad_OunceV7_0().loadFile(savedFile);				
			Assert.That(o2Assessment.notNull(), "O2AssessmentLoad_OunceV7_0 failed to load");		
			
			// check that we CAN'T load the saved file with 6.1 
			o2Assessment = new O2AssessmentLoad_OunceV6_1().loadFile(savedFile);					
			Assert.That(o2Assessment.isNull(), "O2AssessmentLoad_OunceV6_1 failed to load");		
			
			// check that we CAN'T load the saved file with 6.0 
			o2Assessment = new O2AssessmentLoad_OunceV6().loadFile(savedFile);					
			Assert.That(o2Assessment.isNull(), "O2AssessmentLoad_OunceV6_0 failed to load");		
		}
		
		[Test]
		public void CheckAssessmentRunData_AfterLoad_and_InMemorySave() 
		{
			var directlyLoaded_Assessment = O2Assessment_OunceV7_Utils.getVersionFromDirectLoad(TestFile);
			var fromSaveEngine_Assessment = O2Assessment_OunceV7_Utils.getVersionFromSaveEngine(TestFile);
			
			Assert.That(directlyLoaded_Assessment.name == fromSaveEngine_Assessment.name ,"name didn't match");
		}

		
		[Test]
		public void SaveAndLoad_FindingsCountIsTheSame()
		{
			var savedFile = new O2AssessmentSave_OunceV7().save(Findings_Load);
			var savedFindings = savedFile.loadO2Findings();
			Assert.That(savedFindings.size() > 0 , "There where no findings loaded"); 
			Assert.That(savedFindings.size() == Findings_Load.size() , "Finding's size doesn't match"); 
		}
//			var findingsSaved = savedFile.loadO2Findings();
//			Assert.That(findingsSaved.size()>0, "There where no saved Findings loaded");
//    	}
    }
}
