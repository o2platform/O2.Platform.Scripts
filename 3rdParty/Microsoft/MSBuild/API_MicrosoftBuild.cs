using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using O2.Views.ASCX.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.Windows;
using O2.DotNetWrappers.ExtensionMethods;
using O2.External.SharpDevelop.ExtensionMethods;
using Microsoft.Build.Logging;
using Microsoft.Build.Evaluation;

//O2Ref:Microsoft.Build.Framework.dll
//O2Ref:Microsoft.Build.dll


namespace O2.XRules.Database.APIs
{	
	
	public class API_MicrosoftBuild
	{
		public string DefaultProgramFile = "Program_UseWith_O2_CreatedExes";		
	}
	
	public static class API_MicrosoftBuild_ExtensionMethods
	{
		public static string createProjectFile(this string projectName, string sourceFile, string pathToAssemblies, string targetDir)
		{
			return projectName.createProjectFile(sourceFile, pathToAssemblies, targetDir, new List<string>(),null,null);
		}
		
		public static string createProjectFile(this string projectName, string sourceFile, string pathToAssemblies, string targetDir, List<string> extraEmbebbedResources, Action<List<string>> beforeAddingReferences, Action<List<string>> beforeEmbedingFiles)
		{
			//O2.Kernel.debug._break();
			sourceFile.file_Copy(targetDir);
			var assemblyFiles = pathToAssemblies.files(false,"*.dll","*.exe");
											
			var projectFile =  targetDir.pathCombine(projectName + ".csproj");
			
			var projectCollection = new ProjectCollection();
			
			var outputPath = "bin";
			Project project = new Project(projectCollection);
			project.SetProperty("DefaultTargets", "Build");
		 	
			var propertyGroup = project.Xml.CreatePropertyGroupElement();
			project.Xml.InsertAfterChild(propertyGroup, project.Xml.LastChild);
			propertyGroup.AddProperty("TargetFrameworkVersion", "v4.0");
			propertyGroup.AddProperty("ProjectGuid", Guid.NewGuid().str());
			propertyGroup.AddProperty("OutputType", "WinExe");
			propertyGroup.AddProperty("OutputPath", outputPath);
			propertyGroup.AddProperty("AssemblyName", projectName);
			propertyGroup.AddProperty("PlatformTarget", "x86");
						
			var targets = project.Xml.AddItemGroup();
			targets.AddItem("Compile", sourceFile.fileName()); 
			
			
			var references = project.Xml.AddItemGroup();
			references.AddItem("Reference", "mscorlib");
			references.AddItem("Reference", "System");
			references.AddItem("Reference", "System.Core");
			references.AddItem("Reference", "System.Windows.Forms");
						
			beforeAddingReferences.invoke(assemblyFiles);
			foreach(var assemblyFile in assemblyFiles)
			{
                var assembly =  assemblyFile.fileName().assembly(); // first load from local AppDomain (so that we don't lock the dll in the target folder)
                if (assembly.isNull())
                    assembly  =  assemblyFile.assembly();
				//only load the O2 assemblies 				
                if (assembly.str().lower().contains("o2") || assembly.str().lower().contains("fluentsharp"))
				{
					var item = references.AddItem("Reference",assemblyFile.fileName_WithoutExtension());
					item.AddMetadata("HintPath",assemblyFile.fileName()); 
					item.AddMetadata("Private",@"False");  
				}
			} 
			
			var gzAssemblyFiles = new List<string>();
			beforeEmbedingFiles.invoke(assemblyFiles);
						
			/*foreach(var assemblyFile in assemblyFiles)
			{
				var gzFile = targetDir.pathCombine(assemblyFile.fileName() + ".gz");
				assemblyFile.fileInfo().compress(gzFile);	
				assemblyFile.file_Copy(targetDir);
				gzAssemblyFiles.add(gzFile);
			}*/			
			
			var embeddedResources = project.Xml.AddItemGroup();
						
			foreach(var assemblyFile in gzAssemblyFiles)				
				embeddedResources.AddItem("EmbeddedResource",assemblyFile.fileName()); 
			
			var defaultIcon = "O2Logo.ico";
			extraEmbebbedResources.add(assemblyFiles);
            
			foreach(var extraResource in extraEmbebbedResources)
			{
				if (extraResource.extension(".dll") || extraResource.extension(".exe"))
				{				
					//ignore these since they are already embded in the FluentSharp.REPL.exe dll
                    if(extraEmbebbedResources.fileNames().contains("FluentSharp.REPL.exe") && 
                       (extraResource.contains(
								//"WeifenLuo.WinFormsUI.Docking.dll",
                    			//"QuickGraph.dll",
                    			"Ionic.Zip.dll", "Mono.Cecil.dll" )))
                    {			
                        continue;										
                    }
					var gzFile = targetDir.pathCombine(extraResource.fileName() + ".gz");
					extraResource.fileInfo().compress(gzFile);	
					extraResource.file_Copy(targetDir);
					embeddedResources.AddItem("EmbeddedResource",gzFile.fileName()); 
				}
				else
				{
					extraResource.file_Copy(targetDir);
					embeddedResources.AddItem("EmbeddedResource",extraResource.fileName()); 					
					if (extraResource.extension(".ico"))
						defaultIcon = extraResource;
				}
			}			
			
			
			//add two extra folders (needs refactoring)
			Action<string> addSpecialResources = 
				(resourceFolder)=>{
										var folder = targetDir.pathCombine(resourceFolder);
										if (folder.dirExists())
										{
											"found {0} Folder so adding it as a zip:{1}".debug(resourceFolder, folder);
											var zipFile = folder.zip_Folder(folder + ".zip");				
											embeddedResources.AddItem("EmbeddedResource",zipFile.fileName()); 	
											if (folder.files("*.ico").size()>0)
											{
												var icon = folder.files("*.ico").first();
												"Found default application ICON: {0}".debug(icon);
												defaultIcon = icon;
											}											
										}
								   };
			addSpecialResources("O2.Platform.Scripts");			
			
			//now add the icon
			propertyGroup.AddProperty("ApplicationIcon", defaultIcon);
			
			var importElement = project.Xml.CreateImportElement(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");
			project.Xml.InsertAfterChild(importElement, project.Xml.LastChild);
			
			project.Save(projectFile);
			
			"O2Logo.ico".local().file_Copy(targetDir);
			
			return projectFile;
		}
		
		public static bool buildProject(this string projectFile, bool redirectToConsole = false)
		{
			try
			{
				var fileLogger = new FileLogger();
				var logFile = projectFile.directoryName().pathCombine(projectFile.fileName() + ".log");			
				fileLogger.field("logFileName", logFile);
				if (logFile.fileExists())
					logFile.file_Delete();
					
				var projectCollection = new ProjectCollection();
				var project = projectCollection.LoadProject(projectFile);
				if (project.isNull())
				{
					"could not load project file: {0}".error(projectFile);
					return false;
				}
				if (redirectToConsole)
					projectCollection.RegisterLogger(new ConsoleLogger());
					
				
				projectCollection.RegisterLogger(fileLogger);	
				var result = project.Build();
				fileLogger.Shutdown();
				return result;
			}
			catch(Exception ex)
			{
				ex.log();
				return false;
			}
			
		}
		
		public static string createProjectFile_and_Build(this string projectName, string sourceFile, string pathToAssemblies, string targetDir, List<string> extraEmbebbedResources,  Action<List<string>> beforeAddingReferences = null, Action<List<string>> beforeEmbedingFiles = null)
		{
			var projectFile = projectName.createProjectFile(sourceFile, pathToAssemblies, targetDir, extraEmbebbedResources, beforeAddingReferences ,beforeEmbedingFiles);
			var buildResult= projectFile.buildProject();
			if (buildResult)
				return targetDir.pathCombine("bin").pathCombine(projectName + ".exe");
			return null;
		}
		
		public static string createProjectFile_and_Build(this string projectName, string sourceFile, string pathToAssemblies, string targetDir, List<string> extraEmbebbedResources,  Panel panel)
		{
			var createdExe = projectName.createProjectFile_and_Build(sourceFile, pathToAssemblies, targetDir, extraEmbebbedResources);
			panel.showProjectBuildResult(projectName, targetDir, createdExe.valid());
			return createdExe;
		}
		
		public static TextBox showProjectBuildResult(this Control panel , string projectName , string targetDir, bool buildOk)
		{
			var logFile = targetDir.pathCombine(projectName + ".csproj.log");			
			return panel.clear().add_TextArea().wordWrap(false).set_Text(logFile.fileContents())
						.backColor(buildOk ? Color.LightGreen 
										   : Color.LightPink); 										      
		}
		
		public static T add_MenuItem_with_TestScripts<T>(this T control, Action<string> onItemSelected)
			where T : Control
		{
			control .add_ContextMenu()
					.add_MenuItem("Test with: LogViewer"								, true, ()=> onItemSelected("Util - LogViewer.h2"))
					.add_MenuItem("Test with: C# REPL Editor"							, true, ()=> onItemSelected("Util - C# REPL Script [4.0].h2"))
					.add_MenuItem("Test with: Package O2 Script into separate Folder"	, true, ()=> onItemSelected("Util - Package O2 Script into separate Folder.h2"))		
					.add_MenuItem("Test with: HtmlAgilityPack - Filter Html Code"		, true, ()=> onItemSelected("HtmlAgilityPack - Filter Html Code.h2"))							
					.add_MenuItem("View available Scripts"								, true, ()=> "Util - O2 Available scripts.h2".executeFirstMethod());
			return control;					
		}
		
		public static string package_Script(this string scriptFile)
		{
			var compiledScript = "";
			var pathToAssemblies = ""; 
			var projectFile = "";
			return scriptFile.package_Script(ref compiledScript, ref pathToAssemblies, ref projectFile);
		}
		 
		public static string package_Script(this string scriptFile, ref string compiledScript, ref string pathToAssemblies, ref string projectFile,   Action<List<string>> beforeAddingReferences = null, Action<List<string>> beforeEmbedingFiles = null)
		{	
		
			Action<string,List<string>> handleReferencesFor_ToolsOrApis = 
				(buildFilesDir,extraEmbebbedResources)=>{
									var toolsO2ApisFolder = buildFilesDir.pathCombine("_ToolsOrApis");
									toolsO2ApisFolder.copyToolReferencesToFolder(scriptFile);
									if (toolsO2ApisFolder.folders().size() > 0)
									{
										var zipFolder = buildFilesDir.pathCombine("_ToolsOrApis.zip");
										toolsO2ApisFolder.zip_Folder(zipFolder);
										extraEmbebbedResources.add(zipFolder);										
									}
								 };
			compiledScript =  scriptFile.compileScriptFile_into_SeparateFolder();		
			
			if (compiledScript.notNull())
			{									
				pathToAssemblies = compiledScript.directoryName();
				var buildFilesDir = pathToAssemblies.pathCombine("_BuildFiles").createDir();									
				
				//create wrapping exe using MicrosoftBuild
				var projectName = scriptFile.fileName_WithoutExtension();
				projectFile = buildFilesDir.pathCombine(projectName + ".csproj");
				var sourceFile = "Program_UseWith_O2_CreatedExes.cs".local();										
				
				//add special folders
				O2Setup.createEmbeddedFolder_Scripts(buildFilesDir)
					   .copyFileReferencesToFolder(scriptFile);
												
				var extraEmbebbedResources = buildFilesDir.mapExtraEmbebbedResources(scriptFile);
				extraEmbebbedResources.add(scriptFile.local()) // include original script as an embeded file
									  .add(sourceFile);		   //         and file that created the exe
				
				handleReferencesFor_ToolsOrApis(buildFilesDir,extraEmbebbedResources);
				
				var createdExe = projectName.createProjectFile_and_Build(sourceFile, pathToAssemblies, buildFilesDir,extraEmbebbedResources,beforeAddingReferences, beforeEmbedingFiles);				
				if (createdExe.valid())
				{
					createdExe.file_WaitFor_CanOpen();				
					Files.deleteAllFilesFromDir(pathToAssemblies);
					compiledScript = createdExe.file_Copy(pathToAssemblies);
					compiledScript = createdExe.file_Copy(buildFilesDir); // for now also copy it to the buildFileDir
					"CompiledScript: {0}".info(compiledScript);					
					return compiledScript;
				}										 	 			
			}			
			return null;
		}
	}
	
	
}
		