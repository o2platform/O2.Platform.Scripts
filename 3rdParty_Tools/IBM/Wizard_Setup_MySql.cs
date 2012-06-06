// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using O2.Interfaces.O2Core;
using O2.Kernel;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.Views.ASCX;
using O2.Rules.OunceLabs.DataLayer_OunceV6;
using O2.Views.ASCX.MerlinWizard;
using O2.Views.ASCX.MerlinWizard.O2Wizard_ExtensionMethods;
//O2Ref:merlin.dll
using Merlin;
using MerlinStepLibrary;
//O2Ref:nunit.framework.dll
using NUnit.Framework; 
//O2Ref:O2_Rules_OunceLabs.dll

namespace O2.Script
{
    public class Wizard_Setup_MySql
    {    
    	private static IO2Log log = PublicDI.log;

		public void runWizard()
		{
	        var o2Wizard = new O2Wizard("Setup AppScan Source Edition (OunceLabs) MySql database");
	        	        
	        addStep_WelcomeMessage(o2Wizard);	        
	        addStep_EnterMySqlDetails(o2Wizard);	        
	        addStep_ShowMySqlDetails(o2Wizard);
	        addStep_TestMySqlDetails(o2Wizard);
	        
	        o2Wizard.start();		
        }
        public void addStep_WelcomeMessage(O2Wizard o2Wizard)
        {
			o2Wizard.Steps.add_Message("Welcome to this Wizard", "The next steps will take you over the process of configuring the\r\nIBM AppScan Source Edition (ex OunceLabs)\r\nMySql Services");
        }

        public void addStep_EnterMySqlDetails(O2Wizard o2Wizard)
        {        
        	// add step
			var step = o2Wizard.Steps.add_Panel("Enter MySql connection details");

			// add labels and position them 20 pixel apart
			var serverIP_Label = step.add_Label("MySql IP address:", 10);			
			var serverPort_Label = step.add_Label("MySql IP Port:",30);
			var userName_Label = step.add_Label("MySql UserName:",50);
			var password_Label = step.add_Label("MySql Password:",70);
			
			// add the textBoxes
			serverIP_Label	.append_TextBox(OunceMySql.MySqlServerIP, 		(newValue)=> OunceMySql.MySqlServerIP = newValue);
			serverPort_Label.append_TextBox(OunceMySql.MySqlServerPort, 	(newValue)=> OunceMySql.MySqlServerPort = newValue);
			userName_Label	.append_TextBox(OunceMySql.MySqlLoginUsername, 	(newValue)=> OunceMySql.MySqlLoginUsername = newValue);
			password_Label	.append_TextBox(OunceMySql.MySqlLoginPassword, 	(newValue)=> OunceMySql.MySqlLoginPassword = newValue);						
        }
        
        public void addStep_ShowMySqlDetails(O2Wizard o2Wizard)
        {        	
        	o2Wizard.Steps.add_Action("Confirm connection details", showMySqlDetails);
        }
                
        public void showMySqlDetails(IStep step)
        {        	
        	var message = String.Format("The Current MySql connection details are: {0} {0}" + 
        								"    Server IP: {2} {0}" +
        								"    Server Port: {2} {0}" +
        								"    UserName: {3} {0}" +
        								"    Password: {4} {0}" +
        								"{0} {0}" + 
        								"Connection String: {5}", 
        								Environment.NewLine, 
        								OunceMySql.MySqlServerIP,
        								OunceMySql.MySqlServerPort,
        								OunceMySql.MySqlLoginUsername,
        								OunceMySql.MySqlLoginPassword, 
        								OunceMySql.getLddbConnectionString());
			
			step.set_Text(message);						
        }
        
        public void addStep_TestMySqlDetails(O2Wizard o2Wizard)
        {
        	o2Wizard.Steps.add_Action("Testing MySql connection details", testMySqlDetails);
        }
        
        public void testMySqlDetails(IStep step)
        {
        	// run tasks on separate thread
        	O2Thread.mtaThread(
        		()=> {        			
        				step.append_Line("Trying to connect and loging to MySql server \r\n");
        				if (OunceMySql.isConnectionOpen())
        					step.append_Line("ALL OK: Sucessfully connected to MySql database");	
			        	else
			        		step.append_Line("ERROR!: It was not possible to connected to MySql database, check Log Viewer for error details");	
			        	step.append_Line("");
						step.append_Line("Test completed");
        			});
         	
        }
	        
        
    }
}
