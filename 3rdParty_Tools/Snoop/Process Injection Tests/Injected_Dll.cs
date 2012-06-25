// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms; 
using System.Text;

namespace O2.Script
{
    public class Test
    {    
    	static Process CurrentProcess = System.Diagnostics.Process.GetCurrentProcess();
    	static string  assemblyLocation = @"E:\O2_V4\O2.Platform.Projects\binaries\O2_FluentSharp_CoreLib.dll";
		static Assembly assembly = Assembly.LoadFrom(assemblyLocation);
			
/*		public static void invokeStatic(Assembly assembly, string method, string )
    	{
    		var type = assembly.GetType("O2.Kernel.PublicDI");
			var method = type.GetMethod("get_log");
			var kConfig = method.Invoke(null, new object[] { });
			var info = kConfig.GetType().GetMethod("i");
			info.Invoke(kConfig, new object[] { message});
    	}*/
		
    	public static void info(string message)
    	{
    		var type = assembly.GetType("O2.Kernel.PublicDI");
			var method = type.GetMethod("get_log");
			var kConfig = method.Invoke(null, new object[] { });
			var info = kConfig.GetType().GetMethod("i");
			info.Invoke(kConfig, new object[] { message});
    	}    	
    	
    	public static void startProcess(string exe)
    	{
    		var type = assembly.GetType("O2.Kernel.exec");
			var obj = Activator.CreateInstance(type);
			var cmd = type.GetMethods()[0];				
			cmd.Invoke(obj, new object[] { exe });
    	}
    	
    	public static void listLoadedAssemblies()
    	{
    		foreach(ProcessModule module in CurrentProcess.Modules)
    		{
    			info(module.ModuleName);
    		}	
    	}
    	
    	public static Assembly loadAssembly(string path)
    	{
    		return loadAssembly(path, true);
    	}
    	
    	public static Assembly loadAssembly(string path, bool fromGac)
    	{
//    		info("Loading assembly " + path);			
    		try
    		{	
    			var assembly = (fromGac) ? Assembly.Load(path)
    									 : Assembly.LoadFrom(path);		        			
				if (assembly!= null)    		
	    			info(string.Format("Loaded Assembly : {0}", assembly));
	    		else
	    			info(string.Format("Error: Failed to load Assembly : {0}", assembly));    			
	    		return assembly;
	    	}
	    	catch(Exception ex)
	    	{ 
	    		info("Error: loadAssembly: " + ex.Message);
	    	}
    		return null;
    	}
    	
    	public static Assembly compileFile(string file)
    	{
			var compileEngineType = assembly.GetType("O2.DotNetWrappers.DotNet.CompileEngine");
			var compileEngine = Activator.CreateInstance(compileEngineType);
			var compileSourceFile = compileEngineType.GetMethod("compileSourceFile");
			var compiledAssembly = (Assembly)compileSourceFile.Invoke(compileEngine, new object[] {file});
			if (compiledAssembly != null)
				info("Compiled file ok to: " + compiledAssembly.Location);
			return compiledAssembly;

    	}
    	
    	public static void executeFirstMethod(Assembly _assembly)
    	{
    		if (_assembly != null)
    		{
    			var method = _assembly.GetTypes()[0].GetMethods()[0];//.ToString();
				method.Invoke(null, new object[] {});
			}
    	}
    	
    	public static void injectO2Script()
    	{
    		info("Injecting O2 REPL editor");
			var file = @"E:\O2_V4\O2.Platform.Scripts\Utils\O2\ascx_Quick_Development_GUI.cs.o2";
			var compiledAssembly = compileFile(file);
			executeFirstMethod(compiledAssembly);
			info("Injected O2 REPL editor");
    	}
    	
		public static string GoBabyGo()  
		{
			try
			{
				Debug.Write(">> Inside GoBabyGo method in Process " +CurrentProcess.ProcessName);
				injectO2Script();
				//listLoadedAssemblies();
				//info("Starting nodepad");
			//	startProcess("notepad.exe");
				//return new O2Code().test();
				
				return ">> done";				
			}
			catch(Exception ex)
			{
				info("Error: " + ex.Message);
				return "Post Error: " + ex.Message;
			}
			
			
			
			//MessageBox.Show("this is from another dll");
			//var process= Processes.getCurrentProcess();
			//"Util - O2 Available scripts.h2".local().executeH2Script();
			//"ascx_Quick_Development_GUI.cs.o2".local().executeFirstMethod();
			//"aaa".popupWindow().add_LogViewer();			
			//return "".applicationWinForms().size().str();;
			//return "P ID {0} with title: {1}".format(process.Id,process.MainWindowTitle);			
		}
    }
}
