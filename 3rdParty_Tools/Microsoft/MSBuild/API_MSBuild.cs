using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.Views.ASCX.DataViewers;
using O2.Views.ASCX.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.XRules.Database.Utils;

//O2File:_Extra_methods_Misc.cs
//O2File:_Extra_methods_Windows.cs
//O2File:_Extra_methods_WinForms_Controls.cs
//O2File:_Extra_methods_WinForms_TextBox.cs
//O2File:_Extra_methods_WinForms_Misc.cs
//O2File:_Extra_methods_WinForms_TableList.cs

namespace O2.XRules.Database.APIs
{	
	public class API_MSBuild_Test
	{
		public static void test()
		{
			"API MSBuild".open_MSBuild_Gui();
		}
	}	
	
	public class API_MSBuild
	{	
		public string MSBuild_Exe     { get; set; } 
		public string MSBuild_Exe_4_0 { get; set; }
		public string MSBuild_Exe_3_5 { get; set; }
		public string MSBuild_Exe_2_0 { get; set; }
		
		public Action<string> 	OnConsoleOut 		{ get; set; }
		public string 			CompilationTarget 	{ get; set; }
		public Process 			MSBuild_Process 	{ get; set; }
		public StringBuilder 	ConsoleOut 			{ get; set; }
		public bool			 	LogConsoleOut 		{ get; set; }
		public DateTime			BuildStartTime		{ get; set; }
		public TimeSpan			BuildDuration		{ get; set; }
		
		public API_MSBuild()
		{
			MSBuild_Exe_2_0 = @"C:\Windows\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe"; 
			MSBuild_Exe_3_5 = @"C:\Windows\Microsoft.NET\Framework\v3.5\MSBuild.exe"; 
			MSBuild_Exe_4_0 = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"; 
						
			MSBuild_Exe 	= MSBuild_Exe_4_0;
			
			OnConsoleOut	= default_OnConsoleOut;
			LogConsoleOut	= false;
		}
		
		public void default_OnConsoleOut(string message)
		{
			ConsoleOut.AppendLine(message);
			if (LogConsoleOut)
				message.info();
		}
	}
	
	public static class API_MSBuild_ExtensionMethods
	{
		public static API_MSBuild build(this string compilationTarget)
		{
			return new API_MSBuild().build(compilationTarget);
		}
		
		public static API_MSBuild start(this API_MSBuild msBuild)
		{
			return msBuild.build();
		}
		
		public static API_MSBuild build(this API_MSBuild msBuild)
		{
			return msBuild.build(msBuild.CompilationTarget);
		}
		
		public static API_MSBuild build(this API_MSBuild msBuild, string compilationTarget)
		{
			msBuild.CompilationTarget = compilationTarget;
			if (compilationTarget.fileExists())
			{
				msBuild.ConsoleOut 				= new StringBuilder();					
				msBuild.BuildStartTime 			= DateTime.Now;								
				msBuild.MSBuild_Process 		= msBuild.MSBuild_Exe.startProcess( msBuild.CompilationTarget, msBuild.OnConsoleOut);											
			}
			else
				"[API_MSBuild] in build, provided compilationTarget file doesn't exists: {0}".error(compilationTarget);
			return msBuild;
		}
		
		public static API_MSBuild waitForBuildCompletion(this API_MSBuild msBuild)
		{
			if (msBuild.MSBuild_Process.notNull())
			{
				msBuild.MSBuild_Process.WaitForExit();
				msBuild.BuildDuration = msBuild.BuildStartTime - DateTime.Now;
			}
			return msBuild;
		}
		
		public static bool build_Ok(this API_MSBuild msBuild, string compilationTarget)
		{			
			msBuild.build(compilationTarget)
				   .waitForBuildCompletion();
			
			var buildResult = msBuild.ConsoleOut.str().contains("Build succeeded.");
			if (buildResult)
				"[API_MSBuild] Project build OK: {0}".debug(compilationTarget);
			else				
				"[API_MSBuild] Project Failed to build: {0}".error(compilationTarget);
			return buildResult;
		}
	}
	
	public static class API_MSBuild_ExtensionMethods_Misc_Utils
	{
		public static List<string> csproj_Files(this string path)
		{
			return path.files("*.csproj",true);
		}
	}
	
	public static class API_MSBuild_ExtensionMethods_GuiHelpers
	{
		public static API_MSBuild_Gui open_MSBuild_Gui(this string title)
		{
			var topPanel = title.popupWindow(1000,550);
			topPanel.insert_LogViewer();
			return topPanel.add_MSBuild_Gui();
		}
		
		public static API_MSBuild_Gui add_MSBuild_Gui(this Panel topPanel)
		{
			return new API_MSBuild_Gui(topPanel);
		}
	}
	
	public  class API_MSBuild_Gui
	{
		public API_MSBuild 		msBuild;
		public bool			 	buildEnabled;
		public Panel			topPanel;
		public TextBox 			consoleOut_TextArea;
		//public TextBox			currentTarget_TextBox;
		public Label			status_Label;
		public CheckBox			buildEnabled_CheckBox;
		public ascx_TableList	tableList;
		public Action<string> 	startBuild;
		public Action<string>  	buildProjects;
		
		public API_MSBuild_Gui(Panel topPanel)
		{
			buildGui(topPanel);
		}
		public API_MSBuild_Gui buildGui(Panel _topPanel)
		{
			topPanel = _topPanel;
			msBuild = new API_MSBuild(); 
			buildEnabled = true;
			consoleOut_TextArea = topPanel.insert_Right(300).add_GroupBox("Console Out details").add_TextArea_With_SearchPanel().wordWrap(false);
			tableList = topPanel.clear().add_TableList("VisualStudio MSBuild results");
			tableList.add_Columns("Project", "Path", "Status", "Time", "Console Out");
			 
			startBuild = (pathToProject) =>
								{
									var buildStatus = "SKIPPED";
									var buildResult = false;
									if (buildEnabled)
									{															
										buildResult = msBuild.build_Ok(pathToProject); 
										buildStatus = buildResult ? "OK" :  "FAILED";																				
									}
									else
									{
										"buildEnabled was set to false, so skipping build for: {0}".error(pathToProject);
										msBuild.ConsoleOut = new StringBuilder();
									}
										
									tableList.add_ListViewItem(	pathToProject.fileName(), 
															   	pathToProject, 
															   	buildStatus, 
															   	msBuild.BuildDuration.timeSpan_ToString(),
															   	msBuild.ConsoleOut.str())
											 .foreColor(buildResult, Color.Green, Color.Red);
										
							   	}; 
			 
			tableList.afterSelect_get_Cell(4, (value)=>consoleOut_TextArea.set_Text(value)); 
			buildEnabled_CheckBox  	= tableList.insert_Below(50,"Config")
									 			.add_CheckBox("Build Enabled",3,0,(value) => buildEnabled = value).check();
			//currentTarget_TextBox	= buildEnabled_CheckBox.append_Label("Current Target").top(8).autoSize().append_TextBox("").width(300);
			status_Label 			= buildEnabled_CheckBox.append_Label("...").autoSize().top(8);
			
			tableList.add_ContextMenu()
					 .add_MenuItem("Recompile project" , true ,
					 	()=>{		 			
					 			startBuild(tableList.selected().values().second());
							})
					 .add_MenuItem("Open Project folder" ,  true,
					 	()=>{		 			
					 			tableList.selected().values()
					 								.second()
					 								.directoryName()
					 								.startProcess();
							})
					.add_MenuItem("Clear table" ,
						()=> tableList.clearRows() ); 
			
			//tableList.onDoubleClick_get_Row((row)=> buildProject(row.values().second()));
			tableList.columns_Width(200,200,50, 100,-2);
			
			buildProjects = (fileOrFolder)=>
								{
									//currentTarget_TextBox.set_Text(fileOrFolder);
									O2Thread.mtaThread(
										()=>{												
												tableList.listView().backColor(Color.Azure);
												"Loading file(s) from: {0}".info(fileOrFolder);
												if (fileOrFolder.fileExists())
												{
													status_Label.set_Text("[1/1] Building: {0}".format(fileOrFolder.fileName()));
													startBuild(fileOrFolder);
												}
												else
												{
													var files = fileOrFolder.csproj_Files();
													"Found {0} csproj files to process: {0}".debug(files.size());
													var processed = 1;
													foreach(var file in files)
													{
														status_Label.set_Text("[{0}/{1}] Building: {2}".format(processed ++, files.size(), file.fileName()));
														startBuild(file);																											
													}
												}
												status_Label.set_Text("Build complete");
												tableList.listView().backColor(Color.White);		
											});
								};	
			
			tableList.listView().onDrop(buildProjects);
			return this;
		}
	}
	
}