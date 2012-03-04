// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Xml;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Collections;  
using System.Collections.Generic;
using System.ComponentModel;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.Views.ASCX.classes.MainGUI;
using O2.Views.ASCX.ExtensionMethods;

//O2File:_Extra_methods_Items.cs
//O2File:_Extra_methods_Files.cs

namespace O2.XRules.Database.Utils
{	
	
	public static class _Extra_Compilation_ExtensionMethods
	{
	
		public static string compileIntoDll_inFolder(this string fileToCompile, string targetFolder)
		{
			"Compiling file: {0} ".debug(fileToCompile);
			//var fileToCompile = currentFolder.pathCombine(file + ".cs");
			var filenameWithoutExtension = fileToCompile.fileName_WithoutExtension();
			var compiledDll = targetFolder.pathCombine(filenameWithoutExtension + ".dll");
			var mainClass = "";
			if (fileToCompile.fileExists().isFalse()) 
				"could not find file to compile: {0}".error(fileToCompile);  
			else
			{ 
				var assembly = new CompileEngine().compileSourceFiles(new List<string> {fileToCompile}, 
																	  mainClass, 
																	  filenameWithoutExtension);
				if (assembly.isNull()) 
					"no compiled assembly object created for: {0}".error(fileToCompile);
				else
				{ 
					Files.Copy(assembly.Location, compiledDll);
					"Copied: {0} to {1}".info(assembly.Location, compiledDll);
					if (compiledDll.fileExists().isFalse())
						"compiled file not created in: {0}".error(compiledDll);
					else
						return compiledDll;
				}
			}  
			return null;
		}
		
		public static string compileToDll(this string sourceFile, string targetFolder)
		{
			return sourceFile.compileToExtension(".dll", "",targetFolder);
		}
		
		public static string compileToExe(this string sourceFile, string mainClass, string targetFolder)
		{
			return sourceFile.compileToExtension(".exe", mainClass,targetFolder);
		}
		
		public static string compileToExtension(this string sourceFile, string targetExtension, string mainClass, string targetFolder)
		{
			var name = sourceFile.fileName_WithoutExtension();
			return name.compileToExtension(targetExtension, mainClass, sourceFile.parentFolder(), targetFolder);			
		}
		
		public static string compileToExtension(this string name, string extension,string mainClass, string currentFolder, string targetFolder)
    	{            
            var fileToCompile = currentFolder.pathCombine(name + ".cs");
            var compiledDll = targetFolder.pathCombine(name + extension);
            if (fileToCompile.fileExists().isFalse())
                "could not find file to compile: {0}".error(fileToCompile); 
            else
            {
                var assembly = (mainClass.valid())
                                    ? new CompileEngine().compileSourceFiles(new List<string> {fileToCompile}, mainClass)
                                    : new CompileEngine().compileSourceFiles(new List<string> {fileToCompile}, mainClass, System.IO.Path.GetFileNameWithoutExtension(compiledDll));
                if (assembly.isNull())
                    "no compiled assembly object created for: {0}".error(fileToCompile);
                else
                {
                	if (compiledDll.fileExists())
                		compiledDll.deleteFile();                		
                    Files.Copy(assembly.Location, compiledDll);
                    "Copied: {0} to {1}".info(assembly.Location, compiledDll);
                    if (compiledDll.fileExists().isFalse())
                        "compiled file not created in: {0}".error(compiledDll);
                   
                }
            } 
            return compiledDll;
		}

		public static List<Assembly> copyAssembliesToFolder(this string targetFolder,  params string[] assemblyNames)
		{
			return targetFolder.copyAssembliesToFolder(true, assemblyNames);
		}
		
		public static List<Assembly> copyAssembliesToFolder(this string targetFolder , bool onlyCopyReferencesInO2ExecutionDir, params string[] assemblyNames)
		{
			var assembliesCopied = new List<Assembly>();
			foreach(var assemblyName in assemblyNames)
			{
				var assembly = assemblyName.assembly();
				if (assembly.notNull())
				{
					var location = assembly.Location;
					if (location.isNull())
						"[copyReferencesToTargetFolder] loaded assembly had no Location info: {0}".error(assembly.str());
					{
						if (onlyCopyReferencesInO2ExecutionDir.isFalse() || 
							location.parentFolder() == PublicDI.config.CurrentExecutableDirectory)
						{
							var targetFile = targetFolder.pathCombine(location.fileName());
							if (targetFile.fileExists())
								"[copyReferencesToTargetFolder] skipping copy since it already exists in target folder: {0}".info(targetFile);
							else
							{
								Files.Copy(location, targetFile);
								"[copyReferencesToTargetFolder] copied '{0}' to '{1}'".info(location, targetFile);
							}
							assembliesCopied.Add(assembly);
						}
					}																
				}
			}
			return assembliesCopied;
		}
	}
}
	