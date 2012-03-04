// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Xml;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;  
using System.Collections.Generic;
using System.ComponentModel;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;

//O2File:_Extra_methods_Items.cs
//O2File:_Extra_methods_Files.cs

namespace O2.XRules.Database.Utils
{	
	
	public static class _Extra_Reflection_ExtensionMethods
	{
		//Reflection				
		public static object field<T>(this object _object, string fieldName)
		{
			var type = typeof(T);  
			return _object.field(type, fieldName);
		}
		
		public static object field(this object _object, Type type, string fieldName)
		{			
			"**** type:{0}".error(type.typeName());
			var fieldInfo =  (FieldInfo)type.field(fieldName);
			return PublicDI.reflection.getFieldValue(fieldInfo, type);
		}
		
		public static Type fieldValue(this Type type, string fieldName, object value)
		{
			var fieldInfo = (FieldInfo)type.field(fieldName);			
			PublicDI.reflection.setField(fieldInfo, fieldInfo.ReflectedType, value);
			return type;
		}
		
		public static List<ConstructorInfo> ctors(this Type type)
		{
			return type.GetConstructors(System.Reflection.BindingFlags.NonPublic | 
									    System.Reflection.BindingFlags.Public | 
									    System.Reflection.BindingFlags.Instance).toList();
		}
		


		
		public static T property<T>(this object _object, string propertyName)
		{						
			if (_object.notNull())
			{
				var result = _object.property(propertyName);
				if (result.notNull())
					return (T)result;
			}
			return default(T);
		}
		
		public static object property(this object _object, string propertyName, object value)
		{
			var propertyInfo = PublicDI.reflection.getPropertyInfo(propertyName, _object.type());  
			if (propertyInfo.isNull())
				"Could not find property {0} in type {1}".error(propertyName, _object.type());  
			else
			{
				PublicDI.reflection.setProperty(propertyInfo, _object,value);    
		//		"set {0}.{1} = {2}".info(_object.type(),propertyName, value);
			}
			return _object;

		}
		
		public static List<string> names(this List<PropertyInfo> properties)
		{
			return (from property in properties
					select property.Name).toList();
		}
		
		public static List<object> propertyValues(this object _object)
		{
			var propertyValues = new List<object>();
			var names = _object.type().properties().names();
			foreach(var name in names)
				propertyValues.Add(_object.prop(name));
			return propertyValues;
		}
		
		public static MethodInfo method(this Type type, string name)
		{
			foreach(var method in type.methods())
			{				
				if (method.Name == name)
					return method;
			}
			return null;
		}
		
		
		public static Thread invokeStatic_StaThread(this MethodInfo methodInfo)
		{
			return methodInfo.invokeStatic_StaThread(null);
		}
		
		public static Thread invokeStatic_StaThread(this MethodInfo methodInfo, params object[] invocationParameters)
		{
			return O2Thread.staThread(()=>methodInfo.invokeStatic(invocationParameters));
		}
		
		public static object invokeStatic(this MethodInfo methodInfo)
		{
			return methodInfo.invokeStatic(null);
		}
		
		public static object invokeStatic(this MethodInfo methodInfo, params object[] invocationParameters)
		{	
			if (invocationParameters.notNull())
				return PublicDI.reflection.invokeMethod_Static(methodInfo, new object[] { invocationParameters});
			
			return PublicDI.reflection.invokeMethod_Static(methodInfo);
		}
		
		
		public static List<System.Attribute> attributes(this List<MethodInfo> methods)
		{
			return (from method in methods 
					from attribute in method.attributes()
					select attribute).toList();
		}
		public static List<System.Attribute> attributes(this MethodInfo method)
		{
			return PublicDI.reflection.getAttributes(method);
		}
		
		public static List<MethodInfo> withAttribute(this Assembly assembly, string attributeName)
		{
			return assembly.methods().withAttribute(attributeName);
		}
		
		public static List<MethodInfo> withAttribute(this List<MethodInfo> methods, string attributeName)
		{ 
			return (from method in methods 
					from attribute in method.attributes()		  
					where attributeName == (attribute.TypeId as Type).Name.remove("Attribute")
					select method).toList();						
		}
		
		public static List<MethodInfo> methodsWithAttribute<T>(this Assembly assembly)
			where T : Attribute
		{
			return assembly.methods().withAttribute<T>();
		}
		
		public static List<MethodInfo> withAttribute<T>(this List<MethodInfo> methods)
			where T : Attribute
		{
			return (from method in methods 
					from attribute in method.attributes()		  
					where attribute is T
					select method).toList();						
		}

		
		public static string signature(this MethodInfo methodInfo)
		{
			return "mscorlib".assembly()
							 .type("RuntimeMethodInfo")
							 .invokeStatic("ConstructName",methodInfo)
							 .str();
		}
		
		public static object enumValue(this Type enumType, string value)
		{
			return enumType.enumValue<object>(value);
		}
		public static T enumValue<T>(this Type enumType, string value)
		{
			var fieldInfo = (FieldInfo) enumType.field(value);
			if (fieldInfo.notNull())
				return (T)fieldInfo.GetValue(enumType);
			return default(T);
		}
		
		public static List<AssemblyName> referencedAssemblies(this Assembly assembly)
		{
			return assembly.GetReferencedAssemblies().toList();
		}
		
		public static bool isAssemblyName(this string _string)
		{
			return _string.assemblyName().notNull();
		}
		
		public static AssemblyName assemblyName(this string _string)
		{
			try
			{
				return new System.Reflection.AssemblyName(_string);
			}
			catch(Exception ex)
			{
				"[assemblyName] {0}".error(ex.Message);
				return null;
			}
		}
		
		public static List<string> names(this List<AssemblyName> assemblyNames)
		{
			return (from assemblyName in assemblyNames
					select assemblyName.name()).toList();
		}
		
		public static string name(this AssemblyName assemblyName)
		{
			if(assemblyName.notNull())
				return assemblyName.Name;
			return null;
		}
					
		public static Assembly assembly(this AssemblyName assemblyName)
		{
			return assemblyName.str().assembly();
		}
		
		public static List<MethodInfo> methods(this List<Type> types)
		{
			return (from type in types
					from method in type.methods()
					select method).toList();					
		}
		
		public static string assembly_Location(this string assemblyName)
		{
			return assemblyName.assembly().location();
		}
		
		public static string location(this Assembly assembly)
		{
			if(assembly.notNull())
				return assembly.Location;
			return null;
		}
		
		public static List<T> attributes<T>(this MethodInfo methods)
			where T : Attribute
		{
			return methods.attributes().attributes<T>();
		}
		
		public static List<T> attributes<T>(this List<MethodInfo> methods)
			where T : Attribute
		{
			return methods.attributes<T>();
		}
		
		public static List<T> attributes<T>(this List<Attribute> attributes)
			where T : Attribute
		{
			return (from attribute in attributes
					where attribute is T
					select (T)attribute).toList();
		}
				
		
		public static List<Attribute> attributes(this List<MethodInfo> methods, string name)
		{
			return methods.attributes().withName(name);
		}
		
		public static Attribute attribute(this MethodInfo methodInfo, string name)
		{
			foreach(var attribute in methodInfo.attributes())
				if (attribute.name() == name)
					return attribute;
			return null;			
		}
		
		public static T attribute<T>(this MethodInfo methodInfo)
			where T : Attribute
		{
			return methodInfo.attributes<T>().first();			
		}
		
		public static string name(this Attribute attribute)
		{
			return attribute.typeName().remove("Attribute");
		}
		
		public static List<string> names(this List<Attribute> attributes)
		{
			return (from attribute in attributes
					select attribute.name()).toList();
		}
		
		public static List<Attribute> withName(this List<Attribute> attributes, string name)
		{
			return (from attribute in attributes
					where attribute.name() == name
					select attribute).toList();
		}
		//Array		
		
		public static Array createArray<T>(this Type arrayType,  params T[] values)			
		{
			try
			{
				if (values.isNull())
					return  Array.CreateInstance (arrayType,0);	
					
				var array =  Array.CreateInstance (arrayType,values.size());	
				
				if (values.notNull())
					for(int i=0 ; i < values.size() ; i ++)
						array.SetValue(values[i],i);
				return array;				 				
			}
			catch(Exception ex)
			{
				ex.log("in Array.createArray");
			}
			return null;
		}
		
		
		
		//WebServices SOAP methods
		public static List<MethodInfo> webService_SoapMethods(this Assembly assembly)
		{
			var soapMethods = new List<MethodInfo >(); 
			foreach(var type in assembly.types())
				soapMethods.AddRange(type.webService_SoapMethods());
			return soapMethods;
					
		}
		public static List<MethodInfo> webService_SoapMethods(this object _object)
		{
			Type type = (_object is Type) 	
							? (Type)_object
							: _object.type();				
			var soapMethods = new List<MethodInfo >(); 
			foreach(var method in type.methods())
				foreach(var attribute in method.attributes())
					if (attribute.typeFullName() == "System.Web.Services.Protocols.SoapDocumentMethodAttribute" ||
					    attribute.typeFullName() == "System.Web.Services.Protocols.SoapRpcMethodAttribute")
						soapMethods.Add(method);
			return soapMethods;
		}
		
		public static Items property_Values_AsStrings(this object _object)
		{		
			var propertyValues_AsStrings = new Items();
			foreach(var property in _object.type().properties())				
				propertyValues_AsStrings.add(property.Name.str(), _object.property(property.Name).str());
			return propertyValues_AsStrings;
		}
				
	}		
	
	//used to get values from __COM_Objects
	public static class _Extra_ComObject_ExtensionMethods
	{		
	
		public static T get_Value<T>(this object _comObject, string name)
		{
			return _comObject.prop_ComObject<T>(name);
		}
	
		public static T prop_ComObject<T>(this object _comObject, string name)
		{
			var value = _comObject.prop_ComObject(name);
			if (value is T)
				return (T)value;
			return default(T);
		}
		
		public static object prop_ComObject(this object _comObject, string name)
		{
			return _comObject.prop_ComObject_Value(name);
		}
		
		public static void prop_ComObject(this object _comObject, string name, object value)
		{
			_comObject.prop_ComObject_Value(name, value);
		}
		
		public static List<object> prop_ComObject_toList(this object _comObject, string name)
		{
			return _comObject.prop_ComObject_Value(name).extractList();
		}
		
		public static List<PropertyDescriptor> prop_ComObject_Properties(this object _comObject)
		{
			var properties = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor property in TypeDescriptor.GetProperties(_comObject))
				properties.Add(property);
			return properties;
		}
		
		public static PropertyDescriptor prop_ComObject_Property(this object _comObject, string name)			
		{
			foreach(var property in _comObject.prop_ComObject_Properties())
				if (property.Name.str() == name)
					return property;
			return null;								
		}
		
		public static object prop_ComObject_Value(this object _comObject, string name)			
		{
			var propertyDescriptor = _comObject.prop_ComObject_Property(name);
			if (propertyDescriptor.notNull())
				return propertyDescriptor.GetValue(_comObject);
			"[prop_ComObject_Value (getter)] didn't find property called {0} in provided object".error(name);	
			return null;
		}
		
		public static void prop_ComObject_Value(this object _comObject, string name, object value)	
		{
			var propertyDescriptor = _comObject.prop_ComObject_Property(name);
			if (propertyDescriptor.notNull())
				propertyDescriptor.SetValue(_comObject, value);
			else
				"[prop_ComObject_Value (setter)] didn't find property called {0} in provided object".error(name);
		}	
			
		
		
		//note:the results of this are not consistent
		public static TreeView showInfo_ComObject(this  object _rootObject)
		{
			var treeView = O2Gui.open<Panel>("showInfo_ComObject",400,400).add_TreeView();
			var propertyGrid = treeView.insert_Below<Panel>().add_PropertyGrid();
			
			Action<TreeNode, object> add_Object =
				(treeNode,_object)=>{
									treeNode.clear();									
									//treeNode.add_Node(_object.str(), _object, true);
									Ascx_ExtensionMethods.add_Node(treeNode,_object.str(), _object, true);
								  };
			Action<TreeNode, IEnumerable> add_Objects = 
				(treeNode,items)=>{
									treeNode.clear();
									foreach(var item in items)
										//treeNode.add_Node(item.str(), item, true);
										Ascx_ExtensionMethods.add_Node(treeNode, item.str(), item, true);
								  };
			
			 
			treeView.beforeExpand<object>(
				(treeNode, _object)=>{		
										if (_object is String)
											treeNode.add_Node(_object); 
										else
										{
											if (_object is IEnumerable)
												add_Objects(treeNode, _object as IEnumerable);
											else
												foreach(PropertyDescriptor property in TypeDescriptor.GetProperties(_object))
												{
													try
													{
														var value =  property.GetValue(_object);
														treeNode.add_Node(property.Name.str(),value,true);
													}
													catch(Exception ex)
													{
														treeNode.add_Node(property.Name.str(),"O2 ERROR:".format(ex.Message) ,false);
													}
												}
										}
									 });
			
			treeView.afterSelect<object>(
				(_object)=> propertyGrid.show(_object));
				
			if(_rootObject is IEnumerable)
				add_Objects(treeView.rootNode(), _rootObject as IEnumerable);  
			else
				add_Object(treeView.rootNode(), _rootObject);  
			return treeView;
		}
	}	
	
	
	public static class _extra_Lists_for_ComObject_ExtensionMethods
	{
		//these helps with COM Objects received from IE	
		public static List<string> extractList_String(this object _object)
		{
			return _object.extractList<string>();
		}
		
		public static List<T> extractList<T>(this object _object)
		{
			return _object.extractList<T>(true);
		}
		
		public static List<T> extractList<T>(this object _object, bool logCastErrors)
		{
			var results = new List<T>();
			if (_object is IEnumerable)
			{
				foreach(var item in (IEnumerable)_object)
					if (item is T)
						results.Add((T)item);
					else
						if(logCastErrors)
							"[extractList] inside the IEnumerable, this item was not of type '{0}' it was of type: '{1}'".error(typeof(T), item.comTypeName());
			}
			else
				"[extractList] the provided object was not IEnumerable: {0}".error(_object);
			return results;
		}
		
		public static List<object> extractList(this object _object)
		{
			var results = new List<object>();
			if (_object is IEnumerable)
			{
				foreach(var item in (IEnumerable)_object)					
					results.Add(item);
			}
			else
				"[extractList] the provided object was not IEnumerable: {0}".error(_object);
			return results;
		}
	}
	
	
	public static class _Extra_Reflection_ExtensionMethods_DynamicallyLoadingAssemblies
	{
		public static Assembly resolveAssembly(this string nameOrPath)
		{
			return nameOrPath.resolveAssembly((nameToResolve)=> nameToResolve);
		}
		
		public static Assembly resolveAssembly(this string name, string resolvedPath)
		{
			return name.resolveAssembly((nameToResolve)=> resolvedPath);
		}
		
		public static Assembly resolveAssembly(this string nameOrPath, Func<string,string> resolveName)
		{
			System.ResolveEventHandler assemblyResolve =  
				(sender, args)=>{						
									var name = args.prop("Name").str();
									//"[AssemblyResolve] for name: {0}".debug(name);
									var location = resolveName(name);						
									if (location.valid())
									{ 								
										//"[AssemblyResolve] found location: {0}".info(location);
										var assembly = Assembly.Load(location.fileContents_AsByteArray());
										if (assembly.notNull())
										{										
											//"[AssemblyResolve] loaded Assembly: {0}".info(assembly.FullName);
											return assembly;
										}
										else
											"[AssemblyResolve] failed to load Assembly from location: {0}".error(location);
									}
									else
										"[AssemblyResolve] could not find a location for assembly with name: {0}".error(name);
									return null;
								};
			 
			Func<string,Assembly> loadAssembly = 
				(assemblyToLoad)=>{
										AppDomain.CurrentDomain.AssemblyResolve += assemblyResolve;
										var assembly = assemblyToLoad.assembly();
										AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolve;
										return assembly;
								  };
			return loadAssembly(nameOrPath);
		}
		
		
		public static Assembly loadAssemblyAndAllItsDependencies(this string pathToAssemblyToLoad) 	
		{
			var referencesFolder = pathToAssemblyToLoad.directoryName();
			var referencesFiles = referencesFolder.files(true,"*.dll", "*.exe");
			
			Func<string,string> resolveAssemblyName = 
				(name)=>{   			
							if (name.starts("System"))
								return name; 
							if(name.isAssemblyName())		 		
								name = name.assemblyName().name();  										
							
							var resolvedPath = referencesFiles.find_File_in_List(name, name+ ".dll", name+ ".exe");					
							
							if(resolvedPath.fileExists())
							{
								//"**** Found match:{0}".info(resolvedPath);	 
								return resolvedPath;
							}				 
							
							//"**** Couldn't match:{0}".error(resolvedPath);	
							return null;  
						};
			
			
			var loadedAssemblies = new Dictionary<string, Assembly>();

			Action<Assembly> loadReferencedAssemblies = null;
			Func<string, Assembly> loadAssembly = null;
			loadAssembly = 
				(assemblyPathOrName) => {
											if (loadedAssemblies.hasKey(assemblyPathOrName))
												return loadedAssemblies[assemblyPathOrName];
											var assembly = assemblyPathOrName.resolveAssembly(resolveAssemblyName);											
											if(assembly.notNull())
											{
												loadedAssemblies.add(assemblyPathOrName, assembly);
												loadReferencedAssemblies(assembly); 												
												
												if (assembly.Location.valid().isFalse())
												{
													loadAssembly(assembly.FullName.assemblyName().name()); 
													if (assembly.ManifestModule.Name != "<Unknown>")
														loadAssembly(assembly.ManifestModule.Name);
													else
														loadAssembly(assembly.ManifestModule.ScopeName);
												}
												 //loadAssembly(assembly.ManifestModule.Name);
												 
											} 
											return assembly;
										};
										
			loadReferencedAssemblies = 
				(assembly)=>{
								var referencedAssemblies =  assembly.referencedAssemblies();								
								foreach(var referencedAssembly in referencedAssemblies)
								{									
									var assemblyName = referencedAssembly.str();																																				
										if (loadAssembly(assemblyName).isNull())
											"COULD NOT LOAD Referenced Assembly: {0}".error(assemblyName);
								}
							};										
			
			var mainAssembly = loadAssembly(pathToAssemblyToLoad);
		
			"[loadAssemblyAndAllItsDependencies] there were {0} references loaded/mapped from '{1}'".info(loadedAssemblies.size(), pathToAssemblyToLoad);
			//show.info(loadedAssemblies);			
			
			return mainAssembly;
		}		
	}
	
	public static class _Extra_Misc_Reflection_ExtensionMethods_Which_Use_The_New_ExtensionMethods
	{
		public static Icon set_As_Default_Form_Icon(this Icon icon)			
		{
			typeof(Form).fieldValue("defaultIcon", icon); 
			return icon;
		}
	}
}
    	