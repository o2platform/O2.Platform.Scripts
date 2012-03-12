// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Linq; 
using System.Windows.Forms;
using System.Collections.Generic;  
using NUnit.Framework; 
using O2.Kernel; 
using O2.Kernel.ExtensionMethods;
using O2.XRules.Database.APIs; 
using O2.XRules.Database.Utils;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;


//O2File:Test_TM_IE.cs   
 
namespace O2.SecurityInnovation.TeamMentor
{	
	[TestFixture]
    public class Test_TM_WebSite_SecurityTests
    {   
    	Func<int> startFuzz;
    	Func<int> testEngine;
    	Panel popupWindow;
    	
    	public Test_TM_WebSite_SecurityTests()
		{						
		}
		
		[TestFixtureSetUp]
    	public void test()
    	{
    		var assembly =  "TeamMentor Security  - Tool for Username and Password Fuzzing.h2".local().compile_H2Script();
			var items = (Dictionary<string,object>)assembly.executeFirstMethod();			
			popupWindow = items.value<Panel>("popupWindow");			
			testEngine = items.value<Func<int>>("testEngine");			
			startFuzz = items.value<Func<int>>("startFuzz");
    	}
		
		
    	[Test]
    	public void accountFuzzing_testEngine()  
    	{	
    	
			var issues = testEngine();
			Assert.AreEqual(issues,1, "there should only be one issue here");			
    	} 
    	  
    	[Test]
    	public void accountFuzzing_startFuzz()  
    	{	
    	
			var issues = startFuzz();
			Assert.AreEqual(issues,0, "after fuzzing there should be no issues found");			
    	} 
    	
    	[TestFixtureTearDown]
    	public void tearDown()
    	{
    		popupWindow.closeForm_InNSeconds(2);
    	}
    	
    	
    }
}