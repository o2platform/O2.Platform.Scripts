// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods; 
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.External.IE.ExtensionMethods;
using SHDocVw;
using WatiN.Core;
using O2.XRules.Database.Utils;
//O2File:WatiN_IE_ExtensionMethods.cs    
//O2File:WatiN_IE.cs
//O2Ref:Interop.SHDocVw.dll
//O2Ref:WatiN.Core.1x.dll
//O2Ref:O2_External_IE.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:System.Xml.dll
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll
//O2Ref:Microsoft.mshtml.dll

namespace O2.XRules.Database.APIs
{
    public class API_BTOpenZone
    {    
 
    	public WatiN_IE ie;
    	public string  defaultWebPage;
 
    	public API_BTOpenZone()
    	{
    		ie = "".ie(0,450,800,700);  
    		defaultWebPage =  "http://www.google.co.uk";
    	}
 
    	public API_BTOpenZone login()
    	{
    		return login(@"C:\O2\_USERDATA\BtOpenZone.xml");
    	}
 
    	public API_BTOpenZone login(string fileWithLoginDetails)
    	{											
			ie.open(defaultWebPage); 			
			if (ie.title("Google"))
				"we are already conneted to the internet".info();
			else if (ie.title("BT Openzone"))
			{
				"detected BT Openzone page".info();
				// get the login details				
				ICredential credential = fileWithLoginDetails.credential("BtOpenZone");
				if (credential == null)
					credential = ie.askUserForUsernameAndPassword();      	
					
				if (credential==null)
				{
					"no file with credentials provided, or no credential of type BtOpenZone found".debug();
					credential = ie.askUserForUsernameAndPassword();	
				}
				var BTOpenZone_UserName = credential.username();
				var BTOpenZone_Password = credential.password(); 				
				// populate	fields and submit form
				if (ie.hasField("username") && ie.hasField("password"))
				{
					"detected BT Openzone login Form".info();
 
					"submitting login details".debug();
					ie.field("username").value(BTOpenZone_UserName);
					ie.field("password").value(BTOpenZone_Password);
					ie.buttons()[0].click();					
					if (ie.hasField("username"))
					{
						"Login failed, Aborting worklow".error();
						return this;
					}
					ie.open(defaultWebPage); 					
					if (ie.title("Google").isFalse())
					{
						"Expected Google page did not load".error();			
					}
				}
			}
			return this;
    	}
 
    	public API_BTOpenZone logout()
    	{
    		// logout sequence
			"logging out".debug();
			var logoutPage = "https://www.btopenzone.com:8443/accountLogoff/home";			
			var logoutConfirmedPage = "https://www.btopenzone.com:8443/accountLogoff/home?confirmed=true";
			ie.open(logoutPage);		
			ie.open(logoutConfirmedPage);			
			ie.title().info(); 
			if (ie.hasField("username"))
				"logout ok".info();
			else
				"logout failed".info();	
			return this;
    	}
 
    	public API_BTOpenZone closeInNSeconds(int seconds)
    	{
    		ie.closeInNSeconds(seconds);     
    		return this;
    	}
 
    	public API_BTOpenZone waitNSeconds(int seconds)
    	{
    		ie.waitNSeconds(seconds);     
    		return this;
    	}
 
     }
}

