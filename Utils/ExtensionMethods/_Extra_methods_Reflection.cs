// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Xml;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;  
using System.Collections.Generic;
using System.ComponentModel;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;

//O2File:_Extra_methods_Items.cs
//O2File:_Extra_methods_Files.cs

namespace O2.XRules.Database.Utils
{	
	public static class _Extra_Reflection_ExtensionMethods
	{	
		//methods
		public static List<MethodInfo> methods(this Type type, string methodName)
		{
			return (from method in type.methods()
					where method.Name == methodName
					select method).toList();
		}
		
		public static MethodInfo method_bySignature(this Type type, string methodSignature)
		{
			return (from method in type.methods()
					where method.str() == methodSignature
					select method).first();
		}
		
		//referencedAssemblies
		public static List<AssemblyName> referencedAssemblies(this AssemblyName assemblyName)
		{
			return assemblyName.assembly().referencedAssemblies();
		}
		
		public static List<AssemblyName> referencedAssemblies(this Assembly assembly, bool recursiveSearch)
		{
			return assembly.referencedAssemblies(recursiveSearch, true);
		}
		 public static List<AssemblyName> referencedAssemblies(this Assembly assembly, bool recursiveSearch, bool removeGacEntries)
		{			
			var mappedReferences = new List<string>();
			var resolvedAssemblies = new List<AssemblyName>();
			
			Action<List<AssemblyName>> resolve = null;
			
			resolve =  (assemblyNames)=>{
											if (removeGacEntries)
												assemblyNames = assemblyNames.removeGacAssemblies();
											if (assemblyNames.isNull())
												return;
											foreach(var assemblyName in assemblyNames)
											{							
												if (mappedReferences.contains(assemblyName.str()).isFalse())															
												{ 
													mappedReferences.add(assemblyName.str());
													resolvedAssemblies.add(assemblyName);
													resolve(assemblyName.referencedAssemblies()); 
												}
											}
										}; 
			
			resolve(assembly.referencedAssemblies());
			
			"there where {0} NonGac  assemblies resolved for {1}".debug(resolvedAssemblies.size(), assembly.Location);
			return resolvedAssemblies;
		}
		
		public static List<AssemblyName> removeGacAssemblies(this List<AssemblyName> assemblyNames)
		{			
			var systemRoot = Environment.GetEnvironmentVariable("SystemRoot");
			return (from assemblyName in assemblyNames
					let assembly = assemblyName.assembly()
					where assembly.notNull() && assembly.Location.starts(systemRoot).isFalse()
					select assemblyName).toList();
		}
		
		public static List<string> locations(this List<AssemblyName> assemblyNames)
		{
			
			var locations = new List<string>();
			try
			{
				foreach(var assemblyName in assemblyNames)
				{
					var location = assemblyName.assembly().Location;					
					locations.add(location);
				}
			}
			catch(Exception ex)
			{
				"[Reflection] locations, could not resolve {0}".error(ex.Message);
			}
			return locations;
		}
				
		public static PortableExecutableKinds assembly_PortableExecutableKind(this string assemblyLocation)
		{
			return Assembly.ReflectionOnlyLoadFrom(assemblyLocation).portableExecutableKind();
		}
		public static PortableExecutableKinds portableExecutableKind(this Assembly assembly)
		{ 
	        PortableExecutableKinds peKind;
	        ImageFileMachine imageFileMachine;
	        assembly.ManifestModule.GetPEKind(out peKind, out imageFileMachine);
	        return peKind;
        }
        
		public static string value(this PortableExecutableKinds peKind)
		{			
			switch (peKind)
		     {
		         case PortableExecutableKinds.ILOnly:
		             return "AnyCPU";
		         case PortableExecutableKinds.Required32Bit:		         
		             return"x86";		             
		         case PortableExecutableKinds.PE32Plus:
		            return "x64";
		        case PortableExecutableKinds.Unmanaged32Bit:
		            return "Unmanaged32Bit";
		        case PortableExecutableKinds.NotAPortableExecutableImage:
		            return "NotAPortableExecutableImage";		            
		        default:
		        	return peKind.str();
		            //throw new ArgumentOutOfRangeException();
		    }
		}
	}		

	public static class _Extra_Processes_ExtensionMethods
	{
		public static Process with_Name(this List<Process> processes, string name)
		{
			return (from process in processes
					where process.ProcessName == name
					select process).first();
		}
		
		public static Process with_Id(this List<Process> processes, int id)
		{
			return (from process in processes
					where process.Id == id
					select process).first();
		}
	}
	
	public static class _Extra_Compilation_ExtensionMethods
	{
		public static Assembly compileScriptFile(this string scriptToCompile)
		{
			if (scriptToCompile.valid())
			{				
				if(scriptToCompile.fileExists().isFalse())
					scriptToCompile = scriptToCompile.local();
				if(scriptToCompile.fileExists())				
					return (scriptToCompile.extension(".h2"))
								? scriptToCompile.compile_H2Script()
								: scriptToCompile.compile();
			}
			"[compileScriptFile] could not find file to compile: {0}".error(scriptToCompile);
			return null;			
		}
	}	
	//processes 
}
    	