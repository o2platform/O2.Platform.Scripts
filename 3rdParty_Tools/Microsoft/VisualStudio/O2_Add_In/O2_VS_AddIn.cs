using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using System.Diagnostics;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.FluentSharp.VisualStudio.ExtensionMethods;
using O2.Views.ASCX.ExtensionMethods;
using O2.Views.ASCX.Ascx.MainGUI;
using O2.Views.ASCX.classes.MainGUI;
using O2.DotNetWrappers.DotNet;
using Microsoft.VisualStudio.CommandBars;
using O2.Platform;
using O2.Kernel;
using System.Windows.Forms;

//O2File:Microsoft_VisualStudio_ExtensionMethods.cs
//O2File:CommandBase.cs

//O2File:O2_LogViewer.cs 
//O2File:O2_ScriptGui.cs
//O2File:O2_ScriptWithPanel.cs

//O2Ref:EnvDTE.dll
//O2Ref:EnvDTE80.dll
//O2Ref:Extensibility.dll
//O2Ref:Microsoft.VisualStudio.CommandBars.dll


namespace O2.FluentSharp.VisualStudio
{
	public class O2_VS_AddIn
	{
		public DTE2		VS_Dte							 { get; set; }
		public AddIn	VS_AddIn						 { get; set; }

		public string	VS_Type							 { get; set; }

		public Dictionary<string, CommandBase>	Commands { get; set; }

		public O2_VS_AddIn()
		{
			Commands = new Dictionary<string, CommandBase>();

	//		CompileEngine.LocalReferenceFolders.Add(@"C:\_WorkDir\Git_O2OPlatform\O2.Platform.ReferenceAssemblies\O2_Assemblies");
		}
		
		
		public static O2_VS_AddIn create_FromO2LiveObjects()
		{
			var vsAddIn = new O2_VS_AddIn();
			vsAddIn.VS_Dte 		= (DTE2)  O2LiveObjects.get("VisualStudio_Dte");
    		vsAddIn.VS_AddIn 	= (AddIn) O2LiveObjects.get("VisualStudio_AddIn");
    		vsAddIn.VS_Type 	= (string)O2LiveObjects.get("VisualStudio_Type");    		
    		
			if (vsAddIn.notNull())
			{
				"[O2_VS_AddIn] loaded DTE2 object from O2LiveObjects".info();
				return vsAddIn ;
			}
			
			"[O2_VS_AddIn] failed to load  DTE2 object from  O2LiveObjects".error();
			return null;
		}
		
		

		public O2_VS_AddIn setup(DTE2 dte, AddIn addin, string vsType)
		{
			try
			{
				this.VS_Dte = dte;
				this.VS_AddIn = addin;
				this.VS_Type = vsType;
				
				
				O2LiveObjects.set("VisualStudio_Dte",dte);
				O2LiveObjects.set("VisualStudio_AddIn",addin);
				O2LiveObjects.set("VisualStudio_Type",vsType);
				
				

				//PublicDI.config.setLocalScriptsFolder(PublicDI.config.CurrentExecutableDirectory.pathCombine("O2.Platform.Scripts"));
				openO2LogViewer();

				
				"****O2LiveObjects: Dte: {0}".info(O2LiveObjects.get("VisualStudio_Dte"));
				"****O2LiveObjects: AddIn: {0}".info(O2LiveObjects.get("VisualStudio_AddIn"));
				"****O2LiveObjects: Type: {0}".info(O2LiveObjects.get("VisualStudio_Type"));
				
				
				//syncO2Repositories();
                "Testing Log viewer (from Dynamically compiled Script in O2)".info();
				
				this.add_TopMenu("O2 Platform");


				this.add_Command<O2_ScriptWithPanel>()
					.add_Command<O2_ScriptGui>()
					.add_Command<O2_LogViewer>();
			}
			catch (Exception ex)
			{
				ex.log();
			}
			return this;
		}

		public void syncO2Repositories()
		{
			try
			{
/*				GitHub_Actions.TargetDir = PublicDI.config.CurrentExecutableDirectory;
				GitHub_Actions.LogMessage = (message) => "[GitHub Actions] {0}".info(message);

				GitHub_Actions.LogMessage("Target Dir: {0}".format(GitHub_Actions.TargetDir));
				GitHub_Actions.CloneRepository("O2.Platform.Scripts");*/
			}
			catch (Exception ex)
			{
				ex.log();
			}
		}

		public void openO2LogViewer()
		{
			O2Gui.open<Panel>("O2 Log Viewer", 400, 300).add_LogViewer();
		}

	}

	public static class O2_VS_AddIn_ExtensionMethods_Commands
	{
		public static CommandBarPopup add_TopMenu(this O2_VS_AddIn o2AddIn, string caption)
		{
			var addAfterMenu = "Help"; //"Tools"
			var commandBars = (CommandBars)o2AddIn.VS_Dte.CommandBars;
			var menuCommandBar = commandBars["MenuBar"];
			var position = (commandBars[addAfterMenu].Parent as CommandBarControl).Index;
			position++;
			var newMenu = (CommandBarPopup)menuCommandBar.Controls.Add(MsoControlType.msoControlPopup, System.Type.Missing, System.Type.Missing, position, true);
			newMenu.Caption = caption;
			newMenu.Enabled = true;
			return newMenu;
		}

		public static O2_VS_AddIn add_Command<T>(this O2_VS_AddIn o2Addin)
			where T : CommandBase
		{
			var command = (T)typeof(T).ctor(o2Addin);		
			return o2Addin;
		}

		public static Panel add_WinForm_Panel(this O2_VS_AddIn o2Addin,string title, int width = -1 , int height = -1)
		{
			return o2Addin.VS_Dte.createWindowWithPanel(o2Addin.VS_AddIn, title, width, height);
		}

		public static Panel add_WinForm_Control_from_O2Script(this O2_VS_AddIn o2Addin, string title, string o2Script, string type, int width = -1, int height = -1)
		{
			var assembly = new CompileEngine().compileSourceFile(o2Script.local());
			var editorType = assembly.type(type);

			var panel = o2Addin.add_WinForm_Panel(title, width, height);
			panel.add_Control(editorType);
			return panel;
		}
	}

	public static class O2_VS_AddIn_ExtensionMethods_for_Connect_Calls
	{
		public static bool showCommand(this O2_VS_AddIn o2Addin, string commandName)
		{
	//		Debug.WriteLine("in showCommand: {0}".format(commandName));			
			return o2Addin.Commands.hasKey(commandName);
			//return (commandName == o2Addin.FullName);
		}

		public static bool executeCommand(this O2_VS_AddIn o2Addin, string commandName)
		{
			
			//var result = (commandName == o2Addin.FullName);
			if (o2Addin.Commands.hasKey(commandName))		
			{
				O2Thread.mtaThread(() => o2Addin.Commands[commandName].Execute()); ;
				return true;
				

			//	O2Gui.open<ascx_LogViewer>("Asp.Net O2 LogViewer", 400, 400);
		
			}
			return false;
		}

	}

}
