// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic; 
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
//O2File:WatiN_IE_ExtensionMethods.cs    
//O2File:WatiN_IE.cs
//O2Ref:WatiN.Core.1x.dll


namespace O2.XRules.Database.APIs
{
    public class API_JPetStore
    {    
		public WatiN_IE ie;
		public string appUrl = "http://localhost.:8080/jpetstore";
		
		public API_JPetStore(WatiN_IE _ie)
		{
			ie = _ie;
		}			
		
		public API_JPetStore open(string virtualPath)
		{
			if (virtualPath.starts("/").isFalse())
				virtualPath = "/{0}".format(virtualPath);
			var fullUri = "{0}{1}".format(appUrl, virtualPath).uri();
			ie.open(fullUri.str());
			return this;
		}
    }
    
   	
    public static class API_JPetStore_ExtensionMethods
    {
    	public static API_JPetStore homePage(this API_JPetStore jPetStore)    	
    	{    		
    		jPetStore.open("");
    		return jPetStore;
    	}
    	
    	public static API_JPetStore login_DefaultValues(this API_JPetStore jPetStore)    	
    	{    		
    		jPetStore.open("/shop/signonForm.do"); 
			jPetStore.ie.buttons()[1].click();
    		return jPetStore;
    	}
    	
    	public static bool login(this API_JPetStore jPetStore, string username, string password)    	
    	{    		
    		jPetStore.open("/shop/signonForm.do"); 
			var ie = jPetStore.ie;
			ie.field("username").value(username);
			ie.field("password").value(password);
			jPetStore.ie.buttons()[1].click();
    		return ie.IE.Html.contains("Invalid username or password. Signon failed.");
    	}
    	
    	public static API_JPetStore logout(this API_JPetStore jPetStore)    	
    	{    		
    		jPetStore.open("/shop/signoff.do"); 			
    		return jPetStore;
    	}

		public static API_JPetStore createAccount(this API_JPetStore jPetStore, string username, string password)
		{
			return jPetStore.createAccount(username, password, username,10.randomLetters(),10.randomLetters(),
															   10.randomLetters(),10.randomLetters(),10.randomLetters(),
															   10.randomLetters(),10.randomLetters(),10.randomLetters());
		}
		
		public static API_JPetStore createAccount(this API_JPetStore jPetStore, string username, string password , 
													   string firstName, string lastName, string address1,
													   string phone, string city, string state, string zip,
													   string country, string email)
		{
			jPetStore.open("/shop/newAccount.do");
			var ie = jPetStore.ie;
			ie.field("account.username").value(username);
			ie.field("account.password").value(password);
			ie.field("repeatedPassword").value(password);
			ie.field("account.firstName").value(firstName);
			ie.field("account.lastName").value(lastName);
			ie.field("account.address1").value(address1);
			ie.field("account.phone").value(phone);
			ie.field("account.city").value(city);
			ie.field("account.state").value(state);
			ie.field("account.zip").value(zip);
			ie.field("account.country").value(country);
			ie.field("account.email").value(email);
			ie.button("Save Account Information").click();
    		return jPetStore;
    	}
    	
    	public static API_JPetStore loginPlaceAnOrderAndGoToCheckout(this API_JPetStore jPetStore)
    	{
			jPetStore.homePage();	
			var ie = jPetStore.ie;
			ie.link("Enter the Store").click();			
			var signOffLink = ie.links().where((link)=> link.url().contains("signonForm.do")).first();
			if(signOffLink.notNull())
			{
				signOffLink.click();
				ie.buttons()[1].click();
			}			
			ie.links().where((link)=> link.url().contains("FISH"))[0].click();			
			ie.link("FI-FW-01 ").click();			
			ie.links().where((link)=> link.url().contains("addItemToCart"))[0].click();			
			ie.links().where((link)=> link.url().contains("checkout.do"))[0].click();			
			ie.links().where((link)=> link.url().contains("newOrder.do"))[0].click(); 
			return jPetStore;
		}

    }
}
