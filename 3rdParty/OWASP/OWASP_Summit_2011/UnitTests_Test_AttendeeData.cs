// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.APIs;
using NUnit.Framework;
//O2File:API_OWASP_Summit_2011.cs  
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll
//O2Ref:nunit.framework.dll

namespace O2.UnitTests.OWASP_Summit_2011
{		
	[TestFixture]
    public class Test_AttendeeData
    {        	    
    	[Test]
    	public string canParseAllTemplates()
    	{
    		var summitApi = new API_OWASP_Summit_2011(); 
    		var attendeesPages = summitApi.attendees(true);
    		foreach(var testPage in attendeesPages)
			{
				if (testPage.contains("Summit_2011_Attendee"))
				{
					var page = testPage.split("|")[0].trim();
					//return testPage;
					
					var wikiApi = new OwaspWikiAPI(false); 
					 
					var templateData = new WikiText_Template();
					
					templateData.parse(wikiApi,page);
					Assert.That(templateData.Status.str() == "Parsed", "Could not parse page: {0}".format(page));
				}
			}
			return "ok";
    	}
    	
    }
}
