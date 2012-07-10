using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using O2.Views.ASCX.ExtensionMethods;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
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
			sourceFile.file_Copy(targetDir);
			var assemblyFiles = pathToAssemblies.files(false,"*.dll","*.exe");
			foreach(var assemblyFile in assemblyFiles)
				assemblyFile.file_Copy(targetDir);
				
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
			propertyGroup.AddProperty("PlatformTarget", "AnyCPU");
			
			
			var targets = project.Xml.AddItemGroup();
			targets.AddItem("Compile", sourceFile.fileName()); 
			
			
			var references = project.Xml.AddItemGroup();
			references.AddItem("Reference", "mscorlib");
			references.AddItem("Reference", "System");
			references.AddItem("Reference", "System.Core");
			references.AddItem("Reference", "System.Windows.Forms");
			
			foreach(var assemblyFile in assemblyFiles)
			{
				var item = references.AddItem("Reference",assemblyFile.fileName_WithoutExtension());
				item.AddMetadata("HintPath",assemblyFile.fileName()); 
				item.AddMetadata("Private",@"False");  
			} 
			
			var embeddedResources = project.Xml.AddItemGroup();
			
			foreach(var assemblyFile in assemblyFiles)
				embeddedResources.AddItem("EmbeddedResource",assemblyFile.fileName()); 
			
			var importElement = project.Xml.CreateImportElement(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");
			project.Xml.InsertAfterChild(importElement, project.Xml.LastChild);
			
			project.Save(projectFile);
			
			return projectFile;
		}
		
		public static bool buildProject(this string projectFile, bool redirectToConsole = false)
		{
			var projectCollection = new ProjectCollection();
			var project = projectCollection.LoadProject(projectFile);
			if (redirectToConsole)
				projectCollection.RegisterLogger(new ConsoleLogger());
				
			var fileLogger = new FileLogger();
			var logFile = projectFile.directoryName().pathCombine(projectFile.fileName() + ".log");			
			fileLogger.field("logFileName", logFile);
			
			projectCollection.RegisterLogger(fileLogger);	
			var result = project.Build();
			fileLogger.Shutdown();
			return result;
		}
		
		public static string createProjectFile_and_Build(this string projectName, string sourceFile, string pathToAssemblies, string targetDir)
		{
			var projectFile = projectName.createProjectFile(sourceFile, pathToAssemblies, targetDir);
			var buildResult= projectFile.buildProject();
			if (buildResult)
				return targetDir.pathCombine("bin").pathCombine(projectName + ".exe");
			return null;
		}
		
		public static string createProjectFile_and_Build(this string projectName, string sourceFile, string pathToAssemblies, string targetDir, Panel topPanel)
		{
			var createdExe = projectName.createProjectFile_and_Build(sourceFile, pathToAssemblies, targetDir);
			var logFile = targetDir.pathCombine(projectName + ".csproj.log");			
			topPanel.add_TextArea().wordWrap(false).set_Text(logFile.fileContents())
					.backColor((createdExe.valid()) ? Color.LightGreen 
										      : Color.LightPink); 
			return createdExe;
		}
	}
}
		