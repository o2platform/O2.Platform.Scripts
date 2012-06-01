// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using O2.Kernel;
using O2.Kernel.ExtensionMethods;
using O2.DotNetWrappers.Windows;
//O2File:API_IKVM.cs

namespace O2.XRules.Database.APIs.IKVM
{
    public class JavaCompile 
    {
		public API_IKVM ikvm;
		
        public JavaCompile(API_IKVM _ikvm)
        {
        	ikvm = _ikvm;
            ikvm.checkIfJavaPathIsCorrectlySet();
        }

        public string compileJavaFile(string fileToCompile)
        {
            var expectedClassFile = fileToCompile.Replace(".java", ".class");
            if (Files.deleteFile(expectedClassFile))
            {
                var javaCompilerArgumentsFormat = "-classpath \"{0}\\*\" \"{1}\"";                
                var compilationArguments = string.Format(javaCompilerArgumentsFormat, ikvm.jarStubsCacheDir,
                                                         fileToCompile); //IKVMConfig.javaCompilerArguments
                var processExecResult = Processes.startProcessAsConsoleApplicationAndReturnConsoleOutput(ikvm.javaCompilerExecutable,compilationArguments);
                "Compilation result: {0}".info(processExecResult);
                if (File.Exists(expectedClassFile))
                    return expectedClassFile;
            }
            return "";
        }

        public string createJarStubForDotNetDll(string dllToConvert, string targetDirectory)
        {
            if (ikvm.checkIKVMInstallation())
            {
                var processExecutionArguments = string.Format("\"{0}\"", dllToConvert);
                var processExecResult =
                    Processes.startProcessAsConsoleApplicationAndReturnConsoleOutput(ikvm.IKVMStubExecutable,
                                                                                     processExecutionArguments,targetDirectory,  true);
                processExecResult.info();


                /*var createdJarFile =
                    Path.Combine(Path.GetDirectoryName(dllToConvert), Path.GetFileNameWithoutExtension(dllToConvert)) +
                    ".jar";
                */
                var createdJarFile = Path.Combine(targetDirectory, Path.GetFileNameWithoutExtension(dllToConvert)) +".jar";
                if (File.Exists(createdJarFile))
                {
                    //var jarFileInTargetDirectory = Files.Copy(createdJarFile, targetDirectory);
                    "Created Jar file: {0}".info(createdJarFile);
                    return (createdJarFile);
                }
                "Was not able to create Jar file for dll: {0}".info( dllToConvert);
            }
            return "";
        }
    }
}
