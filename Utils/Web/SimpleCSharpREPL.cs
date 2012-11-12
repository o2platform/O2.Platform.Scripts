// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
//O2Ref:O2_FluentSharp_BCL.dll

namespace O2.XRules.Database.Utils
{
    public class SimpleCSharpREPL
    {    
		public static Panel launch()
		{
			var topPanel = "Util - Text Based C# REPL".popupWindow(800,400).insert_LogViewer();  			
			
			var replGui = topPanel.add_REPL_Gui();   
			 
			var codeText = replGui.Code_Panel.add_TextArea().allowTabs(); 
			 
			 
			Action execute = 
				()=>{ 
						var compilationOk = false;   
						var code = codeText.get_Text();
						var result = code.compileAndExecuteCodeSnippet(
											(okMsg)=>   { replGui.showOutput(okMsg); compilationOk = true;},
											(failMsg)=> { replGui.showErrorMessage(failMsg); });
						if(compilationOk)
							replGui.showOutput(result); 
						//replGui.Output_RichTextBox.set_Text(code); 
					}; 
					
			replGui.On_ExecuteCode = execute;  
			
			codeText.set_Text("return \"Hello World\";");
			
			replGui.Execute_Button.click();
			return topPanel;
		}
    }
}
