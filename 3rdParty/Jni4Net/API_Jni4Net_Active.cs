// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using O2.Kernel; 
using O2.DotNetWrappers.ExtensionMethods;
using java.lang;
using net.sf.jni4net;
using net.sf.jni4net.jni;
//Installer:Jni4Net_Installer.cs!Jni4Net/bin/proxygen.exe
//O2Ref:Jni4Net/lib/jni4net.n-0.8.7.0.dll

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs

namespace O2.XRules.Database.APIs
{
	public class API_Jni4Net_Active
	{
		
	}
	
	public static class API_Jni4Net_Active_ExtensionMethods
	{
		public static bool isJVMAvaiable(this API_Jni4Net_Active jni4Net)
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
		
		public static java.lang.Object java_Invoke(this java.lang.Object liveObject, string method, string signature, params java.lang.Object[] methodParameters)
		{
			try
			{
				var result = liveObject.getClass().GetMethod(method,signature,false)
				  					   .invoke(liveObject, methodParameters);
				return result;
			}
			catch(System.Exception ex) 
			{
				ex.log();
				return null;
			}
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
		
		public static List<Class> java_From_ClassLoader_get_Loaded_Classes(this API_Jni4Net_Active jni4Net)
		{
			var classes = new List<Class>();
			var classLoader = ClassLoader.getSystemClassLoader();
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
	}
	
	
	public static class API_Jni4Net_Active_ExtensionMethods_GUI_Helpers
	{
		public static TreeView java_SetTreeView_To_Show_Jni4Net_Reflection_Data(this TreeView treeView)
		{						
			treeView.beforeExpand<java.lang.Class>(
				(treeNode, @class)=>{
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
}