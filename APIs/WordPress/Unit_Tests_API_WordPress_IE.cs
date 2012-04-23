// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework; 
using O2.Kernel;
using O2.XRules.Database.APIs;
using O2.XRules.Database.Utils;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Kernel.ExtensionMethods;
using WatiN.Core.DialogHandlers;
//O2File:Unit_Tests_using_IE.cs
//O2File:API_WordPress_IE.cs
//O2File:API_GuiAutomation.cs

//O2File:_Extra_methods_WinForms_Controls.cs

namespace O2.UnitTests 
{		
	[TestFixture]
    public class Unit_Tests_API_WordPress_IE : Unit_Tests_using_IE
    {	
    	public API_WordPress_IE apiWordPressIE;
    	public AlertAndConfirmDialogHandler alertsHander;
    	public Unit_Tests_API_WordPress_IE() 
    	{    		
    		base.set_IE_Object("Unit_Tests_API_WordPress_IE");
    		alertsHander = ie.getAlertsHandler();
    		var serverUrl = "https://o2platform.wordpress.com".uri();
			apiWordPressIE = new API_WordPress_IE(ie, serverUrl);		
			
			base.CLOSE_BROWSER_IN_SECONDS = 0;
			//base.minimized();
    	}
    	
    	//[Test]
    	public string openWordPressWebsite_with_Click_on_Popup_Error()
    	{   
    		O2Thread.mtaThread(()=> apiWordPressIE.wordPress_com() );
    		"No".click_Button_in_Window("Script Error"); 
    		ie.waitForComplete();
    		return "ok: openWordPressWebsite";
    	}
    	
    	//[Test]
    	public string login()
    	{
    		apiWordPressIE.UserDetails = @"C:\O2\_USERDATA\Accounts.xml".credential("o2platform");     		    						
			apiWordPressIE.login();
			var links = ie.links();
			
			Assert.IsTrue(ie.url().contains(apiWordPressIE.ServerUri.append("wp-admin").str()), "URL was not the expected one");
			Assert.AreEqual("Dashboard ‹ OWASP O2 Platform Blog — WordPress", ie.title(), "URL was not the expected one");
			Assert.IsNotNull(ie.link("New Post"), "didn't find expected link");
			return "ok: login";
		}
    	
    	[Test]
    	public string mediaUpload_CheckValues_for_PostId_and_Nounce()
    	{    		    	
    		var test_PostId  = 1000.random();
    		apiWordPressIE.open("/wp-admin/media-upload.php?post_id={0}&type=image".format(test_PostId));
			var _wpnonce = ie.field("_wpnonce");
			var postId = ie.field("post_id");
			Assert.IsNotNull(_wpnonce, "_wpnonce");
			Assert.IsNotNull(postId, "postId");
			Assert.IsTrue(_wpnonce.value().valid() , "wpnonce value");
			Assert.IsTrue(postId.value().valid() , "postId value");
			Assert.IsTrue(postId.value().toInt() == test_PostId,"postId didn't match test_PostId");
			Assert.IsTrue(_wpnonce.value().hexToLong() > 0, "_wpnonce should be an Hex and its long value should be bigger than 0");
			return "ok: mediaUpload_CheckValues_for_PostId_and_Nounce";
		}			
 
    	[Test]
    	public string closeIE()
    	{
    		base.close_IE_Object();
    		return "ok: closing IE after {0} seconds".format(base.CLOSE_BROWSER_IN_SECONDS);
    	}
    	
    }
}
    	