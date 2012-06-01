using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;
//O2Ref:MS_SDK_SvcUtil.exe

namespace O2.XRules.Database.Languages_and_Frameworks.DotNet
{

	public class DotNet_AspNet_Compiler_Test
	{
		public static Form launchGui()
		{
			var aspnetCompiler = new DotNet_AspNet_Compiler();
			var topPanel = O2Gui.open<Panel>("Tool - Precompile Asp.Net websites", 700,400);  
			var controls = topPanel.add_1x1("CompilationResult","Files in output dir");
			var compilationResult = controls[0].add_TextArea().wordWrap(false);
			var filesInOutputDir= controls[1].add_Directory();
			var settings = topPanel.insert_Above<Panel>(90);
			
			var websiteToCompile  = settings.add_Label("Website To Compile:")
											.append_TextBox("")  
											.align_Right(settings);
											
			var targetFolder  = settings.add_Label("Target Folder:           ",24,0)
											.append_TextBox("") 
											.align_Right(settings);								
											
			var virtualPath  = settings.add_Label("Virtual Path:              ",44,0)
											.append_TextBox("") 
											.align_Right(settings);					
											
			var precompileButton = settings.add_Button("PreCompile Website",64,112);
			precompileButton.append_Link("view aspnet_compiler.exe help", ()=> aspnetCompiler.show_Help());
			precompileButton.onClick(
				()=>{
						precompileButton.enabled(false);	
						filesInOutputDir.open(targetFolder.get_Text());
						O2Thread.mtaThread(
							()=>{									
									compilationResult.set_Text(aspnetCompiler.compile(websiteToCompile.get_Text(), 
															   targetFolder.get_Text(),
															   virtualPath.get_Text()));
			
									precompileButton.enabled(true);
								});					
					});
			
			websiteToCompile.set_Text(@"C:\O2\DemoData\HacmeBank_v2.0 (Dinis version - 7 Dec 08)\HacmeBank_v2_Website");
			targetFolder.set_Text(@"C:\O2\DemoData\HacmeBank_Precompiled\Website");
			virtualPath.set_Text("/");
			
			return topPanel.parentForm();			
		}
	}
		
    public class DotNet_AspNet_Compiler
    {
    	public string AspnetV4 { get; set;}
    	public string AspnetV2 { get; set;}
    	public string Aspnet_Compiler_Exe { get; set; }
    	//public string OutputTarget { get; set; }    		
    	//public string MetadataDocumentPath { get; set; }

		//modified
		
		public DotNet_AspNet_Compiler()
		{
			AspnetV4 = @"C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319";;
			Aspnet_Compiler_Exe = AspnetV4.pathCombine("aspnet_compiler.exe");			
		}
								
		
		public string execute(string parameters)
		{
			return Processes.startProcessAsConsoleApplicationAndReturnConsoleOutput(Aspnet_Compiler_Exe,parameters);
		}				
       
    }
    
    public static class DotNet_AspNet_Compiler_ExtensionMethods
    {
    	
    	public static string help(this DotNet_AspNet_Compiler aspnetCompiler)
		{
			return aspnetCompiler.execute("-?");
		}
		
		public static DotNet_AspNet_Compiler show_Help(this DotNet_AspNet_Compiler aspnetCompiler)
		{
			var panel= O2Gui.open<Panel>("Aspnet_Compiler Help",400,600); 
			panel.add_TextArea().set_Text(aspnetCompiler.help());  
			return aspnetCompiler;
		}
		
		public static string compile(this DotNet_AspNet_Compiler aspnetCompiler, string websiteToCompile, string targetFolder,string virtualPath)
		{
			var commands = " -v \"{0}\" -p \"{1}\" -d -f \"{2}\"".format(virtualPath,websiteToCompile,  targetFolder);
			"Commands to execute: {0}".info(commands);
			return aspnetCompiler.execute(commands); 
		}		
    }
}
