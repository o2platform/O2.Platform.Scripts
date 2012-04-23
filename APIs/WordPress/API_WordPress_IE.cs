// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Web;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using O2.Kernel;
using O2.Interfaces.O2Core;
using O2.XRules.Database.Utils;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.ExtensionMethods;

//O2File:WatiN_IE_ExtensionMethods.cs 
//O2File:HtmlAgilityPack_ExtensionMethods.cs 
//O2Ref:WatiN.Core.1x.dll

//O2File:_Extra_methods_Misc.cs

namespace O2.XRules.Database.APIs
{	
	/*public class API_WordPress_IE_Test
	{
		public void testGui()
		{
			var popupWindow = "API_WordPress_IE".popupWindow(700,500);						
		}
	}*/
	
    public class API_WordPress_IE
    {        	
    	public Uri ServerUri		{ get; set;}    	
    	public ICredential UserDetails	{ get; set;}    	
    	public WatiN_IE ie;    	
    	
    	public API_WordPress_IE(WatiN_IE _ie, Uri serverUri) 
    	{
    		this.ie = _ie;   
    		this.ServerUri = serverUri;
    		
    		configureDependencies();
    	}
    	
    	public void configureDependencies()
    	{
    		API_IE_WordPress_IE_ExtensionMethods_Urls.ie = this.ie;
    		API_IE_WordPress_IE_ExtensionMethods_Actions.ie = this.ie;
    	}
    }
    
    public static class API_IE_WordPress_IE_ExtensionMethods_Urls
    {
    	public static WatiN_IE ie;
    	
    	public static API_WordPress_IE open(this API_WordPress_IE apiWordPressIE, string virtualPath) 
    	{
    		var fullUri = apiWordPressIE.ServerUri.append(virtualPath);
    		apiWordPressIE.ie.open(fullUri.str());
    		return apiWordPressIE;
    	}
    	
    	public static API_WordPress_IE wordPress_com(this API_WordPress_IE apiWordPressIE) 
    	{     		
    		apiWordPressIE.ie.open("http://wordpress.com");
    		return apiWordPressIE;
    	}
    	
    	public static API_WordPress_IE homePage(this API_WordPress_IE apiWordPressIE) 
    	{ return apiWordPressIE.open("/");}
    	
    	public static API_WordPress_IE loginPage(this API_WordPress_IE apiWordPressIE) 
    	{ return apiWordPressIE.open("/wp-login.php");}
    }
   
    
    public static class API_IE_WordPress_IE_ExtensionMethods_Actions
    {
    	public static WatiN_IE ie;
    	
    	public static API_WordPress_IE login(this API_WordPress_IE apiWordPressIE)
    	{    	
    		if (apiWordPressIE.UserDetails.isNull())
				apiWordPressIE.UserDetails = ascx_AskUserForLoginDetails.ask();			
			var credential = apiWordPressIE.UserDetails;
			if (credential.notNull())
			{
				apiWordPressIE.loginPage();
  				ie.field("log", credential.UserName);
  				ie.field("pwd", credential.Password);
  				ie.button("Log In").click();
  			}
  			else
  				"[API_WordPress_IE] [login] no credentials provided".error();
  			return apiWordPressIE;
		}
		
		public static string uploadPicture_viaHttp(this API_WordPress_IE apiWordPressIE, string fileToUpload, string wpnonce, string postId, string loggedInCookie, string authCookie, string cookies)
		{		
			var httpMultiPartForm = new HttpMultiPartForm();
			
			string postUrl = apiWordPressIE.ServerUri.append("/wp-admin/async-upload.php").str();			
			string fileName = fileToUpload.fileName(); 
			//string fileFormat = "jpeg";
			string userAgent = "O2Platform.com";				
			
			FileStream fs = new FileStream(fileToUpload, FileMode.Open, FileAccess.Read);
			byte[] data = new byte[fs.Length];
			fs.Read(data, 0, data.Length);
			fs.Close();
			
			var postParameters = new Dictionary<string, object>();
			postParameters.Add("Filename", fileName);
			postParameters.Add("_wpnonce", wpnonce);
			postParameters.Add("post_id",postId);
			postParameters.Add("type", "image");
			postParameters.Add("tab", "type"); 
			postParameters.Add("logged_in_cookie", loggedInCookie);
			postParameters.Add("short", "1"); 
			postParameters.Add("auth_cookie", authCookie);
			
			
			string fileContentType = "multipart/form-data";//"application/octet-stream";
			
			postParameters.Add("async-upload", new HttpMultiPartForm.FileParameter(data, fileName, fileContentType)); 
			
			var httpWebResponse = httpMultiPartForm.MultipartFormDataPost( postUrl,  userAgent, postParameters,  cookies);
			StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream());
			string  responseHtml = reader.ReadToEnd();
			httpWebResponse.Close();
			return responseHtml;

		}
    }
}