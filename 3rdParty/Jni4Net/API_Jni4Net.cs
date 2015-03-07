// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using FluentSharp.CoreLib;
using FluentSharp.WinForms;
using FluentSharp.Web35;
using java.lang;
using java.io;
using java.net;
using java.util.zip;
using java.lang.reflect;
using net.sf.jni4net;
using net.sf.jni4net.jni;

//Installer:Jni4Net_Installer.cs!Jni4Net/bin/proxygen.exe
//O2Ref:FluentSharp.Web.dll
//O2Ref:Jni4Net/lib/jni4net.n-0.8.8.0.dll

namespace O2.XRules.Database.APIs
{
	public class API_Jni4Net
	{
		public BridgeSetup 	bridgeSetup;
		public JNIEnv 		jniEnv;
	}
	
	public static class API_Jni4Net_Active_ExtensionMethods
	{
	}
	
	
	public static class API_Jni4Net_Active_ExtensionMethods_JVM
	{
	
		public static API_Jni4Net setUpBride(this API_Jni4Net jni4Net)
		{
			return jni4Net.setUpBride(new BridgeSetup(){ Verbose=true });
		}
		public static API_Jni4Net setUpBride(this API_Jni4Net jni4Net, BridgeSetup _bridgeSetup)
		{	
			jni4Net.bridgeSetup = _bridgeSetup;
			try
			{								
				Bridge.CreateJVM(jni4Net.bridgeSetup);
				jni4Net.jniEnv = JNIEnv.ThreadEnv;
				return jni4Net;
			}
			catch(System.Exception ex)
			{
				ex.log();
				return null;
			}
		}
		

		public static bool isJVMAvaiable(this API_Jni4Net jni4Net)
		{
			try
			{
				var env = JNIEnv.ThreadEnv;
				var br = env.FindClass("java/lang/Object");
				return true;
			}
			catch(System.Exception ex)
			{
				ex.log();
				return false;
			}
			
		}
	}
	public static class API_Jni4Net_Active_ExtensionMethods_ClassLoader
	{	
		public static ClassLoader systemClassLoader(this  API_Jni4Net jni4Net)
		{
			return ClassLoader.getSystemClassLoader();
		}
		public static ClassLoader addJarToSystemClassLoader(this string pathToJar)
		{
			return ClassLoader.getSystemClassLoader().loadJar(pathToJar);
		}
		public static ClassLoader loadJar(this ClassLoader classLoader, string pathToJar)
		{
			try
			{
				var addUrl = classLoader.getClass()
										.getSuperclass()
										.getDeclaredMethod("addURL", new Class[]{URL._class});
				addUrl.setAccessible(true);
				addUrl.invoke(classLoader, new java.lang.Object[]{ pathToJar.java_File().toURL() });	
			}
			catch(Exception ex)
			{
				"[ClassLoader][loadJar] failed to load file {0} , exception: {1}".error(pathToJar, ex.Message);
			}
			return classLoader;			 
		}
		public static ClassLoader loadJars(this ClassLoader classLoader, List<string> pathToJars)
		{			
			foreach(var pathToJar in pathToJars)
				classLoader.loadJar(pathToJar);
			return classLoader;				
		}
		public static List<string> jarsInClassPath(this  ClassLoader classLoader)
		{			
			var urls = classLoader.getClass().getMethod("getURLs",null).invoke(classLoader, null);			
			return urls.getElementsFromArray<URL>().toStringList().select((file)=>file.remove("file:/"));
		}
		
		public static ClassLoader java_ClassLoader_forJars(this URL[] pathsToJarFiles)
		{			
			var urlArray =  URL._class.createArray(pathsToJarFiles);
			var urlClassLoaderClass = "java.net.URLClassLoader".java_Find_Class();
			return urlClassLoaderClass.ctor<ClassLoader>(urlArray);
		}
		public static ClassLoader java_ClassLoader_forJar(this string pathToJarFile)
		{
			var url = pathToJarFile.java_File().toURL();
			var urlArray =  URL._class.createArray(url);
			var urlClassLoaderClass = "java.net.URLClassLoader".java_Find_Class();
			return urlClassLoaderClass.ctor<ClassLoader>(urlArray);
		}
		public static string java_Get_Class_Jar_File_Location(this Class targetClass)
		{
			try
			{				
				var protectionDomain = targetClass.getProtectionDomain();
				var protectionDomain_Class = protectionDomain.getClass();
				
				var getCodeSource = protectionDomain_Class.GetMethod("getCodeSource", "()Ljava/security/CodeSource;", false);
				var codeSource = getCodeSource.invoke(protectionDomain, new java.lang.Object[] {});
				var getLocation = codeSource.getClass().GetMethod("getLocation","()Ljava/net/URL;",false);
				var location = getLocation.invoke(codeSource, new java.lang.Object[] {});
				
				var toString = location.getClass().GetMethod("toString", "()Ljava/lang/String;",false)
									   			  .invoke(location,  new java.lang.Object[] {});		
				var path = toString.str().urlDecode().subString(6);					   			  
				return path;
			}
			catch(System.Exception ex)
			{
				ex.log("in getClassJarFileLocation");
				return null;
			}
		}		
		
		public static List<Class> java_From_ClassLoader_get_Loaded_Classes(this API_Jni4Net jni4Net)
		{			
			var systemClassLoader = ClassLoader.getSystemClassLoader();
			return jni4Net.java_From_ClassLoader_get_Loaded_Classes(systemClassLoader);
		}
		public static List<Class> java_From_ClassLoader_get_Loaded_Classes(this API_Jni4Net jni4Net, ClassLoader classLoader)
		{	
			var classes = new List<Class>();
			var clClass = classLoader.getClass();
			while(clClass != java.lang.ClassLoader._class)
			{
				"{0} - {1}".info(clClass.str(), clClass.getSuperclass().str());
				clClass = clClass.getSuperclass();
			}
			
			var classes_Field = clClass.getDeclaredField("classes");
			var classes_Value = classes_Field.get(classLoader);
			
			var elements_Method = classes_Value.getClass().getMethod("elements",null);
			var elements = elements_Method.invoke(classes_Value,null);
			
			
			while (elements.Invoke<bool>("hasMoreElements","()Z"))			
				classes.add(elements.Invoke<java.lang.Class>("nextElement", "()Ljava/lang/Object;"));
				
			return classes;
		}
		
		public static List<Class> loadClasses(this ClassLoader classLoader, List<string> classesToLoad)
		{
			var classes = new List<Class>();
			foreach(var classToLoad in classesToLoad)
			{
				try
				{					
					var loadedClass = classLoader.loadClass(classToLoad);
					if(loadedClass.notNull())
						classes.add(loadedClass);
					"Loaded class: {0}".info(classToLoad);
				}
				catch(System.Exception ex)
				{
					"[ClassLoader] loading class {0} threw: {1}".error(classToLoad, ex.Message);
				}
			}
			return classes;
		}
		
		public static List<ZipEntry> java_Jar_ZipEntries(this string pathToJarFile)
		{
			try
			{
				var jarFile = "java.util.jar.JarFile".java_Class().ctor(pathToJarFile.java_String());  
				return jarFile.java_Invoke("entries") 
					  	      .getElementsFromList<ZipEntry>();
			}
			catch(System.Exception ex)
			{
				ex.log("java_Jar_Contents");
				return new List<ZipEntry>();
			}
		}	
		public static List<string> java_Jar_Classes_Names(this string pathToJarFile)
		{
			return (from element in pathToJarFile.java_Jar_ZipEntries().toStringList()
					where element.ends(".class")
					select element.remove(".class").replace("/",".")).toList();
		}
				
		public static List<Class> java_Jar_Classes(this string pathToJarFile)
		{
			try
			{
				var classLoader = pathToJarFile.java_ClassLoader_forJar();
				return classLoader.loadClasses(pathToJarFile.java_Jar_Classes_Names());
			}
			catch(System.Exception ex)
			{
				"[java_Jar_Classes] loading jar {0} threw: {1}".error(pathToJarFile, ex.Message);
				return null;
			}
		}
		
		public static Class java_Jar_Class(this string pathToJarFile, string className)
		{
			try
			{
				var classLoader = pathToJarFile.java_ClassLoader_forJar();
				return classLoader.loadClass(className);
			}
			catch(System.Exception ex)
			{
				"[java_Jar_Classes] loading jar {0} threw: {1}".error(pathToJarFile, ex.Message);
				return null;
			}
		}
	}
	
	public static class API_Jni4Net_Active_ExtensionMethods_Reflection
	{		
		public static Class java_Class(this string className)
		{
			return className.java_Find_Class();
		}
		public static Class java_Find_Class(this string className)
		{
			try
			{
				return JNIEnv.ThreadEnv.FindClass(className.replace(".", "/"));
			}
			catch(System.Exception ex) 
			{
				ex.log();
				return null;
			}
		}					
		public static java.lang.Object java_Invoke(this java.lang.Object liveObject, string methodName,params java.lang.Object[] methodParameters)
		{
			return liveObject.java_Invoke_UsingSignature(methodName, null, methodParameters);
		}
		public static java.lang.Object java_Invoke_UsingSignature(this java.lang.Object liveObject, string methodName, string signature, params java.lang.Object[] methodParameters)
		{
			try
			{
				var method = (signature.isNull()) 
								? liveObject.getClass().getMethod(methodName,null)
								: liveObject.getClass().GetMethod(methodName,signature,false);
				var result = method.invoke(liveObject, methodParameters);
				return result;
			}
			catch(System.Exception ex) 
			{
				ex.log();
				return null;
			}
		}
		public static List<Constructor> ctors(this Class targetClass)
		{
			return targetClass.getConstructors().toList();
		}		
		public static Object ctor(this Class targetClass, params Object[] parameters)
		{
			var ctor = targetClass.getConstructor(parameters.objectClasses());
			if(ctor.notNull())			
				return ctor.newInstance(parameters);
			return null;
		}
		public static T ctor<T>(this Class targetClass, params Object[] parameters) where T : Object
		{
			var newObject = targetClass.ctor(parameters);
			return newObject as T;
		}		
		public static Class[] objectClasses(this Object[] javaObjects)
		{
			/*var classes = new List<Class>();
			foreach(var javaObject in javaObjects)
				classes.Add(javaObject.getClass());
			return classes.ToArray();*/
			return (from javaObject in javaObjects				// not working
					select javaObject.getClass()).ToArray();
		}		
		public static Method method(this Object targetObject, string methodName)
		{
			return targetObject.getClass().getMethod(methodName,null);
		}
	}
	
	public static class API_Jni4Net_Active_ExtensionMethods_ClassHelpers
	{
		public static String java_String(this string value)
		{
			return JNIEnv.ThreadEnv.NewString(value);
		}
		public static java.lang.Object createArray(this Class targetClass, params java.lang.Object[] values)
		{
			var array = JNIEnv.ThreadEnv.NewObjectArray(values.size(), targetClass, null);
			for(int i=0; i < values.size(); i ++)
				JNIEnv.ThreadEnv.SetObjectArrayElement(array, i, values[i]);
			return array;
		}
		public static File java_File(this string pathToFile)
		{
			return new File(pathToFile);
		}
		public static URL java_Url(this string path)
		{			
			return new URL(path);
		}
		public static List<Object> getElementsFromList(this Object list)
		{
			return list.getElementsFromList<Object>();
		}
		public static List<T> getElementsFromList<T>(this Object list) where T : Object
		{
			var hasMoreElements = list.method("hasMoreElements");
			var nextElement     = list.method("nextElement");
			if (hasMoreElements.notNull() && nextElement.notNull())
			{
				var elements = new List<T>();
				while((bool)(java.lang.Boolean)hasMoreElements.invoke(list, null))
				{
					var value = nextElement.invoke(list, null);
					if (value is T)
						elements.Add((T)value);
				}
				return elements;
			}
			"[getElementsFromArray] provided object did not have the hasMoreElements and nextElement objects".error();
			return null;
		}
		
		public static List<Object> getElementsFromArray(this Object array)
		{
			return array.getElementsFromArray<Object>();
		}
		public static List<T> getElementsFromArray<T>(this Object array) where T : Object
		{			
			if (array.getClass().isArray().isFalse())
			{
				"[getElementsFromArray] provided object is not an java array".error();
				return null;
			}
						
			var getArray = "java.lang.reflect.Array".java_Class()
												    .getMethods()
													.Where((method)=>method.getName() =="get").first();
			
			var arrayValues = new List<T>();
			
			for(int i = 0 ; i < (int)JNIEnv.ThreadEnv.invoke("GetArrayLength", array) ; i++)
			{	
				var index = new java.lang.Integer(i);
				var value = (T)getArray.invoke(null, new java.lang.Object[] { array, index});
				arrayValues.Add(value);
				
			}
			return arrayValues;
		}
		
	}	
	
	
	
	public static class API_Jni4Net_Active_ExtensionMethods_GUI_Helpers
	{
	
		public static TreeView java_SetTreeView_To_Show_Jni4Net_Reflection_Data(this TreeView treeView)
		{						
			treeView.beforeExpand<java.lang.Class>(
				(treeNode, @class)=>{
										try
										{
											var ctors = @class.getDeclaredConstructors();
											var methods = @class.getDeclaredMethods();
											var fields = @class.getDeclaredFields();
											var superClass = @class.getSuperclass();
											//var annotations = @class.getDeclaredAnnotations();			
											if (superClass.notNull()) treeNode.add_Node("SuperClass")
																			  .add_Node(superClass.getName(), superClass, true).foreColor(Color.Orange);;
											 
											if (ctors.size() > 0)
												treeNode.add_Node("ctors ({0})".format(ctors.size()), ctors,true);
											if (methods.size() > 0)			
												treeNode.add_Node("methods ({0})".format(methods.size()), methods,true);										
											if (fields.size() > 0)		
												treeNode.add_Node("fields ({0})".format(fields.size()), fields,true);										
										}
										catch(System.Exception ex)
										{
											ex.log("[java_SetTreeView_To_Show_Jni4Net_Reflection_Data] before expand: ");
										}
									});
									
			treeView.beforeExpand<java.lang.reflect.Method[]>(
				(treeNode,methods)=>{
										treeNode.add_Nodes(methods, (method)=>"{0}           {1}".format(method.getName(),method.GetSignature()), true);
										treeNode.nodes().colorNodes(Color.Blue);
								    });
			treeView.beforeExpand<java.lang.reflect.Constructor[]>(
				(treeNode,ctors) => {
										treeNode.add_Nodes(ctors, (ctor)=>"{0}           {1}".format(ctor.getName(),ctor.GetSignature()), true);
										treeNode.nodes().colorNodes(Color.Blue);
								    });
								    
			treeView.beforeExpand<java.lang.reflect.Field[]>(
				(treeNode,fields) => {
										treeNode.add_Nodes(fields, (field)=>"{0}           {1}".format(field.getName(),field.GetSignature()), true);
										treeNode.nodes().colorNodes( Color.Blue);
								    });
								    
			treeView.beforeExpand<java.lang.reflect.Method>(					  
				(treeNode,method)=>{
										treeNode.add_Node("ToString: {0}".format(method.toGenericString()), method).foreColor(Color.DarkGray);
										treeNode.add_Node("Java Signature: {0}".format(method.GetSignature()), method).foreColor(Color.DarkGray);;
										var parameterNode = treeNode.add_Node("_Parameters");
										parameterNode.add_Nodes(method.getParameterTypes(), (@class)=>@class.getName(),true);
										parameterNode.nodes().colorNodes( Color.DarkOrange);
										treeNode.add_Node("_ReturnType").add_Node(method.getReturnType().getName(), method.getReturnType(),true)
																		 .foreColor(Color.DarkOrange);
								    });
			
			treeView.beforeExpand<java.lang.reflect.Constructor>(					  
				(treeNode,ctor)=>{
										treeNode.add_Node("ToString: {0}".format(ctor.toGenericString()), ctor).foreColor(Color.DarkGray);
										treeNode.add_Node("Java Signature: {0}".format(ctor.GetSignature()), ctor).foreColor(Color.DarkGray);;
										var parameterNode = treeNode.add_Node("_Parameters");
										parameterNode.add_Nodes(ctor.getParameterTypes(), (@class)=>@class.getName(),true);
										parameterNode.nodes().colorNodes( Color.DarkOrange);							
										
								    });
			
			treeView.beforeExpand<java.lang.reflect.Field>(					  
				(treeNode,field)=>{
										treeNode.add_Node("ToString: {0}".format(field.toGenericString()), field).foreColor(Color.DarkGray);
										treeNode.add_Node("Java Signature: {0}".format(field.GetSignature()), field).foreColor(Color.DarkGray);;
								    });
			return treeView;								    
		}	
	}
	
	public static class API_Jni4Net_Active_ExtensionMethods_Collections
	{
		public static List<string> toStringList<T>(this List<T> list) where T : Object
		{
			var converted = new List<string>();
			foreach(var item in list)
				converted.Add(item.str());
			return converted;
		}
	}
}