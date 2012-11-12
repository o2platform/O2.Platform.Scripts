using System;
using System.Diagnostics;
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

namespace O2.XRules.Database.APIs
{
	public class API_O2_Injector
	{			 			
		
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
		 							 	  .replace(@"E:\O2_V4\O2.Platform.Projects\binaries",PublicDI.config.CurrentExecutableDirectory)
		 							 	  .replace(@"E:\O2_V4\O2.Platform.Scripts", PublicDI.config.LocalScriptsFolder);
		 	//fixedCourceCode.showInCodeViewer();
			var compiledAssembly = new CompileEngine().compileSourceCode(fixedCourceCode);
			if (compiledAssembly.notNull())
			{
				var className = "O2.Script.Test"; //"Snoop.SnoopUI";
				var methodName = "GoBabyGo";
				return injectIntoProcess(process, compiledAssembly.Location, className, methodName, x64, runtime40);
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
				//var process = Processes.getProcess(12168);
				var hwnd = process.MainWindowHandle;
//				var hwnd = process.Handle;
				//var hwnd = new IntPtr(398680);
				if (hwnd == IntPtr.Zero)
				{
				 	"Could not get MainWindow handle".error();
				 	return false;
				}
				"Got main Window Handle: {0}".info(hwnd);	
				
				var snoopAssembly = @"Snoop\Snoop\Snoop.exe".assembly(); 
				
				var directoryName = snoopAssembly.Location.directoryName();
				//return process.MainModule;
//				var suffix = snoopAssembly.type("Injector")
//										   .invokeStatic("Suffix", hwnd);
//				suffix = "64-4.0";
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
	
	public static class API_O2_Injector_ExtensionMethods
	{
		public static Process process_MainWindow_BringToFront(this Process process)
		{
			if (process.MainWindowHandle != IntPtr.Zero)
				"WindowsBase.dll".assembly()
							 	.type("UnsafeNativeMethods")					 
							 	.invokeStatic("SetForegroundWindow",new HandleRef(null, process.MainWindowHandle)) ;
			else
				"[process_MainWindow_BringToFront] provided process has no main Window".error();
			return process;
		}
		
		public static bool is64BitProcess(this Process process )
	    {
	    	if (process.isNull())
	    	{
	    		"in process.is64BitProcess provided process value was null!".error();
	    		return false;
	    	}
	        bool lIs64BitProcess = false;
	        if ( System.Environment.Is64BitOperatingSystem ) {
	            IsWow64Process( process.Handle, out lIs64BitProcess );
	        }
	        "[Is Target Process 64Bit = {0}]".debug(lIs64BitProcess);
	        return lIs64BitProcess;
	    }
	    
    	[DllImport( "kernel32.dll" )]
    	static extern bool IsWow64Process( System.IntPtr aProcessHandle, out bool lpSystemInfo );
	}
	
	
    

}