using System;
using System.Linq;
using System.Text; 
using System.Collections.Generic;
using O2.DotNetWrappers.ExtensionMethods;

namespace O2.XRules.Database.APIs
{
	[Serializable]
	public class HttpRequest
	{
		public string Request_Url 		{ get; set;}
		public string Request_Method 	{ get; set;}
		public string Request_Headers  	{ get; set;}
		public byte[] Request_Data 		{ get; set;}
		public string Response_Headers  { get; set;}		
		public string Response_Code  	{ get; set;}
		public byte[] Response_Data_Raw { get; set;}
		
		public override string ToString()
		{
			return "{0}     - {1}".format(this.Request_Url.str(), this.responseHtml().size().kBytesStr());
		}
	}
	
	public class API_HttpRequests
	{
			
	}
	
	public static class Api_WebRequests_ExtensionMethods
	{
		public static string responseHtml(this HttpRequest httpRequest)
		{
			return httpRequest.Response_Data_Raw.gzip_Decompress_toString();
		}
		
		public static HttpRequest http_GET(this Uri uri)
		{
			var httpRequest = new HttpRequest();
			httpRequest.Request_Url = uri.str();
			var html = uri.getHtml();			
			httpRequest.Response_Data_Raw = html.gzip_Compress();
			return httpRequest;
		}
		
		public static string save_to_Folder(this HttpRequest httpRequest, string targetFolder)
		{
			if (httpRequest.Request_Url.notValid())
			{
				"[HttpRequest] in save_to_Folder, httpRequest.Url was not valid".error();
				return null;
			}
			var targetFile = targetFolder.pathCombine(httpRequest.Request_Url.safeFileName() + ".xml");
			httpRequest.saveAs(targetFile);
			return targetFile;
		}
	}
}