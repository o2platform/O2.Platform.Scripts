// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.IO;
using FluentSharp.CoreLib;
using FluentSharp.CoreLib.API;

//using O2.XRules.Database.Languages_and_Frameworks.DotNet;

//Installer:IKVM_Installer.cs!IKVM\ikvm-7.1.4532.2\bin\ikvm.exe

//_O2File:DotNet_SDK_GacUtil.cs  

namespace O2.XRules.Database.APIs.IKVM
{
    public class API_IKVM	
    {
        /*static IKVMConfig()
        {
            IKVMInstall.checkIKVMInstallation();
        }*/

        public string _IKVMRuntimeDir { get; set; }        
        public string IKVMExecutable { get; set; }
        public string IKVMCompilerExecutable { get; set; }
        public string IKVMCompilerArgumentsFormat { get; set; }
        public string IKVMStubExecutable { get; set; }
        public string IKVMExecution_Script { get; set; }
        public string jarStubsCacheDir { get; set; }
        public string convertedJarsDir { get; set; }
        public string javaCompilerExecutable { get; set; }
        public string javaParserExecutable { get; set; }
        
        public API_IKVM()
        {        	
        
        	_IKVMRuntimeDir = PublicDI.config.ToolsOrApis.pathCombine(@"\IKVM\ikvm-7.1.4532.2\bin");        	
        	//"_IKVM_Runtime.zip";
        	 
        	IKVMExecutable = Path.Combine(_IKVMRuntimeDir, "ikvm.exe");
        	IKVMCompilerExecutable = Path.Combine(_IKVMRuntimeDir, "ikvmc.exe");
        	IKVMCompilerArgumentsFormat = "\"{0}\" -out:\"{1}\"";
        	IKVMStubExecutable = Path.Combine(_IKVMRuntimeDir, "ikvmstub.exe");
        	IKVMExecution_Script = "-classpath \"{0};\" {1} {2}";
        	jarStubsCacheDir = Path.Combine(_IKVMRuntimeDir, "jarStubsCacheDir");
        	convertedJarsDir = Path.Combine(_IKVMRuntimeDir, "convertedJarsDir");        
        	javaCompilerExecutable ="";
        	javaParserExecutable = "";
        	this.checkIKVMInstallation();
        }
    
    }
    
    public static class API_IKVM_ExtensionMethods_Install
    {
    	public static bool checkIKVMInstallation(this API_IKVM ikvm)
        {
            Files.checkIfDirectoryExistsAndCreateIfNot(ikvm.jarStubsCacheDir);
            if (ikvm.checkIfJavaPathIsCorrectlySet())
            {
                if (Directory.Exists(ikvm._IKVMRuntimeDir) && File.Exists(ikvm.IKVMCompilerExecutable) && File.Exists(ikvm.IKVMStubExecutable))
                    return true;                
            }
            return false;
        }
      
        
/*        public static API_IKVM install_IKVM_Assemblies_on_GAC(this API_IKVM ikvm)
        {
        	"Installing IKVM dlls in local GAC folder".info();
        	var gacUtil =  new DotNet_SDK_GacUtil();  
        	foreach(var file in ikvm._IKVMRuntimeDir.files("ikvm*.*"))
        		if (file.fileName().neq("ikvm-native.dll") && gacUtil.install(file).isFalse())              		
        		{
        			"Failed to install into GAC, so stopping installation process".error();
        			break;
        		}
			return ikvm;
        }
*/
        public static bool checkIfJavaPathIsCorrectlySet(this API_IKVM ikvm)
        {
            var javaHome = Environment.GetEnvironmentVariable("Java_Home");
            if (string.IsNullOrEmpty(javaHome))
                "in checkIfJavaPathIsCorrectlySet, could not find Java_Home variable".error();
            else
            {
                var javaCompiler = Path.Combine(javaHome, @"bin\javac.exe");
                if (!File.Exists(javaCompiler))
                    "in checkIfJavaPathIsCorrectlySet, could not find javaCompiler executable: {0}".error(javaCompiler);
                {
                    
                    var javaParser = Path.Combine(javaHome, @"bin\javap.exe");
                    if (!File.Exists(javaParser))
                        "in checkIfJavaPathIsCorrectlySet, could not find javaParser executable: {0}".error(javaParser);
                    else
                    {
                        ikvm.javaCompilerExecutable = javaCompiler;
                        ikvm.javaParserExecutable = javaParser;

                        return true;
                    }
                }
            }            
            return false;
        }

        public static void uninstallIKVM(this API_IKVM ikvm)
        {
            if (Directory.Exists(ikvm._IKVMRuntimeDir))
                Files.deleteFolder(ikvm._IKVMRuntimeDir, true);
        }
    }
    
	public static class API_IKVM_ExtensionMethods_Utils
    {
            
        public static string convertJarFileIntoDotNetAssembly(this API_IKVM ikvm, string pathToJarFile, string targetDirectory)
        {
            if (File.Exists(pathToJarFile) && Path.GetExtension(pathToJarFile) == ".jar")
            {
                var destinationFile = Path.Combine(targetDirectory,
                                                   Path.GetFileNameWithoutExtension(pathToJarFile) + ".dll");
                Files.deleteFile(destinationFile);
                var executionParameters = string.Format(ikvm.IKVMCompilerArgumentsFormat, pathToJarFile,
                                                        destinationFile);
                var executionResult =
                    Processes.startProcessAsConsoleApplicationAndReturnConsoleOutput(ikvm.IKVMCompilerExecutable,
                                                                                     executionParameters);
                if (File.Exists(destinationFile))
                    return destinationFile;
                
                "in IKVMUtils.convertJarToDotNetAssembly, Jar file was not Converted into .Net Dll".error();
            }
            else
                "in IKVMUtils.convertJarToDotNetAssembly, only jar files are supported".error();
            return "";
        }
    }
}
