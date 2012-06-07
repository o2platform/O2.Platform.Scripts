// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Data.SqlClient;
using O2.Kernel;
using O2.Interfaces.O2Core;
using Microsoft.Security.Application;
//O2Ref:AntiXSSLibrary.dll

namespace O2.XRules.Database._Rules._Sample_Vulnerabilities
{
    public class Web_Sql_Injection : Page
    {    
        private static IO2Log log = PublicDI.log;
        
        public SqlConnection sqlConnection { get; set; }
    	    	
        public void controller()
        {
            var taintedData = source_HttpRequest();
            sink_SqlDataExecute(taintedData);
            sink_SqlDataExecute2(taintedData);
        //    sink_ResponseWrite(taintedData);
        }
		
        public string source_HttpRequest()
        { 
            var taitedData = Request["payload"];
            return taitedData;
        }
		
        public void sink_SqlDataExecute(string whereStatement)
        {
        	//if (Regex.IsMatch(Request.Cookies["SSN"].ToString(),@"^\d{3}-\d{2}-\d{4}$"))
        	{
            	//var sql = "Select name from users where customer = " + whereStatement;            
            	//var cmd = new SqlCommand(sql, null);		
            	var sqlText = "Select name from users where customer = @customer";
            	var command = new SqlCommand(sqlText, sqlConnection);            	
            	command.Parameters.AddWithValue("customer", whereStatement);
            	command.ExecuteNonQuery();
            }
        }    	    	    	    	    
        
        public void sink_SqlDataExecute2(string whereStatement)
        {
        	Response.Write(whereStatement);
        	//note how the scanning happensin real time
        	//Once we fixed the problem it doesn't show up
            var sqlText = "Select name from accounts where account = @whereStatement";// + whereStatement;			
            var command = new SqlCommand(sqlText, sqlConnection);		
            command.Parameters.Add("@whereStatement",whereStatement);
            command.ExecuteNonQuery();
        }    	  
        
        public void sink_ResponseWrite(string whereStatement)
        {                	
        	Response.Write(AntiXss.HtmlEncode(whereStatement));
        	Response.Write(Server.HtmlDecode(whereStatement));        	
        	//Response.Write(whereStatement);
        }
    }
}