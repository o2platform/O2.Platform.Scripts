using System;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods; 
using O2.DotNetWrappers.DotNet; 
using O2.DotNetWrappers.Windows; 
using O2.XRules.Database.Utils;
using O2.External.SharpDevelop.ExtensionMethods;
using System.Runtime.InteropServices;

//O2File:API_CLR.cs
//O2Ref:Snoop\Snoop\Snoop.exe
//Installer:Snoop_Installer.cs!Snoop\Snoop\Snoop.exe

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.APIs
{
	public class API_O2_Injector
	{			 			
		public string targetFile = "SimpleCSharpREPL.cs".local();
		//"SimpleCSharpREPL.cs".local()
		
		public bool injectIntoProcess(int processId)
		{
			var process = Processes.getProcess(processId);
			return injectIntoProcess(process);
		}
		
		public bool injectIntoProcess(Process process)
		{		
			var x64 = process.is64BitProcess();
			var runtime40 = (process).isRuntime_V4();
			return injectIntoProcess(process, x64, runtime40);
		}
		
		public bool injectIntoProcess(Process process, bool x64, bool runtime40)
		{
		 	var codeFile = "Injected_Dll.cs".local();
		 	var fixedCourceCode = codeFile.fileContents()	//ensure that we are pointing to the current locations of O2 folders
		 							 	  .replace(@"%CurrentExecutableDirectory%",PublicDI.config.CurrentExecutableDirectory)
		 							 	  .replace(@"%scriptToExecute%",targetFile );
		 	return 	injectIntoProcess(process, x64,runtime40, fixedCourceCode.saveWithExtension(".cs"));					 	  
		}
		
		public bool injectIntoProcess(Process process, bool x64, bool runtime40, string sourceCodeFile)
		{
		 	//fixedCourceCode.showInCodeViewer();
		 	var compileEngine = new CompileEngine(runtime40 ? "v4.0" : "v3.5") { useCachedAssemblyIfAvailable = false };		 	
			//var compiledAssembly = compileEngine.compileSourceCode(fixedCourceCode);			
			var compiledAssembly = compileEngine.compileSourceFile(sourceCodeFile);			
			return injectIntoProcess(process,x64, runtime40,compiledAssembly);
		}
		
		public bool injectIntoProcess(Process process, bool x64, bool runtime40, Assembly assemblyToExecute)
		{
			if (assemblyToExecute.notNull())
			{				
				var className = "O2.Script.Test"; //"Snoop.SnoopUI";
				var methodName = "GoBabyGo";
				return injectIntoProcess(process, assemblyToExecute.Location, className, methodName, x64, runtime40);
			}
			return false;
		}
		
		
		
		public bool injectIntoProcess(Process process,string assemblyToInject, string className, string methodName, bool x64, bool runtime40)
		{		
			try
			{	
				if (process.isNull())
				{
								
					"The value of the process variable was null".error();
					return false;
				}
				
				"Injecting into process {0}:{1} dll {1}".info(process.Id,process.ProcessName, assemblyToInject);				
						
				var suffix = (x64) ? "64-" : "32-";
				suffix += (runtime40) ? "4.0" : "3.5";
				
				var hwnd = process.MainWindowHandle;
				if (hwnd == IntPtr.Zero)
				{
				 	"Could not get MainWindow handle".error();
				 	return false;
				}
				"Got main Window Handle: {0}".info(hwnd);	
				
				var snoopAssembly = @"Snoop\Snoop\Snoop.exe".assembly(); 
				
				var directoryName = snoopAssembly.Location.directoryName();
				var fileName = directoryName.pathCombine("ManagedInjectorLauncher" + suffix+ ".exe");							  
				
								var windowHandle= hwnd;							
	
				var arguments = string.Concat(new object[]
					{
						windowHandle, " \"", assemblyToInject,"\" \"", className, "\" \"", methodName,"\""
					});
				"Starting process {0} with arguments {1}".info(fileName, arguments);
				fileName.startProcess(arguments);
				return true;
			}
			catch(Exception ex)
			{
				ex.logStackTrace();
				return false;
			}		
		}
	}


	public static class API_O2_Injector_GUI_Helpers
	{
		public static FlowLayoutPanel add_FlowLayoutPanel_with_DetectedModules(this Control targetControl, Process process)
		{
			targetControl.clear();
			
			var modules_byModuleName =  process.modules_Indexed_by_ModuleName();
			var modules_byFileName =  process.modules_Indexed_by_FileName();
			
			var flowPanel = targetControl.add_FlowLayoutPanel();
			Func<string,string, Button> add_Mapping = 
				(key, text)=>{	
									if (modules_byModuleName.hasKey(key) || modules_byFileName.hasKey(key))
										return flowPanel.add_Button(text);
									return null;
							  };
			
			
			add_Mapping("mscoree.dll"												, "CLR").green();
			add_Mapping(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\clr.dll" 	, "CLR 4.0 32bit").blue();
			add_Mapping(@"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\clr.dll" 	, "CLR 4.0 64bit").blue();
			add_Mapping("java.dll", "JVM").green();
			return flowPanel;
		}
	}
}