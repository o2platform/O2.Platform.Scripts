// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

//O2Ref:nunit.framework.dll
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.GuiAutomation;
using NUnit.Framework;

//O2File:API_Eclipse.cs

namespace O2.XRules.Database.APIs
{
	[TestFixture] 
	public class UnitTests_GUI_API_Eclipse
	{		
		[Test]	
		public string checkInstallation() 
		{
			var eclipse = new API_Eclipse();
			Assert.That(eclipse.install().isInstalled(), "eclipse.install().isInstalled() was false");						
			return "ok";
		}
		
		[Test]
		public string stopAllEclipseProcesses()
		{
			Processes.getProcessesCalled("eclipse").stop();
			Assert.That(Processes.getProcessesCalled("eclipse").size()==0, "there were still eclipse processes");
			return "ok";
		}
		
		[Test]
		public string startAndStopEclipse()
		{
			stopAllEclipseProcesses();
			var eclipse = new API_Eclipse();
			eclipse.start();
			this.sleep(1000);
			Assert.That(Processes.getProcessesCalled("eclipse").size()==1, "there should only be one eclipse process");
			eclipse.stop();
			Assert.That(Processes.getProcessesCalled("eclipse").size()==1, "the eclipse process was still there");
			return "ok";
    	}
    	
    	[Test]
		public string startAndAcceptDefaultWorkspaceLocation()
		{
			var eclipse = new API_Eclipse();
			eclipse.start();
			Assert.That(eclipse.Eclipse_Process.notNull(), "eclipse.Eclipse_Process was null");
			var workSpaceLauncher = eclipse.getWindow_WorkspaceLauncher();
			Assert.That(workSpaceLauncher.notNull(), "workSpaceLauncher was null");
			var textBox = workSpaceLauncher.workspaceLauncher_get_WorkspaceLocation_TextBox ();
			var button = workSpaceLauncher.workspaceLauncher_get_Ok_Button(); 
			Assert.That(textBox.notNull() && button.notNull(), "failed to get workSpaceLauncher textbox or button");
			button.Click(); 
			var windows = eclipse.GuiAutomation.windows(); 
			Assert.That(windows.size()==1, "There should only be one window openned");
			eclipse.stop();
			Assert.That(eclipse.Eclipse_Process.HasExited,"Eclipse process should had exited");
			return "ok";
		}
    	
    }
}
