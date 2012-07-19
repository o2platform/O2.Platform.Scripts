using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using O2.Views.ASCX.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
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
			return projectName.createProjectFile(sourceFile, pathToAssemblies, targetDir, new List<string>());
		}
		
		public static string createProjectFile(this string projectName, string sourceFile, string pathToAssemblies, string targetDir, List<string> extraEmbebbedResources)
		{
			sourceFile.file_Copy(targetDir);
			
			var assemblyFiles = pathToAssemblies.files(false,"*.dll","*.exe");
			var gzAssemblyFiles = new List<string>();
			
			foreach(var assemblyFile in assemblyFiles)
			{
				var gzFile = targetDir.pathCombine(assemblyFile.fileName() + ".gz");
				assemblyFile.fileInfo().compress(gzFile);	
				assemblyFile.file_Copy(targetDir);
				gzAssemblyFiles.add(gzFile);
			}
			
				
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
						
			foreach(var assemblyFile in assemblyFiles)
			{
                var assembly =  assemblyFile.fileName().assembly(); // first load from local AppDomain (so that we don't lock the dll in the target folder)
                if (assembly.isNull())
                    assembly  =  assemblyFile.assembly();
				//hack to only load the assemblies that are not strongly named (dealt with a  problem with loading/embedding Microsoft.cli.dll
                if (assembly.str().contains("PublicKeyToken=null"))
				{
					var item = references.AddItem("Reference",assemblyFile.fileName_WithoutExtension());
					item.AddMetadata("HintPath",assemblyFile.fileName()); 
					item.AddMetadata("Private",@"False");  
				}
			} 
			
			var embeddedResources = project.Xml.AddItemGroup();
			
			foreach(var assemblyFile in gzAssemblyFiles)				
				embeddedResources.AddItem("EmbeddedResource",assemblyFile.fileName()); 
			
			var defaultIcon = "O2Logo.ico";
			foreach(var extraResource in extraEmbebbedResources)
			{
				extraResource.file_Copy(targetDir);
				embeddedResources.AddItem("EmbeddedResource",extraResource.fileName()); 	
				if (extraResource.extension(".ico"))
					defaultIcon = extraResource;
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
			addSpecialResources(projectName + ".Data");
			
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
		
		public static string createProjectFile_and_Build(this string projectName, string sourceFile, string pathToAssemblies, string targetDir, List<string> extraEmbebbedResources)
		{
			var projectFile = projectName.createProjectFile(sourceFile, pathToAssemblies, targetDir, extraEmbebbedResources);
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
					.add_MenuItem("Test with: C# REPL Editor"							, true, ()=> onItemSelected("Util - C# REPL Script [4.0].cs.o2"))
					.add_MenuItem("Test with: Package O2 Script into separate Folder"	, true, ()=> onItemSelected("Util - Package O2 Script into separate Folder.h2"))		
					.add_MenuItem("Test with: HtmlAgilityPack - Filter Html Code"		, true, ()=> onItemSelected("HtmlAgilityPack - Filter Html Code.h2"))							
					.add_MenuItem("View available Scripts"								, true, ()=> "Util - O2 Available scripts.h2".executeFirstMethod());
			return control;					
		}
	}
}
		