// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;

//O2File:API_IIS_Logs

namespace O2.XRules.Database.APIs
{
	public class API_IIS_Request
	{
		public List<IIS_Log_Entry> LogEntries { get; set; } 
		
		public API_IIS_Request()
		{
			LogEntries = new List<IIS_Log_Entry>();
		}
				
		
		public class Folder
		{
			public string Name { get; set; }
			//public string Name { get; set; }
		}		
	}
}