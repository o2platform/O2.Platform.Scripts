using O2.External.SharpDevelop.ExtensionMethods;
using FluentSharp.CoreLib;
using FluentSharp.WinForms;
using FluentSharp.WinForms.Controls;
using FluentSharp.REPL;
using FluentSharp.REPL.Controls;
using System.Drawing;
using System.Windows.Forms;
using System; 


namespace O2.XRules.Database.Utils
{
	public class O2CommandPrompt
	{
		public static void run()
		{ 						
			var script = O2Gui.open<ascx_Simple_Script_Editor>("O2 Command Prompt (press enter to execute the command)", 700, 70);
			script.splitContainer_CommandBox.distance(400);
			script.splitContainer_CommandBox.Panel2.clear();
			var groupBox = script.splitContainer_Results.controls(true).@get<GroupBox>();
			script.splitContainer_CommandBox.Panel2.@add(groupBox);
			script.commandsToExecute.textEditor().showInvalidLines(false);
			//script.commandsToExecute.textEditor().showLineNumbers(false);
			script.ExecuteOnEnter = true;
            script.onCompileExecuteOnce();
			script.commandsToExecute.set_Text("hello".line());            
		}
	}
}