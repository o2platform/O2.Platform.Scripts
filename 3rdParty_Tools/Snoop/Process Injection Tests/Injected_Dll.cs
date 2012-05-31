// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
/*using O2.DotNetWrappers.DotNet;
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
    	static Process CurrentProcess = System.Diagnostics.Process.GetCurrentProcess();
    	static string  assemblyLocation = @"E:\O2_V4\O2.Platform.Projects\binaries\O2_FluentSharp_CoreLib.dll";
		static Assembly assembly = Assembly.LoadFrom(assemblyLocation);
			
    	public static void info(string message)
    	{
    		var type = assembly.GetType("O2.Kernel.PublicDI");
			var method = type.GetMethod("get_log");
			var kConfig = method.Invoke(null, new object[] { });
			var info = kConfig.GetType().GetMethod("i");
			info.Invoke(kConfig, new object[] { message});
    	}
    	
    	public static void startProcess(string exe)
    	{
    		var type = assembly.GetType("O2.Kernel.exec");
			var obj = Activator.CreateInstance(type);
			var cmd = type.GetMethods()[0];				
			cmd.Invoke(obj, new object[] { exe });
    	}
    	
    	public static void listLoadedAssemblies()
    	{
    		foreach(ProcessModule module in CurrentProcess.Modules)
    		{
    			info(module.ModuleName);
    		}	
    	}
    	
    	
		public static string GoBabyGo()  
		{
			try
			{
				Debug.Write(">> Inside GoBabyGo method in Process " +CurrentProcess.ProcessName);
				info("a message from O2"); 
				listLoadedAssemblies();
			//	info("Starting nodepad");
			//	startProcess("notepad.exe");
				return ">> done";				
			}
			catch(Exception ex)
			{
				return "Error: " + ex.Message;
			}
			
			
			
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
