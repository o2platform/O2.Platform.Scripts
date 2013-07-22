// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Collections.Generic;
using System.Linq;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.GuiAutomation;
using FluentSharp.REPL.Utils;
using FluentSharp.Watin;
using White.Core.UIItems.WindowItems;

//O2File:API_GuiAutomation.cs
//O2Ref:FluentSharp.Watin.dll
//O2Ref:Watin.Core.dll
//O2Ref:White.Core.dll 

namespace O2.XRules.Database.APIs
{
    public class API_SI_Exams
    {
    	public WatiN_IE ie { get; set; }    	    	
    	public API_GuiAutomation guiAutomation { get; set; } 
    	public Window guiAutomationWindow { get; set; }	
    	//public White.Core.UIItems.GroupBox guiAutomationExamGroupBox { get; set; }    	
    	
    	public White.Core.UIItems.Button ForwardButton { get; set; }
    	public White.Core.UIItems.Button SubmitButton { get; set; }
    	public string BaseFolder { get; set; }
    	public string ExamFile { get; set; }
    	
    	public List<White.Core.UIItems.CheckBox> checkBoxes { get; set; }
    	public List<White.Core.UIItems.RadioButton> radioButtons { get; set; }
    	
    	
    	public API_SI_Exams(WatiN_IE _ie)
    	{    		
    		this.ie = _ie;
    		//setGuiAutomation();
    	}
    	
    	public API_GuiAutomation setGuiAutomation()
    	{
    		"Mapping GuiAutomation Objects".info();
    		//var localProcess = Processes.getCurrentProcess();    		
    		//guiAutomation = new API_GuiAutomation(localProcess);
    		var ieProcess = Processes.getProcesses()
						.Where((process)=> process.MainWindowTitle.contains("SI Question Pool"))
						.First();
			guiAutomation = new API_GuiAutomation(ieProcess);						
    		if (guiAutomation.notNull())
    			"got GuiAutomation Object for local Process".info();    		
    		guiAutomationWindow = guiAutomation.windows()[0]; 
    		if (guiAutomationWindow.notNull())
    			"got GuiAutomation Window Object for local Process".info();
    		else
    			"failed to get GuiAutomation Window Object for local Process".error();
    		
    		//setGuiAutomationExamGroupBox();
    		//if (guiAutomation.isNull() || guiAutomationWindow.isNull() || guiAutomationExamGroupBox.isNull());
    		//	"in setGuiAutomation, one of the expected UIAutomation objects could not be calculated".error();
    		return guiAutomation;
    	}    	
    	
    	/*public API_GuiAutomation setGuiAutomationExamGroupBox()
    	{
    		guiAutomationExamGroupBox = guiAutomationWindow.groupBox("Exam");
    		if (guiAutomationExamGroupBox.notNull())
    			"got GuiAutomation Exams GroupBox for local Process".info();
    		else
    			"failed to get GuiAutomation Exams Object for local Process".error();
			return guiAutomation;    			
    	}*/
    }
    
    public static class API_SI_Exams_ExtensionMethods
    {
    	public static API_SI_Exams startPage(this API_SI_Exams apiSiExams)
    	{
    		apiSiExams.ie.open(apiSiExams.ExamFile);
    		return apiSiExams;
    	}
    	    	
    	public static API_SI_Exams ie_ScrollUp(this API_SI_Exams apiSiExams)
    	{
    		apiSiExams.ie.elements().elements("OBJECT").first().scrollIntoView();  
    		return apiSiExams;
    	}
    	
    	public static API_SI_Exams ie_ScrollDown(this API_SI_Exams apiSiExams)
    	{
    		apiSiExams.ie.elements().elements("TD").first().scrollIntoView();
    		return apiSiExams;
    	}
    	
    	public static White.Core.UIItems.Button  ie_FlashButtonClick(this API_SI_Exams apiSiExams, string buttonName)
    	{
    		apiSiExams.ie_ScrollDown(); 
    		"getting button reference".info();    		
			var button = apiSiExams.guiAutomationWindow.button(buttonName);
			if (button.notNull())
			{
				"[ie_FlashButtonClick] got button reference".debug();
				button.mouse();
				apiSiExams.guiAutomation.mouse_Click();
			}
			else
				"[ie_FlashButtonClick] could not get button reference for: {0}".error(buttonName);
			return button;
    	}
    	public static API_SI_Exams ie_Forward(this API_SI_Exams apiSiExams)
    	{
    		apiSiExams.ForwardButton = apiSiExams.ie_FlashButtonClick("Forward ");
    		return apiSiExams;
    	}
    	
    	public static API_SI_Exams ie_Back(this API_SI_Exams apiSiExams)
    	{
    		apiSiExams.ie_FlashButtonClick("Back ");
    		return apiSiExams;
    	}
    	
    	public static API_SI_Exams ie_Submit(this API_SI_Exams apiSiExams)
    	{
    		apiSiExams.SubmitButton = apiSiExams.ie_FlashButtonClick("Submit");
    		if (apiSiExams.SubmitButton.notNull())
    			"Got SubmitButton".debug();
    		return apiSiExams;
    	}
    	
    	public static API_SI_Exams ie_ClickCheckBox(this API_SI_Exams apiSiExams)
    	{    	
    		if (apiSiExams.checkBoxes.isNull())
    		{
    			"Finding CheckBoxes".info();    		
    			apiSiExams.checkBoxes = apiSiExams.guiAutomationWindow.checkBoxes();    		
    		}
    		else
    		{
    			"reusing checkboxes".info();
    		}
    		var checkBoxes = apiSiExams.checkBoxes;
    		if (checkBoxes.size()>0)
    		{
    			"[ie_ClickCheckBox] Found {0} CheckBoxes".debug(checkBoxes.size());
    			checkBoxes[0].Click();
    			show.info(checkBoxes[0]);
    		}
    		else
    		{
    			"[ie_ClickCheckBox] There where no CheckBoxes in guiAutomationExamGroupBox".debug();
    		/*	checkBoxes = apiSiExams.guiAutomationWindow.checkBoxes();    		
    			if (checkBoxes.size() > 0)
	    		{
	    			"[ie_ClickCheckBox] Found {0} CheckBoxes".debug(checkBoxes.size());
    				checkBoxes[0].Click();
	    		}
	    		else
	    		{
	    			"[ie_ClickCheckBox] There where no CheckBoxes in guiAutomationWindow".debug();
	    		}*/
	    	}
    			
    		return apiSiExams;
    	}
    	
    	public static API_SI_Exams ie_ClickRadioButton(this API_SI_Exams apiSiExams)
    	{
    		if (apiSiExams.radioButtons.isNull())
    		{
    			"Finding RadioButtons".info();
    			apiSiExams.radioButtons = apiSiExams.guiAutomationWindow.radioButtons();    		
    		}
    		else
    		{
    			"reusing RadioButtons".info();
    		}
    		var radioButtons = apiSiExams.radioButtons;
    		if (radioButtons.size()>0)
    		{
    			"[ie_ClickRadioButton] Found {0} CheckBoxes".debug(radioButtons.size());
    			radioButtons[0].Click();
    			show.info(radioButtons[0]);
    		}
    		else
    		{
    			"[ie_ClickRadioButton] There where no radioButtons in guiAutomationExamGroupBox".debug();
    			/*radioButtons = apiSiExams.guiAutomationWindow.radioButtons();    		
    			if (radioButtons.size() > 0)
	    		{
	    			"[ie_ClickCheckBox] Found {0} CheckBoxes".debug(radioButtons.size());
    				radioButtons[0].Click();
	    		}
	    		else
	    		{
	    			"[ie_ClickCheckBox] There where no CheckBoxes in guiAutomationWindow".debug();
	    		}*/
	    	}
    			
    		return apiSiExams;
    	}
    	
    }
}