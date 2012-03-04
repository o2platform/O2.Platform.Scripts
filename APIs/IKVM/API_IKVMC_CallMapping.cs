// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.O2Misc;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.Zip;
using O2.XRules.Database.Utils;
using O2.External.SharpDevelop.Ascx;
using O2.External.SharpDevelop.ExtensionMethods;
 
//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs
//O2File:API_IKVMC.cs

namespace O2.XRules.Database.APIs.IKVM
{	
	[Serializable]
	public class Method_CallMapping
	{
		public string Signature {get;set;}		
		public List<MethodCall> IsCalledBy {get;set;}
		public List<MethodCall> CallsTo {get;set;}
		public List<MethodCall> ImplementedBy {get;set;}
		public List<MethodCall> Implements {get;set;}
		//public List<string> CallsTo_NotResolved {get;set;}
		
		public Method_CallMapping()
		{
			IsCalledBy = new List<MethodCall>();
			CallsTo = new List<MethodCall>();
			ImplementedBy = new List<MethodCall>();
			Implements = new List<MethodCall>();
		}			
		
		public Method_CallMapping(string signature) : this()
		{
			Signature = signature;
		}
		
		public override string ToString()
		{ 
			return Signature;
			//return "[{0}] [{1}] {2}".format(CallsTo.size(), IsCalledBy.size(), Signature);
		}
	}
	
	public class MethodCall
	{
		public string Signature {get;set;}		
		public bool IsAbstract { get; set;}
		public List<MethodCall_Location> AtLocations {get;set;}
		
		public MethodCall()
		{
			AtLocations = new List<MethodCall_Location>();
		}
	}
	
	public class MethodCall_Location
	{		
		public string File {get;set;}
		public int Line {get;set;}
		public int Pc {get;set;}		
		public String Signature {get;set;}  	// populated dinamically
		
		public override string ToString()
		{
			var mappedFile = SourceCodeMappingsUtils.mapFile(File);
			if(mappedFile.fileExists())
				return mappedFile.fileContents().lines(false)[Line-1];
			return Signature ?? "...Signature value not set...";
		}
	}
	
	public class JavaMetadata_XRefs
	{
		public API_IKVMC_Java_Metadata JavaMetadata = null;    
		public Dictionary<string, Method_CallMapping> CallMappings {get;set;}
		public Dictionary<string, Java_Class> Classes_by_Signature {get; set;} 
		public Dictionary<string, Java_Method> Methods_by_Signature {get; set;} 		
		public Dictionary<string, List<Java_Class>> Classes_MappedTo_Implementations {get; set;} 
		public Dictionary<string, List<Java_Class>> Classes_by_EnclosingMethod {get; set;} 
		public Dictionary<string, string> Method_by_EnclosingMethod {get; set;} 
		
		public JavaMetadata_XRefs(API_IKVMC_Java_Metadata javaMetadata)
		{
			JavaMetadata = javaMetadata;
			this.map_JavaMetadata_XRefs();
		}		
	}
	
	
	public static class Method_CallMapping_ExtensionMethods
	{
		public static Method_CallMapping callMapping(this Java_Method method)
		{
			return method.Signature.callMapping();
		}
		public static Method_CallMapping callMapping(this string signature)
		{
			return new Method_CallMapping(signature);
		}
		 
		public static MethodCall methodCall(this string signature)
		{
			return new MethodCall { Signature = signature};
		}
		public static MethodCall_Location methodCall_Location(this Java_Class _class, int line, int pc)
		{
			return new MethodCall_Location { File = _class.file() , Line = line , Pc = pc};
		}
		
		public static JavaMetadata_XRefs get_XRefs(this API_IKVMC_Java_Metadata javaMetadata)
		{
			return javaMetadata.map_JavaMetadata_XRefs();
		}
		
		public static JavaMetadata_XRefs map_JavaMetadata_XRefs(this API_IKVMC_Java_Metadata javaMetadata)
		{
			return new JavaMetadata_XRefs(javaMetadata).map_JavaMetadata_XRefs();
		}
		
		public static JavaMetadata_XRefs map_JavaMetadata_XRefs(this JavaMetadata_XRefs xRefs)
		{			
			xRefs.CallMappings = new Dictionary<string, Method_CallMapping>();
			xRefs.Classes_by_Signature = xRefs.JavaMetadata.classes_IndexedBySignature();
			xRefs.Methods_by_Signature = xRefs.JavaMetadata.methods_IndexedBySignature();	
			xRefs.Classes_MappedTo_Implementations = xRefs.JavaMetadata.classes_MappedTo_Implementations();
			xRefs.Classes_by_EnclosingMethod = xRefs.JavaMetadata.classes_MappedTo_EnclosingMethod();
			xRefs.Method_by_EnclosingMethod = xRefs.JavaMetadata.methods_MappedTo_EnclosingMethod();
			
			foreach(var _class in xRefs.JavaMetadata.Classes) 
				foreach(var method in _class.Methods)  
				{  					
					var callMapping =  xRefs.CallMappings.add_Method(method.Signature); 
					
					//CallsTo and IsCalledBy
					foreach(var methodCalled in method.methodRefs(_class))  
					{  						
						callMapping.CallsTo.Add(methodCalled);
						var methodCalled_CallMapping = xRefs.CallMappings.add_Method(methodCalled.Signature);   
												
						methodCalled_CallMapping.IsCalledBy.Add(
							new MethodCall { Signature = method.Signature,
											 AtLocations = methodCalled.AtLocations});
					}
					//ImplementedBy
					
					foreach(var implementedMethod in method.implementations(xRefs.Classes_MappedTo_Implementations))						
					{
						if (implementedMethod.IsAbstract)
							callMapping.ImplementedBy.Add(implementedMethod.Signature.methodCall());	
							
						if (method.IsAbstract)
						{
							var implementsCallMapping = xRefs.CallMappings.add_Method(implementedMethod.Signature); 
							implementsCallMapping.Implements.Add(method.Signature.methodCall());												
						}
					}
					
				}
			xRefs.resolveMethodImplementations();	
			return xRefs;
		}
		
		// this will add extra mappings that handle the cases where a classA extends classB and then method X(...) (that is implemented in class B)
		// is called using the convension classA.X(...)
		public static JavaMetadata_XRefs resolveMethodImplementations(this JavaMetadata_XRefs xRefs)
		{
			foreach(var _class in xRefs.Classes_MappedTo_Implementations) 
			{ 
				var baseClass = _class.Key;	
				foreach(var implementedClass in _class.Value) 	
					if (xRefs.Classes_by_Signature.hasKey(baseClass))
						foreach(var method in xRefs.Classes_by_Signature[baseClass].Methods)
						{
							var rootClassMethod = method.Signature;
							var methodToFind = rootClassMethod.replace(method.ClassName,implementedClass.Signature); 
																					
							if (xRefs.CallMappings.hasKey(methodToFind))							
							{
								if (xRefs.CallMappings[methodToFind].Implements.signatures().contains(rootClassMethod).isFalse())
									xRefs.CallMappings[methodToFind].ImplementedBy.Add(rootClassMethod.methodCall()); 													
							}
							if (xRefs.CallMappings.hasKey(rootClassMethod)) 							
							{
								if (method.IsAbstract.isFalse())			
									xRefs.CallMappings[rootClassMethod].Implements.Add(methodToFind.methodCall()); 							
								else								
								{
									//if (xRefs.CallMappings[rootClassMethod].Implements.signatures().contains(methodToFind).isFalse())
									xRefs.CallMappings[rootClassMethod].ImplementedBy.Add(methodToFind.methodCall()); 									
								}																															
							}
								
						}	
			}
			return 	xRefs;	
		}
		 
		public static Method_CallMapping add_Method(this Dictionary<string, Method_CallMapping> callMappings , string signature)
		{			
			//return existing ones
			if (callMappings.hasKey(signature))
				return callMappings[signature];
			
			//or create it
			var callMapping = signature.callMapping();
			
			callMappings.Add(signature, callMapping);
			return callMapping;
		}
		
		public static List<MethodCall> get_MethodsRef_FromContantsPool(this List<ConstantPool> constantsPool,string file, bool isAbstract, Dictionary<int,List<Java_Instruction>> methodRefs_AtLines)
		{
			var methodRefs = new List<MethodCall>();
			foreach(var constantPool in constantsPool)
			if (constantPool.Id>0)
			{
				var methodCall = constantPool.Value.methodCall();
				methodCall.IsAbstract = isAbstract;
				foreach(var instruction in methodRefs_AtLines[constantPool.Id])									
					if (instruction.OpCode.starts("__invoke"))
						methodCall.AtLocations.Add(new MethodCall_Location { File = file, Line = instruction.Line_Number , Pc = instruction.Pc} );				
				methodRefs.Add(methodCall);						
			}	
			return methodRefs;
		}
		
		public static List<string> signatures(this List<MethodCall> methodCalls)
		{
			return (from methodCall in methodCalls
					select methodCall.Signature).toList();
		}
		
		public static List<MethodCall> methodRefs(this Java_Method method,  Java_Class methodClass)
		{
			var file = methodClass.file();
			var constants_byType = method.constantsPool_byType(methodClass);			
			if (constants_byType.hasKey("Methodref") || constants_byType.hasKey("InterfaceMethodref"))
			{
				var methodRefs_AtLines = method.getConstantsPoolUsage_byIndex_WithLineNumbers();
				var methodRefs = new List<MethodCall>();
				
				if (constants_byType.hasKey("Methodref"))
					methodRefs.AddRange(constants_byType["Methodref"].get_MethodsRef_FromContantsPool(file, false,  methodRefs_AtLines));
					
				if (constants_byType.hasKey("InterfaceMethodref"))
					methodRefs.AddRange(constants_byType["InterfaceMethodref"].get_MethodsRef_FromContantsPool(file, true,  methodRefs_AtLines));
				return methodRefs;
			}
			return new List<MethodCall>();
		}
		
		public  static MethodCall regExSignature(this List<MethodCall> methodCalls, string value)
		{
			foreach(var methodCall in methodCalls)
				if (methodCall.Signature.regEx(value) || methodCall.Signature.contains(value))
					return methodCall;
			return null;
		}
		
		public static List<string> methodRefs_old(this Java_Class methodClass, Java_Method method)
		{
			var constants_byType = method.constantsPool_byType(methodClass);
			if (constants_byType.hasKey("Methodref"))
				return constants_byType["Methodref"].values();
			return new List<string>();
		}
		
		public static Method_CallMapping regEx(this Dictionary<string, Method_CallMapping> callMappings, string value)
		{
			foreach(var callMapping in callMappings.Values)
				if (callMapping.Signature.regEx(value))
					return callMapping;
			return null;
		}
		
	}	
	
	public static class Method_CallMapping_ExtensionMethods_Gui_Helpers
	{			
		public static TreeView add_CallMappings(this TreeView treeView, 
												JavaMetadata_XRefs javaXRefs,
												string filter, 
												bool show_CallsTo,
												bool show_IsCalledBy,
												bool show_ImplementedBy,
												bool show_Implements,
												bool show_EnclosingMethod)
		{
			Func<Method_CallMapping, bool> hasChildNodes = 
				(value)=>{	
							return (show_CallsTo    		&& javaXRefs.CallMappings.hasMapping_CallsTo(value.Signature)) || 
								   (show_IsCalledBy 		&& javaXRefs.CallMappings.hasMapping_IsCalledBy(value.Signature)) ||
								   (show_ImplementedBy 		&& javaXRefs.CallMappings.hasMapping_ImplementedBy(value.Signature)) ||
								   (show_Implements 		&& javaXRefs.CallMappings.hasMapping_Implements(value.Signature)) ||								   
								   (show_EnclosingMethod    && javaXRefs.hasMapping_EnclosingMethod(value.Signature));
						 };
			var filteredValues = javaXRefs.CallMappings
										  .Values
										  .OrderBy((value)=>value.Signature)
										  .Where((value)=> hasChildNodes(value) &&
													   	   (filter.inValid() ||
													   		value.Signature.regEx(filter) ||
													   		value.Signature.contains(filter)
													   	   )).toList();

			treeView.add_Nodes(filteredValues,
							   (value) => value.Signature,
							   (value) => value,
							   (value) => hasChildNodes(value),
							   (value) => value.Signature.treeNodeColor(javaXRefs.Methods_by_Signature));
			return treeView;
		}
	
		public static bool hasMapping_CallsTo(this Dictionary<string, Method_CallMapping> callMappings, string signature)
		{
			return callMappings.hasKey(signature).isTrue() && callMappings[signature].CallsTo.size()>0;
		}
		
		public static bool hasMapping_IsCalledBy(this Dictionary<string, Method_CallMapping> callMappings, string signature)
		{
			return callMappings.hasKey(signature).isTrue() && callMappings[signature].IsCalledBy.size()>0;
		} 
		
		public static bool hasMapping_ImplementedBy(this Dictionary<string, Method_CallMapping> callMappings, string signature)
		{
			return callMappings.hasKey(signature).isTrue() && callMappings[signature].ImplementedBy.size()>0;
		}
		
		public static bool hasMapping_Implements(this Dictionary<string, Method_CallMapping> callMappings, string signature)
		{
			return callMappings.hasKey(signature).isTrue() && callMappings[signature].Implements.size()>0;
		}
		
		public static bool hasMapping_EnclosingMethod(this JavaMetadata_XRefs javaXRefs, string signature)
		{
			return javaXRefs.Classes_by_EnclosingMethod.hasKey(signature) ||
				   javaXRefs.Method_by_EnclosingMethod.hasKey(signature);
		}
			
		public static TreeNode add_CallsTo(this TreeNode treeNode, Method_CallMapping callMapping, Dictionary<string, Method_CallMapping> callMappings , Dictionary<string, Java_Method> methods_bySignature , bool showStubTreeNode)
		{	
			if (callMapping.CallsTo.size()==0)
				return treeNode;
				
			//var callToLocations = treeNode.add_Node("_CallTo_Locations");														
			
			var locations = new List<MethodCall_Location>();
			foreach(var callTo in callMapping.CallsTo)
				foreach(var atLocation in callTo.AtLocations)				
				{
					atLocation.Signature = callTo.Signature;		// did this so that we don't have to store/serialize the Signature here
					locations.Add(atLocation);					
				}
			locations = locations.OrderBy((atLocation)=>atLocation.Pc)
								 .OrderBy((atLocation)=>atLocation.Signature).toList();	
			
			//callToLocations.add_Nodes(locations);
											
			var callsToNode = (showStubTreeNode) 
									? treeNode.add_Node("CallsTo") 
									: treeNode;
			
			callsToNode.add_Node("_CallTo_Locations")
						.add_Nodes(locations);												
			
			callsToNode.add_Nodes( callMapping.CallsTo.OrderBy((callTo)=>callTo.Signature),
								   (callTo) => callTo.Signature,
								   (callTo) => callMappings[callTo.Signature], 								   
								   (callTo) => true, // callMappings.hasMapping_CallsTo(callTo.Signature), 
								   (callTo) => callTo.Signature.treeNodeColor(methods_bySignature));								   						
			return treeNode;
		}				
		
		public static TreeNode add_IsCalledBy(this TreeNode treeNode, Method_CallMapping callMapping, Dictionary<string, Method_CallMapping> callMappings , Dictionary<string, Java_Method> methods_bySignature , bool showStubTreeNode)		
		{			
			if (callMapping.IsCalledBy.size()==0)
				return treeNode;

			//var isCalledByLocations = treeNode.add_Node("_IsCalledBy_Locations");																		
			var locations = new List<MethodCall_Location>();
			foreach(var isCalledBy in callMapping.IsCalledBy)
				foreach(var atLocation in isCalledBy.AtLocations)				
				{
					atLocation.Signature = isCalledBy.Signature;		// did this so that we don't have to store/serialize the Signature here
					locations.Add(atLocation);					
				}
			locations = locations.OrderBy((atLocation)=>atLocation.Pc)
								 .OrderBy((atLocation)=>atLocation.Signature).toList();	
			
			//isCalledByLocations.add_Nodes(locations);
		
			var isCalledByToNode = (showStubTreeNode) 
										? treeNode.add_Node("IsCalledBy").color(Color.Gray)
										: treeNode;
			
			isCalledByToNode.add_Node("_IsCalledBy_Locations").color(Color.Gray)
							.add_Nodes(locations);
			
			isCalledByToNode.add_Nodes(callMapping.IsCalledBy
												  .OrderBy((calledBy)=>calledBy.Signature)
												  .Where((calledBy)=>callMappings.hasKey(calledBy.Signature)),
									   (calledBy) => calledBy.Signature,
									   (calledBy) => callMappings[calledBy.Signature],
									   (calledBy) => true, //callMappings.hasMapping_IsCalledBy(calledBy.Signature), 
									   (calledBy) => methods_bySignature.hasKey(calledBy.Signature) 
										   				? Color.DarkGreen
										   				: Color.DarkRed);
			return treeNode;
		}
		
		public static TreeNode add_ImplementedBy(this TreeNode treeNode, Method_CallMapping callMapping, Dictionary<string, Method_CallMapping> callMappings , Dictionary<string, Java_Method> methods_bySignature , bool showStubTreeNode)		
		{			
			if (callMapping.ImplementedBy.size()==0)
				return treeNode;
			var implementedByNode = (showStubTreeNode) 
										? treeNode.add_Node("ImplementedBy")
										: treeNode;	
										
			implementedByNode.add_Nodes(callMapping.ImplementedBy
												   .OrderBy((implementedBy)=>implementedBy.Signature)
												   .Where((implementedBy)=>callMappings.hasKey(implementedBy.Signature)),
									   (implementedBy) => implementedBy.Signature,
									   (implementedBy) => callMappings[implementedBy.Signature],
									   (implementedBy) => true, //callMappings.hasMapping_ImplementedBy(implementedBy.Signature), 
									   (implementedBy) => implementedBy.Signature.treeNodeColor(methods_bySignature));
										   						
			return treeNode;
		}
		
		public static TreeNode add_Implements(this TreeNode treeNode, Method_CallMapping callMapping, Dictionary<string, Method_CallMapping> callMappings , Dictionary<string, Java_Method> methods_bySignature , bool showStubTreeNode)		
		{			
			if (callMapping.Implements.size()==0)
				return treeNode;
			var implementsNode = (showStubTreeNode) 
										? treeNode.add_Node("Implements").color(Color.Gray)
										: treeNode;	
			
			implementsNode.add_Nodes(callMapping.Implements
											 	.OrderBy((implements)=>implements.Signature)
											 	.Where((implements)=>callMappings.hasKey(implements.Signature)),
									 (implements) => implements.Signature,
									 (implements) => callMappings[implements.Signature],
									 (implements) => true, //callMappings.hasMapping_ImplementedBy(implementedBy.Signature), 
									 (implements) => implements.Signature.treeNodeColor(methods_bySignature));
										   						
			return treeNode;
		}
		
		
		
		public static TreeNode add_EnclosingMethod(this TreeNode treeNode, Method_CallMapping callMapping,JavaMetadata_XRefs xRefs, bool showStubTreeNode)
		{
			if (xRefs.Classes_by_EnclosingMethod.hasKey(callMapping.Signature))
			{
				var enclosedMethodNode = (showStubTreeNode) 
											? treeNode.add_Node("_EnclosedClass (i.e. class by EnclosedMethod)")
											: treeNode;	
				foreach(var _class in xRefs.Classes_by_EnclosingMethod[callMapping.Signature])
				{
					enclosedMethodNode.add_Node(_class.Signature, _class)
									  .add_Nodes(_class.Methods,
									  			 (method)=> method.Signature,
									  			 (method) => xRefs.CallMappings[method.Signature],
											   	 (method) => true, //callMappings.hasMapping_ImplementedBy(implementedBy.Signature), 
											   	 (method) => method.Signature.treeNodeColor(xRefs.Methods_by_Signature));
				}
			}
			
			if (xRefs.Method_by_EnclosingMethod.hasKey(callMapping.Signature))
			{
				var enclosedMethodNode = (showStubTreeNode) 
											? treeNode.add_Node("_EnclosedMethod").color(Color.Gray)
											: treeNode;	
				var enclosedMethod = xRefs.Method_by_EnclosingMethod[callMapping.Signature];
				
				enclosedMethodNode.add_Node(enclosedMethod, 
											xRefs.CallMappings[enclosedMethod],
											true)
								  .color(enclosedMethod.treeNodeColor(xRefs.Methods_by_Signature));
			}
			
			//enclosedMethodNode.add_Nodes(xRefs.Classes_by_EnclosingMethod[callMapping.Signature].OrderBy((enclosedMethod)=>enclosedMethod.Signature));
			/*EnclosedMethodNode.add_Nodes(xRefs.Classes_by_EnclosingMethod[callMapping.Signature].OrderBy((enclosedMethod)=>enclosedMethod.Signature),
									   	(enclosedMethod) => enclosedMethod.Signature,
									   	(enclosedMethod) => xRefs.CallMappings[enclosedMethod.Signature],
									   	(enclosedMethod) => true, //callMappings.hasMapping_ImplementedBy(implementedBy.Signature), 
									   	(enclosedMethod) => enclosedMethod.Signature.treeNodeColor(xRefs.Methods_by_Signature));
			*/
			
			return treeNode;
		}
			
		public static Color treeNodeColor(this string signature, Dictionary<string, Java_Method> methods_bySignature)
		{
			return methods_bySignature.hasKey(signature) 
		   				? methods_bySignature[signature].IsAbstract
		   						? Color.DarkBlue 
		   						: Color.DarkGreen
		   				: Color.DarkRed;
		}
		
/*		public static TreeNode add_InterfaceImplementations(this TreeNode treeNode, Method_CallMapping callMapping, Dictionary<string, Method_CallMapping> callMappings , Dictionary<string, Java_Method> methods_bySignature, Dictionary<string, List<Java_Class>> classes_MappedTo_Implementations)
		{									
			if (methods_bySignature.hasKey(callMapping.Signature))
			{
				var method = methods_bySignature[callMapping.Signature];
				if (method.IsAbstract)
				{
					var implementations = method.implementations(classes_MappedTo_Implementations);
					var implementationsNode = treeNode.add_Node("_Implementations");
					if (implementations.size()==0)
						implementationsNode.add_Node("..no implementations found..");
					else
						foreach(var implementation in implementations)
							if (callMappings.hasKey(implementation.Signature))
							{
								var callMappingImplementation = callMappings[implementation.Signature];
								implementationsNode.add_Node(callMappingImplementation.str(), callMappingImplementation, true)
												   .color(Color.Sienna);
							}
							else
								implementationsNode.add_Node(implementation.Signature);
					
				}
			}
			return treeNode;
		}
*/		

		public static JavaMetadata_XRefs showInCodeViewer(this JavaMetadata_XRefs xRefs, ascx_SourceCodeViewer codeViewer,  string signature)
		{
			if (xRefs.Methods_by_Signature.hasKey(signature))
			{
				var method = xRefs.Methods_by_Signature[signature];
				var _class = xRefs.Classes_by_Signature[method.ClassName];
				codeViewer.showInCodeViewer(_class,method);				
			}
			return xRefs;
		}
	}	
}