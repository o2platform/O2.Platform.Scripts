// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.Services;
using System.Data.SqlClient; 
//O2Ref:System.Web.Services.dll
   
namespace O2.XRules.Database._Rules._Sample_Vulnerabilities
{
	public class LiveDemo : Page
	{
		
		public void taintometer()
		{
			webMethod_Start(Request["Form"]);			
		}
		
		[WebMethod]
		public void webMethod_Start(string when)
		{			
			var safeText = Server.HtmlEncode(when);
			var a = when + "aaaa";
			Process.Start(when);
			Process.Start(when);
			startEngine(a);
		}
				
		public void startNow()
		{
			startEngine("now");
		}
		
		public void startIn1Hour()
		{
			startEngine("1hour");
		}
		
		public void startEngine(string time)
		{						
			SqlConnection sqlConnection =null;
			var sql = "Select * from engine when time=" + time;
			var sqlCommand = new SqlCommand(sql,sqlConnection);
		}
		
	}



    public class HelloPage : Page
    {

		public void hello()
    	{
    		//helloAgain(Request["name"]);
    	}
		  	
    	public void helloAgain(string name)
    	{    		
    		string pageName= "ll";//Request["name"];
    		var userMessage = "Hello there, you are in page " + pageName + name;
    		Response.Write(userMessage);
    	}
    	
    	[WebMethod]
    	public void multipeActions(string name)
    	{       		
    		new SqlCommand(name);
    		Response.Write(name);
    		File.OpenRead(name);
    		Process.Start(name);
    		Response.Redirect(name);
    		
    	}
	}



    public class Web_Sql_Injection : Page
    {                   
        
        public void startEngine_When()
        {
        	startEngine_When("");
        }
        
        [WebMethod]
        public void startEngine_When(string when)
        {        	
        	startEngine(when);
        }
        
        public void startEngine_Tomorrow()
		{
			startEngine("-tomorrow");
		}
        
		public void startEngine_Now()
		{
			startEngine("-now");
		}
		
		public void startEngine_in1Hour()
		{
			startEngine("-1hour");
		}
		
    	private void startEngine(string arguments)
    	{    		
    		sink_SqlDataExecute(arguments);
    	}
    	
    	
    	public SqlConnection sqlConnection { get; set; }
    	
        public void controller()
        {
            var taintedData = source_HttpRequest();
            sink_SqlDataExecute(taintedData); 
            sink_SqlDataExecute2(taintedData);
            sink_ResponseWrite(taintedData);
            createResponse(taintedData);   
        }
		
        public string source_HttpRequest()
        { 
            var taitedData = Request["payload"];
            var anotherVar = "test" + taitedData;
            return taitedData;
        }
		
        public void sink_SqlDataExecute(string whereStatement)
        {
        	if (Regex.IsMatch(whereStatement,@"^\d{3}-\d{2}-\d{4}$"))        	
        	{
            	var sqlText = "Select name from users where customer = " + whereStatement;            
            	var command = new SqlCommand(sqlText, sqlConnection);		            	
            	command.ExecuteNonQuery();
            }
        }    	    	    	    	    
        
        public void sink_SqlDataExecute2(string whereStatement)
        {        	
        	//note how the scanning happens in real time
        	//Once we fixed the problem it doesn't show up
            var sqlText = "Select name from accounts where account = @whereStatement";// + whereStatement;			
            var command = new SqlCommand(sqlText, sqlConnection);		
            command.Parameters.Add("@whereStatement",whereStatement);
            command.ExecuteNonQuery();
        }    	  
        
        public void createResponse(string data)
        {
        	sink_ResponseWrite(data);
        }
        
        public void sink_ResponseWrite(string whereStatement)
        {                	
        	//Response.Write(AntiXss.HtmlEncode(whereStatement));
        	//Response.Write(Server.HtmlDecode(whereStatement));        	
        	Response.Write(whereStatement);    
        }
        
        public static void Main()
        {}
    }
}