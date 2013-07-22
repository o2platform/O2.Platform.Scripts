using System.Diagnostics;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;
using FluentSharp.FluentRoslyn;
using FluentSharp.Git;
using FluentSharp.REPL;
using FluentSharp.WinForms;

//O2File:Tool_API.cs
//O2File:_Extra_methods_Roslyn_API.cs
//O2File:API_MSBuild.cs

//O2Ref:FluentSharp.NGit.dll
//O2Ref:Mono.Posix.dll
//O2Ref:ICSharpCode.SharpZipLib.dll 
//O2Ref:Roslyn.Services.dll
//O2Ref:Roslyn.Compilers.dll
//O2Ref:Roslyn.Compilers.CSharp.dll

//O2Ref:NGit.dll
//O2Ref:NSch.dll
//O2Ref:Mono.Security.dll 
//O2Ref:Sharpen.dll 
namespace O2.XRules.Database.APIs
{
	public class Installer_SosNet_Test
	{
		public void test()
		{ 
			new Installer_SosNet().start();
		}
	}
	
	public class Installer_SosNet : Tool_API 
	{	
		public Installer_SosNet() : this(true)
		{			
		}
		
		public Installer_SosNet(bool install)
		{
			if (install)
				download_Update_and_Install();
		}
		
		public void download_Update_and_Install()
		{		
			//ensure this dll is loaded in memory			
			"FluentSharp.NGit.dll".assembly().Location.info();
			
			//use the O2 Fork:
			config("SosNet", 
				   "https://github.com/o2platform/O2_Fork_SoS_Net.git".uri(),
				   "Sos.Net.exe");
			var codeDir = this.Install_Dir.pathCombine("O2_Fork_SoS_Net");	   			
			
			if (codeDir.dirExists().isFalse())
			{				
				this.Install_Uri.git_Clone(codeDir);	   
			}
			else
			{
				codeDir.git_Pull();
				var solutionFile = codeDir.pathCombine("SOS.Net.sln");				
				
				solutionFile.build().waitForBuildCompletion().ConsoleOut.str().info();
				Files.copyFilesFromDirectoryToDirectory(codeDir.pathCombine(@"SOS.Net\bin\Debug"), this.Install_Dir);
								
				//show.info(assemblies);
			}			
			/* 
			//Previously we use the original version from bitbucket
			config("SosNet", 
				   "https://bitbucket.org/grozeille/sosnet/downloads/SOS.Net.zip".uri(),
				   "Sos.Net.exe");
			installFromZip_Web();
			*/
			//get PssCor4.dll and sosex.dll windb extensions
			
			var x86_Folder = this.Install_Dir.pathCombine("x86").createDir();
			var x64_Folder = this.Install_Dir.pathCombine("x64").createDir();

			//Psscor4
			var psscorDir = this.Install_Dir.pathCombine("Psscor4").createDir();
			var pssCor4Zip = Install_Dir.pathCombine("Psscor4.zip");
			if (psscorDir.fileExists().isFalse())
			{
				if (pssCor4Zip.fileExists().isFalse())
				{
					var pssCor4Exe =  "http://download.microsoft.com/download/2/C/E/2CE778CF-3A42-48FE-9EFF-4C76B4ECB147/Psscor4.EXE".download();
					
					pssCor4Exe.startProcess("/Q /T:\"{0}\"".format(Install_Dir))
							  .WaitForExit();
				}	
				var pssCor4_x86 = psscorDir.pathCombine(@"x86\x86\psscor4.dll");
				var pssCor4_x64 = psscorDir.pathCombine(@"amd64\amd64\psscor4.dll");
				if (pssCor4_x86.fileExists().isFalse())
				{
					if (pssCor4Zip.fileExists())
					{
						pssCor4Zip.unzip(Install_Dir);							   
						Files.copy(pssCor4_x86,x86_Folder);				
						Files.copy(pssCor4_x86.replace(".dll",".pdb"),x86_Folder);
						Files.copy(pssCor4_x64,x64_Folder);				
						Files.copy(pssCor4_x64.replace(".dll",".pdb"),x64_Folder);				
					}
					else
						"[psscor4 install], could not find psscor4  zip file: {0}".error(pssCor4Zip);
				}				
			}
			
			//sosex
			var sosEx = x86_Folder.pathCombine("sosex.dll");
			if (sosEx.fileExists().isFalse())
			{				
				var sos32_zip = "http://www.stevestechspot.com/downloads/sosex_32.zip".download();
				sos32_zip.unzip(x86_Folder);
				
				
				var sos64_zip = "http://www.stevestechspot.com/downloads/sosex_64.zip".download();
				sos64_zip.unzip(x64_Folder);
			}
			//this.showInfo();
			/*Action<string, string,Uri> install = (installFolder, installFile, installUri)=>
				{					
					config("DebugAnalyzer", "DebugAnalyzer v1.2.5", installFile);			
		    		this.Install_Uri = installUri;
		    		this.Executable = this.Install_Dir.pathCombine(installFolder + @"\DebugAnalyzer.exe");
		    		installFromZip_Web();
		    	};		  
		    			    
		    install	("x86", "DAx86.zip", "http://www.debuganalyzer.net/file.axd?file=DAx86.zip".uri());
		    install	("x64", "DAx64.zip", "http://www.debuganalyzer.net/file.axd?file=DAx64.zip".uri());
    		//install 2.0 version    		
    		//install("Acorns.Hawkeye.125.N2", "Acorns.Hawkeye.125.N2.zip", "http://download.codeplex.com/Download/Release?ProjectName=hawkeye&DownloadId=196206&FileTime=129391670111230000&Build=18924".uri());
    		//install 4.0 version
    		//install("Acorns.Hawkeye.125.N4", "Acorns.Hawkeye.125.N4.zip", "http://download.codeplex.com/Download/Release?ProjectName=hawkeye&DownloadId=196207&FileTime=129391675391630000&Build=18924".uri());    		
    		*/
		}
		
		//we can't use Roslyn in the main since it doesn't support the embedding of resources
		public Installer_SosNet compileUsingRoslyn()
		{
			var codeDir = this.Install_Dir.pathCombine("O2_Fork_SoS_Net");	   			
			var solutionFile = codeDir.pathCombine("SOS.Net.sln");		
			var roslynAssemblies = this.Install_Dir.pathCombine("_Roslyn_Assemblies");
			var assemblies = solutionFile.compileSolution(createUniqueAssemblies : false);
			foreach(var assembly in assemblies)
			{				
				//new System.Reflection.Assembly().GetName
				var assemblyName = "{0}.{1}".format(assembly.GetName().Name, assembly.EntryPoint.isNull() ? "dll" : "exe");
				var assemblyLocation = roslynAssemblies.pathCombine(assemblyName);
				Files.copy(assembly.Location, roslynAssemblies).info();				
			}
			return this;
		}
		
				
		public Process start()
		{
			if (isInstalled())
				return this.Executable.startProcess();			
			return null;
		}		
	}
}