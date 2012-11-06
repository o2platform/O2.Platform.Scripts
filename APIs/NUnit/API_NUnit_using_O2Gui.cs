// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.Ascx.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.XRules.Database.Utils;
using O2.Core.XRules.Ascx;

//O2Ref:O2_Core_XRules.dll
//O2Ref:O2_External_O2Mono.dll

namespace O2.XRules.Database.APIs
{
    public class API_NUnit_using_O2Gui : Control
    {        	
		
        public static Panel executeCurrentScript()
        {
        	return executeScript(PublicDI.CurrentScript);
        }
        public static Panel executeScript(string script)
        {        	
			var topPanel = O2Gui.open<Panel>("Unit Test Execution", 400,500);
			topPanel.insert_Below<ascx_LogViewer>(120).fill();
			var unitTest = topPanel.add_Control<ascx_XRules_UnitTests>();  
			unitTest.loadFile(script);
			unitTest.getXRulesTreeView().expand();
			unitTest.invoke("executeAllLoadedTests");
			return topPanel;
        }
		
		public static Control createExecutionGuiForFolder(string folder)
		{
			var topPanel = O2Gui.open<Panel>("Unit Test Execution for folder:{0}".format(folder) , 700,500);
			
			topPanel.insert_Below(100).add_LogViewer();
			var files_TreeView = topPanel.insert_Left("Unit Test Files to compiled an execute").add_TreeView();  			

			var unitTest = topPanel.add_Control<ascx_XRules_UnitTests>();  
			var textBox = topPanel.insert_Above<Panel>(25)
								  .add_LabelAndTextAndButton("Unit Test file","","load", (file)=> unitTest.loadFile(file))
								  .controls<TextBox>();
			textBox.onDrop(
				(text)=>{
							textBox.set_Text(text); 
							unitTest.loadFile(text);
						});
						 
			Action<string> loadFileOrFolder = 			 
				(fileOrFolder)=>{
									if (fileOrFolder.fileExists())
										files_TreeView.add_Node(fileOrFolder.fileName(),fileOrFolder);
									else
										files_TreeView.add_Files(fileOrFolder, "*.cs",false); 	  
								};
								
			Action<string> compileSelectedFile =  
				(file)=>{			
							files_TreeView.backColor(Color.Azure);
							O2Thread.mtaThread(
								()=>{					
										var assembly = file.compile();							
										if (assembly.isNull())
											files_TreeView.backColor(Color.LightPink); 
										else
										{
											files_TreeView.backColor(Color.White);
											unitTest.loadFile(file);
										}							
									});
						};
			
			files_TreeView.afterSelect<string>((file)=> compileSelectedFile(file));
			files_TreeView.onDoubleClick<string>((file)=> compileSelectedFile(file));
			
			files_TreeView.onDrop(loadFileOrFolder);
			
			files_TreeView.add_ContextMenu()
						  .add_MenuItem("Edit File", 
						  		()=> files_TreeView.selected()
						  						   .get_Tag()
						  						   .str()
						  						   .showInCodeEditor());
			loadFileOrFolder(folder);
			return topPanel;
		}
		
    }
}
