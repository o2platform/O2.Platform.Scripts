// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.Views.ASCX;
using O2.XRules.Database.Utils;

//O2File:API_GuiAutomation.cs
//O2Ref:White.Core.dll
//O2Ref:WatiN.Core.1x.dll

namespace O2.XRules.Database.APIs
{

	public class API_RDP_Test
	{
		public void launch()
		{
			new API_RDP().launchRdpClient("120.0.0.1","aaa","bbb");	
		}
	}
	
    public class API_RDP 
    {
    	public API_GuiAutomation launchRdpClient(string ipAddress)
    	{
    		return launchRdpClient(ipAddress,null,null);
    	}
    	
    	public API_GuiAutomation launchRdpClient(string ipAddress, string username, string password)
    	{
    		var terminalServicesClient = Processes.startProcess("mstsc.exe");
			var guiAutomation = new API_GuiAutomation(terminalServicesClient);
			var window = guiAutomation.window("Remote Desktop Connection");
			window.textBox("Computer:").set_Text(ipAddress);
			
			if (username.valid())
			{
				window.button("Options ").click(); 					
				this.sleep(1000) ;	
				window.textBox("User name:").set_Text(username); 				
			}
			
			window.button("Connect").mouse().click();
			var loginWindow = guiAutomation.window("Windows Security",3);	
			if (password.valid())		
				guiAutomation.keyboard_sendText(password);									
			loginWindow.button("OK").click(); 
			
			return guiAutomation;
		}
    }
}