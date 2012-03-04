// Tshis file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Views.ASCX.ExtensionMethods;
//O2File:ascx_ObjectViewer

namespace O2.XRules.Database.Utils
{	
		
	public static class _Extra_ObjectDetails_ExtensionMethods
	{		 				
		// so that it is automatically available in the O2 Scriping environment (was in public static class ascx_ObjectViewer_ExtensionMethods)
		public static void details<T>(this T _object)
		{
			O2Thread.mtaThread(()=>_object.showObject());
		}						
	}
}