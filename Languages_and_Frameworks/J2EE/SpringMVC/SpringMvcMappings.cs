// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;
using O2.XRules.Database.Utils;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using www.springframework.org.schema.beans;
using O2.XRules.Database.APIs.IKVM;
//O2File:spring-servlet.xsd.cs
//O2Ref:O2_Misc_Microsoft_MPL_Libs.dll

//O2File:_Extra_methods_To_Add_to_Main_CodeBase.cs
//O2File:API_IKVMC_CallMapping.cs
//O2File:API_IKVMC_JavaMetadata.cs

 

namespace O2.XRules.Database.Languages_and_Frameworks.J2EE
{
	[Serializable]
    public class SpringMvcMappings
    {
        public string id { get; set; }
        public string XmlFile { get; set; }
        public List<SpringMvcController> Controllers { get; set; }        
        
        [XmlIgnore]
        public beans springBeans;
        
        public SpringMvcMappings()
        {
        	Controllers = new List<SpringMvcController> ();
        }
        
        public SpringMvcMappings(string xmlFile) : this()
        {
        	XmlFile = xmlFile;
        	springBeans = beans.Load(xmlFile); 
        }
        
    }
    
    [Serializable]
    public class SpringMvcController
    {
        public string JavaClass { get; set; }
        public string JavaFunction { get; set; }
        public string JavaClassAndFunction { get; set; }    
        public string HttpRequestUrl { get; set; }
        public string HttpRequestMethod { get; set; }
        public string HttpMappingParameter { get; set; }
        public List<SpringMvcParameter> AutoWiredJavaObjects { get; set; }
        public string CommandName { get; set; }
        public string CommandClass { get; set; }
        public string FileName { get; set; }
        public uint LineNumber { get; set; }        
        public Items Properties {get;set;}
     
     	public SpringMvcController()
     	{
     		Properties = new Items();
     		AutoWiredJavaObjects = new List<SpringMvcParameter>();
     	}
     	
     	public override string ToString()
     	{	
     		return this.HttpRequestUrl;
     	}     	     	
    }
            
    [Serializable]
    public class SpringMvcParameter
    {
        public string Name { get; set; }
        public string ClassName { get; set; }
        //public bool autoWiredObject { get; set; }
        public string autoWiredMethodUsed { get; set; }
    }
    
    
    public static class SpringMvcMappings_ExtensionMethods_Mapping
    {
    	public static SpringMvcMappings springMvcMappings(this String xmlFile)
    	{
    		var mvcMappings = new SpringMvcMappings(xmlFile);

			var urlBeans = mvcMappings.springBeans.urlBeans(); 
			var beans_byId = mvcMappings.springBeans.beans_byId();
			 
			foreach(var urlBean in urlBeans)
			{ 
				var controller = new SpringMvcController()
										{
											HttpRequestUrl = urlBean.name,
											JavaClass = urlBean.@class							
										};
				foreach(var _property in urlBean.property)	
					controller.Properties.add(_property.name, _property.value + 
															 (((_property.@ref).valid()) ? "ref:{0}".format(_property.@ref) : "" ));
				
				if (controller.JavaClass.inValid() && beans_byId.hasKey(urlBean.parent))
				{
					controller.JavaClass = beans_byId[urlBean.parent].@class;								
					foreach(var _property in beans_byId[urlBean.parent].property)	
						controller.Properties.add(_property.name, _property.value + 
															 (((_property.@ref).valid()) ? "ref:{0}".format(_property.@ref) : "" ));
				
				} 
				
				if (controller.JavaClass.valid())
					controller.FileName = "{0}.java".format(controller.JavaClass.replace(".","\\")); 
					
				controller.CommandName = controller.Properties["commandName"];
				controller.CommandClass= controller.Properties["commandClass"];  
				
				mvcMappings.Controllers.Add(controller); 
			}
			return mvcMappings;
    	}
    	
    	public static SpringMvcMappings mapCommandClass_using_XRefs(this SpringMvcMappings mvcMappings, string classFiles)
    	{
    		var xRefs =  classFiles.javaMetadata().get_XRefs();   
    		return mvcMappings.mapCommandClass_using_XRefs(xRefs);
    	}
    	
    	public static SpringMvcMappings mapCommandClass_using_XRefs(this SpringMvcMappings mvcMappings, JavaMetadata_XRefs xRefs)
    	{    		  
			var controllers_by_JavaClass = mvcMappings.controllers_by_JavaClass(); 
			
			foreach(var controller in mvcMappings.Controllers)
			{
				controller.mapCommandClass(xRefs);	
				if(controller.CommandName.notNull() && controller.CommandClass.isNull())
				{
					"CommandClass not found but CommandName exists: {0}".debug(controller);
					if (xRefs.Classes_by_Signature.hasKey(controller.JavaClass))
					{
						"Found XRefs for Class: {0}".debug(controller.JavaClass);
						var javaClass = xRefs.Classes_by_Signature[controller.JavaClass];
						controller.mapCommandClass(javaClass.SuperClass,xRefs);			
					}
				}
			}
    		return mvcMappings;	
    	}
    	
    	public static SpringMvcController mapCommandClass(this SpringMvcController controller, JavaMetadata_XRefs xRefs)
    	{
    		return controller.mapCommandClass(controller.JavaClass, xRefs);
    	}
    	
    	public static SpringMvcController mapCommandClass(this SpringMvcController controller, string javaClass, JavaMetadata_XRefs xRefs)
    	{
    		if (controller.CommandClass.isNull() && controller.JavaClass.valid()) 
			{ 		 
				 var _class = xRefs.JavaMetadata.java_Classes().signature(javaClass);				 
				 if (_class.notNull()) 
				 {
				 	if (controller.CommandName.isNull())
				 	{
				 		var initMethod = _class.java_Methods().name("<init>"); 		 	
						foreach(var methodRef in initMethod.methodRefs(_class))
							if (methodRef.Signature.contains("setCommandName"))
							{
								var location = methodRef.AtLocations[0];
								var instructions_byPc = initMethod.instructions_byPc(); 
								var targetIndex = instructions_byPc[location.Pc -2].Target_Index;					
								controller.CommandName =  _class.constantsPool_byIndex()[targetIndex].str();
								"resolved command name for {0} = {1}".info(controller.JavaClass, controller.CommandName);
								break;
							}
					}
					if (controller.CommandClass.isNull())
					{										
						var formBackingObject = _class.java_Methods().name("formBackingObject"); 
						if (formBackingObject.notNull())
						{
							var numberOfInstructions = formBackingObject.Instructions.size();
							if (formBackingObject.Instructions[numberOfInstructions-3].OpCode == "__aload" && 
								formBackingObject.Instructions[numberOfInstructions-2].OpCode == "__areturn")
							{
								
								var variablesByIndex = formBackingObject.variables_byIndex(); 
								var variableIndex = formBackingObject.Instructions[numberOfInstructions-3].Target_Index;
								if (variablesByIndex[variableIndex].size() > 0)	
								{
									controller.CommandClass = variablesByIndex[variableIndex].last().Descriptor.removeFirstChar().removeLastChar();
									"resolved CommandClass for CommandName {0} -> {1}".info(controller.CommandName, controller.CommandClass);
								}								
							}
						}											
					}						
				}		 				 
			}		
			return controller;
 	   }
	}
    
    
    public static class SpringMvcMappings_ExtensionMethods_getData
    {
    	public static List<beans.beanLocalType> urlBeans(this beans _beans)
    	{    		
    		return (from bean in _beans.bean	
			   	    where bean.name.notNull() && bean.name[0]== '/'
			   		select bean).toList();			
		}
		
		public static Dictionary<string, beans.beanLocalType> beans_byId(this beans _beans)
		{
			var beans_byID = new Dictionary<string, beans.beanLocalType>();
			foreach(var bean in _beans.bean)	
				beans_byID.Add(bean.id ?? bean.name.str(), bean);
			return beans_byID;
		}
		
		
		public static Dictionary<string, SpringMvcController> controllers_by_JavaClass(this SpringMvcMappings mvcMappings)
		{
			var controllers_by_JavaClass = new Dictionary<string, SpringMvcController>();
			foreach(var controller in mvcMappings.Controllers)
				controllers_by_JavaClass.add(controller.JavaClass, controller);
			return controllers_by_JavaClass;
		}
		
		public static Dictionary<string, List<SpringMvcController>> controllers_by_CommandClass(this SpringMvcMappings mvcMappings)
		{
			var byCommandClass = new Dictionary<string, List<SpringMvcController>>();

			foreach(var controller in mvcMappings.Controllers)
			{
				if (controller.CommandClass.valid())
					byCommandClass.add(controller.CommandClass, controller);
				//var commandClass= controller.CommandClass ?? "[no commandName]";
				//byCommandClass.add(commandClass, controller);
			}
			return byCommandClass;
		}
	}
	
	public static class SpringMvcMappings_ExtensionMethods_Gui_Helpers
	{
		public static TreeView add_TreeView_For_CommandClasses_Visualization(this Control control, JavaMetadata_XRefs xRefs)
		{
			var treeView = control.add_TreeView();   

			var showHttpName = false; 
			var configPanel = control.insert_Below(30);
			configPanel.add_CheckBox("Show Http Variable Name",0,0,(value)=> showHttpName = value)
					   .autoSize(); 
			
			Func<Java_Method,string> getMethodNodeText =
				(method) => {
								if (showHttpName)
									return method.Name
												 .subString(3)
												 .lowerCaseFirstLetter();
									
								return "{0}   {1}".format(method.Name, method.returnType());
								//.GenericSignature.isNull()
								//											? method.ParametersAndReturnType
								//											: method.GenericSignature.returnType());
							};
			Action<TreeNode, string> add_Getters = 
				(treeNode, className)=> {
											if (xRefs.Classes_by_Signature.hasKey(className))
											{									
												var _class = xRefs.Classes_by_Signature[className];  
												var getters = _class.java_Methods_Getters_Public().with_Primitive_ReturnType(false); 
												
												treeNode.add_Nodes( getters.Where((method)=>method.returnType()!="java.lang.String"),   
																	(method)=> getMethodNodeText(method),
																	//(method)=> method.str(),
																	(method)=> method.returnType(), 
																	(method)=> true, 
																	(method)=> Color.DarkBlue);
			
											}
											else
												treeNode.add_Node("[Getters] ... class not found: {0}".format(className))
														.color(Color.DarkRed);
										}; 
			  
			Action<TreeNode, string> add_Setters =  
				(treeNode, className)=> { 
											if (xRefs.Classes_by_Signature.hasKey(className))
											{									 
												var _class = xRefs.Classes_by_Signature[className];     
												var setters = _class.java_Methods_Setters_Public().with_Primitive_Parameter(true);  
												
												treeNode.add_Nodes( setters, 
																	//(method)=> "{0}   {1}".format(method.Name, method.ParametersAndReturnType), 
																	(method)=> getMethodNodeText(method),
																	(method)=> method ,
																	(method)=> false,
																	(method)=> Color.DarkGreen);
			
											}
											else
												treeNode.add_Node("[Setters] ... class not found: {0}".format(className))
														.color(Color.DarkRed);
										};
										
			
			treeView.beforeExpand<string>(
				(treeNode, returnType)=>{	
											var className = (returnType.starts("L"))
																? returnType.removeFirstChar().removeLastChar()
																: returnType;
											add_Getters(treeNode, className);
											add_Setters(treeNode, className);								
											//if (xRefs.Classes_by_Signature.hasKey(className))
											//	treeNode.add_Node("FOUND Class!!!");
											//else
											//	treeNode.add_Node(returnType);					
							  			});
			return treeView;							  			
		}
		
	}
}
