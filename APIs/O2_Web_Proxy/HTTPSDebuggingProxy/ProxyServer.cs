//based on the code from http://www.codeproject.com/KB/IP/HTTPSDebuggingProxy.aspx
//originally coded by @matt_mcknight  

// see O2_Web_Proxy.cs API for a way to consume this Proxy from O2
 
//O2File:ProxyCache.cs 

//O2Ref:System.Configuration.dll 
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.APIs;
using O2.XRules.Database.Utils;

//O2File:HttpData.cs
//O2File:ProxyCache.cs

namespace HTTPProxyServer
{       
	
    public sealed class ProxyServer
    {
    	//O2 specific
    	public static Func<HttpWebRequest,string, bool> HandleWebRequestProxyCommands;
        public static Func<string, string> InterceptRemoteUrl;
        public static Func<Uri, bool> InterceptResponseHtml;    
        public static Func<Uri,string, string> HtmlContentReplace;
        public static Action<HttpWebRequest> InterceptWebRequest;
        public static Action<RequestResponseData> OnResponseReceived;
        
        public static bool ExtraLogging { get; set; }
        
        public bool CaptureRequests { get; set; }
        public bool ProxyStarted { get; set; }
        public bool ProxyDisabled { get; set; }
        
        
        public static ProxyCache proxyCache;

		//Original
        private static ProxyServer _server = new ProxyServer();

        private static readonly int BUFFER_SIZE = 8192;
        private static readonly char[] semiSplit = new char[] { ';' };
        private static readonly char[] equalSplit = new char[] { '=' };
        private static readonly String[] colonSpaceSplit = new string[] { ": " };
        private static readonly char[] spaceSplit = new char[] { ' ' };
        private static readonly char[] commaSplit = new char[] { ',' };
        private static readonly Regex cookieSplitRegEx = new Regex(@",(?! )");
        private static X509Certificate2 _certificate;
        private static object _outputLockObj = new object();


        private TcpListener _listener;
        private Thread _listenerThread;
//        private Thread _cacheMaintenanceThread;
                
        public IPAddress ListeningIPInterface 	{ get ; set; } //DC                
        public Int32 ListeningPort 				{ get ; set; } //DC        

        private ProxyServer()
        {        	
        	setDefaultValues();
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }
		
		
//        public Boolean DumpHeaders { get; set; }
//        public Boolean DumpPostData { get; set; }
//        public Boolean DumpResponseData { get; set; }

        public static ProxyServer Server
        {
            get { return _server; }
        }
		
		public void setDefaultValues()
		{
			proxyCache = new ProxyCache();
			CaptureRequests = true;
			this.clearPastRequests();
			IPAddress ipAddress = IPAddress.Loopback;
			IPAddress.TryParse("127.0.0.1", out ipAddress);
			ListeningIPInterface = ipAddress;
			ListeningPort = 8081;
		}		        

        public bool Start(string certFilePath)
        {             	        	
            "..Starting Proxy Server using Certificate: {0}".info(certFilePath);
            try
            {                
                try 
                {
                    _certificate = new X509Certificate2(certFilePath);
                }
                catch (Exception ex)   
                {
                	"[ProxyServer] Start: Could not create the certificate from file from {0}. Exception: ".error(certFilePath, ex.Message);
                	return false;                    
                }
                
                "[ProxyServer] Starting listener on IP {0} and Port {1}".format(ListeningIPInterface, ListeningPort);
                _listener = new TcpListener(ListeningIPInterface, ListeningPort);                
                _listener.Start();
            }
            catch (Exception ex)
            {
            	"[ProxyServer] Start: {0}".error(ex.Message);                 
                return false;
            }
			"[ProxyServer] listener started".info();
            _listenerThread = new Thread(new ParameterizedThreadStart(Listen));
            _listenerThread.Start(_listener);
			this.ProxyStarted = true;
            return true;
        }

        public void Stop()
        {
            "Stopping Proxy Server".info();
            this.ProxyStarted = false;
            _listener.Stop();

            //wait for server to finish processing current connections...

            _listenerThread.Abort();
            _listenerThread.Join();
            _listenerThread.Join();
        }

        private void Listen(Object obj)
        {
            TcpListener listener = (TcpListener)obj;
            try
            {
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    while (!ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessClient), client)) ;
                }
            }
            catch (ThreadAbortException) { }
            catch (SocketException) { }
        }


        private void ProcessClient(Object obj)
        {
        	O2Thread.mtaThread(
        		()=>{
			            TcpClient client = (TcpClient)obj;
			            try
			            {
			                DoHttpProcessing(client);
			            }
			            catch (Exception ex)
			            {
			                Console.WriteLine(ex.Message);
			                "[ProcessClient]".error(ex.Message);
			            }
			            finally
			            {
			                client.Close();
			            }
					});
        }			

        private void DoHttpProcessing(TcpClient client)
        {      
        	var threadId = Thread.CurrentThread.ManagedThreadId;
        	if (ProxyDisabled)
        	{
        		"[DoHttpProcessing]: ProxyDisabled is set, aborting request".error();
        		return;
        	}
        	
            Stream clientStream = client.GetStream();
            Stream outStream = clientStream; //use this stream for writing out - may change if we use ssl
            SslStream sslStream = null;
            StreamReader clientStreamReader = new StreamReader(clientStream);
            
            MemoryStream cacheStream = null;			
            
            try
            {
            	HttpWebRequest webReq = null;
                HttpWebResponse response = null;
                var rawRequestHeaders = new StringBuilder();
                var rawResponseHeaders = new StringBuilder();
                byte[] requestPostBytes = null;                                								                              
                var contentLen = 0;
                var skipRemaingSteps = false; 
                
                //read the first line HTTP command
                String httpCmd = clientStreamReader.ReadLine();
                if (String.IsNullOrEmpty(httpCmd))
                {
                    clientStreamReader.Close();
                    clientStream.Close();
                    return;
                }
                //break up the line into three components
                String[] splitBuffer = httpCmd.Split(spaceSplit, 3);

                String method = splitBuffer[0];
                String remoteUri = splitBuffer[1];
                Version version = new Version(1, 0);

                ExtraLogging.ifDebug("[{2}][DoHttpProcessing]: Got request for: {0} {1} [{2}]", method, remoteUri, threadId);
                
                Action handleSLL_CONNECT_withCaller = 
                	()=>{
                			if (skipRemaingSteps)
								return;
																						
			                if (splitBuffer[0].ToUpper() == "CONNECT")
			                {
			                	ExtraLogging.ifInfo("[{1}][DoHttpProcessing][handleSLL_CONNECT_withCaller] {0} [{1}]", remoteUri, threadId);
			                    //Browser wants to create a secure tunnel
			                    //instead = we are going to perform a man in the middle "attack"
			                    //the user's browser should warn them of the certification errors however.
			                    //Please note: THIS IS ONLY FOR TESTING PURPOSES - you are responsible for the use of this code
			                    remoteUri = "https://" + splitBuffer[1];
			                    
			                    ExtraLogging.ifInfo("[{1}][DoHttpProcessing][handleSLL_CONNECT_withCaller] updated remoteUri {0}, [{1}]", remoteUri, threadId);
			                    
			                    while (!String.IsNullOrEmpty(clientStreamReader.ReadLine())) ;
			                    
			                    ExtraLogging.ifInfo("[{0}][DoHttpProcessing][handleSLL_CONNECT_withCaller] after clientStreamReader.ReadLine [{0}]", threadId);
			                    
			                    StreamWriter connectStreamWriter = new StreamWriter(clientStream);
			                    connectStreamWriter.WriteLine("HTTP/1.0 200 Connection established");
			                    connectStreamWriter.WriteLine(String.Format("Timestamp: {0}", DateTime.Now.ToString()));
			                    connectStreamWriter.WriteLine("Proxy-agent: O2 Platform Web Proxy");
			                    connectStreamWriter.WriteLine();
			                    			                    
			                    connectStreamWriter.Flush();
			        //            ExtraLogging.ifDebug("[{0}][DoHttpProcessing][handleSLL_CONNECT_withCaller] afterFlush [{0}]", threadId);
			                    //connectStreamWriter.Close();   //see if it has side effects with ssl sites
			                    //ExtraLogging.ifDebug("[DoHttpProcessing][handleSLL_CONNECT_withCaller] afterClose");
/*			                }
						};

				Action handleSLL_CONNECT_withRemote = 
                	()=>{
                	
                			if (skipRemaingSteps)
								return;							
*/							
//			                if (splitBuffer[0].ToUpper() == "CONNECT")
//			                {			                			
			                	//ExtraLogging.ifInfo("[DoHttpProcessing][handleSLL_CONNECT_withRemote] {0}", remoteUri);								
			                    
			                    try
			                    {
			                    	var keepStreamOpen = true;
			                    	var checkCertificateRevocation = true;
			                    	ExtraLogging.ifInfo("[{1}][DoHttpProcessing][handleSLL_CONNECT_withCaller] creating sslStream and AuthenticateAsServer {0} [{1}]", remoteUri, threadId);
			                    	//sslStream = new SslStream(clientStream, false);		//original
			                    	sslStream = new SslStream(clientStream, keepStreamOpen);
			                        //sslStream.AuthenticateAsServer(_certificate, false, SslProtocols.Tls | SslProtocols.Ssl3 | SslProtocols.Ssl2, true);   //original
			                        //sslStream.AuthenticateAsServer(_certificate, false, SslProtocols.Tls | SslProtocols.Ssl3 | SslProtocols.Ssl2, false);			                        
			                        sslStream.AuthenticateAsServer(_certificate, false, SslProtocols.Tls | SslProtocols.Ssl3 | SslProtocols.Ssl2, checkCertificateRevocation);			                        			                        
			                    }
			                    catch (Exception ex)
			                    {
			                    	skipRemaingSteps = true;			                        
			                    	"[{1}][Proxy Server][handleSLL_CONNECT] in sslStream.AuthenticateAsServer: {0} [{1}]".error(ex.Message, threadId);			                    		
			                    	if (ex.Message == "Authentication failed because the remote party has closed the transport stream.")
			                    		"[Proxy Server][handleSLL_CONNECT] NOTE: this error is usually callsed by the browser not accepting the temp certificate".info();
			                    	else
			                    		"[Proxy Server][handleSLL_CONNECT] NOTE: this error is usually caused by running with UAC, start the script in non UAC".info();  //.lineBefore().lineBeforeAndAfter()
			                        try
			                        {
			                        	sslStream.Close();
			                        	clientStreamReader.Close();
			                        	connectStreamWriter.Close(); // check for side effect
			                        	clientStream.Close();			                    	
			                        }
			                        catch(Exception ex2)
			                        {
			                        	"[{1}][Proxy Server][handleSLL_CONNECT] in CATCH of sslStream.AuthenticateAsServer: {0} [{1}]".error(ex2.Message, threadId);
			                        }
			                    }			                    
			                    if (skipRemaingSteps)
			                    	return;
			
			                    //HTTPS server created - we can now decrypt the client's traffic
			                    clientStream = sslStream;
			                    clientStreamReader = new StreamReader(sslStream);
			                    outStream = sslStream;
			                    //read the new http command.
			                    try
			                    {
			                    	httpCmd = clientStreamReader.ReadLine();
			                    }
			                    catch(Exception ex)
			                    {
			                    	"[{1}][Proxy Server] in clientStreamReader.ReadLine() {0} [{1}]".error(ex.Message, threadId);
			                    	httpCmd = null;
			                    }
			                    if (String.IsNullOrEmpty(httpCmd))
			                    {
			                        clientStreamReader.Close();
			                        clientStream.Close();
			                        sslStream.Close();
			                        return;
			                    }
			                    splitBuffer = httpCmd.Split(spaceSplit, 3);
			                    method = splitBuffer[0];
			                    remoteUri = remoteUri + splitBuffer[1];
			                }
		                };
				
				Action createWebRequest =
					()=>{	
							if (skipRemaingSteps)
								return;
							ExtraLogging.ifInfo("[{1}][DoHttpProcessing][createWebRequest] {0} [{1}]", remoteUri, threadId);
							//construct the web request that we are going to issue on behalf of the client.
							remoteUri = remoteUri.uri().str();	// fixes some issues where the uri is missing the initial protocol
			                webReq = (HttpWebRequest)HttpWebRequest.Create(remoteUri);
			                webReq.Method = method;
			                webReq.ProtocolVersion = version;
			
			                //read the request headers from the client and copy them to our request
			                contentLen = ReadRequestHeaders(clientStreamReader, webReq, rawRequestHeaders);
			                
			                webReq.Proxy = null;
			                webReq.KeepAlive = false;
			                webReq.AllowAutoRedirect = false;
			                webReq.AutomaticDecompression = DecompressionMethods.None;
						};
						                
				Action capturePostData = 
					()=>{
							if (skipRemaingSteps)
								return;							
			                if (method.ToUpper() == "POST")
			                {			        			                	
			                	ExtraLogging.ifInfo("[{1}][DoHttpProcessing][capturePostData] {0} [{1}]", remoteUri, threadId);
			                	var bytesToRead = contentLen;
			                	var maxReadBlock = 2048;		// try 100 to see better what is going on			                	
			                				                    
			                    int bytesRead;
			                    //int totalBytesRead = 0;			                    
			                    var readString = new StringBuilder();
			                    var postData = new StreamWriter(new MemoryStream());
			                    //DCz I had to change how the original was doing since ReadBlock would free on large posts
			                    //while (totalBytesRead < contentLen && (bytesRead = clientStreamReader.ReadBlock(postBuffer, 0, contentLen)) > 0)
			                    do 
			                    {
			                    	var readThisBytes = (bytesToRead > maxReadBlock) 
						                					? maxReadBlock
						                					: bytesToRead;
			                    	char[] postBuffer = new char[readThisBytes];			                    	
			                    	bytesRead = clientStreamReader.Read(postBuffer, 0, readThisBytes);
			                    	//Weirdly the conversion into string gives me a more accurate length than bytes read
			                    	var snipptet = Encoding.UTF8.GetBytes(postBuffer).ascii();
			                    	readString.Append(snipptet);			                    	
			                        //totalBytesRead += snipptet.size(); 			                        
			                        postData.Write(postBuffer, 0, bytesRead);			                        			                        
			                        bytesToRead -= snipptet.size(); //depending on the chars this will change
			                        
			                        /*"bytes read:{0} of {1} (from {2}  still left {3})  {4}  readString: {4}".info(
			                    				bytesRead, readThisBytes, contentLen ,bytesToRead, 
			                    				snipptet, readString.size());*/
			                    }        
			                    while(bytesToRead >0 || bytesRead==0);			                    			                    
			                    postData.Flush();			                    
			                    requestPostBytes = (postData.BaseStream as MemoryStream).ToArray();				                    
			                } 
						};
				
				
				Action writePostDataToRequestStream =
					()=>{
							if (skipRemaingSteps)
								return;							
							if (method.ToUpper() == "POST")
			                {
			                	ExtraLogging.ifInfo("[{1}][DoHttpProcessing][writePostDataToRequestStream] {0} [{1}]", remoteUri, threadId);
			                	 var requestPostChars = Encoding.UTF8.GetChars(requestPostBytes);
								StreamWriter sw = new StreamWriter(webReq.GetRequestStream());
								sw.Write(requestPostChars.ToArray(), 0, requestPostChars.Length);
								sw.Flush();
								sw.Close();
							}
						};

				
				Action getResponse = 
					()=>{
							if (skipRemaingSteps)
								return;
							ExtraLogging.ifInfo("[{1}][DoHttpProcessing][getResponse] {0} [{1}]", remoteUri, threadId);
							
							webReq.Timeout = 15000;
		                    try
		                    {
		                        response = (HttpWebResponse)webReq.GetResponse();
		                    }
		                    catch (WebException webEx)
		                    {
		                        response = webEx.Response as HttpWebResponse;
		                        "[{1}][DoHttpProcessing][getResponse] {0} [{1}]: {2}".error( remoteUri, threadId, webEx.Message);		                        
		                        //webEx.log();
		                    }		                    
						};
				
				 
				Action handleResponse_viaCache = 
					()=>{
							if (skipRemaingSteps)
								return;
							ExtraLogging.ifInfo("[{1}][DoHttpProcessing][handleResponse_viaCache] {0} [{1}]", remoteUri, threadId);
							if (proxyCache.enabled())
	                        {	
	                        	ExtraLogging.ifInfo("[{1}][DoHttpProcessing][handleResponse_viaCache] Replying using Cache: {0} [{1}]", remoteUri, threadId);
	                        	//Stream responseStream = response.GetResponseStream();
	                        	var cacheObject = proxyCache.getMapping(webReq, requestPostBytes);
		                        if (cacheObject.notNull())
		                        {		                        			                
		                        	StreamWriter myResponseWriter = new StreamWriter(outStream);
		                        	WriteResponseStatus(cacheObject.WebResponse.StatusCode,
		                        					    cacheObject.WebResponse.StatusDescription, 
		                        					    myResponseWriter);
		                        					    
		                            WriteResponseHeaders(myResponseWriter, cacheObject.Response_Headers);		                            
		                        	outStream.Write(cacheObject.Response_Bytes, 0, cacheObject.Response_Bytes.Length);		                        			                        	
			                        //responseStream.Close();
			                        outStream.Flush();
			                        
			                        //responseStream.Close();
			                        //response.Close();
			                        myResponseWriter.Close();		                            		                        
			                        skipRemaingSteps = true;
			                    }
		                   	}							
						};
						
				Action handleResponse_viaRealTarget =  
					()=>{	
							if (skipRemaingSteps)
								return;
								
							ExtraLogging.ifInfo("[{1}][DoHttpProcessing][handleResponse_viaRealTarget]: {0} [{1}]", remoteUri, threadId);
							if (response == null)		                				                	
		                		"[{1}][ProxyServer][DoHttpProcessing][handleResponse_viaRealTarget] Response was null {0} [{1}]".error(remoteUri, threadId);
		                	
		                	StreamWriter myResponseWriter;
		                	Stream responseStream;
		                	List<Tuple<String,String>> responseHeaders = null;
		                	var memoryStream = new MemoryStream();
							var binaryWriter = new  BinaryWriter(memoryStream);
		                	
		                	Action addResponseToCache =
		                		()=>{
				                		var responseString = response.isNull()
				                								? ""
				                								:	(response.ContentEncoding == "gzip")
				                            							? memoryStream.ToArray().gzip_Decompress().ascii()
				                            							: memoryStream.ToArray().ascii();                            							                            	
		                            	                            	
		                            	var requestResponseData = proxyCache.add(webReq, response, requestPostBytes,  memoryStream.ToArray(), responseString);
		                            	
		                            	requestResponseData.Request_Headers_Raw = rawRequestHeaders.str();                            	
		                            	requestResponseData.Response_Headers_Raw = rawResponseHeaders.str();                            	
		                            	requestResponseData.Response_Headers = responseHeaders;                            	                            	
		                            	
			                            if (OnResponseReceived.notNull()) 	                            
			                            	OnResponseReceived(requestResponseData);//webReq, response,memoryStream.ToArray().ascii());//UTF8Encoding.UTF8.GetString(memoryStream.ToArray(), 0, (int)memoryStream.Length));
				                	};
		                	
		                	try
		                	{
		                		if (response.isNull() || response.StatusCode.str() == "BadRequest")
		                		{
		                			ExtraLogging.ifInfo("[{0}][ProxyServer][handleResponse] skipping due to response being null or response.StatusCode == BadRequest ", threadId);
		                			"[{0}][ProxyServer][handleResponse] skipping due to response being null or response.StatusCode == BadRequest ".error(threadId);
		                			addResponseToCache();
							    	return; 
		                		}
	                        	myResponseWriter = new StreamWriter(outStream);
	                        	responseStream = response.GetResponseStream();	                        	                        	                        		                   	
								responseHeaders = ProcessResponse(response, rawResponseHeaders);								
							}
							catch(Exception ex)
							{
							    "[{2}][ProxyServer][handleResponse] before processing response body:  {0}  {1} [{2}]".error(ex.Message,  remoteUri, threadId);
							    addResponseToCache();
							    return; 
							}
							
	                        try
	                        {	                        	
	                            //send the response status and response headers
	                            WriteResponseStatus(response.StatusCode,response.StatusDescription, myResponseWriter);
	                            WriteResponseHeaders(myResponseWriter, responseHeaders);	                            								
								
	                            Byte[] buffer;
	                            if (response.ContentLength > 0)
	                                buffer = new Byte[response.ContentLength];
	                            else
	                                buffer = new Byte[BUFFER_SIZE];
	
	                            int bytesRead;
	                            
								
																
	                            while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)	                            
	                            	binaryWriter.Write(buffer, 0, bytesRead);
	                            	
	                            binaryWriter.Flush();
	                            
	                            if (memoryStream.Length >  Int32.MaxValue)
	                            	"[ProxyServer][handleResponse]: memoryStream.Length >  Int32.MaxValue".error();
	                            
	                            try
	                            {
	                            	outStream.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
	                            }
	                            catch(Exception ex)
	                            {
	                            	"[{2}][ProxyServer][handleResponse] in outStream.Write: {0}  {1}".error(ex.Message,  remoteUri, threadId);	                            
	                            }
	                            
	                            addResponseToCache();
	                            		
	                            responseStream.Close();	                            	                            
	                            outStream.Flush();
	                        }
	                        catch (Exception ex)
	                        {	                            
	                            "[{2}][ProxyServer][handleResponse]  while processing response body: {0}  {1} [{2}]".error(ex.logStackTrace().Message,  remoteUri, threadId);	                            
	                        }
	                        finally
	                        {
	                            responseStream.Close();
	                            response.Close();
	                            myResponseWriter.Close();
	                        }		                    
						};																
				
				
				handleSLL_CONNECT_withCaller();
				//handleSLL_CONNECT_withRemote();								
				// O2 callback				
                if (InterceptRemoteUrl.notNull())						                                	
                    remoteUri = InterceptRemoteUrl(remoteUri);				
												
				createWebRequest();	
				capturePostData();
				
				//put capturePostDatainterceptor callback here				
				handleResponse_viaCache();												
				
				if (HandleWebRequestProxyCommands.notNull() && HandleWebRequestProxyCommands(webReq,remoteUri) == false )			// O2 callback
					skipRemaingSteps = true;				     

//				handleSLL_CONNECT_withRemote();

                if (InterceptWebRequest.notNull())
                    InterceptWebRequest(webReq);                                 
				
				writePostDataToRequestStream();
		
				getResponse();
							
				handleResponse_viaRealTarget();		                
				
				if (skipRemaingSteps)
					ExtraLogging.ifError("[{1}][DoHttpProcessing] skipRemaingSteps was set for: {0} [{1}]", remoteUri, threadId);
					
				ExtraLogging.ifDebug("[{1}][DoHttpProcessing] ended for: {0} [{1}]", remoteUri, threadId);
				
                
            }
            catch (Exception ex)
            {                
                "[ProxyServer][DoHttpProcessing]: {0}".error(ex.logStackTrace().Message);
            }
            finally
            {
                /*if (Server.DumpHeaders || Server.DumpPostData || Server.DumpResponseData)
                {
                    //release the lock
                    Monitor.Exit(_outputLockObj);
                }*/

                clientStreamReader.Close();
                clientStream.Close();
                if (sslStream != null)
                    sslStream.Close();
                outStream.Close();
                if (cacheStream != null)
                    cacheStream.Close();
            }            
        }

        private List<Tuple<String,String>> ProcessResponse(HttpWebResponse response, StringBuilder rawResponseHeaders)
        {        	
            String value=null;
            String header=null;
            List<Tuple<String, String>> returnHeaders = new List<Tuple<String, String>>();
            foreach (String s in response.Headers.Keys)
            {
            	rawResponseHeaders.AppendLine("{0}: {1}".format(s, response.Headers[s]));
            	
                if (s.ToLower() == "set-cookie")				//DCz: it looks like this will not cause multiple set-cookies to fail
                {
                    header = s;
                    value = response.Headers[s];
                }
                else
                    returnHeaders.Add(new Tuple<String, String>(s, response.Headers[s]));
            }
            
            //if (!String.IsNullOrWhiteSpace(value))
            if (!String.IsNullOrEmpty(value))
            {
                response.Headers.Remove(header);
                String[] cookies = cookieSplitRegEx.Split(value);
                foreach (String cookie in cookies)
                    returnHeaders.Add(new Tuple<String, String>("Set-Cookie", cookie));

            }
            returnHeaders.Add(new Tuple<String, String>("X-Proxied-By", "O2-Platform-Proxy")); //"matt-dot-net proxy"));                        
            
            return returnHeaders;
        }

        private void WriteResponseStatus(HttpStatusCode code, String description, StreamWriter myResponseWriter)
        {
        	try
        	{
            	String s = String.Format("HTTP/1.0 {0} {1}", (Int32)code, description);
            	myResponseWriter.WriteLine(s);                        
            }
			catch(Exception ex)
			{
				"[ProxyCache][WriteResponseStatus]: {0}".error(ex.Message);
			}
        }

        private void WriteResponseHeaders(StreamWriter myResponseWriter, List<Tuple<String,String>> headers)
        {
        	try
        	{
	            if (headers != null)
	            {
	                foreach (Tuple<String,String> header in headers)
	                    myResponseWriter.WriteLine(String.Format("{0}: {1}", header.Item1,header.Item2));
	            }
	            myResponseWriter.WriteLine();
	            myResponseWriter.Flush();
			}
			catch(Exception ex)
			{
				"[{1}][ProxyCache][WriteResponseHeaders]: {0}".error(ex.Message, Thread.CurrentThread.ManagedThreadId);
			}
        }
        

        private int ReadRequestHeaders(StreamReader sr, HttpWebRequest webReq, StringBuilder rawRequestHeaders)
        {               	
        	String httpCmd;
	        int contentLen = 0;
	        try
        	{
        		sr.Peek();
        	}
        	catch//(Exception ex)		// means the stream is closed (probably due to an response with "(400) Bad Request" 
        	{
        		return contentLen;
        	}
        	try
        	{	            
	            do
	            {	            	
	            	httpCmd = sr.ReadLine();
	                if (String.IsNullOrEmpty(httpCmd))
	                    return contentLen;
	                rawRequestHeaders.AppendLine(httpCmd);
	                String[] header = httpCmd.Split(colonSpaceSplit, 2, StringSplitOptions.None);
	            	try
	            	{		                
		                switch (header[0].ToLower())
		                {
		                    case "host":
		                        //webReq.Host = header[1];
		                        break;
		                    case "user-agent":
		                        webReq.UserAgent = header[1];
		                        break;
		                    case "accept":
		                        webReq.Accept = header[1];
		                        break; 
		                    case "referer":
		                        webReq.Referer = header[1];
		                        break;
		                    case "cookie":
		                        webReq.Headers["Cookie"] = header[1];
		                        break;
		                    case "proxy-connection":
		                    case "connection":
		                    case "keep-alive":
		                        //ignore these
		                        break;
		                    case "content-length":
		                        int.TryParse(header[1], out contentLen);
		                        break;
		                    case "content-type":
		                        webReq.ContentType = header[1];
		                        break;
		                    case "if-modified-since":
		                        String[] sb = header[1].Trim().Split(semiSplit);
		                        DateTime d;
		                        if (DateTime.TryParse(sb[0], out d))
		                            webReq.IfModifiedSince = d;
		                        break;
		                    case "expect":						//DCz this was throwing an error (see http://haacked.com/archive/2004/05/15/http-web-request-expect-100-continue.aspx if a better is  solution is needed)
		                        //webReq.Expect = header[1];
		                        break;    
		                    default:
		                            webReq.Headers.Add(header[0], header[1]);
		                        break;	
						}
	                }					
                    catch (Exception ex)
                    {
                        "[{3}][ProxyCache][ReadRequestHeaders][do loop]: Could not add header {0} = {1}.  Exception message:{2}".error(header[0], header[1], ex.Message,Thread.CurrentThread.ManagedThreadId);
                    }

	            } while (!String.IsNullOrEmpty(httpCmd)); //(!String.IsNullOrWhiteSpace(httpCmd));	        
	        }
			catch(Exception ex)
			{
				"[{1}][ProxyCache][ReadRequestHeaders]: {0}".error(ex.logStackTrace().Message, Thread.CurrentThread.ManagedThreadId);
			}
            return contentLen;
        }
    }         
    
    
    
    public static class ProxyServer_ExtensionMethods_Misc
    {
    	public static ProxyServer clearPastRequests(this ProxyServer proxyServer)
    	{
    		if (ProxyCache.Requests.notNull())
    		{
    			"[ProxyServer] clearing PastRequests object".info();
    			ProxyCache.Requests.Clear();
    		}
    		else    		
	    		ProxyCache.Requests = new List<RequestResponseData>();
	    	return proxyServer;
    	}
    	
    	public static List<RequestResponseData> requests(this ProxyServer proxyServer)
    	{
    		return ProxyCache.Requests;
    	}
    }
    
}
