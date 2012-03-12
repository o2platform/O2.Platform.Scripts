// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework; 
using O2.Kernel;
using O2.XRules.Database.APIs;
using O2.XRules.Database.Utils;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Kernel.ExtensionMethods;

//O2File:Test_TM_IE.cs

namespace O2.SecurityInnovation.TeamMentor
{	
	[TestFixture]
    public class Test_TM_IE_UnitTest_AppliedFilters : Test_TM_IE
    {    	    	    	    	    	
    	public Test_TM_IE_UnitTest_AppliedFilters()
		{			
			var ieKey = "Test_TM_IE_UnitTest_AppliedFilters";
			base.set_IE_Object(ieKey);	
			Assert.That(ie.notNull(), "ie object was null");	
			Test_TM.CLOSE_BROWSER_IN_SECONDS = 14;    
			WatiN_IE_ExtensionMethods.WAITFORJSVARIABLE_MAXSLEEPTIMES = 20;
		}
		

		[Test]		
		public void AppliedFilters_View_All_GuidanceItems()  
    	{		
    		lock(ie)
    		{
				base.open("html_pages/_UnitTest_Helpers/AppliedFilters/AppliedFilters_View_All_GuidanceItems.html?time=" + DateTime.Now.Ticks); 						
				var value = ie.waitForJsVariable("UnitTest_Helper_AppliedFilters").str();			
				Assert.That(value.str()=="True","UnitTest_Helper_AppliedFilters value was not True");
				//ie.eval("showFiltersWithCurrentData()");
				Assert.IsNotNull	(ie.getJsVariable("$('#pivotPanel_Technology input')"), "#pivotPanel_Technology input");
				Assert.That			(ie.getJsVariable("$('#pivotPanel_Technology input').length").str().toInt() > 0 ,  "pivotPanel_Technology input').length < 1");						
			}
		}
		
		[Test]		
		public void AppliedFilters_View_All_GuidanceItems__shows_IE_load_prob()  
    	{		
    		lock(ie)
    		{
				base.open("html_pages/_UnitTest_Helpers/AppliedFilters/AppliedFilters_View_All_GuidanceItems - shows IE load prob.html?time=" + DateTime.Now.Ticks); 						
				var value = ie.waitForJsVariable("UnitTest_Helper_AppliedFilters").str();			
				Assert.That(value.str()=="True","UnitTest_Helper_AppliedFilters value was not True");
				ie.eval("showLibrary(2)");				
			}
		}
		

    	
    	[Test]   	
    	[TestFixtureTearDown]
    	public void close_IE()
    	{  	
    		lock(ie)
    		{
    			"ok: close_IE (in {0} seconds)".format(Test_TM.CLOSE_BROWSER_IN_SECONDS).jQuery_Append_Body(ie);
    			base.close_IE_Object();    		
    		}
    	}
    	
    	
    }
}