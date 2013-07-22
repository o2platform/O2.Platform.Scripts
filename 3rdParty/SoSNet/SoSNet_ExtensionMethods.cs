using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using SOS.Net;
using SOS.Net.Core;
using SOS.Net.Core.Cdb;
using SOS.Net.Core.Cdb.Commands;

//Installer:Installer_SosNet.cs!SosNet\Debug\SOS.Net.Core.dll
//O2Ref:SosNet\Debug\SOS.Net.Core.dll
//O2Ref:SosNet\Debug\SOS.Net.exe

namespace O2.XRules.Database.Utils
{
	//these are not thread safe
	public static class SoSNet_ExtensionMethods_SosController_DirectExecution 
	{
		public static SosController sosController;
		
		public static SosController setController(this SosController _sosController)
		{
			return _sosController.setDefaultController();
		}
		public static SosController setDefaultController(this SosController _sosController)
		{
			sosController = _sosController;
			return sosController;
		}
		
		public static string sosExec(this string commandToExecute)
		{
			return sosController.executeCommand(commandToExecute);
		}
		
		
		public static List<string> instances(this TypeInfo typeInfo)
		{			
			return sosController.types_In_Heap_Raw(typeInfo.ClassName);
		}
		
		public static InstanceInfoDetails  instance_Details(this string instanceAddress)
		{
			return sosController.instance_Details(instanceAddress);
		}
		
		public static InstanceInfoDetails  instance_Details(this TypeInfo typeInfo)
		{
			return sosController.instance_Details(typeInfo.Address);
		}
		
		public static List<InstanceFieldInfo>  instance_Fields(this TypeInfo typeInfo)
		{
			return sosController.instance_Fields(typeInfo.Address);
		}
		
		public static List<string> names(this  List<InstanceFieldInfo>  instanceFields)
		{
			return (from instanceField in instanceFields
					select instanceField.TypeName).toList();
		}
		
		public static List<InstanceFieldInfo>  instance_Fields(this string instanceAddress)
		{
			return sosController.instance_Fields(instanceAddress);
		}
		
		public static InstanceFieldInfo  instance_Field(this string instanceAddress, string name )
		{
			return sosController.instance_Field(instanceAddress, name);
		}
		
		public static string  dumpObject(this string objectAddress)
		{
			return sosController.dumpObject_Raw(objectAddress);
		}
		
	}
	
	public static class SoSNet_ExtensionMethods_SosController
	{	
		public static SosController loadSettings(this SosController sosController)
		{
			var settings = new CdbSettings()
								{
									CdbPath = (clr.x64())
									? @"C:\Program Files\Windows Kits\8.0\Debuggers\x64"
									: @"C:\Program Files (x86)\Windows Kits\8.0\Debuggers\x86"
								};
					
			sosController.field("settings",settings);
			sosController.setDefaultController();
			return sosController;
		}
		public static SosController attach(this SosController sosController, int processId)
		{
			"[SosController] attaching to process with ID: {0}".info(processId);
			sosController.loadSettings();										
			sosController.AttachToProcess(processId.str()); 
			
			sosController.loadSoS();	
			return sosController;
		}
	
		public static SosController loadSoS(this SosController sosController)
		{
			var clr2 = clr.clr2();
			var x64 = clr.x64();
			return sosController.loadSoS(clr2,x64);
		}
		
		public static SosController loadSoS(this SosController sosController, bool clr2, bool x64)
		{
			var sosFile = (clr2) ?	(x64) ? @"C:\Windows\Microsoft.NET\Framework64\v2.0.50727\sos.dll"	
										  : @"C:\Windows\Microsoft.NET\Framework\v2.0.50727\sos.dll"
								 :	(x64) ? @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\sos.dll"	
										  : @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\sos.dll";													  					
			return sosController.execute(".load " + sosFile);	
		}
		
		public static CdbProcess cdbProcess(this SosController sosController)
		{
			return (CdbProcess)sosController.field("cdb");
		}
		
		public static Process cdb(this CdbProcess cdbProcess)
		{
			return (Process)cdbProcess.field("cdb");
		}
		
		public static string executeCommand(this SosController sosController, string commandToExecute)
		{
			string response = null;
			sosController.execute(commandToExecute, ref response);
			return response;
		}
		
		public static SosController execute(this SosController sosController, string commandToExecute)
		{
			string response = null;
			sosController.execute(commandToExecute, ref response);			
			return sosController;
		}
		public static SosController execute(this SosController sosController, string commandToExecute, ref string response)
		{
			//"[SosController] ExecuteCommand: {0} \n\n{1}".info(commandToExecute, response);
			response = sosController.ExecuteCommand(commandToExecute);
			return sosController;
		}	
		
		public static string sos_Help(this SosController sosController)
		{
			return sosController.executeCommand("!help");	
		}
		
		//this will hang the current thread until there is a breakpoint or Ctrl+C
		public static string go(this SosController sosController)
		{
			return sosController.executeCommand("g");	
		}
		
		public static SosController detach(this SosController sosController)
		{
			if (sosController.Attached)
				sosController.Detach();	
			return sosController;
		}
		
		public static SosController set_Breakpoint(this SosController sosController, string module, string method)
		{
			var command = "!bpmd {0} {1}".format(module,method);
			sosController.executeCommand(command).info();
			return sosController;
		}
		public static SosController breakpoints_Clear(this SosController sosController)
		{
			var command = "!bpmd -clearall";
			sosController.executeCommand(command).info();
			return sosController;			
		}
		
		public static string breakpoints(this SosController sosController)
		{
			return sosController.executeCommand("!bpmd -list");
		}
	}
	
	public static class SoSNet_ExtensionMethods_Stack
	{
		public static string eax(this SosController sosController)
		{
			try
			{
				sosController.setController();
				var eax =  "!dso".sosExec().split_onLines()[2]
							 .split(" ")
							 .removeEmpty()
							 .second();
				return eax;			 
			}
			catch(Exception ex)
			{
				"Prob getting eax".error();
				return null;
			}
			
		}
	}

	public static class SoSNet_ExtensionMethods_Data
	{
		public static List<T> toList<T>(this IEnumerable<CdbQueryable<T>> items)
		{
			return (from item in items
					select item.Value).toList();
		}
		
		public static List<AssemblyInfo> assemblies(this SosController sosController)
		{
			return  new AssemblyInfoCommand().Result(sosController.cdbProcess()).toList();
		}
		
		public static List<AssemblyInfo> notInGac(this List<AssemblyInfo> assemblies)
		{
			return assemblies.where((assembly)=>assembly.Name.contains(@"\assembly").isFalse());
		}
		
				
		public static List<TypeInfo> types_In_Heap(this SosController sosController)
		{
			return sosController.cdbProcess().GetTypes().toList();		
		}
		
		public static TypeInfo type_In_Heap(this SosController sosController, string name)
		{
			return sosController.types_In_Heap().where((typeInfo)=> typeInfo.ClassName == name).first();
		}			
		
		public static List<string> names(this List<TypeInfo> typeInfos)
		{
			return (from typeInfo in typeInfos
					select typeInfo.ClassName).toList();
		}		
		
		public static InstanceInfoDetails instance_Details(this SosController sosController, string instanceAddress)
		{
			return new  InstanceInfoDetailsCommand(instanceAddress).Result(sosController.cdbProcess()).toList().first();				
		}
		
		public static List<InstanceFieldInfo> instance_Fields(this SosController sosController, string instanceAddress)
		{
			return new  InstanceFieldInfoCommand(instanceAddress)
							.Result(sosController.cdbProcess())
							.toList()
							.where((instance)=> instance.FieldAddress.contains(">").isFalse()); //fix bug in SoSNet parsing code							
		}
		
		public static List<InstanceFieldInfo> strings(this List<InstanceFieldInfo> instancesFieldInfos)
		{
			return instancesFieldInfos.where((instance)=> instance.TypeName == "System.String");					
		}
		
		public static InstanceFieldInfo instance_Field(this SosController sosController,string instanceAddress, string name)
		{
			return sosController.instance_Fields(instanceAddress).name(name);
		}
		
		public static InstanceFieldInfo name(this List<InstanceFieldInfo> instancesFieldInfos , string name)
		{
			return instancesFieldInfos.where((instance)=> instance.FieldName == name).first();					
		}
	}
	
	public static class SoSNet_ExtensionMethods_Execute_Helpers
	{
		public static List<string> types_In_Heap_Raw(this SosController sosController, string typeName)
		{
			var result = sosController.executeCommand("!DumpHeap -short -type " + typeName);
			if (result.contains("Missing value for option -type"))
				return new List<string>();
			return result.split(" ").where((value)=> value.contains(">","-").isFalse());
		}
		
		public static string dumpObject_Raw(this SosController sosController, string instanceAddress)
		{
			return sosController.executeCommand("!DumpObj " + instanceAddress);
		}
	}
	
	public static class SoSNet_ExtensionMethods_Execute_Getters
	{
		public static object value(this InstanceFieldInfo instanceFieldInfo)
		{
			switch(instanceFieldInfo.TypeName)
			{
				case "System.String":
				{
					return instanceFieldInfo.Value.instance_Details().String.trim();
					return "a string";
				}
				case "System.Int32":
					return instanceFieldInfo.Value;
				default:
					return instanceFieldInfo.Value;
			}
		}
	}
	public static class SoSNet_ExtensionMethods_Execute_Settters
	{
		public static void set_Int32_Value(this string instanceAddress ,string offset,  Int32 value)
		{
			instanceAddress.set_Int32_Value(offset,value.hex());
		}
		
		public static void set_Int32_Value(this string instanceAddress ,string offset,  string value)
		{
		//public static InstanceFieldInfo set_Int32_Value(this InstanceFieldInfo instanceFieldInfo , Int32 value)
		//{							
			var setCommand = "ed {0}+{1} {2}".format(instanceAddress, offset, value);				
			setCommand.debug().sosExec();//.info();
		}		
		
		public static InstanceFieldInfo set_String_Value_DangerousWay(this InstanceFieldInfo instanceFieldInfo , string value)
		{
			if (instanceFieldInfo.TypeName == "System.String")
			{			
				var offset = clr.x64() ? "c" : "8";
				var setCommand = "ezu {0}+{1} \"{2}\"".format(instanceFieldInfo.Value, offset, value);				
				setCommand.sosExec();
			}	
			return instanceFieldInfo;
		}
	}
	
}