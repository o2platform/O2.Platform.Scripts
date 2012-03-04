// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.Zip;
using O2.XRules.Database.Utils;

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs
//O2File:API_IKVMC.cs

namespace O2.XRules.Database.APIs.IKVM 
{	
	[Serializable]
    public class API_IKVMC_Java_Metadata
    {
    	[XmlAttribute] 
    	public string FileProcessed {get; set;}  
    	[XmlElement("JavaClass")]
    	public List<Java_Class> Classes {get; set;}
    	
    	public API_IKVMC_Java_Metadata()
    	{    		
    		Classes = new List<Java_Class>();
    	}
    	
    	public override string ToString()
    	{
    		return "{0}            [{1} classes]".format(FileProcessed.fileName(), Classes.size());
    	}
	}
	
	[Serializable]
	public class Java_Class
	{		
		[XmlAttribute] public string Signature {get;set;}
		[XmlAttribute] public string GenericSignature {get;set;}
		[XmlAttribute] public string Name {get;set;}
		[XmlAttribute] public string SuperClass {get;set;}
		[XmlAttribute] public string EnclosingMethod {get;set;}
		[XmlAttribute] public string SourceFile {get;set;}
		[XmlAttribute] public bool IsAbstract {get;set;}
		[XmlAttribute] public bool IsInterface {get;set;}
		[XmlAttribute] public bool IsInternal {get;set;}
		[XmlAttribute] public bool IsPublic {get;set;}
					   public Object[] Annotations {get;set;}
					   public List<string> Interfaces {get;set;}					   
					   public List<Java_Field> Fields {get;set;}
					   public List<Java_Method> Methods {get;set;}
					   public List<ConstantPool> ConstantsPool {get;set;}					   
		
		public Java_Class()
		{
			Interfaces = new List<string>();
			Fields = new List<Java_Field>();
			Methods = new List<Java_Method>();
			ConstantsPool = new List<ConstantPool>();
		}
		
		public override string ToString()
    	{
    		return Name;
    		//return "{0}            [{1} methods]".format(Name, Methods.size());
    	}
	}
	
	
	[Serializable]
	public class Java_Field
	{
		[XmlAttribute] 				public string Name {get;set;}
		[XmlAttribute] 				public string Signature {get;set;}		
		[XmlAttribute] 				public string ConstantValue {get;set;}
	}
	
	[Serializable]
	public class Java_Method 
	{
		[XmlAttribute] 				public string Name {get;set;}
		[XmlAttribute] 				public string ParametersAndReturnType {get;set;}		
		[XmlAttribute] 				public string ReturnType {get;set;}		
		[XmlAttribute] 				public string ClassName {get;set;}
		[XmlAttribute]				public string Signature {get;set;}
		[XmlAttribute]				public string SimpleSignature {get;set;}
		[XmlAttribute] 				public string GenericSignature {get;set;}
		[XmlAttribute] 				public bool IsAbstract {get;set;}
		[XmlAttribute] 			  	public bool IsClassInitializer {get;set;}		
		[XmlAttribute] 			  	public bool IsNative {get;set;}		
		[XmlAttribute] 			  	public bool IsPublic {get;set;}
		[XmlAttribute] 			  	public bool IsStatic {get;set;}		
									public Object[] Annotations {get;set;}
									public Object[] ParameterAnnotations {get;set;}
		[XmlElement("Variable")]	public List<Java_Variable> Variables {get;set;}
		[XmlElement("LineNumber")]	public List<LineNumber> LineNumbers {get;set;}
		[XmlElement("Instruction")] public List<Java_Instruction> Instructions {get;set;}
		
		public Java_Method()
		{
			Variables = new List<Java_Variable>();
			Instructions = new List<Java_Instruction>();
			LineNumbers = new List<LineNumber>();
		}
		
		public override string ToString()
    	{
    		return GenericSignature.isNull()
    					? "{0}{1}".format(Name, ParametersAndReturnType)
    					: "{0}{1}   - GenericSignature:  {2}".format(Name, ParametersAndReturnType, GenericSignature);
    	}
	}
	
	[Serializable]
	public class ConstantPool
	{
		[XmlAttribute] public int Id {get;set;}
		[XmlAttribute] public string Type {get;set;}
		[XmlAttribute] public string Value {get;set;}
		[XmlAttribute] public bool ValueEncoded {get;set;}
		
		public ConstantPool()
		{
		}
		
		public ConstantPool(int id, string type, string value)
		{
			Id = id;
			Type = type;
			Value = value;
		}
		
		public ConstantPool(int id, string type, string value, bool valueEncoded) : this(id, type, value)
		{
			ValueEncoded = valueEncoded;
		}
		
		public override string ToString()	
		{
			return (ValueEncoded) ? Value.base64Decode() : Value;
		}
		
	}
	
	[Serializable]
	public class Java_Instruction
	{		
		[XmlAttribute] public int Pc {get;set;}	
		[XmlAttribute] public string OpCode {get;set;}		
		[XmlAttribute] public int Target_Index {get;set;}				
		[XmlAttribute] public int Line_Number {get;set;}
	}		
	
	[Serializable]
	public class Java_Variable
	{		
		[XmlAttribute] public string Descriptor {get;set;}		
		[XmlAttribute] public int Index {get;set;}		
		[XmlAttribute] public int Length {get;set;}		
		[XmlAttribute] public string Name {get;set;}		
		[XmlAttribute] public int Start_Pc {get;set;}			
	}		
	
	[Serializable]
	public class LineNumber
	{
		[XmlAttribute] public int Line_Number {get;set;}
		[XmlAttribute] public int Start_Pc {get;set;}		
	}	
	
	public static class API_IKVMC_Java_Metadata_ExtensionMethods
	{
		public static API_IKVMC_Java_Metadata javaMetadata(this string file)
		{
			API_IKVMC_Java_Metadata javaMetadata = null;
			try
			{	
				javaMetadata = (API_IKVMC_Java_Metadata)O2LiveObjects.get(file);	// try first to get a cached version of this file
			}
			catch
			{}
			if (javaMetadata.isNull() && file.fileExists())
			{
				"Cached version not found, loading from disk".info();
				if (file.extension(".xml"))
					javaMetadata =  file.load<API_IKVMC_Java_Metadata>();
				else
					javaMetadata =  new API_IKVMC().create_JavaMetadata(file);						
				O2LiveObjects.set(file, javaMetadata);
			}
			return javaMetadata;
		}
		
		public static Dictionary<string, Java_Class> classes_IndexedBySignature(this API_IKVMC_Java_Metadata javaMetadata)
		{
			return javaMetadata.Classes.indexedBySignature();				
		}
					
		public static Dictionary<string, Java_Method> methods_IndexedBySignature(this API_IKVMC_Java_Metadata javaMetadata)
		{
			var methodsBySignature = new Dictionary<string, Java_Method>();
			foreach(var _class in javaMetadata.Classes)
				foreach(var item in _class.Methods.indexedBySignature())
					methodsBySignature.Add(item.Key, item.Value);
			return methodsBySignature;
		}


		public static Dictionary<string, Java_Class> indexedBySignature(this List<Java_Class> classes)
		{
			var classesBySignature = new Dictionary<string, Java_Class>();
			if (classes.notNull())
				foreach(var _class in classes) 
					classesBySignature.add(_class.Signature, _class);
			return classesBySignature;
		}			
		
		public static Dictionary<string, Java_Method> indexedBySignature(this List<Java_Method> methods)
		{
			var methodsBySignature = new Dictionary<string, Java_Method>();			
			foreach(var method in methods)
				methodsBySignature.Add(method.Signature, method);
			return methodsBySignature;
		}
		
		public static List<Java_Class> classes_ThatAre_Interfaces(this API_IKVMC_Java_Metadata javaMetadata)
		{
			return (from _class in javaMetadata.Classes
					where _class.IsInterface					
					select _class).toList();
		}
		
		public static Dictionary<string, List<Java_Class>> classes_MappedTo_Implementations(this API_IKVMC_Java_Metadata javaMetadata)
		{	
			//next map the Interfaces
			var implementations = new Dictionary<string, List<Java_Class>>();
			foreach(var _class in javaMetadata.Classes)			
				foreach(var _interface in _class.Interfaces)
					implementations.add(_interface, _class);
					
					
			//next map the SuperClasses
			var classes_bySignature = javaMetadata.classes_IndexedBySignature();
			foreach(var _class in javaMetadata.Classes)			
				if (_class.SuperClass.valid() && classes_bySignature.hasKey(_class.SuperClass))
					implementations.add(_class.SuperClass, _class);//.Signature, classes_bySignature[_class.SuperClass]);
					
			return implementations;
		}
		
		public static Dictionary<string, List<Java_Class>> classes_MappedTo_EnclosingMethod(this API_IKVMC_Java_Metadata javaMetadata)
		{
			var enclosingMethods = new Dictionary<string, List<Java_Class>>();
			foreach(var _class in javaMetadata.Classes)
				if (_class.EnclosingMethod.notNull())
					enclosingMethods.add(_class.EnclosingMethod, _class);
			return 	enclosingMethods;	
		}
		
		public static Dictionary<string, string> methods_MappedTo_EnclosingMethod(this API_IKVMC_Java_Metadata javaMetadata)
		{
			var enclosingMethods = new Dictionary<string, string>();
			foreach(var _class in javaMetadata.Classes)
				if (_class.EnclosingMethod.notNull())
					foreach(var method in _class.Methods)
						enclosingMethods.add(method.Signature, _class.EnclosingMethod);
			return 	enclosingMethods;			
		}		
		
		public static List<Java_Method> implementations(this Java_Method method, Dictionary<string, List<Java_Class>> classesImplementations)
		{
			var implementations = new List<Java_Method>();
			if (classesImplementations.hasKey(method.ClassName))
			{
				foreach(var _class in classesImplementations[method.ClassName])
					foreach(var implementationMethod in _class.Methods)
						if (method.SimpleSignature == implementationMethod.SimpleSignature)
						{
							implementations.Add(implementationMethod);
							break;
						}
			}
			return implementations;						
		}
		
		
		public static List<int> uniqueTargetIndexes(this Java_Method method)
		{
			return  (from instruction in method.Instructions
		 			 where instruction.Target_Index > 0
					 select instruction.Target_Index).Distinct().toList();
		}
		
		public static List<string> values(this List<ConstantPool> list)
		{
			return (from item in list
					select item.str()).toList();
		}
		
		public static List<Java_Class> java_Classes(this API_IKVMC_Java_Metadata javaMetadata)
		{
			return javaMetadata.Classes;
		}
		
		public static List<Java_Class> java_Classes_with_EnclosingMethod(this API_IKVMC_Java_Metadata javaMetadata)
		{
			return (from _class in javaMetadata.java_Classes()
					where _class.EnclosingMethod.notNull()
					select _class).toList();
		}
		
		public static List<Java_Method> java_Methods(this Java_Class _class)
		{
			if (_class.notNull())				
				return _class.Methods;
			return new List<Java_Method>();
		}
		
		public static List<Java_Method> java_Methods(this API_IKVMC_Java_Metadata javaMetadata)
		{
			return (from _class in javaMetadata.Classes
					from method in _class.Methods
					select method).toList();			
		}
		
		public static List<Java_Method> java_Methods_Abstract(this API_IKVMC_Java_Metadata javaMetadata)
		{
			return (from method in javaMetadata.java_Methods()
					where method.IsAbstract
					select method).toList();
		}
		
		public static List<Java_Method> java_Methods_Setters_Public(this Java_Class _class)
		{
			return (from method in _class.Methods
					where method.IsPublic && method.Name.starts("set")
					select method).toList();
		}
		
		public static List<Java_Method> java_Methods_Getters_Public(this Java_Class _class)
		{
			return (from method in _class.Methods
					where method.IsPublic && method.Name.starts("get")
					select method).toList();
		}
		
		public static List<Java_Method> with_Primitive_Parameter(this List<Java_Method> methods, bool value)
		{
			var primitiveReturnTypes = 
				new List<string> { 
									"(Ljava.lang.Integer;)V",
									"(Ljava.util.Date;)V",
									"(Ljava.lang.String;)V",
									"(Ljava.lang.Boolean;)V",
									"(D)V",
									"(Z)V",
									"(I)V"
								 };
											
			return (from method in methods	
					where primitiveReturnTypes.contains(method.ParametersAndReturnType) == value	// need to make this search in the actual parameter values
					select method).toList();
		}
		
		public static List<Java_Method> with_Primitive_ReturnType(this List<Java_Method> methods, bool value)
		{
			var primitiveReturnTypes = 
				new List<string> { 
									"Ljava.lang.Integer;",
									"Ljava.util.Date;",
									"Ljava.lang.String;",
									"Ljava.lang.Boolean;",
									"I",
									"Z",
									"D"
									
								 };
											
			return (from method in methods
					where primitiveReturnTypes.contains(method.ReturnType) == value
					select method).toList();
		}
		
		
		public static string returnType(this Java_Method method)
		{
			var returnType = "";
			if (method.GenericSignature.isNull())
				returnType = method.ReturnType;
			
			if (returnType.inValid())	
				returnType = method.GenericSignature.subString_After("java/util/Set<")
													.removeLastChar().removeLastChar()
													.replace("/",".").trim();
			if (returnType.inValid())				
				returnType = method.GenericSignature.subString_After("java/util/Collection<")
													.removeLastChar().removeLastChar()
													.replace("/",".").trim();
			return (returnType.starts("L"))
					? returnType.removeFirstChar().removeLastChar()
					: returnType;												

		}
		
		public static Java_Method name(this List<Java_Method> methods, string value)
		{
			return methods.withValue("Name", value);
		}
		
		public static Java_Method signature(this List<Java_Method> methods, string value)
		{
			return methods.withValue("Signature", value);
		}
		
		public static Java_Method simpleSignature(this List<Java_Method> methods, string value)
		{
			return methods.withValue("SimpleSignature", value);
		}
		
		public static Java_Method withValue(this List<Java_Method> methods, string name, string value)
		{
			foreach(var method in methods)
				if (method.prop(name).str() == value)
					return method;
			return null;
		}
		
		public static Java_Class signature(this List<Java_Class> classes, string value)
		{
			return classes.withValue("Signature", value);
		}		
		
		public static Java_Class withValue(this List<Java_Class> classes, string name, string value)
		{
			foreach(var _class in classes)
				if (_class.prop(name).str() == value)
					return _class;
			return null;
		}
		
		public static Dictionary<int, Java_Instruction> instructions_byPc(this Java_Method method)
		{
			var instructions_byPc = new Dictionary<int, Java_Instruction>();
			foreach(var instruction in method.Instructions)
				instructions_byPc.Add(instruction.Pc, instruction);
			return instructions_byPc;
		}
		
		public static Dictionary<int, List<Java_Variable>> variables_byIndex(this Java_Method method)
		{
			var variables_byIndex = new Dictionary<int, List<Java_Variable>>();
			foreach(var variable in method.Variables)
				variables_byIndex.add(variable.Index, variable);
			return variables_byIndex;
		}
	}
	
	
}