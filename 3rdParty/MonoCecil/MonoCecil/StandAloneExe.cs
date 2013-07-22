// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)

using System.IO;
using System.Reflection;
using FluentSharp.CoreLib.API;
using Mono.Cecil;

//O2File:CecilUtils.cs
//O2File:CecilAssemblyDependencies.cs

namespace O2.External.O2Mono.MonoCecil
{
    public class StandAloneExe
    {        
        //public StandAloneExe(MethodInfo targetMethodInfo)
        //{
//
 //       }        
        public static string createMainPointingToMethodInfo(MethodInfo targetMethodInfo)
        {
            var exeName = targetMethodInfo.Name;

            var cecilAssemblyBuilder = new CecilAssemblyBuilder(exeName, ModuleKind.Console);
            TypeDefinition tdType = cecilAssemblyBuilder.addType("O2StandAloneExe", "Program");
            MethodDefinition mdMain = cecilAssemblyBuilder.addMainMethod(tdType);
            cecilAssemblyBuilder.codeBlock_CallToMethod(mdMain, targetMethodInfo);
            var exeFileCreated = cecilAssemblyBuilder.Save(PublicDI.config.O2TempDir);
            
            PublicDI.log.info("Exe file created: {0}", exeFileCreated);
            return exeFileCreated;
        }

        public static string createPackagedDirectoryForMethod(MethodInfo targetMethod)
        {
            return createPackagedDirectoryForMethod(targetMethod, "");
        }

        public static string createPackagedDirectoryForMethod(MethodInfo targetMethod, string loadDllsFrom)
        {
            var standAloneExe = createMainPointingToMethodInfo(targetMethod);

            if (File.Exists(standAloneExe))
            {
                var createdDirectory = CecilAssemblyDependencies.populateDirectoryWithAllDependenciesOfAssembly(standAloneExe, loadDllsFrom);
                if (Directory.Exists(createdDirectory))
                {
                    var exeInCreatedDirectory = Path.Combine(createdDirectory, Path.GetFileName(standAloneExe));
                    if (File.Exists(exeInCreatedDirectory))
                    {
                        var newDirectoryName = PublicDI.config.getTempFolderInTempDirectory(targetMethod.Name);
                        Files.copyFilesFromDirectoryToDirectory(createdDirectory, newDirectoryName);
                        Files.deleteFolder(createdDirectory);

                        var exeInFinalDirectory = Path.Combine(newDirectoryName, Path.GetFileName(standAloneExe));
                        var pdbFile = IL.createPdb(exeInFinalDirectory, false);
                        PublicDI.log.debug("created temp exe for method execution: {0}", exeInFinalDirectory);
                        return exeInFinalDirectory;
                    }
                }
            }
            PublicDI.log.error("in createPackagedDirectoryForMethod: could not create exe for method");
            return "";
        }
    }
}
