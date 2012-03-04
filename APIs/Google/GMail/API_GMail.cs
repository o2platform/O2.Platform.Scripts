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
//O2File:ISecretData.cs
//O2Ref:Interop.SHDocVw.dll
//O2Ref:WatiN.Core.1x.dll
//O2Ref:O2_External_IE.dll
//O2Ref:System.Xml.Linq.dll
//O2Ref:System.Xml.dll
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll
//O2Ref:Microsoft.mshtml.dll
 
namespace O2.XRules.Database.APIs
{
    public class API_GMail
    {    
 
    	public WatiN_IE ie;
    	public string  defaultWebPage;
 
    	public API_GMail()
    	{
    		
    	}
 
    	public API_GMail createNewEmailAccount()
    	{
    		var ie = "".ie(0,480,800,600);
 
			ie.open("https://www.google.com/accounts/NewAccount?service=mail");
			 
			var credential = ie.askUserForUsernameAndPassword();	
			 
			var newUsername = credential.username();
			var newPassword = credential.password(); 
			 
			ie.textField("FirstName").value("first");    
			ie.textField("LastName").value("last");     
			ie.textField("Email").value(newUsername);   
			ie.textField("Passwd").value(newPassword);    
			ie.textField("PasswdAgain").value(newPassword);   
			ie.checkBox("homepageSet").uncheck(); 
			ie.checkBox("PersistentCookie").uncheck();
			ie.checkBox("smhck").uncheck(); 
			ie.selectList("questions").select(2); 
			ie.textField("IdentityAnswer").value("This is an answer to the question");   
			 
			 
			ie.button("check availability!").click(); 
			ie.wait(500);	// need to sleep a little bit to allow the Javascript update to happen
			ie.resolveCaptcha("checkavailurl", "checkavailcaptcha");
			ie.button("check availability!").click(); 
			 
			ie.resolveCaptcha("newaccounturl", "newaccountcaptcha");
			ie.button("submitbutton").click();
			 
			// next page:
			if (ie.textFieldExists("MobileNumber")) 
			{
				var mobileNumber = ie.askUserQuestion("What is your mobile number", "O2 Question","");
				ie.textField("MobileNumber").value(mobileNumber);  
				ie.button("submitbutton").click();
			 
				//verify mobile
				var answerInMobile = ie.askUserQuestion("What is the verification code", "O2 Question","");
				ie.textField("idvGivenAnswer").value(answerInMobile);  
				ie.button("submitbutton").click();
			 
				// we should be in http://mail.google.com/mail/help/intro.html
				ie.open("http://mail.google.com/mail");
			}
			//ie.closeInNSeconds(50);    			
			return this;
		} 
     }
}

