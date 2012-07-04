using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using Microsoft.Samples.Debugging.MdbgEngine;
using Microsoft.Samples.Debugging.CorDebug;
using Microsoft.Samples.Debugging.CorMetadata;
using Microsoft.Samples.Debugging.CorDebug.NativeApi;

//O2File:_Extra_methods_Reflection.cs

//O2Ref:MDbg_Sample\MDbg\mdbgeng.dll
//O2Ref:MDbg_Sample\MDbg\corapi.dll 
//O2Ref:MDbg_Sample\MDbg\mdbgeng.dll  
//O2Ref:MDbg_Sample\MDbg\mdbg.exe
//O2Ref:MDbg_Sample\MDbg\mdbgext.dll
//O2Ref:MDbg_Sample\MDbg\raw.dll

namespace O2.XRules.Database.Utils
{
		
	public static class MDbg_ExtensionMethods_MDbgEngine
	{
		public static MDbgEngine go(this MDbgEngine engine)
		{
			engine.activeProcess().Go();
			return engine;
		}
		
		public static MDbgEngine stop(this MDbgEngine engine)
		{
			return engine.@break();
		}
		public static MDbgEngine @break(this MDbgEngine engine)
		{
			engine.activeProcess().AsyncStop().WaitOne();
			return engine;
		}
		
		public static MDbgEngine detach(this MDbgEngine engine)
		{
			if (engine.hasActiveProcess())
				engine.process().Detach().WaitOne();
			return engine;
		}
		
	}
	
    public static class MDbg_ExtensionMethods_Process
    {	
    	public static MDbgEngine startProcess(this MDbgEngine engine, string exeToDebug, string arguments = "")
    	{
    		engine.CreateProcess(exeToDebug, arguments, DebugModeFlag.Debug, null);
    		return engine;
    	}
    	
    	public static bool hasActiveProcess(this MDbgEngine engine)
    	{
    		return engine.activeProcess().notNull();
    	}
    	
    	public static MDbgProcess process(this MDbgEngine engine)
    	{
    		return engine.activeProcess();
    	}
    	
		public static MDbgProcess activeProcess(this MDbgEngine engine)
		{
			if (engine.Processes.size() > 0)
				return engine.Processes.Active;
			return null;
		}
		
		public static MDbgEngine goAndWait(this MDbgEngine engine)
		{
			engine.activeProcess().goAndWait();
			return engine;
		}
		
		public static MDbgProcess goAndWait(this MDbgProcess process)
		{
			process.Go().WaitOne();
			return process;
		}
		
		public static MDbgEngine stop_Process(this MDbgEngine engine)
		{
			if (engine.activeProcess().notNull())
			{
				"[MDbgEngine] in stop_Process".info();
				engine.@break();
				engine.activeProcess().Kill();
			}
			return engine;
		}
		
		public static MDbgEngine process_StopInNSeconds(this MDbgEngine engine, int seconds)
		{
			O2Thread.mtaThread(
				()=>{		
						engine.sleep(seconds * 1000);
						engine.stop_Process();							  
					});
			return engine;
		}		
    }
    public static class MDbg_ExtensionMethods_Process_Attach
    {
    	public static MDbgEngine attach(this MDbgEngine engine, Process process)
    	{
    		if (process.isNull())
    		{
    			"[MDbgEngine] in attach provided process object was null".error();
    			return engine;
    		}
    		return engine.attach(process.Id);
    	}
    	
    	public static MDbgEngine attach(this MDbgEngine engine, int processId)
    	{
    		engine.Attach(processId);
    		engine.goAndWait();
    		return engine;
    	}
    	
    	public static List<Process> attachableProcesses(this MDbgEngine engine)
    	{
	    	var attachableProcesses = new List<Process>();
			foreach (var process in Process.GetProcesses())
	        {
	            if (Process.GetCurrentProcess().Id == process.Id)  // let's hide our process                
	                continue;
	
	            CLRMetaHost mh = null;
	            try
	            {
	                mh = new CLRMetaHost();
	            }
	            catch (System.EntryPointNotFoundException)
	            {
	                continue;
	            }
	
	            IEnumerable<CLRRuntimeInfo> runtimes = null;
	            try
	            {
	                runtimes = mh.EnumerateLoadedRuntimes(process.Id);
	            }                
	            catch
	            {                    
	                continue;
	            }
	 
	            //if there are no runtimes in the target process, don't print it out
	            if (!runtimes.GetEnumerator().MoveNext())                
	                continue;
	            attachableProcesses.add(process);    
	        }
	        return attachableProcesses;
		}
		
		public static string clrDetails(this List<Process> clrProcesses)
		{
			var clrDetails = "";
			foreach(var clrProcess in clrProcesses)
			{
				var id = clrProcess.Id;
				var name = clrProcess.ProcessName;
				var fileName =  clrProcess.MainModule.FileName;				 
				var runtimes = new CLRMetaHost().EnumerateLoadedRuntimes(id);
				var versions = (from runtime in runtimes
								select runtime.GetVersionString()).toList().join(",");
				clrDetails += "id: {0} \t name: {1} \t runtimes: {2} \t fileName: {3}".line().format(id, name, versions, fileName);
			}
			return clrDetails;
		}
	}	
	
    
    
    public static class MDbg_ExtensionMethods_Threads
    {
    	public static MDbgThread activeThread(this MDbgEngine engine)
    	{
    		return engine.activeProcess().Threads.Active;
    	}
    	
    	public static List<MDbgThread> threads(this MDbgEngine engine)
    	{
    		var threads = new List<MDbgThread>();
    		foreach(MDbgThread thread in engine.activeProcess().Threads)
    			threads.add(thread);
    		return threads;    			
    	}
    }
    
    public static class MDbg_ExtensionMethods_Modules
    {		
    	public static MDbgModule module(this  MDbgEngine engine, string moduleName)
    	{
    		return engine.activeProcess().Modules.Lookup(moduleName);
    	}
    	public static List<MDbgModule> modules(this MDbgEngine engine)
		{
			var modules = new List<MDbgModule>();			
			foreach(MDbgModule module in engine.activeProcess().Modules)
				modules.Add(module);
			return modules;
		}
		
		public static List<string> names(this List<MDbgModule> modules)
		{
			return (from module in modules
					select module.CorModule.Name).toList();
		}
	}
	
	public static class MDbg_ExtensionMethods_Types
	{
		public static List<MetadataType> types(this MDbgModule module)
		{
			var types = new List<MetadataType>();		
			foreach( MetadataType type in module.Importer.DefinedTypes)
				types.add(type);
			return types;
		}
		
		public static MetadataType type(this MDbgModule module, string typeName)
		{
			return (from type in module.types()
					where type.Name == typeName
					select type).first();
		}
	}
	
	public static class MDbg_ExtensionMethods_Methods
	{			
		public static List<MethodInfo> methods(this MetadataType type)
		{
			//the bindingAttr is not used
			return type.GetMethods(BindingFlags.Default).toList();
		}
		
		public static List<MethodInfo> methods(this MetadataType type, string methodName)
		{
			return (from method in type.methods()
					where method.Name == methodName
					select method).toList();
		}
		
		public static MethodInfo method(this MetadataType type, string methodName)
		{
			return (from method in type.methods()
					where method.Name == methodName
					select method).first();
		}	
		
		public static CorFunction corFunction(this MethodInfo type, MDbgModule module)
		{
			return module.CorModule.GetFunctionFromToken(type.MetadataToken);
		}
		
	}
	
	public static class MDbg_ExtensionMethods_Eval
	{
		public static CorEval corEval(this MDbgEngine engine)
		{
			return engine.activeThread().CorThread.CreateEval();
		}
		
		public static CorValue[] corValues(this MDbgEngine engine, params string[] stringValues)
		{
			var corValues = new List<CorValue>();
			foreach(var stringValue in stringValues)
				corValues.add(engine.create_String(stringValue));
			return corValues.ToArray();	
		}
		public static CorValue create_String(this MDbgEngine engine, string stringValue)
		{
			CorEval eval = engine.corEval();
      		eval.NewString(stringValue);      
      		engine.goAndWait();
      		CorValue corValue = (engine.activeProcess().StopReason as EvalCompleteStopReason).Eval.Result;
      		return corValue;
		}
		
		public static CorGenericValue create_Char(this MDbgEngine engine, char value)
		{
			return engine.create_Object(CorElementType.ELEMENT_TYPE_CHAR, value);
		}
		
		public static CorGenericValue create_Bool(this MDbgEngine engine, bool value)
		{
			return engine.create_Object(CorElementType.ELEMENT_TYPE_BOOLEAN, value);
		}
		
		public static CorGenericValue create_Object(this MDbgEngine engine, CorElementType elementType, object value)
		{
			var corEval = engine.corEval();
			var corClass = engine.process().ResolveClass(value.typeFullName());
			var corValue = corEval.CreateValue(elementType , corClass);
 			var genericValue = corValue.CastToGenericValue();
 			return genericValue.value(value); 			
 		}
 		
 		public static T value<T>(this CorGenericValue genericValue)
 		{
 			var value = genericValue.GetValue();
 			if (value is T)
 				return (T)value;
 			return default(T);
 		}
 		
 		public static object value(this CorGenericValue genericValue)
 		{
 			return genericValue.GetValue();
 		}
 		
 		public static CorGenericValue value(this CorGenericValue genericValue, object value)
		{
			genericValue.SetValue(value);
			return genericValue;
		}
		
		
		public static MDbgEngine invoke_Method(this MDbgEngine engine,  CorFunction function, params string[] stringValues)
		{
			var corValues = engine.corValues(stringValues);
			return engine.invoke_Method(function, corValues);
		}
		
		public static MDbgEngine invoke_Method(this MDbgEngine engine, string moduleName, string typeName, string methodName, params string[] stringValues)
		{
			var corValues = engine.corValues(stringValues);
			return engine.invoke_Method(moduleName, typeName, methodName, corValues);
		}
		
		public static MDbgEngine invoke_Method(this MDbgEngine engine, string moduleName, string typeName, string methodName, CorValue[] parameters)
		{
			var module = engine.module(moduleName);
			var function = engine.activeProcess().ResolveFunctionName(module, typeName, methodName);
			return engine.invoke_Method(function.CorFunction, parameters);
		}
		
		public static MDbgEngine invoke_Method(this MDbgEngine engine, CorFunction function, CorValue[] parameters)
		{
			try
			{
	    		var eval = engine.corEval();
	    		eval.CallFunction(function, parameters);
				engine.goAndWait();
			}
			catch(Exception ex)
			{
				"[MDbgEngine] invoke_Method: {0}".error(ex.Message);
			}
			return engine;
		}
		
		public static MDbgEngine invoke_Method(this MethodInfo methodInfo, MDbgEngine engine, params string[] stringValues)
		{
			var corValues = engine.corValues(stringValues);
			return methodInfo.invoke_Method(engine, corValues);
		}
		
		public static MDbgEngine invoke_Method(this MethodInfo methodInfo, MDbgEngine engine, CorValue[] parameters)
		{
		//	var methodInfo = debugger.module("mscorlib").type("System.Console").method("Write");		
			var module = engine.process().ResolveClass(methodInfo.DeclaringType.FullName).Module;
			var corFunction = module.GetFunctionFromToken(methodInfo.MetadataToken);
			return engine.invoke_Method(corFunction,parameters);
		}

	}
	
	public static class MDbg_ExtensionMethods_Eval_Helpers
	{
		public static MDbgEngine console_WriteLine(this MDbgEngine engine)
		{
			return engine.invoke_Method("mscorlib", "System.Console","WriteLine");
		}
		
		public static MDbgEngine console_WriteLine(this MDbgEngine engine, string message)
		{
			typeof(Console).method_bySignature("Void WriteLine(System.String)")
					       .invoke_Method(engine, message);
			return engine;		      
		}			     
					   
		
	}
}