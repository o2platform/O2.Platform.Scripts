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
	}		


	
}
    	