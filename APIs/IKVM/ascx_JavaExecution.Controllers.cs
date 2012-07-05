// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using System.Collections.Generic;
using O2.DotNetWrappers.DotNet;
using O2.DotNetWrappers.ExtensionMethods;
using O2.DotNetWrappers.Windows;
using O2.Kernel.CodeUtils;
using O2.Kernel.ExtensionMethods;
using System.IO;
//O2File:ascx_JavaExecution.cs
//O2File:ascx_JavaExecution.Designer.cs


namespace O2.XRules.Database.APIs.IKVM
{
    public partial class ascx_JavaExecution
    {
        public bool runOnLoad = true;
        public API_IKVM ikvm;

        private void onLoad()
        {
            if (DesignMode == false && runOnLoad) 
            {
            	ikvm = new API_IKVM();
                loadDefaultSetOfFilesToConvert();
                directoryWithJarStubFiles.openDirectory(ikvm.jarStubsCacheDir);
                directoryToDropJarsToConvertIntoDotNetAssemblies.openDirectory(ikvm.convertedJarsDir);
            }
        }

        public void loadDefaultSetOfFilesToConvert()
        {
            dotNetAssembliesToConvert.clearMappings();
            dotNetAssembliesToConvert.setExtensionsToShow(".dll .exe");
            dotNetAssembliesToConvert.addFiles(CompileEngine.getListOfO2AssembliesInExecutionDir());
            dotNetAssembliesToConvert.addFiles(AppDomainUtils.getDllsInCurrentAppDomain_FullPath());
            runOnLoad = false;
        }

        private void createJarStubFiles()
        {
            O2Thread.mtaThread(
                () =>
                    {
                        // reset progress bar values
                        this.invokeOnThread(() =>
                                                {
                                                    progressBarForJarStubCreation.Maximum = dotNetAssembliesToConvert.loadedFiles.Count;
                                                    progressBarForJarStubCreation.Value = 0;
                                                    btCreateJarStubFiles.Enabled = false;
                                                });
                        // process all files in dotNetAssembliesToConvert
                        foreach (var fileToProcess in dotNetAssembliesToConvert.loadedFiles)
                        {
                            var jarStubFile = new JavaCompile(ikvm).createJarStubForDotNetDll(fileToProcess, ikvm.jarStubsCacheDir);
                            if (!File.Exists(jarStubFile))
                                "Jar stub file not created for :{0}".error(jarStubFile);
                            this.invokeOnThread(() => progressBarForJarStubCreation.Value++);
                        }
                        deleteEmptyJarStubs();
                        this.invokeOnThread(() => btCreateJarStubFiles.Enabled = true);                                                
                    });            
        }

        private void deleteJarStubs()
        {
            Files_WinForms.deleteFiles(directoryWithJarStubFiles.getFiles(),true);            
        }

        private void deleteEmptyJarStubs()
        {
            var filesToDelete = new List<string>();
            foreach (var file in directoryWithJarStubFiles.getFiles())
            {
                var fileSize = Files_WinForms.getFileSize(file);
                if (fileSize == 0)
                    filesToDelete.Add(file);
            }
            Files_WinForms.deleteFiles(filesToDelete, true);
        }


        private void convertJarToDotNetAssembly(string droppedFileOrFolder, string targetDirectory)
        {
            if (File.Exists(droppedFileOrFolder))
                O2Thread.mtaThread(
                    () =>
                        {
                            directoryToDropJarsToConvertIntoDotNetAssemblies.setDisableMove(false);
                            ikvm.convertJarFileIntoDotNetAssembly(droppedFileOrFolder, targetDirectory);
                            directoryToDropJarsToConvertIntoDotNetAssemblies.setDisableMove(true);
                        });
            else
                "in convertJarToDotNetAssembly, only files supported".error();
        }
    }
}
