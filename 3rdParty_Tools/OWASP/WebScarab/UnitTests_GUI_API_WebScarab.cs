// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Network;
using O2.DotNetWrappers.Windows;
using NUnit.Framework;
//O2File:API_WebScarab.cs
//O2Ref:NUnit.Framework.dll

namespace O2.XRules.Database.APIs
{
	[TestFixture] 
	public class UnitTests_GUI_API_WebScarab
	{		
		[Test]	
		public string CheckInstallation() 
		{
			var webScarab = new API_WebScarab();
			Assert.That(webScarab.install(), "webScarab.install() was false");						
			return "ok";
		}
		[Test] 
		public string stop_RuninngInstances()    
		{
			var webScarab = new API_WebScarab();
			webScarab.attach();			
			webScarab.stop();
			ensureNoRunningInstances();
			return "ok";
		}
		
		[Test]
		public string ensureNoRunningInstances() 
		{
			var webScarab = new API_WebScarab();
			"in ensureNoRunningInstances".debug();
			webScarab.attach();
			Assert.That(webScarab.WebScarab_Process.isNull(), "webScarab.WebScarab_Process was not null, which means that there was a running instance of WebScarab");
			return "ok";
		}
		 
		[Test]
		public string launch_WebScarab() 
		{ 
			var webScarab = new API_WebScarab();
			stop_RuninngInstances();
			webScarab.start();  
			 
			Assert.That(webScarab.WebScarab_Process.notNull(), "webScarab.WebScarab_Process was null");
			Assert.That(webScarab.WebScarab_Process.HasExited.isFalse(), "webScarab.WebScarab_Process.HasExited was true");			
			Assert.That(webScarab.WebScarab_Process.MainWindowHandle != IntPtr.Zero, "webScarab.WebScarab_Process.MainWindowHandle was 0");
						
			webScarab.stop();
			ensureNoRunningInstances();
			return "ok";  
		}
		
		[Test]
		public string set_Interface_AdvancedMode() 
		{ 
			var webScarab = new API_WebScarab();
			stop_RuninngInstances();
			
			Assert.That(webScarab.start().started(), "WebScarab did not start");
			"Moving mouse to set interface to advanced mode".info();
			webScarab.setInterface_FullFeatured(); 			
			webScarab.stop();
			ensureNoRunningInstances();
			return "ok"; 
		}
		
		[Test]
		public string set_Interface_LiteMode() 
		{ 
			var webScarab = new API_WebScarab();
			stop_RuninngInstances(); 
			
			Assert.That(webScarab.start().started(), "WebScarab did not start");
			webScarab.setInterface_Lite(); 			
			"Moving mouse to set interface to Lite".info();
			webScarab.stop();
			ensureNoRunningInstances();
			return "ok"; 
		}				
	}
}