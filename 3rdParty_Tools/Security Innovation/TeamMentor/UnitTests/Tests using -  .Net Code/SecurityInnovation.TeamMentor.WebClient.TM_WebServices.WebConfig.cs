using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;    
using NUnit.Framework;  
using O2.Kernel;   
using O2.Kernel.ExtensionMethods; 
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
  
//O2File:Test_TM_Config.cs

namespace O2.SecurityInnovation.TeamMentor
{		  
	[TestFixture]  
    public class Test_TM_WebServices_WebConfig
    {     			 
    	public string WebConfigFile { get; set; }
    	
    	public Test_TM_WebServices_WebConfig()    
    	{    		    		    		
    		WebConfigFile = Test_TM.tmWebSiteFolder.pathCombine("web.config");
    	} 
    	    	
    	[Test]  
    	public void loadWebConfigFile()
    	{       		
    		Assert.That(WebConfigFile.fileExists(), "could not find, web.config file: {0}".format(WebConfigFile));
    		var xRoot = WebConfigFile.xRoot();
    		Assert.IsNotNull(xRoot, "xRoot of webConfigFile");
    	}    	
    	
    	[Test]  
    	public void system_web_compilation_debug_IS_NOT_TRUE()
    	{   
    		var compilation = WebConfigFile.xRoot().element("system.web").element("compilation");
    		Assert.IsNotNull(compilation, "compilation element");
    		var debugAttribute = compilation.attribute("debug");
    		Assert.IsNotNull(debugAttribute, "debug attribute");			
			var debugValue = debugAttribute.value();
			Assert.AreNotEqual(debugValue.lower() ,"true", "system.web / compilation / debug attribute value should not be true");		
		}
		
/*		[Test]  
    	public void system_web_compilation_customErrors_IS_NOT_Off()
    	{   
    		var customErrors = WebConfigFile.xRoot().element("system.web").element("customErrors");
    		Assert.IsNotNull(customErrors, "customErrors element");
    		var modeAttribute = customErrors.attribute("mode");
    		Assert.IsNotNull(modeAttribute, "mode attribute");			
			var value = modeAttribute.value();
			Assert.AreNotEqual(value.lower() ,"off", "system.web / customErrors / mode value should not be Off");		
		}					
*/		
		[Test]  
    	public void system_web_compilation_customErrors_IS_Off()
    	{   
    		var customErrors = WebConfigFile.xRoot().element("system.web").element("customErrors");
    		Assert.IsNotNull(customErrors, "customErrors element");
    		var modeAttribute = customErrors.attribute("mode");
    		Assert.IsNotNull(modeAttribute, "mode attribute");			
			var value = modeAttribute.value();
			Assert.AreEqual(value.lower() ,"off", "system.web / customErrors / mode value should be Off");		
		}					
	}       
}