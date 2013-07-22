// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using NuGet;
using NuGet.Common;

//O2Ref:NuGet\NutGet.exe
//Installer:NuGet_Installer.cs!NuGet\NutGet.exe

namespace O2.XRules.Database.APIs
{
	public class API_NuGet
	{
		//public string NuGet_Exe { get; set;}
		//public string NuGet_Exe_DownloadUrl { get; set;}
		public Program NuGet_Program { get; set; }
		public Console NuGet_Console { get; set; }
		
		public API_NuGet()
		{
			//this.NuGet_Exe = PublicDI.config.ToolsOrApis
			//						 .pathCombine("NuGet").createDir()
			//						 .pathCombine("NuGet.exe");			
			
		}	
		
		public API_NuGet SetUp()
		{
			NuGet_Program = new Program();
			NuGet_Console = new Console();
            return this;
		}
		/*public string execute(string command)
		{
			return this.NuGet_Exe.startProcess_getConsoleOut(command);
		}*/
	}
	
	public static class API_NuGet_ExtensionMethods
	{
		
	}
	
	/*public static class API_NuGet_ExtensionMethods
	{		
		
		public static string list(this API_NuGet nuGet, string filter)
		{
			return nuGet.execute("list " + filter);
		}
		public static string install(this API_NuGet nuGet, string packageName)
		{
			return nuGet.execute("install " + packageName);
		}
		
		public static string setAPI(this API_NuGet nuGet, string apiKey)
		{
			return nuGet.execute("SetApiKey " + apiKey);
		}
		
		public static string pack(this API_NuGet nuGet, string pathToNuSpec)		
		{
			if (pathToNuSpec.fileExists())
				return nuGet.execute("Pack " + pathToNuSpec);
			return "[API_NuGet] could not find provided NuSpec file: {0}".error(pathToNuSpec);			
		}
		
		public static string push(this API_NuGet nuGet, string pathToNuSpec)		
		{
			if (pathToNuSpec.fileExists())
				return nuGet.execute("Push " + pathToNuSpec);
			return "[API_NuGet] could not find provided NuSpec file: {0}".error(pathToNuSpec);			
		}
	}*/
}

/*

panel.clear().add_ConsoleOut(false);	// adds a console out viewer

var program = new Program();
var console = new NuGet.Common.Console();
console.WriteLine("Testing NuGet Console out");

Action<Command> setupCommand = 
	(command)=>{
					//	this.Settings = Settings.LoadDefaultSettings(this.FileSystem);
					var defaultSettings = Settings.LoadDefaultSettings(command.FileSystem);
					command.prop("Settings", defaultSettings);	 	   
					
					//this.SourceProvider = PackageSourceBuilder.CreateSourceProvider(this.Settings);
					var sourceProvider = (IPackageSourceProvider)"NuGet.exe".assembly()
																			.type("PackageSourceBuilder")
																			.invokeStatic("CreateSourceProvider",
																						  defaultSettings);
					command.prop("SourceProvider", sourceProvider);
					
					//SettingsCredentialProvider defaultCredentialProvider 
					//	    = new SettingsCredentialProvider(new ConsoleCredentialProvider(this.Console), 
					//									     this.SourceProvider, this.Console);
					var defaultCredentialProvider = new SettingsCredentialProvider(
														    new ConsoleCredentialProvider(command.Console), 
														    sourceProvider, command.Console);
					
					HttpClient.DefaultCredentialProvider = defaultCredentialProvider;
					
					//this.RepositoryFactory = new CommandLineRepositoryFactory(this.Console);
					command.prop("RepositoryFactory", new CommandLineRepositoryFactory(command.Console));
				};

var tempDir = "_NuGet".tempDir(false);
var	fileSystem = new PhysicalFileSystem(tempDir);
program.invoke("Initialize", fileSystem, console);

var commands = program.Commands
			          .ToDictionary((command)=> command.CommandAttribute.CommandName);
var listCommand = (ListCommand)commands["list"];

setupCommand(listCommand);

var start = DateTime.Now;
"Starting download of Metadata".info();
var packages = listCommand.GetPackages().toList(); // force download of all metadata
"Metadata completed in {0} for {1} packages".debug(start.duration_to_Now(), packages.size());

packages.script_Me("_packages");

return "Continue on scriptMe";

//return "done. packages size = {0}    sharedPackageRepository size = {1}"
//			.format(packages.size(), sharedPackageRepository.GetPackages().size());


return "done";






Web.Https.ignoreServerSslErrors();

return listCommand.GetPackages().take(250);

"before".info();
var sw = new Stopwatch();
sw.Start();
//using System.Diagnostics
listCommand.GetPackages().take(5500);
"after".info();
sw.Stop();
return sw.Elapsed.str();



//using NuGet
//using NuGet.Common
//using NuGet.Commands
//O2Ref:NuGet/Nuget.exe

*/


/*

var tempDir = "_NuGet".tempDir(false);

var packages = (List<IPackage>)_packages;

var start = DateTime.Now;
var sharedPackageRepository = new SharedPackageRepository(tempDir);
var count = 0;
var errors = new List<IPackage>();
foreach(var package in packages)				// download the nupkg files
{
	try
	{
		"[{0}/{1}] fetching package {2}".info(++count, packages.size(), package.Id);
		sharedPackageRepository.AddPackage(package);
	}
	catch(Exception ex)
	{
		errors.add(package);
		ex.log("in package: {0}".info(package.Id));
	}
}

"Completed download of all packages in {0}".debug(start.duration_to_Now());
"There where {0} errors".error(errors.size());
return errors;
//using NuGet
//using NuGet.Common
//using NuGet.Commands
//O2Ref:NuGet/Nuget.exe
//O2Ref:mscorlib.dll
//O2Tag_SetInvocationParametersToDynamic
//O2Ref:Microsoft.CSharp.dll

*/