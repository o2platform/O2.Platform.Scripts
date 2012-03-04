// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using O2.Kernel;
using O2.Interfaces.O2Core;
using O2.XRules.Database._Rules._Sample_Vulnerabilities._MockClasses;
//O2File:HttpContext.cs
//O2File:HttpRequest.cs
//O2File:SqlCommand.cs

namespace O2.XRules.Database._Rules._Sample_Vulnerabilities
{
    public class Web_Sql_Injection
    {    
        private static IO2Log log = PublicDI.log;
        
    	
        public void controller()
        {
            var taintedData = source_HttpRequest();
            sink_SqlDataExecute(taintedData);
        }
		
        public string source_HttpRequest()
        {
            return HttpContext.Current.Request["payload"];
        }
		
        public void sink_SqlDataExecute(string whereStatement)
        {
            var sql = "Select name from users where " + whereStatement;			
            var cmd = new SqlCommand(sql, null);			
        }    	    	    	    	    
    }
}