// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO; 
using System.Linq;
using System.Collections.Generic;  
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using O2.Kernel; 
using O2.XRules.Database.APIs;
using O2.XRules.Database.Utils;
using O2.DotNetWrappers.Network;
using O2.Kernel.ExtensionMethods;
 
//O2File:Test_TM_Config.cs
//O2File:TM_WebServices.cs
//O2File:_Extra_methods_Web.cs
//O2File:_Extra_methods_Collections.cs

//O2Ref:nunit.framework.dll

namespace O2.SecurityInnovation.TeamMentor
{	
	[TestFixture]
    public class Test_TM_WSDL_SecurityTests
    {
    	public static TM_WebServices tmWebServices;
    	
    	public Test_TM_WSDL_SecurityTests()
    	{
			tmWebServices = new TM_WebServices();    	
			tmWebServices.Url = Test_TM.tmWebServices;
    	}    	    	    	    	
    	
    	
    	public void testForErrorsInException(Action action)
    	{
    		try
			{
				action();
			}
			catch(Exception ex)
			{
				ex.log();
				Assert.IsFalse(ex.Message.contains("TM_Xml_Database_JavaScriptProxy.cs") ||
							   ex.Message.contains("TM_Xml_Database_JavaScriptProxy.cs"), "Error Message disclosed filePath");
			}			
    	}
    	
    	
    	[Test]
    	public void GetGuidanceItemById_InformationDisclosure_EmptyRequest()
    	{			    		
    		testForErrorsInException(
    			()=>  tmWebServices.GetGuidanceItemById(""));				
    	}    
    	
    	[Test]
    	public void GetGuidanceItemById_InformationDisclosure_BadGui()
    	{			    		
    		testForErrorsInException(
    			()=>  tmWebServices.GetGuidanceItemById("asfdasdf"));				
    	}	
    }
}