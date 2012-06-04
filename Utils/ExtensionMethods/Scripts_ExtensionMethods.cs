using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.Views.ASCX.Ascx.MainGUI;
//O2File:ascx_Simple_Script_Editor.cs.o2
//O2Ref:System.Xml.dll
//O2Ref:System.Xml.Linq.dll

namespace O2.XRules.Database.Utils
{
    public static class Scripts_ExecutionMethods
    {
        public static ascx_Simple_Script_Editor add_ScriptExecution(this Control hostControl)
        {
            return hostControl.add_Script();
        }

        public static ascx_Simple_Script_Editor add_Script(this Control hostControl)
        {
            return hostControl.add<ascx_Simple_Script_Editor>();           
        }

        public static ascx_Simple_Script_Editor add_Script(this Control hostControl, bool codeCompleteSupport)
        {
            return (ascx_Simple_Script_Editor)hostControl.invokeOnThread(
                () =>
                {
                    var scriptControl = new ascx_Simple_Script_Editor(codeCompleteSupport);
                    scriptControl.fill();
                    hostControl.add(scriptControl);
                    return scriptControl;
                });
        }

        public static ascx_Simple_Script_Editor set_Command(this ascx_Simple_Script_Editor scriptEditor, string commandText)
        {
            return (ascx_Simple_Script_Editor)scriptEditor.invokeOnThread(
                () =>
                {
                    scriptEditor.commandsToExecute.set_Text(commandText);
                    return scriptEditor;
                });
        }

        public static ascx_Simple_Script_Editor add_DevEnvironment<T>(this Control control)
            where T : Control
        {
            return control.add_DevEnvironment<T>(false);
        }

        public static ascx_Simple_Script_Editor add_DevEnvironment<T>(this Control control, bool includeLogViewer)
            where T : Control
        {
            var tTypeName = typeof(T).name().lowerCaseFirstLetter();
            var groupBoxes = control.add_1x1("Script", tTypeName, false, control.Width / 2);
            var tControl = groupBoxes[1].add<T>();
            if (tControl is TextBox)				// it might make more sent to add this to the control.add<..> method
                (tControl as TextBox).multiLine();
            var script = groupBoxes[0].add_Script(false);
            script.InvocationParameters.add(tTypeName, tControl);
            if (includeLogViewer)
                tControl.insert_Below<ascx_LogViewer>(150);
            return script;
        }

        public static ascx_Simple_Script_Editor execute(this ascx_Simple_Script_Editor scriptEditor, params string[] codesToExecute)
        {
            var codeToExecute = "";
            foreach (var code in codesToExecute)
                codeToExecute += code.line();
            return (ascx_Simple_Script_Editor)scriptEditor.invokeOnThread(
                () =>
                {
                    scriptEditor.commandsToExecute.set_Text(codeToExecute);
                    scriptEditor.onCompileExecuteOnce();
                    return scriptEditor;
                });
        }

        public static ascx_Simple_Script_Editor onCompileExecuteOnce(this ascx_Simple_Script_Editor scriptEditor)
        {
            return (ascx_Simple_Script_Editor)scriptEditor.invokeOnThread(
                () =>
                {
                    scriptEditor.onCompilationOk =
                        () =>
                        {
                            scriptEditor.execute();
                            scriptEditor.onCompilationOk = null;
                        };
                    return scriptEditor;
                });
        }    	    
    }
}
