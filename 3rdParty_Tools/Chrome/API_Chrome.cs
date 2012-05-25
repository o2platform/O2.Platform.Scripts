// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;

namespace O2.XRules.Database.APIs
{
    public class API_Chrome
    {    
    	public string CefSharp_Download { get; set;}
    	
    	public API_Chrome()
    	{
    		CefSharp_Download = "https://github.com/downloads/ataranto/CefSharp/CefSharp-0.11-bin.7z";
    	}
    	
    	
    	
    }
    
    public static class API_Chrome_Install
   	{
   	}
}