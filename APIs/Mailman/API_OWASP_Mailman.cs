// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;
//O2File:API_Mailman.cs

namespace O2.XRules.Database.APIs
{
	public class API_OWASP_Mailman : API_Mailman
	{
		public API_OWASP_Mailman()
		{
			this.BaseUrl = "https://lists.owasp.org/";
			this.TempDir = "_owasp_mailman_Data".tempDir(false);
		}
		
		
	}
}	