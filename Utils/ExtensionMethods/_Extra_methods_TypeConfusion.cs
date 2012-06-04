// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Forms;
using System.Collections;  
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;

//O2File:DynamicTypes.cs 
//O2File:_Extra_methods_Misc.cs
//O2File:_Extra_methods_Reflection.cs

namespace O2.XRules.Database.Utils
{	
		
	public class TypeConfusion
	{
		public static string create_DLL_TO_castViaTypeConfusion()
		{
			var assemblyName = "TypeConfusion";
			var dllName = assemblyName + ".dll";
			var appDomain = AppDomain.CurrentDomain;
			var targetDir = "".tempDir(false);			
			var assemblyBuilder = assemblyName.assemblyBuilder_forSave(targetDir);  // use this if wanting to Save the assembly created
			var moduleBuilder = assemblyBuilder.dynamicModule(dllName);			
			var typeBuilder = moduleBuilder.dynamicType(assemblyName);
			
			var methodBuilder = typeBuilder.dynamicMethod("castObjectIntoType", null, typeof(object));
			
			var genericParameters = methodBuilder.DefineGenericParameters("T");
			var returnType = genericParameters[0];
			methodBuilder.SetReturnType(returnType);
			
			var ilGenerator = methodBuilder.il();
			ilGenerator.DeclareLocal(typeof(object));
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Stloc_0);
			ilGenerator.Emit(OpCodes.Ldloc_0);
			ilGenerator.ret();
			
			var type = typeBuilder.create();
			
			assemblyBuilder.Save(dllName);
			return targetDir.pathCombine(dllName);			
		}
	}
	public static class _extra_TypeConfusion_ExtensionMethods
	{		
		
		//this one doesn't work all the time	
		public static T castViaTypeConfusion<T>(this object _objectToCast)
		{
			var assemblyName = "TypeConfusion";
			var dllName = assemblyName + ".dll";
			var appDomain = AppDomain.CurrentDomain;
			var assemblyBuilder = assemblyName.assemblyBuilder();
			//var assemblyBuilder = assemblyName.assemblyBuilder_forSave(targetDir);  // use this if wanting to Save the assembly created
			var moduleBuilder = assemblyBuilder.dynamicModule(dllName);			
			var typeBuilder = moduleBuilder.dynamicType(assemblyName);
			
			var methodBuilder = typeBuilder.dynamicMethod("castObjectIntoType", null, typeof(object));
			
			var genericParameters = methodBuilder.DefineGenericParameters("T");
			var returnType = genericParameters[0];
			methodBuilder.SetReturnType(returnType);
			
			var ilGenerator = methodBuilder.il();
			ilGenerator.DeclareLocal(typeof(object));
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Stloc_0);
			ilGenerator.Emit(OpCodes.Ldloc_0);
			ilGenerator.ret();
			
			var type = typeBuilder.create();
			
			//assemblyBuilder.Save(dllName);
			
			var liveObject = type.ctor();
			var method = type.method("castObjectIntoType");						 
			var generic = method.MakeGenericMethod(typeof(T));
			
			return (T)generic.Invoke(liveObject,new object[] { _objectToCast}); 
		}
		
		public static T storeObjectInTag<T>(this Control control, Func<T> ctorCallback)
		{
			if (control.Tag.notNull())
				try
				{
					return control.Tag.castViaTypeConfusion<T>();
				}
				catch(Exception ex)
				{
					"[storeObjectInTag] failed casting exiting object: {0}".error(ex.Message);
				}			
			var _object = ctorCallback();
			control.Tag = _object;
			return _object;						
		}
		
		public static T o2CacheViaTypeConfusion<T>(this string cacheKey, Func<T> ctorCallback)
		{
			if (cacheKey.liveObject().notNull())
				try
				{
					return cacheKey.liveObject().castViaTypeConfusion<T>();
				}
				catch(Exception ex)
				{
					"[o2CacheViaTypeConfusion] failed casting exiting object: {0}".error(ex.Message);
				}			
			var _object = ctorCallback();
			cacheKey.liveObject(_object);
			return _object;		
		}
	}
}
    	