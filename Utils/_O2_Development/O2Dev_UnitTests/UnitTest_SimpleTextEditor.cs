// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.REPL;
using FluentSharp.WinForms;
using NUnit.Framework;


//O2Ref:nunit.framework.dll
//O2File:API_InputSimulator.cs

namespace O2.XRules.Database.UnitTests
{		
	[TestFixture]
    public class Test_SimpleTextEditor
    {        	    
    	string scriptFile = "Util - Simple Text Editor.h2".local();    	
    	
    	[Test]
    	public string compile()
    	{    		
    		Assert.That(scriptFile.fileExists(),"Could not find script file: {0}".format(scriptFile));    		
    		Assert.That(scriptFile.compile_H2Script().notNull(),"Could not compile script file: {0}".format(scriptFile));
    		return "ok";
    	}
    	
    	[Test]
    	public string launchGui()
    	{
    		var compiledScript = scriptFile.compile_H2Script() ;
    		var returnData = compiledScript.executeFirstMethod();
    		Assert.That(returnData.notNull(), "First method execution returned null");
    		var parentForm = (returnData as Control).parentForm();
    		Assert.That(parentForm.notNull() && parentForm is Form, "could not get Form object");
    		parentForm.close();
    		parentForm.sleep(500);
    		Assert.That(parentForm.Visible == false, "Form should not be visible by now");    		
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
    	public string writeText()
    	{
    		var dynamicText = "this is a Dynamic Text";
    		
 			var form = openForm();
 			var inputSimulator = new API_InputSimulator();
 			 			
 			inputSimulator.mouse_MoveTo(form);
 			inputSimulator.mouse_Click();
			inputSimulator.text_Send(dynamicText); 			
			var richTextBox = form.controls<RichTextBox>();
			
			Application.DoEvents();
			form.sleep(500);
			var currentText = richTextBox.get_Text();
			"current Text: {0}".info(currentText);
			Assert.That (currentText == dynamicText, "currentText != dynamicText: {0} != {1}".format(currentText,dynamicText)); 			
 			form.sleep(1000); 			 			
 			closeForm(form);
 			
			
			return "ok";    								  
    	}
    	
    	
    	
    }
}
