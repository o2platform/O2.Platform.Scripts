// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Diagnostics;  
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;

namespace O2.Script
{
    public class Test
    {       	
		public static string GoBabyGo()  
		{
			Console.Write(">>>This was injected into target process :)".line());  
			Debug.Write("Inside injected Process");
			return "Currentely at process: " + System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
			
			//MessageBox.Show("this is from another dll");
			//var process= Processes.getCurrentProcess();
			//"Util - O2 Available scripts.h2".local().executeH2Script();
			//"ascx_Quick_Development_GUI.cs.o2".local().executeFirstMethod();
			//"aaa".popupWindow().add_LogViewer();			
			//return "".applicationWinForms().size().str();;
			//return "P ID {0} with title: {1}".format(process.Id,process.MainWindowTitle);			
		}
    }
}
