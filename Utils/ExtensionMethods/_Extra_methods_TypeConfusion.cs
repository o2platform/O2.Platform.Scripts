// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Windows.Forms;
using FluentSharp.CoreLib;

//O2Tag_DontAddExtraO2Files
//O2Ref:TypeConfusion.dll

namespace O2.XRules.Database.Utils
{	
			
	public static class _extra_TypeConfusion_ExtensionMethods
	{			
		public static T castViaTypeConfusion<T>(this object _objectToCast)
		{
			return (T)new TypeConfusion().castObjectIntoType<T>((object)_objectToCast);
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


        /*public class TypeConfusion
    {
        public static string create_DLL_TO_castViaTypeConfusion()
        {
            var assemblyName = "TypeConfusion";
            var dllName = assemblyName + ".dll";
            var appDomain = AppDomain.CurrentDomain;
            var targetDir = "".tempDir(false);		
            var targetFile = targetDir.pathCombine(dllName);
            if (targetFile.fileExists())
                return targetFile;
				
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
            return targetFile;
        }
    }*/
		/*
		//also doesn't work
		public static T castViaTypeConfusion<T>(this object _objectToCast)
		{
			var assembly = TypeConfusion.create_DLL_TO_castViaTypeConfusion().assembly();
			var typeConfusion = assembly.type("TypeConfusion");
			var method = typeConfusion.method("castObjectIntoType");
			
			try
			{
				var liveObject = typeConfusion.ctor();				
				method = method.MakeGenericMethod(new Type[] { typeof(T) });	
				
				return (T)castedObject;
			}
			catch(Exception ex)
			{
				ex.log("in castViaTypeConfusion");
				return default(T);
			}
		}
		*/
	}
}
    	