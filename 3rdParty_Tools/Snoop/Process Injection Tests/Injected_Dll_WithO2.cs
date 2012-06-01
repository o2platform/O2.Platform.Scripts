// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
/*using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
*/
namespace O2.Script
{
    public class Test
    {        				
		public static string GoBabyGo()  
		{
			Debug.Write	("*** in Injected_Dll_WithO2.cs");
			try
			{
				//"test".info();				
				return "ok";			
			}
			catch(Exception ex)
			{
				Debug.Write	("Error: " + ex.Message);
				return "Error";
			}
				
		}
    }
}
