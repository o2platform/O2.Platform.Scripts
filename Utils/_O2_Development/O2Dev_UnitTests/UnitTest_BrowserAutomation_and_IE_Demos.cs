// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using White.Core;

//O2Ref:nunit.framework.dll
//O2File:API_GuiAutomation.cs
//O2Ref:White.Core.dll

namespace O2.XRules.Database.UnitTests
{		
	[TestFixture]
    public class UnitTest_BrowserAutomation_and_IE_Demos
    {        	    
    	public UnitTest_BrowserAutomation_and_IE_Demos()
    	{
    		API_GuiAutomation.Mouse_Move_SleepValue = 0;
    	}
    	[Test]
    	public string OpenGuiAndExecuteOneTest()
    	{
    		var desktopIconText = "OWASP O2 Platform (ClickOnce version)";  

			var step2_windowTitle = "OWASP O2 Platform - v1.4 beta (Sep 2010)";
			var step3_windowTitle = "O2 GUI - Browser Automation / BlackBox Testing";
			var step4_windowTitle = "Browser Automation / IE Demos";
			var guiAutomation = new API_GuiAutomation();
			
			Action step1 = 
				()=>{	
						guiAutomation.showDesktop();				 
						var icon = guiAutomation.desktopIcon(desktopIconText)
								 				.mouse()
								 				.doubleClick(); 
								 				
						//guiAutomation.desktopWindow("O2 Simple Script Editor").restored();
					}; 
					 						 			 
			Action step2 = 
				()=>{
						var window = guiAutomation.desktopWindow(step2_windowTitle, 20);
						window.bringToFront();
						window.move(0,0);					
						var buttonText = " Browser Automation / BlackBox Testing  ";		
						window.button(buttonText).mouse().click();
					};
				
			Action step3 =	
				()=>{			
						var step3_window = guiAutomation.desktopWindow(step3_windowTitle,20).restored();
						step3_window.tabPage("Demo Vulnerable Applications").mouse().click();
						step3_window.button("Browser Automation - IE Demos").mouse().click(); 
					};
					
			Action step4 =	 
				()=>{			
						var step4_window = guiAutomation.desktopWindow(step4_windowTitle,20).restored();
						step4_window.move(0,0);			
						step4_window.treeView()
								    .treeNodes()[2]
								    .mouse().doubleClick();
						step4_window.treeView() 
								    .treeNodes()[3]
								    .mouse().doubleClick();	 				    			
									 
					};
			
			Action step5 =	
				()=>{	
						//this.sleep(10*1000);	// wait 10 seconds before closing everying down
						guiAutomation.desktopWindow(step4_windowTitle).bringToFront().button("Close").mouse().click();
						guiAutomation.desktopWindow(step3_windowTitle).bringToFront().button("Close").mouse().click();
						guiAutomation.desktopWindow(step2_windowTitle).bringToFront().button("Close").mouse().click();
					};
			step1();
			step2(); 
			step3();
			step4();
			step5();
			return "ok - OpenGuiAndExecuteOneTest";
    	}
    	
    }
}
