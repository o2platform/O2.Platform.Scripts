// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;
using WPF = System.Windows.Controls;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.XRules.Database.APIs;
using NUnit.Framework;
using O2.XRules.Database.Utils;
using WindowsInput.Native;
using WindowsInput;
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll

//O2Ref:nunit.framework.dll
//O2File:API_InputSimulator.cs

//O2Ref:WindowsFormsIntegration.dll
//O2Ref:PresentationFramework.dll
//O2Ref:PresentationCore.dll
//O2Ref:WindowsBase.dll
//O2Ref:System.Xaml.dll

namespace O2.XRules.Database.UnitTests
{		
	[TestFixture]
    public class Test_O2TwitterClient
    {        	    
    	string scriptFile = "Twitter Client.h2".local();    	
    	
    	//[Test]
    	public string compile()
    	{    		
    		Assert.That(scriptFile.fileExists(),"Could not find script file: {0}".format(scriptFile));    		
    		Assert.That(scriptFile.compile_H2Script().notNull(),"Could not compile script file: {0}".format(scriptFile));
    		return "ok";
    	}
    	
    	public Form openForm()
    	{
    		var parentForm = (scriptFile.compile_H2Script()
    								   .executeFirstMethod() as Control)
    								   .parentForm();
			Assert.That(parentForm.notNull() && parentForm is Form, "could not get Form object");
			return parentForm;
    	}
    	
    	public void closeForm(Form form)
    	{
    		form.close();
    		form.sleep(500);
    		Assert.That(form.Visible == false, "Form should not be visible by now");    		
    	}
    	
    	[Test]
    	public string loginAsTestUser()
    	{
    		string testUserName = "useo2";
    		string testPassword = "....";
    		
    		var inputSimulator = new API_InputSimulator();
    		var form = openForm();    	
			
			//get Gui and click on the "User details" link
			
    		var wpfGui = form.control("WPF_GUI",true);
    		var o2TwitterApi = wpfGui.Tag;
    		Assert.That(o2TwitterApi.typeName() == "O2TwitterAPI","wpfGui tag object was not of type O2TwitterAPI");
    		Assert.That(o2TwitterApi.prop("IsLoggedIn").str() == "False" , "We should be logged out at this stage");
    		
    		var link = ((List<WPF.Button>)wpfGui.prop("Links")).name("User details");
    		Assert.That(link.notNull(),"Could not get the requested link object");    		
    		    		
    		inputSimulator.mouse_MoveTo_Wpf(link).click();    		
    		form.sleep(1000);
    		
    		// get the winFormPanel
    		var winFormPanel = (Panel)wpfGui.prop("WinFormPanel");
    		Assert.That(winFormPanel.notNull(), "Could not get the WinFormPanel control from the WinGui");
    		
    		// Populate username and password textboxes
    		var textBoxes = winFormPanel.controls<TextBox>(true);    		
    		TextBox usernameTextBox = null;
    		TextBox passwordTextBox = null;
    		foreach(var textBox in textBoxes)
    		{
    			if (textBox.Tag.str() == "username")
    			 	usernameTextBox = textBox;    				
    			if (textBox.Tag.str() == "password")
    				passwordTextBox = textBox;
    		}
    		Assert.That(usernameTextBox.notNull(), "Could not get the username TextBox");
    		Assert.That(passwordTextBox.notNull(), "Could not get the password TextBox");
    		
    		inputSimulator.mouse_MoveTo(usernameTextBox);
    		usernameTextBox.set_Text(testUserName);
    		inputSimulator.mouse_MoveTo(passwordTextBox);
    		passwordTextBox.set_Text(testPassword);    		
    		
    		//get the login button
    		var LoginButton = winFormPanel.button("Login");
    		inputSimulator.mouse_MoveTo(LoginButton);
    		inputSimulator.mouse_Click();
    		form.sleep(500);
    		Application.DoEvents();
    		form.sleep(3000);
    		Assert.That(o2TwitterApi.prop("IsLoggedIn").str() == "True" , "We should be logged in at this stage");
    		closeForm(form);
    		return "ok";
    	}
    	    	    	    	    	
    	
    	
    }
}
