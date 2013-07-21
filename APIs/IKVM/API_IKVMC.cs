// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.Zip;
using O2.XRules.Database.Utils;
using O2.External.SharpDevelop.Ascx;
using O2.External.SharpDevelop.ExtensionMethods;

//O2File:API_IKVMC_JavaMetadata.cs
//O2Ref:IKVM\ikvm-7.1.4532.2\bin\IKVM.Runtime.dll
//O2Ref:IKVM\ikvm-7.1.4532.2\bin\IKVM.Runtime.JNI.dll
//O2Ref:IKVM\ikvm-7.1.4532.2\bin\IKVM.OpenJDK.Util.dll
//O2Ref:IKVM\ikvm-7.1.4532.2\bin\IKVM.OpenJDK.Core.dll
//O2Ref:IKVM\ikvm-7.1.4532.2\bin\IKVM.Reflection.dll
//O2Ref:IKVM\ikvm-7.1.4532.2\bin\ikvmc.exe

//_O2Ref:IKVM.OpenJDK.SwingAWT.dll
//_O2Ref:IKVM.OpenJDK.Security.dll
//_O2Ref:IKVM.Runtime.dll
//_O2Ref:IKVM.OpenJDK.Util.dll
//_O2Ref:IKVM.OpenJDK.Core.dll
//_O2Ref:IKVM.Reflection.dll
//_O2Ref:IKVM.Runtime.JNI.dll

namespace O2.XRules.Database.APIs.IKVM
{
    public class API_IKVMC	
    {
    	public Assembly IkvmcAssembly   { get; set;}
    	public Type StaticCompiler  { get; set;}
    	public object IkvmRuntime 	  { get; set;}
    	public object IkvmRuntimeJni  { get; set;}
    	public object IkvmcCompiler   { get; set;}
    	public object CompilerOptions { get; set;}
    	 
    	public API_IKVMC()
    	{
    		setup();
    	}
    	
    	public void setup()
    	{    		
    		//IkvmcAssembly = "ikvmc.exe".assembly();
    		IkvmcAssembly = @"IKVM\ikvm-7.1.4532.2\bin\ikvmc.exe".assembly();
    		StaticCompiler = IkvmcAssembly.type("StaticCompiler");
			//IkvmRuntime = StaticCompiler.invokeStatic("LoadFile",Environment.CurrentDirectory.pathCombine("IKVM.Runtime.dll")); 
			IkvmRuntime = StaticCompiler.invokeStatic("LoadFile","IKVM.Runtime.dll".assembly().Location); 			 			
			PublicDI.reflection.setField((FieldInfo)StaticCompiler.field("runtimeAssembly"),IkvmRuntime);  				
			
			//IkvmRuntimeJni = StaticCompiler.invokeStatic("LoadFile",Environment.CurrentDirectory.pathCombine("IKVM.Runtime.JNI.dll")); 
			IkvmRuntimeJni = StaticCompiler.invokeStatic("LoadFile","IKVM.Runtime.JNI.dll".assembly().Location); 
			PublicDI.reflection.setField((FieldInfo)StaticCompiler.field("runtimeJniAssembly"),IkvmRuntimeJni);  
			
			IkvmcCompiler =  IkvmcAssembly.type("IkvmcCompiler").ctor();
			
			CompilerOptions = IkvmcAssembly.type("CompilerOptions").ctor();
			PublicDI.reflection.setField((FieldInfo)StaticCompiler.field("toplevel"),CompilerOptions);  
    	}
	}
	
	public static class API_IKVMC_ExtensionMethods_CreateJavaMetadata
	{
		public static IDictionary getRawClassesData_FromFile_ClassesOrJar(this API_IKVMC ikvmc, string classOrJar)
		{
			var targets = typeof(List<>).MakeGenericType(new System.Type[] { ikvmc.CompilerOptions.type()}).ctor();
			var args = classOrJar.wrapOnList().GetEnumerator();
			ikvmc.IkvmcCompiler.invoke("ParseCommandLine", args, targets, ikvmc.CompilerOptions);  				
			var compilerOptions =  (targets as IEnumerable).first();
			//var classes =   (Dictionary<string, byte[]>) compilerOptions.field("classes");  
			var classes =   (IDictionary) compilerOptions.field("classes");  			
			return classes;
		}				
		
		public static object createClassFile(this API_IKVMC ikvmc, byte[] bytes)
		{
			// 0 = ClassFileParseOptions.None, 
			// 1 = ClassFileParseOptions.LocalVariableTable , 
			// 2 = ClassFileParseOptions.LineNumberTable
			return ikvmc.createClassFile(bytes,0);			
		}
		
		public static object createClassFile(this API_IKVMC ikvmc, byte[] bytes, int classFileParseOptions)
		{
			var classFileType = ikvmc.IkvmcAssembly.type("ClassFile");
			var ctor = classFileType.ctors().first();					
		 
			var classFile = ctor.Invoke(new object[] {bytes,0, bytes.Length, null, classFileParseOptions });   
			if (classFile.notNull())
				return classFile;
			"[API_IKVMC] in createClassObject failed to create class".info();
			return null;
		}
		 
		public static API_IKVMC_Java_Metadata create_JavaMetadata(this API_IKVMC ikvmc, string fileToProcess)
		{
			var o2Timer = new O2Timer("Created JavaData for {0}".format(fileToProcess)).start();
			var javaMetaData = new API_IKVMC_Java_Metadata();
			javaMetaData.FileProcessed = fileToProcess;
			var classes = ikvmc.getRawClassesData_FromFile_ClassesOrJar(fileToProcess);
			foreach(DictionaryEntry item in classes)
			{
				var name = item.Key.str();
				var bytes = (byte[])item.Value.field("data");
				var classFile = ikvmc.createClassFile(bytes,1);				// value of 1 gets the local variables
				var javaClass = new Java_Class {
													Signature = name,													
													Name = name.contains(".") ? name.split(".").last() : name,
													SourceFile = classFile.prop("SourceFileAttribute").str(),
							 						IsAbstract = classFile.prop("IsAbstract").str().toBool(),
													IsInterface = classFile.prop("IsInterface").str().toBool(),
													IsInternal = classFile.prop("IsInternal").str().toBool(),
													IsPublic = classFile.prop("IsPublic").str().toBool(),
													SuperClass = classFile.prop("SuperClass").str()
											   };
				
				if (classFile.prop("GenericSignature").notNull())
					javaClass.GenericSignature = classFile.prop("GenericSignature").str();
					
				javaClass.ConstantsPool = classFile.getConstantPoolEntries();
				javaClass.map_Annotations(classFile)
						 .map_Interfaces(classFile)
						 .map_Fields(classFile)
						 .map_Methods(classFile)
						 .map_EnclosingMethod(classFile);								
						 				
				javaClass.map_LineNumbers(ikvmc.createClassFile(bytes,2)); // for this we need to call createClassFile with the value of 2 (to get the source code references)
				
				javaMetaData.Classes.Add(javaClass);												
				//break; // 
			}
			o2Timer.stop();
			return javaMetaData;
		}
		
		public static Java_Class map_EnclosingMethod(this Java_Class javaClass, object classFile)
		{		
			if (classFile.prop("EnclosingMethod").notNull()) 
			{
				var enclosingMethodData = (string[])classFile.prop("EnclosingMethod");
				if (enclosingMethodData.size()==3)  // when the size it is 1, it represents the case of normal inner classes (not annonymous classes defined inside methods which is what I'm after)				
					javaClass.EnclosingMethod = "{0}.{1}{2}".format(enclosingMethodData[0], enclosingMethodData[1],enclosingMethodData[2]);									
			}
			return javaClass;
		}
		
		public static Java_Class map_Annotations(this Java_Class javaClass, object classFile)
		{
			var annotations = (Object[])classFile.prop("Annotations");
			if (annotations.notNull())			
				javaClass.Annotations = annotations;			
			return javaClass;
		}
		
		public static Java_Method map_Annotations(this Java_Method javaMethod, object method)
		{
			var annotations = (Object[])method.prop("Annotations");
			if (annotations.notNull())			
				javaMethod.Annotations = annotations;			
				
			var parameterAnnotations = (Object[])method.prop("ParameterAnnotations");
			if (parameterAnnotations.notNull())			
				javaMethod.ParameterAnnotations = parameterAnnotations;								
				
			return javaMethod;
		}
		
		public static Java_Class map_Interfaces(this Java_Class javaClass, object classFile)
		{
			var interfaces = (IEnumerable)classFile.prop("Interfaces");						
			foreach(var _interface in interfaces)
				javaClass.Interfaces.Add(_interface.prop("Name").str());				
			return javaClass;
		}
		
		public static Java_Class map_Fields(this Java_Class javaClass, object classFile)
		{
			//var interfaces = (IEnumerable)classFile.prop("Interfaces");						
			//foreach(var _interface in interfaces)
			//	javaClass.Interfaces.Add(_interface.prop("Name").str());				
			var fields = (IEnumerable)classFile.prop("Fields");
			foreach(var field in fields)			
				javaClass.Fields.Add(new Java_Field
										{
											Name = field.prop("Name").str(),
											Signature = field.prop("Signature").str(),
											ConstantValue= field.prop("ConstantValue").notNull() 
																? field.prop("ConstantValue").str()
																: null
										});				
			
			return javaClass;
		}
		
		public static Java_Class map_Methods(this Java_Class javaClass, object classFile)
		{
			var mappedConstantsPools = javaClass.ConstantsPool.getDictionaryWithValues();
			var methods = (IEnumerable)classFile.prop("Methods");
			foreach(var method in methods)
			{		
				var javaMethod = new Java_Method {
													Name = method.prop("Name").str(),
													ParametersAndReturnType = method.prop("Signature").str(),														
													ClassName = javaClass.Signature,
													IsAbstract = method.prop("IsAbstract").str().toBool(), 
													IsClassInitializer = method.prop("IsClassInitializer").str().toBool(), 
													IsNative = method.prop("IsNative").str().toBool(), 
													IsPublic = method.prop("IsPublic").str().toBool(), 
													IsStatic = method.prop("IsStatic").str().toBool()
												 };															 
				javaMethod.SimpleSignature = "{0}{1}".format(javaMethod.Name,javaMethod.ParametersAndReturnType);
				javaMethod.ReturnType = javaMethod.ParametersAndReturnType.subString_After(")");
				javaMethod.Signature =  "{0}.{1}".format(javaMethod.ClassName, javaMethod.SimpleSignature);
				if (method.prop("GenericSignature").notNull())
					javaMethod.GenericSignature = method.prop("GenericSignature").str();
				javaMethod.map_Annotations(method)
						  .map_Variables(method);
				
				
				var instructions = (IEnumerable) method.prop("Instructions");
				if (instructions.notNull())
				{	
					foreach(var instruction in instructions)
					{
						var javaInstruction = new Java_Instruction();
						javaInstruction.Pc = instruction.prop("PC").str().toInt(); 
						javaInstruction.OpCode = instruction.prop("NormalizedOpCode").str(); 
						javaInstruction.Target_Index =  instruction.prop("TargetIndex").str().toInt();
						javaMethod.Instructions.Add(javaInstruction);						
					}
				}
				
				javaClass.Methods.Add(javaMethod);
			}
			return javaClass;
		}
		
		public static Java_Method map_Variables(this Java_Method javaMethod, object method)
		{			
			var localVariablesTable = (IEnumerable)method.prop("LocalVariableTableAttribute");
			if (localVariablesTable.notNull())			
				foreach(var localVariable in localVariablesTable)
					javaMethod.Variables.Add(new Java_Variable  {
																	Descriptor = localVariable.field("descriptor").str(), 
																	Index = localVariable.field("index").str().toInt(), 
																	Length = localVariable.field("length").str().toInt(), 
																	Name = localVariable.field("name").str(), 
																	Start_Pc = localVariable.field("start_pc").str().toInt()
																});							
			return javaMethod;
		}
		
		//we need to do this because the IKVMC doesn't have a option to get both fields and source code references at the same time
		public static Java_Class map_LineNumbers(this Java_Class javaClass, object classFileWithLineNumbers)
		{
			var methodsWithLineNumbers = ((IEnumerable)classFileWithLineNumbers.prop("Methods"));
			var currentMethodId = 0;
			foreach(var methodWithLineNumbers in methodsWithLineNumbers)
			{
				var javaMethod = javaClass.Methods[currentMethodId];
				var lineNumberTableAttributes = (IEnumerable)methodWithLineNumbers.prop("LineNumberTableAttribute");				
				if (lineNumberTableAttributes.notNull())
				{
					foreach(var lineNumberTableAttribute in lineNumberTableAttributes)
						javaMethod.LineNumbers.Add(new LineNumber {
																	Line_Number = lineNumberTableAttribute.field("line_number").str().toInt(),
																	Start_Pc = lineNumberTableAttribute.field("start_pc").str().toInt()																		
																  });					
																  
					var  mapppedLineNumbers = new Dictionary<int,int>();
					foreach(var lineNumber in javaMethod.LineNumbers)
						mapppedLineNumbers.Add(lineNumber.Start_Pc, lineNumber.Line_Number);
					var lastLineNumber = 0;	
					foreach(var instruction in javaMethod.Instructions)
						if (mapppedLineNumbers.hasKey(instruction.Pc))
						{
							instruction.Line_Number = mapppedLineNumbers[instruction.Pc];
							lastLineNumber = instruction.Line_Number;
						}
						else
							instruction.Line_Number = lastLineNumber;
				}
				currentMethodId++;																  
			}						
			return javaClass;
		}
		
		public static List<ConstantPool> add_Entry(this List<ConstantPool> constantsPool,int id, string type, string value)
		{
			return constantsPool.add_Entry(id, type, value, false);
		}
		
		public static List<ConstantPool> add_Entry(this List<ConstantPool> constantsPool,int id, string type, string value, bool valueEncoded)
		{
			var constantPool = new ConstantPool(id, type, value,valueEncoded);
			constantsPool.Add(constantPool);
			return constantsPool;
		}
		public static List<ConstantPool> getConstantPoolEntries(this object classFile)
		{
			var constantsPool = new List<ConstantPool>();
			
			var constantPoolRaw = new Dictionary<int,object>();
			var constantPool =  (IEnumerable)classFile.field("constantpool");  
			if (constantPool.isNull())
				"in getConstantPoolEntries , classFile.field(\"constantpool\") was null".error();
			else
			{;
				var index = 0;
				foreach(var constant in constantPool)
					constantPoolRaw.add(index++, constant); 
				
			//var constantPoolValues = new Dictionary<int,string>();	 
		//	var stillToMap = new List<object>();
				for(int i=0; i < constantPoolRaw.size() ; i++)
				{
					var currentItem = constantPoolRaw[i];
					var currentItemType = currentItem.str();
					switch(currentItemType)
					{
						case "IKVM.Internal.ClassFile+ConstantPoolItemClass":											
							constantsPool.add_Entry(i, currentItemType.remove("IKVM.Internal.ClassFile+ConstantPoolItem"), currentItem.prop("Name").str());						
							break;
						case "IKVM.Internal.ClassFile+ConstantPoolItemMethodref":											
							constantsPool.add_Entry(i,currentItemType.remove("IKVM.Internal.ClassFile+ConstantPoolItem"), 
													"{0}.{1}{2}".format(currentItem.prop("Class"),
													   	 				   currentItem.prop("Name"),
													   	 				   currentItem.prop("Signature")));									
							break;												   	 				   
						case "IKVM.Internal.ClassFile+ConstantPoolItemInterfaceMethodref": 
							constantsPool.add_Entry(i,currentItemType.remove("IKVM.Internal.ClassFile+ConstantPoolItem"),
													"{0}.{1}{2}".format(currentItem.prop("Class"),
													   	 				   currentItem.prop("Name"),
													   	 				   currentItem.prop("Signature")));									
							break;	
						case "IKVM.Internal.ClassFile+ConstantPoolItemFieldref":																		
							constantsPool.add_Entry(i,currentItemType.remove("IKVM.Internal.ClassFile+ConstantPoolItem"), 
													"{0}.{1} : {2}".format(currentItem.prop("Class"),
													   	 				   currentItem.prop("Name"),
													   	 				   currentItem.prop("Signature")));									
							break;
						case "IKVM.Internal.ClassFile+ConstantPoolItemNameAndType":	 // skip this one since don;t know what they point to
							//constantPoolValues.Add(i,"IKVM.Internal.ClassFile+ConstantPoolItemNameAndType");	
							break; 
						case "IKVM.Internal.ClassFile+ConstantPoolItemString":
						case "IKVM.Internal.ClassFile+ConstantPoolItemInteger":
						case "IKVM.Internal.ClassFile+ConstantPoolItemFloat":
						case "IKVM.Internal.ClassFile+ConstantPoolItemDouble": 
						case "IKVM.Internal.ClassFile+ConstantPoolItemLong":
							var value = currentItem.prop("Value").str();
							value = value.base64Encode();//HACK to deal with BUG in .NET Serialization and Deserialization (to reseatch further)
							constantsPool.add_Entry(i,currentItemType.remove("IKVM.Internal.ClassFile+ConstantPoolItem"),value, true);
							break;					
						case "[null value]":
							//constantsPool.add_Entry(i,"[null value]", null);						
							break; 
						default:
							"Unsupported constantPoll type: {0}".error(currentItem.str());
							break;
					}		
				}
			}
			return constantsPool;	
		}		
		
		public static Dictionary<int,string> constantsPool_Values(this Java_Class _class)
		{
			return _class.ConstantsPool.getDictionaryWithValues();
		}
		
		public static Dictionary<int,ConstantPool> constantsPool_byIndex(this Java_Class _class)
		{
			return _class.ConstantsPool.getDictionary_byIndex();
		}
		
		public static Dictionary<string,List<ConstantPool>> constantsPool_byType(this Java_Class _class)
		{
			return _class.ConstantsPool.getDictionary_byType();
		}
		
		public static Dictionary<string,List<ConstantPool>> constantsPool_byType(this Java_Method method, Java_Class _class)
		{
			return _class.ConstantsPool.getDictionary_byType(method);
		}
		
		public static Dictionary<int,string> getDictionaryWithValues(this  List<ConstantPool> constantsPool)
		{
			var dictionary = new Dictionary<int,string>();
			foreach(var item in constantsPool)
			{
				if (item.ValueEncoded)
					dictionary.Add(item.Id, "\"{0}\"".format(item.Value.base64Decode()));	
				else
					dictionary.Add(item.Id, item.Value);					
			}
			return dictionary;
		}
		
		public static Dictionary<int,ConstantPool> getDictionary_byIndex(this  List<ConstantPool> constantsPool)
		{
			var dictionary = new Dictionary<int,ConstantPool>();
			foreach(var item in constantsPool)								
				dictionary.Add(item.Id, item);			
			return dictionary;
		}
		 
		public static Dictionary<string,List<ConstantPool>> getDictionary_byType(this  List<ConstantPool> constantsPool)
		{
			var dictionary = new Dictionary<string,List<ConstantPool>>();
			foreach(var item in constantsPool)								
				dictionary.add(item.Type, item);			
			return dictionary;
		} 
		
		public static Dictionary<int,List<Java_Instruction>> getConstantsPoolUsage_byIndex_WithLineNumbers(this Java_Method method)
		{
			var dictionary = new Dictionary<int,List<Java_Instruction>>(); 
			foreach(var instruction in method.Instructions)	 						
				if(instruction.Target_Index >0)
				dictionary.add(instruction.Target_Index, instruction);			
			return dictionary;
		} 

		public static Dictionary<string,List<ConstantPool>> getDictionary_byType(this  List<ConstantPool> constantsPool, Java_Method method)		
		{
			var usedInMethod = method.uniqueTargetIndexes();
			//show.info(usedInMethod);
			var mappedByIndex = constantsPool.getDictionary_byIndex();
			//show.info(mappedByIndex); 
			var dictionary = new Dictionary<string,List<ConstantPool>>();
			foreach(var index in usedInMethod)
			{
				if (mappedByIndex.hasKey(index))
				{
					var constantPool = mappedByIndex[index];
					dictionary.add(constantPool.Type, constantPool);
				}
			}
			return dictionary;
		}					
		

	}
	
	public static class API_IKVMC_ExtensionMethods_CreateDotNetAssemblies
	{
		public static string createAssembly_FromFile_ClasssesOrJar(this API_IKVMC ikvmc, string classOrJar)
		{				
			var console = "IKVM Stored Console out and error".capture_Console();
			var targetFile = "_IKVM_Dlls".tempDir(false).pathCombine("{0}.dll".format(classOrJar.fileName()));
			
			var args = new List<string> {	
											classOrJar,
											"-out:{0}".format(targetFile)
										} .GetEnumerator();
			//return args.toList();								
			var targets = typeof(List<>).MakeGenericType(new System.Type[] { ikvmc.CompilerOptions.type()}).ctor();

			ikvmc.IkvmcCompiler.invoke("ParseCommandLine", args, targets, ikvmc.CompilerOptions); 

			//ikvmcCompiler.details();
			var compilerClassLoader = ikvmc.IkvmcAssembly.type("CompilerClassLoader");
			var compile = compilerClassLoader.method("Compile"); 
			
			var createCompiler = compilerClassLoader.method("CreateCompiler"); 
			
			PublicDI.reflection.invokeMethod_Static(compile, new object[] {null,targets});  
			//ikvmcCompiler.details(); 
			return console.readToEnd();
		}	
	}
	
	public static class API_IKVMC_ExtensionMethods_Gui_Helpers
	{
		public static TreeNode add_Instructions(this TreeNode treeNode, Java_Method method, Java_Class methodClass)
		{
			var values = methodClass.ConstantsPool.getDictionaryWithValues();
			foreach(var instruction in method.Instructions)
			{
				var nodeText = "[line:{0}] \t {1}".format(instruction.Line_Number,instruction.OpCode);
				if (instruction.Target_Index > 0 && values.hasKey(instruction.Target_Index))								
					nodeText = "{0} {1}".format(nodeText , values[instruction.Target_Index]);
				treeNode.add_Node(nodeText, instruction)
						.color(Color.DarkGreen);
			
			}
			return treeNode;
		}
		
		public static TreeNode add_Variables(this TreeNode treeNode, Java_Method method)
		{
			if (method.Variables.size()>0)
			{
				var variablesNode = treeNode.add_Node("_Variables");
				foreach(var variable in method.Variables)
					variablesNode.add_Node("{0}  :   {1}".format(variable.Name , variable.Descriptor),variable);
			}
			return treeNode;
		}
		
		public static TreeNode add_Annotations(this TreeNode treeNode, bool addAnnotationsNode, object[] annotations)
		{
			if (annotations.notNull())
			{					
				if(addAnnotationsNode) 
					treeNode.add_Node("_Annotations", annotations, true);
				else									
					foreach(var annotation in annotations)
						treeNode.add_Node(annotation.str(),annotation, annotation is object[]);
			}
			return treeNode;
		}
		
		public static TreeNode add_ConstantsPool(this TreeNode treeNode, Java_Class _class)
		{
			var constantsPoolNode = treeNode.add_Node("_ConstantsPool");
			foreach(var item in _class.constantsPool_byType())
				constantsPoolNode.add_Node(item.Key, item.Value, true);
			return treeNode;
		}
		
		public static TreeNode add_ConstantsPool(this TreeNode treeNode, Java_Method method, Java_Class methodClass)
		{
			var constantsPoolNode = treeNode.add_Node("_ConstantsPool");
			foreach(var item in method.constantsPool_byType(methodClass))
				constantsPoolNode.add_Node(item.Key, item.Value, true);
			return treeNode;
		}				
		
		public static string file(this Java_Class _class)
		{
			var file = (_class.Signature.EndsWith(_class.Name)) 
							? _class.Signature.subString(0, _class.Signature.size() - _class.Name.size())
							: _class.Signature;
						
			file = "{0}{1}".format(file.replace(".","\\"), _class.SourceFile); 
			return file;
		}
		
		public static ascx_SourceCodeViewer showInCodeViewer(this ascx_SourceCodeViewer codeViewer ,Java_Class _class, Java_Method method)
		{
			codeViewer.editor().showInCodeEditor(_class, method);
			return codeViewer;
		}
		
		public static ascx_SourceCodeEditor showInCodeEditor(this ascx_SourceCodeEditor codeEditor ,Java_Class _class, Java_Method method)
		{										
			//var _class = classes_bySignature[classSignature];
			var file = _class.file();
			codeEditor.open(file);
			var lineNumber = 0; 
			if (method.isNull() ||method.LineNumbers.isNull())
				return codeEditor;
			foreach(var item in method.LineNumbers)
				if (item.Line_Number > 1)
					if (lineNumber == 0 || item.Line_Number < lineNumber)
						lineNumber = item.Line_Number;
						
			//this to match the method name to the location (vs the first method)
			var sourceCodeLines = codeEditor.getSourceCode().lines(false);
			if (method.Name.regEx("<.*init*.>").isFalse())
			{
				for(int i=0 ; i < 10 ; i++)
				{
					
					if (lineNumber > i &&   sourceCodeLines.size() > lineNumber-i)
					{
						var line = sourceCodeLines[lineNumber-i];					
						if (sourceCodeLines[lineNumber-i].contains(method.Name) &&
							line.regEx("public|private|internal|protected"))
						{						
							lineNumber = lineNumber -i + 1;						
							break;
						}
					}
				}
			}			
			codeEditor.gotoLine(lineNumber,4);
			
			return codeEditor;
   		}
			 
		public static Action<string> viewJavaMappings(this Control control, API_IKVMC_Java_Metadata javaMappings)			
		{	
			control.clear();
			Dictionary<string, Java_Class> classes_bySignature = null;
			Dictionary<string, Java_Method> methods_bySignature = null;
			Dictionary<string, List<Java_Class>> classes_MappedTo_Implementations = null;
			
			var showFullClassNames = false;
			var openSourceCodeReference = false;
			var classFilter = "";
			Action refresh = null;
			API_IKVMC_Java_Metadata currentJavaMetaData = null;
			var treeView = control.add_TreeView_with_PropertyGrid();
			var codeEditor = control.insert_Right().add_SourceCodeEditor();
			var configPanel = control.insert_Below(40,"Config");
			
			treeView.insert_Above(20).add_TextBox("Class Filter","")
									 .onEnter((text) => { 
											 				classFilter = text ;
											 				refresh();
											 			});
			
			configPanel.add_CheckBox("Show Full Class names", 0,0, 
							(value)=>{
										showFullClassNames = value;
										treeView.collapse();
									 }).autoSize()
					   .append_Control<CheckBox>().set_Text("Open SourceCode Reference")
					   							  .autoSize()					   							  
					   							  .onChecked((value)=> openSourceCodeReference=value)
					   							  .check();
			//BeforeExpand Events
			Action<TreeNode, List<Java_Class>> add_Classes = 
				(treeNode, classes)=>{
										treeNode.add_Nodes(classes.Where((_class)=> classFilter.inValid() || _class.Signature.regEx(classFilter))
																  .OrderBy((_class)=>_class.Name),
														   (_class)=> (showFullClassNames)
														   					? _class.Signature
														   					: _class.Name ,
							 							   (_class)=> true,
														   (_class)=> (_class.IsInterface) 
														   					? Color.DarkRed 
														   					: Color.DarkOrange);
									 };
									 
			treeView.beforeExpand<API_IKVMC_Java_Metadata>(
				(treeNode, javaMetadata)=>{												
												add_Classes(treeNode, javaMetadata.Classes);
										  });
					 					  
			treeView.beforeExpand<Java_Class>(   
				(treeNode, _class)=>{
										if(classes_MappedTo_Implementations.hasKey(_class.Signature))
										{
											add_Classes(treeNode.add_Node("_Implementations"), 
														classes_MappedTo_Implementations[_class.Signature]);
										}
										
										
										treeNode.add_ConstantsPool(_class)
												.add_Annotations(true, _class.Annotations);
										treeNode.add_Nodes(_class.Methods.OrderBy((item)=>item.Name),
														   (method)=> method.str(),
														   (method)=> method.IsAbstract.isFalse(),
													 	   (method)=> (method.IsAbstract)
													 	   					? Color.DarkRed
													 	   					: Color.DarkBlue);
									});
										    
			treeView.beforeExpand<Java_Method>( 
				(treeNode, method)=>{  																						
										treeNode.add_ConstantsPool(method,classes_bySignature[method.ClassName])
												.add_Annotations(true, method.Annotations)
											    .add_Variables(method);

										treeNode.add_Node("_Instructions")  											    
												.add_Instructions(method,classes_bySignature[method.ClassName]); 
																		
									}); 
									
			treeView.beforeExpand<object[]>(     
				(treeNode, annotations)=>{
											treeNode.add_Annotations(false, annotations);
										});		
			
			treeView.beforeExpand<List<ConstantPool>>(
				(treeNode, constantsPool)=>{
											treeNode.add_Nodes(constantsPool.OrderBy((item)=>item.str()),
															   (constant)=> constant.str(),
														       (constant)=> false,
													 	       (constant)=> methods_bySignature.hasKey(constant.Value)
													 	       					? Color.Green
													 	       					: Color.Sienna);
										});												
			//AfterSelect Events
						  
			treeView.afterSelect<Java_Class>(  
				(_class) => { 
								if (openSourceCodeReference)
								{
									codeEditor.showInCodeEditor(classes_bySignature[_class.Signature], _class.Methods.first());	
									treeView.focus();  
								}
							});
			 
			treeView.afterSelect<Java_Method>(
				(method) => {
								if (openSourceCodeReference)
								{
									codeEditor.showInCodeEditor(classes_bySignature[method.ClassName], method); 
									treeView.focus();  
								}
							});
							
			treeView.afterSelect<Java_Instruction>(
				(instruction) => {	
									if (openSourceCodeReference)
									{
										codeEditor.gotoLine(instruction.Line_Number,4); 
										treeView.focus();  
									}
							});				
			treeView.afterSelect<ConstantPool>(			
				(constantPool)=> {
									if (methods_bySignature.hasKey(constantPool.Value))
									{
										var method = methods_bySignature[constantPool.Value];
										codeEditor.showInCodeEditor(classes_bySignature[method.ClassName], method); 									
										treeView.focus();
									}
								});
			//Other events						
			
			refresh = ()=>{
								if(currentJavaMetaData.notNull())
								{
									treeView.clear();
									classes_bySignature				 = currentJavaMetaData.classes_IndexedBySignature();    			
									methods_bySignature 			 = currentJavaMetaData.methods_IndexedBySignature();    										
									classes_MappedTo_Implementations = currentJavaMetaData.classes_MappedTo_Implementations(); 									
									treeView.add_Node(currentJavaMetaData.str(), currentJavaMetaData, true); 				
									treeView.nodes().first().expand();
									//treeView.focus();
								}
									else
										treeView.add_Node("Drop Jar/Zip or class file to view its contents");		
							};
			
			Action<API_IKVMC_Java_Metadata> loadJavaMappings =
				(_javaMappings)=>{		
									currentJavaMetaData = _javaMappings;
									refresh();									
								 };
								 
			Action<string> loadFile = 
				(file)=>{
							treeView.azure();
							O2Thread.mtaThread(
								()=>{
										if(file.extension(".xml"))
											loadJavaMappings(file.load<API_IKVMC_Java_Metadata>());
										else
											loadJavaMappings(new API_IKVMC().create_JavaMetadata(file));
										treeView.white();
									});
						};
					
			treeView.onDrop(loadFile);
			
			loadJavaMappings(javaMappings);			
		
			return loadFile;
		}
	}	
}