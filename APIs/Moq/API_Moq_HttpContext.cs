// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Web;
using System.Security.Principal;
using System.Collections.Specialized;
using FluentSharp.CoreLib;
using Moq;


//O2Ref:Moq.dll
//O2Ref:System.Web.Abstractions.dll

namespace O2.XRules.Database.APIs
{
    public class API_Moq_HttpContext 
    {        	
		public Mock<HttpContextBase> MockContext { get; set; }
		public Mock<HttpRequestBase> MockRequest { get; set; }
		public Mock<HttpResponseBase> MockResponse { get; set; }
		public Mock<HttpSessionStateBase> MockSession { get; set; }
		public Mock<HttpServerUtilityBase> MockServer { get; set; }		
		
        public HttpContextBase HttpContextBase  	{ get; set; }
        public HttpRequestBase HttpRequestBase  	{ get; set; }
        public HttpResponseBase HttpResponseBase  	{ get; set; }  
        
        public String BaseDir						{ get;set; }
	    
		public API_Moq_HttpContext() : this(null)
		{			
		}
		
		public API_Moq_HttpContext(string baseDir)
		{
			BaseDir = baseDir;
			createBaseMocks();
			setupNormalRequestValues();
		}
		
    	public API_Moq_HttpContext createBaseMocks()
    	{
    		MockContext = new Mock<HttpContextBase>();
	        MockRequest = new Mock<HttpRequestBase>();
	        MockResponse = new Mock<HttpResponseBase>();
	        MockSession = new Mock<HttpSessionStateBase>();
	        MockServer = new Mock<HttpServerUtilityBase>();
	        
	
	        MockContext.Setup(ctx => ctx.Request).Returns(MockRequest.Object);
	        MockContext.Setup(ctx => ctx.Response).Returns(MockResponse.Object);
	        MockContext.Setup(ctx => ctx.Session).Returns(MockSession.Object);
	        MockContext.Setup(ctx => ctx.Server).Returns(MockServer.Object);
	        
	     	
	     	HttpContextBase = MockContext.Object; 
	     	HttpRequestBase = MockRequest.Object;
	     	HttpResponseBase = MockResponse.Object;
	     		     	
	     	return this;
		}
		
		public Func<string,string> context_Server_MapPath {get;set;} 
		
		public API_Moq_HttpContext setupNormalRequestValues()		
		{							        
	        var genericIdentity = new GenericIdentity("genericIdentity");
	        var genericPrincipal = new GenericPrincipal(genericIdentity, new string[] {});
			MockContext.Setup(context => context.User).Returns(genericPrincipal);	     	
	     	MockContext.Setup(context => context.Cache).Returns(HttpRuntime.Cache);
			MockContext.Setup(context => context.Server.MapPath(It.IsAny<string>())).Returns((string path) =>  this.BaseDir.pathCombine(path));
			
	     	//Request
	     	MockRequest.Setup(request =>request.InputStream	).Returns(new MemoryStream()); 
	     	MockRequest.Setup(request =>request.Headers		).Returns(new NameValueCollection()); 
	     	MockRequest.Setup(request =>request.QueryString	).Returns(new NameValueCollection()); 
	     	MockRequest.Setup(request =>request.Form		).Returns(new NameValueCollection()); 
	     	
	     	//Response
	     	var outputStream = new MemoryStream();
	     	MockResponse.Setup(response =>response.OutputStream).Returns(outputStream); 
	     	
	     	//var writer = new StringWriter();
//			context.Expect(ctx => ctx.Response.Output).Returns(writer);
	     	
	     	MockResponse.Setup(response =>response.Write(It.IsAny<string>())).Callback((string code) => outputStream.Write(code.asciiBytes(), 0, code.size()));
	     	var cache = new Mock<HttpCachePolicyBase>();
            MockResponse.SetupGet(response => response.Cache).Returns(cache.Object);
	     	return this;
		}
	}
	
	public static class API_Moq_HttpContext_ExtensionMethods
	{
		public static HttpContextBase httpContext(this API_Moq_HttpContext moqHttpContext)
		{
			return moqHttpContext.HttpContextBase;
		}
		
		public static HttpContextBase request_Write_Clear(this API_Moq_HttpContext moqHttpContext)
		{
			moqHttpContext.MockRequest.Setup(request =>request.InputStream).Returns(new MemoryStream()); 
			
			return moqHttpContext.httpContext();
		}
		
		public static HttpContextBase request_Write(this HttpContextBase httpContextBase,string text)
		{														
			httpContextBase.stream_Write(httpContextBase.Request.InputStream, text);			
			return httpContextBase;
		}
				
		public static string request_Read(this HttpContextBase httpContextBase)
		{					
			return httpContextBase.stream_Read(httpContextBase.Request.InputStream);
		}
		
		public static HttpContextBase response_Write(this HttpContextBase httpContextBase,string text)
		{														
			httpContextBase.stream_Write(httpContextBase.Response.OutputStream, text);			
			return httpContextBase;
		}
		
		public static string response_Read(this HttpContextBase httpContextBase)
		{					
			return httpContextBase.stream_Read(httpContextBase.Response.OutputStream);						
		}
		
		public static string response_Read_All(this HttpContextBase httpContextBase)
		{
			httpContextBase.Response.OutputStream.Flush();
			httpContextBase.Response.OutputStream.Position = 0;
			return httpContextBase.response_Read();
		}
		
		public static HttpContextBase stream_Write(this HttpContextBase httpContextBase, Stream inputStream, string text)
		{														
			var streamWriter = new StreamWriter(inputStream);
			
			inputStream.Position = inputStream.property("Length").str().toInt();  
			//inputStream.Position = (int)inputStream.Length; // the line above can also be this
			streamWriter.Write(text);    
			streamWriter.Flush(); 			
			inputStream.Position = 0; 			
			
			return httpContextBase;
		}
		
		public static string stream_Read(this HttpContextBase httpContextBase, Stream inputStream)
		{								
			var originalPosition = inputStream.Position;
			var streamReader = new StreamReader(inputStream);
			var requestData = streamReader.ReadToEnd();	
			inputStream.Position = originalPosition;
			return requestData;
		}
	}
}        
