// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Windows.Forms; 
using System.Collections.Generic; 
using O2.Kernel; 
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
//O2File:API_IE_ExecutionGUI.cs
//O2File:WatiN_IE_ExtensionMethods.cs
//O2Ref:WatiN.Core.1x.dll

namespace O2.XRules.Database.APIs
{	
	
    public class API_IE_CxWebClient : API_IE_ExecutionGUI
    {   
    	public string DEFAULT_SERVER = 	"https://www.cxprivatecloud.com";   
    	
    	public API_IE_CxWebClient(WatiN_IE ie) : base(ie)
    	{    		
    	 	config(DEFAULT_SERVER);
    	}
    	public API_IE_CxWebClient(WatiN_IE ie, string targetServer) : base(ie)
    	{
    		config(targetServer);
    	}
    	
    	public API_IE_CxWebClient(Control hostControl)	: base(hostControl) 
    	{    		
    		config(DEFAULT_SERVER);
    	}  
    	
    	public void config(string targetServer)
    	{
    		this.TargetServer = targetServer;
    		API_IE_CxWebClient_HelperMethods.ie = this.ie;
    	}
    }
    
    public static class API_IE_CxWebClient_Actions
    {    					
		
		[ShowInGui(Folder ="root")]
		public static API_IE_ExecutionGUI homepage(this API_IE_CxWebClient cxWeb)
		{
			return cxWeb.open(""); 
		}		
		
		
		/*[ShowInGui(Folder ="links")]
		public static API_IE_ExecutionGUI images(this IE_Google ieExecution)
		{
			return ieExecution.open("imghp?hl=en&tab=wi"); 
		}
		
		[ShowInGui(Folder ="links")]
		public static API_IE_ExecutionGUI videos(this IE_Google ieExecution)
		{
			return ieExecution.open("http://videos.google.com/?hl=en&tab=wv"); 
		}

		[ShowInGui(Folder ="links")]
		public static API_IE_ExecutionGUI maps(this IE_Google ieExecution)
		{
			return ieExecution.open("/maps?hl=en&tab=wl"); 
		}
		*/
		
	}   
	
	public static class API_IE_CxWebClient_HelperMethods
    {    					
    	public static WatiN_IE ie;
    	
    	public static API_IE_CxWebClient login(this API_IE_CxWebClient cxClient, ICredential credential)
    	{
			return cxClient.login(credential.UserName, credential.Password);
    	}
    	
    	public static API_IE_CxWebClient login(this API_IE_CxWebClient cxClient, string username, string password)
    	{    	
    		if (cxClient.loggedIn())
    			"[API_IE_CxWebClient][login] user already logged in, skipping login".info();
    		else
    		{
				cxClient.open("/CxWebClient/login.aspx");	     			
				ie.field("txtUserName").value(username);
				ie.field("txtPassword").value(password);
				ie.button("Login").click();				
			}
			return cxClient;
		}
		
		public static bool loggedIn(this API_IE_CxWebClient cxClient)		
    	{    	
    		return ie.hasLink("Logout"); // better detection modes will be needed
    	}
	}
}   