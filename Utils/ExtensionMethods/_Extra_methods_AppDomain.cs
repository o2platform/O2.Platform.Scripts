// Tshis file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.Objects;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;


namespace O2.XRules.Database.Utils
{		
	public static class _Extra_AppDomain_ExtensionMethods
	{
		public static string executeScriptInSeparateAppDomain(this string scriptToExecute)
		{
			return scriptToExecute.executeScriptInSeparateAppDomain(true, false);
		}
		
		public static string executeScriptInSeparateAppDomain(this string scriptToExecute, bool showLogViewer, bool openScriptGui)
		{
			var appDomainName = 12.randomLetters();
			var o2AppDomain =  new O2AppDomainFactory(appDomainName);
			o2AppDomain.load("O2_XRules_Database"); 	
			o2AppDomain.load("O2_Kernel");
			o2AppDomain.load("O2_DotNetWrappers");
			
			var o2Proxy =  (O2Proxy)o2AppDomain.getProxyObject("O2Proxy");
			if (o2Proxy.isNull())
			{
				"in executeScriptInSeparateAppDomain, could not create O2Proxy object".error();
				return null;
			}
			o2Proxy.InvokeInStaThread = true;
			if (showLogViewer)
				o2Proxy.executeScript( "O2Gui.open<Panel>(\"Util - LogViewer\", 400,140).add_LogViewer();");
			if (openScriptGui)
				o2Proxy.executeScript("O2Gui.open<Panel>(\"Script editor\", 700, 300)".line() + 
 	  								  "		.add_Script(false)".line() + 
									  " 	.onCompileExecuteOnce()".line() + 
									  " 	.Code = \"hello\";".line() + 
									  "//O2File:Scripts_ExtensionMethods.cs");
			o2Proxy.executeScript(scriptToExecute);
			
			//PublicDI.log.showMessageBox
			MessageBox.Show("Click OK to close the '{0}' AppDomain (and close all open windows)".format(appDomainName));
			o2AppDomain.unLoadAppDomain();
			return scriptToExecute;
		}
		
		public static O2Proxy executeScript(this O2Proxy o2Proxy, string scriptToExecute)
		{
			o2Proxy.staticInvocation("O2_External_SharpDevelop","FastCompiler_ExtensionMethods","executeSourceCode",new object[]{ scriptToExecute });						
			return o2Proxy;
		}
		
		public static string execute_InScriptEditor_InSeparateAppDomain(this string scriptToExecute)
		{
			var script_Base64Encoded = scriptToExecute.base64Encode();
			var scriptLauncher = "O2Gui.open<Panel>(\"Script editor\", 700, 300)".line() + 
 	  								  "		.add_Script(false)".line() + 
									  " 	.onCompileExecuteOnce()".line() + 
									  " 	.Code = \"{0}\".base64Decode();".line().format(script_Base64Encoded) + 
									  "//O2File:Scripts_ExtensionMethods.cs";
			scriptLauncher.executeScriptInSeparateAppDomain(false,false);
			return scriptLauncher;									  
		}
		
		public static string localExeFolder(this string fileName)
		{
			var mappedFile = PublicDI.config.CurrentExecutableDirectory.pathCombine(fileName);
			return (mappedFile.fileExists())
						? mappedFile
						: null;					
		}
	}	
}